/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

//2022-01-30: Initial release

namespace Medo.Device;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Text;

public sealed class CyberCard : IDisposable {

    /// <summary>
    /// Initializes a new instance of the class using the specified port name.
    /// </summary>
    /// <param name="portName">The port to use.</param>
    /// <exception cref="ArgumentNullException">Port name cannot be null.</exception>
    public CyberCard(string portName) {
        if (portName == null) { throw new ArgumentNullException(nameof(portName), "Port name cannot be null."); }
        portName = portName.Trim().TrimEnd(':');

        var uart = new SerialPort(portName, 2400, Parity.None, 8, StopBits.One) {
            ReadTimeout = 100,
            WriteTimeout = 100,
            NewLine = "\0"
        };
        uart.Open();
        Uart = uart;
        UartStream = uart.BaseStream;
    }

    /// <summary>
    /// Initializes a new instance of the class using the specified stream.
    /// </summary>
    /// <param name="stream">The stream to use.</param>
    /// <exception cref="ArgumentNullException">Stream cannot be null.</exception>
    public CyberCard(Stream stream) {
        if (stream == null) { throw new ArgumentNullException(nameof(stream), "Stream cannot be null."); }
        UartStream = stream;
    }


    private readonly SerialPort? Uart;
    private readonly Stream UartStream;
    private readonly object UartLock = new();


    #region IDispose

    /// <inheritdoc />
    public void Dispose() {
        lock (UartLock) {
            if (Uart != null) {
                UartStream.Dispose();  // dispose only if opened it in the first place
                if (Uart.IsOpen) {
                    Uart.Close();
                    Uart.Dispose();
                }
            }
        }
    }

    #endregion IDispose

    #region SerialPort

    /// <summary>
    /// Gets an array of serial port names.
    /// This returns all serial port names without checking they're for the specific device.
    /// </summary>
    public static string[] GetSerialPortNames() {
        return SerialPort.GetPortNames();
    }

    #endregion SerialPort


    /// <summary>
    /// Returns model name.
    /// Value will be null if information cannot be retrieved.
    /// </summary>
    public string? GetDeviceModel() {
        return RetrieveModelInformation()?.Model;
    }

    /// <summary>
    /// Returns firmware version.
    /// Value will be null if information cannot be retrieved.
    /// </summary>
    public string? GetDeviceFirmware() {
        return RetrieveModelInformation()?.Firmware;
    }

    /// <summary>
    /// Returns serial number or an empty string is serial number is not available.
    /// Value will be null if information cannot be retrieved.
    /// </summary>
    public string? GetDeviceSerial() {
        var info = RetrieveModelInformation();
        if (info != null) {
            var serial = info.Value.Serial.Trim();
            if (serial.Replace("0", "").Length == 0) { return ""; }  // all 0's
            return serial;
        }
        return null;
    }

    /// <summary>
    /// Returns manufacturer name.
    /// Value will be null if information cannot be retrieved.
    /// </summary>
    public string? GetDeviceManufacturer() {
        return RetrieveModelInformation()?.Manufacturer;
    }


    /// <summary>
    /// Returns capacity in Watts.
    /// Value will be null if information cannot be retrieved.
    /// </summary>
    public int? GetDeviceCapacity() {
        return RetrieveCapacityInformation()?.CapacityInW;
    }

    /// <summary>
    /// Returns capacity in VA.
    /// Value will be null if information cannot be retrieved.
    /// </summary>
    public int? GetDeviceCapacityVA() {
        return RetrieveCapacityInformation()?.CapacityInVA;
    }

    /// <summary>
    /// Returns design voltage.
    /// Value will be null if information cannot be retrieved.
    /// </summary>
    public int? GetDeviceVoltage() {
        return RetrieveCapacityInformation()?.Voltage;
    }


    /// <summary>
    /// Returns input voltage.
    /// Value will be null if information cannot be retrieved.
    /// </summary>
    public double? GetInputVoltage() {
        return RetrieveCurrentInformation()?.InputVoltage;
    }

    /// <summary>
    /// Returns output voltage.
    /// Value will be null if information cannot be retrieved.
    /// </summary>
    public double? GetOutputVoltage() {
        return RetrieveCurrentInformation()?.OutputVoltage;
    }

    /// <summary>
    /// Returns AC frequency.
    /// Value will be null if information cannot be retrieved.
    /// </summary>
    public double? GetFrequency() {
        return RetrieveCurrentInformation()?.Frequency;
    }

    /// <summary>
    /// Returns load in percents.
    /// Value will be null if information cannot be retrieved.
    /// </summary>
    public double? GetLoadPercentage() {
        return RetrieveCurrentInformation()?.LoadPercentage;
    }

    /// <summary>
    /// Returns remaining battery in percents.
    /// Value will be null if information cannot be retrieved.
    /// </summary>
    public double? GetBatteryPercentage() {
        return RetrieveCurrentInformation()?.BatteryPercentage;
    }

    /// <summary>
    /// Returns remaining battery in minutes.
    /// Value will be null if information cannot be retrieved.
    /// </summary>
    public int? GetBatteryRuntime() {
        return RetrieveCurrentInformation()?.BatteryRuntime;
    }


    /// <summary>
    /// Returns true if immediate power off was successful.
    /// </summary>
    public bool PowerOff() {
        return Execute("S")?.Equals("0") ?? false;
    }

    /// <summary>
    /// Returns true if power off operation has been queued.
    /// </summary>
    /// <param name="waitPowerOffInMinutes">How much time to wait before doing power off.</param>
    /// <exception cref="ArgumentOutOfRangeException">Power off wait period must be between 0 and 99 minutes.</exception>
    public bool PowerOff(int waitPowerOffInMinutes) {
        if (waitPowerOffInMinutes is < 0 or > 99) { throw new ArgumentOutOfRangeException(nameof(waitPowerOffInMinutes), "Power off wait period must be between 0 and 99 minutes."); }
        return Execute("S" + waitPowerOffInMinutes.ToString("00", CultureInfo.InvariantCulture))?.Equals("0") ?? false;
    }

    /// <summary>
    /// Returns true if power off operation has been queued.
    /// Wait period will be rounded down to the minute resolution.
    /// </summary>
    /// <param name="waitPowerOff">How much time to wait before doing power off.</param>
    /// <exception cref="ArgumentOutOfRangeException">Power off wait period must be between 0 and 99 minutes.</exception>
    public bool PowerOff(TimeSpan waitPowerOff) {
        if (waitPowerOff.TotalMinutes is < 0 or > 99) { throw new ArgumentOutOfRangeException(nameof(waitPowerOff), "Power off wait period must be between 0 and 99 minutes."); }
        var minutes = (int)waitPowerOff.TotalMinutes;
        return PowerOff(minutes);
    }

    /// <summary>
    /// Returns true if power off/on operation has been queued.
    /// </summary>
    public bool PowerReset() {
        return PowerReset(0, 0);
    }

    /// <summary>
    /// Returns true if power off/on operation has been queued.
    /// Wait period will be rounded down to the minute resolution.
    /// </summary>
    /// <param name="waitPowerOffInMinutes">How much time to wait before doing power off.</param>
    /// <param name="waitPowerOnInMinutes">How much time to wait before doing power off.</param>
    /// <exception cref="ArgumentOutOfRangeException">Power off wait period must be between 0 and 99 minutes. -or- Power on wait period must be between 0 and 9999 minutes.</exception>
    public bool PowerReset(int waitPowerOffInMinutes, int waitPowerOnInMinutes) {
        if (waitPowerOffInMinutes is < 0 or > 99) { throw new ArgumentOutOfRangeException(nameof(waitPowerOffInMinutes), "Power off wait period must be between 0 and 99 minutes."); }
        if (waitPowerOnInMinutes is < 0 or > 9999) { throw new ArgumentOutOfRangeException(nameof(waitPowerOnInMinutes), "Power on wait period must be between 0 and 9999 minutes."); }
        var commandText = string.Format(CultureInfo.InvariantCulture, "S{0:00}R{1:0000}", waitPowerOffInMinutes, waitPowerOnInMinutes);
        return Execute(commandText)?.Equals("0") ?? false;
    }

    /// <summary>
    /// Returns true if power off/on operation has been queued.
    /// Wait period will be rounded down to the minute resolution.
    /// </summary>
    /// <param name="waitPowerOff">How much time to wait before doing power off.</param>
    /// <param name="waitPowerOn">How much time to wait before doing power off.</param>
    /// <exception cref="ArgumentOutOfRangeException">Power off wait period must be between 0 and 99 minutes. -or- Power on wait period must be between 0 and 9999 minutes.</exception>
    public bool PowerReset(TimeSpan waitPowerOff, TimeSpan waitPowerOn) {
        if (waitPowerOff.TotalMinutes is < 0 or > 99) { throw new ArgumentOutOfRangeException(nameof(waitPowerOff), "Power off wait period must be between 0 and 99 minutes."); }
        if (waitPowerOn.TotalMinutes is < 0 or > 9999) { throw new ArgumentOutOfRangeException(nameof(waitPowerOn), "Power on wait period must be between 0 and 9999 minutes."); }
        var minutesPowerOff = (int)waitPowerOff.TotalMinutes;
        var minutesPowerOn = (int)waitPowerOn.TotalMinutes;
        return PowerReset(minutesPowerOff, minutesPowerOn);
    }

    /// <summary>
    /// Returns true if power off cancellation was successful.
    /// </summary>
    public bool CancelPowerOff() {
        return Execute("C")?.Equals("0") ?? false;
    }

    /// <summary>
    /// Returns true if immediate power on was successful.
    /// </summary>
    public bool PowerOn() {
        return Execute("W")?.Equals("0") ?? false;
    }


    /// <summary>
    /// Returns true if beeper was enabled.
    /// </summary>
    public bool AlarmEnable() {
        return Execute("C7:1")?.Equals("0") ?? false;
    }

    /// <summary>
    /// Returns true if beeper was disabled.
    /// </summary>
    public bool AlarmDisable() {
        return Execute("C7:0")?.Equals("0") ?? false;
    }


    /// <summary>
    /// Returns true if power on is pending.
    /// </summary>
    public bool? IsPendingPowerOn() {
        return RetrieveCurrentInformation()?.IsPendingPowerOn;
    }

    /// <summary>
    /// Returns true if power off is pending.
    /// </summary>
    public bool? IsPendingPowerOff() {
        return RetrieveCurrentInformation()?.IsPendingPowerOff;
    }

    /// <summary>
    /// Returns true if test is currently running.
    /// </summary>
    public bool? IsTestInProgress() {
        return RetrieveCurrentInformation()?.IsTesting;
    }

    /// <summary>
    /// Returns true if device alarm is active.
    /// </summary>
    public bool? IsAlarmActive() {
        return RetrieveCurrentInformation()?.IsBeeping;
    }

    /// <summary>
    /// Returns true if battery is currently used.
    /// </summary>
    public bool? IsUsingBattery() {
        return RetrieveCurrentInformation()?.IsUsingBattery;
    }

    /// <summary>
    /// Returns true if battery is low.
    /// </summary>
    public bool? IsBatteryLow() {
        return RetrieveCurrentInformation()?.IsBatteryLow;
    }

    /// <summary>
    /// Returns true if battery is charging.
    /// </summary>
    public bool? IsBatteryCharging() {
        return RetrieveCurrentInformation()?.IsBatteryCharging;
    }

    /// <summary>
    /// Returns true if battery is full.
    /// </summary>
    public bool? IsBatteryFull() {
        return RetrieveCurrentInformation()?.IsBatteryFull;
    }

    /// <summary>
    /// Returns true if UPS is powered off.
    /// </summary>
    public bool? IsPoweredOff() {
        return RetrieveCurrentInformation()?.IsPoweredOff;
    }

    /// <summary>
    /// Returns true if UPS is powered on.
    /// </summary>
    public bool? IsPoweredOn() {
        var status = RetrieveCurrentInformation()?.IsPoweredOff;
        if (status == null) { return null; }
        return !status.Value;
    }


    #region Execute

    private string? Execute(string command) {
        lock (UartLock) {
            var outBytes = Encoding.Latin1.GetBytes(command + "\r");
            UartStream.Write(outBytes);

            var inBytes = new Queue<byte>();
            var buffer = new byte[256];
            var time = Stopwatch.StartNew();
            while (true) {
                int count;
                try {
                    count = UartStream.Read(buffer, 0, buffer.Length);
                } catch (TimeoutException) {
                    return null;
                }

                if (count > 0) {  // we have some new data
                    for (var i = 0; i < count; i++) {  // add to queue
                        inBytes.Enqueue(buffer[i]);
                    }
                    if (buffer[count - 1] == 0x0D) { break; }  // assume we're done since last character is CR
                } else {  // nothing more to read
                    break;
                }

                if (time.ElapsedMilliseconds > 500) { return null; }  // too much time has passed
            }

            var text = Encoding.Latin1.GetString(inBytes.ToArray());
            var lines = text.Split('\r', StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length > 0) {
                var lastText = lines[^1];  // there might be multiple lines if something was wrong
                if (lastText.StartsWith('#')) {
                    return lastText[1..];
                }
            }
        }
        return null;
    }

    #endregion Execute

    #region Cache: ModelInformation

    private struct ModelInformation {
        public string Model;
        public string Firmware;
        public string Serial;
        public string Manufacturer;
    }

    private ModelInformation? CachedModelInformation;  // permanently cached

    private ModelInformation? RetrieveModelInformation() {
        if (CachedModelInformation == null) {
            var data = Execute("P4");
            if (data != null) {
                var parts = data.Split(',');
                if (parts.Length >= 4) {
                    var info = new ModelInformation() {
                        Model = parts[0],
                        Firmware = parts[1],
                        Serial = parts[2],
                        Manufacturer = parts[3],
                    };
                    CachedModelInformation = info;
                }
            }
        }
        return CachedModelInformation;
    }

    #endregion Cache: ModelInformation

    #region Cache: CapacityInformation

    private struct CapacityInformation {
        public int? CapacityInVA;
        public int? CapacityInW;
        public int? Voltage;
    }

    private CapacityInformation? CachedCapacityInformation;  // permanently cached

    private CapacityInformation? RetrieveCapacityInformation() {
        if (CachedCapacityInformation == null) {
            var data = Execute("P2");
            if (data != null) {
                var parts = data.Split(',');
                if (parts.Length >= 2) {
                    var info = new CapacityInformation() {
                        CapacityInVA = double.TryParse(parts[0], NumberStyles.Number, CultureInfo.InvariantCulture, out double capacityVA) ? (int)capacityVA : null,
                        CapacityInW = double.TryParse(parts[1], NumberStyles.Number, CultureInfo.InvariantCulture, out double capacityW) ? (int)capacityW : null,
                        Voltage = double.TryParse(parts[2], NumberStyles.Number, CultureInfo.InvariantCulture, out double voltage) ? (int)voltage : null,
                    };
                    CachedCapacityInformation = info;
                }
            }
        }
        return CachedCapacityInformation;
    }

    #endregion Cache: CapacityInformation

    #region Cache: CurrentInformation

    private struct CurrentInformation {
        public double? InputVoltage;
        public double? OutputVoltage;
        public double? Frequency;
        public double? LoadPercentage;
        public double? BatteryPercentage;
        public int? BatteryRuntime;
        public bool? IsPendingPowerOn;
        public bool? IsPendingPowerOff;
        public bool? IsTesting;
        public bool? IsBeeping;
        public bool? IsBatteryLow;
        public bool? IsUsingBattery;
        public bool? IsBatteryCharging;
        public bool? IsBatteryFull;
        public bool? IsPoweredOff;
    }

    private readonly Stopwatch CachedCurrentInformationStopwatch = Stopwatch.StartNew();
    private CurrentInformation? CachedCurrentInformation;  // cached so UPS is queried only every second

    private CurrentInformation? RetrieveCurrentInformation() {
        if ((CachedCurrentInformation == null) || (CachedCurrentInformationStopwatch.ElapsedMilliseconds > 1000)) {
            CachedCurrentInformation = null;
            var data = Execute("D");
            if (data != null) {
                var info = new CurrentInformation();
                var parts = SplitOnLetter(data);
                foreach (var part in parts) {
                    switch (part[0]) {
                        case 'I':
                            if (double.TryParse(part[1..], NumberStyles.Number, CultureInfo.InvariantCulture, out double inputVoltage)) {
                                info.InputVoltage = inputVoltage;
                            }
                            break;
                        case 'O':
                            if (double.TryParse(part[1..], NumberStyles.Number, CultureInfo.InvariantCulture, out double outputVoltage)) {
                                info.OutputVoltage = outputVoltage;
                            }
                            break;
                        case 'L':
                            if (double.TryParse(part[1..], NumberStyles.Number, CultureInfo.InvariantCulture, out double loadPercentage)) {
                                info.LoadPercentage = loadPercentage / 100.0;
                            }
                            break;
                        case 'B':
                            if (double.TryParse(part[1..], NumberStyles.Number, CultureInfo.InvariantCulture, out double batteryPercentage)) {
                                info.BatteryPercentage = batteryPercentage / 100.0;
                            }
                            break;
                        case 'F':
                            if (double.TryParse(part[1..], NumberStyles.Number, CultureInfo.InvariantCulture, out double frequency)) {
                                info.Frequency = frequency;
                            }
                            break;
                        case 'R':
                            if (double.TryParse(part[1..], NumberStyles.Number, CultureInfo.InvariantCulture, out double runtime)) {
                                info.BatteryRuntime = (int)runtime;
                            }
                            break;
                        case 'S':
                            var bytes = Encoding.Latin1.GetBytes(part[1..]);
                            if ((bytes.Length >= 1) && ((bytes[0] & 0x80) == 0x80)) {
                                info.IsPendingPowerOn = (bytes[0] & 0x02) == 0x02;
                                info.IsPendingPowerOff = (bytes[0] & 0x04) == 0x04;
                                info.IsTesting = (bytes[0] & 0x08) == 0x08;
                                info.IsBeeping = (bytes[0] & 0x10) == 0x10;
                                info.IsBatteryLow = (bytes[0] & 0x20) == 0x20;
                                info.IsUsingBattery = (bytes[0] & 0x40) == 0x40;
                            }
                            if ((bytes.Length >= 3) && ((bytes[2] & 0x80) == 0x80)) {
                                info.IsBatteryCharging = (bytes[2] & 0x10) == 0x10;
                                info.IsBatteryFull = (bytes[2] & 0x40) == 0x40;
                            }
                            if ((bytes.Length >= 5) && ((bytes[4] & 0x80) == 0x80)) {
                                info.IsPoweredOff = (bytes[4] & 0x40) == 0x40;
                            }
                            break;
                    }
                }
                CachedCurrentInformation = info;
                CachedCurrentInformationStopwatch.Restart();
            }
        }
        return CachedCurrentInformation;
    }

    private static string[] SplitOnLetter(string text) {
        var allParts = new Queue<string>();
        var currPart = new Queue<char>();
        foreach (var ch in text) {
            if (ch is >= 'A' and <= 'Z') {
                if (currPart.Count > 0) {
                    allParts.Enqueue(new string(currPart.ToArray()));
                    currPart.Clear();
                }
            }
            currPart.Enqueue(ch);
        }

        if (currPart.Count > 0) {
            allParts.Enqueue(new string(currPart.ToArray()));
        }

        return allParts.ToArray();
    }


    #endregion Cache: CurrentInformation

}
