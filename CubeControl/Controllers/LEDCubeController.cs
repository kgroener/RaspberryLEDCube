using LEDCube.Animations.Models;
using LEDCube.CanonicalSchema.Contract;
using RaspberryLEDCube.CanonicalSchema.Protocol;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace RaspberryLEDCube.CubeControl.Controllers
{
    public class LEDCubeController : LEDCubeBase, ILEDCubeController
    {
        private readonly ProtocolBulkColorBuffer _cubeColorBuffer;

        private readonly ProtocolColorBuffer[] _cubeColorBuffers;

        private readonly LEDController _ledController;

        public LEDCubeController(LEDController ledController)
        {
            _ledController = ledController;

            var layers = ResolutionZ;
            var ledsPerLayer = ResolutionX * ResolutionY;

            _cubeColorBuffers = Enumerable.Range(0, layers).Select(l => new ProtocolColorBuffer(ledsPerLayer, (byte)l)).ToArray();
            _cubeColorBuffer = new ProtocolBulkColorBuffer(_cubeColorBuffers);
        }

        public override int ResolutionX => 8;

        public override int ResolutionY => 8;

        public override int ResolutionZ => 8;

        public async Task DrawAsync(double dimFactor = 1)
        {
            if (HasBeenUpdated)
            {
                for (byte x = 0; x < ResolutionX; x++)
                {
                    for (byte y = 0; y < ResolutionY; y++)
                    {
                        for (byte z = 0; z < ResolutionZ; z++)
                        {
                            var color = GetLEDColorAbsolute(x, y, z);
                            int index = GetLEDBufferIndex((byte)x, (byte)z);
                            SetLEDColorInBuffer((byte)y, index, color, dimFactor);
                        }
                    }
                }

                await _ledController.WriteBulkColorBufferAsync(_cubeColorBuffer);
                HasBeenUpdated = false;
            }
        }

        private int GetLEDBufferIndex(byte x, byte z)
        {
            int index;

            var isZEven = (z % 2) == 0;

            if (isZEven)
            {
                index = x + (z * ResolutionX);
            }
            else
            {
                index = ResolutionX - x - 1 + (z * ResolutionX);
            }

            return index;
        }

        private void SetLEDColorInBuffer(byte layer, int index, Color color, double dimFactor = 1)
        {
            _cubeColorBuffers[layer][index].Red = (byte)(color.R * dimFactor);
            _cubeColorBuffers[layer][index].Green = (byte)(color.G * dimFactor);
            _cubeColorBuffers[layer][index].Blue = (byte)(color.B * dimFactor);
        }

        private struct ColorPart
        {
            public System.Drawing.Color Color { get; set; }
            public double Part { get; set; }
        }
    }
}