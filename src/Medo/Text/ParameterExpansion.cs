/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

namespace Medo.Text {
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Performs basic shell parameter expansion.
    /// </summary>
    /// <remarks>https://www.gnu.org/software/bash/manual/html_node/Shell-Parameter-Expansion.html</remarks>
    public sealed class ParameterExpansion {

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public ParameterExpansion() {
        }


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
                            } else if ((ch == ':') || (ch == '-')) {
                                sbParameterInstructions.Clear();
                                sbParameterInstructions.Append(ch);
                                state = State.ComplexParameterWithInstructions;
                            } else {
                                sbParameterName.Append(ch);
                            }
                        }
                        break;

                    case State.ComplexParameterWithInstructions: {
                            if (ch == '}') {  // parameter done
                                var instructions = sbParameterInstructions.ToString();
                                if (instructions.StartsWith(":-")) {
                                    var defaultValue = instructions[2..];
                                    OnRetrieveParameter(sbParameterName.ToString(), defaultValue, out var value);
                                    if (string.IsNullOrEmpty(value)) { value = defaultValue; }
                                    sbOutput.Append(value);
                                } else if (instructions.StartsWith("-")) {
                                    var defaultValue = instructions[1..];
                                    OnRetrieveParameter(sbParameterName.ToString(), defaultValue, out var value);
                                    sbOutput.Append(value);
                                } else {
                                    OnRetrieveParameter(sbParameterName.ToString(), null, out var value);
                                    sbOutput.Append(value);
                                }

                                state = State.Text;
                            } else {
                                sbParameterInstructions.Append(ch);
                            }
                        }
                        break;

                }
            }

            return sbOutput.ToString();
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

        private Dictionary<string, string?> RetrievedParameters = new();

        private void OnRetrieveParameter(string name, string? defaultValue, out string? value) {
            if (!RetrievedParameters.TryGetValue(name, out value)) {
                var e = new ParameterExpansionEventArgs(name, defaultValue);
                RetrieveParameter?.Invoke(this, e);
                value = e.Value;
                RetrievedParameters.Add(name, value);
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
