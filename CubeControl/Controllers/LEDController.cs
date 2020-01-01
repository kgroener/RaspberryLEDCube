using RaspberryLEDCube.CanonicalSchema.Enums;
using RaspberryLEDCube.CanonicalSchema.Protocol;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Spi;

namespace RaspberryLEDCube.CubeControl.Controllers
{
    public class LEDController
    {
        private const int SPI_FREQUENCY = 10000000;

        private SpiDevice _ledDriver;
        private ChipSelectLines _csLine;

        public LEDController(ChipSelectLines csLine)
        {
            _csLine = csLine;
        }

        public async Task WriteColorBufferAsync(ProtocolColorBuffer colorBuffer)
        {
            var bytes = colorBuffer.GetBytes();

            //Debug.WriteLine(string.Join(", ", bytes));

            await Task.Delay(2);

            // SPI device requires the CS pin to be reset every 8 bytes, thus write the bytes in 8 byte chuncks
            while (bytes.Length > 0)
            {
                var bytesToWrite = bytes.Take(8).ToArray();
                bytes = bytes.Skip(8).ToArray();
                _ledDriver.Write(bytesToWrite);
            }
        }

        public async Task WriteBulkColorBufferAsync(ProtocolBulkColorBuffer buffer)
        {
            var bytes = buffer.GetBytes();

            // SPI device requires the CS pin to be reset every 8 bytes, thus write the bytes in 8 byte chuncks
            while (bytes.Length > 0)
            {
                var bytesToWrite = bytes.Take(8).ToArray();
                bytes = bytes.Skip(8).ToArray();
                _ledDriver.Write(bytesToWrite);
            }
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
    }
}
