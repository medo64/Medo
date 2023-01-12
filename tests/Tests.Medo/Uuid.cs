using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
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
        Assert.AreEqual("0000000000000000000000000", Uuid.Empty.ToId25String());
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

    [Ignore]
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
    public void Uuid_OperatorLessThan() {
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
    public void Uuid_OperatorMoreThan() {
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

    [TestMethod]
    public void Uuid_CompareTo() {
        var uuid1 = new Uuid(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, });
        var uuid2 = new Uuid(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, });
        var uuid3 = new Uuid(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 9, 0, 0, 0, 0, 0, 0, 0, });
        var uuid4 = new Uuid(new byte[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, });
        var uuid5 = new Uuid(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, });

        Assert.IsTrue(uuid1.CompareTo(uuid1) == 0);
        Assert.IsTrue(uuid1.CompareTo(uuid2) < 0);
        Assert.IsTrue(uuid1.CompareTo(uuid3) < 0);
        Assert.IsTrue(uuid1.CompareTo(uuid4) < 0);
        Assert.IsTrue(uuid1.CompareTo(uuid5) < 0);

        Assert.IsTrue(uuid2.CompareTo(uuid1) > 0);
        Assert.IsTrue(uuid2.CompareTo(uuid2) == 0);
        Assert.IsTrue(uuid2.CompareTo(uuid3) < 0);
        Assert.IsTrue(uuid2.CompareTo(uuid4) < 0);
        Assert.IsTrue(uuid2.CompareTo(uuid5) < 0);

        Assert.IsTrue(uuid3.CompareTo(uuid1) > 0);
        Assert.IsTrue(uuid3.CompareTo(uuid2) > 0);
        Assert.IsTrue(uuid3.CompareTo(uuid3) == 0);
        Assert.IsTrue(uuid3.CompareTo(uuid4) < 0);
        Assert.IsTrue(uuid3.CompareTo(uuid5) < 0);

        Assert.IsTrue(uuid4.CompareTo(uuid1) > 0);
        Assert.IsTrue(uuid4.CompareTo(uuid2) > 0);
        Assert.IsTrue(uuid4.CompareTo(uuid3) > 0);
        Assert.IsTrue(uuid4.CompareTo(uuid4) == 0);
        Assert.IsTrue(uuid4.CompareTo(uuid5) < 0);

        Assert.IsTrue(uuid5.CompareTo(uuid1) > 0);
        Assert.IsTrue(uuid5.CompareTo(uuid2) > 0);
        Assert.IsTrue(uuid5.CompareTo(uuid3) > 0);
        Assert.IsTrue(uuid5.CompareTo(uuid4) > 0);
        Assert.IsTrue(uuid5.CompareTo(uuid5) == 0);
    }

    [TestMethod]
    public void Uuid_CompareToGuid() {
        var uuid1 = new Uuid(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, });
        var uuid2 = new Uuid(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, });
        var uuid3 = new Uuid(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 9, 0, 0, 0, 0, 0, 0, 0, });
        var uuid4 = new Uuid(new byte[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, });
        var uuid5 = new Uuid(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, });

        Assert.IsTrue(uuid1.CompareTo(new Guid(uuid1.ToByteArray())) == 0);
        Assert.IsTrue(uuid1.CompareTo(new Guid(uuid2.ToByteArray())) < 0);
        Assert.IsTrue(uuid1.CompareTo(new Guid(uuid2.ToByteArray())) < 0);
        Assert.IsTrue(uuid1.CompareTo(new Guid(uuid2.ToByteArray())) < 0);
        Assert.IsTrue(uuid1.CompareTo(new Guid(uuid2.ToByteArray())) < 0);

        Assert.IsTrue(uuid2.CompareTo(new Guid(uuid1.ToByteArray())) > 0);
        Assert.IsTrue(uuid2.CompareTo(new Guid(uuid2.ToByteArray())) == 0);
        Assert.IsTrue(uuid2.CompareTo(new Guid(uuid3.ToByteArray())) < 0);
        Assert.IsTrue(uuid2.CompareTo(new Guid(uuid4.ToByteArray())) < 0);
        Assert.IsTrue(uuid2.CompareTo(new Guid(uuid5.ToByteArray())) < 0);

        Assert.IsTrue(uuid3.CompareTo(new Guid(uuid1.ToByteArray())) > 0);
        Assert.IsTrue(uuid3.CompareTo(new Guid(uuid2.ToByteArray())) > 0);
        Assert.IsTrue(uuid3.CompareTo(new Guid(uuid3.ToByteArray())) == 0);
        Assert.IsTrue(uuid3.CompareTo(new Guid(uuid4.ToByteArray())) < 0);
        Assert.IsTrue(uuid3.CompareTo(new Guid(uuid5.ToByteArray())) < 0);

        Assert.IsTrue(uuid4.CompareTo(new Guid(uuid1.ToByteArray())) > 0);
        Assert.IsTrue(uuid4.CompareTo(new Guid(uuid2.ToByteArray())) > 0);
        Assert.IsTrue(uuid4.CompareTo(new Guid(uuid3.ToByteArray())) > 0);
        Assert.IsTrue(uuid4.CompareTo(new Guid(uuid4.ToByteArray())) == 0);
        Assert.IsTrue(uuid4.CompareTo(new Guid(uuid5.ToByteArray())) < 0);

        Assert.IsTrue(uuid5.CompareTo(new Guid(uuid1.ToByteArray())) > 0);
        Assert.IsTrue(uuid5.CompareTo(new Guid(uuid2.ToByteArray())) > 0);
        Assert.IsTrue(uuid5.CompareTo(new Guid(uuid3.ToByteArray())) > 0);
        Assert.IsTrue(uuid5.CompareTo(new Guid(uuid4.ToByteArray())) > 0);
        Assert.IsTrue(uuid5.CompareTo(new Guid(uuid5.ToByteArray())) == 0);
    }

    [TestMethod]
    public void Uuid_Equals() {
        var uuid1 = new Uuid(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, });
        var uuid2 = new Uuid(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, });
        var uuid3 = new Uuid(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 9, 0, 0, 0, 0, 0, 0, 0, });
        var uuid4 = new Uuid(new byte[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, });
        var uuid5 = new Uuid(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, });

        Assert.IsTrue(uuid1.Equals(uuid1));
        Assert.IsFalse(uuid1.Equals(uuid2));
        Assert.IsFalse(uuid1.Equals(uuid3));
        Assert.IsFalse(uuid1.Equals(uuid4));
        Assert.IsFalse(uuid1.Equals(uuid5));

        Assert.IsFalse(uuid2.Equals(uuid1));
        Assert.IsTrue(uuid2.Equals(uuid2));
        Assert.IsFalse(uuid2.Equals(uuid3));
        Assert.IsFalse(uuid2.Equals(uuid4));
        Assert.IsFalse(uuid2.Equals(uuid5));

        Assert.IsFalse(uuid3.Equals(uuid1));
        Assert.IsFalse(uuid3.Equals(uuid2));
        Assert.IsTrue(uuid3.Equals(uuid3));
        Assert.IsFalse(uuid3.Equals(uuid4));
        Assert.IsFalse(uuid3.Equals(uuid5));

        Assert.IsFalse(uuid4.Equals(uuid1));
        Assert.IsFalse(uuid4.Equals(uuid2));
        Assert.IsFalse(uuid4.Equals(uuid3));
        Assert.IsTrue(uuid4.Equals(uuid4));
        Assert.IsFalse(uuid4.Equals(uuid5));

        Assert.IsFalse(uuid5.Equals(uuid1));
        Assert.IsFalse(uuid5.Equals(uuid2));
        Assert.IsFalse(uuid5.Equals(uuid3));
        Assert.IsFalse(uuid5.Equals(uuid4));
        Assert.IsTrue(uuid5.Equals(uuid5));
    }

    [TestMethod]
    public void Uuid_EqualsGuid() {
        var uuid1 = new Uuid(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, });
        var uuid2 = new Uuid(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, });
        var uuid3 = new Uuid(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 9, 0, 0, 0, 0, 0, 0, 0, });
        var uuid4 = new Uuid(new byte[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, });
        var uuid5 = new Uuid(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, });

        Assert.IsTrue(uuid1.Equals(new Guid(uuid1.ToByteArray())));
        Assert.IsFalse(uuid1.Equals(new Guid(uuid2.ToByteArray())));
        Assert.IsFalse(uuid1.Equals(new Guid(uuid3.ToByteArray())));
        Assert.IsFalse(uuid1.Equals(new Guid(uuid4.ToByteArray())));
        Assert.IsFalse(uuid1.Equals(new Guid(uuid5.ToByteArray())));

        Assert.IsFalse(uuid2.Equals(new Guid(uuid1.ToByteArray())));
        Assert.IsTrue(uuid2.Equals(new Guid(uuid2.ToByteArray())));
        Assert.IsFalse(uuid2.Equals(new Guid(uuid3.ToByteArray())));
        Assert.IsFalse(uuid2.Equals(new Guid(uuid4.ToByteArray())));
        Assert.IsFalse(uuid2.Equals(new Guid(uuid5.ToByteArray())));

        Assert.IsFalse(uuid3.Equals(new Guid(uuid1.ToByteArray())));
        Assert.IsFalse(uuid3.Equals(new Guid(uuid2.ToByteArray())));
        Assert.IsTrue(uuid3.Equals(new Guid(uuid3.ToByteArray())));
        Assert.IsFalse(uuid3.Equals(new Guid(uuid4.ToByteArray())));
        Assert.IsFalse(uuid3.Equals(new Guid(uuid5.ToByteArray())));

        Assert.IsFalse(uuid4.Equals(new Guid(uuid1.ToByteArray())));
        Assert.IsFalse(uuid4.Equals(new Guid(uuid2.ToByteArray())));
        Assert.IsFalse(uuid4.Equals(new Guid(uuid3.ToByteArray())));
        Assert.IsTrue(uuid4.Equals(new Guid(uuid4.ToByteArray())));
        Assert.IsFalse(uuid4.Equals(new Guid(uuid5.ToByteArray())));

        Assert.IsFalse(uuid5.Equals(new Guid(uuid1.ToByteArray())));
        Assert.IsFalse(uuid5.Equals(new Guid(uuid2.ToByteArray())));
        Assert.IsFalse(uuid5.Equals(new Guid(uuid3.ToByteArray())));
        Assert.IsFalse(uuid5.Equals(new Guid(uuid4.ToByteArray())));
        Assert.IsTrue(uuid5.Equals(new Guid(uuid5.ToByteArray())));
    }

    [DataTestMethod]
    [DataRow(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, "00000000-0000-0000-0000-000000000000")]
    [DataRow(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 }, "00000000-0000-0000-0000-000000000001")]
    [DataRow(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2 }, "00000000-0000-0000-0000-000000000002")]
    [DataRow(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 9, 0, 0, 0, 0, 0, 0, 0 }, "00000000-0000-0000-0900-000000000000")]
    [DataRow(new byte[] { 0, 1, 2, 3, 52, 53, 118, 119, 184, 185, 250, 251, 252, 253, 254, 255 }, "00010203-3435-7677-b8b9-fafbfcfdfeff")]
    [DataRow(new byte[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, "01000000-0000-0000-0000-000000000000")]
    [DataRow(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, "02000000-0000-0000-0000-000000000000")]
    [DataRow(new byte[] { 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255 }, "ffffffff-ffff-ffff-ffff-ffffffffffff")]
    public void Uuid_String(byte[] bytes, string text) {
        var uuid = new Uuid(bytes);
        Assert.AreEqual(text, uuid.ToString());
        Assert.AreEqual(uuid, Uuid.FromString(text));
    }

    [DataTestMethod]
    [DataRow(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, "0000000000000000000000000")]
    [DataRow(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 }, "0000000000000000000000001")]
    [DataRow(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2 }, "0000000000000000000000002")]
    [DataRow(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 9, 0, 0, 0, 0, 0, 0, 0 }, "00000000000006q3abpe675nu")]
    [DataRow(new byte[] { 0, 1, 2, 3, 52, 53, 118, 119, 184, 185, 250, 251, 252, 253, 254, 255 }, "000jnpiacvek52kvka6to5ogn")]
    [DataRow(new byte[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, "042kt5d5ybb4r50zg5p72g3f1")]
    [DataRow(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, "0856marbwnn9ha1yxbde4x6v2")]
    [DataRow(new byte[] { 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255 }, "usz5xbbiqsfq7s727n0pzr2xa")]
    public void Uuid_Id25(byte[] bytes, string id25Text) {
        var uuid = new Uuid(bytes);
        Assert.AreEqual(id25Text, uuid.ToId25String());
        Assert.AreEqual(uuid, Uuid.FromId25String(id25Text));
    }

    [DataTestMethod]
    [DataRow("0001/0203/3435/7677/b8b9/fafb/fcfd/feff", "00/0jn/pi/acv/ek/52k/vk/a6t/o5/ogn")]
    [DataRow("ffffffff-ffff-ffff-ffff-ffffffffffff", "usz5x bbiqs fq7s7 27n0p zr2xa")]
    public void Uuid_Id25Dirty(string uuidText, string id25Text) {
        var expectedUuid = Uuid.FromString(uuidText);
        var id25Uuid = Uuid.FromId25String(id25Text);
        Assert.AreEqual(expectedUuid, id25Uuid);
        Assert.AreNotEqual(Uuid.Empty, expectedUuid);
        Assert.AreNotEqual(Uuid.Empty, id25Uuid);
    }


    [TestMethod]
    public void Uuid_MarshalBytes() {
        var uuid = Uuid.NewUuid7();

        int size = Marshal.SizeOf(uuid);
        Assert.AreEqual(16, size);

        var bytes = new byte[size];
        var ptr = Marshal.AllocHGlobal(size);
        Marshal.StructureToPtr(uuid, ptr, true);
        Marshal.Copy(ptr, bytes, 0, size);
        Marshal.FreeHGlobal(ptr);  // it's a test, no need for try/finally block

        Assert.IsTrue(CompareArrays(uuid.ToByteArray(), bytes));
    }

    [TestMethod]
    public void Uuid_UnmarshalBytes() {
        var bytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };

        var ptr = Marshal.AllocHGlobal(bytes.Length);
        Marshal.Copy(bytes, 0, ptr, bytes.Length);
        var uuid = (Uuid)Marshal.PtrToStructure(ptr, typeof(Uuid));
        Marshal.FreeHGlobal(ptr);

        Assert.IsTrue(CompareArrays(bytes, uuid.ToByteArray()));
    }


    private UInt64 UuidToNumber(Uuid uuid) {  // first 64 bits will do
        var bytes = uuid.ToByteArray();
        if (BitConverter.IsLittleEndian) {
            Array.Reverse(bytes, 0, 8);
        }
        return BitConverter.ToUInt64(bytes);
    }

    private static bool CompareArrays(byte[] buffer1, byte[] buffer2) {
        var comparer = EqualityComparer<byte>.Default;
        if (buffer1.Length != buffer2.Length) { return false; }  // should not really happen
        for (int i = 0; i < buffer1.Length; i++) {
            if (!comparer.Equals(buffer1[i], buffer2[i])) {
                return false;
            }
        }
        return true;
    }

}
