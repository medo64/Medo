using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Medo.Application;
using System.IO;

namespace Tests;

[TestClass]
public class Upgrade_Tests {

    [TestMethod]
    public void Upgrade_Basic() {
        var file = Upgrade.GetUpgradeFile(new Uri("https://medo64.com/upgrade"), "test", new Version(1, 0, 0, 0));
        Assert.IsNotNull(file);

        var stream = new MemoryStream();
        file.GetStream().CopyTo(stream);
        Assert.AreNotEqual(0, stream.Length);

        var counter = 0;
        file.ProgressChanged += (sender, e) => counter++;

        var array = file.DownloadData();
        Assert.AreEqual(BitConverter.ToString(array), BitConverter.ToString(stream.ToArray()));
        Assert.AreNotEqual(0, counter);
    }

    [TestMethod]
    public async Task Upgrade_BasicAsync() {
        var file = await Upgrade.GetUpgradeFileAsync(new Uri("https://medo64.com/upgrade"), "test", new Version(1, 0, 0, 0));
        Assert.IsNotNull(file);

        var stream = new MemoryStream();
        await (await file.GetStreamAsync()).CopyToAsync(stream);
        Assert.AreNotEqual(0, stream.Length);

        var counter = 0;
        file.ProgressChanged += (sender, e) => counter++;

        var array = await file.DownloadDataAsync();
        Assert.AreEqual(BitConverter.ToString(array), BitConverter.ToString(stream.ToArray()));
        Assert.AreNotEqual(0, counter);
    }

    [TestMethod]
    public void Upgrade_NoUpgrade() {
        var file = Upgrade.GetUpgradeFile(new Uri("https://medo64.com/upgrade"), "test", new Version(1, 2, 3, 4));
        Assert.IsNull(file);
    }

    [TestMethod]
    public async Task Upgrade_NoUpgradeAsync() {
        var file = await Upgrade.GetUpgradeFileAsync(new Uri("https://medo64.com/upgrade"), "test", new Version(1, 2, 3, 4));
        Assert.IsNull(file);
    }

}
