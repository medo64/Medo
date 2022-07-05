using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

/// <summary>
/// Implements TCP bus where all connections mix bytes together (like they would on RS-485).
/// Not to be used for anything but testing.
/// </summary>
internal static class TcpBus {

    private static readonly TcpBusStream BusStream = new();

    public static void CreateServer() {
        ThreadPool.QueueUserWorkItem(delegate {
            var server = new TcpListener(IPAddress.Loopback, 28433);
            server.Start();
            while (true) {
                var client = server.AcceptTcpClient();
                client.NoDelay = true;
                client.ReceiveBufferSize = 1;
                client.SendBufferSize = 1;
                client.ReceiveTimeout = 50;
                client.SendTimeout = 50;
                BusStream.AddStream(client.GetStream());
            }
        });
    }

    public static Stream ConnectClient() {
        var client = new TcpClient("localhost", 28433) {
            NoDelay = true,
            ReceiveBufferSize = 1,
            SendBufferSize = 1,
            ReceiveTimeout = 50,
            SendTimeout = 50,
        };
        return client.GetStream();
    }

}
