/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Medo.Configuration {
    /// <summary>
    /// Provides read-only access for to a configuration file.
    /// File should follow Microsoft's INI file format but other variations are supported too.
    /// </summary>
    public class IniFile {

        /// <summary>
        /// Creates a new empty instance.
        /// </summary>
        public IniFile() {
            ParserObject = new Parser("");
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="path">File path.</param>
        /// <exception cref="ArgumentNullException">File path cannot be null.</exception>
        /// <exception cref="FileNotFoundException">File not found.</exception>
        public IniFile(string path) :
            this(File.OpenRead(path ?? throw new ArgumentNullException(nameof(path), "File path cannot be null."))) {
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="stream">Stream.</param>
        /// <exception cref="ArgumentNullException">Stream cannot be null.</exception>
        public IniFile(Stream stream) {
            if (stream == null) { throw new ArgumentNullException(nameof(stream), "Stream cannot be null."); }
            using var reader = new StreamReader(stream, Encoding.UTF8);
            ParserObject = new Parser(reader.ReadToEnd());  // just read the whole darn thing
        }


        #region Read

        /// <summary>
        /// Returns all the values for the specified key.
        /// </summary>
        /// <param name="section">Section name.</param>
        /// <param name="key">Key name.</param>
        /// <exception cref="ArgumentNullException">Section cannot be null. -or- Key cannot be null.</exception>
        public IReadOnlyList<string> ReadAll(string section, string key) {
            if (section == null) { throw new ArgumentNullException(nameof(section), "Section cannot be null."); }
            if (key == null) { throw new ArgumentNullException(nameof(key), "Key cannot be null."); }
            return ParserObject.GetValue(section, key);
        }

        /// <summary>
        /// Returns the value for the specified section and key or null if value is not found.
        /// </summary>
        /// <param name="section">Section name.</param>
        /// <param name="key">Key name.</param>
        /// <exception cref="ArgumentNullException">Section cannot be null. -or- Key cannot be null.</exception>
        public string? Read(string section, string key) {
            var results = ReadAll(section, key);
            return (results.Count > 0) ? results[^1] : null;
        }


        /// <summary>
        /// Returns string value for the specified key.
        /// </summary>
        /// <param name="section">Section name.</param>
        /// <param name="key">Key name.</param>
        /// <param name="defaultValue">The value to return if the entry does not exist.</param>
        /// <exception cref="ArgumentNullException">Section cannot be null. -or- Key cannot be null. -or- Default value cannot be null.</exception>
        public string Read(string section, string key, string defaultValue) {
            if (defaultValue == null) { throw new ArgumentNullException(nameof(defaultValue), "Default value cannot be null."); }
            return Read(section, key) ?? defaultValue;
        }


        /// <summary>
        /// Returns boolean value for the specified key.
        /// </summary>
        /// <param name="section">Section name.</param>
        /// <param name="key">Key name.</param>
        /// <param name="defaultValue">The value to return if the entry does not exist or cannot be converted.</param>
        /// <exception cref="ArgumentNullException">Section cannot be null. -or- Key cannot be null.</exception>
        public bool Read(string section, string key, bool defaultValue) {
            return GetValue(Read(section, key), defaultValue);
        }

        /// <summary>
        /// Returns integer value for the specified key.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="section">Section name.</param>
        /// <param name="key">Key name.</param>
        /// <param name="defaultValue">The value to return if the entry does not exist or cannot be converted.</param>
        /// <exception cref="ArgumentNullException">Section cannot be null. -or- Key cannot be null.</exception>
        public int Read(string section, string key, int defaultValue) {
            return GetValue(Read(section, key), defaultValue);
        }

        /// <summary>
        /// Returns integer value for the specified key.
        /// </summary>
        /// <param name="section">Section name.</param>
        /// <param name="key">Key name.</param>
        /// <param name="defaultValue">The value to return if the entry does not exist or cannot be converted.</param>
        /// <exception cref="ArgumentNullException">Section cannot be null. -or- Key cannot be null.</exception>
        public long Read(string section, string key, long defaultValue) {
            return GetValue(Read(section, key), defaultValue);
        }

        /// <summary>
        /// Returns floating point value for the specified key.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="section">Section name.</param>
        /// <param name="key">Key name.</param>
        /// <param name="defaultValue">The value to return if the entry does not exist or cannot be converted.</param>
        /// <exception cref="ArgumentNullException">Section cannot be null. -or- Key cannot be null.</exception>
        public float Read(string section, string key, float defaultValue) {
            return GetValue(Read(section, key), defaultValue);
        }

        /// <summary>
        /// Returns floating point value for the specified key.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="section">Section name.</param>
        /// <param name="key">Key name.</param>
        /// <param name="defaultValue">The value to return if the entry does not exist or cannot be converted.</param>
        public double Read(string section, string key, double defaultValue) {
            return GetValue(Read(section, key), defaultValue);
        }

        #endregion Read


        #region Parsing

        private readonly Parser ParserObject;

        private class Parser {
            internal Parser(string text) {
                Text = text;
            }

            private readonly string Text;
            private enum State { Normal, Single, Double, Done }

            private readonly object SyncParse = new();
            private Dictionary<string, Dictionary<string, List<string>>>? CachedResult;
            private int CachedResultCount;
            private Dictionary<string, Dictionary<string, List<string>>> Parse() {  // key without name is a section
                lock (SyncParse) {
                    if (CachedResult == null) {
                        var rawEntries = new List<(string section, string key, string value)>();

                        var nonEmptyLines = Text.Split(new String[] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
                        var lines = new List<string>();
                        foreach (var line in nonEmptyLines) {
                            var trimmedLine = line.Trim();
                            if (trimmedLine.Length == 0) { continue; }  // ignore empty lines
                            if (line.StartsWith('#')) { continue; }  // ignore full comment lines
                            lines.Add(trimmedLine);
                        }

                        var lastSection = "";
                        foreach (var line in lines) {
                            if (line.StartsWith('[')) {  // this is a section
                                if (line.EndsWith(']')) {
                                    lastSection = line[1..^1].Trim();
                                } else {  // if no end bracket, just take it all in
                                    lastSection = line[1..].Trim();
                                }
                            } else {  // key value
                                var parts = line.Split(new char[] { ':', '=' }, 2);
                                if (parts.Length == 2) {  // ignore lines that don't have both key and value
                                    var key = parts[0]?.Trim() ?? "";
                                    if (!key.Contains('#')) {  // make sure comment doesn't start in key
                                        var valueChars = new Queue<char>(parts[1]);
                                        valueChars.Enqueue('\n');  // add new line to make parsing easier

                                        var state = State.Normal;
                                        var valueParts = new List<(string Text, bool Trimmable)>();
                                        var valueText = new StringBuilder();
                                        while (valueChars.Count > 0) {
                                            var ch = valueChars.Dequeue();

                                            switch (state) {
                                                case State.Normal:
                                                    if (ch == '\n') {  // done with parsing
                                                        if (valueText.Length > 0) {  // just add trimmable text if there's any
                                                            valueParts.Add((valueText.ToString(), Trimmable: true));
                                                            valueText.Length = 0;
                                                        }
                                                    } else if ((ch == '#') || (ch == ';')) {  // comment
                                                        if (valueText.Length > 0) {  // just add trimmable text if there's any
                                                            valueParts.Add((valueText.ToString(), Trimmable: true));
                                                            valueText.Length = 0;
                                                        }
                                                        state = State.Done;
                                                    } else if (ch == '\'') {  // single quote
                                                        if (valueText.Length > 0) {  // just add trimmable text if there's any
                                                            valueParts.Add((valueText.ToString(), Trimmable: true));
                                                            valueText.Length = 0;
                                                        }
                                                        state = State.Single;
                                                    } else if (ch == '"') {  // double quote
                                                        if (valueText.Length > 0) {  // just add trimmable text if there's any
                                                            valueParts.Add((valueText.ToString(), Trimmable: true));
                                                            valueText.Length = 0;
                                                        }
                                                        state = State.Double;
                                                    } else {  // any other valid character
                                                        valueText.Append(ch);
                                                    }
                                                    break;

                                                case State.Single:
                                                    if (ch == '\n') {  // done with parsing
                                                        valueParts.Add((valueText.ToString(), Trimmable: false));  // just add whatever you have
                                                    } else if (ch == '\'') {
                                                        if (valueChars.Peek() == '\'') {  // double apostrophe
                                                            valueChars.Dequeue();
                                                            valueText.Append('\'');
                                                        } else {
                                                            valueParts.Add((valueText.ToString(), Trimmable: false));  // add to list
                                                            valueText.Length = 0;
                                                            state = State.Normal;
                                                        }
                                                    } else {
                                                        valueText.Append(ch);
                                                    }
                                                    break;

                                                case State.Double:
                                                    if (ch == '\n') {  // done with parsing
                                                        valueParts.Add((valueText.ToString(), Trimmable: false));  // just add whatever you have
                                                    } else if (ch == '"') {
                                                        valueParts.Add((valueText.ToString(), Trimmable: false));  // add to list
                                                        valueText.Length = 0;
                                                        state = State.Normal;
                                                    } else if (ch == '\\') {
                                                        ch = valueChars.Dequeue();  // get next char
                                                        switch (ch) {
                                                            case '\n': break;  // ignore escape
                                                            case '0': valueText.Append('\0'); break;
                                                            case 'a': valueText.Append('\a'); break;
                                                            case 'b': valueText.Append('\b'); break;
                                                            case 'f': valueText.Append('\f'); break;
                                                            case 'n': valueText.Append('\n'); break;
                                                            case 'r': valueText.Append('\r'); break;
                                                            case 't': valueText.Append('\t'); break;
                                                            case 'v': valueText.Append('\v'); break;

                                                            case 'u':
                                                                if (valueChars.Count <= 4) {  //not enough characters, abandon parse
                                                                    valueChars.Clear();
                                                                    valueChars.Enqueue('\n');  // return end-of-line so that it can be catched in next loop iteration
                                                                } else {
                                                                    var number = 0;
                                                                    for (var i = 0; i < 4; i++) {
                                                                        if (number == -1) { continue; }  // just consume
                                                                        var nextChar = valueChars.Dequeue();
                                                                        if (int.TryParse(nextChar.ToString(), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var parsedHex)) {
                                                                            number = (number << 4) | parsedHex;
                                                                        } else {
                                                                            number = -1;  // abandon parse
                                                                        }
                                                                    }
                                                                    if (number >= 0) {  // don't add if not fully parsed
                                                                        valueText.Append(char.ConvertFromUtf32(number));
                                                                    }
                                                                }
                                                                break;

                                                            case 'U':
                                                                if (valueChars.Count <= 8) {  //not enough characters, abandon parse
                                                                    valueChars.Clear();
                                                                    valueChars.Enqueue('\n');  // return end-of-line so that it can be catched in next loop iteration
                                                                } else {
                                                                    var number = 0;
                                                                    for (var i = 0; i < 8; i++) {
                                                                        if (number == -1) { continue; }  // just consume
                                                                        var nextChar = valueChars.Dequeue();
                                                                        if (int.TryParse(nextChar.ToString(), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var parsedHex)) {
                                                                            number = (number << 4) | parsedHex;
                                                                        } else {
                                                                            number = -1;  // abandon parse
                                                                        }
                                                                    }
                                                                    if ((number >= 0) && (number <= 0x00FFFFFF)) {  // don't add if not fully parsed
                                                                        valueText.Append(char.ConvertFromUtf32(number));
                                                                    }
                                                                }
                                                                break;

                                                            case 'x':
                                                                if (valueChars.Count > 1) {  //only parse if we have more than 1 character (i.e. more than new-line)
                                                                    var number = 0;
                                                                    for (var i = 0; i < 4; i++) {
                                                                        var nextChar = valueChars.Peek();
                                                                        if (int.TryParse(nextChar.ToString(), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var parsedHex)) {
                                                                            valueChars.Dequeue();
                                                                            number = (number << 4) | parsedHex;
                                                                        } else {
                                                                            if (i == 0) { valueChars.Dequeue(); number = -1; }  // even in the case of errors, consume at least one character
                                                                            break;
                                                                        }
                                                                    }
                                                                    if (number >= 0) {  // don't add if not fully parsed
                                                                        valueText.Append(char.ConvertFromUtf32(number));
                                                                    }
                                                                }
                                                                break;

                                                            default: valueText.Append(ch); break;
                                                        }
                                                    } else {
                                                        valueText.Append(ch);
                                                    }
                                                    break;

                                                case State.Done:
                                                    break;
                                            }
                                        }

                                        var composedValue = new StringBuilder();
                                        for (var i = 0; i < valueParts.Count; i++) {
                                            var part = valueParts[i];
                                            var text = part.Text;
                                            if ((valueParts.Count == 1) && (part.Trimmable)) {
                                                composedValue.Append(text.Trim());
                                            } else if ((i == 0) && (part.Trimmable)) {
                                                composedValue.Append(text.TrimStart());
                                            } else if ((i == valueParts.Count - 1) && (part.Trimmable)) {
                                                composedValue.Append(text.TrimEnd());
                                            } else {
                                                composedValue.Append(text);
                                            }
                                        }
                                        rawEntries.Add((lastSection, key, composedValue.ToString()));
                                    }
                                }
                            }
                        }

                        var newResult = new Dictionary<string, Dictionary<string, List<string>>>(StringComparer.InvariantCultureIgnoreCase);
                        foreach (var (section, key, value) in rawEntries) {
                            if (!newResult.TryGetValue(section, out var perSectionEntries)) {
                                perSectionEntries = new Dictionary<string, List<string>>(StringComparer.InvariantCultureIgnoreCase);
                                newResult.Add(section, perSectionEntries);
                            }
                            if (!perSectionEntries.TryGetValue(key, out var perKeyEntries)) {
                                perKeyEntries = new List<string>();
                                perSectionEntries.Add(key, perKeyEntries);
                            }
                            perKeyEntries.Add(value);
                        }

                        CachedResult = newResult;
                        CachedResultCount = rawEntries.Count;
                    }

                    return CachedResult;
                }
            }
            private static (string section, string key, string value) ParseRawEntry(StringBuilder sbSection, StringBuilder sbKey, StringBuilder? sbValue) {  // just a helper
                var section = sbSection.ToString();
                var key = sbKey.ToString();
                var value = sbValue?.ToString() ?? "";
                sbKey.Clear();
                if (sbValue != null) { sbValue.Clear(); }
                return (section, key, value);
            }
            private static StringBuilder TrimEnd(StringBuilder sb) {
                var outputLen = sb.Length;
                for (var i = sb.Length - 1; i >= 0; i--) {
                    if (!char.IsWhiteSpace(sb[i])) { break; }
                    outputLen = i;
                }
                sb.Length = outputLen;
                return sb;  // just return it for call chaining
            }

            internal IReadOnlyList<string> GetSections() {
                var perSectionDict = Parse();
                return new List<string>(perSectionDict.Keys).AsReadOnly();
            }

            internal IReadOnlyList<string> GetKeys(string section) {
                var perSectionDict = Parse();
                if (perSectionDict.TryGetValue(section, out var perKeyDict)) {
                    return new List<string>(perKeyDict.Keys).AsReadOnly();
                }
                return Array.Empty<string>();
            }

            internal IReadOnlyList<string> GetValue(string section, string key) {
                var perSectionDict = Parse();
                if (perSectionDict.TryGetValue(section, out var perKeyDict)) {
                    if (perKeyDict.TryGetValue(key, out var valueList)) {
                        return valueList.AsReadOnly();
                    }
                }
                return Array.Empty<string>();
            }

            internal int GetCount() {
                var _ = Parse();
                return CachedResultCount;
            }
        }

        #endregion Parsing

        #region Sections/Keys

        /// <summary>
        /// Returns all sections that have keys.
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<string> GetSections() {
            return ParserObject.GetSections();
        }

        /// <summary>
        /// Returns all keys in given section.
        /// </summary>
        /// <param name="section">Section name.</param>
        /// <exception cref="ArgumentNullException">Section cannot be null.</exception>
        public IReadOnlyList<string> GetKeys(string section) {
            if (section == null) { throw new ArgumentNullException(nameof(section), "Section cannot be null."); }
            return ParserObject.GetKeys(section);
        }

        /// <summary>
        /// Gets number of properties present.
        /// </summary>
        public int Count {
            get {
                return ParserObject.GetCount();
            }
        }

        #endregion Sections/Keys


        #region Convert

        /// <summary>
        /// Returns value if non-empty or default value otherwise.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <param name="defaultValue">Default value.</param>
        /// <exception cref="ArgumentNullException">Default value cannot be null.</exception>
        internal static string GetValue(string? value, string defaultValue) {
            if (defaultValue == null) { throw new ArgumentNullException(nameof(defaultValue), "Default value cannot be null."); }
            return value is not null ? value : defaultValue;
        }

        /// <summary>
        /// Returns boolean value if non-empty or default value if it cannot be converted.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <param name="defaultValue">Default value.</param>
        internal static bool GetValue(string? value, bool defaultValue) {
            if (value == null) { return defaultValue; }
            if (bool.TryParse(value, out var result)) {
                return result;
            } else if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var resultInt)) {
                return (resultInt != 0);
            } else {
                return defaultValue;
            }
        }

        /// <summary>
        /// Returns integer value if non-empty or default value if it cannot be converted.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <param name="defaultValue">Default value.</param>
        internal static int GetValue(string? value, int defaultValue) {
            if (value == null) { return defaultValue; }
            if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result)) {
                return result;
            } else {
                return defaultValue;
            }
        }

        /// <summary>
        /// Returns integer value if non-empty or default value if it cannot be converted.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <param name="defaultValue">Default value.</param>
        internal static long GetValue(string? value, long defaultValue) {
            if (value == null) { return defaultValue; }
            if (long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result)) {
                return result;
            } else {
                return defaultValue;
            }
        }

        /// <summary>
        /// Returns floating point value if non-empty or default value if it cannot be converted.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <param name="defaultValue">Default value.</param>
        internal static float GetValue(string? value, float defaultValue) {
            if (value == null) { return defaultValue; }
            if (float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var result)) {
                return result;
            } else {
                return defaultValue;
            }
        }

        /// <summary>
        /// Returns floating point value if non-empty or default value if it cannot be converted.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <param name="defaultValue">Default value.</param>
        internal static double GetValue(string? value, double defaultValue) {
            if (value == null) { return defaultValue; }
            if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var result)) {
                return result;
            } else {
                return defaultValue;
            }
        }

        #endregion

    }
}
