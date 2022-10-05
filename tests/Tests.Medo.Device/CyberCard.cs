using System;
using System.IO;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Medo.Device;

namespace Tests;

[TestClass]
public class CyberCard_Tests {

    [TestMethod]
    public void CyberCard_NullPort() {
        Assert.ThrowsException<ArgumentNullException>(() => {
            var _ = new CyberCard(default(String));
        });
        Assert.ThrowsException<ArgumentNullException>(() => {
            var _ = new CyberCard(default(Stream));
        });
    }

    [TestMethod]
    public void CyberCard_ModelInformation() {
        var stream = new TestStream();
        using var device = new CyberCard(stream);

        // survive without data
        Assert.IsNull(device.GetDeviceModel());
        Assert.IsNull(device.GetDeviceFirmware());
        Assert.IsNull(device.GetDeviceSerial());
        Assert.IsNull(device.GetDeviceManufacturer());
        Assert.AreEqual("P4\rP4\rP4\rP4\r", Encoding.ASCII.GetString(stream.ToWrittenArray()));  // check we tried for each

        // now we have data
        stream.SetupRead(Encoding.ASCII.GetBytes("#OR700LCDRM1U,BFE7103_8S1,000000000000,CyberPower\r"));
        Assert.AreEqual("OR700LCDRM1U", device.GetDeviceModel());
        Assert.AreEqual("BFE7103_8S1", device.GetDeviceFirmware());
        Assert.AreEqual("", device.GetDeviceSerial());  // ignore all 0's serial
        Assert.AreEqual("CyberPower", device.GetDeviceManufacturer());
        Assert.AreEqual("P4\rP4\rP4\rP4\rP4\r", Encoding.ASCII.GetString(stream.ToWrittenArray()));  // check we tried only once more

        // subsequent read goes from cache
        Assert.AreEqual("OR700LCDRM1U", device.GetDeviceModel());
        Assert.AreEqual("BFE7103_8S1", device.GetDeviceFirmware());
        Assert.AreEqual("", device.GetDeviceSerial());
        Assert.AreEqual("CyberPower", device.GetDeviceManufacturer());
        Assert.AreEqual("P4\rP4\rP4\rP4\rP4\r", Encoding.ASCII.GetString(stream.ToWrittenArray()));  // no new reads
    }

    [TestMethod]
    public void CyberCard_CapabilityInformation() {
        var stream = new TestStream();
        using var device = new CyberCard(stream);

        // survive without data
        Assert.IsNull(device.GetDeviceCapacity());
        Assert.IsNull(device.GetDeviceCapacityVA());
        Assert.IsNull(device.GetDeviceVoltage());
        Assert.AreEqual("P2\rP2\rP2\r", Encoding.ASCII.GetString(stream.ToWrittenArray()));  // check we tried for each

        // now we have data
        stream.SetupRead(Encoding.ASCII.GetBytes("#0700,0400,120,057,063\r"));
        Assert.AreEqual(400, device.GetDeviceCapacity());
        Assert.AreEqual(700, device.GetDeviceCapacityVA());
        Assert.AreEqual(120, device.GetDeviceVoltage());
        Assert.AreEqual("P2\rP2\rP2\rP2\r", Encoding.ASCII.GetString(stream.ToWrittenArray()));  // check we tried only once more

        // subsequent read goes from cache
        Assert.AreEqual(400, device.GetDeviceCapacity());
        Assert.AreEqual(700, device.GetDeviceCapacityVA());
        Assert.AreEqual(120, device.GetDeviceVoltage());
        Assert.AreEqual("P2\rP2\rP2\rP2\r", Encoding.ASCII.GetString(stream.ToWrittenArray()));  // no new reads
    }

    [TestMethod]
    public void CyberCard_CurrentInformation() {
        var stream = new TestStream();
        using var device = new CyberCard(stream);

        // survive without data
        Assert.IsNull(device.GetInputVoltage());
        Assert.IsNull(device.GetOutputVoltage());
        Assert.IsNull(device.GetFrequency());
        Assert.IsNull(device.GetLoadPercentage());
        Assert.IsNull(device.GetBatteryPercentage());
        Assert.IsNull(device.GetBatteryRuntime());
        Assert.AreEqual("D\rD\rD\rD\rD\rD\r", Encoding.ASCII.GetString(stream.ToWrittenArray()));  // check we tried for each

        // now we have data
        stream.SetupRead(Encoding.ASCII.GetBytes("#I121.0O121.0L042B088F060.1R073S\x80\x84\x90\x80\x80\r"));
        Assert.AreEqual(121, device.GetInputVoltage());
        Assert.AreEqual(121, device.GetOutputVoltage());
        Assert.AreEqual(60.1, device.GetFrequency());
        Assert.AreEqual(0.42, device.GetLoadPercentage());
        Assert.AreEqual(0.88, device.GetBatteryPercentage());
        Assert.AreEqual(73, device.GetBatteryRuntime());
        Assert.AreEqual("D\rD\rD\rD\rD\rD\rD\r", Encoding.ASCII.GetString(stream.ToWrittenArray()));  // check we tried only once more

        // subsequent read goes from cache
        Assert.AreEqual(121, device.GetInputVoltage());
        Assert.AreEqual(121, device.GetOutputVoltage());
        Assert.AreEqual(60.1, device.GetFrequency());
        Assert.AreEqual(0.42, device.GetLoadPercentage());
        Assert.AreEqual(0.88, device.GetBatteryPercentage());
        Assert.AreEqual(73, device.GetBatteryRuntime());
        Assert.AreEqual("D\rD\rD\rD\rD\rD\rD\r", Encoding.ASCII.GetString(stream.ToWrittenArray()));  // no new reads
    }


    [TestMethod]
    public void CyberCard_CurrentInformation_Flags() {
        var stream = new TestStream();
        using var device = new CyberCard(stream);

        // survive without data
        Assert.IsNull(device.IsPendingPowerOn());
        Assert.IsNull(device.IsPendingPowerOff());
        Assert.IsNull(device.IsTestInProgress());
        Assert.IsNull(device.IsAlarmActive());
        Assert.IsNull(device.IsUsingBattery());
        Assert.IsNull(device.IsBatteryLow());
        Assert.IsNull(device.IsBatteryCharging());
        Assert.IsNull(device.IsBatteryFull());
        Assert.IsNull(device.IsPoweredOff());
        Assert.IsNull(device.IsPoweredOn());
        Assert.AreEqual("D\rD\rD\rD\rD\rD\rD\rD\rD\rD\r", Encoding.ASCII.GetString(stream.ToWrittenArray()));  // check we tried for each

        // now we have data
        stream.SetupRead(Encoding.Latin1.GetBytes("#I121.0O121.0L042B088F060.1R073S\x80\x84\x90\x80\x80\r"));
        Assert.IsFalse(device.IsPendingPowerOn());
        Assert.IsFalse(device.IsPendingPowerOff());
        Assert.IsFalse(device.IsTestInProgress());
        Assert.IsFalse(device.IsAlarmActive());
        Assert.IsFalse(device.IsUsingBattery());
        Assert.IsFalse(device.IsBatteryLow());
        Assert.IsTrue(device.IsBatteryCharging());
        Assert.IsFalse(device.IsBatteryFull());
        Assert.IsFalse(device.IsPoweredOff());
        Assert.IsTrue(device.IsPoweredOn());
        Assert.AreEqual("D\rD\rD\rD\rD\rD\rD\rD\rD\rD\rD\r", Encoding.ASCII.GetString(stream.ToWrittenArray()));  // check we tried only once more

        // subsequent read goes from cache
        Assert.IsFalse(device.IsPendingPowerOn());
        Assert.IsFalse(device.IsPendingPowerOff());
        Assert.IsFalse(device.IsTestInProgress());
        Assert.IsFalse(device.IsAlarmActive());
        Assert.IsFalse(device.IsUsingBattery());
        Assert.IsFalse(device.IsBatteryLow());
        Assert.IsTrue(device.IsBatteryCharging());
        Assert.IsFalse(device.IsBatteryFull());
        Assert.IsFalse(device.IsPoweredOff());
        Assert.IsTrue(device.IsPoweredOn());
        Assert.AreEqual("D\rD\rD\rD\rD\rD\rD\rD\rD\rD\rD\r", Encoding.ASCII.GetString(stream.ToWrittenArray()));  // no new reads

        // wait for cache to expire
        Thread.Sleep(1000);
        stream.SetupRead(Encoding.Latin1.GetBytes("#I000.0O120.0L000B100F060.1R084S\xc0\x81\x88\x80\x80\r"));
        Assert.IsFalse(device.IsPendingPowerOn());
        Assert.IsFalse(device.IsPendingPowerOff());
        Assert.IsFalse(device.IsTestInProgress());
        Assert.IsFalse(device.IsAlarmActive());
        Assert.IsTrue(device.IsUsingBattery());
        Assert.IsFalse(device.IsBatteryLow());
        Assert.IsFalse(device.IsBatteryCharging());
        Assert.IsFalse(device.IsBatteryFull());
        Assert.IsFalse(device.IsPoweredOff());
        Assert.IsTrue(device.IsPoweredOn());
        Assert.AreEqual("D\rD\rD\rD\rD\rD\rD\rD\rD\rD\rD\rD\r", Encoding.ASCII.GetString(stream.ToWrittenArray()));  // check we tried only once more
    }

    [TestMethod]
    public void CyberCard_CurrentInformation_Cache() {
        var stream = new TestStream();
        using var device = new CyberCard(stream);

        // we have data
        stream.SetupRead(Encoding.ASCII.GetBytes("#I121.0O121.0L042B088F060.1R073S\x80\x84\x90\x80\x80\r"));
        Assert.AreEqual(121, device.GetInputVoltage());
        Assert.AreEqual(121, device.GetOutputVoltage());
        Assert.AreEqual(60.1, device.GetFrequency());
        Assert.AreEqual(0.42, device.GetLoadPercentage());
        Assert.AreEqual(0.88, device.GetBatteryPercentage());
        Assert.AreEqual(73, device.GetBatteryRuntime());
        Assert.AreEqual("D\r", Encoding.ASCII.GetString(stream.ToWrittenArray()));  // check we tried only once

        // subsequent read goes from cache
        Assert.AreEqual(121, device.GetInputVoltage());
        Assert.AreEqual(121, device.GetOutputVoltage());
        Assert.AreEqual(60.1, device.GetFrequency());
        Assert.AreEqual(0.42, device.GetLoadPercentage());
        Assert.AreEqual(0.88, device.GetBatteryPercentage());
        Assert.AreEqual(73, device.GetBatteryRuntime());
        Assert.AreEqual("D\r", Encoding.ASCII.GetString(stream.ToWrittenArray()));  // no new reads

        // expire cache
        Thread.Sleep(1000);
        Assert.IsNull(device.GetInputVoltage());
        Assert.IsNull(device.GetOutputVoltage());
        Assert.IsNull(device.GetFrequency());
        Assert.IsNull(device.GetLoadPercentage());
        Assert.IsNull(device.GetBatteryPercentage());
        Assert.IsNull(device.GetBatteryRuntime());
        Assert.AreEqual("D\rD\rD\rD\rD\rD\rD\r", Encoding.ASCII.GetString(stream.ToWrittenArray()));  // cache failure causes a new read every time

        // new data
        stream.SetupRead(Encoding.ASCII.GetBytes("#I121.4O122L41B087F060.4R033S\x80\x84\x90\x80\x80\r"));
        Assert.AreEqual(121.4, device.GetInputVoltage());
        Assert.AreEqual(122, device.GetOutputVoltage());
        Assert.AreEqual(60.4, device.GetFrequency());
        Assert.AreEqual(0.41, device.GetLoadPercentage());
        Assert.AreEqual(0.87, device.GetBatteryPercentage());
        Assert.AreEqual(33, device.GetBatteryRuntime());
        Assert.AreEqual("D\rD\rD\rD\rD\rD\rD\rD\r", Encoding.ASCII.GetString(stream.ToWrittenArray()));  // only one new read once we have data

        // subsequent read goes from cache
        Assert.AreEqual(121.4, device.GetInputVoltage());
        Assert.AreEqual(122, device.GetOutputVoltage());
        Assert.AreEqual(60.4, device.GetFrequency());
        Assert.AreEqual(0.41, device.GetLoadPercentage());
        Assert.AreEqual(0.87, device.GetBatteryPercentage());
        Assert.AreEqual(33, device.GetBatteryRuntime());
        Assert.AreEqual("D\rD\rD\rD\rD\rD\rD\rD\r", Encoding.ASCII.GetString(stream.ToWrittenArray()));  // no new reads
    }


    [TestMethod]
    public void CyberCard_PowerOn() {
        var stream = new TestStream();
        using var device = new CyberCard(stream);

        // immediate power off
        device.PowerOff();
        Assert.AreEqual("S\r", Encoding.ASCII.GetString(stream.ToWrittenArray()));

        // power off after 60 minutes
        device.PowerOff(new TimeSpan(1, 0, 0));
        Assert.AreEqual("S\rS60\r", Encoding.ASCII.GetString(stream.ToWrittenArray()));

        // power reset
        device.PowerReset();
        Assert.AreEqual("S\rS60\rS00R0000\r", Encoding.ASCII.GetString(stream.ToWrittenArray()));

        // power off after 1 minute and power on after 900 minutes
        device.PowerReset(new TimeSpan(0, 1, 0), new TimeSpan(0, 900, 0));
        Assert.AreEqual("S\rS60\rS00R0000\rS01R0900\r", Encoding.ASCII.GetString(stream.ToWrittenArray()));

        // power on immediatelly
        device.PowerOn();
        Assert.AreEqual("S\rS60\rS00R0000\rS01R0900\rW\r", Encoding.ASCII.GetString(stream.ToWrittenArray()));

        // cancel power off
        device.CancelPowerOff();
        Assert.AreEqual("S\rS60\rS00R0000\rS01R0900\rW\rC\r", Encoding.ASCII.GetString(stream.ToWrittenArray()));
    }

    [TestMethod]
    public void CyberCard_Alarm() {
        var stream = new TestStream();
        using var device = new CyberCard(stream);

        // enable
        device.AlarmDisable();
        Assert.AreEqual("C7:0\r", Encoding.ASCII.GetString(stream.ToWrittenArray()));

        // enable
        device.AlarmEnable();
        Assert.AreEqual("C7:0\rC7:1\r", Encoding.ASCII.GetString(stream.ToWrittenArray()));
    }

}
