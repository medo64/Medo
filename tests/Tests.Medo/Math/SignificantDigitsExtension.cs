using Microsoft.VisualStudio.TestTools.UnitTesting;
using Medo.Math;

namespace Tests;

[TestClass]
public class SignificantDigitsExtension_Tests {

    private const double Delta = 0.0000000000001;

    [TestMethod]
    public void SignificantDigitsExtension_P1() {
        Assert.AreEqual(0.001, 0.001234.ToSignificantDigits(1), Delta);
        Assert.AreEqual(0.01, 0.01234.ToSignificantDigits(1), Delta);
        Assert.AreEqual(0.1, 0.1234.ToSignificantDigits(1), Delta);
        Assert.AreEqual(1.0, 1.234.ToSignificantDigits(1), Delta);
        Assert.AreEqual(10.0, 12.34.ToSignificantDigits(1), Delta);
        Assert.AreEqual(100.0, 123.4.ToSignificantDigits(1), Delta);
        Assert.AreEqual(1000.0, 1234.0.ToSignificantDigits(1), Delta);
    }

    [TestMethod]
    public void SignificantDigitsExtension_P2() {
        Assert.AreEqual(0.0012, 0.001234.ToSignificantDigits(2), Delta);
        Assert.AreEqual(0.012, 0.01234.ToSignificantDigits(2), Delta);
        Assert.AreEqual(0.12, 0.1234.ToSignificantDigits(2), Delta);
        Assert.AreEqual(1.2, 1.234.ToSignificantDigits(2), Delta);
        Assert.AreEqual(12.0, 12.34.ToSignificantDigits(2), Delta);
        Assert.AreEqual(120.0, 123.4.ToSignificantDigits(2), Delta);
        Assert.AreEqual(1200.0, 1234.0.ToSignificantDigits(2), Delta);
    }

    [TestMethod]
    public void SignificantDigitsExtension_N1() {
        Assert.AreEqual(-0.001, -0.001234.ToSignificantDigits(1), Delta);
        Assert.AreEqual(-0.01, -0.01234.ToSignificantDigits(1), Delta);
        Assert.AreEqual(-0.1, -0.1234.ToSignificantDigits(1), Delta);
        Assert.AreEqual(-1.0, -1.234.ToSignificantDigits(1), Delta);
        Assert.AreEqual(-10.0, -12.34.ToSignificantDigits(1), Delta);
        Assert.AreEqual(-100.0, -123.4.ToSignificantDigits(1), Delta);
        Assert.AreEqual(-1000.0, -1234.0.ToSignificantDigits(1), Delta);
    }

    [TestMethod]
    public void SignificantDigitsExtension_N2() {
        Assert.AreEqual(-0.0012, -0.001234.ToSignificantDigits(2), Delta);
        Assert.AreEqual(-0.012, -0.01234.ToSignificantDigits(2), Delta);
        Assert.AreEqual(-0.12, -0.1234.ToSignificantDigits(2), Delta);
        Assert.AreEqual(-1.2, -1.234.ToSignificantDigits(2), Delta);
        Assert.AreEqual(-12.0, -12.34.ToSignificantDigits(2), Delta);
        Assert.AreEqual(-120.0, -123.4.ToSignificantDigits(2), Delta);
        Assert.AreEqual(-1200.0, -1234.0.ToSignificantDigits(2), Delta);
    }

}
