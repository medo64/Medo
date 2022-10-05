using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Medo.Extensions.HexString;

namespace Tests;

[TestClass]
public class HexStringExtension_Tests {

    [TestMethod]
    public void HexStringExtension_BasicSpan() {
        var span = new byte[] { 1, 2, 42 }.AsSpan();
        Assert.AreEqual("01022a", span.ToHexString());
        Assert.AreEqual("022a", span[1..].ToHexString());

        Assert.AreEqual("01-02-2A", BitConverter.ToString("01022a".FromHexString()));

    }

    [TestMethod]
    public void HexStringExtension_BasicArray() {
        var array = new byte[] { 1, 2, 42 };
        Assert.AreEqual("01022a", array.ToHexString());
        Assert.AreEqual("022a", array.ToHexString(1, 2));

        Assert.AreEqual("01-02-2A", BitConverter.ToString("01022a".FromHexString()));

    }

    [TestMethod]
    public void HexStringExtension_InvalidArrayPosition() {
        var array = new byte[1];
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => { array.ToHexString(1, 1); });
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => { array.ToHexString(0, -1); });
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => { array.ToHexString(-1, 1); });
        Assert.AreEqual("00", array.ToHexString(0, 1));
        Assert.AreEqual("", array.ToHexString(1, 0));
    }

    [TestMethod]
    public void HexStringExtension_Null() {
        Assert.ThrowsException<ArgumentNullException>(() => {
            byte[] array = null;
            Assert.IsNull(array.ToHexString());
        });

        Assert.ThrowsException<ArgumentNullException>(() => {
            string text = null;
            Assert.IsNull(text.FromHexString());
        });
    }

}
