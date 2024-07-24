using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Medo.Configuration;
using System.IO;

namespace Tests;

[TestClass]
public class RecentFiles_Tests {

    [TestMethod]
    public void Config_MaximumCount() {
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => {
            RecentFiles.MaximumCount = 0;
        });
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => {
            RecentFiles.MaximumCount = 101;
        });
    }

    [TestMethod]
    public void Config_AddNull() {
        Assert.ThrowsException<ArgumentNullException>(() => {
            RecentFiles.Add(null);
        });
    }

    [TestMethod]
    public void Config_RemoveNull() {
        Assert.ThrowsException<ArgumentNullException>(() => {
            RecentFiles.Remove(null);
        });
    }


    [TestMethod]
    public void Config_Limit() {
        var file1 = new FileInfo("test1.txt");
        var file2 = new FileInfo("test2.txt");
        var file3 = new FileInfo("test3.txt");
        var file4 = new FileInfo("test4.txt");
        var file5 = new FileInfo("test5.txt");

        RecentFiles.MaximumCount = 3;
        RecentFiles.Add(file1);
        RecentFiles.Add(file2);
        RecentFiles.Add(file3);
        RecentFiles.Add(file4);
        RecentFiles.Add(file5);

        var files = RecentFiles.GetFiles();
        Assert.AreEqual(3, files.Count);
        Assert.AreEqual(file5.FullName, files[0].FullName);
        Assert.AreEqual(file4.FullName, files[1].FullName);
        Assert.AreEqual(file3.FullName, files[2].FullName);
    }

    [TestMethod]
    public void Config_IgnoreSame() {
        var file1 = new FileInfo("test1.txt");
        var file2 = new FileInfo("test2.txt");
        var file3 = new FileInfo("test3.txt");

        RecentFiles.MaximumCount = 3;
        RecentFiles.Add(file1);
        RecentFiles.Add(file2);
        RecentFiles.Add(file3);
        RecentFiles.Add(file2);
        RecentFiles.Add(file2);

        var files = RecentFiles.GetFiles();
        Assert.AreEqual(3, files.Count);
        Assert.AreEqual(file2.FullName, files[0].FullName);
        Assert.AreEqual(file3.FullName, files[1].FullName);
        Assert.AreEqual(file1.FullName, files[2].FullName);
    }

    [TestMethod]
    public void Config_Clear() {
        var file1 = new FileInfo("test1.txt");
        var file2 = new FileInfo("test2.txt");
        var file3 = new FileInfo("test3.txt");

        RecentFiles.MaximumCount = 3;
        RecentFiles.Add(file1);
        RecentFiles.Add(file2);
        RecentFiles.Add(file3);
        RecentFiles.Clear();

        var files = RecentFiles.GetFiles();
        Assert.AreEqual(0, files.Count);
    }

    [TestMethod]
    public void Config_Remove() {
        var file1 = new FileInfo("test1.txt");
        var file2 = new FileInfo("test2.txt");
        var file3 = new FileInfo("test3.txt");

        RecentFiles.MaximumCount = 3;
        RecentFiles.Add(file1);
        RecentFiles.Add(file2);
        RecentFiles.Add(file3);
        RecentFiles.Remove(file2);

        var files = RecentFiles.GetFiles();
        Assert.AreEqual(2, files.Count);
        Assert.AreEqual(file3.FullName, files[0].FullName);
        Assert.AreEqual(file1.FullName, files[1].FullName);
    }

    [TestMethod]
    public void Config_EscapeControl1() {
        var file1 = new FileInfo("test1\b.txt");
        var file2 = new FileInfo("test2\t.txt");
        var file3 = new FileInfo("test3\r.txt");
        var file4 = new FileInfo("test4\n.txt");

        RecentFiles.Clear();
        RecentFiles.MaximumCount = 4;
        RecentFiles.Add(file4);
        RecentFiles.Add(file3);
        RecentFiles.Add(file2);
        RecentFiles.Add(file1);

        var files = RecentFiles.GetFiles();
        Assert.AreEqual(4, files.Count);
        Assert.AreEqual(file1.FullName, files[0].FullName);
        Assert.AreEqual(file2.FullName, files[1].FullName);
        Assert.AreEqual(file3.FullName, files[2].FullName);
        Assert.AreEqual(file4.FullName, files[3].FullName);
    }

    [TestMethod]
    public void Config_EscapeControl2() {
        var file1 = new FileInfo("test1\u0005.txt");

        RecentFiles.Clear();
        RecentFiles.Add(file1);

        var files = RecentFiles.GetFiles();
        Assert.AreEqual(1, files.Count);
        Assert.AreEqual(file1.FullName, files[0].FullName);
    }

}
