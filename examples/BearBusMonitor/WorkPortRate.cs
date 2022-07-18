using Medo.Device;
using System.IO.Ports;

namespace BearBusMonitor;

internal static class WorkPortRate {

    public static void Run(string portName) {
        var rates = new int[] { 115200, 57600, 38400, 19200, 9600, };

        var keySelection = new KeySelection();
        foreach (var rate in rates) {
            keySelection.Add(rate.ToString("#,##0"), () => {
                var port = new SerialPort(portName, rate, Parity.None, 8, StopBits.One);
                port.Open();
                WorkBus.Run(BearBus.CreateMonitor(port.BaseStream));
            });
        }
        Output.Select("Select baud rate (" + portName + ")", keySelection);
    }

}
