using LEDCube.CanonicalSchema.Contract;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace LEDCube.Animations.Models
{
    public abstract class LEDCubeBase : ILEDCube
    {
        private Dictionary<int, Dictionary<int, Dictionary<int, List<ColorPart>>>> _virtualCube;

        public LEDCubeBase()
        {
            HasBeenInitialized = false;
        }

        public bool HasBeenInitialized { get; private set; }

        public bool HasBeenUpdated { get; protected set; }

        public abstract int ResolutionX { get; }

        public abstract int ResolutionY { get; }

        public abstract int ResolutionZ { get; }

        public void Clear()
        {
            foreach (var x in _virtualCube.Keys)
            {
                foreach (var y in _virtualCube[x].Keys)
                {
                    foreach (var z in _virtualCube[x][y].Keys)
                    {
                        _virtualCube[x][y][z].Clear();
                    }
                }
            }
            HasBeenUpdated = true;
        }

        public void Fill(Color fillColor)
        {
            foreach (var x in _virtualCube.Keys)
            {
                foreach (var y in _virtualCube[x].Keys)
                {
                    foreach (var z in _virtualCube[x][y].Keys)
                    {
                        SetLEDColorAbsolute(x, y, z, fillColor);
                    }
                }
            }
        }

        public Color GetLEDColor(double x, double y, double z)
        {
            var minX = (int)Math.Floor((x * ResolutionX) - 0.5);
            var maxX = (int)Math.Ceiling((x * ResolutionX) - 0.5);
            var minY = (int)Math.Floor((y * ResolutionY) - 0.5);
            var maxY = (int)Math.Ceiling((y * ResolutionY) - 0.5);
            var minZ = (int)Math.Floor((z * ResolutionZ) - 0.5);
            var maxZ = (int)Math.Ceiling((z * ResolutionZ) - 0.5);

            var colors = new List<ColorPart>();

            foreach (int ix in new[] { minX, maxX }.Distinct())
            {
                foreach (int iy in new[] { minY, maxY }.Distinct())
                {
                    foreach (int iz in new[] { minZ, maxZ }.Distinct())
                    {
                        var dX = 1 - (ResolutionX * Math.Abs((ix / (double)ResolutionX) - x));
                        var dY = 1 - (ResolutionY * Math.Abs((iy / (double)ResolutionY) - y));
                        var dZ = 1 - (ResolutionZ * Math.Abs((iz / (double)ResolutionZ) - z));

                        var d = (float)Math.Sqrt((dX * dX) + (dY * dY) + (dZ * dZ));

                        var colorPart = _virtualCube[ix][iy][iz];

                        colors.Add(new ColorPart()
                        {
                            Color = GetColor(_virtualCube[ix][iy][iz]),
                            Part = d
                        });
                    }
                }
            }

            return GetColor(colors);
        }

        public Color GetLEDColorAbsolute(int x, int y, int z)
        {
            return GetColor(_virtualCube[x][y][z]);
        }

        public void Initialize()
        {
            if (HasBeenInitialized)
            {
                return;
            }

            _virtualCube = new Dictionary<int, Dictionary<int, Dictionary<int, List<ColorPart>>>>();
            for (int ix = -1; ix < ResolutionX + 1; ix++)
            {
                _virtualCube[ix] = new Dictionary<int, Dictionary<int, List<ColorPart>>>();
                for (int iy = -1; iy < ResolutionY + 1; iy++)
                {
                    _virtualCube[ix][iy] = new Dictionary<int, List<ColorPart>>();
                    for (int iz = -1; iz < ResolutionZ + 1; iz++)
                    {
                        _virtualCube[ix][iy][iz] = new List<ColorPart>();
                    }
                }
            }
            HasBeenInitialized = true;
        }

        public void SetLEDColor(double x, double y, double z, Color color)
        {
            var ox = (x * (ResolutionX - 1));
            var oy = (y * (ResolutionY - 1));
            var oz = (z * (ResolutionZ - 1));

            var minX = (int)Math.Floor(ox);
            var maxX = (int)Math.Ceiling(ox);
            var minY = (int)Math.Floor(oy);
            var maxY = (int)Math.Ceiling(oy);
            var minZ = (int)Math.Floor(oz);
            var maxZ = (int)Math.Ceiling(oz);

            foreach (int ix in new[] { minX, maxX }.Distinct())
            {
                if (ix < -1 || ix > ResolutionX)
                {
                    continue;
                }

                foreach (int iy in new[] { minY, maxY }.Distinct())
                {
                    if (iy < -1 || iy > ResolutionY)
                    {
                        continue;
                    }

                    foreach (int iz in new[] { minZ, maxZ }.Distinct())
                    {
                        if (iz < -1 || iz > ResolutionZ)
                        {
                            continue;
                        }

                        var dX = ((ResolutionX - 1) * Math.Abs((ix / (double)(ResolutionX - 1)) - x));
                        var dY = ((ResolutionY - 1) * Math.Abs((iy / (double)(ResolutionY - 1)) - y));
                        var dZ = ((ResolutionZ - 1) * Math.Abs((iz / (double)(ResolutionZ - 1)) - z));

                        var d = 1 - (float)Math.Sqrt((dX * dX) + (dY * dY) + (dZ * dZ));

                        if (d > 1 || d < 0)
                        {
                            continue;
                        }

                        d *= d;

                        lock (_virtualCube[ix][iy][iz])
                        {
                            _virtualCube[ix][iy][iz].Add(new ColorPart()
                            {
                                Color = System.Drawing.Color.FromArgb(
                                    (byte)(d * color.R),
                                    (byte)(d * color.G),
                                    (byte)(d * color.B)),
                                Part = d,
                            });
                        }

                        HasBeenUpdated = true;
                    }
                }
            }
        }

        public void SetLEDColorAbsolute(int x, int y, int z, Color color)
        {
            _virtualCube[x][y][z].Clear();
            _virtualCube[x][y][z].Add(new ColorPart() { Color = color, Part = 1 });
            HasBeenUpdated = true;
        }

        private static System.Drawing.Color GetColor(IEnumerable<ColorPart> colors)
        {
            if (!colors.Any())
            {
                return Color.Empty;
            }

            var totalParts = colors.Sum(p => p.Part);

            return System.Drawing.Color.FromArgb(
                    (byte)(colors.Sum(c => c.Color.R / 255.0 * (c.Part / totalParts)) * 255),
                    (byte)(colors.Sum(c => c.Color.G / 255.0 * (c.Part / totalParts)) * 255),
                    (byte)(colors.Sum(c => c.Color.B / 255.0 * (c.Part / totalParts)) * 255)
                );
        }

        private struct ColorPart
        {
            public System.Drawing.Color Color { get; set; }
            public double Part { get; set; }
        }
    }
}