using LEDCube.Animations.Models;
using LEDCube.CanonicalSchema.Contract;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace LEDCube.Animations.Helpers
{
    public static class SphereHelper
    {
        public static void DrawSphere(this ILEDCube cube, Coordinate origin, double radius, Color color)
        {
            for (int x = 0; x < cube.ResolutionX; x++)
            {
                for (int y = 0; y < cube.ResolutionY; y++)
                {
                    for (int z = 0; z < cube.ResolutionZ; z++)
                    {
                        var dx = origin.X - (x / (double)cube.ResolutionX);
                        var dy = origin.Y - (y / (double)cube.ResolutionY);
                        var dz = origin.Z - (z / (double)cube.ResolutionZ);

                        var distance = Math.Sqrt((dx * dx) + (dy * dy) + (dz * dz));

                        if (distance <= radius)
                        {
                            cube.SetLEDColor(dx + origin.X, dy + origin.Y, dz + origin.Z, color);
                        }
                    }
                }
            }
        }
    }
}