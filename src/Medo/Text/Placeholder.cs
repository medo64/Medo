/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

//2021-11-25: Refactored to use pattern matching
//2021-10-05: Refactored for .NET 5
//2013-03-11: Nulls are supported
//2013-03-08: Bug-fixing
//2013-03-04: Initial version

namespace Medo.Text {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Text;

    /// <summary>
    /// Composite formatting based on placeholder name.
    /// </summary>
    public static class Placeholder {

        /// <summary>
        /// Replaces one or more format items in a specified string with the string representation of a specified object.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="items">Replacement items.</param>
        /// <exception cref="System.ArgumentNullException">Format string cannot be null.</exception>
        public static string Format(string format, IDictionary<string, object> items) {
            return Format(CultureInfo.CurrentCulture, format, items);
        }

        /// <summary>
        /// Replaces one or more format items in a specified string with the string representation of a specified object.
        /// </summary>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="items">Replacement items.</param>
        /// <exception cref="System.ArgumentNullException">Provider cannot be null. -or- Format string cannot be null.</exception>
        /// <exception cref="System.ArgumentException">Invalid closing brace.</exception>
        public static string Format(IFormatProvider provider, string format, IDictionary<string, object> items) {
            if (provider == null) { throw new ArgumentNullException(nameof(provider), "Provider cannot be null."); }
            if (format == null) { throw new ArgumentNullException(nameof(format), "Format string cannot be null."); }

            var sbFormat = new StringBuilder();
            var sbArgName = new StringBuilder();
            var sbArgFormat = new StringBuilder();
            var argIndex = 0;
            var args = new List<object?>();

            var state = State.Default;
            foreach (var ch in format) {
                switch (state) {
                    case State.Default: {
                            if (ch == '{') {
                                state = State.LeftBrace;
                            } else if (ch == '}') {
                                state = State.RightBrace;
                            } else {
                                sbFormat.Append(ch);
                            }
                        }
                        break;

                    case State.LeftBrace: {
                            if (ch == '{') {
                                sbFormat.Append("{{");
                                state = State.Default;
                            } else {
                                sbArgName.Append(ch);
                                state = State.ArgName;
                            }
                        }
                        break;

                    case State.RightBrace: {
                            if (ch == '}') {
                                sbFormat.Append("}}");
                                state = State.Default;
                            } else {
                                throw new ArgumentException("Invalid closing brace.", nameof(format));
                            }
                        }
                        break;

                    case State.ArgName: {
                            if (ch == '}') {
                                var argName = sbArgName.ToString();
                                var arg = GetArg(items, argName);
                                args.Add(arg);
                                sbFormat.AppendFormat(CultureInfo.InvariantCulture, "{{{0}}}", argIndex);
                                argIndex += 1;
                                sbArgName.Length = 0;
                                state = State.Default;
                            } else if (ch == ':') {
                                state = State.ArgFormat;
                            } else {
                                sbArgName.Append(ch);
                            }
                        }
                        break;

                    case State.ArgFormat: {
                            if (ch == '}') {
                                var argName = sbArgName.ToString();
                                args.Add(GetArg(items, argName));
                                var argFormat = sbArgFormat.ToString();
                                sbFormat.AppendFormat(CultureInfo.InvariantCulture, "{{{0}:{1}}}", argIndex, argFormat);
                                argIndex += 1;
                                sbArgName.Length = 0;
                                sbArgFormat.Length = 0;
                                state = State.Default;
                            } else {
                                sbArgFormat.Append(ch);
                            }
                        }
                        break;

                    default: Trace.Fail("Unknown state (" + state.ToString() + ")."); break;
                }
            }

            return string.Format(provider, sbFormat.ToString(), args.ToArray());
        }


        /// <summary>
        /// Replaces one or more format items in a specified string with the string representation of a specified object.
        /// </summary>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="namesAndValues">Names and values interleaved together.</param>
        /// <exception cref="System.ArgumentNullException">Provider cannot be null. -or- Format string cannot be null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">There must be even number of names and values. -or- Name must be a string.</exception>
        public static string Format(IFormatProvider provider, string format, params object[] namesAndValues) {
            var items = new Dictionary<string, object>();
            if (namesAndValues != null) {
                if ((namesAndValues.Length % 2) != 0) { throw new ArgumentOutOfRangeException(nameof(namesAndValues), "There must be even number of names and values."); }
                for (int i = 0; i < namesAndValues.Length; i += 2) {
                    if (namesAndValues[i] is not string name) { throw new ArgumentOutOfRangeException(nameof(namesAndValues), "Name must be a string."); }
                    var value = namesAndValues[i + 1];
                    items.Add(name, value);
                }
            }
            return Format(provider, format, items);
        }

        /// <summary>
        /// Replaces one or more format items in a specified string with the string representation of a specified object.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="namesAndValues">Names and values interleaved together.</param>
        /// <exception cref="System.ArgumentNullException">Provider cannot be null. -or- Format string cannot be null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">There must be even number of names and values. -or- Name must be a string.</exception>
        public static string Format(string format, params object[] namesAndValues) {
            return Format(CultureInfo.CurrentCulture, format, namesAndValues);
        }


        private static object? GetArg(IDictionary<string, object> items, string argumentName) {
            if (items.TryGetValue(argumentName, out var value)) {
                return value;
            } else {
                return null;
            }
        }


        private enum State {
            Default,
            LeftBrace,
            RightBrace,
            ArgName,
            ArgFormat,
        }

    }
}
