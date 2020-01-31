using Microsoft.IoT.Lightning.Providers;
using RaspberryLEDCube.CanonicalSchema.Enums;
using RaspberryLEDCube.CanonicalSchema.Protocol;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices;
using Windows.Devices.Spi;

namespace RaspberryLEDCube.CubeControl.Controllers
{
    public class LEDController
    {
        private const byte ESCAPE_BYTE = 0x10;
        private const byte RESET_BYTE = 0x0C;
        private const int SPI_FREQUENCY = 10000000;
        private readonly ChipSelectLines _csLine;
        private SpiDevice _ledDriver;

        public LEDController(ChipSelectLines csLine)
        {
            _csLine = csLine;
        }

        public async Task InitializeAsync()
        {
            try
            {
                var settings = new SpiConnectionSettings((int)_csLine);
                settings.ClockFrequency = SPI_FREQUENCY;
                settings.Mode = SpiMode.Mode3;

                var spiController = await SpiController.GetDefaultAsync();
                if (spiController == null)
                {
                    throw new Exception("SPI device is in use by another application");
                }

                _ledDriver = spiController.GetDevice(settings);
            }
            catch (Exception ex)
            {
                throw new Exception("SPI Initialization Failed", ex);
            }
        }

        public async Task WriteBulkColorBufferAsync(ProtocolBulkColorBuffer buffer)
        {
            var bytes = buffer.GetBytes();

            await WaitForBusyDeviceAsync();

            // SPI device requires the CS pin to be reset every 8 bytes, thus write the bytes in 8 byte chuncks
            while (bytes.Length > 0)
            {
                var bytesToWrite = bytes.Take(8).ToArray();
                bytes = bytes.Skip(8).ToArray();
                _ledDriver.Write(bytesToWrite);
            }
        }

        public async Task WriteColorBufferAsync(ProtocolColorBuffer colorBuffer)
        {
            var bytes = colorBuffer.GetBytes();

            await WaitForBusyDeviceAsync();

            // SPI device requires the CS pin to be reset every 8 bytes, thus write the bytes in 8 byte chuncks
            while (bytes.Length > 0)
            {
                var bytesToWrite = bytes.Take(8).ToArray();
                bytes = bytes.Skip(8).ToArray();
                _ledDriver.Write(bytesToWrite);
            }
        }

        private async Task WaitForBusyDeviceAsync()
        {
            _ledDriver.Write(new byte[] { ESCAPE_BYTE, RESET_BYTE });

            bool isBusy;
            do
            {
                await Task.Delay(1);
                var readByte = new byte[1];
                _ledDriver.Read(readByte);
                isBusy = readByte[0] > 0;
            }
            while (isBusy);
        }
    }
}