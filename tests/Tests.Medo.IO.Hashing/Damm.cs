using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Medo.IO.Hashing;

namespace Tests;

[TestClass]
public class Damm_Tests {

    [TestMethod]
    public void Damm_Basic() {
        var checksum = new Damm();
        checksum.Append(new byte[] { 0x35, 0x37, 0x32 });
        Assert.AreEqual("34", BitConverter.ToString(checksum.GetCurrentHash()));
        Assert.AreEqual(4, checksum.HashAsNumber);
        Assert.AreEqual('4', checksum.HashAsChar);
    }

    [TestMethod]
    public void Damm_BasicAscii() {
        var checksum = new Damm();
        checksum.Append(Encoding.ASCII.GetBytes("572"));
        Assert.AreEqual("4", Encoding.ASCII.GetString(checksum.GetCurrentHash()));
        Assert.AreEqual(4, checksum.HashAsNumber);
        Assert.AreEqual('4', checksum.HashAsChar);
    }

    [TestMethod]
    public void Damm_BasicUtf8() {
        var checksum = new Damm();
        checksum.Append(Encoding.UTF8.GetBytes("572"));
        Assert.AreEqual("4", Encoding.UTF8.GetString(checksum.GetCurrentHash()));
        Assert.AreEqual(4, checksum.HashAsNumber);
        Assert.AreEqual('4', checksum.HashAsChar);
    }

    [TestMethod]
    public void Damm_BasicUtf8WithPrefix() {
        var checksum = new Damm();
        checksum.Append(Encoding.UTF8.GetBytes("00572")); //any number of zeros can be added at front
        Assert.AreEqual("4", Encoding.UTF8.GetString(checksum.GetCurrentHash()));
        Assert.AreEqual(4, checksum.HashAsNumber);
        Assert.AreEqual('4', checksum.HashAsChar);
    }


    [TestMethod]
    public void Damm_HelperComputeHash() {
        var hash = Damm.ComputeHash("572");
        Assert.AreEqual("4", hash);
    }

    [TestMethod]
    public void Damm_HelperComputeHashSpaces() {
        var hash = Damm.ComputeHash(" 5 7 2 ");
        Assert.AreEqual("4", hash);
    }

    [TestMethod]
    public void Damm_HelperComputeHashDashes() {
        var hash = Damm.ComputeHash("05-72");
        Assert.AreEqual("4", hash);
    }


    [TestMethod]
    public void Damm_HelperComputeHashFull() {
        var hash = Damm.ComputeHash("572", returnAllDigits: true);
        Assert.AreEqual("5724", hash);
    }

    [TestMethod]
    public void Damm_HelperComputeHashSpacesFull() {
        var hash = Damm.ComputeHash(" 5 7 2 ", returnAllDigits: true);
        Assert.AreEqual("5724", hash);
    }

    [TestMethod]
    public void Damm_HelperComputeHashDashesFull() {
        var hash = Damm.ComputeHash("05-72", returnAllDigits: true);
        Assert.AreEqual("05724", hash);
    }


    [TestMethod]
    public void Damm_HelperValidateHash() {
        var result = Damm.ValidateHash("5724");
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Damm_HelperValidateHashSpaces() {
        var result = Damm.ValidateHash(" 57 24 ");
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Damm_HelperValidateHashDashes() {
        var result = Damm.ValidateHash("-00-57-24-");
        Assert.IsTrue(result);
    }


    [TestMethod]
    public void Damm_HelperValidateHashFails() {
        var result = Damm.ValidateHash("5720");
        Assert.IsFalse(result);
    }


    [TestMethod]
    public void Damm_InvalidCharacters572() {
        var checksum = new Damm();
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => {
            checksum.Append(new byte[] { 5, 7, 2 });
        });
    }

    [TestMethod]
    public void Damm_InvalidCharactersABC() {
        var checksum = new Damm();
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => {
            checksum.Append(Encoding.UTF8.GetBytes("ABC"));
        });
    }


    [TestMethod]
    public void Damm_Reuse() {
        var crc = new Damm();
        crc.Append(Encoding.ASCII.GetBytes("4428922675"));
        Assert.AreEqual(6, crc.HashAsNumber);
        Assert.AreEqual("36", BitConverter.ToString(crc.GetHashAndReset()).Replace("-", ""));
        crc.Append(Encoding.ASCII.GetBytes("4428922675"));
        Assert.AreEqual(6, crc.HashAsNumber);
        Assert.AreEqual("36", BitConverter.ToString(crc.GetCurrentHash()).Replace("-", ""));
    }

}
