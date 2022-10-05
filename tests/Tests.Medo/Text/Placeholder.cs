using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Medo.Text;

namespace Tests;

[TestClass]
public class Placeholder_Tests {

    [TestMethod]
    public void Placeholder_None() {
        Assert.AreEqual("Test", Placeholder.Format(CultureInfo.InvariantCulture, "Test"));
    }

    [TestMethod]
    public void Placeholder_LeftBraceEscape() {
        Assert.AreEqual("{", Placeholder.Format(CultureInfo.InvariantCulture, "{{"));
    }

    [TestMethod]
    public void Placeholder_RightBraceEscape() {
        Assert.AreEqual("}", Placeholder.Format(CultureInfo.InvariantCulture, "}}"));
    }

    [TestMethod]
    public void Placeholder_BothBraceEscape() {
        Assert.AreEqual("{}", Placeholder.Format(CultureInfo.InvariantCulture, "{{}}"));
    }


    [TestMethod]
    public void Placeholder_String() {
        var dict = new Dictionary<string, object> {
            { "Text", "Text" }
        };
        Assert.AreEqual("Test: Text.", Placeholder.Format(CultureInfo.InvariantCulture, "Test: {Text}.", dict));
    }

    [TestMethod]
    public void Placeholder_MultipleStrings() {
        var dict = new Dictionary<string, object> {
            { "Number", "42" },
            { "Text", "Fortytwo" }
        };
        Assert.AreEqual("Test: 42 (Fortytwo).", Placeholder.Format(CultureInfo.InvariantCulture, "Test: {Number} ({Text}).", dict));
    }

    [TestMethod]
    public void Placeholder_MultipleStrings2() {
        Assert.AreEqual("Test: 42 (Fortytwo).", Placeholder.Format(CultureInfo.InvariantCulture, "Test: {Number} ({Text}).", "Number", 42, "Text", "Fortytwo"));
    }

    [TestMethod]
    public void Placeholder_DoubleString() {
        var dict = new Dictionary<string, object> {
            { "Text", "Text" }
        };
        Assert.AreEqual("Test: Text, Text.", Placeholder.Format(CultureInfo.InvariantCulture, "Test: {Text}, {Text}.", dict));
    }

    [TestMethod]
    public void Placeholder_Integer() {
        var dict = new Dictionary<string, object> {
            { "Number", 42 }
        };
        Assert.AreEqual("Test: 42.", Placeholder.Format(CultureInfo.InvariantCulture, "Test: {Number}.", dict));
    }

    [TestMethod]
    public void Placeholder_IntegerWithFormat() {
        var dict = new Dictionary<string, object> {
            { "Number", 42 }
        };
        Assert.AreEqual("Test: 42.0.", Placeholder.Format(CultureInfo.InvariantCulture, "Test: {Number:0.0}.", dict));
    }

    [TestMethod]
    public void Placeholder_EnclosedDouble() {
        var dict = new Dictionary<string, object> {
            { "Number", 42.0 }
        };
        Assert.AreEqual("Test: {42.0}.", Placeholder.Format(CultureInfo.InvariantCulture, "Test: {{{Number:0.0}}}.", dict));
    }

    [TestMethod]
    public void Placeholder_DateTime() {
        var dict = new Dictionary<string, object> {
            { "Time", new DateTime(1979, 01, 28, 18, 15, 0) }
        };
        Assert.AreEqual("Test: 19790128T181500.", Placeholder.Format(CultureInfo.InvariantCulture, "Test: {Time:yyyyMMdd'T'HHmmss}.", dict));
    }

    [TestMethod]
    public void Placeholder_DateTimeOffset() {
        var dict = new Dictionary<string, object> {
            { "Time", new DateTimeOffset(1979, 01, 28, 18, 15, 0, TimeSpan.FromHours(1)) }
        };
        Assert.AreEqual("Test: 19790128T181500.", Placeholder.Format(CultureInfo.InvariantCulture, "Test: {Time:yyyyMMdd'T'HHmmss}.", dict));
    }


    [TestMethod]
    public void Placeholder_MultipleFormats() {
        Assert.AreEqual("Test: 42 (Fortytwo).", Placeholder.Format(CultureInfo.InvariantCulture, "Test: {Number:0} ({Text}).", "Number", "42", "Text", "Fortytwo"));
    }

    [TestMethod]
    public void Placeholder_Nulls() {
        Assert.AreEqual("Test:  ().", Placeholder.Format(CultureInfo.InvariantCulture, "Test: {Number:0} ({Text}).", "Number", null, "Text", null));
    }


    [TestMethod]
    public void Placeholder_DoesNotExist() {
        var dict = new Dictionary<string, object> {
            { "Text", "Text" }
        };
        Assert.AreEqual("Test: .", Placeholder.Format(CultureInfo.InvariantCulture, "Test: {Unknown}.", dict));
    }

}
