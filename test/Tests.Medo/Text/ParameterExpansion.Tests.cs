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


        [Fact(DisplayName = "ParameterExpansion: Complex expand default (1)")]
        public void ComplexExpandDefault1() {
            var shell = new ParameterExpansion();
            var output = shell.Expand("${VAR1-default}");
            Assert.Equal("default", output);
        }

        [Fact(DisplayName = "ParameterExpansion: Complex expand default (2)")]
        public void ComplexExpandDefault2() {
            var shell = new ParameterExpansion();
            shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
                e.Value = (e.Name == "VAR1") ? "nondefault" : null;
            };
            var output = shell.Expand("${VAR1-default}");
            Assert.Equal("nondefault", output);
        }

        [Fact(DisplayName = "ParameterExpansion: Complex expand default (3)")]
        public void ComplexExpandDefault3() {
            var shell = new ParameterExpansion();
            shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
                e.Value = (e.Name == "VAR1") ? "" : null;
            };
            var output = shell.Expand("${VAR1-default}");
            Assert.Equal("", output);
        }

        [Fact(DisplayName = "ParameterExpansion: Complex expand default (4)")]
        public void ComplexExpandDefault4() {
            var shell = new ParameterExpansion();
            shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
                e.Value = (e.Name == "VAR1") ? "" : null;
            };
            var output = shell.Expand("${VAR1:-default}");
            Assert.Equal("default", output);
        }

        [Fact(DisplayName = "ParameterExpansion: Complex expand default (5)")]
        public void ComplexExpandDefault5() {
            var shell = new ParameterExpansion();
            shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
                e.Value = (e.Name) switch {
                    "VAR2" => "2",
                    _ => null,
                };
            };
            var output = shell.Expand("${VAR1:-$VAR2}");
            Assert.Equal("2", output);
        }

        [Fact(DisplayName = "ParameterExpansion: Complex expand default (6)")]
        public void ComplexExpandDefault6() {
            var shell = new ParameterExpansion();
            shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
                e.Value = (e.Name) switch {
                    "VAR2" => "2",
                    _ => null,
                };
            };
            var output = shell.Expand("${VAR1:-${VAR2}}");
            Assert.Equal("2", output);
        }

        [Fact(DisplayName = "ParameterExpansion: Complex expand default (7)")]
        public void ComplexExpandDefault7() {
            var shell = new ParameterExpansion();
            var output = shell.Expand("${VAR1:-${VAR2-3}}");
            Assert.Equal("3", output);
        }

        [Fact(DisplayName = "ParameterExpansion: Complex expand default (8)")]
        public void ComplexExpandDefault8() {
            var shell = new ParameterExpansion();
            var output = shell.Expand("${VAR1:-${VAR2-3}X}");
            Assert.Equal("3X", output);
        }


        [Fact(DisplayName = "ParameterExpansion: Complex expand default with set (1)")]
        public void ComplexExpandDefaultWithSet1() {
            var shell = new ParameterExpansion();
            Assert.Equal("abc", shell.Expand("${var=abc}"));
            Assert.Equal("abc", shell.Expand("${var=xyz}"));
        }

        [Fact(DisplayName = "ParameterExpansion: Complex expand default with set (2)")]
        public void ComplexExpandDefaultWithSet2() {
            var shell = new ParameterExpansion();
            shell.Parameters.Add("var", "");
            Assert.Equal("", shell.Expand("${var=abc}"));
            Assert.Equal("", shell.Expand("${var=xyz}"));
        }

        [Fact(DisplayName = "ParameterExpansion: Complex expand default with set (3)")]
        public void ComplexExpandDefaultWithSet3() {
            var shell = new ParameterExpansion();
            shell.Parameters.Add("var", "");
            Assert.Equal("abc", shell.Expand("${var:=abc}"));
            Assert.Equal("abc", shell.Expand("${var:=xyz}"));
        }


        [Fact(DisplayName = "ParameterExpansion: Complex expand alternate (1)")]
        public void ComplexExpandAlternate1() {
            var shell = new ParameterExpansion();
            Assert.Equal("", shell.Expand("${var1+xyz}"));
        }

        [Fact(DisplayName = "ParameterExpansion: Complex expand alternate (2)")]
        public void ComplexExpandAlternate2() {
            var shell = new ParameterExpansion();
            shell.Parameters.Add("var1", "XXX");
            Assert.Equal("xyz", shell.Expand("${var1+xyz}"));
        }

        [Fact(DisplayName = "ParameterExpansion: Complex expand alternate (3)")]
        public void ComplexExpandAlternate3() {
            var shell = new ParameterExpansion();
            shell.Parameters.Add("var1", "");
            Assert.Equal("", shell.Expand("${var1:+xyz}"));
        }

        [Fact(DisplayName = "ParameterExpansion: Complex expand alternate (4)")]
        public void ComplexExpandAlternate4() {
            var shell = new ParameterExpansion();
            shell.Parameters.Add("var1", "X");
            Assert.Equal("xyz", shell.Expand("${var1:+xyz}"));
        }


        [Fact(DisplayName = "ParameterExpansion: Complex expand extra incomplete")]
        public void ComplexExpandUnfinishedExtra() {
            var shell = new ParameterExpansion();
            shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
                e.Value = (e.Name == "VAR1") ? "test" : null;
            };
            var output = shell.Expand("${VAR1:}");
            Assert.Equal("test", output);
        }

        [Fact(DisplayName = "ParameterExpansion: Environment variable")]
        public void EnvironmentVariable() {
            var shell = new ParameterExpansion();
            var output = shell.Expand("$USERNAME");
            Assert.Equal(Environment.UserName, output);
        }

        [Fact(DisplayName = "ParameterExpansion: Environment variable overwrite")]
        public void EnvironmentVariableOverwrite() {
            var shell = new ParameterExpansion();
            shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
                e.Value = (e.Name == "USERNAME") ? "--" : null;
            };
            var output = shell.Expand("$USERNAME");
            Assert.Equal("--", output);
        }

        [Fact(DisplayName = "ParameterExpansion: Environment variable disable")]
        public void EnvironmentVariableDisable() {
            var shell = new ParameterExpansion() {
                UseEnvironmentVariables = false
            };
            var output = shell.Expand("$USERNAME");
            Assert.Equal("", output);
        }

        [Fact(DisplayName = "ParameterExpansion: Indirection")]
        public void Indirection() {
            var shell = new ParameterExpansion();
            shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
                e.Value = (e.Name) switch {
                    "IND" => "VAR",
                    "VAR" => "VALUE",
                    _ => null,
                };
            };
            var output = shell.Expand("${!IND}");
            Assert.Equal("VALUE", output);
        }

        [Fact(DisplayName = "ParameterExpansion: Indirection not found")]
        public void IndirectionNotFound() {
            var shell = new ParameterExpansion();
            shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
                e.Value = (e.Name) switch {
                    "IND" => "VAR",
                    _ => null,
                };
            };
            var output = shell.Expand("${!IND}");
            Assert.Equal("", output);
        }

        [Fact(DisplayName = "ParameterExpansion: Parameter length")]
        public void ParameterLength() {
            var shell = new ParameterExpansion();
            shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
                e.Value = (e.Name) switch {
                    "VAR" => "Value",
                    _ => null,
                };
            };
            var output = shell.Expand("${#VAR}");
            Assert.Equal("5", output);
        }

        [Fact(DisplayName = "ParameterExpansion: Parameter length expanded")]
        public void ParameterLengthExpanded() {
            var shell = new ParameterExpansion();
            shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
                e.Value = (e.Name) switch {
                    "X" => "VAR",
                    "VAR" => "Value",
                    _ => null,
                };
            };
            var output = shell.Expand("${#$X}");
            Assert.Equal("5", output);
        }

    }
}
