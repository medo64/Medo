using System;
using System.IO;
using System.Reflection;
using Xunit;

namespace Medo.Tests.Configuration.IniFile {
    using Medo.Configuration;

    public class Tests {

        [Fact(DisplayName = "IniFile: Empty")]
        public void Empty() {
            var ini = new IniFile();
            Assert.Equal(0, ini.Count);
        }

        [Fact(DisplayName = "IniFile: Null file name")]
        public void NullFile() {
            var ex = Assert.Throws<ArgumentNullException>(() => {
                _ = new IniFile(default(string));
            });
        }

        [Fact(DisplayName = "IniFile: Null stream")]
        public void NullStream() {
            var ex = Assert.Throws<ArgumentNullException>(() => {
                _ = new IniFile(default(Stream));
            });
        }

        [Fact(DisplayName = "IniFile: Missing file throws FileNotFoundException")]
        public void MissingFile() {
            var ex = Assert.Throws<FileNotFoundException>(() => {
                _ = new IniFile(@"C:\dummy-file-that-does-not-exist.txt");
            });
        }

        [Fact(DisplayName = "IniFile: Empty file")]
        public void EmptyFile() {
            var ini = new IniFile(GetResourceStream("Empty.ini"));
            Assert.Equal(0, ini.Count);
        }

        [Fact(DisplayName = "IniFile: Section has characters after end")]
        public void SectionCharactersAfterEnd() {
            var ini = new IniFile(GetResourceStream("SectionCharactersAfterEnd.ini"));
            Assert.Equal(1, ini.Count);

            Assert.Equal("Value", ini.Read("Section]Extra", "Key"));

            Assert.Single(ini.GetSections());
            Assert.Equal("Section]Extra", ini.GetSections()[0]);
        }

        [Fact(DisplayName = "IniFile: Section allows inner brackets")]
        public void SectionIgnoresInnerBrackets() {
            var ini = new IniFile(GetResourceStream("SectionIgnoresInnerBrackets.ini"));
            Assert.Equal(1, ini.Count);

            Assert.Equal("Value", ini.Read("Section[i]", "Key"));

            Assert.Single(ini.GetSections());
            Assert.Equal("Section[i]", ini.GetSections()[0]);
        }

        [Fact(DisplayName = "IniFile: Section ends upon the end of line")]
        public void SectionImplicitEnd() {
            var ini = new IniFile(GetResourceStream("SectionImplicitEnd.ini"));
            Assert.Equal(1, ini.Count);

            Assert.Equal("Value", ini.Read("Section", "Key"));

            Assert.Single(ini.GetSections());
            Assert.Equal("Section", ini.GetSections()[0]);
        }

        [Fact(DisplayName = "IniFile: Empty key name")]
        public void EmptyKey() {
            var ini = new IniFile(GetResourceStream("EmptyKey.ini"));
            Assert.Equal(1, ini.Count);

            Assert.Equal("Value", ini.Read("", ""));

            Assert.Single(ini.GetSections());
            Assert.Equal("", ini.GetSections()[0]);
        }

        [Fact(DisplayName = "IniFile: Comment")]
        public void Comment() {
            var ini = new IniFile(GetResourceStream("Comment.ini"));
            //Assert.Equal(1, ini.Count);

            Assert.Equal("Value", ini.Read("", "Key"));

            Assert.Single(ini.GetSections());
            Assert.Equal("", ini.GetSections()[0]);
        }

        [Fact(DisplayName = "IniFile: Conversions")]
        public void Conversions() {
            var ini = new IniFile(GetResourceStream("Conversions.ini"));
            Assert.Equal(23, ini.Count);

            Assert.Equal(5, ini.GetSections().Count);
            Assert.Equal("Boolean", ini.GetSections()[0]);
            Assert.Equal("Int32", ini.GetSections()[1]);
            Assert.Equal("Int64", ini.GetSections()[2]);
            Assert.Equal("Single", ini.GetSections()[3]);
            Assert.Equal("Double", ini.GetSections()[4]);

            Assert.True(ini.Read("Boolean", "True1", false));
            Assert.True(ini.Read("Boolean", "True2", false));
            Assert.True(ini.Read("Boolean", "True3", false));
            Assert.True(ini.Read("Boolean", "True4", false));
            Assert.True(ini.Read("Boolean", "True5", false));
            Assert.False(ini.Read("Boolean", "False1", true));
            Assert.False(ini.Read("Boolean", "False2", true));
            Assert.False(ini.Read("Boolean", "False3", true));
            Assert.False(ini.Read("Boolean", "False4", true));

            Assert.Equal(0, ini.Read("Int32", "Zero", 42));
            Assert.Equal(Int32.MinValue, ini.Read("Int32", "Min", 42));
            Assert.Equal(Int32.MaxValue, ini.Read("Int32", "Max", 42));

            Assert.Equal(0L, ini.Read("Int64", "Zero", 42L));
            Assert.Equal(Int64.MinValue, ini.Read("Int64", "Min", 42L));
            Assert.Equal(Int64.MaxValue, ini.Read("Int64", "Max", 42L));

            Assert.Equal(0.0F, ini.Read("Single", "Zero", 42.0F));
            Assert.Equal(Single.NaN, ini.Read("Single", "NaN", 42.0F));
            Assert.Equal(Single.MinValue, ini.Read("Single", "Min", 42.0F));
            Assert.Equal(Single.MaxValue, ini.Read("Single", "Max", 42.0F));

            Assert.Equal(0.0, ini.Read("Double", "Zero", 42.0));
            Assert.Equal(Double.NaN, ini.Read("Double", "NaN", 42.0));
            Assert.Equal(Double.MinValue, ini.Read("Double", "Min", 42.0));
            Assert.Equal(Double.MaxValue, ini.Read("Double", "Max", 42.0));
        }

        [Fact(DisplayName = "IniFile: Merge sections")]
        public void MergeSections() {
            var ini = new IniFile(GetResourceStream("MergeSections.ini"));
            Assert.Equal(3, ini.Count);

            Assert.Equal("A", ini.Read("Section", "Key1"));
            Assert.Equal("B", ini.Read("Section", "Key2"));
            Assert.Equal("C", ini.Read("Section", "Key3"));

            Assert.Single(ini.GetSections());
            Assert.Equal("SECTION", ini.GetSections()[0]);
        }

        [Fact(DisplayName = "IniFile: Ignore empty sections")]
        public void IgnoreEmptySections() {
            var ini = new IniFile(GetResourceStream("IgnoreEmptySections.ini"));
            Assert.Equal(0, ini.Count);

            Assert.Empty(ini.GetSections());
        }

        [Fact(DisplayName = "IniFile: Ignore incomplete properties")]
        public void IgnoreIncompleteProperties() {
            var ini = new IniFile(GetResourceStream("IgnoreIncompleteProperties.ini"));
            Assert.Equal(0, ini.Count);

            Assert.Empty(ini.GetSections());
        }

        [Fact(DisplayName = "IniFile: Starting whitespace")]
        public void WhitespaceAtStart() {
            var ini = new IniFile(GetResourceStream("WhitespaceAtStart.ini"));
            Assert.Equal(1, ini.Count);
            Assert.Equal("Value", ini.Read("Section", "Key"));
        }

        [Fact(DisplayName = "IniFile: Ending whitespace")]
        public void WhitespaceAtEnd() {
            var ini = new IniFile(GetResourceStream("WhitespaceAtEnd.ini"));
            Assert.Equal(1, ini.Count);
            Assert.Equal("Value", ini.Read("Section", "Key"));
        }

        [Fact(DisplayName = "IniFile: Singe quote")]
        public void SingleQuote() {
            var ini = new IniFile(GetResourceStream("SingleQuote.ini"));
            Assert.Equal(3, ini.Count);
            Assert.Equal("Value'd", ini.Read("Section", "Normal"));
            Assert.Equal(" Whitespace'd ", ini.Read("Section", "Whitespace"));
            Assert.Equal("Start Middle End", ini.Read("Section", "Multi"));
        }

        [Fact(DisplayName = "IniFile: Double quote")]
        public void DoubleQuote() {
            var ini = new IniFile(GetResourceStream("DoubleQuote.ini"));
            Assert.Equal(10, ini.Count);
            Assert.Equal("Value'd", ini.Read("Section", "Normal"));
            Assert.Equal(" Whitespace'd ", ini.Read("Section", "Whitespace"));
            Assert.Equal(" Start Middle End ", ini.Read("Section", "Multi"));
            Assert.Equal("\"Really\"", ini.Read("Section", "Quotes"));
            Assert.Equal("AçB", ini.Read("Section", "Short"));
            Assert.Equal("👽", ini.Read("Section", "Long"));
            Assert.Equal("Aç|ç|çB", ini.Read("Section", "Variable"));
            Assert.Equal("A", ini.Read("Section", "IncompleteShort"));
            Assert.Equal("A", ini.Read("Section", "IncompleteLong"));
            Assert.Equal("A", ini.Read("Section", "IncompleteVariable"));
        }

        [Fact(DisplayName = "IniFile: Mixed")]
        public void Mixed() {
            var ini = new IniFile(GetResourceStream("Mixed.ini"));
            Assert.Equal(9, ini.Count);
            Assert.Equal(" Start Middle End ", ini.Read("Section", "Quoting"));
            Assert.Equal("Normal", ini.Read("Section", "ExtraWhitespace1"));
            Assert.Equal("Normal", ini.Read("Section", "ExtraWhitespace2"));
            Assert.Equal("Normal A", ini.Read("Section", "ExtraWhitespace3"));
            Assert.Equal("Normal A", ini.Read("Section", "ExtraWhitespace4"));
            Assert.Equal(" Normal", ini.Read("Section", "ExtraWhitespace5"));
            Assert.Equal(" Normal", ini.Read("Section", "ExtraWhitespace6"));
            Assert.Equal("A Normal", ini.Read("Section", "ExtraWhitespace7"));
            Assert.Equal("A Normal", ini.Read("Section", "ExtraWhitespace8"));
        }


        #region Utils

        private static Stream GetResourceStream(string resourceName) {
            var resAssembly = typeof(Tests).GetTypeInfo().Assembly;
            return resAssembly.GetManifestResourceStream("Medo.Tests._Resources.Configuration.IniFile." + resourceName);
        }

        #endregion

    }
}
