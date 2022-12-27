using Microsoft.VisualStudio.TestTools.UnitTesting;
using Medo.Math;
using System;

namespace Tests;

[TestClass]
public class WeightedSelection_Tests {

    [TestMethod]
    public void WeightedSelection_OnlyOne() {
        var selector = new WeightedSelection<string>(42);
        selector.Add(1, "A");
        Assert.AreEqual("A", selector.GetItem());
        Assert.AreEqual("A", selector.GetItem());
        Assert.AreEqual("A", selector.GetItem());
    }

    [TestMethod]
    public void WeightedSelection_OnlyOneEnabled() {
        var selector = new WeightedSelection<string>(42);
        selector.Add(0, "N");
        selector.Add(1, "A");
        selector.Add(0, "N");
        Assert.AreEqual("A", selector.GetItem());
        Assert.AreEqual("A", selector.GetItem());
        Assert.AreEqual("A", selector.GetItem());
    }

    [TestMethod]
    public void WeightedSelection_NoEnabled() {
        var selector = new WeightedSelection<string>(42);
        selector.Add(0, "N");
        selector.Add(0, "N");
        selector.Add(0, "N");
        Assert.ThrowsException<InvalidOperationException>(() => {
            Assert.AreEqual("N", selector.GetItem());
        });
    }


    [TestMethod]
    public void WeightedSelection_SelectDisabledFromOnlyOneEnabled() {
        var selector = new WeightedSelection<string>(42) {
            ReturnDisabledIfNoOtherItemQualifies = true,
        };
        selector.Add(0, "N");
        selector.Add(1, "A");
        selector.Add(0, "N");
        Assert.AreEqual("A", selector.GetItem());
        Assert.AreEqual("A", selector.GetItem());
        Assert.AreEqual("A", selector.GetItem());
    }

    [TestMethod]
    public void WeightedSelection_SelectDisabledFromNoEnabled() {
        var selector = new WeightedSelection<string>(42) {
            ReturnDisabledIfNoOtherItemQualifies = true,
        };
        selector.Add(0, "N");
        selector.Add(0, "N");
        selector.Add(0, "N");
        Assert.AreEqual("N", selector.GetItem());
        Assert.AreEqual("N", selector.GetItem());
        Assert.AreEqual("N", selector.GetItem());
    }


    [TestMethod]
    public void WeightedSelection_FiftyFifty() {
        var selector = new WeightedSelection<string>(42) {
            ReturnDisabledIfNoOtherItemQualifies = true,
        };
        selector.Add(50, "A");
        selector.Add(50, "B");
        var a = 0;
        var b = 0;
        for (var i = 0; i < 1000; i++) {
            switch (selector.GetItem()) {
                case "A": a++; break;
                case "B": b++; break;
            }
        }
        Assert.AreEqual(1000, a + b);
        Assert.IsTrue(Math.Abs(a - b) < 10);
    }

    [TestMethod]
    public void WeightedSelection_NinetyTen() {
        var selector = new WeightedSelection<string>(42) {
            ReturnDisabledIfNoOtherItemQualifies = true,
        };
        selector.Add(9000, "A");
        selector.Add(1000, "B");
        var a = 0;
        var b = 0;
        for (var i = 0; i < 1000; i++) {
            switch (selector.GetItem()) {
                case "A": a++; break;
                case "B": b++; break;
            }
        }
        Assert.AreEqual(1000, a + b);
        Assert.IsTrue(a > b);
        Assert.IsTrue(a > b * 8);  // give a bit of wiggle room
    }

    [TestMethod]
    public void WeightedSelection_FiftyTenTenTenTen() {
        var selector = new WeightedSelection<string>(42) {
            ReturnDisabledIfNoOtherItemQualifies = true,
        };
        selector.Add(10000, "B");
        selector.Add(10000, "C");
        selector.Add(50000, "A");
        selector.Add(10000, "D");
        selector.Add(10000, "E");
        int a = 0, b = 0, c = 0, d = 0, e = 0;
        for (var i = 0; i < 1000; i++) {
            switch (selector.GetItem()) {
                case "A": a++; break;
                case "B": b++; break;
                case "C": c++; break;
                case "D": d++; break;
                case "E": e++; break;
            }
        }
        Assert.AreEqual(1000, a + b + c + d + e);
        Assert.IsTrue(a > b);
        Assert.IsTrue(a > c);
        Assert.IsTrue(a > d);
        Assert.IsTrue(a > e);
        Assert.IsTrue(a > b * 4);  // give a bit of wiggle room
        Assert.IsTrue(a > c * 4);  // give a bit of wiggle room
        Assert.IsTrue(a > d * 4);  // give a bit of wiggle room
        Assert.IsTrue(a > e * 4);  // give a bit of wiggle room
    }

}
