using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Xunit;
using Medo.Configuration;

namespace Tests.Medo.Configuration {
    public class ConfigTests {

        [Fact(DisplayName = "Config: Null key throws exception")]
        public void NullKey() {
            var ex = Assert.Throws<ArgumentNullException>(() => {
                Config.Read(null, "");
            });
            Assert.StartsWith("Key cannot be null.", ex.Message);
        }

        [Fact(DisplayName = "Config: Empty key throws exception")]
        public void EmptyKey() {
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => {
                Config.Read("   ", "");
            });
            Assert.StartsWith("Key cannot be empty.", ex.Message);
        }


        [Fact(DisplayName = "Config: Empty file Load/Save")]
        public void EmptySave() {
            using var loader = new ConfigLoader("Empty.cfg");
            Assert.True(Config.Load(), "File should exist before load.");
            Assert.True(Config.Save(), "Save should succeed.");
            Assert.Equal(BitConverter.ToString(loader.Bytes), BitConverter.ToString(File.ReadAllBytes(loader.FileName)));
            Assert.False(Config.IsAssumedInstalled);
        }

        [Fact(DisplayName = "Config: Installation status assumption")]
        public void AssumeInstalled() {
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
                Assert.False(Config.IsAssumedInstalled);
                Assert.NotNull(Config.FileName);
                Assert.Null(Config.OverrideFileName);
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

                Assert.True(Config.IsAssumedInstalled);
                Assert.NotNull(Config.FileName);
                Assert.NotNull(Config.OverrideFileName);
            } finally { //clean files
                if (Directory.Exists(userFileDirectory)) { Directory.Delete(userFileDirectory, true); }
                if (File.Exists(localFileLocation)) { File.Delete(localFileLocation); }
            }

            try {
                Config.Reset();

                Assert.False(Config.IsAssumedInstalled);
                Assert.NotNull(Config.FileName);
                Assert.Null(Config.OverrideFileName);
            } finally { //clean files
                if (Directory.Exists(userFileDirectory)) { Directory.Delete(userFileDirectory, true); }
                if (File.Exists(localFileLocation)) { File.Delete(localFileLocation); }
            }
        }


        [Fact(DisplayName = "Config: CRLF preserved on Save")]
        public void EmptyLinesCRLF() {
            using var loader = new ConfigLoader("EmptyLinesCRLF.cfg");
            Config.Save();
            Assert.Equal(BitConverter.ToString(loader.Bytes), BitConverter.ToString(File.ReadAllBytes(loader.FileName)));
        }

        [Fact(DisplayName = "Config: LF preserved on Save")]
        public void EmptyLinesLF() {
            using var loader = new ConfigLoader("EmptyLinesLF.cfg");
            Config.Save();
            Assert.Equal(BitConverter.ToString(loader.Bytes), BitConverter.ToString(File.ReadAllBytes(loader.FileName)));
        }

        [Fact(DisplayName = "Config: CR preserved on Save")]
        public void EmptyLinesCR() {
            using var loader = new ConfigLoader("EmptyLinesCR.cfg");
            Config.Save();
            Assert.Equal(BitConverter.ToString(loader.Bytes), BitConverter.ToString(File.ReadAllBytes(loader.FileName)));
        }

        [Fact(DisplayName = "Config: Mixed line ending gets normalized on Save")]
        public void EmptyLinesMixed() {
            using var loader = new ConfigLoader("EmptyLinesMixed.cfg", "EmptyLinesMixed.Good.cfg");
            Config.Save();
            Assert.Equal(loader.GoodText, File.ReadAllText(loader.FileName));
        }

        [Fact(DisplayName = "Config: Comments are preserved on Save")]
        public void CommentsOnly() {
            using var loader = new ConfigLoader("CommentsOnly.cfg", "CommentsOnly.Good.cfg");
            Config.Save();
            Assert.Equal(BitConverter.ToString(loader.GoodBytes), BitConverter.ToString(File.ReadAllBytes(loader.FileName)));
        }

        [Fact(DisplayName = "Config: Values with comments are preserved on Save")]
        public void CommentsWithValues() {
            using var loader = new ConfigLoader("CommentsWithValues.cfg");
            Config.Save();
            Assert.Equal(Encoding.UTF8.GetString(loader.Bytes), Encoding.UTF8.GetString(File.ReadAllBytes(loader.FileName)));
            Assert.Equal(BitConverter.ToString(loader.Bytes), BitConverter.ToString(File.ReadAllBytes(loader.FileName)));
        }

        [Fact(DisplayName = "Config: Leading spaces are preserved on Save")]
        public void SpacingEscape() {
            using var loader = new ConfigLoader("SpacingEscape.cfg", "SpacingEscape.Good.cfg");
            Assert.Equal(" Value 1", Config.Read("Key1"));
            Assert.Equal("Value 2 ", Config.Read("Key2"));
            Assert.Equal(" Value 3 ", Config.Read("Key3"));
            Assert.Equal("  Value 4  ", Config.Read("Key4"));
            Assert.Equal("\tValue 5\t", Config.Read("Key5"));
            Assert.Equal("\tValue 6", Config.Read("Key6"));
            Assert.Equal("\0", Config.Read("Null"));

            Config.Save();
            Config.Write("Null", "\0Null\0");
            Config.Save();

            Assert.Equal(loader.GoodText, File.ReadAllText(loader.FileName));
        }

        [Fact(DisplayName = "Config: Basic write")]
        public void WriteBasic() {
            using var loader = new ConfigLoader("Empty.cfg", "WriteBasic.Good.cfg");
            Config.Write("Key1", "Value 1");
            Config.Write("Key2", "Value 2");

            Config.Save();

            Assert.Equal(loader.GoodText, File.ReadAllText(loader.FileName));
        }

        [Fact(DisplayName = "Config: Basic write (without empty line ending)")]
        public void WriteNoEmptyLine() {
            using var loader = new ConfigLoader("WriteNoEmptyLine.cfg", "WriteNoEmptyLine.Good.cfg");
            Config.Write("Key1", "Value 1");
            Config.Write("Key2", "Value 2");

            Config.Save();

            Assert.Equal(loader.GoodText, File.ReadAllText(loader.FileName));
        }

        [Fact(DisplayName = "Config: Separator equals (=) is preserved upon save")]
        public void WriteSameSeparatorEquals() {
            using var loader = new ConfigLoader("WriteSameSeparatorEquals.cfg", "WriteSameSeparatorEquals.Good.cfg");
            Config.Write("Key1", "Value 1");
            Config.Write("Key2", "Value 2");

            Config.Save();

            Assert.Equal(loader.GoodText, File.ReadAllText(loader.FileName));
        }

        [Fact(DisplayName = "Config: Separator space ( ) is preserved upon save")]
        public void WriteSameSeparatorSpace() {
            using var loader = new ConfigLoader("WriteSameSeparatorSpace.cfg", "WriteSameSeparatorSpace.Good.cfg");
            Config.Write("Key1", "Value 1");
            Config.Write("Key2", "Value 2");

            Config.Save();

            Assert.Equal(loader.GoodText, File.ReadAllText(loader.FileName));
        }

        [Fact(DisplayName = "Config: Write replaces existing entry")]
        public void Replace() {
            using var loader = new ConfigLoader("Replace.cfg", "Replace.Good.cfg");
            Config.Write("Key1", "Value 1a");
            Config.Write("Key2", "Value 2a");

            Config.Save();

            Assert.Equal("Value 1a", Config.Read("Key1"));
            Assert.Equal("Value 2a", Config.Read("Key2"));

            Assert.Equal(loader.GoodText, File.ReadAllText(loader.FileName));
        }

        [Fact(DisplayName = "Config: Write preserves spacing")]
        public void SpacingPreserved() {
            using var loader = new ConfigLoader("SpacingPreserved.cfg", "SpacingPreserved.Good.cfg");
            Config.Write("KeyOne", "Value 1a");
            Config.Write("KeyTwo", "Value 2b");
            Config.Write("KeyThree", "Value 3c");

            Config.Save();

            Assert.Equal(loader.GoodText, File.ReadAllText(loader.FileName));
        }

        [Fact(DisplayName = "Config: Write preserves spacing on add")]
        public void SpacingPreservedOnAdd() {
            using var loader = new ConfigLoader("SpacingPreservedOnAdd.cfg", "SpacingPreservedOnAdd.Good.cfg");
            Config.Write("One", "Value 1a");
            Config.Write("Two", new string[] { "Value 2a", "Value 2b" });
            Config.Write("Three", "Value 3a");
            Config.Write("Four", "Value 4a");
            Config.Write("Five", new string[] { "Value 5a", "Value 5b", "Value 5c" });
            Config.Write("FourtyTwo", 42);

            Config.Save();

            Assert.Equal(loader.GoodText, File.ReadAllText(loader.FileName));
        }

        [Fact(DisplayName = "Config: Write without preexisting file")]
        public void WriteToEmpty() {
            using var loader = new ConfigLoader(null, "Replace.Good.cfg");
            Config.Write("Key1", "Value 1a");
            Config.Write("Key2", "Value 2a");

            Config.Save();

            Assert.Equal(loader.GoodText, File.ReadAllText(loader.FileName));
        }

        [Fact(DisplayName = "Config: Write replaces only last instance of same key")]
        public void ReplaceOnlyLast() {
            using var loader = new ConfigLoader("ReplaceOnlyLast.cfg", "ReplaceOnlyLast.Good.cfg");
            Config.Write("Key1", "Value 1a");
            Config.Write("Key2", "Value 2a");

            Config.Save();

            Assert.Equal("Value 1a", Config.Read("Key1"));
            Assert.Equal("Value 2a", Config.Read("Key2"));
            Assert.Equal("Value 3", Config.Read("Key3"));

            Assert.Equal(loader.GoodText, File.ReadAllText(loader.FileName));
        }


        [Fact(DisplayName = "Config: Write creates directory")]
        public void SaveInNonexistingDirectory1() {
            var propertiesFile = Path.Combine(Path.GetTempPath(), "ConfigDirectory", "Test.cfg");
            try {
                Directory.Delete(Path.Combine(Path.GetTempPath(), "ConfigDirectory"), true);
            } catch (IOException) { }
            Config.FileName = propertiesFile;

            Assert.False(Config.Load(), "No file present for load.");

            var x = Config.Read("Test", "test");
            Assert.Equal("test", x);

            Assert.True(Config.Save(), "Save should create directory structure and succeed.");
            Assert.True(File.Exists(propertiesFile));

            Directory.Delete(Path.Combine(Path.GetTempPath(), "ConfigDirectory"), true);
        }

        [Fact(DisplayName = "Config: Write creates directory (2 levels deep)")]
        public void SaveInNonexistingDirectory2() {
            var propertiesFile = Path.Combine(Path.GetTempPath(), "ConfigDirectoryOuter", "ConfigDirectoryInner", "Test.cfg");
            try {
                Directory.Delete(Path.Combine(Path.GetTempPath(), "ConfigDirectoryOuter"), true);
            } catch (IOException) { }
            Config.FileName = propertiesFile;

            Assert.False(Config.Load(), "No file present for load.");

            var x = Config.Read("Test", "test");
            Assert.Equal("test", x);

            Assert.True(Config.Save(), "Save should create directory structure and succeed.");
            Assert.True(File.Exists(propertiesFile));

            Directory.Delete(Path.Combine(Path.GetTempPath(), "ConfigDirectoryOuter"), true);
        }

        [Fact(DisplayName = "Config: Write creates directory (3 levels deep)")]
        public void SaveInNonexistingDirectory3() {
            var propertiesFile = Path.Combine(Path.GetTempPath(), "ConfigDirectoryOuter", "ConfigDirectoryMiddle", "ConfigDirectoryInner", "Test.cfg");
            try {
                Directory.Delete(Path.Combine(Path.GetTempPath(), "ConfigDirectoryOuter"), true);
            } catch (IOException) { }
            Config.FileName = propertiesFile;

            Assert.False(Config.Load(), "No file present for load.");

            var x = Config.Read("Test", "test");
            Assert.Equal("test", x);

            Assert.True(Config.Save(), "Save should create directory structure and succeed.");
            Assert.True(File.Exists(propertiesFile));

            Directory.Delete(Path.Combine(Path.GetTempPath(), "ConfigDirectoryOuter"), true);
        }


        [Fact(DisplayName = "Config: Removing entry")]
        public void RemoveSingle() {
            using var loader = new ConfigLoader("Remove.cfg", "Remove.Good.cfg");
            Config.Delete("Key1");
            Config.Save();

            Assert.Equal(loader.GoodText, File.ReadAllText(loader.FileName));
        }

        [Fact(DisplayName = "Config: Removing multiple entries")]
        public void RemoveMulti() {
            using var loader = new ConfigLoader("RemoveMulti.cfg", "RemoveMulti.Good.cfg");
            Config.Delete("Key2");
            Config.Save();

            Assert.Equal(loader.GoodText, File.ReadAllText(loader.FileName));
        }


        [Fact(DisplayName = "Config: Override is used first")]
        public void UseOverrideFirst() {
            using var loader = new ConfigLoader("Replace.cfg", resourceOverrideFileName: "Replace.Good.cfg");
            Assert.Equal("Value 1a", Config.Read("Key1"));
        }

        [Fact(DisplayName = "Config: Override is not written")]
        public void DontOverwriteOverride() {
            using var loader = new ConfigLoader("Replace.cfg", resourceOverrideFileName: "Replace.Good.cfg");
            Config.ImmediateSave = true;

            Config.Write("Key1", "XXX");
            Assert.Equal("Value 1a", Config.Read("Key1"));
            Config.OverrideFileName = null;
            Assert.Equal("XXX", Config.Read("Key1"));
        }


        [Fact(DisplayName = "Config: Reading multiple entries")]
        public void ReadMulti() {
            using var loader = new ConfigLoader("ReplaceOnlyLast.Good.cfg");
            var list = new List<string>(Config.ReadAll("Key2"));
            Assert.Equal(2, list.Count);
            Assert.Equal("Value 2", list[0]);
            Assert.Equal("Value 2a", list[1]);
        }

        [Fact(DisplayName = "Config: Reading multiple entries from override")]
        public void ReadMultiFromOverride() {
            using var loader = new ConfigLoader("ReplaceOnlyLast.Good.cfg", resourceOverrideFileName: "RemoveMulti.cfg");
            var list = new List<string>(Config.ReadAll("Key2"));
            Assert.Equal(3, list.Count);
            Assert.Equal("Value 2a", list[0]);
            Assert.Equal("Value 2b", list[1]);
            Assert.Equal("Value 2c", list[2]);
        }

        [Fact(DisplayName = "Config: Reading multi entries when override is not found")]
        public void ReadMultiFromOverrideNotFound() {
            using var loader = new ConfigLoader("ReplaceOnlyLast.Good.cfg", resourceOverrideFileName: "RemoveMulti.cfg");
            var list = new List<string>(Config.ReadAll("Key3"));
            Assert.Single(list);
            Assert.Equal("Value 3", list[0]);
        }

        [Fact(DisplayName = "Config: Multi-value write")]
        public void MultiWrite() {
            using var loader = new ConfigLoader(null, resourceFileNameGood: "WriteMulti.Good.cfg");
            Config.Write("Key1", "Value 1");
            Config.Write("Key2", new string[] { "Value 2a", "Value 2b", "Value 2c" });
            Config.Write("Key3", "Value 3");
            Config.Save();
            Assert.Equal(loader.GoodText, File.ReadAllText(loader.FileName));

            Assert.Equal("Value 1", Config.Read("Key1"));
            Assert.Equal("Value 3", Config.Read("Key3"));

            var list = new List<string>(Config.ReadAll("Key2"));
            Assert.Equal(3, list.Count);
            Assert.Equal("Value 2a", list[0]);
            Assert.Equal("Value 2b", list[1]);
            Assert.Equal("Value 2c", list[2]);
        }

        [Fact(DisplayName = "Config: Multi-value replace")]
        public void MultiReplace() {
            using var loader = new ConfigLoader("WriteMulti.cfg", resourceFileNameGood: "WriteMulti.Good.cfg");
            Config.Write("Key2", new string[] { "Value 2a", "Value 2b", "Value 2c" });
            Config.Save();
            Assert.Equal(loader.GoodText, File.ReadAllText(loader.FileName));

            Assert.Equal("Value 1", Config.Read("Key1"));
            Assert.Equal("Value 3", Config.Read("Key3"));

            var list = new List<string>(Config.ReadAll("Key2"));
            Assert.Equal(3, list.Count);
            Assert.Equal("Value 2a", list[0]);
            Assert.Equal("Value 2b", list[1]);
            Assert.Equal("Value 2c", list[2]);
        }

        [Fact(DisplayName = "Config: Multi-value override is not written")]
        public void DontOverwriteOverrideMulti() {
            using var loader = new ConfigLoader("ReplaceOnlyLast.Good.cfg", resourceOverrideFileName: "RemoveMulti.cfg");
            Config.Write("Key2", "Value X");
            var list = new List<string>(Config.ReadAll("Key2"));
            Assert.Equal(3, list.Count);
            Assert.Equal("Value 2a", list[0]);
            Assert.Equal("Value 2b", list[1]);
            Assert.Equal("Value 2c", list[2]);
        }


        [Fact(DisplayName = "Config: Test conversion")]
        public void TestConversion() {
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
            Assert.Equal(loader.GoodText, File.ReadAllText(loader.FileName));

            using var loader2 = new ConfigLoader(loader.FileName, resourceFileNameGood: "WriteConverted.Good.cfg");
            Assert.Equal(42, Config.Read("Integer", 0));
            Assert.Equal(int.MinValue, Config.Read("Integer Min", 0));
            Assert.Equal(int.MaxValue, Config.Read("Integer Max", 0));
            Assert.Equal(42, Config.Read("Long", 0L));
            Assert.Equal(long.MinValue, Config.Read("Long Min", 0L));
            Assert.Equal(long.MaxValue, Config.Read("Long Max", 0L));
            Assert.True(Config.Read("Boolean", false));
            Assert.Equal(42.42, Config.Read("Double", 0.0));
            Assert.Equal(System.Math.PI, Config.Read("Double Pi", 0.0));
            Assert.Equal(1.0 / 3, Config.Read("Double Third", 0.0));
            Assert.Equal(1.0 / 7, Config.Read("Double Seventh", 0.0));
            Assert.Equal(double.MinValue, Config.Read("Double Min", 0.0));
            Assert.Equal(double.MaxValue, Config.Read("Double Max", 0.0));
            Assert.Equal(double.NaN, Config.Read("Double NaN", 0.0));
            Assert.Equal(double.PositiveInfinity, Config.Read("Double Infinity+", 0.0));
            Assert.Equal(double.NegativeInfinity, Config.Read("Double Infinity-", 0.0));
        }

        [Fact(DisplayName = "Config: Key whitespace reading and saving")]
        public void KeyWhitespace() {
            using var loader = new ConfigLoader("KeyWhitespace.cfg", "KeyWhitespace.Good.cfg");
            Config.Save();

            Assert.Equal(loader.GoodText, File.ReadAllText(loader.FileName));

            Assert.Equal("Value 1", Config.Read("Key 1"));
            Assert.Equal("Value 3", Config.Read("Key 3"));

            var list = new List<string>(Config.ReadAll("Key 2"));
            Assert.Equal(3, list.Count);
            Assert.Equal("Value 2a", list[0]);
            Assert.Equal("Value 2b", list[1]);
            Assert.Equal("Value 2c", list[2]);
        }


        [Fact(DisplayName = "Config: Delete all values")]
        public void DeleteAll() {
            using var loader = new ConfigLoader("WriteBasic.Good.cfg", "Empty.cfg");
            Assert.Equal("Value 1", Config.Read("Key1"));
            Assert.Equal("Value 2", Config.Read("Key2"));
            Config.DeleteAll();

            Assert.Null(Config.Read("Key1"));
            Assert.Null(Config.Read("Key2"));

            Assert.NotEqual(loader.GoodText, File.ReadAllText(loader.FileName)); //check that changes are not saved

            Config.Save();
            Assert.Equal(loader.GoodText, File.ReadAllText(loader.FileName)); //check that changes are saved
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
            var resAssembly = typeof(ConfigTests).GetTypeInfo().Assembly;
            var resStream = resAssembly.GetManifestResourceStream("Tests.Medo._Resources.Configuration.Config." + fileName);
            var buffer = new byte[(int)resStream.Length];
            resStream.Read(buffer, 0, buffer.Length);
            return buffer;
        }

        #endregion

    }
}
