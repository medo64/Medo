using System;
using Xunit;
using Medo.Device;
using System.IO;
using System.Text;
using System.Threading;

namespace Tests.Medo.Device {
    public class CyberCardTests {

        [Fact(DisplayName = "CyberCard: Out of range")]
        public void NullPort() {
            Assert.Throws<ArgumentNullException>(() => {
                var _ = new CyberCard(default(String));
            });
            Assert.Throws<ArgumentNullException>(() => {
                var _ = new CyberCard(default(Stream));
            });
        }

        [Fact(DisplayName = "CyberCard: Model information")]
        public void ModelInformation() {
            var stream = new TestStream();
            var device = new CyberCard(stream);

            // survive without data
            Assert.Null(device.GetDeviceModel());
            Assert.Null(device.GetDeviceFirmware());
            Assert.Null(device.GetDeviceSerial());
            Assert.Null(device.GetDeviceManufacturer());
            Assert.Equal("P4\rP4\rP4\rP4\r", Encoding.ASCII.GetString(stream.ToWrittenArray()));  // check we tried for each

            // now we have data
            stream.SetupRead(Encoding.ASCII.GetBytes("#OR700LCDRM1U,BFE7103_8S1,000000000000,CyberPower\r"));
            Assert.Equal("OR700LCDRM1U", device.GetDeviceModel());
            Assert.Equal("BFE7103_8S1", device.GetDeviceFirmware());
            Assert.Equal("", device.GetDeviceSerial());  // ignore all 0's serial
            Assert.Equal("CyberPower", device.GetDeviceManufacturer());
            Assert.Equal("P4\rP4\rP4\rP4\rP4\r", Encoding.ASCII.GetString(stream.ToWrittenArray()));  // check we tried only once more

            // subsequent read goes from cache
            Assert.Equal("OR700LCDRM1U", device.GetDeviceModel());
            Assert.Equal("BFE7103_8S1", device.GetDeviceFirmware());
            Assert.Equal("", device.GetDeviceSerial());
            Assert.Equal("CyberPower", device.GetDeviceManufacturer());
            Assert.Equal("P4\rP4\rP4\rP4\rP4\r", Encoding.ASCII.GetString(stream.ToWrittenArray()));  // no new reads
        }

        [Fact(DisplayName = "CyberCard: Capability information")]
        public void CapabilityInformation() {
            var stream = new TestStream();
            var device = new CyberCard(stream);

            // survive without data
            Assert.Null(device.GetDeviceCapacity());
            Assert.Null(device.GetDeviceCapacityVA());
            Assert.Null(device.GetDeviceVoltage());
            Assert.Equal("P2\rP2\rP2\r", Encoding.ASCII.GetString(stream.ToWrittenArray()));  // check we tried for each

            // now we have data
            stream.SetupRead(Encoding.ASCII.GetBytes("#0700,0400,120,057,063\r"));
            Assert.Equal(400, device.GetDeviceCapacity());
            Assert.Equal(700, device.GetDeviceCapacityVA());
            Assert.Equal(120, device.GetDeviceVoltage());
            Assert.Equal("P2\rP2\rP2\rP2\r", Encoding.ASCII.GetString(stream.ToWrittenArray()));  // check we tried only once more

            // subsequent read goes from cache
            Assert.Equal(400, device.GetDeviceCapacity());
            Assert.Equal(700, device.GetDeviceCapacityVA());
            Assert.Equal(120, device.GetDeviceVoltage());
            Assert.Equal("P2\rP2\rP2\rP2\r", Encoding.ASCII.GetString(stream.ToWrittenArray()));  // no new reads
        }

        [Fact(DisplayName = "CyberCard: Current information")]
        public void CurrentInformation() {
            var stream = new TestStream();
            var device = new CyberCard(stream);

            // survive without data
            Assert.Null(device.GetInputVoltage());
            Assert.Null(device.GetOutputVoltage());
            Assert.Null(device.GetFrequency());
            Assert.Null(device.GetLoadPercentage());
            Assert.Null(device.GetBatteryPercentage());
            Assert.Null(device.GetBatteryRuntime());
            Assert.Equal("D\rD\rD\rD\rD\rD\r", Encoding.ASCII.GetString(stream.ToWrittenArray()));  // check we tried for each

            // now we have data
            stream.SetupRead(Encoding.ASCII.GetBytes("#I121.0O121.0L042B088F060.1R073S\x80\x84\x90\x80\x80\r"));
            Assert.Equal(121, device.GetInputVoltage());
            Assert.Equal(121, device.GetOutputVoltage());
            Assert.Equal(60.1, device.GetFrequency());
            Assert.Equal(0.42, device.GetLoadPercentage());
            Assert.Equal(0.88, device.GetBatteryPercentage());
            Assert.Equal(73, device.GetBatteryRuntime());
            Assert.Equal("D\rD\rD\rD\rD\rD\rD\r", Encoding.ASCII.GetString(stream.ToWrittenArray()));  // check we tried only once more

            // subsequent read goes from cache
            Assert.Equal(121, device.GetInputVoltage());
            Assert.Equal(121, device.GetOutputVoltage());
            Assert.Equal(60.1, device.GetFrequency());
            Assert.Equal(0.42, device.GetLoadPercentage());
            Assert.Equal(0.88, device.GetBatteryPercentage());
            Assert.Equal(73, device.GetBatteryRuntime());
            Assert.Equal("D\rD\rD\rD\rD\rD\rD\r", Encoding.ASCII.GetString(stream.ToWrittenArray()));  // no new reads
        }


        [Fact(DisplayName = "CyberCard: Current information (flags)")]
        public void CurrentInformation_Flags() {
            var stream = new TestStream();
            var device = new CyberCard(stream);

            // survive without data
            Assert.Null(device.IsPendingPowerOn());
            Assert.Null(device.IsPendingPowerOff());
            Assert.Null(device.IsTestInProgress());
            Assert.Null(device.IsAlarmActive());
            Assert.Null(device.IsUsingBattery());
            Assert.Null(device.IsBatteryLow());
            Assert.Null(device.IsBatteryCharging());
            Assert.Null(device.IsBatteryFull());
            Assert.Null(device.IsPoweredOff());
            Assert.Null(device.IsPoweredOn());
            Assert.Equal("D\rD\rD\rD\rD\rD\rD\rD\rD\rD\r", Encoding.ASCII.GetString(stream.ToWrittenArray()));  // check we tried for each

            // now we have data
            stream.SetupRead(Encoding.Latin1.GetBytes("#I121.0O121.0L042B088F060.1R073S\x80\x84\x90\x80\x80\r"));
            Assert.False(device.IsPendingPowerOn());
            Assert.False(device.IsPendingPowerOff());
            Assert.False(device.IsTestInProgress());
            Assert.False(device.IsAlarmActive());
            Assert.False(device.IsUsingBattery());
            Assert.False(device.IsBatteryLow());
            Assert.True (device.IsBatteryCharging());
            Assert.False(device.IsBatteryFull());
            Assert.False(device.IsPoweredOff());
            Assert.True(device.IsPoweredOn());
            Assert.Equal("D\rD\rD\rD\rD\rD\rD\rD\rD\rD\rD\r", Encoding.ASCII.GetString(stream.ToWrittenArray()));  // check we tried only once more

            // subsequent read goes from cache
            Assert.False(device.IsPendingPowerOn());
            Assert.False(device.IsPendingPowerOff());
            Assert.False(device.IsTestInProgress());
            Assert.False(device.IsAlarmActive());
            Assert.False(device.IsUsingBattery());
            Assert.False(device.IsBatteryLow());
            Assert.True(device.IsBatteryCharging());
            Assert.False(device.IsBatteryFull());
            Assert.False(device.IsPoweredOff());
            Assert.True(device.IsPoweredOn());
            Assert.Equal("D\rD\rD\rD\rD\rD\rD\rD\rD\rD\rD\r", Encoding.ASCII.GetString(stream.ToWrittenArray()));  // no new reads

            // wait for cache to expire
            Thread.Sleep(1000);
            stream.SetupRead(Encoding.Latin1.GetBytes("#I000.0O120.0L000B100F060.1R084S\xc0\x81\x88\x80\x80\r"));
            Assert.False(device.IsPendingPowerOn());
            Assert.False(device.IsPendingPowerOff());
            Assert.False(device.IsTestInProgress());
            Assert.False(device.IsAlarmActive());
            Assert.True(device.IsUsingBattery());
            Assert.False(device.IsBatteryLow());
            Assert.False(device.IsBatteryCharging());
            Assert.False(device.IsBatteryFull());
            Assert.False(device.IsPoweredOff());
            Assert.True(device.IsPoweredOn());
            Assert.Equal("D\rD\rD\rD\rD\rD\rD\rD\rD\rD\rD\rD\r", Encoding.ASCII.GetString(stream.ToWrittenArray()));  // check we tried only once more
        }

        [Fact(DisplayName = "CyberCard: Current information (short caching)")]
        public void CurrentInformation_Cache() {
            var stream = new TestStream();
            var device = new CyberCard(stream);

            // we have data
            stream.SetupRead(Encoding.ASCII.GetBytes("#I121.0O121.0L042B088F060.1R073S\x80\x84\x90\x80\x80\r"));
            Assert.Equal(121, device.GetInputVoltage());
            Assert.Equal(121, device.GetOutputVoltage());
            Assert.Equal(60.1, device.GetFrequency());
            Assert.Equal(0.42, device.GetLoadPercentage());
            Assert.Equal(0.88, device.GetBatteryPercentage());
            Assert.Equal(73, device.GetBatteryRuntime());
            Assert.Equal("D\r", Encoding.ASCII.GetString(stream.ToWrittenArray()));  // check we tried only once

            // subsequent read goes from cache
            Assert.Equal(121, device.GetInputVoltage());
            Assert.Equal(121, device.GetOutputVoltage());
            Assert.Equal(60.1, device.GetFrequency());
            Assert.Equal(0.42, device.GetLoadPercentage());
            Assert.Equal(0.88, device.GetBatteryPercentage());
            Assert.Equal(73, device.GetBatteryRuntime());
            Assert.Equal("D\r", Encoding.ASCII.GetString(stream.ToWrittenArray()));  // no new reads

            // expire cache
            Thread.Sleep(1000);
            Assert.Null(device.GetInputVoltage());
            Assert.Null(device.GetOutputVoltage());
            Assert.Null(device.GetFrequency());
            Assert.Null(device.GetLoadPercentage());
            Assert.Null(device.GetBatteryPercentage());
            Assert.Null(device.GetBatteryRuntime());
            Assert.Equal("D\rD\rD\rD\rD\rD\rD\r", Encoding.ASCII.GetString(stream.ToWrittenArray()));  // cache failure causes a new read every time

            // new data
            stream.SetupRead(Encoding.ASCII.GetBytes("#I121.4O122L41B087F060.4R033S\x80\x84\x90\x80\x80\r"));
            Assert.Equal(121.4, device.GetInputVoltage());
            Assert.Equal(122, device.GetOutputVoltage());
            Assert.Equal(60.4, device.GetFrequency());
            Assert.Equal(0.41, device.GetLoadPercentage());
            Assert.Equal(0.87, device.GetBatteryPercentage());
            Assert.Equal(33, device.GetBatteryRuntime());
            Assert.Equal("D\rD\rD\rD\rD\rD\rD\rD\r", Encoding.ASCII.GetString(stream.ToWrittenArray()));  // only one new read once we have data

            // subsequent read goes from cache
            Assert.Equal(121.4, device.GetInputVoltage());
            Assert.Equal(122, device.GetOutputVoltage());
            Assert.Equal(60.4, device.GetFrequency());
            Assert.Equal(0.41, device.GetLoadPercentage());
            Assert.Equal(0.87, device.GetBatteryPercentage());
            Assert.Equal(33, device.GetBatteryRuntime());
            Assert.Equal("D\rD\rD\rD\rD\rD\rD\rD\r", Encoding.ASCII.GetString(stream.ToWrittenArray()));  // no new reads
        }


        [Fact(DisplayName = "CyberCard: Power on")]
        public void PowerOn() {
            var stream = new TestStream();
            var device = new CyberCard(stream);

            // immediate power off
            device.PowerOff();
            Assert.Equal("S\r", Encoding.ASCII.GetString(stream.ToWrittenArray()));

            // power off after 60 minutes
            device.PowerOff(new TimeSpan(1, 0, 0));
            Assert.Equal("S\rS60\r", Encoding.ASCII.GetString(stream.ToWrittenArray()));

            // power reset
            device.PowerReset();
            Assert.Equal("S\rS60\rS00R0000\r", Encoding.ASCII.GetString(stream.ToWrittenArray()));

            // power off after 1 minute and power on after 900 minutes
            device.PowerReset(new TimeSpan(0, 1, 0), new TimeSpan(0, 900, 0));
            Assert.Equal("S\rS60\rS00R0000\rS01R0900\r", Encoding.ASCII.GetString(stream.ToWrittenArray()));

            // power on immediatelly
            device.PowerOn();
            Assert.Equal("S\rS60\rS00R0000\rS01R0900\rW\r", Encoding.ASCII.GetString(stream.ToWrittenArray()));

            // cancel power off
            device.CancelPowerOff();
            Assert.Equal("S\rS60\rS00R0000\rS01R0900\rW\rC\r", Encoding.ASCII.GetString(stream.ToWrittenArray()));
        }

        [Fact(DisplayName = "CyberCard: Alarm")]
        public void Alarm() {
            var stream = new TestStream();
            var device = new CyberCard(stream);

            // enable
            device.AlarmDisable();
            Assert.Equal("C7:0\r", Encoding.ASCII.GetString(stream.ToWrittenArray()));

            // enable
            device.AlarmEnable();
            Assert.Equal("C7:0\rC7:1\r", Encoding.ASCII.GetString(stream.ToWrittenArray()));
        }

    }
}
