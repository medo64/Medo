using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Medo.IO.Hashing;

namespace Tests;

[TestClass]
public class Crc64_Tests {

    [TestMethod]
    public void Crc64_Custom() {
        var crc = Crc64.GetCustom(0x42F0E1EBA9EA3693, 0x0000000000000000, true, true, 0x0000000000000000);
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x2B9C7EE4E2780C8AUL, crc.HashAsUInt64);
    }

    [TestMethod]
    public void Crc64_Custom_2() {
        var crc = Crc64.GetCustom(0x42F0E1EBA9EA3693, 0x0000000000000000, true, true, 0x0000000000000000);
        crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("C8026C2DE7116FF8", $"{crc.HashAsUInt64:X16}");
        Assert.AreEqual("F86F11E72D6C02C8", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }

    [TestMethod]
    public void Crc64_Default() {
        var crcDotNet = new System.IO.Hashing.Crc64();
        crcDotNet.Append(Encoding.ASCII.GetBytes("123456789"));

        var crc = Crc64.GetDefault();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x6C40DF5F0B497347UL, crc.HashAsUInt64);

        Assert.AreEqual(BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""),
                        BitConverter.ToString(crcDotNet.GetCurrentHash()).Replace("-", ""));
    }


    [TestMethod]  // ECMA-182
    public void Crc64_Ecma182() {
        var crc = Crc64.GetEcma182();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x6C40DF5F0B497347UL, crc.HashAsUInt64);
    }

    [TestMethod]  // ECMA-182
    public void Crc64_Ecma182_2() {
        var crc = Crc64.GetEcma182();
        crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("347543C2ECE5EAD2", $"{crc.HashAsUInt64:X16}");
        Assert.AreEqual("347543C2ECE5EAD2",  BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }

    [TestMethod]  // GO-ISO
    public void Crc64_GoIso() {
        var crc = Crc64.GetGoIso();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xB90956C775A41001UL, crc.HashAsUInt64);
    }

    [TestMethod]  // GO-ISO
    public void Crc64_GoIso_2() {
        var crc = Crc64.GetGoIso();
        crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("E79495727E142029", $"{crc.HashAsUInt64:X16}");
        Assert.AreEqual("2920147E729594E7", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }

    [TestMethod]  // MS
    public void Crc64_Ms() {
        var crc = Crc64.GetMs();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x75D4B74F024ECEEAUL, crc.HashAsUInt64);
    }

    [TestMethod]  // MS
    public void Crc64_Ms_2() {
        var crc = Crc64.GetMs();
        crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("971B8382DF43B770", $"{crc.HashAsUInt64:X16}");
        Assert.AreEqual("70B743DF82831B97", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }

    [TestMethod]  // REDIS
    public void Crc64_Redis() {
        var crc = Crc64.GetRedis();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0xE9C6D914C4B8D9CA, crc.HashAsUInt64);
    }

    [TestMethod]  // REDIS
    public void Crc64_Redis_2() {
        var crc = Crc64.GetRedis();
        crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("60FED901EB026F01", $"{crc.HashAsUInt64:X16}");
        Assert.AreEqual("016F02EB01D9FE60", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }

    [TestMethod]  // WE
    public void Crc64_We() {
        var crc = Crc64.GetWe();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x62EC59E3F1A4F00AUL, crc.HashAsUInt64);
    }

    [TestMethod]  // WE
    public void Crc64_We_2() {
        var crc = Crc64.GetWe();
        crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("0861C6A8270A1D82", $"{crc.HashAsUInt64:X16}");
        Assert.AreEqual("0861C6A8270A1D82", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }

    [TestMethod]  // XZ
    public void Crc64_Xz() {
        var crc = Crc64.GetXz();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x995DC9BBDF1939FAUL, crc.HashAsUInt64);
    }

    [TestMethod]  // XZ
    public void Crc64_Xz_2() {
        var crc = Crc64.GetXz();
        crc.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("C2ED9BFEB1B047C4", $"{crc.HashAsUInt64:X16}");
        Assert.AreEqual("C447B0B1FE9BEDC2", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }

    [TestMethod]  // GO-ECMA
    public void Crc64_GoEcma() {
        var crc = Crc64.GetGoEcma();
        crc.Append(Encoding.ASCII.GetBytes("123456789"));
        Assert.AreEqual(0x995DC9BBDF1939FAUL, crc.HashAsUInt64);
    }


    [TestMethod]
    public void Crc64_Reuse() {
        var checksum = Crc64.GetEcma182();
        checksum.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("347543C2ECE5EAD2", BitConverter.ToString(checksum.GetHashAndReset()).Replace("-", ""));
        checksum.Append(Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        Assert.AreEqual("347543C2ECE5EAD2", BitConverter.ToString(checksum.GetCurrentHash()).Replace("-", ""));
    }


    [TestMethod]
    public void Crc64_ToReversedReciprocal() {
        unchecked {
            Assert.AreEqual(0xA17870F5D4F51B49UL, Crc64.ToReversedReciprocalPolynomial(0x42F0E1EBA9EA3693UL));
            Assert.AreEqual(0x800000000000000DUL, Crc64.ToReversedReciprocalPolynomial(0x000000000000001BUL));

            Assert.AreEqual(0xD6C9E91ACA649AD4UL, Crc64.ToReversedReciprocalPolynomial(0xAD93D23594C935A9UL));
            Assert.AreEqual(0x800000000000000DUL, Crc64.ToReversedReciprocalPolynomial(0x000000000000001BUL));
            Assert.AreEqual(0x800000000000002BUL, Crc64.ToReversedReciprocalPolynomial(0x0000000000000057UL));
        }
    }

    [TestMethod]
    public void Crc64_FromReversedReciprocal() {
        unchecked {
            Assert.AreEqual(0x42F0E1EBA9EA3693UL, Crc64.FromReversedReciprocalPolynomial(0xA17870F5D4F51B49UL));
            Assert.AreEqual(0x000000000000001BUL, Crc64.FromReversedReciprocalPolynomial(0x800000000000000DUL));

            Assert.AreEqual(0xAD93D23594C935A9UL, Crc64.FromReversedReciprocalPolynomial(0xD6C9E91ACA649AD4UL));
            Assert.AreEqual(0x000000000000001BUL, Crc64.FromReversedReciprocalPolynomial(0x800000000000000DUL));
            Assert.AreEqual(0x0000000000000057UL, Crc64.FromReversedReciprocalPolynomial(0x800000000000002BUL));
        }
    }

}
