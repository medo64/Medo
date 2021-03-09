using System;
using Xunit;

namespace Medo.Tests.Text.ParameterExpansion {
    using Medo.Text;

    public class Tests {

        [Fact(DisplayName = "ParameterExpansion: Empty")]
        public void Empty() {
            var shell = new ParameterExpansion();
            var output = shell.Expand("");
            Assert.Equal("", output);
        }

        [Fact(DisplayName = "ParameterExpansion: Only text")]
        public void OnlyText() {
            var shell = new ParameterExpansion();
            var output = shell.Expand("Test");
            Assert.Equal("Test", output);
        }

        [Fact(DisplayName = "ParameterExpansion: Lone dollar (1)")]
        public void LoneDollar1() {
            var shell = new ParameterExpansion();
            var output = shell.Expand("$");
            Assert.Equal("$", output);
        }

        [Fact(DisplayName = "ParameterExpansion: Lone dollar (2)")]
        public void LoneDollar2() {
            var shell = new ParameterExpansion();
            var output = shell.Expand("#$-");
            Assert.Equal("#$-", output);
        }

        [Fact(DisplayName = "ParameterExpansion: Cannot expand (1)")]
        public void CannotExpand1() {
            var shell = new ParameterExpansion();
            var output = shell.Expand("$X");
            Assert.Equal("", output);
        }

        [Fact(DisplayName = "ParameterExpansion: Cannot expand (2)")]
        public void CannotExpand() {
            var shell = new ParameterExpansion();
            var output = shell.Expand("-$X-");
            Assert.Equal("--", output);
        }

        [Fact(DisplayName = "ParameterExpansion: Simple expand (1)")]
        public void SimpleExpand() {
            var shell = new ParameterExpansion();
            shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
                e.Value = (e.Name == "X") ? "Y" : null;
            };
            var output = shell.Expand("$X");
            Assert.Equal("Y", output);
        }

        [Fact(DisplayName = "ParameterExpansion: Simple expand (2)")]
        public void SimpleExpand2() {
            var shell = new ParameterExpansion();
            shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
                e.Value = (e.Name == "X") ? "Y" : null;
            };
            var output = shell.Expand("-$X-");
            Assert.Equal("-Y-", output);
        }

        [Fact(DisplayName = "ParameterExpansion: Special chars")]
        public void SpecialChars() {
            var shell = new ParameterExpansion();
            shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
                e.Value = (e.Name == "X") ? "\n" : null;
            };
            var output = shell.Expand("\t$X\r\0");
            Assert.Equal("\t\n\r\0", output);
        }

        [Fact(DisplayName = "ParameterExpansion: Complex expand (1)")]
        public void ComplexExpand1() {
            var shell = new ParameterExpansion();
            shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
                e.Value = (e.Name == "X") ? "Y" : null;
            };
            var output = shell.Expand("${X}");
            Assert.Equal("Y", output);
        }

        [Fact(DisplayName = "ParameterExpansion: Complex expand (2)")]
        public void ComplexExpand2() {
            var shell = new ParameterExpansion();
            shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
                e.Value = (e.Name == "X") ? "Y" : null;
            };
            var output = shell.Expand("A${X}B");
            Assert.Equal("AYB", output);
        }

        [Fact(DisplayName = "ParameterExpansion: Expand variable gets cached")]
        public void ExpandVariableGetsCached() {
            var retrieveCount = 0;
            var shell = new ParameterExpansion();
            shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
                e.Value = (e.Name == "X") ? "Y" : null;
                retrieveCount += 1;
            };
            var output = shell.Expand("A${X}${X}B");
            Assert.Equal("AYYB", output);
            Assert.Equal(1, retrieveCount);
        }

        [Fact(DisplayName = "ParameterExpansion: Incomplete complex expand")]
        public void IncompleteComplexExpand() {
            var shell = new ParameterExpansion();
            shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
                e.Value = (e.Name == "X") ? "Y" : null;
            };
            var output = shell.Expand("A${X");
            Assert.Equal("A", output);
        }

    }
}
