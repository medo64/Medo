/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

//2021-11-25: Refactored to use pattern matching
//2021-03-16: Initial version

namespace Medo.Text;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

/// <summary>
/// Writing comma separated values.
/// </summary>
/// <example>
/// <code>
/// using var csv = new CsvTextOutput(stream);
/// csv.WriteHeader("A", "B", "C");
/// csv.WriteData("11", "12", "13");
/// csv.WriteData("21", "22", "23");
/// </code>
/// <example>
public sealed class CsvTextOutput : IDisposable {

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="stream">Stream used for writing.</param>
    /// <exception cref="ArgumentNullException">Stream cannot be null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Stream must be writable.</exception>
    public CsvTextOutput(Stream stream) {
        if (stream == null) { throw new ArgumentNullException(nameof(stream), "Stream cannot be null."); }
        if (!stream.CanWrite) { throw new ArgumentOutOfRangeException(nameof(stream), "Stream must be writable."); }
        Stream = stream;
    }

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="fileName">File name for output file.</param>
    /// <exception cref="ArgumentNullException">File name cannot be null.</exception>
    public CsvTextOutput(string fileName) {
        if (fileName == null) { throw new ArgumentNullException(nameof(fileName), "File name cannot be null."); }
        Stream = File.OpenWrite(fileName);
    }


    private char _separator = ',';
    /// <summary>
    /// Gets/sets separator.
    /// Only comma (,) and semicolon (;) are allowed.
    /// Default value is comma (,).
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Only comma and semicolon are supported characters.</exception>
    /// <exception cref="InvalidOperationException">Cannot change separator after starting to write.</exception>
    public char Separator {
        get { return _separator; }
        set {
            if (HeaderFieldCount > 0) { throw new InvalidOperationException("Cannot change separator after starting to write."); }
            if (value is not ',' and not ';') { throw new ArgumentOutOfRangeException(nameof(value), "Only comma and semicolon are supported characters."); }
            _separator = value;
        }
    }


    private string _newLine = Environment.NewLine;
    /// <summary>
    /// Gets/sets new line character.
    /// Only line feed (LF), line feed/carriage return (CR), and carriage return (CR) are allowed.
    /// Default value is same as Environment.NewLine.
    /// </summary>
    /// <exception cref="ArgumentNullException">Value cannot be null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Only LF, CRLF, and CR are supported characters.</exception>
    /// <exception cref="InvalidOperationException">Cannot change separator after starting to write.</exception>
    public string NewLine {
        get { return _newLine; }
        set {
            if (HeaderFieldCount > 0) { throw new InvalidOperationException("Cannot change separator after starting to write."); }
            if (value == null) { throw new ArgumentNullException(nameof(value), "Value cannot be null."); }
            if (value is not "\n" and not "\r\n" and not "\r") { throw new ArgumentOutOfRangeException(nameof(value), "Only LF, CRLF, and CR are supported characters."); }
            _newLine = value;
        }
    }


    /// <summary>
    /// Writes header fields.
    /// </summary>
    /// <param name="headerFields">Header fields.</param>
    /// <exception cref="ArgumentNullException">Header fields cannot be null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Header fields cannot be empty.</exception>
    /// <exception cref="InvalidOperationException">Cannot write header twice.</exception>
    public void WriteHeader(params string[] headerFields) {
        if (headerFields == null) { throw new ArgumentNullException(nameof(headerFields), "Header fields cannot be null."); }
        if (headerFields.Length == 0) { throw new ArgumentOutOfRangeException(nameof(headerFields), "Header fields cannot be empty."); }
        if (HeaderFieldCount > 0) { throw new InvalidOperationException("Cannot write header twice."); }

        HeaderFieldCount = headerFields.Length;

        Stream.Write(Encoding.GetBytes(GetLineText(headerFields, headerFields.Length)));
    }

    /// <summary>
    /// Writes data.
    /// Number of fields must be same or lower than number of fields in header.
    /// </summary>
    /// <param name="dataFields">Data fields.</param>
    /// <exception cref="ArgumentNullException">Data fields cannot be null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Cannot have more data than header fields.</exception>
    /// <exception cref="InvalidOperationException">Cannot write data fields until headers are written.</exception>
    public void WriteData(params string[] dataFields) {
        if (dataFields == null) { throw new ArgumentNullException(nameof(dataFields), "Data fields cannot be null."); }
        if (HeaderFieldCount == 0) { throw new InvalidOperationException("Cannot write data fields until headers are written."); }
        if (dataFields.Length > HeaderFieldCount) { throw new ArgumentOutOfRangeException(nameof(dataFields), "Cannot have more data than header fields."); }

        Stream.Write(Encoding.GetBytes(GetLineText(dataFields, HeaderFieldCount)));
    }


    #region IDispose

    private bool disposedValue;

    private void Dispose(bool disposing) {
        if (!disposedValue) {
            if (disposing) {
                Stream.Flush();
                Stream.Dispose();
            }
            disposedValue = true;
        }
    }

    public void Dispose() {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    #endregion IDispose


    #region Internals

    private readonly Stream Stream;
    private readonly Encoding Encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

    private int HeaderFieldCount = 0;

    private string GetLineText(IEnumerable<string> columns, int columnCount) {
        var sb = new StringBuilder();

        var columnIndex = 0;
        foreach (var column in columns) {
            if (columnIndex > 0) { sb.Append(_separator); }

            var colStart = sb.Length;
            var needQuoting = false;
            foreach (var ch in column) {
                sb.Append(ch);
                if (ch == Separator) {
                    needQuoting = true;
                } else if (ch == '"') {
                    sb.Append('"');  // add extra quote
                    needQuoting = true;
                }
            }
            if (needQuoting) {
                sb.Insert(colStart, '"');
                sb.Append('"');
            }
            columnIndex += 1;
        }

        if (columnIndex == 0) { columnIndex = 1; }
        for (var i = columnIndex; i < columnCount; i++) {
            sb.Append(Separator);
        }

        sb.Append(_newLine);
        return sb.ToString();
    }

    #endregion Internals

}
