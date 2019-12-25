using LEDCube.CanonicalSchema.Contract;
using LEDCube.Simulator.WPF.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LEDCube.Simulator.WPF.Cube
{
    struct ColorPart
    {
        public System.Drawing.Color Color { get; set; }
        public double Part { get; set; }
    }

    class LEDCubeController : ILEDCubeController
    {

        private const float LED_TRANSPARENCY = 0.4f;
        private readonly LEDCubeGeometryGroup _cube;
        private readonly Dictionary<int, Dictionary<int, Dictionary<int, List<ColorPart>>>> _virtualCube;
        private readonly SemaphoreSlim _drawingLock;

        public LEDCubeController(LEDCubeGeometryGroup ledCube)
        {
            _cube = ledCube;
            _virtualCube = new Dictionary<int, Dictionary<int, Dictionary<int, List<ColorPart>>>>();
            for (int ix = -1; ix < _cube.ResolutionX + 1; ix++)
            {
                _virtualCube[ix] = new Dictionary<int, Dictionary<int, List<ColorPart>>>();
                for (int iy = -1; iy < _cube.ResolutionY + 1; iy++)
                {
                    _virtualCube[ix][iy] = new Dictionary<int, List<ColorPart>>();
                    for (int iz = -1; iz < _cube.ResolutionZ + 1; iz++)
                    {
                        _virtualCube[ix][iy][iz] = new List<ColorPart>();
                    }
                }
            }

            _drawingLock = new SemaphoreSlim(1, 1);
        }

        public int ResolutionX => _cube.ResolutionX;

        public int ResolutionY => _cube.ResolutionY;

        public int ResolutionZ => _cube.ResolutionZ;

        public void Clear()
        {
            foreach (var x in _virtualCube.Keys)
            {
                foreach (var y in _virtualCube[x].Keys)
                {
                    foreach (var z in _virtualCube[x][y].Keys)
                    {
                        lock (_virtualCube[x][y][z])
                        {
                            _virtualCube[x][y][z].Clear();
                        }
                    }
                }
            }
        }

        public async Task DrawAsync()
        {
            await _drawingLock.WaitAsync();
            var _ = Task.Run(() =>
            {
                try
                {
                    for (int x = 0; x < ResolutionX; x++)
                    {
                        for (int y = 0; y < ResolutionY; y++)
                        {
                            for (int z = 0; z < ResolutionZ; z++)
                            {
                                var color = GetColor(_virtualCube[x][y][z]);
                                UIHelper.UISafeInvoke(() =>
                                {
                                    _cube.GetLEDAt(x, y, z).LEDColor = new SolidColorBrush(new System.Windows.Media.Color()
                                    {
                                        A = color.A,
                                        R = color.R,
                                        G = color.G,
                                        B = color.B
                                    });
                                });
                            }
                        }
                    }
                }
                finally
                {
                    _drawingLock.Release();
                }
            });
        }

        public System.Drawing.Color GetLEDColorAbsolute(int x, int y, int z)
        {
            return GetColor(_virtualCube[x][y][z]);
        }

        public System.Drawing.Color GetLEDColor(double x, double y, double z)
        {
            var minX = (int)Math.Floor(x * ResolutionX - 0.5);
            var maxX = (int)Math.Ceiling(x * ResolutionX - 0.5);
            var minY = (int)Math.Floor(y * ResolutionY - 0.5);
            var maxY = (int)Math.Ceiling(y * ResolutionY - 0.5);
            var minZ = (int)Math.Floor(z * ResolutionZ - 0.5);
            var maxZ = (int)Math.Ceiling(z * ResolutionZ - 0.5);

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

                        var d = (float)Math.Sqrt(dX * dX + dY * dY + dZ * dZ);

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

        private static System.Drawing.Color GetColor(IEnumerable<ColorPart> colors)
        {
            lock (colors)
            {
                if (!colors.Any())
                {
                    return System.Drawing.Color.FromArgb(

                        (byte)(255 * LED_TRANSPARENCY),
                        (byte)0,
                        (byte)0,
                        (byte)0
                    );
                }

                var totalParts = colors.Sum(p => p.Part);

                return System.Drawing.Color.FromArgb(

                        (byte)(255 * LED_TRANSPARENCY),
                        (byte)(colors.Sum(c => c.Color.R / 255.0 * (c.Part / totalParts)) * 255),
                        (byte)(colors.Sum(c => c.Color.G / 255.0 * (c.Part / totalParts)) * 255),
                        (byte)(colors.Sum(c => c.Color.B / 255.0 * (c.Part / totalParts)) * 255)
                    );
            }
        }

        public void SetLEDColorAbsolute(int x, int y, int z, System.Drawing.Color color)
        {
            lock (_virtualCube[x][y][z])
            {
                color = System.Drawing.Color.FromArgb((byte)(255 * LED_TRANSPARENCY), color);
                _virtualCube[x][y][z].Clear();
                _virtualCube[x][y][z].Add(new ColorPart() { Color = color, Part = 1 });
            }
        }

        public void SetLEDColor(double x, double y, double z, System.Drawing.Color color)
        {
            color = System.Drawing.Color.FromArgb((byte)(255 * LED_TRANSPARENCY), color);

            var minX = (int)Math.Floor(x * ResolutionX - 0.5);
            var maxX = (int)Math.Ceiling(x * ResolutionX - 0.5);
            var minY = (int)Math.Floor(y * ResolutionY - 0.5);
            var maxY = (int)Math.Ceiling(y * ResolutionY - 0.5);
            var minZ = (int)Math.Floor(z * ResolutionZ - 0.5);
            var maxZ = (int)Math.Ceiling(z * ResolutionZ - 0.5);

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

                        var dX = (ResolutionX * Math.Abs((ix / (double)ResolutionX) - x));
                        var dY = (ResolutionY * Math.Abs((iy / (double)ResolutionY) - y));
                        var dZ = (ResolutionZ * Math.Abs((iz / (double)ResolutionZ) - z));

                        if (ix < x * ResolutionX)
                        {
                            dX = 1 - dX;
                        }
                        if (iy < y * ResolutionY)
                        {
                            dY = 1 - dY;
                        }
                        if (iz < z * ResolutionZ)
                        {
                            dZ = 1 - dZ;
                        }
                        var d = (float)Math.Sqrt(dX * dX + dY * dY + dZ * dZ);

                        if (d > 1 || d < 0)
                        {
                            continue;
                        }

                        d = 1 - d;

                        lock (_virtualCube[ix][iy][iz])
                        {
                            _virtualCube[ix][iy][iz].Add(new ColorPart()
                            {
                                Color = System.Drawing.Color.FromArgb(
                                    (byte)(LED_TRANSPARENCY * 255),
                                    (byte)(d * color.R),
                                    (byte)(d * color.G),
                                    (byte)(d * color.B)),
                                Part = d,
                            });
                        }
                    }
                }
            }
        }
    }
}
