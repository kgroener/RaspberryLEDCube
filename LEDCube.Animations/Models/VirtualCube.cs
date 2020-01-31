using LEDCube.Animations.Controllers;
using LEDCube.CanonicalSchema.Contract;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace LEDCube.Animations.Models
{
    internal class VirtualCube : LEDCubeBase
    {
        private int _resolutionX;
        private int _resolutionY;
        private int _resolutionZ;

        public VirtualCube()
        {
        }

        public override int ResolutionX => _resolutionX;

        public override int ResolutionY => _resolutionY;

        public override int ResolutionZ => _resolutionZ;

        public void Initialize(ILEDCube cube)
        {
            Initialize(cube.ResolutionX, cube.ResolutionY, cube.ResolutionZ);
        }

        public void Initialize(int rx, int ry, int rz)
        {
            _resolutionX = rx;
            _resolutionY = ry;
            _resolutionZ = rz;
            Initialize();
        }

        public void WriteToCube(ILEDCube cube)
        {
            for (int x = 0; x < ResolutionX; x++)
            {
                for (int y = 0; y < ResolutionY; y++)
                {
                    for (int z = 0; z < ResolutionZ; z++)
                    {
                        var color = GetLEDColorAbsolute(x, y, z);
                        if (!color.IsEmpty)
                        {
                            cube.SetLEDColorAbsolute(x, y, z, color);
                        }
                    }
                }
            }
        }
    }
}