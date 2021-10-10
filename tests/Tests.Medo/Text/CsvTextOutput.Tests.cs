using System;
using System.IO;
using System.Reflection;
using Xunit;
using Medo.Text;

namespace Tests.Medo.Text {
    public class CsvTextOutputTests {

        [Fact(DisplayName = "CsvTextOutput: Basic")]
        public void Basic() {
            var templateLines = GetLines("Basic.csv");
            var stream = new MemoryStream();
            using var csv = new CsvTextOutput(stream);
            csv.WriteHeader("A", "B", "C");
            csv.WriteData("11", "12", "13");
            csv.WriteData("21", "22", "23");
            csv.WriteData("3X", "3Y", "3Z");
            var lines = GetLines(stream);

            Assert.Equal(templateLines.Length, lines.Length);
            for (var i = 0; i < lines.Length; i++) {
                Assert.Equal(templateLines[i], lines[i]);
            }
        }

        [Fact(DisplayName = "CsvTextOutput: Escaping")]
        public void Escaping() {
            var templateLines = GetLines("Escaping.csv");
            var stream = new MemoryStream();
            using var csv = new CsvTextOutput(stream);
            csv.WriteHeader("Column Space", "Column,Comma", "Column\"Quotes", "Column'Apostrophe");
            csv.WriteData("3.14", "6.28", "27-Jan", "18-May");
            csv.WriteData("A B", "X,Y", "C\"D", "E'F");
            var lines = GetLines(stream);

            Assert.Equal(templateLines.Length, lines.Length);
            for (var i = 0; i < lines.Length; i++) {
                Assert.Equal(templateLines[i], lines[i]);
            }
        }

        [Fact(DisplayName = "CsvTextOutput: Incomplete")]
        public void Incomplete() {
            var templateLines = GetLines("Incomplete.csv");
            var stream = new MemoryStream();
            using var csv = new CsvTextOutput(stream);
            csv.WriteHeader("A", "B", "C");
            csv.WriteData("1", "2", "3");
            csv.WriteData("4", "5");
            csv.WriteData("6");
            csv.WriteData();
            var lines = GetLines(stream);

            Assert.Equal(templateLines.Length, lines.Length);
            for (var i = 0; i < lines.Length; i++) {
                Assert.Equal(templateLines[i], lines[i]);
            }
        }


        [Fact(DisplayName = "CsvTextOutput: Constructor errors")]
        public void ConstructorErrors() {
            Assert.Throws<ArgumentNullException>(delegate {
                var csv = new CsvTextOutput(default(Stream));
            });
            Assert.Throws<ArgumentOutOfRangeException>(delegate {
                var csv = new CsvTextOutput(new MemoryStream(Array.Empty<byte>(), writable: false));
            });
            Assert.Throws<ArgumentNullException>(delegate {
                var csv = new CsvTextOutput(default(string));
            });
        }

        [Fact(DisplayName = "CsvTextOutput: Header errors")]
        public void HeaderErrors() {
            var csv = new CsvTextOutput(new MemoryStream());
            Assert.Throws<ArgumentNullException>(delegate {
                csv.WriteHeader(null);  // no null header
            });
            Assert.Throws<ArgumentOutOfRangeException>(delegate {
                csv.WriteHeader();  // no empty header
            });
            csv.WriteHeader("A");
            Assert.Throws<InvalidOperationException>(delegate {
                csv.WriteHeader("B");  // no double header
            });
        }

        [Fact(DisplayName = "CsvTextOutput: Data errors")]
        public void DataErrors() {
            var csv = new CsvTextOutput(new MemoryStream());
            Assert.Throws<InvalidOperationException>(delegate {
                csv.WriteData();  // no data before header
            });
            csv.WriteHeader("A");
            Assert.Throws<ArgumentNullException>(delegate {
                csv.WriteData(null);  // no null data
            });
            Assert.Throws<ArgumentOutOfRangeException>(delegate {
                csv.WriteData("1", "2");  // no data longer than header
            });
        }

        [Fact(DisplayName = "CsvTextOutput: Separator errors")]
        public void SeparatorErrors() {
            var csv = new CsvTextOutput(new MemoryStream());
            Assert.Throws<ArgumentOutOfRangeException>(delegate {
                csv.Separator = '.';  // unknown separator
            });

            csv.Separator = ',';
            csv.Separator = ';';

            csv.WriteHeader("A");
            Assert.Throws<InvalidOperationException>(delegate {
                csv.Separator = ';';  // cannot set after write
            });
        }

        [Fact(DisplayName = "CsvTextOutput: NewLine errors")]
        public void NewLineErrors() {
            var csv = new CsvTextOutput(new MemoryStream());
            Assert.Throws<ArgumentOutOfRangeException>(delegate {
                csv.NewLine = ".";  // unknown new line
            });

            csv.NewLine = "\n";
            csv.NewLine = "\r\n";
            csv.NewLine = "\r";

            Assert.Throws<ArgumentNullException>(delegate {
                csv.NewLine = null;  // no null
            });

            csv.WriteHeader("A");
            Assert.Throws<InvalidOperationException>(delegate {
                csv.NewLine = "\n";  // cannot set after write
            });
        }


        #region Private

        private static string[] GetLines(MemoryStream stream) {
            stream.Position = 0;
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd().Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
        }

        private static string[] GetLines(string name) {
            using var stream = GetStream(name);
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd().Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
        }

        private static Stream GetStream(string name) {
            var streamName = "Tests.Medo._Resources.Text.CsvTextOutput." + name;
            return Assembly.GetExecutingAssembly().GetManifestResourceStream(streamName);
        }

        #endregion Private

    }
}
