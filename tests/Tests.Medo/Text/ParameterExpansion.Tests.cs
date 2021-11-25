using System;
using Xunit;
using Medo.Text;
using System.Globalization;

namespace Tests.Medo.Text {
    public class ParameterExpansionTests {

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

        [Fact(DisplayName = "ParameterExpansion: Lone dollar (3)")]
        public void LoneDollar3() {
            var shell = new ParameterExpansion();
            var output = shell.Expand("$X$");
            Assert.Equal("$", output);
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

        [Fact(DisplayName = "ParameterExpansion: Simple expand (3)")]
        public void SimpleExpand3() {
            var shell = new ParameterExpansion();
            shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
                e.Value = e.Name.ToLowerInvariant();
            };
            var output = shell.Expand("$X$Y$Z");
            Assert.Equal("xyz", output);
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


        [Fact(DisplayName = "ParameterExpansion: Operator uppercase")]
        public void OperatorUppercase() {
            var shell = new ParameterExpansion();
            shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
                e.Value = (e.Name) switch {
                    "X" => "valuE",
                    _ => null,
                };
            };
            var output = shell.Expand("${X@U}");
            Assert.Equal("VALUE", output);
        }

        [Fact(DisplayName = "ParameterExpansion: Operator titlecase")]
        public void OperatorTitlecase() {
            var shell = new ParameterExpansion();
            shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
                e.Value = (e.Name) switch {
                    "X" => "valuE",
                    _ => null,
                };
            };
            var output = shell.Expand("${X@u}");
            Assert.Equal("ValuE", output);
        }

        [Fact(DisplayName = "ParameterExpansion: Operator lowercase")]
        public void OperatorLowercase() {
            var shell = new ParameterExpansion();
            shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
                e.Value = (e.Name) switch {
                    "X" => "valuE",
                    _ => null,
                };
            };
            var output = shell.Expand("${X@L}");
            Assert.Equal("value", output);
        }


        [Fact(DisplayName = "ParameterExpansion: Substring (1)")]
        public void Substring1() {
            var shell = new ParameterExpansion();
            shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
                e.Value = (e.Name) switch {
                    "string" => "01234567890abcdefgh",
                    _ => null,
                };
            };
            var output = shell.Expand("${string:7}");
            Assert.Equal("7890abcdefgh", output);
        }

        [Fact(DisplayName = "ParameterExpansion: Substring (2)")]
        public void Substring2() {
            var shell = new ParameterExpansion();
            shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
                e.Value = (e.Name) switch {
                    "string" => "01234567890abcdefgh",
                    _ => null,
                };
            };
            var output = shell.Expand("${string:7:0}");
            Assert.Equal("", output);
        }

        [Fact(DisplayName = "ParameterExpansion: Substring (3)")]
        public void Substring3() {
            var shell = new ParameterExpansion();
            shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
                e.Value = (e.Name) switch {
                    "string" => "01234567890abcdefgh",
                    _ => null,
                };
            };
            var output = shell.Expand("${string:7:2}");
            Assert.Equal("78", output);
        }

        [Fact(DisplayName = "ParameterExpansion: Substring (4)")]
        public void Substring4() {
            var shell = new ParameterExpansion();
            shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
                e.Value = (e.Name) switch {
                    "string" => "01234567890abcdefgh",
                    _ => null,
                };
            };
            var output = shell.Expand("${string:7:-2}");
            Assert.Equal("7890abcdef", output);
        }

        [Fact(DisplayName = "ParameterExpansion: Substring (5)")]
        public void Substring5() {
            var shell = new ParameterExpansion();
            shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
                e.Value = (e.Name) switch {
                    "string" => "01234567890abcdefgh",
                    _ => null,
                };
            };
            var output = shell.Expand("${string: -7}");
            Assert.Equal("bcdefgh", output);
        }

        [Fact(DisplayName = "ParameterExpansion: Substring (6)")]
        public void Substring6() {
            var shell = new ParameterExpansion();
            shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
                e.Value = (e.Name) switch {
                    "string" => "01234567890abcdefgh",
                    _ => null,
                };
            };
            var output = shell.Expand("${string: -7:0}");
            Assert.Equal("", output);
        }

        [Fact(DisplayName = "ParameterExpansion: Substring (7)")]
        public void Substring7() {
            var shell = new ParameterExpansion();
            shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
                e.Value = (e.Name) switch {
                    "string" => "01234567890abcdefgh",
                    _ => null,
                };
            };
            var output = shell.Expand("${string: -7:2}");
            Assert.Equal("bc", output);
        }

        [Fact(DisplayName = "ParameterExpansion: Substring (8)")]
        public void Substring8() {
            var shell = new ParameterExpansion();
            shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
                e.Value = (e.Name) switch {
                    "string" => "01234567890abcdefgh",
                    _ => null,
                };
            };
            var output = shell.Expand("${string: -7:-2}");
            Assert.Equal("bcdef", output);
        }

        [Fact(DisplayName = "ParameterExpansion: Substring (9)")]
        public void Substring9() {
            var shell = new ParameterExpansion();
            shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
                e.Value = (e.Name) switch {
                    "string" => "01234567890abcdefgh",
                    _ => null,
                };
            };
            var output = shell.Expand("${string:18}");
            Assert.Equal("h", output);
        }

        [Fact(DisplayName = "ParameterExpansion: Substring out of range (1)")]
        public void SubstringOutOfRange1() {
            var shell = new ParameterExpansion();
            shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
                e.Value = (e.Name) switch {
                    "string" => "",
                    _ => null,
                };
            };
            var output = shell.Expand("${string:1}");
            Assert.Equal("", output);
        }

        [Fact(DisplayName = "ParameterExpansion: Substring out of range (2)")]
        public void SubstringOutOfRange2() {
            var shell = new ParameterExpansion();
            shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
                e.Value = (e.Name) switch {
                    "string" => "",
                    _ => null,
                };
            };
            var output = shell.Expand("${string: -1}");
            Assert.Equal("", output);
        }

        [Fact(DisplayName = "ParameterExpansion: Substring out of range (3)")]
        public void SubstringOutOfRange3() {
            var shell = new ParameterExpansion();
            shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
                e.Value = (e.Name) switch {
                    "string" => "AB",
                    _ => null,
                };
            };
            var output = shell.Expand("${string:0:-3}");
            Assert.Equal("", output);
        }

        [Fact(DisplayName = "ParameterExpansion: Substring out of range (4)")]
        public void SubstringOutOfRange4() {
            var shell = new ParameterExpansion();
            shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
                e.Value = (e.Name) switch {
                    "string" => "AB",
                    _ => null,
                };
            };
            var output = shell.Expand("${string:0:3}");
            Assert.Equal("AB", output);
        }


        [Fact(DisplayName = "ParameterExpansion: Uppercase (1)")]
        public void Uppercase1() {
            var shell = new ParameterExpansion();
            shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
                e.Value = (e.Name) switch {
                    "X" => "testing",
                    _ => null,
                };
            };
            var output = shell.Expand("${X^}");
            Assert.Equal("Testing", output);
        }

        [Fact(DisplayName = "ParameterExpansion: Uppercase (2)")]
        public void Uppercase2() {
            var shell = new ParameterExpansion();
            shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
                e.Value = (e.Name) switch {
                    "X" => "testing",
                    _ => null,
                };
            };
            var output = shell.Expand("${X^^}");
            Assert.Equal("TESTING", output);
        }


        [Fact(DisplayName = "ParameterExpansion: Lowercase (1)")]
        public void Lowercase1() {
            var shell = new ParameterExpansion();
            shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
                e.Value = (e.Name) switch {
                    "X" => "TESTING",
                    _ => null,
                };
            };
            var output = shell.Expand("${X,}");
            Assert.Equal("tESTING", output);
        }

        [Fact(DisplayName = "ParameterExpansion: Lowercase (2)")]
        public void Lowercase2() {
            var shell = new ParameterExpansion();
            shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
                e.Value = (e.Name) switch {
                    "X" => "TESTING",
                    _ => null,
                };
            };
            var output = shell.Expand("${X,,}");
            Assert.Equal("testing", output);
        }


        [Fact(DisplayName = "ParameterExpansion: With AutoAddParameters")]
        public void WithAutoAdd() {
            var retrieveCount = 0;
            var shell = new ParameterExpansion() { AutoAddParameters = true };
            shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
                retrieveCount += 1;
                e.Value = (e.Name) switch {
                    "X" => retrieveCount.ToString(CultureInfo.InvariantCulture),
                    _ => null,
                };
            };
            var output = shell.Expand("${X}${X}${X}");
            Assert.Equal("111", output);
            Assert.Equal(1, retrieveCount);
        }

        [Fact(DisplayName = "ParameterExpansion: Without AutoAddParameters")]
        public void WithoutAutoAdd() {
            var retrieveCount = 0;
            var shell = new ParameterExpansion() { AutoAddParameters = false };
            shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
                retrieveCount += 1;
                e.Value = (e.Name) switch {
                    "X" => retrieveCount.ToString(CultureInfo.InvariantCulture),
                    _ => null,
                };
            };
            var output = shell.Expand("${X}${X}${X}");
            Assert.Equal("123", output);
            Assert.Equal(3, retrieveCount);
        }

        [Fact(DisplayName = "ParameterExpansion: Indirect with AutoAddParameters")]
        public void IndirectWithAutoAdd() {
            var retrieveCount = 0;
            var shell = new ParameterExpansion() { AutoAddParameters = true };
            shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
                retrieveCount += 1;
                e.Value = (e.Name) switch {
                    "X" => retrieveCount.ToString(CultureInfo.InvariantCulture),
                    "Y" => "X",
                    "Z" => "Z",
                    _ => null,
                };
            };
            var output = shell.Expand("${X}${X}${X}${!Y}${X}${!Y}${Z}");
            Assert.Equal("111111Z", output);
            Assert.Equal(3, retrieveCount);
        }

        [Fact(DisplayName = "ParameterExpansion: Indirect without AutoAddParameters")]
        public void IndirectWithoutAutoAdd() {
            var retrieveCount = 0;
            var shell = new ParameterExpansion() { AutoAddParameters = false };
            shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
                retrieveCount += 1;
                e.Value = (e.Name) switch {
                    "X" => retrieveCount.ToString(CultureInfo.InvariantCulture),
                    "Y" => "X",
                    "Z" => "Z",
                    _ => null,
                };
            };
            var output = shell.Expand("${X}${X}${X}${!Y}${X}${!Y}${Z}");
            Assert.Equal("123568Z", output);
            Assert.Equal(9, retrieveCount);
        }

        [Fact(DisplayName = "ParameterExpansion: Complex with AutoAddParameters")]
        public void ComplexWithAutoAdd() {
            var retrieveCount = 0;
            var shell = new ParameterExpansion() { AutoAddParameters = true };
            shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
                retrieveCount += 1;
                e.Value = (e.Name) switch {
                    "X" => retrieveCount.ToString(CultureInfo.InvariantCulture),
                    "Z" => "Z",
                    _ => null,
                };
            };
            var output = shell.Expand("${UPS1:-${X}}${UPS2:-${X}}$X$X$Z");
            Assert.Equal("1111Z", output);
            Assert.Equal(4, retrieveCount);
        }

        [Fact(DisplayName = "ParameterExpansion: Complex without AutoAddParameters")]
        public void ComplexWithoutAutoAdd() {
            var retrieveCount = 0;
            var shell = new ParameterExpansion() { AutoAddParameters = false };
            shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
                retrieveCount += 1;
                e.Value = (e.Name) switch {
                    "X" => retrieveCount.ToString(CultureInfo.InvariantCulture),
                    "Z" => "Z",
                    _ => null,
                };
            };
            var output = shell.Expand("${UPS1:-${X}}${UPS2:-${X}}$X$X$Z");
            Assert.Equal("1356Z", output);
            Assert.Equal(7, retrieveCount);
        }

    }
}
