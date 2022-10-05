using System;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Medo.Text;

namespace Tests;

[TestClass]
public class ParameterExpansion_Tests {

    [TestMethod]
    public void CsvTextOutput_Empty() {
        var shell = new ParameterExpansion();
        var output = shell.Expand("");
        Assert.AreEqual("", output);
    }

    [TestMethod]
    public void CsvTextOutput_OnlyText() {
        var shell = new ParameterExpansion();
        var output = shell.Expand("Test");
        Assert.AreEqual("Test", output);
    }

    [TestMethod]
    public void CsvTextOutput_LoneDollar1() {
        var shell = new ParameterExpansion();
        var output = shell.Expand("$");
        Assert.AreEqual("$", output);
    }

    [TestMethod]
    public void CsvTextOutput_LoneDollar2() {
        var shell = new ParameterExpansion();
        var output = shell.Expand("#$-");
        Assert.AreEqual("#$-", output);
    }

    [TestMethod]
    public void CsvTextOutput_LoneDollar3() {
        var shell = new ParameterExpansion();
        var output = shell.Expand("$X$");
        Assert.AreEqual("$", output);
    }

    [TestMethod]
    public void CsvTextOutput_CannotExpand1() {
        var shell = new ParameterExpansion();
        var output = shell.Expand("$X");
        Assert.AreEqual("", output);
    }

    [TestMethod]
    public void CsvTextOutput_CannotExpand() {
        var shell = new ParameterExpansion();
        var output = shell.Expand("-$X-");
        Assert.AreEqual("--", output);
    }

    [TestMethod]
    public void CsvTextOutput_SimpleExpand() {
        var shell = new ParameterExpansion();
        shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
            e.Value = (e.Name == "X") ? "Y" : null;
        };
        var output = shell.Expand("$X");
        Assert.AreEqual("Y", output);
    }

    [TestMethod]
    public void CsvTextOutput_SimpleExpand2() {
        var shell = new ParameterExpansion();
        shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
            e.Value = (e.Name == "X") ? "Y" : null;
        };
        var output = shell.Expand("-$X-");
        Assert.AreEqual("-Y-", output);
    }

    [TestMethod]
    public void CsvTextOutput_SimpleExpand3() {
        var shell = new ParameterExpansion();
        shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
            e.Value = e.Name.ToLowerInvariant();
        };
        var output = shell.Expand("$X$Y$Z");
        Assert.AreEqual("xyz", output);
    }

    [TestMethod]
    public void CsvTextOutput_SpecialChars() {
        var shell = new ParameterExpansion();
        shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
            e.Value = (e.Name == "X") ? "\n" : null;
        };
        var output = shell.Expand("\t$X\r\0");
        Assert.AreEqual("\t\n\r\0", output);
    }

    [TestMethod]
    public void CsvTextOutput_ComplexExpand1() {
        var shell = new ParameterExpansion();
        shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
            e.Value = (e.Name == "X") ? "Y" : null;
        };
        var output = shell.Expand("${X}");
        Assert.AreEqual("Y", output);
    }

    [TestMethod]
    public void CsvTextOutput_ComplexExpand2() {
        var shell = new ParameterExpansion();
        shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
            e.Value = (e.Name == "X") ? "Y" : null;
        };
        var output = shell.Expand("A${X}B");
        Assert.AreEqual("AYB", output);
    }

    [TestMethod]
    public void CsvTextOutput_ExpandVariableGetsCached() {
        var retrieveCount = 0;
        var shell = new ParameterExpansion();
        shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
            e.Value = (e.Name == "X") ? "Y" : null;
            retrieveCount += 1;
        };
        var output = shell.Expand("A${X}${X}B");
        Assert.AreEqual("AYYB", output);
        Assert.AreEqual(1, retrieveCount);
    }

    [TestMethod]
    public void CsvTextOutput_IncompleteComplexExpand() {
        var shell = new ParameterExpansion();
        shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
            e.Value = (e.Name == "X") ? "Y" : null;
        };
        var output = shell.Expand("A${X");
        Assert.AreEqual("A", output);
    }


    [TestMethod]
    public void CsvTextOutput_ComplexExpandDefault1() {
        var shell = new ParameterExpansion();
        var output = shell.Expand("${VAR1-default}");
        Assert.AreEqual("default", output);
    }

    [TestMethod]
    public void CsvTextOutput_ComplexExpandDefault2() {
        var shell = new ParameterExpansion();
        shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
            e.Value = (e.Name == "VAR1") ? "nondefault" : null;
        };
        var output = shell.Expand("${VAR1-default}");
        Assert.AreEqual("nondefault", output);
    }

    [TestMethod]
    public void CsvTextOutput_ComplexExpandDefault3() {
        var shell = new ParameterExpansion();
        shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
            e.Value = (e.Name == "VAR1") ? "" : null;
        };
        var output = shell.Expand("${VAR1-default}");
        Assert.AreEqual("", output);
    }

    [TestMethod]
    public void CsvTextOutput_ComplexExpandDefault4() {
        var shell = new ParameterExpansion();
        shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
            e.Value = (e.Name == "VAR1") ? "" : null;
        };
        var output = shell.Expand("${VAR1:-default}");
        Assert.AreEqual("default", output);
    }

    [TestMethod]
    public void CsvTextOutput_ComplexExpandDefault5() {
        var shell = new ParameterExpansion();
        shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
            e.Value = (e.Name) switch {
                "VAR2" => "2",
                _ => null,
            };
        };
        var output = shell.Expand("${VAR1:-$VAR2}");
        Assert.AreEqual("2", output);
    }

    [TestMethod]
    public void CsvTextOutput_ComplexExpandDefault6() {
        var shell = new ParameterExpansion();
        shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
            e.Value = (e.Name) switch {
                "VAR2" => "2",
                _ => null,
            };
        };
        var output = shell.Expand("${VAR1:-${VAR2}}");
        Assert.AreEqual("2", output);
    }

    [TestMethod]
    public void CsvTextOutput_ComplexExpandDefault7() {
        var shell = new ParameterExpansion();
        var output = shell.Expand("${VAR1:-${VAR2-3}}");
        Assert.AreEqual("3", output);
    }

    [TestMethod]
    public void CsvTextOutput_ComplexExpandDefault8() {
        var shell = new ParameterExpansion();
        var output = shell.Expand("${VAR1:-${VAR2-3}X}");
        Assert.AreEqual("3X", output);
    }


    [TestMethod]
    public void CsvTextOutput_ComplexExpandDefaultWithSet1() {
        var shell = new ParameterExpansion();
        Assert.AreEqual("abc", shell.Expand("${var=abc}"));
        Assert.AreEqual("abc", shell.Expand("${var=xyz}"));
    }

    [TestMethod]
    public void CsvTextOutput_ComplexExpandDefaultWithSet2() {
        var shell = new ParameterExpansion();
        shell.Parameters.Add("var", "");
        Assert.AreEqual("", shell.Expand("${var=abc}"));
        Assert.AreEqual("", shell.Expand("${var=xyz}"));
    }

    [TestMethod]
    public void CsvTextOutput_ComplexExpandDefaultWithSet3() {
        var shell = new ParameterExpansion();
        shell.Parameters.Add("var", "");
        Assert.AreEqual("abc", shell.Expand("${var:=abc}"));
        Assert.AreEqual("abc", shell.Expand("${var:=xyz}"));
    }


    [TestMethod]
    public void CsvTextOutput_ComplexExpandAlternate1() {
        var shell = new ParameterExpansion();
        Assert.AreEqual("", shell.Expand("${var1+xyz}"));
    }

    [TestMethod]
    public void CsvTextOutput_ComplexExpandAlternate2() {
        var shell = new ParameterExpansion();
        shell.Parameters.Add("var1", "XXX");
        Assert.AreEqual("xyz", shell.Expand("${var1+xyz}"));
    }

    [TestMethod]
    public void CsvTextOutput_ComplexExpandAlternate3() {
        var shell = new ParameterExpansion();
        shell.Parameters.Add("var1", "");
        Assert.AreEqual("", shell.Expand("${var1:+xyz}"));
    }

    [TestMethod]
    public void CsvTextOutput_ComplexExpandAlternate4() {
        var shell = new ParameterExpansion();
        shell.Parameters.Add("var1", "X");
        Assert.AreEqual("xyz", shell.Expand("${var1:+xyz}"));
    }


    [TestMethod]
    public void CsvTextOutput_ComplexExpandUnfinishedExtra() {
        var shell = new ParameterExpansion();
        shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
            e.Value = (e.Name == "VAR1") ? "test" : null;
        };
        var output = shell.Expand("${VAR1:}");
        Assert.AreEqual("test", output);
    }


    [TestMethod]
    public void CsvTextOutput_EnvironmentVariable() {
        var shell = new ParameterExpansion();
        var output = shell.Expand("$USERNAME");
        Assert.AreEqual(Environment.UserName, output);
    }

    [TestMethod]
    public void CsvTextOutput_EnvironmentVariableOverwrite() {
        var shell = new ParameterExpansion();
        shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
            e.Value = (e.Name == "USERNAME") ? "--" : null;
        };
        var output = shell.Expand("$USERNAME");
        Assert.AreEqual("--", output);
    }

    [TestMethod]
    public void CsvTextOutput_EnvironmentVariableDisable() {
        var shell = new ParameterExpansion() {
            UseEnvironmentVariables = false
        };
        var output = shell.Expand("$USERNAME");
        Assert.AreEqual("", output);
    }


    [TestMethod]
    public void CsvTextOutput_Indirection() {
        var shell = new ParameterExpansion();
        shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
            e.Value = (e.Name) switch {
                "IND" => "VAR",
                "VAR" => "VALUE",
                _ => null,
            };
        };
        var output = shell.Expand("${!IND}");
        Assert.AreEqual("VALUE", output);
    }

    [TestMethod]
    public void CsvTextOutput_IndirectionNotFound() {
        var shell = new ParameterExpansion();
        shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
            e.Value = (e.Name) switch {
                "IND" => "VAR",
                _ => null,
            };
        };
        var output = shell.Expand("${!IND}");
        Assert.AreEqual("", output);
    }


    [TestMethod]
    public void CsvTextOutput_ParameterLength() {
        var shell = new ParameterExpansion();
        shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
            e.Value = (e.Name) switch {
                "VAR" => "Value",
                _ => null,
            };
        };
        var output = shell.Expand("${#VAR}");
        Assert.AreEqual("5", output);
    }

    [TestMethod]
    public void CsvTextOutput_ParameterLengthExpanded() {
        var shell = new ParameterExpansion();
        shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
            e.Value = (e.Name) switch {
                "X" => "VAR",
                "VAR" => "Value",
                _ => null,
            };
        };
        var output = shell.Expand("${#$X}");
        Assert.AreEqual("5", output);
    }


    [TestMethod]
    public void CsvTextOutput_OperatorUppercase() {
        var shell = new ParameterExpansion();
        shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
            e.Value = (e.Name) switch {
                "X" => "valuE",
                _ => null,
            };
        };
        var output = shell.Expand("${X@U}");
        Assert.AreEqual("VALUE", output);
    }

    [TestMethod]
    public void CsvTextOutput_OperatorTitlecase() {
        var shell = new ParameterExpansion();
        shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
            e.Value = (e.Name) switch {
                "X" => "valuE",
                _ => null,
            };
        };
        var output = shell.Expand("${X@u}");
        Assert.AreEqual("ValuE", output);
    }

    [TestMethod]
    public void CsvTextOutput_OperatorLowercase() {
        var shell = new ParameterExpansion();
        shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
            e.Value = (e.Name) switch {
                "X" => "valuE",
                _ => null,
            };
        };
        var output = shell.Expand("${X@L}");
        Assert.AreEqual("value", output);
    }


    [TestMethod]
    public void CsvTextOutput_Substring1() {
        var shell = new ParameterExpansion();
        shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
            e.Value = (e.Name) switch {
                "string" => "01234567890abcdefgh",
                _ => null,
            };
        };
        var output = shell.Expand("${string:7}");
        Assert.AreEqual("7890abcdefgh", output);
    }

    [TestMethod]
    public void CsvTextOutput_Substring2() {
        var shell = new ParameterExpansion();
        shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
            e.Value = (e.Name) switch {
                "string" => "01234567890abcdefgh",
                _ => null,
            };
        };
        var output = shell.Expand("${string:7:0}");
        Assert.AreEqual("", output);
    }

    [TestMethod]
    public void CsvTextOutput_Substring3() {
        var shell = new ParameterExpansion();
        shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
            e.Value = (e.Name) switch {
                "string" => "01234567890abcdefgh",
                _ => null,
            };
        };
        var output = shell.Expand("${string:7:2}");
        Assert.AreEqual("78", output);
    }

    [TestMethod]
    public void CsvTextOutput_Substring4() {
        var shell = new ParameterExpansion();
        shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
            e.Value = (e.Name) switch {
                "string" => "01234567890abcdefgh",
                _ => null,
            };
        };
        var output = shell.Expand("${string:7:-2}");
        Assert.AreEqual("7890abcdef", output);
    }

    [TestMethod]
    public void CsvTextOutput_Substring5() {
        var shell = new ParameterExpansion();
        shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
            e.Value = (e.Name) switch {
                "string" => "01234567890abcdefgh",
                _ => null,
            };
        };
        var output = shell.Expand("${string: -7}");
        Assert.AreEqual("bcdefgh", output);
    }

    [TestMethod]
    public void CsvTextOutput_Substring6() {
        var shell = new ParameterExpansion();
        shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
            e.Value = (e.Name) switch {
                "string" => "01234567890abcdefgh",
                _ => null,
            };
        };
        var output = shell.Expand("${string: -7:0}");
        Assert.AreEqual("", output);
    }

    [TestMethod]
    public void CsvTextOutput_Substring7() {
        var shell = new ParameterExpansion();
        shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
            e.Value = (e.Name) switch {
                "string" => "01234567890abcdefgh",
                _ => null,
            };
        };
        var output = shell.Expand("${string: -7:2}");
        Assert.AreEqual("bc", output);
    }

    [TestMethod]
    public void CsvTextOutput_Substring8() {
        var shell = new ParameterExpansion();
        shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
            e.Value = (e.Name) switch {
                "string" => "01234567890abcdefgh",
                _ => null,
            };
        };
        var output = shell.Expand("${string: -7:-2}");
        Assert.AreEqual("bcdef", output);
    }

    [TestMethod]
    public void CsvTextOutput_Substring9() {
        var shell = new ParameterExpansion();
        shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
            e.Value = (e.Name) switch {
                "string" => "01234567890abcdefgh",
                _ => null,
            };
        };
        var output = shell.Expand("${string:18}");
        Assert.AreEqual("h", output);
    }

    [TestMethod]
    public void CsvTextOutput_SubstringOutOfRange1() {
        var shell = new ParameterExpansion();
        shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
            e.Value = (e.Name) switch {
                "string" => "",
                _ => null,
            };
        };
        var output = shell.Expand("${string:1}");
        Assert.AreEqual("", output);
    }

    [TestMethod]
    public void CsvTextOutput_SubstringOutOfRange2() {
        var shell = new ParameterExpansion();
        shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
            e.Value = (e.Name) switch {
                "string" => "",
                _ => null,
            };
        };
        var output = shell.Expand("${string: -1}");
        Assert.AreEqual("", output);
    }

    [TestMethod]
    public void CsvTextOutput_SubstringOutOfRange3() {
        var shell = new ParameterExpansion();
        shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
            e.Value = (e.Name) switch {
                "string" => "AB",
                _ => null,
            };
        };
        var output = shell.Expand("${string:0:-3}");
        Assert.AreEqual("", output);
    }

    [TestMethod]
    public void CsvTextOutput_SubstringOutOfRange4() {
        var shell = new ParameterExpansion();
        shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
            e.Value = (e.Name) switch {
                "string" => "AB",
                _ => null,
            };
        };
        var output = shell.Expand("${string:0:3}");
        Assert.AreEqual("AB", output);
    }


    [TestMethod]
    public void CsvTextOutput_Uppercase1() {
        var shell = new ParameterExpansion();
        shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
            e.Value = (e.Name) switch {
                "X" => "testing",
                _ => null,
            };
        };
        var output = shell.Expand("${X^}");
        Assert.AreEqual("Testing", output);
    }

    [TestMethod]
    public void CsvTextOutput_Uppercase2() {
        var shell = new ParameterExpansion();
        shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
            e.Value = (e.Name) switch {
                "X" => "testing",
                _ => null,
            };
        };
        var output = shell.Expand("${X^^}");
        Assert.AreEqual("TESTING", output);
    }


    [TestMethod]
    public void CsvTextOutput_Lowercase1() {
        var shell = new ParameterExpansion();
        shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
            e.Value = (e.Name) switch {
                "X" => "TESTING",
                _ => null,
            };
        };
        var output = shell.Expand("${X,}");
        Assert.AreEqual("tESTING", output);
    }

    [TestMethod]
    public void CsvTextOutput_Lowercase2() {
        var shell = new ParameterExpansion();
        shell.RetrieveParameter += delegate (object sender, ParameterExpansionEventArgs e) {
            e.Value = (e.Name) switch {
                "X" => "TESTING",
                _ => null,
            };
        };
        var output = shell.Expand("${X,,}");
        Assert.AreEqual("testing", output);
    }


    [TestMethod]
    public void CsvTextOutput_WithAutoAdd() {
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
        Assert.AreEqual("111", output);
        Assert.AreEqual(1, retrieveCount);
    }

    [TestMethod]
    public void CsvTextOutput_WithoutAutoAdd() {
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
        Assert.AreEqual("123", output);
        Assert.AreEqual(3, retrieveCount);
    }

    [TestMethod]
    public void CsvTextOutput_IndirectWithAutoAdd() {
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
        Assert.AreEqual("111111Z", output);
        Assert.AreEqual(3, retrieveCount);
    }

    [TestMethod]
    public void CsvTextOutput_IndirectWithoutAutoAdd() {
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
        Assert.AreEqual("123568Z", output);
        Assert.AreEqual(9, retrieveCount);
    }

    [TestMethod]
    public void CsvTextOutput_ComplexWithAutoAdd() {
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
        Assert.AreEqual("1111Z", output);
        Assert.AreEqual(4, retrieveCount);
    }

    [TestMethod]
    public void CsvTextOutput_ComplexWithoutAutoAdd() {
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
        Assert.AreEqual("1356Z", output);
        Assert.AreEqual(7, retrieveCount);
    }

}
