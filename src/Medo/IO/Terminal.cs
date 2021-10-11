/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

namespace Medo.IO {
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    /// <summary>
    /// Basic terminal operations.
    /// </summary>
    public sealed class Terminal {

        /// <summary>
        /// Creates a new instance that writes data to output stream.
        /// Use Terminal.AnsiConsole or Terminal.Console for direct console use.
        /// </summary>
        /// <param name="outputStream">Output stream.</param>
        /// <exception cref="ArgumentNullException">Output stream cannot be null. -or- Error stream cannot be null.</exception>
        public Terminal(Stream outputStream)
            : this(outputStream, outputStream) {
        }


        /// <summary>
        /// Creates a new instance that writes data to output stream.
        /// Use Terminal.AnsiConsole or Terminal.Console for direct console use.
        /// </summary>
        /// <param name="outputStream">Output stream.</param>
        /// <param name="errorStream">Error stream.</param>
        /// <exception cref="ArgumentNullException">Output stream cannot be null. -or- Error stream cannot be null.</exception>
        public Terminal(Stream outputStream, Stream errorStream) {
            if (outputStream is null) { throw new ArgumentNullException(nameof(outputStream), "Output stream cannot be null."); }
            if (errorStream is null) { throw new ArgumentNullException(nameof(errorStream), "Error stream cannot be null."); }
            OutputStream = outputStream;
            ErrorStream = errorStream;
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        private Terminal() {
        }

        private Stream? OutputStream { get; init; }
        private Stream? ErrorStream { get; init; }
        private bool UsingErrorStream;

        private readonly object SyncOutput = new();


        private bool _suppressAttributes;
        /// <summary>
        /// Gets/sets if color (and other ANSI sequences) are to be supressed.
        /// </summary>
        public bool SuppressAttributes {
            get { lock (SyncOutput) { return _suppressAttributes; } }
            set { lock (SyncOutput) { _suppressAttributes = value; } }
        }

        /// <summary>
        /// All write operations will use standard output stream.
        /// </summary>
        public void UseStandardStream() {
            lock (SyncOutput) { UsingErrorStream = false; }
        }

        /// <summary>
        /// All write operations will use error output stream.
        /// </summary>
        public void UseErrorStream() {
            lock (SyncOutput) { UsingErrorStream = true; }
        }


        /// <summary>
        /// Clears the screen.
        /// </summary>
        public Terminal Clear() {
            lock (SyncOutput) {
                if (SuppressAttributes) { return this; }
                var stream = UsingErrorStream ? ErrorStream : OutputStream;
                if (stream is not null) {
                    ComposingBytes[1] = 0x5B;  // [
                    ComposingBytes[2] = 0x32;  // 2
                    ComposingBytes[3] = 0x4A;  // J
                    ComposingBytes[4] = 0x1B;  // Esc
                    ComposingBytes[5] = 0x5B;  // [
                    ComposingBytes[6] = 0x48;  // H
                    stream.Write(ComposingBytes, 0, 7);
                } else {  // Fallback
                    System.Console.Clear();
                }
            }
            return this;  // just to allow for nicer calls
        }

        /// <summary>
        /// Resets color and other attributes.
        /// </summary>
        public Terminal Reset() {
            lock (SyncOutput) {
                if (SuppressAttributes) { return this; }
                var stream = UsingErrorStream ? ErrorStream : OutputStream;
                if (stream is not null) {
                    ComposingBytes[1] = 0x5B;  // [
                    ComposingBytes[2] = 0x30;  // 0
                    ComposingBytes[3] = 0x6D;  // m
                    stream.Write(ComposingBytes, 0, 4);
                } else {  // FallBack: reset colors and variables
                    System.Console.ResetColor();
                    FallbackIsBold = false;
                    FallbackIsInverted = false;
                }
            }
            return this;  // just to allow for nicer calls
        }


        /// <summary>
        /// Sets foreground color.
        /// </summary>
        /// <param name="color">Color to use.</param>
        public Terminal Foreground(ConsoleColor color) {
            lock (SyncOutput) {
                if (SuppressAttributes) { return this; }
                var stream = UsingErrorStream ? ErrorStream : OutputStream;
                if (stream is not null) {
                    var (colorByte, isColorBright) = GetColorByte(color);
                    ComposingBytes[1] = 0x5B;  // [
                    ComposingBytes[2] = isColorBright ? (byte)0x39 : (byte)0x33;  // 3 or 9
                    ComposingBytes[3] = colorByte;
                    ComposingBytes[4] = 0x6D;  // m
                    stream.Write(ComposingBytes, 0, 5);
                } else {  // Fallback: just use color directly
                    if (UsingErrorStream) { return this; }  // no color for error stream
                    System.Console.ForegroundColor = color;
                }
            }
            return this;  // just to allow for nicer calls
        }

        /// <summary>
        /// Resets foreground color.
        /// </summary>
        public Terminal ResetForeground() {
            lock (SyncOutput) {
                if (SuppressAttributes) { return this; }
                var stream = UsingErrorStream ? ErrorStream : OutputStream;
                if (stream is not null) {
                    ComposingBytes[1] = 0x5B;  // [
                    ComposingBytes[2] = 0x33;  // 3
                    ComposingBytes[3] = 0x39;  // 9
                    ComposingBytes[4] = 0x6D;  // m
                    stream.Write(ComposingBytes, 0, 5);
                } else {  // Fallback: just use color directly
                    if (UsingErrorStream) { return this; }  // no color for error stream
                    var backColor = System.Console.BackgroundColor;
                    System.Console.ResetColor();
                    System.Console.BackgroundColor = backColor;
                }
            }
            return this;  // just to allow for nicer calls
        }

        /// <summary>
        /// Sets background color.
        /// </summary>
        /// <param name="color">Color to use.</param>
        public Terminal Background(ConsoleColor color) {
            lock (SyncOutput) {
                if (SuppressAttributes) { return this; }
                var stream = UsingErrorStream ? ErrorStream : OutputStream;
                if (stream is not null) {
                    var (colorByte, isColorBright) = GetColorByte(color);
                    ComposingBytes[1] = 0x5B;  // [
                    if (!isColorBright) {
                        ComposingBytes[2] = 0x34;  // 4
                        ComposingBytes[3] = colorByte;
                        ComposingBytes[4] = 0x6D;  // m
                        stream.Write(ComposingBytes, 0, 5);
                    } else {
                        ComposingBytes[2] = 0x31;  // 1
                        ComposingBytes[3] = 0x30;  // 0
                        ComposingBytes[4] = colorByte;
                        ComposingBytes[5] = 0x6D;  // m
                        stream.Write(ComposingBytes, 0, 6);
                    }
                } else {  // Fallback: just use color directly
                    if (UsingErrorStream) { return this; }  // no color for error stream
                    System.Console.BackgroundColor = color;
                }
            }
            return this;  // just to allow for nicer calls
        }

        /// <summary>
        /// Resets background color.
        /// </summary>
        public Terminal ResetBackground() {
            lock (SyncOutput) {
                if (SuppressAttributes) { return this; }
                var stream = UsingErrorStream ? ErrorStream : OutputStream;
                if (stream is not null) {
                    ComposingBytes[1] = 0x5B;  // [
                    ComposingBytes[2] = 0x34;  // 4
                    ComposingBytes[3] = 0x39;  // 9
                    ComposingBytes[4] = 0x6D;  // m
                    stream.Write(ComposingBytes, 0, 5);
                } else {  // Fallback: just use color directly
                    if (UsingErrorStream) { return this; }  // no color for error stream
                    var foreColor = System.Console.ForegroundColor;
                    System.Console.ResetColor();
                    System.Console.ForegroundColor = foreColor;
                }
            }
            return this;  // just to allow for nicer calls
        }


        /// <summary>
        /// Sets bold attribute.
        /// </summary>
        public Terminal Bold() {
            return SetBold(newState: true);
        }

        /// <summary>
        /// Resets bold attribute.
        /// </summary>
        public Terminal ResetBold() {
            return SetBold(newState: false);
        }

        private Terminal SetBold(bool newState) {
            lock (SyncOutput) {
                if (SuppressAttributes) { return this; }
                var stream = UsingErrorStream ? ErrorStream : OutputStream;
                if (stream is not null) {
                    if (newState) {
                        ComposingBytes[1] = 0x5B;  // [
                        ComposingBytes[2] = 0x31;  // 1
                        ComposingBytes[3] = 0x6D;  // m
                        stream.Write(ComposingBytes, 0, 4);
                    } else {
                        ComposingBytes[1] = 0x5B;  // [
                        ComposingBytes[2] = 0x32;  // 2
                        ComposingBytes[3] = 0x32;  // 2
                        ComposingBytes[4] = 0x6D;  // m
                        stream.Write(ComposingBytes, 0, 5);
                    }
                } else {  // Fallback: let's try to use brightness
                    if (newState) {
                        var colorValue = (int)System.Console.ForegroundColor;
                        if (colorValue <= 7) {  // just do it for dark colors
                            FallbackIsBold = true;
                            System.Console.ForegroundColor = (ConsoleColor)(colorValue + 8);
                        }
                    } else {
                        var colorValue = (int)System.Console.ForegroundColor;
                        if (FallbackIsBold && (colorValue > 7)) {  // just do it for bright colors
                            System.Console.ForegroundColor = (ConsoleColor)(colorValue - 8);
                        }
                        FallbackIsBold = false;  // cancel bold either way
                    }
                }
            }
            return this;  // just to allow for nicer calls
        }


        /// <summary>
        /// Sets underline attribute.
        /// No fallback for Console.
        /// </summary>
        public Terminal Underline() {
            return SetUnderline(newState: true);
        }

        /// <summary>
        /// Resets underline.
        /// </summary>
        public Terminal ResetUnderline() {
            return SetUnderline(newState: false);
        }

        private Terminal SetUnderline(bool newState) {
            lock (SyncOutput) {
                if (SuppressAttributes) { return this; }
                var stream = UsingErrorStream ? ErrorStream : OutputStream;
                if (stream is not null) {
                    if (newState) {
                        ComposingBytes[1] = 0x5B;  // [
                        ComposingBytes[2] = 0x34;  // 4
                        ComposingBytes[3] = 0x6D;  // m
                        stream.Write(ComposingBytes, 0, 4);
                    } else {
                        ComposingBytes[1] = 0x5B;  // [
                        ComposingBytes[2] = 0x32;  // 2
                        ComposingBytes[3] = 0x34;  // 4
                        ComposingBytes[4] = 0x6D;  // m
                        stream.Write(ComposingBytes, 0, 5);
                    }
                } else {  // Fallback: none
                }
            }
            return this;  // just to allow for nicer calls
        }


        /// <summary>
        /// Sets color invert attribute.
        /// </summary>
        public Terminal Invert() {
            return SetInvert(newState: true);
        }

        /// <summary>
        /// Resets color invert attribute.
        /// </summary>
        public Terminal ResetInvert() {
            return SetInvert(newState: false);
        }

        private Terminal SetInvert(bool newState) {
            lock (SyncOutput) {
                if (SuppressAttributes) { return this; }
                var stream = UsingErrorStream ? ErrorStream : OutputStream;
                if (stream is not null) {
                    if (newState) {
                        ComposingBytes[1] = 0x5B;  // [
                        ComposingBytes[2] = 0x37;  // 7
                        ComposingBytes[3] = 0x6D;  // m
                        stream.Write(ComposingBytes, 0, 4);
                    } else {
                        ComposingBytes[1] = 0x5B;  // [
                        ComposingBytes[2] = 0x32;  // 2
                        ComposingBytes[3] = 0x37;  // 7
                        ComposingBytes[4] = 0x6D;  // m
                        stream.Write(ComposingBytes, 0, 5);
                    }
                } else {  // Fallback
                    if (newState) {
                        if (!FallbackIsInverted) {
                            var foreColor = System.Console.ForegroundColor;
                            System.Console.ForegroundColor = System.Console.BackgroundColor;
                            System.Console.BackgroundColor = foreColor;
                            FallbackIsInverted = true;
                        }
                    } else {
                        if (FallbackIsInverted) {
                            var foreColor = System.Console.ForegroundColor;
                            System.Console.ForegroundColor = System.Console.BackgroundColor;
                            System.Console.BackgroundColor = foreColor;
                            FallbackIsInverted = false;
                        }
                    }
                }
            }
            return this;  // just to allow for nicer calls
        }


        /// <summary>
        /// Moves cursor left.
        /// </summary>
        public Terminal MoveLeft() {
            return MoveLeft(amount: 1);
        }

        /// <summary>
        /// Moves cursor left.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Amount must be a positive number less than 65536.</exception>
        public Terminal MoveLeft(int amount) {
            if ((amount < 1) || (amount > 65535)) { throw new ArgumentNullException(nameof(amount), "Amount must be a positive number less than 65536."); }
            return MoveHorizontally(-amount);
        }

        /// <summary>
        /// Moves cursor right.
        /// </summary>
        public Terminal MoveRight() {
            return MoveRight(amount: 1);
        }

        /// <summary>
        /// Moves cursor right.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Amount must be a positive number less than 65536.</exception>
        public Terminal MoveRight(int amount) {
            if ((amount < 1) || (amount > 65535)) { throw new ArgumentNullException(nameof(amount), "Amount must be a positive number less than 65536."); }
            return MoveHorizontally(amount);
        }

        private Terminal MoveHorizontally(int amount) {
            if (amount == 0) { return this; }
            lock (SyncOutput) {
                if (SuppressAttributes) { return this; }
                var stream = UsingErrorStream ? ErrorStream : OutputStream;
                if (stream is not null) {
                    ComposingBytes[1] = 0x5B;  // [
                    int nextIndex;
                    if (amount > 0) {  // move forward
                        nextIndex = InsertNumber((ushort)amount, ComposingBytes, 2);
                        ComposingBytes[nextIndex] = 0x43;  // C
                    } else {  // move backward
                        nextIndex = InsertNumber((ushort)(-amount), ComposingBytes, 2);
                        ComposingBytes[nextIndex] = 0x44;  // D
                    }
                    stream.Write(ComposingBytes, 0, nextIndex + 1);
                } else {  // FallBack: use CursorLeft
                    if (amount > 0) {  // move forward
                        if (System.Console.CursorLeft + amount >= System.Console.BufferWidth) {
                            System.Console.CursorLeft = System.Console.BufferWidth - 1;
                        } else {
                            System.Console.CursorLeft += amount;
                        }
                    } else {  // move backward
                        if (System.Console.CursorLeft + amount < 0) {
                            System.Console.CursorLeft = 0;
                        } else {
                            System.Console.CursorLeft += amount;
                        }
                    }
                }
            }
            return this;  // just to allow for nicer calls
        }


        /// <summary>
        /// Moves cursor up.
        /// </summary>
        public Terminal MoveUp() {
            return MoveUp(amount: 1);
        }

        /// <summary>
        /// Moves cursor up.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Amount must be a positive number less than 65536.</exception>
        public Terminal MoveUp(int amount) {
            if ((amount < 1) || (amount > 65535)) { throw new ArgumentNullException(nameof(amount), "Amount must be a positive number less than 65536."); }
            return MoveVertically(-amount);
        }

        /// <summary>
        /// Moves cursor down.
        /// </summary>
        public Terminal MoveDown() {
            return MoveVertically(amount: 1);
        }

        /// <summary>
        /// Moves cursor down.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Amount must be a positive number less than 65536.</exception>
        public Terminal MoveDown(int amount) {
            if ((amount < 1) || (amount > 65535)) { throw new ArgumentNullException(nameof(amount), "Amount must be a positive number less than 65536."); }
            return MoveVertically(amount);
        }

        private Terminal MoveVertically(int amount) {
            if (amount == 0) { return this; }
            lock (SyncOutput) {
                if (SuppressAttributes) { return this; }
                var stream = UsingErrorStream ? ErrorStream : OutputStream;
                if (stream is not null) {
                    ComposingBytes[1] = 0x5B;  // [
                    int nextIndex;
                    if (amount > 0) {  // move forward
                        nextIndex = InsertNumber((ushort)amount, ComposingBytes, 2);
                        ComposingBytes[nextIndex] = 0x42;  // B
                    } else {  // move backward
                        nextIndex = InsertNumber((ushort)(-amount), ComposingBytes, 2);
                        ComposingBytes[nextIndex] = 0x41;  // A
                    }
                    stream.Write(ComposingBytes, 0, nextIndex + 1);
                } else {  // FallBack: use CursorTop
                    if (amount > 0) {  // move forward
                        if (System.Console.CursorTop + amount >= System.Console.BufferHeight) {
                            System.Console.CursorTop = System.Console.BufferHeight - 1;
                        } else {
                            System.Console.CursorTop += amount;
                        }
                    } else {  // move backward
                        if (System.Console.CursorTop + amount < 0) {
                            System.Console.CursorTop = 0;
                        } else {
                            System.Console.CursorTop += amount;
                        }
                    }
                }
            }
            return this;  // just to allow for nicer calls
        }

        /// <summary>
        /// Moves to specified coordinates.
        /// </summary>
        /// <param name="x">X coordinate; 0 ignores the parameter.</param>
        /// <param name="y">Y coordinate; 0 ignores the parameter.</param>
        /// <exception cref="ArgumentOutOfRangeException">X must be either 0 or between 1 and 65535. -or- Y must be either 0 or between 1 and 65535.</exception>
        public Terminal MoveTo(int x, int y) {
            if ((x < 0) || (x > 65535)) { throw new ArgumentOutOfRangeException(nameof(x), "X must be either 0 or between 1 and 65535."); }
            if ((y < 0) || (y > 65535)) { throw new ArgumentOutOfRangeException(nameof(y), "Y must be either 0 or between 1 and 65535."); }

            var moveHor = (x > 0);
            var moveVer = (y > 0);
            if (!moveHor && !moveVer) { return this; }

            lock (SyncOutput) {
                if (SuppressAttributes) { return this; }
                var stream = UsingErrorStream ? ErrorStream : OutputStream;
                if (stream is not null) {
                    ComposingBytes[1] = 0x5B;  // [
                    if (moveHor && moveVer) {  // move both horizontally and vertically
                        var nextIndex = InsertNumber((ushort)y, ComposingBytes, 2);
                        ComposingBytes[nextIndex++] = 0x3B;  // ;
                        nextIndex = InsertNumber((ushort)x, ComposingBytes, nextIndex);
                        ComposingBytes[nextIndex] = 0x48;  // H
                        stream.Write(ComposingBytes, 0, nextIndex + 1);
                    } else if (moveHor) {  // move just horizontally
                        var nextIndex = InsertNumber((ushort)x, ComposingBytes, 2);
                        ComposingBytes[nextIndex] = 0x47;  // G
                        stream.Write(ComposingBytes, 0, nextIndex + 1);
                    } else if (moveVer) {  // move just vertically
                        var nextIndex = InsertNumber((ushort)y, ComposingBytes, 2);
                        ComposingBytes[nextIndex] = 0x64;  // d
                        stream.Write(ComposingBytes, 0, nextIndex + 1);
                    }
                } else {  // FallBack: use CursorLeft and CursorTop
                    if (moveHor) {
                        if (x > System.Console.BufferWidth) {
                            System.Console.CursorLeft = System.Console.BufferWidth - 1;
                        } else {
                            System.Console.CursorLeft = x - 1;
                        }
                    }
                    if (moveVer) {
                        if (y > System.Console.BufferHeight) {
                            System.Console.CursorTop = System.Console.BufferHeight - 1;
                        } else {
                            System.Console.CursorTop = y - 1;
                        }
                    }
                }
            }
            return this;  // just to allow for nicer calls
        }


        public Terminal StoreCursor() {
            lock (SyncOutput) {
                if (SuppressAttributes) { return this; }
                var stream = UsingErrorStream ? ErrorStream : OutputStream;
                if (stream is not null) {
                    ComposingBytes[1] = 0x5B;  // [
                    ComposingBytes[2] = 0x73;  // s
                    stream.Write(ComposingBytes, 0, 3);
                } else {  // FallBack: save coordinates
                    FallbackLeft = System.Console.CursorLeft;
                    FallbackTop = System.Console.CursorTop;
                }
            }
            return this;  // just to allow for nicer calls
        }

        public Terminal RestoreCursor() {
            lock (SyncOutput) {
                if (SuppressAttributes) { return this; }
                var stream = UsingErrorStream ? ErrorStream : OutputStream;
                if (stream is not null) {
                    ComposingBytes[1] = 0x5B;  // [
                    ComposingBytes[2] = 0x75;  // u
                    stream.Write(ComposingBytes, 0, 3);
                } else {  // FallBack: use saved coordinates
                    System.Console.CursorLeft = FallbackLeft;
                    System.Console.CursorTop = FallbackTop;
                }
            }
            return this;  // just to allow for nicer calls
        }


        /// <summary>
        /// Writes text.
        /// </summary>
        /// <param name="text">Text to write.</param>
        public Terminal Write(string text) {
            if (string.IsNullOrEmpty(text)) { return this; }  // just ignore
            lock (SyncOutput) {
                var stream = UsingErrorStream ? ErrorStream : OutputStream;
                if (stream is not null) {
                    var bytes = Utf8.GetBytes(text);
                    stream.Write(bytes, 0, bytes.Length);
                } else {  // Fallback: just write
                    System.Console.Write(text);
                }
            }
            return this;  // just to allow for nicer calls
        }

        /// <summary>
        /// Writes text.
        /// Color is reset to default after write is complete.
        /// </summary>
        /// <param name="text">Text to write.</param>
        /// <param name="foregroundColor">Foreground color.</param>
        public Terminal Write(string text, ConsoleColor foregroundColor) {
            lock (SyncOutput) {
                Foreground(foregroundColor);
                Write(text);
                ResetForeground();
            }
            return this;  // just to allow for nicer calls
        }

        /// <summary>
        /// Writes text.
        /// Color is reset to default after write is complete.
        /// </summary>
        /// <param name="text">Text to write.</param>
        /// <param name="foregroundColor">Foreground color.</param>
        /// <param name="backgroundColor">Background color.</param>
        public Terminal Write(string text, ConsoleColor foregroundColor, ConsoleColor backgroundColor) {
            lock (SyncOutput) {
                Foreground(foregroundColor);
                Background(backgroundColor);
                Write(text);
                ResetBackground();
                ResetForeground();
            }
            return this;  // just to allow for nicer calls
        }


        /// <summary>
        /// Writes a new line.
        /// </summary>
        public Terminal WriteLine() {
            Write(Environment.NewLine);
            return this;  // just to allow for nicer calls
        }

        /// <summary>
        /// Writes text and ends it with a new line.
        /// </summary>
        /// <param name="text">Text to write.</param>
        public Terminal WriteLine(string text) {
            Write(text);
            Write(Environment.NewLine);
            return this;  // just to allow for nicer calls
        }

        /// <summary>
        /// Writes text and ends it with a new line.
        /// Color is reset to default after write is complete.
        /// </summary>
        /// <param name="text">Text to write.</param>
        /// <param name="foregroundColor">Foreground color.</param>
        public Terminal WriteLine(string text, ConsoleColor foregroundColor) {
            lock (SyncOutput) {
                Foreground(foregroundColor);
                WriteLine(text);
                ResetForeground();
            }
            return this;  // just to allow for nicer calls
        }

        /// <summary>
        /// Writes text and ends it with a new line.
        /// Color is reset to default after write is complete.
        /// </summary>
        /// <param name="text">Text to write.</param>
        /// <param name="foregroundColor">Foreground color.</param>
        /// <param name="backgroundColor">Background color.</param>
        public Terminal WriteLine(string text, ConsoleColor foregroundColor, ConsoleColor backgroundColor) {
            lock (SyncOutput) {
                Foreground(foregroundColor);
                Background(backgroundColor);
                WriteLine(text);
                ResetBackground();
                ResetForeground();
            }
            return this;  // just to allow for nicer calls
        }


        private static readonly object SyncInput = new();

        /// <summary>
        /// Returns the next character available.
        /// Character is always read from Console.
        /// </summary>
        public static char? ReadCharIfAvailable() {
            lock (SyncInput) {
                return System.Console.KeyAvailable ? System.Console.ReadKey(intercept: true).KeyChar : null;
            }
        }

        /// <summary>
        /// Returns the next character available.
        /// Character is always read from Console.
        /// </summary>
        public static char ReadChar() {
            lock (SyncInput) {
                return System.Console.ReadKey(intercept: true).KeyChar;
            }
        }

        /// <summary>
        /// Returns all currently available keys.
        /// Character is always read from Console.
        /// </summary>
        public static IEnumerable<char> ReadAvailableChars() {
            lock (SyncInput) {
                while (System.Console.KeyAvailable) {
                    yield return System.Console.ReadKey(intercept: true).KeyChar;
                }
            }
        }

        /// <summary>
        /// Returns the next key available.
        /// Character is always read from Console.
        /// </summary>
        public static ConsoleKey ReadKey() {
            lock (SyncInput) {
                return System.Console.ReadKey(intercept: true).Key;
            }
        }

        /// <summary>
        /// Returns the next key available.
        /// Character is always read from Console.
        /// </summary>
        public static ConsoleKey? ReadKeyIfAvailable() {
            lock (SyncInput) {
                return System.Console.KeyAvailable ? System.Console.ReadKey(intercept: true).Key : null;
            }
        }

        /// <summary>
        /// Returns all currently available keys.
        /// Character is always read from Console.
        /// </summary>
        public static IEnumerable<ConsoleKey> ReadAvailableKeys() {
            lock (SyncInput) {
                while (System.Console.KeyAvailable) {
                    yield return System.Console.ReadKey(intercept: true).Key;
                }
            }
        }


        /// <summary>
        /// Gets/sets if Ctrl+C will be treated as a normal input.
        /// </summary>
        public static bool TreatControlCAsInput {
            get { return System.Console.TreatControlCAsInput; }
            set { System.Console.TreatControlCAsInput = value; }
        }


        #region Static

        private static readonly Lazy<Terminal> _ansiConsole = new(() => new Terminal() {
            OutputStream = System.Console.OpenStandardOutput(),
            ErrorStream = System.Console.OpenStandardError(),
        });
        /// <summary>
        /// Returns ANSI terminal based on the system console.
        /// </summary>
        public static Terminal AnsiConsole { get { return _ansiConsole.Value; } }

        private static readonly Lazy<Terminal> _console = new(() => new Terminal() {
            OutputStream = null,
            ErrorStream = null,
        });
        /// <summary>
        /// Returns terminal that uses just the basic Console interface.
        /// </summary>
        public static Terminal Console { get { return _console.Value; } }

        #endregion Static


        #region Privates

        private readonly byte[] ComposingBytes = new byte[] { 0x1B, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };  // used to compose escape sequences, escape char already in
        private readonly Encoding Utf8 = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

        private bool FallbackIsBold;  // remember bold status for console fallback
        private bool FallbackIsInverted;  // remember inversion status for console fallback
        private int FallbackLeft;
        private int FallbackTop;

        private static (byte colorIndex, bool isColorBright) GetColorByte(ConsoleColor color) {
            return color switch {  // convert to ansi byte
                ConsoleColor.Black => (0x30, false),
                ConsoleColor.DarkRed => (0x31, false),
                ConsoleColor.DarkGreen => (0x32, false),
                ConsoleColor.DarkYellow => (0x33, false),
                ConsoleColor.DarkBlue => (0x34, false),
                ConsoleColor.DarkMagenta => (0x35, false),
                ConsoleColor.DarkCyan => (0x36, false),
                ConsoleColor.Gray => (0x37, false),
                ConsoleColor.DarkGray => (0x30, true),
                ConsoleColor.Red => (0x31, true),
                ConsoleColor.Green => (0x32, true),
                ConsoleColor.Yellow => (0x33, true),
                ConsoleColor.Blue => (0x34, true),
                ConsoleColor.Magenta => (0x35, true),
                ConsoleColor.Cyan => (0x36, true),
                ConsoleColor.White => (0x37, true),
                _ => (0x37, false),// should never actually happen unless console colors are expanded
            };
        }

        private static int InsertNumber(ushort number, byte[] bytes, int offset) {
            var numberBytes = new byte[5];
            var numberOffset = 0;
            while (number > 0) {
                numberBytes[numberOffset] = (byte)(number % 10);  // take last digit
                numberOffset += 1;
                number /= 10;
            }
            for (var i = 0; i < numberOffset; i++) {
                bytes[offset + i] = (byte)(0x30 + numberBytes[numberOffset - i - 1]);
            }
            return offset + numberOffset;
        }

        #endregion Privates

    }
}
