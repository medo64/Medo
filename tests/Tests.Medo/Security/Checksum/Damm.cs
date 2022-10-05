using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Medo.Security.Checksum;

namespace Tests;

[TestClass]
public class Damm_Tests {

#pragma warning disable CS0618 // Type or member is obsolete

    [TestMethod]
    public void Damm_Basic() {
        using var checksum = new Damm();
        var hash = checksum.ComputeHash(new byte[] { 0x35, 0x37, 0x32 });
        Assert.AreEqual("34", BitConverter.ToString(hash));
        Assert.AreEqual(4, checksum.HashAsNumber);
        Assert.AreEqual('4', checksum.HashAsChar);
    }

    [TestMethod]
    public void Damm_BasicAscii() {
        using var checksum = new Damm();
        var hash = checksum.ComputeHash(Encoding.ASCII.GetBytes("572"));
        Assert.AreEqual("4", Encoding.ASCII.GetString(hash));
        Assert.AreEqual(4, checksum.HashAsNumber);
        Assert.AreEqual('4', checksum.HashAsChar);
    }

    [TestMethod]
    public void Damm_BasicUtf8() {
        using var checksum = new Damm();
        var hash = checksum.ComputeHash(Encoding.UTF8.GetBytes("572"));
        Assert.AreEqual("4", Encoding.UTF8.GetString(hash));
        Assert.AreEqual(4, checksum.HashAsNumber);
        Assert.AreEqual('4', checksum.HashAsChar);
    }

    [TestMethod]
    public void Damm_BasicUtf8WithPrefix() {
        using var checksum = new Damm();
        var hash = checksum.ComputeHash(Encoding.UTF8.GetBytes("00572")); //any number of zeros can be added at front
        Assert.AreEqual("4", Encoding.UTF8.GetString(hash));
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
        using var checksum = new Damm();
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => {
            var hash = checksum.ComputeHash(new byte[] { 5, 7, 2 });
        });
    }

    [TestMethod]
    public void Damm_InvalidCharactersABC() {
        using var checksum = new Damm();
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => {
            var hash = checksum.ComputeHash(Encoding.UTF8.GetBytes("ABC"));
        });
    }


    [TestMethod]
    public void Damm_Reuse() {
        var crc = new Damm();
        crc.ComputeHash(Encoding.ASCII.GetBytes("4428922675"));
        Assert.AreEqual(6, crc.HashAsNumber);
        Assert.AreEqual("36", BitConverter.ToString(crc.Hash).Replace("-", ""));
        crc.ComputeHash(Encoding.ASCII.GetBytes("4428922675"));
        Assert.AreEqual(6, crc.HashAsNumber);
        Assert.AreEqual("36", BitConverter.ToString(crc.Hash).Replace("-", ""));
    }

#pragma warning restore CS0618 // Type or member is obsolete

}
