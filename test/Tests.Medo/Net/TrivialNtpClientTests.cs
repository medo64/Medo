using System;
using Xunit;

namespace Medo.Tests.Net.TrivialNtpClient {
    using Medo.Net;

    public class Tests {

        [Fact(DisplayName = "TrivialNtpClient: Basic")]
        public void Basic() {
            var time = TrivialNtpClient.RetrieveTime("0.medo64.pool.ntp.org");
            var diff = DateTime.UtcNow - time;
            Assert.InRange<double>(diff.TotalSeconds, 0, 5);
        }

        [Fact(DisplayName = "TrivialNtpClient: Async")]
        async public void Async() {
            var time = await TrivialNtpClient.RetrieveTimeAsync("0.medo64.pool.ntp.org");
            var diff = DateTime.UtcNow - time;
            Assert.InRange<double>(diff.TotalSeconds, 0, 5);
        }


        [Fact(DisplayName = "TrivialNtpClient: Timeout")]
        public void Timeout() {
            using var client = new TrivialNtpClient("0.medo64.pool.ntp.org") { Timeout = 1 };
            Assert.Throws<InvalidOperationException>(() => {
                var time = client.RetrieveTime();
            });
        }

        [Fact(DisplayName = "TrivialNtpClient: Timeout (async)")]
        async public void TimeoutAsync() {
            using var client = new TrivialNtpClient("0.medo64.pool.ntp.org") { Timeout = 1 };
            await Assert.ThrowsAsync<InvalidOperationException>(async () => {
                var time = await client.RetrieveTimeAsync();
            });
        }


        [Theory(DisplayName = "TrivialNtpClient: Invalid host")]
        [InlineData("")]
        [InlineData("  ")]
        public void InvalidHostName(object data) {
            var hostName = data as string;
            Assert.Throws<ArgumentOutOfRangeException>(() => {
                var _ = new TrivialNtpClient(hostName);
            });
        }

        [Theory(DisplayName = "TrivialNtpClient: Invalid port")]
        [InlineData(0)]
        [InlineData(65536)]
        public void InvalidPort(object data) {
            var port = (int)data;
            Assert.Throws<ArgumentOutOfRangeException>(() => {
                var _ = new TrivialNtpClient("0.medo64.pool.ntp.org", port);
            });
        }

        [Fact(DisplayName = "TrivialNtpClient: Non-existing host")]
        public void NonExistingHost() {
            using var client = new TrivialNtpClient("nonexisting.medo64.com");
            Assert.Throws<InvalidOperationException>(() => {
                var time = client.RetrieveTime();
            });
        }

        [Fact(DisplayName = "TrivialNtpClient: Non-existing host (async)")]
        async public void NonExistingHostAsync() {
            using var client = new TrivialNtpClient("nonexisting.medo64.com");
            await Assert.ThrowsAsync<InvalidOperationException>(async () => {
                var time = await client.RetrieveTimeAsync();
            });
        }

    }
}
