using System.IO.Ports;

namespace BearBusMonitor;

internal static class WorkPort {

    public static void Run() {
        var portNames = SerialPort.GetPortNames();
        if (portNames.Length == 0) {
            Output.Error("No serial ports.", waitForEnter: true);
        }

        var keySelection = new KeySelection();
        foreach (var portName in portNames) {
            keySelection.Add(portName, () => {
                WorkPortRate.Run(portName);
            });
        }
        Output.Select("Select port", keySelection);
    }

}
