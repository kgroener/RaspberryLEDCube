using LEDCube.Animations.Mathematics;
using LEDCube.Animations.Models;
using LEDCube.CanonicalSchema.Contract;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace LEDCube.Animations.Sprites
{
    public class Sprite3D
    {
        private readonly bool[][][] _pixels;
        private ColorFactory _colorFactory;

        public Sprite3D(bool[,,] spritePixels, ColorFactory colorFactory)
        {
            _colorFactory = colorFactory;
            var pixelsX = new List<bool[][]>();

            for (int x = 0; x < 8; x++)
            {
                var pixelsY = new List<bool[]>();

                for (int y = 0; y < 8; y++)
                {
                    var pixelsZ = new List<bool>();

                    for (int z = 0; z < 8; z++)
                    {
                        pixelsZ.Add(spritePixels[z, y, x]);
                    }

                    pixelsY.Add(pixelsZ.ToArray());
                }

                pixelsX.Add(pixelsY.ToArray());
            }

            _pixels = pixelsX.ToArray();
        }

        public Sprite3D(bool[,,] spritePixels, Color color) : this(spritePixels, (x, y, z) => color)
        {
        }

        public Sprite3D(bool[,,] spritePixels) : this(spritePixels, Color.Empty)
        {
        }

        public delegate Color ColorFactory(int x, int y, int z);

        public static void Draw(ILEDCube cube, Sprite3D sprite, double scale = 1, Coordinate offset = null, RotationMatrix matrix = null)
        {
            for (int x = 0; x < cube.ResolutionX; x++)
            {
                for (int y = 0; y < cube.ResolutionY; y++)
                {
                    for (int z = 0; z < cube.ResolutionZ; z++)
                    {
                        if (sprite._pixels[x][y][z])
                        {
                            var coordinate = new Coordinate(x / (double)cube.ResolutionX, y / (double)cube.ResolutionY, z / (double)cube.ResolutionZ);
                            if (matrix != null)
                            {
                                coordinate = RotationMatrix.Rotate(coordinate, new Coordinate(0.5, 0.5, 0.5), matrix);
                            }

                            coordinate.X *= scale;
                            coordinate.Y *= scale;
                            coordinate.Z *= scale;

                            coordinate.X += offset?.X ?? 0;
                            coordinate.Y += offset?.Y ?? 0;
                            coordinate.Z += offset?.Z ?? 0;

                            cube.SetLEDColor(coordinate.X, coordinate.Y, coordinate.Z, sprite._colorFactory(x, y, z));
                        }
                    }
                }
            }
        }

        public static void DrawAbsolute(ILEDCube cube, Sprite3D sprite, AbsoluteCoordinate offset = null)
        {
            for (int x = 0; x < cube.ResolutionX; x++)
            {
                for (int y = 0; y < cube.ResolutionY; y++)
                {
                    for (int z = 0; z < cube.ResolutionZ; z++)
                    {
                        if (sprite._pixels[x][y][z])
                        {
                            var px = x + (offset?.X ?? 0);
                            var py = y + (offset?.Y ?? 0);
                            var pz = z + (offset?.Z ?? 0);

                            if (px < 0 || px >= cube.ResolutionX || py < 0 || py >= cube.ResolutionY || pz < 0 || pz >= cube.ResolutionZ)
                            {
                                continue;
                            }

                            cube.SetLEDColorAbsolute(px, py, pz, sprite._colorFactory(x, y, z));
                        }
                    }
                }
            }
        }

        public static Sprite3D Rotate(Sprite3D sprite, RotationMatrix matrix)
        {
            for (var x = 0; x < sprite._pixels.Count(); x++)
            {
                for (var y = 0; y < sprite._pixels[x].Count(); y++)
                {
                    for (var z = 0; z < sprite._pixels[x][y].Count(); z++)
                    {
                    }
                }
            }

            return null;
        }

        public void SetColorFactory(ColorFactory colorFactory)
        {
            _colorFactory = colorFactory;
        }
    }
}