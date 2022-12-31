using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Medo;

namespace Tests;

[TestClass]
public class Uuid_Tests {

    [TestMethod]
    public void Uuid_Empty() {
        Assert.AreEqual(Guid.Empty, Uuid.Empty.ToGuid());
        Assert.IsTrue(Uuid.Empty.Equals(Guid.Empty));
        Assert.IsTrue(Uuid.Empty.Equals(Guid.Empty));
        Assert.AreEqual("00000000-0000-0000-0000-000000000000", Uuid.Empty.ToString());
    }

    [TestMethod]
    public void Uuid_New() {
        var uuid1 = Uuid.NewUuid7();
        var uuid2 = Uuid.NewUuid7();
        Assert.AreNotEqual(uuid1, uuid2);
    }

    [TestMethod]
    public void Uuid_GuidAndBack() {
        var uuid1 = Uuid.NewUuid7();
        var guid = uuid1.ToGuid();
        var uuid2 = new Uuid(guid);
        Assert.AreEqual(uuid1, uuid2);
    }

    [TestMethod]
    public void Uuid_TestAlwaysIncreasing() {
        var oldUuid = Uuid.Empty;
        for (var i = 0; i < 1000000; i++) {  // assuming we're not generating more than 3072 each millisecond, they should be monotonically increasing
            var newUuid = Uuid.NewUuid7();
            Assert.IsTrue(UuidToNumber(newUuid) > UuidToNumber(oldUuid), $"Failed at iteration {i}\n{oldUuid}\n{newUuid}");  // using UuidToNumber intentionaly as to avoid trusting operator overloads
            oldUuid = newUuid;
        }
    }

    [TestMethod]
    public void Uuid_TestMany() {
        var sw = Stopwatch.StartNew();
        var i = 0;
        while (sw.ElapsedMilliseconds < 1000) {  // assuming we're not generating more than 3072 each millisecond, they should be monotonically increasing
            _ = Uuid.NewUuid7();
            i++;
        }
        //Console.WriteLine($"Generated {i:#,##0} UUIDs in 1 second");
    }

    [TestMethod]
    public void Uuid_LessThan() {
        var uuid1 = new Uuid(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, });
        var uuid2 = new Uuid(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, });
        var uuid3 = new Uuid(new byte[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, });
        var uuid4 = new Uuid(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, });
        Assert.IsTrue(uuid1 < uuid2);
        Assert.IsTrue(uuid1 < uuid3);
        Assert.IsTrue(uuid1 < uuid4);
        Assert.IsTrue(uuid2 < uuid3);
        Assert.IsTrue(uuid3 < uuid4);
        Assert.IsTrue(uuid3 < uuid4);
        Assert.IsFalse(uuid1 > uuid2);
        Assert.IsFalse(uuid1 > uuid3);
        Assert.IsFalse(uuid1 > uuid4);
        Assert.IsFalse(uuid2 > uuid3);
        Assert.IsFalse(uuid3 > uuid4);
        Assert.IsFalse(uuid3 > uuid4);
        Assert.IsFalse(uuid1 == uuid2);
        Assert.IsFalse(uuid1 == uuid3);
        Assert.IsFalse(uuid1 == uuid4);
        Assert.IsFalse(uuid2 == uuid3);
        Assert.IsFalse(uuid3 == uuid4);
        Assert.IsFalse(uuid3 == uuid4);
    }

    [TestMethod]
    public void Uuid_MoreThan() {
        var uuid1 = new Uuid(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, });
        var uuid2 = new Uuid(new byte[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, });
        var uuid3 = new Uuid(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, });
        var uuid4 = new Uuid(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, });
        Assert.IsTrue(uuid1 > uuid2);
        Assert.IsTrue(uuid1 > uuid3);
        Assert.IsTrue(uuid1 > uuid4);
        Assert.IsTrue(uuid2 > uuid3);
        Assert.IsTrue(uuid3 > uuid4);
        Assert.IsTrue(uuid3 > uuid4);
        Assert.IsFalse(uuid1 < uuid2);
        Assert.IsFalse(uuid1 < uuid3);
        Assert.IsFalse(uuid1 < uuid4);
        Assert.IsFalse(uuid2 < uuid3);
        Assert.IsFalse(uuid3 < uuid4);
        Assert.IsFalse(uuid3 < uuid4);
        Assert.IsFalse(uuid1 == uuid2);
        Assert.IsFalse(uuid1 == uuid3);
        Assert.IsFalse(uuid1 == uuid4);
        Assert.IsFalse(uuid2 == uuid3);
        Assert.IsFalse(uuid3 == uuid4);
        Assert.IsFalse(uuid3 == uuid4);
    }


    private UInt64 UuidToNumber(Uuid uuid) {  // first 64 bits will do
        var bytes = uuid.ToByteArray();
        if (BitConverter.IsLittleEndian) {
            Array.Reverse(bytes, 0, 8);
        }
        return BitConverter.ToUInt64(bytes);
    }

}
