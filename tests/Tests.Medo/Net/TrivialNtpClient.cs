using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Medo.Net;
using System.Threading.Tasks;

namespace Tests;

[TestClass]
public class TrivialNtpClient_Tests {

    [TestMethod]
    public void TrivialNtpClient_Basic() {
        var time = TrivialNtpClient.RetrieveTime("0.medo64.pool.ntp.org");
        var diff = DateTime.UtcNow - time;
        Assert.IsTrue(Math.Abs(diff.TotalSeconds) < 2);
    }

    [TestMethod]
    public async Task Async() {
        var time = await TrivialNtpClient.RetrieveTimeAsync("0.medo64.pool.ntp.org");
        var diff = DateTime.UtcNow - time;
        Assert.IsTrue(Math.Abs(diff.TotalSeconds) < 2);
    }


    [TestMethod]
    public void TrivialNtpClient_Timeout() {
        using var client = new TrivialNtpClient("0.medo64.pool.ntp.org") { Timeout = 1 };
        Assert.ThrowsException<InvalidOperationException>(() => {
            var time = client.RetrieveTime();
        });
    }

    [TestMethod]
    public async Task TrivialNtpClient_TimeoutAsync() {
        using var client = new TrivialNtpClient("0.medo64.pool.ntp.org") { Timeout = 1 };
        await Assert.ThrowsExceptionAsync<InvalidOperationException>(async () => {
            var time = await client.RetrieveTimeAsync();
        });
    }


    [DataTestMethod]
    [DataRow("")]
    [DataRow("  ")]
    public void TrivialNtpClient_InvalidHostName(object data) {
        var hostName = data as string;
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => {
            var _ = new TrivialNtpClient(hostName);
        });
    }

    [DataTestMethod]
    [DataRow(0)]
    [DataRow(65536)]
    public void TrivialNtpClient_InvalidPort(object data) {
        var port = (int)data;
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => {
            var _ = new TrivialNtpClient("0.medo64.pool.ntp.org", port);
        });
    }

    [TestMethod]
    public void TrivialNtpClient_NonExistingHost() {
        using var client = new TrivialNtpClient("nonexisting.medo64.com");
        Assert.ThrowsException<InvalidOperationException>(() => {
            var time = client.RetrieveTime();
        });
    }

    [TestMethod]
    public async Task TrivialNtpClient_NonExistingHostAsync() {
        using var client = new TrivialNtpClient("nonexisting.medo64.com");
        await Assert.ThrowsExceptionAsync<InvalidOperationException>(async () => {
            var time = await client.RetrieveTimeAsync();
        });
    }

}
