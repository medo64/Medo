using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Medo.Security.Cryptography;
using System.Reflection;

namespace Tests;

[TestClass]
public class ProtectedBuffer_Tests {

    [DataTestMethod]
    [DataRow(new byte[] { }, "")]
    [DataRow(new byte[] { 2 }, "02")]
    [DataRow(new byte[] { 2, 3 }, "02-03")]
    [DataRow(new byte[] { 2, 3, 5 }, "02-03-05")]
    [DataRow(new byte[] { 2, 3, 5, 7 }, "02-03-05-07")]
    [DataRow(new byte[] { 2, 3, 5, 7, 11 }, "02-03-05-07-0B")]
    [DataRow(new byte[] { 2, 3, 5, 7, 11, 13 }, "02-03-05-07-0B-0D")]
    [DataRow(new byte[] { 2, 3, 5, 7, 11, 13, 17 }, "02-03-05-07-0B-0D-11")]
    [DataRow(new byte[] { 2, 3, 5, 7, 11, 13, 17, 19 }, "02-03-05-07-0B-0D-11-13")]
    [DataRow(new byte[] { 2, 3, 5, 7, 11, 13, 17, 19, 23 }, "02-03-05-07-0B-0D-11-13-17")]
    [DataRow(new byte[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29 }, "02-03-05-07-0B-0D-11-13-17-1D")]
    [DataRow(new byte[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31 }, "02-03-05-07-0B-0D-11-13-17-1D-1F")]
    [DataRow(new byte[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37 }, "02-03-05-07-0B-0D-11-13-17-1D-1F-25")]
    [DataRow(new byte[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41 }, "02-03-05-07-0B-0D-11-13-17-1D-1F-25-29")]
    [DataRow(new byte[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43 }, "02-03-05-07-0B-0D-11-13-17-1D-1F-25-29-2B")]
    [DataRow(new byte[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47 }, "02-03-05-07-0B-0D-11-13-17-1D-1F-25-29-2B-2F")]
    [DataRow(new byte[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53 }, "02-03-05-07-0B-0D-11-13-17-1D-1F-25-29-2B-2F-35")]
    [DataRow(new byte[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59 }, "02-03-05-07-0B-0D-11-13-17-1D-1F-25-29-2B-2F-35-3B")]
    public void ProtectedBuffer_Basic(byte[] inputData, string expectedHex) {
        using var protectedBuffer = new ProtectedBuffer();
        protectedBuffer.ProtectData(inputData);

        var outputData = protectedBuffer.UnprotectData();
        try {
            Assert.AreEqual(inputData.Length, outputData.Length);
            Assert.AreEqual(expectedHex, BitConverter.ToString(outputData));
        } finally {
            Array.Clear(outputData);
        }
    }

    [DataTestMethod]
    [DataRow(new byte[] { }, "")]
    [DataRow(new byte[] { 2 }, "02")]
    [DataRow(new byte[] { 2, 3 }, "02-03")]
    [DataRow(new byte[] { 2, 3, 5 }, "02-03-05")]
    [DataRow(new byte[] { 2, 3, 5, 7 }, "02-03-05-07")]
    [DataRow(new byte[] { 2, 3, 5, 7, 11 }, "02-03-05-07-0B")]
    [DataRow(new byte[] { 2, 3, 5, 7, 11, 13 }, "02-03-05-07-0B-0D")]
    [DataRow(new byte[] { 2, 3, 5, 7, 11, 13, 17 }, "02-03-05-07-0B-0D-11")]
    [DataRow(new byte[] { 2, 3, 5, 7, 11, 13, 17, 19 }, "02-03-05-07-0B-0D-11-13")]
    [DataRow(new byte[] { 2, 3, 5, 7, 11, 13, 17, 19, 23 }, "02-03-05-07-0B-0D-11-13-17")]
    [DataRow(new byte[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29 }, "02-03-05-07-0B-0D-11-13-17-1D")]
    [DataRow(new byte[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31 }, "02-03-05-07-0B-0D-11-13-17-1D-1F")]
    [DataRow(new byte[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37 }, "02-03-05-07-0B-0D-11-13-17-1D-1F-25")]
    [DataRow(new byte[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41 }, "02-03-05-07-0B-0D-11-13-17-1D-1F-25-29")]
    [DataRow(new byte[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43 }, "02-03-05-07-0B-0D-11-13-17-1D-1F-25-29-2B")]
    [DataRow(new byte[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47 }, "02-03-05-07-0B-0D-11-13-17-1D-1F-25-29-2B-2F")]
    [DataRow(new byte[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53 }, "02-03-05-07-0B-0D-11-13-17-1D-1F-25-29-2B-2F-35")]
    [DataRow(new byte[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59 }, "02-03-05-07-0B-0D-11-13-17-1D-1F-25-29-2B-2F-35-3B")]
    public void ProtectedBuffer_ExactBuffer(byte[] inputData, string expectedHex) {
        using var protectedBuffer = new ProtectedBuffer();
        protectedBuffer.ProtectData(inputData);

        var outputData = new byte[inputData.Length];
        Assert.AreEqual(true, protectedBuffer.TryUnprotectData(outputData, out var dataLength));
        Assert.AreEqual(inputData.Length, dataLength);
        Assert.AreEqual(expectedHex, BitConverter.ToString(outputData, 0, dataLength));
    }

    [DataTestMethod]
    [DataRow(new byte[] { }, "")]
    [DataRow(new byte[] { 2 }, "02")]
    [DataRow(new byte[] { 2, 3 }, "02-03")]
    [DataRow(new byte[] { 2, 3, 5 }, "02-03-05")]
    [DataRow(new byte[] { 2, 3, 5, 7 }, "02-03-05-07")]
    [DataRow(new byte[] { 2, 3, 5, 7, 11 }, "02-03-05-07-0B")]
    [DataRow(new byte[] { 2, 3, 5, 7, 11, 13 }, "02-03-05-07-0B-0D")]
    [DataRow(new byte[] { 2, 3, 5, 7, 11, 13, 17 }, "02-03-05-07-0B-0D-11")]
    [DataRow(new byte[] { 2, 3, 5, 7, 11, 13, 17, 19 }, "02-03-05-07-0B-0D-11-13")]
    [DataRow(new byte[] { 2, 3, 5, 7, 11, 13, 17, 19, 23 }, "02-03-05-07-0B-0D-11-13-17")]
    [DataRow(new byte[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29 }, "02-03-05-07-0B-0D-11-13-17-1D")]
    [DataRow(new byte[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31 }, "02-03-05-07-0B-0D-11-13-17-1D-1F")]
    [DataRow(new byte[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37 }, "02-03-05-07-0B-0D-11-13-17-1D-1F-25")]
    [DataRow(new byte[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41 }, "02-03-05-07-0B-0D-11-13-17-1D-1F-25-29")]
    [DataRow(new byte[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43 }, "02-03-05-07-0B-0D-11-13-17-1D-1F-25-29-2B")]
    [DataRow(new byte[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47 }, "02-03-05-07-0B-0D-11-13-17-1D-1F-25-29-2B-2F")]
    [DataRow(new byte[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53 }, "02-03-05-07-0B-0D-11-13-17-1D-1F-25-29-2B-2F-35")]
    [DataRow(new byte[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59 }, "02-03-05-07-0B-0D-11-13-17-1D-1F-25-29-2B-2F-35-3B")]
    public void ProtectedBuffer_LargeBuffer(byte[] inputData, string expectedHex) {
        using var protectedBuffer = new ProtectedBuffer();
        protectedBuffer.ProtectData(inputData);

        var outputData = new byte[4096];
        Assert.AreEqual(true, protectedBuffer.TryUnprotectData(outputData, out var dataLength));
        Assert.AreEqual(inputData.Length, dataLength);
        Assert.AreEqual(expectedHex, BitConverter.ToString(outputData, 0, dataLength));
    }

    [DataTestMethod]
    [DataRow(new byte[] { 2 }, "")]
    [DataRow(new byte[] { 2, 3 }, "00")]
    [DataRow(new byte[] { 2, 3, 5 }, "00-00")]
    [DataRow(new byte[] { 2, 3, 5, 7 }, "00-00-00")]
    [DataRow(new byte[] { 2, 3, 5, 7, 11 }, "00-00-00-00")]
    [DataRow(new byte[] { 2, 3, 5, 7, 11, 13 }, "00-00-00-00-00")]
    [DataRow(new byte[] { 2, 3, 5, 7, 11, 13, 17 }, "00-00-00-00-00-00")]
    [DataRow(new byte[] { 2, 3, 5, 7, 11, 13, 17, 19 }, "00-00-00-00-00-00-00")]
    [DataRow(new byte[] { 2, 3, 5, 7, 11, 13, 17, 19, 23 }, "00-00-00-00-00-00-00-00")]
    [DataRow(new byte[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29 }, "00-00-00-00-00-00-00-00-00")]
    [DataRow(new byte[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31 }, "00-00-00-00-00-00-00-00-00-00")]
    [DataRow(new byte[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37 }, "00-00-00-00-00-00-00-00-00-00-00")]
    [DataRow(new byte[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41 }, "00-00-00-00-00-00-00-00-00-00-00-00")]
    [DataRow(new byte[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43 }, "00-00-00-00-00-00-00-00-00-00-00-00-00")]
    [DataRow(new byte[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47 }, "00-00-00-00-00-00-00-00-00-00-00-00-00-00")]
    [DataRow(new byte[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53 }, "00-00-00-00-00-00-00-00-00-00-00-00-00-00-00")]
    [DataRow(new byte[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59 }, "00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00")]
    public void ProtectedBuffer_TooShort(byte[] inputData, string expectedHex) {
        using var protectedBuffer = new ProtectedBuffer();
        protectedBuffer.ProtectData(inputData);

        var outputData = new byte[inputData.Length - 1];
        Assert.AreEqual(false, protectedBuffer.TryUnprotectData(outputData, out var dataLength));
        Assert.AreEqual(0, dataLength);
        Assert.AreEqual(expectedHex, BitConverter.ToString(outputData));
    }

    [DataTestMethod]
    [DataRow(new byte[] { 2 })]
    [DataRow(new byte[] { 2, 3 })]
    [DataRow(new byte[] { 2, 3, 5 })]
    [DataRow(new byte[] { 2, 3, 5, 7 })]
    [DataRow(new byte[] { 2, 3, 5, 7, 11 })]
    [DataRow(new byte[] { 2, 3, 5, 7, 11, 13 })]
    [DataRow(new byte[] { 2, 3, 5, 7, 11, 13, 17 })]
    [DataRow(new byte[] { 2, 3, 5, 7, 11, 13, 17, 19 })]
    [DataRow(new byte[] { 2, 3, 5, 7, 11, 13, 17, 19, 23 })]
    [DataRow(new byte[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29 })]
    [DataRow(new byte[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31 })]
    [DataRow(new byte[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37 })]
    [DataRow(new byte[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41 })]
    [DataRow(new byte[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43 })]
    [DataRow(new byte[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47 })]
    [DataRow(new byte[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53 })]
    [DataRow(new byte[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59 })]
    public void ProtectedBuffer_MessWithInternalArray(byte[] inputData) {
        using var protectedBuffer = new ProtectedBuffer();
        protectedBuffer.ProtectData(inputData);

        var dataBytesPrivateField = typeof(ProtectedBuffer).GetField("DataBytes", BindingFlags.Instance | BindingFlags.NonPublic);
        var dataBytes = (byte[])dataBytesPrivateField.GetValue(protectedBuffer);
        dataBytes[inputData.Length] -= 1;

        var outputData = new byte[4096];
        Assert.AreEqual(false, protectedBuffer.TryUnprotectData(outputData, out var dataLength));
        Assert.AreEqual(0, dataLength);
    }

}
