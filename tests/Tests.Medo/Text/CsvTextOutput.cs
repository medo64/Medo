using System;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Medo.Text;

namespace Tests;

[TestClass]
public class CsvTextOutput_Tests {

    [TestMethod]
    public void CsvTextOutput_Basic() {
        var templateLines = GetLines("Basic.csv");
        var stream = new MemoryStream();
        using var csv = new CsvTextOutput(stream);
        csv.WriteHeader("A", "B", "C");
        csv.WriteData("11", "12", "13");
        csv.WriteData("21", "22", "23");
        csv.WriteData("3X", "3Y", "3Z");
        var lines = GetLines(stream);

        Assert.AreEqual(templateLines.Length, lines.Length);
        for (var i = 0; i < lines.Length; i++) {
            Assert.AreEqual(templateLines[i], lines[i]);
        }
    }

    [TestMethod]
    public void CsvTextOutput_Escaping() {
        var templateLines = GetLines("Escaping.csv");
        var stream = new MemoryStream();
        using var csv = new CsvTextOutput(stream);
        csv.WriteHeader("Column Space", "Column,Comma", "Column\"Quotes", "Column'Apostrophe");
        csv.WriteData("3.14", "6.28", "27-Jan", "18-May");
        csv.WriteData("A B", "X,Y", "C\"D", "E'F");
        var lines = GetLines(stream);

        Assert.AreEqual(templateLines.Length, lines.Length);
        for (var i = 0; i < lines.Length; i++) {
            Assert.AreEqual(templateLines[i], lines[i]);
        }
    }

    [TestMethod]
    public void CsvTextOutput_Incomplete() {
        var templateLines = GetLines("Incomplete.csv");
        var stream = new MemoryStream();
        using var csv = new CsvTextOutput(stream);
        csv.WriteHeader("A", "B", "C");
        csv.WriteData("1", "2", "3");
        csv.WriteData("4", "5");
        csv.WriteData("6");
        csv.WriteData();
        var lines = GetLines(stream);

        Assert.AreEqual(templateLines.Length, lines.Length);
        for (var i = 0; i < lines.Length; i++) {
            Assert.AreEqual(templateLines[i], lines[i]);
        }
    }

    [TestMethod]
    public void CsvTextOutput_ConstructorErrors() {
        Assert.ThrowsException<ArgumentNullException>(delegate {
            var csv = new CsvTextOutput(default(Stream));
        });
        Assert.ThrowsException<ArgumentOutOfRangeException>(delegate {
            var csv = new CsvTextOutput(new MemoryStream(Array.Empty<byte>(), writable: false));
        });
        Assert.ThrowsException<ArgumentNullException>(delegate {
            var csv = new CsvTextOutput(default(string));
        });
    }

    [TestMethod]
    public void CsvTextOutput_HeaderErrors() {
        var csv = new CsvTextOutput(new MemoryStream());
        Assert.ThrowsException<ArgumentNullException>(delegate {
            csv.WriteHeader(null);  // no null header
        });
        Assert.ThrowsException<ArgumentOutOfRangeException>(delegate {
            csv.WriteHeader();  // no empty header
        });
        csv.WriteHeader("A");
        Assert.ThrowsException<InvalidOperationException>(delegate {
            csv.WriteHeader("B");  // no double header
        });
    }

    [TestMethod]
    public void CsvTextOutput_DataErrors() {
        var csv = new CsvTextOutput(new MemoryStream());
        Assert.ThrowsException<InvalidOperationException>(delegate {
            csv.WriteData();  // no data before header
        });
        csv.WriteHeader("A");
        Assert.ThrowsException<ArgumentNullException>(delegate {
            csv.WriteData(null);  // no null data
        });
        Assert.ThrowsException<ArgumentOutOfRangeException>(delegate {
            csv.WriteData("1", "2");  // no data longer than header
        });
    }

    [TestMethod]
    public void CsvTextOutput_SeparatorErrors() {
        var csv = new CsvTextOutput(new MemoryStream());
        Assert.ThrowsException<ArgumentOutOfRangeException>(delegate {
            csv.Separator = '.';  // unknown separator
        });

        csv.Separator = ',';
        csv.Separator = ';';

        csv.WriteHeader("A");
        Assert.ThrowsException<InvalidOperationException>(delegate {
            csv.Separator = ';';  // cannot set after write
        });
    }

    [TestMethod]
    public void CsvTextOutput_NewLineErrors() {
        var csv = new CsvTextOutput(new MemoryStream());
        Assert.ThrowsException<ArgumentOutOfRangeException>(delegate {
            csv.NewLine = ".";  // unknown new line
        });

        csv.NewLine = "\n";
        csv.NewLine = "\r\n";
        csv.NewLine = "\r";

        Assert.ThrowsException<ArgumentNullException>(delegate {
            csv.NewLine = null;  // no null
        });

        csv.WriteHeader("A");
        Assert.ThrowsException<InvalidOperationException>(delegate {
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
        var streamName = "Tests._Resources.Text.CsvTextOutput." + name;
        return Assembly.GetExecutingAssembly().GetManifestResourceStream(streamName);
    }

    #endregion Private

}
