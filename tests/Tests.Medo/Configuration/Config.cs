using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Medo.Configuration;

namespace Tests;

[TestClass]
public class Config_Tests {

    [TestMethod]
    public void Config_NullKey() {
        var ex = Assert.ThrowsException<ArgumentNullException>(() => {
            Config.Read(null, "");
        });
    }

    [TestMethod]
    public void Config_EmptyKey() {
        var ex = Assert.ThrowsException<ArgumentOutOfRangeException>(() => {
            Config.Read("   ", "");
        });
    }


    [TestMethod]
    public void Config_EmptySave() {
        using var loader = new ConfigLoader("Empty.cfg");
        Assert.IsTrue(Config.Load(), "File should exist before load.");
        Assert.IsTrue(Config.Save(), "Save should succeed.");
        Assert.AreEqual(BitConverter.ToString(loader.Bytes), BitConverter.ToString(File.ReadAllBytes(loader.FileName)));
        Assert.IsFalse(Config.IsAssumedInstalled);
    }

    [TestMethod]
    public void Config_AssumeInstalled() {
        var executablePath = Assembly.GetEntryAssembly().Location;

        var userFileLocation = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? Path.Combine(Environment.GetEnvironmentVariable("AppData"), "", "Microsoft.TestHost", "Microsoft.TestHost.cfg")
            : Path.Combine(Environment.GetEnvironmentVariable("HOME") ?? "~", "Microsoft.TestHost.cfg");

        var localFileLocation = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? Path.Combine(Path.GetDirectoryName(executablePath), "Microsoft.TestHost.cfg")
            : Path.Combine(Path.GetDirectoryName(executablePath), ".Microsoft.TestHost");

        var userFileDirectory = Path.GetDirectoryName(userFileLocation);

        //clean files
        if (Directory.Exists(userFileDirectory)) { Directory.Delete(userFileDirectory, true); }
        if (File.Exists(localFileLocation)) { File.Delete(localFileLocation); }

        try {
            Config.Reset();
            Assert.IsFalse(Config.IsAssumedInstalled);
            Assert.IsNotNull(Config.FileName);
            Assert.IsNull(Config.OverrideFileName);
        } finally { //clean files
            if (Directory.Exists(userFileDirectory)) { Directory.Delete(userFileDirectory, true); }
            if (File.Exists(localFileLocation)) { File.Delete(localFileLocation); }
        }

        try { //create empty file in AppData directory and a local file
            Config.Reset();
            Config.IsAssumedInstalled = true;
            if (!Directory.Exists(userFileDirectory)) { Directory.CreateDirectory(userFileDirectory); }
            File.WriteAllText(userFileLocation, "");
            File.WriteAllText(localFileLocation, "");

            Assert.IsTrue(Config.IsAssumedInstalled);
            Assert.IsNotNull(Config.FileName);
            Assert.IsNotNull(Config.OverrideFileName);
        } finally { //clean files
            if (Directory.Exists(userFileDirectory)) { Directory.Delete(userFileDirectory, true); }
            if (File.Exists(localFileLocation)) { File.Delete(localFileLocation); }
        }

        try {
            Config.Reset();

            Assert.IsFalse(Config.IsAssumedInstalled);
            Assert.IsNotNull(Config.FileName);
            Assert.IsNull(Config.OverrideFileName);
        } finally { //clean files
            if (Directory.Exists(userFileDirectory)) { Directory.Delete(userFileDirectory, true); }
            if (File.Exists(localFileLocation)) { File.Delete(localFileLocation); }
        }
    }


    [TestMethod]
    public void Config_EmptyLinesCRLF() {
        using var loader = new ConfigLoader("EmptyLinesCRLF.cfg");
        Config.Save();
        Assert.AreEqual(BitConverter.ToString(loader.Bytes), BitConverter.ToString(File.ReadAllBytes(loader.FileName)));
    }

    [TestMethod]
    public void Config_EmptyLinesLF() {
        using var loader = new ConfigLoader("EmptyLinesLF.cfg");
        Config.Save();
        Assert.AreEqual(BitConverter.ToString(loader.Bytes), BitConverter.ToString(File.ReadAllBytes(loader.FileName)));
    }

    [TestMethod]
    public void Config_EmptyLinesCR() {
        using var loader = new ConfigLoader("EmptyLinesCR.cfg");
        Config.Save();
        Assert.AreEqual(BitConverter.ToString(loader.Bytes), BitConverter.ToString(File.ReadAllBytes(loader.FileName)));
    }

    [TestMethod]
    public void Config_EmptyLinesMixed() {
        using var loader = new ConfigLoader("EmptyLinesMixed.cfg", "EmptyLinesMixed.Good.cfg");
        Config.Save();
        Assert.AreEqual(loader.GoodText, File.ReadAllText(loader.FileName));
    }

    [TestMethod]
    public void Config_CommentsOnly() {
        using var loader = new ConfigLoader("CommentsOnly.cfg", "CommentsOnly.Good.cfg");
        Config.Save();
        Assert.AreEqual(BitConverter.ToString(loader.GoodBytes), BitConverter.ToString(File.ReadAllBytes(loader.FileName)));
    }

    [TestMethod]
    public void Config_CommentsWithValues() {
        using var loader = new ConfigLoader("CommentsWithValues.cfg");
        Config.Save();
        Assert.AreEqual(Encoding.UTF8.GetString(loader.Bytes), Encoding.UTF8.GetString(File.ReadAllBytes(loader.FileName)));
        Assert.AreEqual(BitConverter.ToString(loader.Bytes), BitConverter.ToString(File.ReadAllBytes(loader.FileName)));
    }

    [TestMethod]
    public void Config_SpacingEscape() {
        using var loader = new ConfigLoader("SpacingEscape.cfg", "SpacingEscape.Good.cfg");
        Assert.AreEqual(" Value 1", Config.Read("Key1"));
        Assert.AreEqual("Value 2 ", Config.Read("Key2"));
        Assert.AreEqual(" Value 3 ", Config.Read("Key3"));
        Assert.AreEqual("  Value 4  ", Config.Read("Key4"));
        Assert.AreEqual("\tValue 5\t", Config.Read("Key5"));
        Assert.AreEqual("\tValue 6", Config.Read("Key6"));
        Assert.AreEqual("\0", Config.Read("Null"));

        Config.Save();
        Config.Write("Null", "\0Null\0");
        Config.Save();

        Assert.AreEqual(loader.GoodText, File.ReadAllText(loader.FileName));
    }

    [TestMethod]
    public void Config_WriteBasic() {
        using var loader = new ConfigLoader("Empty.cfg", "WriteBasic.Good.cfg");
        Config.Write("Key1", "Value 1");
        Config.Write("Key2", "Value 2");

        Config.Save();

        Assert.AreEqual(loader.GoodText, File.ReadAllText(loader.FileName));
    }

    [TestMethod]
    public void Config_WriteNoEmptyLine() {
        using var loader = new ConfigLoader("WriteNoEmptyLine.cfg", "WriteNoEmptyLine.Good.cfg");
        Config.Write("Key1", "Value 1");
        Config.Write("Key2", "Value 2");

        Config.Save();

        Assert.AreEqual(loader.GoodText, File.ReadAllText(loader.FileName));
    }

    [TestMethod]
    public void Config_WriteSameSeparatorEquals() {
        using var loader = new ConfigLoader("WriteSameSeparatorEquals.cfg", "WriteSameSeparatorEquals.Good.cfg");
        Config.Write("Key1", "Value 1");
        Config.Write("Key2", "Value 2");

        Config.Save();

        Assert.AreEqual(loader.GoodText, File.ReadAllText(loader.FileName));
    }

    [TestMethod]
    public void Config_WriteSameSeparatorSpace() {
        using var loader = new ConfigLoader("WriteSameSeparatorSpace.cfg", "WriteSameSeparatorSpace.Good.cfg");
        Config.Write("Key1", "Value 1");
        Config.Write("Key2", "Value 2");

        Config.Save();

        Assert.AreEqual(loader.GoodText, File.ReadAllText(loader.FileName));
    }

    [TestMethod]
    public void Config_Replace() {
        using var loader = new ConfigLoader("Replace.cfg", "Replace.Good.cfg");
        Config.Write("Key1", "Value 1a");
        Config.Write("Key2", "Value 2a");

        Config.Save();

        Assert.AreEqual("Value 1a", Config.Read("Key1"));
        Assert.AreEqual("Value 2a", Config.Read("Key2"));

        Assert.AreEqual(loader.GoodText, File.ReadAllText(loader.FileName));
    }

    [TestMethod]
    public void Config_SpacingPreserved() {
        using var loader = new ConfigLoader("SpacingPreserved.cfg", "SpacingPreserved.Good.cfg");
        Config.Write("KeyOne", "Value 1a");
        Config.Write("KeyTwo", "Value 2b");
        Config.Write("KeyThree", "Value 3c");

        Config.Save();

        Assert.AreEqual(loader.GoodText, File.ReadAllText(loader.FileName));
    }

    [TestMethod]
    public void Config_SpacingPreservedOnAdd() {
        using var loader = new ConfigLoader("SpacingPreservedOnAdd.cfg", "SpacingPreservedOnAdd.Good.cfg");
        Config.Write("One", "Value 1a");
        Config.Write("Two", new string[] { "Value 2a", "Value 2b" });
        Config.Write("Three", "Value 3a");
        Config.Write("Four", "Value 4a");
        Config.Write("Five", new string[] { "Value 5a", "Value 5b", "Value 5c" });
        Config.Write("FourtyTwo", 42);

        Config.Save();

        Assert.AreEqual(loader.GoodText, File.ReadAllText(loader.FileName));
    }

    [TestMethod]
    public void Config_WriteToEmpty() {
        using var loader = new ConfigLoader(null, "Replace.Good.cfg");
        Config.Write("Key1", "Value 1a");
        Config.Write("Key2", "Value 2a");

        Config.Save();

        Assert.AreEqual(loader.GoodText, File.ReadAllText(loader.FileName));
    }

    [TestMethod]
    public void Config_ReplaceOnlyLast() {
        using var loader = new ConfigLoader("ReplaceOnlyLast.cfg", "ReplaceOnlyLast.Good.cfg");
        Config.Write("Key1", "Value 1a");
        Config.Write("Key2", "Value 2a");

        Config.Save();

        Assert.AreEqual("Value 1a", Config.Read("Key1"));
        Assert.AreEqual("Value 2a", Config.Read("Key2"));
        Assert.AreEqual("Value 3", Config.Read("Key3"));

        Assert.AreEqual(loader.GoodText, File.ReadAllText(loader.FileName));
    }


    [TestMethod]
    public void Config_SaveInNonexistingDirectory1() {
        var propertiesFile = Path.Combine(Path.GetTempPath(), "ConfigDirectory", "Test.cfg");
        try {
            Directory.Delete(Path.Combine(Path.GetTempPath(), "ConfigDirectory"), true);
        } catch (IOException) { }
        Config.FileName = propertiesFile;

        Assert.IsFalse(Config.Load(), "No file present for load.");

        var x = Config.Read("Test", "test");
        Assert.AreEqual("test", x);

        Assert.IsTrue(Config.Save(), "Save should create directory structure and succeed.");
        Assert.IsTrue(File.Exists(propertiesFile));

        Directory.Delete(Path.Combine(Path.GetTempPath(), "ConfigDirectory"), true);
    }

    [TestMethod]
    public void Config_SaveInNonexistingDirectory2() {
        var propertiesFile = Path.Combine(Path.GetTempPath(), "ConfigDirectoryOuter", "ConfigDirectoryInner", "Test.cfg");
        try {
            Directory.Delete(Path.Combine(Path.GetTempPath(), "ConfigDirectoryOuter"), true);
        } catch (IOException) { }
        Config.FileName = propertiesFile;

        Assert.IsFalse(Config.Load(), "No file present for load.");

        var x = Config.Read("Test", "test");
        Assert.AreEqual("test", x);

        Assert.IsTrue(Config.Save(), "Save should create directory structure and succeed.");
        Assert.IsTrue(File.Exists(propertiesFile));

        Directory.Delete(Path.Combine(Path.GetTempPath(), "ConfigDirectoryOuter"), true);
    }

    [TestMethod]
    public void Config_SaveInNonexistingDirectory3() {
        var propertiesFile = Path.Combine(Path.GetTempPath(), "ConfigDirectoryOuter", "ConfigDirectoryMiddle", "ConfigDirectoryInner", "Test.cfg");
        try {
            Directory.Delete(Path.Combine(Path.GetTempPath(), "ConfigDirectoryOuter"), true);
        } catch (IOException) { }
        Config.FileName = propertiesFile;

        Assert.IsFalse(Config.Load(), "No file present for load.");

        var x = Config.Read("Test", "test");
        Assert.AreEqual("test", x);

        Assert.IsTrue(Config.Save(), "Save should create directory structure and succeed.");
        Assert.IsTrue(File.Exists(propertiesFile));

        Directory.Delete(Path.Combine(Path.GetTempPath(), "ConfigDirectoryOuter"), true);
    }


    [TestMethod]
    public void Config_RemoveSingle() {
        using var loader = new ConfigLoader("Remove.cfg", "Remove.Good.cfg");
        Config.Delete("Key1");
        Config.Save();

        Assert.AreEqual(loader.GoodText, File.ReadAllText(loader.FileName));
    }

    [TestMethod]
    public void Config_RemoveMulti() {
        using var loader = new ConfigLoader("RemoveMulti.cfg", "RemoveMulti.Good.cfg");
        Config.Delete("Key2");
        Config.Save();

        Assert.AreEqual(loader.GoodText, File.ReadAllText(loader.FileName));
    }


    [TestMethod]
    public void Config_UseOverrideFirst() {
        using var loader = new ConfigLoader("Replace.cfg", resourceOverrideFileName: "Replace.Good.cfg");
        Assert.AreEqual("Value 1a", Config.Read("Key1"));
    }

    [TestMethod]
    public void Config_DontOverwriteOverride() {
        using var loader = new ConfigLoader("Replace.cfg", resourceOverrideFileName: "Replace.Good.cfg");
        Config.ImmediateSave = true;

        Config.Write("Key1", "XXX");
        Assert.AreEqual("Value 1a", Config.Read("Key1"));
        Config.OverrideFileName = null;
        Assert.AreEqual("XXX", Config.Read("Key1"));
    }


    [TestMethod]
    public void Config_ReadMulti() {
        using var loader = new ConfigLoader("ReplaceOnlyLast.Good.cfg");
        var list = new List<string>(Config.ReadAll("Key2"));
        Assert.AreEqual(2, list.Count);
        Assert.AreEqual("Value 2", list[0]);
        Assert.AreEqual("Value 2a", list[1]);
    }

    [TestMethod]
    public void Config_ReadMultiFromOverride() {
        using var loader = new ConfigLoader("ReplaceOnlyLast.Good.cfg", resourceOverrideFileName: "RemoveMulti.cfg");
        var list = new List<string>(Config.ReadAll("Key2"));
        Assert.AreEqual(3, list.Count);
        Assert.AreEqual("Value 2a", list[0]);
        Assert.AreEqual("Value 2b", list[1]);
        Assert.AreEqual("Value 2c", list[2]);
    }

    [TestMethod]
    public void Config_ReadMultiFromOverrideNotFound() {
        using var loader = new ConfigLoader("ReplaceOnlyLast.Good.cfg", resourceOverrideFileName: "RemoveMulti.cfg");
        var list = new List<string>(Config.ReadAll("Key3"));
        Assert.AreEqual(1, list.Count);
        Assert.AreEqual("Value 3", list[0]);
    }

    [TestMethod]
    public void Config_MultiWrite() {
        using var loader = new ConfigLoader(null, resourceFileNameGood: "WriteMulti.Good.cfg");
        Config.Write("Key1", "Value 1");
        Config.Write("Key2", new string[] { "Value 2a", "Value 2b", "Value 2c" });
        Config.Write("Key3", "Value 3");
        Config.Save();
        Assert.AreEqual(loader.GoodText, File.ReadAllText(loader.FileName));

        Assert.AreEqual("Value 1", Config.Read("Key1"));
        Assert.AreEqual("Value 3", Config.Read("Key3"));

        var list = new List<string>(Config.ReadAll("Key2"));
        Assert.AreEqual(3, list.Count);
        Assert.AreEqual("Value 2a", list[0]);
        Assert.AreEqual("Value 2b", list[1]);
        Assert.AreEqual("Value 2c", list[2]);
    }

    [TestMethod]
    public void Config_MultiReplace() {
        using var loader = new ConfigLoader("WriteMulti.cfg", resourceFileNameGood: "WriteMulti.Good.cfg");
        Config.Write("Key2", new string[] { "Value 2a", "Value 2b", "Value 2c" });
        Config.Save();
        Assert.AreEqual(loader.GoodText, File.ReadAllText(loader.FileName));

        Assert.AreEqual("Value 1", Config.Read("Key1"));
        Assert.AreEqual("Value 3", Config.Read("Key3"));

        var list = new List<string>(Config.ReadAll("Key2"));
        Assert.AreEqual(3, list.Count);
        Assert.AreEqual("Value 2a", list[0]);
        Assert.AreEqual("Value 2b", list[1]);
        Assert.AreEqual("Value 2c", list[2]);
    }

    [TestMethod]
    public void Config_DontOverwriteOverrideMulti() {
        using var loader = new ConfigLoader("ReplaceOnlyLast.Good.cfg", resourceOverrideFileName: "RemoveMulti.cfg");
        Config.Write("Key2", "Value X");
        var list = new List<string>(Config.ReadAll("Key2"));
        Assert.AreEqual(3, list.Count);
        Assert.AreEqual("Value 2a", list[0]);
        Assert.AreEqual("Value 2b", list[1]);
        Assert.AreEqual("Value 2c", list[2]);
    }


    [TestMethod]
    public void Config_TestConversion() {
        using var loader = new ConfigLoader(null, resourceFileNameGood: "WriteConverted.Good.cfg");
        Config.Write("Integer", 42);
        Config.Write("Integer Min", int.MinValue);
        Config.Write("Integer Max", int.MaxValue);
        Config.Write("Long", 42L);
        Config.Write("Long Min", long.MinValue);
        Config.Write("Long Max", long.MaxValue);
        Config.Write("Boolean", true);
        Config.Write("Double", 42.42);
        Config.Write("Double Pi", System.Math.PI);
        Config.Write("Double Third", 1.0 / 3);
        Config.Write("Double Seventh", 1.0 / 7);
        Config.Write("Double Min", double.MinValue);
        Config.Write("Double Max", double.MaxValue);
        Config.Write("Double NaN", double.NaN);
        Config.Write("Double Infinity+", double.PositiveInfinity);
        Config.Write("Double Infinity-", double.NegativeInfinity);

        Config.Save();
        Assert.AreEqual(loader.GoodText, File.ReadAllText(loader.FileName));

        using var loader2 = new ConfigLoader(loader.FileName, resourceFileNameGood: "WriteConverted.Good.cfg");
        Assert.AreEqual(42, Config.Read("Integer", 0));
        Assert.AreEqual(int.MinValue, Config.Read("Integer Min", 0));
        Assert.AreEqual(int.MaxValue, Config.Read("Integer Max", 0));
        Assert.AreEqual(42, Config.Read("Long", 0L));
        Assert.AreEqual(long.MinValue, Config.Read("Long Min", 0L));
        Assert.AreEqual(long.MaxValue, Config.Read("Long Max", 0L));
        Assert.IsTrue(Config.Read("Boolean", false));
        Assert.AreEqual(42.42, Config.Read("Double", 0.0));
        Assert.AreEqual(System.Math.PI, Config.Read("Double Pi", 0.0));
        Assert.AreEqual(1.0 / 3, Config.Read("Double Third", 0.0));
        Assert.AreEqual(1.0 / 7, Config.Read("Double Seventh", 0.0));
        Assert.AreEqual(double.MinValue, Config.Read("Double Min", 0.0));
        Assert.AreEqual(double.MaxValue, Config.Read("Double Max", 0.0));
        Assert.AreEqual(double.NaN, Config.Read("Double NaN", 0.0));
        Assert.AreEqual(double.PositiveInfinity, Config.Read("Double Infinity+", 0.0));
        Assert.AreEqual(double.NegativeInfinity, Config.Read("Double Infinity-", 0.0));
    }

    [TestMethod]
    public void Config_KeyWhitespace() {
        using var loader = new ConfigLoader("KeyWhitespace.cfg", "KeyWhitespace.Good.cfg");
        Config.Save();

        Assert.AreEqual(loader.GoodText, File.ReadAllText(loader.FileName));

        Assert.AreEqual("Value 1", Config.Read("Key 1"));
        Assert.AreEqual("Value 3", Config.Read("Key 3"));

        var list = new List<string>(Config.ReadAll("Key 2"));
        Assert.AreEqual(3, list.Count);
        Assert.AreEqual("Value 2a", list[0]);
        Assert.AreEqual("Value 2b", list[1]);
        Assert.AreEqual("Value 2c", list[2]);
    }


    [TestMethod]
    public void Config_DeleteAll() {
        using var loader = new ConfigLoader("WriteBasic.Good.cfg", "Empty.cfg");
        Assert.AreEqual("Value 1", Config.Read("Key1"));
        Assert.AreEqual("Value 2", Config.Read("Key2"));
        Config.DeleteAll();

        Assert.IsNull(Config.Read("Key1"));
        Assert.IsNull(Config.Read("Key2"));

        Assert.AreNotEqual(loader.GoodText, File.ReadAllText(loader.FileName)); //check that changes are not saved

        Config.Save();
        Assert.AreEqual(loader.GoodText, File.ReadAllText(loader.FileName)); //check that changes are saved
    }


    #region Utils

    private class ConfigLoader : IDisposable {

        public string FileName { get; }
        public byte[] Bytes { get; }
        public byte[] GoodBytes { get; }

        public ConfigLoader(string resourceFileName, string resourceFileNameGood = null, string resourceOverrideFileName = null) {
            Config.Reset();

            if (File.Exists(resourceFileName)) {
                Bytes = File.ReadAllBytes(resourceFileName);
            } else {
                Bytes = (resourceFileName != null) ? GetResourceStreamBytes(resourceFileName) : null;
            }
            GoodBytes = (resourceFileNameGood != null) ? GetResourceStreamBytes(resourceFileNameGood) : null;
            var overrideBytes = (resourceOverrideFileName != null) ? GetResourceStreamBytes(resourceOverrideFileName) : null;

            FileName = Path.GetTempFileName();
            if (resourceFileName == null) {
                File.Delete(FileName); //to start fresh
            } else {
                File.WriteAllBytes(FileName, Bytes);
            }

            Config.FileName = FileName;

            var overrideFileName = (resourceOverrideFileName != null) ? Path.GetTempFileName() : null;
            if (overrideFileName != null) {
                File.WriteAllBytes(overrideFileName, overrideBytes);
                Config.OverrideFileName = overrideFileName;
            } else {
                Config.OverrideFileName = null;
            }

            Config.ImmediateSave = false;
        }

        private readonly Encoding Utf8 = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
        public string Text { get => Utf8.GetString(Bytes); }
        public string GoodText { get => Utf8.GetString(GoodBytes ?? Array.Empty<byte>()); }

        #region IDisposable Support

        ~ConfigLoader() {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing) {
            try {
                File.Delete(FileName);
            } catch (IOException) { }
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

    }

    private static byte[] GetResourceStreamBytes(string fileName) {
        var resAssembly = typeof(Config_Tests).GetTypeInfo().Assembly;
        var resStream = resAssembly.GetManifestResourceStream("Tests._Resources.Configuration.Config." + fileName);
        var buffer = new byte[(int)resStream.Length];
        resStream.Read(buffer, 0, buffer.Length);
        return buffer;
    }

    #endregion
}
