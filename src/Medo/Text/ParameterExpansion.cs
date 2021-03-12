/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

namespace Medo.Text {
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;

    /// <summary>
    /// Performs basic shell parameter expansion.
    /// </summary>
    /// <remarks>https://www.gnu.org/software/bash/manual/html_node/Shell-Parameter-Expansion.html</remarks>
    public sealed class ParameterExpansion {

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <remarks>
        /// The following substitutions are supported:
        ///     ${parameter-word}
        ///     ${parameter=word}
        ///     ${parameter+word}
        ///     ${parameter:-word}
        ///     ${parameter:=word}
        ///     ${parameter:+word}
        ///     ${parameter:offset}
        ///     ${parameter:offset:length}
        ///     ${!parameter}
        ///     ${#parameter}
        ///     ${parameter@U}
        ///     ${parameter@u}
        ///     ${parameter@L}
        /// </remarks>
        public ParameterExpansion() {
            Parameters = new Dictionary<string, string?>();
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <exception cref="ArgumentNullException">Parameters cannot be null.</exception>
        public ParameterExpansion(IDictionary<string, string?> parameters) {
            if (parameters == null) { throw new ArgumentNullException(nameof(parameters), "Parameters cannot be null."); }
            Parameters = parameters;
        }


        /// <summary>
        /// Gets parameters dictionary.
        /// </summary>
        public IDictionary<string, string?> Parameters { get; init; }

        /// <summary>
        /// Gets/sets if environment variables are used automatically.
        /// Event RetrieveParameter will still be queried but value will be prefilled and used in absence of change.
        /// </summary>
        public bool UseEnvironmentVariables { get; set; } = true;


        /// <summary>
        /// Returns expanded text.
        /// </summary>
        /// <param name="text">Text to expand.</param>
        /// <returns></returns>
        public string Expand(string text) {
            if (text == null) { throw new ArgumentNullException(nameof(text), "Text cannot be null."); }

            var state = State.Text;
            var sbOutput = new StringBuilder();
            var sbParameterName = new StringBuilder();
            var sbParameterInstructions = new StringBuilder();
            var braceLevel = 0;

            var queue = new Queue<char?>();
            foreach (var ch in text) {
                queue.Enqueue(ch);
            }
            queue.Enqueue(null);  // marker for end of parsing

            while (queue.Count > 0) {
                var ch = queue.Dequeue();

                switch (state) {
                    case State.Text: {
                            if (ch == '$') {
                                sbParameterName.Clear();
                                state = State.ParameterStart;
                            } else {
                                sbOutput.Append(ch);
                            }
                        }
                        break;

                    case State.ParameterStart: {
                            if (ch == '{') {  // start complex parameter
                                state = State.ComplexParameter;
                                braceLevel = 1;
                            } else if (ch.HasValue && (char.IsLetterOrDigit(ch.Value) || (ch == '_'))) {  // normal variable
                                sbParameterName.Append(ch);
                                state = State.SimpleParameter;
                            } else {  // just assume it's normal text
                                sbOutput.Append('$');
                                sbOutput.Append(ch);
                                state = State.Text;
                            }
                        }
                        break;

                    case State.SimpleParameter: {
                            if (ch.HasValue && (char.IsLetterOrDigit(ch.Value) || (ch == '_'))) {  // continue as variable
                                sbParameterName.Append(ch);
                            } else {  // parameter done
                                OnRetrieveParameter(sbParameterName.ToString(), null, out var value);
                                sbOutput.Append(value);
                                sbOutput.Append(ch);
                                state = State.Text;
                            }
                        }
                        break;

                    case State.ComplexParameter: {
                            if (ch == '}') {  // parameter done
                                OnRetrieveParameter(sbParameterName.ToString(), null, out var value);
                                sbOutput.Append(value);
                                state = State.Text;
                            } else if ((sbParameterName.Length == 0) && ((ch == '!') || (ch == '#'))) { //indirection must be the first character
                                sbParameterInstructions.Clear();
                                sbParameterInstructions.Append(ch);
                                state = State.ComplexParameterWithInstructions;
                            } else if ((ch == '+') || (ch == '-') || (ch == ':') || (ch == '=') || (ch == '@')) {
                                sbParameterInstructions.Clear();
                                sbParameterInstructions.Append(ch);
                                state = State.ComplexParameterWithInstructions;
                            } else {
                                sbParameterName.Append(ch);
                            }
                        }
                        break;

                    case State.ComplexParameterWithInstructions: {
                            if ((ch == '}') && (braceLevel == 1)) {  // parameter done
                                var expander = new ParameterExpansion(Parameters);
                                expander.RetrieveParameter += delegate (object? sender, ParameterExpansionEventArgs e) {
                                    RetrieveParameter?.Invoke(this, e);
                                };
                                var instructions = expander.Expand(sbParameterInstructions.ToString());
                                var parameterName = sbParameterName.ToString();

                                if (string.IsNullOrEmpty(parameterName) && instructions.StartsWith("!")) {  // ${!parameter} indirect
                                    var parameterNameQuery = instructions[1..];
                                    OnRetrieveParameter(parameterNameQuery, null, out var indirectParameterName);
                                    if (!string.IsNullOrEmpty(indirectParameterName)) {
                                        OnRetrieveParameter(indirectParameterName, null, out var value);
                                        sbOutput.Append(value);
                                    }
                                } else if (string.IsNullOrEmpty(parameterName) && instructions.StartsWith("#")) {  // ${#parameter} parameter length
                                    var innerParameterName = instructions[1..];
                                    OnRetrieveParameter(innerParameterName, null, out var value);
                                    if (string.IsNullOrEmpty(value)) { value = ""; }
                                    sbOutput.Append(value.Length.ToString(CultureInfo.InvariantCulture));
                                } else if (instructions.StartsWith(":+")) {  // ${parameter:+word} use alternate value even if empty
                                    var alternateValue = instructions[2..];
                                    OnRetrieveParameter(parameterName, null, out var value);
                                    if (!string.IsNullOrEmpty(value)) {
                                        sbOutput.Append(alternateValue);
                                    }
                                } else if (instructions.StartsWith("+")) {  // ${parameter+word} use alternate value
                                    var alternateValue = instructions[1..];
                                    OnRetrieveParameter(parameterName, null, out var value);
                                    if (value != null) {
                                        sbOutput.Append(alternateValue);
                                    }
                                } else if (instructions.StartsWith(":-")) {  // ${parameter:-word} use default even if empty
                                    var defaultValue = instructions[2..];
                                    OnRetrieveParameter(parameterName, defaultValue, out var value);
                                    if (string.IsNullOrEmpty(value)) { value = defaultValue; }  // also replace if it's empty
                                    sbOutput.Append(value);
                                } else if (instructions.StartsWith("-")) {  // ${parameter-word} use default
                                    var defaultValue = instructions[1..];
                                    OnRetrieveParameter(parameterName, defaultValue, out var value);
                                    sbOutput.Append(value);
                                } else if (instructions.StartsWith(":=")) {  // ${parameter:=word} use default and set variable even if empty
                                    var defaultValue = instructions[2..];
                                    OnRetrieveParameter(parameterName, defaultValue, out var value);
                                    if (string.IsNullOrEmpty(value)) { value = defaultValue; }  // also replace if it's empty
                                    sbOutput.Append(value);
                                    Parameters[parameterName] = value;
                                } else if (instructions.StartsWith("=")) {  // ${parameter=word} use default and set variable
                                    var defaultValue = instructions[1..];
                                    OnRetrieveParameter(parameterName, defaultValue, out var value);
                                    sbOutput.Append(value);
                                    Parameters[parameterName] = value;
                                } else if (instructions.StartsWith(":")) {  // ${parameter:offset:length} substring
                                    OnRetrieveParameter(parameterName, null, out var value);
                                    if (value != null) {
                                        var newValue = GetSubstring(value, instructions[1..]);
                                        if (newValue != null) { sbOutput.Append(newValue); }
                                    }
                                } else if (instructions.StartsWith("@")) {  // ${parameter@operator} perform modifications
                                    var oper = instructions[1..];
                                    OnRetrieveParameter(parameterName, null, out var value);
                                    if (value != null) {
                                        var newValue = (oper) switch {
                                            "U" => value.ToUpperInvariant(),
                                            "u" => value[0..1].ToUpperInvariant() + value[1..],
                                            "L" => value.ToLowerInvariant(),
                                            _ => value,
                                        };
                                        sbOutput.Append(newValue);
                                    }
                                } else {
                                    OnRetrieveParameter(parameterName, null, out var value);
                                    sbOutput.Append(value);
                                }

                                state = State.Text;
                            } else {
                                sbParameterInstructions.Append(ch);
                                if (ch == '{') {
                                    braceLevel += 1;
                                } else if (ch == '}') {
                                    braceLevel -= 1;
                                }
                            }
                        }
                        break;

                }
            }

            return sbOutput.ToString();
        }


        private static string? GetSubstring(string value, string arguments) {
            var parts = arguments.Split(':', StringSplitOptions.None);
            string? startText = (parts.Length >= 1) && (parts[0].Length > 0) ? parts[0] : null;
            string? lengthText = (parts.Length >= 2) && (parts[1].Length > 0) ? parts[1] : null;

            int start, length;
            if (startText == null) {  // output whole thing
                return value;
            } else {
                if (!int.TryParse(startText, NumberStyles.Integer, CultureInfo.InvariantCulture, out start)) { return null; }
                if (start < 0) { start = value.Length + start; }
                if ((start < 0) || (start > value.Length)) { return null; }
                if (lengthText == null) {
                    length = value.Length - start;
                } else {
                    if (!int.TryParse(lengthText, NumberStyles.Integer, CultureInfo.InvariantCulture, out length)) { return null; }
                    if (length < 0) { length = value.Length - start + length; }
                    if (length < 0) { return null; }
                    if (start + length > value.Length) { length = value.Length - start; }
                }
                return value.Substring(start, length);
            }
        }


        /// <summary>
        /// Event raised to gather parameter values.
        /// </summary>
        public event EventHandler<ParameterExpansionEventArgs>? RetrieveParameter;


        #region State

        private enum State {
            Text,
            ParameterStart,
            SimpleParameter,
            ComplexParameter,
            ComplexParameterWithInstructions
        }

        private void OnRetrieveParameter(string name, string? defaultValue, out string? value) {
            if (!Parameters.TryGetValue(name, out value)) {
                if (UseEnvironmentVariables) {
                    var envValue = Environment.GetEnvironmentVariable(name);
                    if (envValue != null) {
                        defaultValue = envValue;
                    }
                }
                var e = new ParameterExpansionEventArgs(name, defaultValue);
                RetrieveParameter?.Invoke(this, e);
                value = e.Value;
                Parameters.Add(name, value);
            }
        }

        #endregion State

    }



    /// <summary>
    /// Event arguments for parameter expansion.
    /// </summary>
    public class ParameterExpansionEventArgs : EventArgs {

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="parameterName">Parameter name.</param>
        /// <exception cref="ArgumentNullException">Parameter name cannot be null.</exception>
        public ParameterExpansionEventArgs(string parameterName)
            : this(parameterName, null) {
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="defaultValue">Parameter value.</param>
        /// <exception cref="ArgumentNullException">Parameter name cannot be null.</exception>
        public ParameterExpansionEventArgs(string parameterName, string? defaultValue) {
            if (parameterName == null) { throw new ArgumentNullException(nameof(parameterName), "Parameter name cannot be null."); }
            Name = parameterName;
            Value = defaultValue;
        }

        /// <summary>
        /// Gets parameter name.
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        /// Gets/sets parameter value.
        /// If null, parameter will be assumed missing.
        /// </summary>
        public string? Value { get; set; }

    }
}
