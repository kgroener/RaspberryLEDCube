using System.Collections.Generic;

namespace LEDCube.Simulator.WPF.Cube
{
    public class LEDCubeGeometryGroup
    {
        public LEDCubeGeometryGroup(int x, int y, int z)
        {
            ResolutionX = x;
            ResolutionY = y;
            ResolutionZ = z;

            List<LEDGeometryData> leds = new List<Cube.LEDGeometryData>();

            for (int ix = 0; ix < x; ix++)
            {
                for (int iy = 0; iy < y; iy++)
                {
                    for (int iz = 0; iz < z; iz++)
                    {
                        leds.Add(new Cube.LEDGeometryData((4 - ix) * 8, (4 - iy) * 8, (4 - iz) * 8));
                    }
                }
            }

            LEDs = leds.ToArray();
        }

        public LEDGeometryData[] LEDs { get; }
        public LEDGeometryData GetLEDAt(int x, int y, int z)
        {
            return LEDs[z + (y * ResolutionZ) + (x * ResolutionZ * ResolutionY)];
        }

        public int ResolutionX { get; }
        public int ResolutionY { get; }
        public int ResolutionZ { get; }
    }
}