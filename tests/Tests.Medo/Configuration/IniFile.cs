using System;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Medo.Configuration;

namespace Tests;

[TestClass]
public class IniFile_Tests {

    [TestMethod]
    public void IniFile_Empty() {
        var ini = new IniFile();
        Assert.AreEqual(0, ini.Count);
    }

    [TestMethod]
    public void IniFile_NullFile() {
        var ex = Assert.ThrowsException<ArgumentNullException>(() => {
            _ = new IniFile(default(string));
        });
    }

    [TestMethod]
    public void IniFile_NullStream() {
        var ex = Assert.ThrowsException<ArgumentNullException>(() => {
            _ = new IniFile(default(Stream));
        });
    }

    [TestMethod]
    public void IniFile_MissingFile() {
        var ex = Assert.ThrowsException<FileNotFoundException>(() => {
            _ = new IniFile(@"C:\dummy-file-that-does-not-exist.txt");
        });
    }

    [TestMethod]
    public void IniFile_EmptyFile() {
        var ini = new IniFile(GetResourceStream("Empty.ini"));
        Assert.AreEqual(0, ini.Count);
    }

    [TestMethod]
    public void IniFile_SectionCharactersAfterEnd() {
        var ini = new IniFile(GetResourceStream("SectionCharactersAfterEnd.ini"));
        Assert.AreEqual(1, ini.Count);

        Assert.AreEqual("Value", ini.Read("Section]Extra", "Key"));

        Assert.AreEqual(1, ini.GetSections().Count);
        Assert.AreEqual("Section]Extra", ini.GetSections()[0]);
    }

    [TestMethod]
    public void IniFile_SectionIgnoresInnerBrackets() {
        var ini = new IniFile(GetResourceStream("SectionIgnoresInnerBrackets.ini"));
        Assert.AreEqual(1, ini.Count);

        Assert.AreEqual("Value", ini.Read("Section[i]", "Key"));

        Assert.AreEqual(1, ini.GetSections().Count);
        Assert.AreEqual("Section[i]", ini.GetSections()[0]);
    }

    [TestMethod]
    public void IniFile_SectionImplicitEnd() {
        var ini = new IniFile(GetResourceStream("SectionImplicitEnd.ini"));
        Assert.AreEqual(1, ini.Count);

        Assert.AreEqual("Value", ini.Read("Section", "Key"));

        Assert.AreEqual(1, ini.GetSections().Count);
        Assert.AreEqual("Section", ini.GetSections()[0]);
    }

    [TestMethod]
    public void IniFile_EmptyKey() {
        var ini = new IniFile(GetResourceStream("EmptyKey.ini"));
        Assert.AreEqual(1, ini.Count);

        Assert.AreEqual("Value", ini.Read("", ""));

        Assert.AreEqual(1, ini.GetSections().Count);
        Assert.AreEqual("", ini.GetSections()[0]);
    }

    [TestMethod]
    public void IniFile_Comment() {
        var ini = new IniFile(GetResourceStream("Comment.ini"));
        //Assert.AreEqual(1, ini.Count);

        Assert.AreEqual("Value", ini.Read("", "Key"));

        Assert.AreEqual(1, ini.GetSections().Count);
        Assert.AreEqual("", ini.GetSections()[0]);
    }

    [TestMethod]
    public void IniFile_Conversions() {
        var ini = new IniFile(GetResourceStream("Conversions.ini"));
        Assert.AreEqual(23, ini.Count);

        Assert.AreEqual(5, ini.GetSections().Count);
        Assert.AreEqual("Boolean", ini.GetSections()[0]);
        Assert.AreEqual("Int32", ini.GetSections()[1]);
        Assert.AreEqual("Int64", ini.GetSections()[2]);
        Assert.AreEqual("Single", ini.GetSections()[3]);
        Assert.AreEqual("Double", ini.GetSections()[4]);

        Assert.IsTrue(ini.Read("Boolean", "True1", false));
        Assert.IsTrue(ini.Read("Boolean", "True2", false));
        Assert.IsTrue(ini.Read("Boolean", "True3", false));
        Assert.IsTrue(ini.Read("Boolean", "True4", false));
        Assert.IsTrue(ini.Read("Boolean", "True5", false));
        Assert.IsFalse(ini.Read("Boolean", "False1", true));
        Assert.IsFalse(ini.Read("Boolean", "False2", true));
        Assert.IsFalse(ini.Read("Boolean", "False3", true));
        Assert.IsFalse(ini.Read("Boolean", "False4", true));

        Assert.AreEqual(0, ini.Read("Int32", "Zero", 42));
        Assert.AreEqual(Int32.MinValue, ini.Read("Int32", "Min", 42));
        Assert.AreEqual(Int32.MaxValue, ini.Read("Int32", "Max", 42));

        Assert.AreEqual(0L, ini.Read("Int64", "Zero", 42L));
        Assert.AreEqual(Int64.MinValue, ini.Read("Int64", "Min", 42L));
        Assert.AreEqual(Int64.MaxValue, ini.Read("Int64", "Max", 42L));

        Assert.AreEqual(0.0F, ini.Read("Single", "Zero", 42.0F));
        Assert.AreEqual(Single.NaN, ini.Read("Single", "NaN", 42.0F));
        Assert.AreEqual(Single.MinValue, ini.Read("Single", "Min", 42.0F));
        Assert.AreEqual(Single.MaxValue, ini.Read("Single", "Max", 42.0F));

        Assert.AreEqual(0.0, ini.Read("Double", "Zero", 42.0));
        Assert.AreEqual(Double.NaN, ini.Read("Double", "NaN", 42.0));
        Assert.AreEqual(Double.MinValue, ini.Read("Double", "Min", 42.0));
        Assert.AreEqual(Double.MaxValue, ini.Read("Double", "Max", 42.0));
    }

    [TestMethod]
    public void IniFile_MergeSections() {
        var ini = new IniFile(GetResourceStream("MergeSections.ini"));
        Assert.AreEqual(3, ini.Count);

        Assert.AreEqual("A", ini.Read("Section", "Key1"));
        Assert.AreEqual("B", ini.Read("Section", "Key2"));
        Assert.AreEqual("C", ini.Read("Section", "Key3"));

        Assert.AreEqual(1, ini.GetSections().Count);
        Assert.AreEqual("SECTION", ini.GetSections()[0]);
    }

    [TestMethod]
    public void IniFile_IgnoreEmptySections() {
        var ini = new IniFile(GetResourceStream("IgnoreEmptySections.ini"));
        Assert.AreEqual(0, ini.Count);

        Assert.AreEqual(0, ini.GetSections().Count);
    }

    [TestMethod]
    public void IniFile_IgnoreIncompleteProperties() {
        var ini = new IniFile(GetResourceStream("IgnoreIncompleteProperties.ini"));
        Assert.AreEqual(0, ini.Count);

        Assert.AreEqual(0, ini.GetSections().Count);
    }

    [TestMethod]
    public void IniFile_WhitespaceAtStart() {
        var ini = new IniFile(GetResourceStream("WhitespaceAtStart.ini"));
        Assert.AreEqual(1, ini.Count);
        Assert.AreEqual("Value", ini.Read("Section", "Key"));
    }

    [TestMethod]
    public void IniFile_WhitespaceAtEnd() {
        var ini = new IniFile(GetResourceStream("WhitespaceAtEnd.ini"));
        Assert.AreEqual(1, ini.Count);
        Assert.AreEqual("Value", ini.Read("Section", "Key"));
    }

    [TestMethod]
    public void IniFile_SingleQuote() {
        var ini = new IniFile(GetResourceStream("SingleQuote.ini"));
        Assert.AreEqual(3, ini.Count);
        Assert.AreEqual("Value'd", ini.Read("Section", "Normal"));
        Assert.AreEqual(" Whitespace'd ", ini.Read("Section", "Whitespace"));
        Assert.AreEqual("Start Middle End", ini.Read("Section", "Multi"));
    }

    [TestMethod]
    public void IniFile_DoubleQuote() {
        var ini = new IniFile(GetResourceStream("DoubleQuote.ini"));
        Assert.AreEqual(10, ini.Count);
        Assert.AreEqual("Value'd", ini.Read("Section", "Normal"));
        Assert.AreEqual(" Whitespace'd ", ini.Read("Section", "Whitespace"));
        Assert.AreEqual(" Start Middle End ", ini.Read("Section", "Multi"));
        Assert.AreEqual("\"Really\"", ini.Read("Section", "Quotes"));
        Assert.AreEqual("AçB", ini.Read("Section", "Short"));
        Assert.AreEqual("👽", ini.Read("Section", "Long"));
        Assert.AreEqual("Aç|ç|çB", ini.Read("Section", "Variable"));
        Assert.AreEqual("A", ini.Read("Section", "IncompleteShort"));
        Assert.AreEqual("A", ini.Read("Section", "IncompleteLong"));
        Assert.AreEqual("A", ini.Read("Section", "IncompleteVariable"));
    }

    [TestMethod]
    public void IniFile_Mixed() {
        var ini = new IniFile(GetResourceStream("Mixed.ini"));
        Assert.AreEqual(9, ini.Count);
        Assert.AreEqual(" Start Middle End ", ini.Read("Section", "Quoting"));
        Assert.AreEqual("Normal", ini.Read("Section", "ExtraWhitespace1"));
        Assert.AreEqual("Normal", ini.Read("Section", "ExtraWhitespace2"));
        Assert.AreEqual("Normal A", ini.Read("Section", "ExtraWhitespace3"));
        Assert.AreEqual("Normal A", ini.Read("Section", "ExtraWhitespace4"));
        Assert.AreEqual(" Normal", ini.Read("Section", "ExtraWhitespace5"));
        Assert.AreEqual(" Normal", ini.Read("Section", "ExtraWhitespace6"));
        Assert.AreEqual("A Normal", ini.Read("Section", "ExtraWhitespace7"));
        Assert.AreEqual("A Normal", ini.Read("Section", "ExtraWhitespace8"));
    }


    #region Utils

    private static Stream GetResourceStream(string resourceName) {
        var resAssembly = typeof(IniFile_Tests).GetTypeInfo().Assembly;
        return resAssembly.GetManifestResourceStream("Tests._Resources.Configuration.IniFile." + resourceName);
    }

    #endregion

}
