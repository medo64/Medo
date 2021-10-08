using System;
using System.IO;
using System.Reflection;
using Xunit;

namespace Medo.Tests.Text.Placeholder {
    using Medo.Text;
    using System.Collections.Generic;
    using System.Globalization;

    public class Tests {

        [Fact(DisplayName = "Placeholder: None")]
        public void None() {
            Assert.Equal("Test", Placeholder.Format(CultureInfo.InvariantCulture, "Test"));
        }

        [Fact(DisplayName = "Placeholder: LeftBraceEscape")]
        public void LeftBraceEscape() {
            Assert.Equal("{", Placeholder.Format(CultureInfo.InvariantCulture, "{{"));
        }

        [Fact(DisplayName = "Placeholder: RightBraceEscape")]
        public void RightBraceEscape() {
            Assert.Equal("}", Placeholder.Format(CultureInfo.InvariantCulture, "}}"));
        }

        [Fact(DisplayName = "Placeholder: BothBraceEscape")]
        public void BothBraceEscape() {
            Assert.Equal("{}", Placeholder.Format(CultureInfo.InvariantCulture, "{{}}"));
        }


        [Fact(DisplayName = "Placeholder: String")]
        public void String() {
            var dict = new Dictionary<string, object> {
                { "Text", "Text" }
            };
            Assert.Equal("Test: Text.", Placeholder.Format(CultureInfo.InvariantCulture, "Test: {Text}.", dict));
        }

        [Fact(DisplayName = "Placeholder: MultipleStrings")]
        public void MultipleStrings() {
            var dict = new Dictionary<string, object> {
                { "Number", "42" },
                { "Text", "Fortytwo" }
            };
            Assert.Equal("Test: 42 (Fortytwo).", Placeholder.Format(CultureInfo.InvariantCulture, "Test: {Number} ({Text}).", dict));
        }

        [Fact(DisplayName = "Placeholder: MultipleStrings (2)")]
        public void MultipleStrings2() {
            Assert.Equal("Test: 42 (Fortytwo).", Placeholder.Format(CultureInfo.InvariantCulture, "Test: {Number} ({Text}).", "Number", 42, "Text", "Fortytwo"));
        }

        [Fact(DisplayName = "Placeholder: DoubleString")]
        public void DoubleString() {
            var dict = new Dictionary<string, object> {
                { "Text", "Text" }
            };
            Assert.Equal("Test: Text, Text.", Placeholder.Format(CultureInfo.InvariantCulture, "Test: {Text}, {Text}.", dict));
        }

        [Fact(DisplayName = "Placeholder: Integer")]
        public void Integer() {
            var dict = new Dictionary<string, object> {
                { "Number", 42 }
            };
            Assert.Equal("Test: 42.", Placeholder.Format(CultureInfo.InvariantCulture, "Test: {Number}.", dict));
        }

        [Fact(DisplayName = "Placeholder: IntegerWithFormat")]
        public void IntegerWithFormat() {
            var dict = new Dictionary<string, object> {
                { "Number", 42 }
            };
            Assert.Equal("Test: 42.0.", Placeholder.Format(CultureInfo.InvariantCulture, "Test: {Number:0.0}.", dict));
        }

        [Fact(DisplayName = "Placeholder: EnclosedDouble")]
        public void EnclosedDouble() {
            var dict = new Dictionary<string, object> {
                { "Number", 42.0 }
            };
            Assert.Equal("Test: {42.0}.", Placeholder.Format(CultureInfo.InvariantCulture, "Test: {{{Number:0.0}}}.", dict));
        }

        [Fact(DisplayName = "Placeholder: DateTime")]
        public void DateTime() {
            var dict = new Dictionary<string, object> {
                { "Time", new DateTime(1979, 01, 28, 18, 15, 0) }
            };
            Assert.Equal("Test: 19790128T181500.", Placeholder.Format(CultureInfo.InvariantCulture, "Test: {Time:yyyyMMdd'T'HHmmss}.", dict));
        }


        [Fact(DisplayName = "Placeholder: MultipleFormats")]
        public void MultipleFormats() {
            Assert.Equal("Test: 42 (Fortytwo).", Placeholder.Format(CultureInfo.InvariantCulture, "Test: {Number:0} ({Text}).", "Number", "42", "Text", "Fortytwo"));
        }

        [Fact(DisplayName = "Placeholder: Nulls")]
        public void Nulls() {
            Assert.Equal("Test:  ().", Placeholder.Format(CultureInfo.InvariantCulture, "Test: {Number:0} ({Text}).", "Number", null, "Text", null));
        }


        [Fact(DisplayName = "Placeholder: DoesNotExist")]
        public void DoesNotExist() {
            var dict = new Dictionary<string, object> {
                { "Text", "Text" }
            };
            Assert.Equal("Test: .", Placeholder.Format(CultureInfo.InvariantCulture, "Test: {Unknown}.", dict));
        }

    }
}
