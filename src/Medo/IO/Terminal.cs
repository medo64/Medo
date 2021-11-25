/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

//2021-11-25: Refactored to use pattern matching
//2021-11-08: Refactored for .NET 6
//2021-10-10: Initial version

namespace Medo.IO {
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    /// <summary>
    /// Basic terminal operations.
    /// </summary>
    public static class Terminal {

        /// <summary>
        /// Setups ANSI output.
        /// </summary>
        public static void Setup() {
            OutputStream = null;
            ErrorStream = null;
            UseAnsi = true;
            SuppressAttributes = false;
        }

        /// <summary>
        /// Setups ANSI output.
        /// </summary>
        /// <param name="outputStream">Output stream.</param>
        /// <exception cref="ArgumentNullException">Output stream cannot be null.</exception>
        public static void Setup(Stream outputStream) {
            Setup(outputStream, outputStream);
        }

        /// <summary>
        /// Setups ANSI output.
        /// </summary>
        /// <param name="outputStream">Output stream.</param>
        /// <param name="errorStream">Error stream.</param>
        /// <exception cref="ArgumentNullException">Output stream cannot be null. -or- Error stream cannot be null.</exception>
        public static void Setup(Stream outputStream, Stream errorStream) {
            if (outputStream is null) { throw new ArgumentNullException(nameof(outputStream), "Output stream cannot be null."); }
            if (errorStream is null) { throw new ArgumentNullException(nameof(errorStream), "Error stream cannot be null."); }

            OutputStream = outputStream;
            ErrorStream = errorStream;
            UseAnsi = true;
            SuppressAttributes = false;
        }

        /// <summary>
        /// Setups plain console output.
        /// </summary>
        public static void SetupPlain() {
            OutputStream = null;
            ErrorStream = null;
            UseAnsi = false;
            SuppressAttributes = false;
        }


        private static Stream? OutputStream;
        private static Stream? ErrorStream;
        private static bool UsingErrorStream;

        private static readonly object SyncOutput = new();


        private static bool _useAnsi;
        /// <summary>
        /// Gets/sets if ANSI is to be used.
        /// </summary>
        public static bool UseAnsi {
            get { lock (SyncOutput) { return _useAnsi; } }
            set { lock (SyncOutput) { _useAnsi = value; } }
        }

        private static bool _suppressAttributes;
        /// <summary>
        /// Gets/sets if color (and other ANSI sequences) are to be supressed.
        /// </summary>
        public static bool SuppressAttributes {
            get { lock (SyncOutput) { return _suppressAttributes; } }
            set { lock (SyncOutput) { _suppressAttributes = value; } }
        }


        /// <summary>
        /// All write operations will use standard output stream.
        /// </summary>
        public static void UseStandardStream() {
            lock (SyncOutput) { UsingErrorStream = false; }
        }

        /// <summary>
        /// All write operations will use error output stream.
        /// </summary>
        public static void UseErrorStream() {
            lock (SyncOutput) { UsingErrorStream = true; }
        }


        /// <summary>
        /// Clears the screen.
        /// </summary>
        public static Sequence Clear() {
            lock (SyncOutput) {
                if (SuppressAttributes) { return Sequence.Chain; }
                var stream = GetAppropriateStream();
                if (stream is not null) {
                    ComposingBytes[1] = 0x5B;  // [
                    ComposingBytes[2] = 0x32;  // 2
                    ComposingBytes[3] = 0x4A;  // J
                    ComposingBytes[4] = 0x1B;  // Esc
                    ComposingBytes[5] = 0x5B;  // [
                    ComposingBytes[6] = 0x48;  // H
                    stream.Write(ComposingBytes, 0, 7);
                } else {  // Fallback
                    Console.Clear();
                }
            }
            return Sequence.Chain;
        }

        /// <summary>
        /// Resets color and other attributes.
        /// </summary>
        public static Sequence Reset() {
            lock (SyncOutput) {
                if (SuppressAttributes) { return Sequence.Chain; }
                var stream = GetAppropriateStream();
                if (stream is not null) {
                    ComposingBytes[1] = 0x5B;  // [
                    ComposingBytes[2] = 0x30;  // 0
                    ComposingBytes[3] = 0x6D;  // m
                    stream.Write(ComposingBytes, 0, 4);
                } else {  // FallBack: reset colors and variables
                    Console.ResetColor();
                    FallbackIsBold = false;
                    FallbackIsInverted = false;
                }
            }
            return Sequence.Chain;
        }


        /// <summary>
        /// Sets foreground color to black.
        /// </summary>
        public static Sequence Black() => Foreground(ConsoleColor.Black);

        /// <summary>
        /// Sets foreground color to dark red.
        /// </summary>
        public static Sequence DarkRed() => Foreground(ConsoleColor.DarkRed);

        /// <summary>
        /// Sets foreground color to dark green.
        /// </summary>
        public static Sequence DarkGreen() => Foreground(ConsoleColor.DarkGreen);

        /// <summary>
        /// Sets foreground color to dark yellow.
        /// </summary>
        public static Sequence DarkYellow() => Foreground(ConsoleColor.DarkYellow);

        /// <summary>
        /// Sets foreground color to dark blue.
        /// </summary>
        public static Sequence DarkBlue() => Foreground(ConsoleColor.DarkBlue);

        /// <summary>
        /// Sets foreground color to dark magenta.
        /// </summary>
        public static Sequence DarkMagenta() => Foreground(ConsoleColor.DarkMagenta);

        /// <summary>
        /// Sets foreground color to cyan.
        /// </summary>
        public static Sequence DarkCyan() => Foreground(ConsoleColor.DarkCyan);

        /// <summary>
        /// Sets foreground color to gray.
        /// </summary>
        public static Sequence Gray() => Foreground(ConsoleColor.Gray);

        /// <summary>
        /// Sets foreground color to dark gray.
        /// </summary>
        public static Sequence DarkGray() => Foreground(ConsoleColor.DarkGray);

        /// <summary>
        /// Sets foreground color to red.
        /// </summary>
        public static Sequence Red() => Foreground(ConsoleColor.Red);

        /// <summary>
        /// Sets foreground color to green.
        /// </summary>
        public static Sequence Green() => Foreground(ConsoleColor.Green);

        /// <summary>
        /// Sets foreground color to yellow.
        /// </summary>
        public static Sequence Yellow() => Foreground(ConsoleColor.Yellow);

        /// <summary>
        /// Sets foreground color to blue.
        /// </summary>
        public static Sequence Blue() => Foreground(ConsoleColor.Blue);

        /// <summary>
        /// Sets foreground color to magenta.
        /// </summary>
        public static Sequence Magenta() => Foreground(ConsoleColor.Magenta);

        /// <summary>
        /// Sets foreground color to cyan.
        /// </summary>
        public static Sequence Cyan() => Foreground(ConsoleColor.Cyan);

        /// <summary>
        /// Sets foreground color to white.
        /// </summary>
        public static Sequence White() => Foreground(ConsoleColor.White);

        /// <summary>
        /// Resets foreground color.
        /// </summary>
        public static Sequence NoColor() => NoForeground();

        /// <summary>
        /// Sets foreground color.
        /// </summary>
        /// <param name="color">Color to use.</param>
        public static Sequence Foreground(ConsoleColor color) {
            lock (SyncOutput) {
                if (SuppressAttributes) { return Sequence.Chain; }
                var stream = GetAppropriateStream();
                if (stream is not null) {
                    var (colorByte, isColorBright) = GetColorByte(color);
                    ComposingBytes[1] = 0x5B;  // [
                    ComposingBytes[2] = isColorBright ? (byte)0x39 : (byte)0x33;  // 3 or 9
                    ComposingBytes[3] = colorByte;
                    ComposingBytes[4] = 0x6D;  // m
                    stream.Write(ComposingBytes, 0, 5);
                } else {  // Fallback: just use color directly
                    if (UsingErrorStream) { return Sequence.Chain; }  // no color for error stream
                    Console.ForegroundColor = color;
                }
            }
            return Sequence.Chain;
        }

        /// <summary>
        /// Resets foreground color.
        /// </summary>
        public static Sequence NoForeground() {
            lock (SyncOutput) {
                if (SuppressAttributes) { return Sequence.Chain; }
                var stream = GetAppropriateStream();
                if (stream is not null) {
                    ComposingBytes[1] = 0x5B;  // [
                    ComposingBytes[2] = 0x33;  // 3
                    ComposingBytes[3] = 0x39;  // 9
                    ComposingBytes[4] = 0x6D;  // m
                    stream.Write(ComposingBytes, 0, 5);
                } else {  // Fallback: just use color directly
                    if (UsingErrorStream) { return Sequence.Chain; }  // no color for error stream
                    var backColor = Console.BackgroundColor;
                    Console.ResetColor();
                    Console.BackgroundColor = backColor;
                }
            }
            return Sequence.Chain;
        }

        /// <summary>
        /// Sets background color.
        /// </summary>
        /// <param name="color">Color to use.</param>
        public static Sequence Background(ConsoleColor color) {
            lock (SyncOutput) {
                if (SuppressAttributes) { return Sequence.Chain; }
                var stream = GetAppropriateStream();
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
                    if (UsingErrorStream) { return Sequence.Chain; }  // no color for error stream
                    Console.BackgroundColor = color;
                }
            }
            return Sequence.Chain;
        }

        /// <summary>
        /// Resets background color.
        /// </summary>
        public static Sequence NoBackground() {
            lock (SyncOutput) {
                if (SuppressAttributes) { return Sequence.Chain; }
                var stream = GetAppropriateStream();
                if (stream is not null) {
                    ComposingBytes[1] = 0x5B;  // [
                    ComposingBytes[2] = 0x34;  // 4
                    ComposingBytes[3] = 0x39;  // 9
                    ComposingBytes[4] = 0x6D;  // m
                    stream.Write(ComposingBytes, 0, 5);
                } else {  // Fallback: just use color directly
                    if (UsingErrorStream) { return Sequence.Chain; }  // no color for error stream
                    var foreColor = Console.ForegroundColor;
                    Console.ResetColor();
                    Console.ForegroundColor = foreColor;
                }
            }
            return Sequence.Chain;
        }


        /// <summary>
        /// Sets bold attribute.
        /// </summary>
        public static Sequence Bold() {
            return SetBold(newState: true);
        }

        /// <summary>
        /// Resets bold attribute.
        /// </summary>
        public static Sequence NoBold() {
            return SetBold(newState: false);
        }

        private static Sequence SetBold(bool newState) {
            lock (SyncOutput) {
                if (SuppressAttributes) { return Sequence.Chain; }
                var stream = GetAppropriateStream();
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
                        var colorValue = (int)Console.ForegroundColor;
                        if (colorValue <= 7) {  // just do it for dark colors
                            FallbackIsBold = true;
                            Console.ForegroundColor = (ConsoleColor)(colorValue + 8);
                        }
                    } else {
                        var colorValue = (int)Console.ForegroundColor;
                        if (FallbackIsBold && (colorValue > 7)) {  // just do it for bright colors
                            Console.ForegroundColor = (ConsoleColor)(colorValue - 8);
                        }
                        FallbackIsBold = false;  // cancel bold either way
                    }
                }
            }
            return Sequence.Chain;
        }


        /// <summary>
        /// Sets underline attribute.
        /// No fallback for Console.
        /// </summary>
        public static Sequence Underline() {
            return SetUnderline(newState: true);
        }

        /// <summary>
        /// Resets underline.
        /// </summary>
        public static Sequence NoUnderline() {
            return SetUnderline(newState: false);
        }

        private static Sequence SetUnderline(bool newState) {
            lock (SyncOutput) {
                if (SuppressAttributes) { return Sequence.Chain; }
                var stream = GetAppropriateStream();
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
            return Sequence.Chain;
        }


        /// <summary>
        /// Sets color invert attribute.
        /// </summary>
        public static Sequence Invert() {
            return SetInvert(newState: true);
        }

        /// <summary>
        /// Resets color invert attribute.
        /// </summary>
        public static Sequence NoInvert() {
            return SetInvert(newState: false);
        }

        private static Sequence SetInvert(bool newState) {
            lock (SyncOutput) {
                if (SuppressAttributes) { return Sequence.Chain; }
                var stream = GetAppropriateStream();
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
                            (Console.BackgroundColor, Console.ForegroundColor) = (Console.ForegroundColor, Console.BackgroundColor);
                            FallbackIsInverted = true;
                        }
                    } else {
                        if (FallbackIsInverted) {
                            (Console.BackgroundColor, Console.ForegroundColor) = (Console.ForegroundColor, Console.BackgroundColor);
                            FallbackIsInverted = false;
                        }
                    }
                }
            }
            return Sequence.Chain;
        }


        /// <summary>
        /// Moves cursor left.
        /// </summary>
        public static Sequence MoveLeft() {
            return MoveLeft(amount: 1);
        }

        /// <summary>
        /// Moves cursor left.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Amount must be a positive number less than 65536.</exception>
        public static Sequence MoveLeft(int amount) {
            if (amount is < 1 or > 65535) { throw new ArgumentNullException(nameof(amount), "Amount must be a positive number less than 65536."); }
            return MoveHorizontally(-amount);
        }

        /// <summary>
        /// Moves cursor right.
        /// </summary>
        public static Sequence MoveRight() {
            return MoveRight(amount: 1);
        }

        /// <summary>
        /// Moves cursor right.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Amount must be a positive number less than 65536.</exception>
        public static Sequence MoveRight(int amount) {
            if (amount is < 1 or > 65535) { throw new ArgumentNullException(nameof(amount), "Amount must be a positive number less than 65536."); }
            return MoveHorizontally(amount);
        }

        private static Sequence MoveHorizontally(int amount) {
            if (amount == 0) { return Sequence.Chain; }
            lock (SyncOutput) {
                if (SuppressAttributes) { return Sequence.Chain; }
                var stream = GetAppropriateStream();
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
                        if (Console.CursorLeft + amount >= Console.BufferWidth) {
                            Console.CursorLeft = Console.BufferWidth - 1;
                        } else {
                            Console.CursorLeft += amount;
                        }
                    } else {  // move backward
                        if (Console.CursorLeft + amount < 0) {
                            Console.CursorLeft = 0;
                        } else {
                            Console.CursorLeft += amount;
                        }
                    }
                }
            }
            return Sequence.Chain;
        }


        /// <summary>
        /// Moves cursor up.
        /// </summary>
        public static Sequence MoveUp() {
            return MoveUp(amount: 1);
        }

        /// <summary>
        /// Moves cursor up.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Amount must be a positive number less than 65536.</exception>
        public static Sequence MoveUp(int amount) {
            if (amount is < 1 or > 65535) { throw new ArgumentNullException(nameof(amount), "Amount must be a positive number less than 65536."); }
            return MoveVertically(-amount);
        }

        /// <summary>
        /// Moves cursor down.
        /// </summary>
        public static Sequence MoveDown() {
            return MoveVertically(amount: 1);
        }

        /// <summary>
        /// Moves cursor down.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Amount must be a positive number less than 65536.</exception>
        public static Sequence MoveDown(int amount) {
            if (amount is < 1 or > 65535) { throw new ArgumentNullException(nameof(amount), "Amount must be a positive number less than 65536."); }
            return MoveVertically(amount);
        }

        private static Sequence MoveVertically(int amount) {
            if (amount == 0) { return Sequence.Chain; }
            lock (SyncOutput) {
                if (SuppressAttributes) { return Sequence.Chain; }
                var stream = GetAppropriateStream();
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
                        if (Console.CursorTop + amount >= Console.BufferHeight) {
                            Console.CursorTop = Console.BufferHeight - 1;
                        } else {
                            Console.CursorTop += amount;
                        }
                    } else {  // move backward
                        if (Console.CursorTop + amount < 0) {
                            Console.CursorTop = 0;
                        } else {
                            Console.CursorTop += amount;
                        }
                    }
                }
            }
            return Sequence.Chain;
        }

        /// <summary>
        /// Moves to specified coordinates.
        /// </summary>
        /// <param name="x">X coordinate; 0 ignores the parameter.</param>
        /// <param name="y">Y coordinate; 0 ignores the parameter.</param>
        /// <exception cref="ArgumentOutOfRangeException">X must be either 0 or between 1 and 65535. -or- Y must be either 0 or between 1 and 65535.</exception>
        public static Sequence MoveTo(int x, int y) {
            if (x is < 0 or > 65535) { throw new ArgumentOutOfRangeException(nameof(x), "X must be either 0 or between 1 and 65535."); }
            if (y is < 0 or > 65535) { throw new ArgumentOutOfRangeException(nameof(y), "Y must be either 0 or between 1 and 65535."); }

            var moveHor = (x > 0);
            var moveVer = (y > 0);
            if (!moveHor && !moveVer) { return Sequence.Chain; }

            lock (SyncOutput) {
                if (SuppressAttributes) { return Sequence.Chain; }
                var stream = GetAppropriateStream();
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
                        if (x > Console.BufferWidth) {
                            Console.CursorLeft = Console.BufferWidth - 1;
                        } else {
                            Console.CursorLeft = x - 1;
                        }
                    }
                    if (moveVer) {
                        if (y > Console.BufferHeight) {
                            Console.CursorTop = Console.BufferHeight - 1;
                        } else {
                            Console.CursorTop = y - 1;
                        }
                    }
                }
            }
            return Sequence.Chain;
        }


        /// <summary>
        /// Stores cursor location.
        /// </summary>
        public static Sequence StoreCursor() {
            lock (SyncOutput) {
                if (SuppressAttributes) { return Sequence.Chain; }
                var stream = GetAppropriateStream();
                if (stream is not null) {
                    ComposingBytes[1] = 0x5B;  // [
                    ComposingBytes[2] = 0x73;  // s
                    stream.Write(ComposingBytes, 0, 3);
                } else {  // FallBack: save coordinates
                    FallbackLeft = Console.CursorLeft;
                    FallbackTop = Console.CursorTop;
                }
            }
            return Sequence.Chain;
        }

        /// <summary>
        /// Restores cursor location.
        /// </summary>
        public static Sequence RestoreCursor() {
            lock (SyncOutput) {
                if (SuppressAttributes) { return Sequence.Chain; }
                var stream = GetAppropriateStream();
                if (stream is not null) {
                    ComposingBytes[1] = 0x5B;  // [
                    ComposingBytes[2] = 0x75;  // u
                    stream.Write(ComposingBytes, 0, 3);
                } else {  // FallBack: use saved coordinates
                    Console.CursorLeft = FallbackLeft;
                    Console.CursorTop = FallbackTop;
                }
            }
            return Sequence.Chain;
        }


        /// <summary>
        /// Writes text.
        /// </summary>
        /// <param name="text">Text to write.</param>
        public static Sequence Write(string text) {
            if (string.IsNullOrEmpty(text)) { return Sequence.Chain; }  // just ignore
            lock (SyncOutput) {
                var stream = GetAppropriateStream();
                if (stream is not null) {
                    var bytes = Utf8.GetBytes(text);
                    stream.Write(bytes, 0, bytes.Length);
                } else {  // Fallback: just write
                    Console.Write(text);
                }
            }
            return Sequence.Chain;
        }

        /// <summary>
        /// Writes text.
        /// Color is reset to default after write is complete.
        /// </summary>
        /// <param name="text">Text to write.</param>
        /// <param name="foregroundColor">Foreground color.</param>
        public static Sequence Write(string text, ConsoleColor foregroundColor) {
            lock (SyncOutput) {
                Foreground(foregroundColor);
                Write(text);
                NoForeground();
            }
            return Sequence.Chain;
        }

        /// <summary>
        /// Writes text.
        /// Color is reset to default after write is complete.
        /// </summary>
        /// <param name="text">Text to write.</param>
        /// <param name="foregroundColor">Foreground color.</param>
        /// <param name="backgroundColor">Background color.</param>
        public static Sequence Write(string text, ConsoleColor foregroundColor, ConsoleColor backgroundColor) {
            lock (SyncOutput) {
                Foreground(foregroundColor);
                Background(backgroundColor);
                Write(text);
                NoBackground();
                NoForeground();
            }
            return Sequence.Chain;
        }


        /// <summary>
        /// Writes a new line.
        /// </summary>
        public static Sequence WriteLine() {
            Write(Environment.NewLine);
            return Sequence.Chain;
        }

        /// <summary>
        /// Writes text and ends it with a new line.
        /// </summary>
        /// <param name="text">Text to write.</param>
        public static Sequence WriteLine(string text) {
            Write(text);
            Write(Environment.NewLine);
            return Sequence.Chain;
        }

        /// <summary>
        /// Writes text and ends it with a new line.
        /// Color is reset to default after write is complete.
        /// </summary>
        /// <param name="text">Text to write.</param>
        /// <param name="foregroundColor">Foreground color.</param>
        public static Sequence WriteLine(string text, ConsoleColor foregroundColor) {
            lock (SyncOutput) {
                Foreground(foregroundColor);
                WriteLine(text);
                NoForeground();
            }
            return Sequence.Chain;
        }

        /// <summary>
        /// Writes text and ends it with a new line.
        /// Color is reset to default after write is complete.
        /// </summary>
        /// <param name="text">Text to write.</param>
        /// <param name="foregroundColor">Foreground color.</param>
        /// <param name="backgroundColor">Background color.</param>
        public static Sequence WriteLine(string text, ConsoleColor foregroundColor, ConsoleColor backgroundColor) {
            lock (SyncOutput) {
                Foreground(foregroundColor);
                Background(backgroundColor);
                WriteLine(text);
                NoBackground();
                NoForeground();
            }
            return Sequence.Chain;
        }


        private static readonly object SyncInput = new();

        /// <summary>
        /// Returns the next character available.
        /// Character is always read from Console.
        /// </summary>
        public static char? ReadCharIfAvailable() {
            lock (SyncInput) {
                return Console.KeyAvailable ? Console.ReadKey(intercept: true).KeyChar : null;
            }
        }

        /// <summary>
        /// Returns the next character available.
        /// Character is always read from Console.
        /// </summary>
        public static char ReadChar() {
            lock (SyncInput) {
                return Console.ReadKey(intercept: true).KeyChar;
            }
        }

        /// <summary>
        /// Returns all currently available keys.
        /// Character is always read from Console.
        /// </summary>
        public static IEnumerable<char> ReadAvailableChars() {
            lock (SyncInput) {
                while (Console.KeyAvailable) {
                    yield return Console.ReadKey(intercept: true).KeyChar;
                }
            }
        }

        /// <summary>
        /// Returns the next key available.
        /// Character is always read from Console.
        /// </summary>
        public static ConsoleKey ReadKey() {
            lock (SyncInput) {
                return Console.ReadKey(intercept: true).Key;
            }
        }

        /// <summary>
        /// Returns the next key available.
        /// Character is always read from Console.
        /// </summary>
        public static ConsoleKey? ReadKeyIfAvailable() {
            lock (SyncInput) {
                return Console.KeyAvailable ? Console.ReadKey(intercept: true).Key : null;
            }
        }

        /// <summary>
        /// Returns all currently available keys.
        /// Character is always read from Console.
        /// </summary>
        public static IEnumerable<ConsoleKey> ReadAvailableKeys() {
            lock (SyncInput) {
                while (Console.KeyAvailable) {
                    yield return Console.ReadKey(intercept: true).Key;
                }
            }
        }


        /// <summary>
        /// Gets/sets if Ctrl+C will be treated as a normal input.
        /// </summary>
        public static bool TreatControlCAsInput {
            get { return Console.TreatControlCAsInput; }
            set { Console.TreatControlCAsInput = value; }
        }


        #region Sequences

        /// <summary>
        /// Helper class used to chain calls.
        /// </summary>
        public sealed class Sequence {

            private Sequence() { }
            internal static Sequence Chain { get; } = new Sequence();

#pragma warning disable CA1822 // Mark members as static - used for chaining so intentional

            /// <summary>
            /// Clears the screen.
            /// </summary>
            public Sequence Clear() => Terminal.Clear();

            /// <summary>
            /// Resets color and other attributes.
            /// </summary>
            public Sequence Reset() => Terminal.Reset();

            /// <summary>
            /// Sets foreground color to black.
            /// </summary>
            public Sequence Black() => Terminal.Black();

            /// <summary>
            /// Sets foreground color to dark red.
            /// </summary>
            public Sequence DarkRed() => Terminal.DarkRed();

            /// <summary>
            /// Sets foreground color to dark green.
            /// </summary>
            public Sequence DarkGreen() => Terminal.DarkGreen();

            /// <summary>
            /// Sets foreground color to dark yellow.
            /// </summary>
            public Sequence DarkYellow() => Terminal.DarkYellow();

            /// <summary>
            /// Sets foreground color to dark blue.
            /// </summary>
            public Sequence DarkBlue() => Terminal.DarkBlue();

            /// <summary>
            /// Sets foreground color to dark magenta.
            /// </summary>
            public Sequence DarkMagenta() => Terminal.DarkMagenta();

            /// <summary>
            /// Sets foreground color to cyan.
            /// </summary>
            public Sequence DarkCyan() => Terminal.DarkCyan();

            /// <summary>
            /// Sets foreground color to gray.
            /// </summary>
            public Sequence Gray() => Terminal.Gray();

            /// <summary>
            /// Sets foreground color to dark gray.
            /// </summary>
            public Sequence DarkGray() => Terminal.DarkGray();

            /// <summary>
            /// Sets foreground color to red.
            /// </summary>
            public Sequence Red() => Terminal.Red();

            /// <summary>
            /// Sets foreground color to green.
            /// </summary>
            public Sequence Green() => Terminal.Green();

            /// <summary>
            /// Sets foreground color to yellow.
            /// </summary>
            public Sequence Yellow() => Terminal.Yellow();

            /// <summary>
            /// Sets foreground color to blue.
            /// </summary>
            public Sequence Blue() => Terminal.Blue();

            /// <summary>
            /// Sets foreground color to magenta.
            /// </summary>
            public Sequence Magenta() => Terminal.Magenta();

            /// <summary>
            /// Sets foreground color to cyan.
            /// </summary>
            public Sequence Cyan() => Terminal.Cyan();

            /// <summary>
            /// Sets foreground color to white.
            /// </summary>
            public Sequence White() => Terminal.White();

            /// <summary>
            /// Resets foreground color.
            /// </summary>
            public Sequence NoColor() => Terminal.NoColor();

            /// <summary>
            /// Sets foreground color.
            /// </summary>
            /// <param name="color">Color to use.</param>
            public Sequence Foreground(ConsoleColor color) => Terminal.Foreground(color);

            /// <summary>
            /// Resets foreground color.
            /// </summary>
            public Sequence NoForeground() => Terminal.NoForeground();

            /// <summary>
            /// Sets background color.
            /// </summary>
            /// <param name="color">Color to use.</param>
            public Sequence Background(ConsoleColor color) => Terminal.Background(color);

            /// <summary>
            /// Resets background color.
            /// </summary>
            public Sequence NoBackground() => Terminal.NoBackground();

            /// <summary>
            /// Sets bold attribute.
            /// </summary>
            public Sequence Bold() => Terminal.Bold();

            /// <summary>
            /// Resets bold attribute.
            /// </summary>
            public Sequence NoBold() => Terminal.NoBold();

            /// <summary>
            /// Sets underline attribute.
            /// No fallback for Console.
            /// </summary>
            public Sequence Underline() => Terminal.Underline();

            /// <summary>
            /// Resets underline.
            /// </summary>
            public Sequence NoUnderline() => Terminal.NoUnderline();

            /// <summary>
            /// Sets color invert attribute.
            /// </summary>
            public Sequence Invert() => Terminal.Invert();

            /// <summary>
            /// Resets color invert attribute.
            /// </summary>
            public Sequence NoInvert() => Terminal.NoInvert();

            /// <summary>
            /// Moves cursor left.
            /// </summary>
            public Sequence MoveLeft() => Terminal.MoveLeft();

            /// <summary>
            /// Moves cursor left.
            /// </summary>
            /// <exception cref="ArgumentOutOfRangeException">Amount must be a positive number less than 65536.</exception>
            public Sequence MoveLeft(int amount) => Terminal.MoveLeft(amount);

            /// <summary>
            /// Moves cursor right.
            /// </summary>
            public Sequence MoveRight() => Terminal.MoveRight();

            /// <summary>
            /// Moves cursor right.
            /// </summary>
            /// <exception cref="ArgumentOutOfRangeException">Amount must be a positive number less than 65536.</exception>
            public Sequence MoveRight(int amount) => Terminal.MoveRight(amount);

            /// <summary>
            /// Moves cursor up.
            /// </summary>
            public Sequence MoveUp() => Terminal.MoveUp();

            /// <summary>
            /// Moves cursor up.
            /// </summary>
            /// <exception cref="ArgumentOutOfRangeException">Amount must be a positive number less than 65536.</exception>
            public Sequence MoveUp(int amount) => Terminal.MoveUp(amount);

            /// <summary>
            /// Moves cursor down.
            /// </summary>
            public Sequence MoveDown() => Terminal.MoveDown();

            /// <summary>
            /// Moves cursor down.
            /// </summary>
            /// <exception cref="ArgumentOutOfRangeException">Amount must be a positive number less than 65536.</exception>
            public Sequence MoveDown(int amount) => Terminal.MoveDown(amount);

            /// <summary>
            /// Moves to specified coordinates.
            /// </summary>
            /// <param name="x">X coordinate; 0 ignores the parameter.</param>
            /// <param name="y">Y coordinate; 0 ignores the parameter.</param>
            /// <exception cref="ArgumentOutOfRangeException">X must be either 0 or between 1 and 65535. -or- Y must be either 0 or between 1 and 65535.</exception>
            public Sequence MoveTo(int x, int y) => Terminal.MoveTo(x, y);

            /// <summary>
            /// Stores cursor location.
            /// </summary>
            public Sequence StoreCursor() => Terminal.StoreCursor();

            /// <summary>
            /// Restores cursor location.
            /// </summary>
            public Sequence RestoreCursor() => Terminal.RestoreCursor();

            /// <summary>
            /// Writes text.
            /// </summary>
            /// <param name="text">Text to write.</param>
            public Sequence Write(string text) => Terminal.Write(text);

            /// <summary>
            /// Writes text.
            /// Color is reset to default after write is complete.
            /// </summary>
            /// <param name="text">Text to write.</param>
            /// <param name="foregroundColor">Foreground color.</param>
            public Sequence Write(string text, ConsoleColor foregroundColor) => Terminal.Write(text, foregroundColor);

            /// <summary>
            /// Writes text.
            /// Color is reset to default after write is complete.
            /// </summary>
            /// <param name="text">Text to write.</param>
            /// <param name="foregroundColor">Foreground color.</param>
            /// <param name="backgroundColor">Background color.</param>
            public Sequence Write(string text, ConsoleColor foregroundColor, ConsoleColor backgroundColor) => Terminal.Write(text, foregroundColor, backgroundColor);

            /// <summary>
            /// Writes a new line.
            /// </summary>
            public Sequence WriteLine() => Terminal.WriteLine();

            /// <summary>
            /// Writes text and ends it with a new line.
            /// </summary>
            /// <param name="text">Text to write.</param>
            public Sequence WriteLine(string text) => Terminal.WriteLine(text);

            /// <summary>
            /// Writes text and ends it with a new line.
            /// Color is reset to default after write is complete.
            /// </summary>
            /// <param name="text">Text to write.</param>
            /// <param name="foregroundColor">Foreground color.</param>
            public Sequence WriteLine(string text, ConsoleColor foregroundColor) => Terminal.WriteLine(text, foregroundColor);

            /// <summary>
            /// Writes text and ends it with a new line.
            /// Color is reset to default after write is complete.
            /// </summary>
            /// <param name="text">Text to write.</param>
            /// <param name="foregroundColor">Foreground color.</param>
            /// <param name="backgroundColor">Background color.</param>
            public Sequence WriteLine(string text, ConsoleColor foregroundColor, ConsoleColor backgroundColor) => Terminal.WriteLine(text, foregroundColor, backgroundColor);

#pragma warning restore CA1822 // Mark members as static

        }

        #endregion Sequences


        #region Privates

        private static readonly byte[] ComposingBytes = new byte[] { 0x1B, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };  // used to compose escape sequences, escape char already in
        private static readonly Encoding Utf8 = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

        private static bool FallbackIsBold;  // remember bold status for console fallback
        private static bool FallbackIsInverted;  // remember inversion status for console fallback
        private static int FallbackLeft;
        private static int FallbackTop;

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

        private static Stream? GetAppropriateStream() {
            if (UseAnsi) {
                if (OutputStream is null) { OutputStream = Console.OpenStandardOutput(); }
                if (ErrorStream is null) { ErrorStream = Console.OpenStandardError(); }
                return UsingErrorStream ? ErrorStream : OutputStream;
            } else {
                return null;
            }
        }

        #endregion Privates

    }
}
