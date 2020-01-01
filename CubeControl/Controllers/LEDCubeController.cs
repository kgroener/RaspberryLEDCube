using LEDCube.CanonicalSchema.Contract;
using RaspberryLEDCube.CanonicalSchema.Enums;
using RaspberryLEDCube.CanonicalSchema.Protocol;
using RaspberryLEDCube.CanonicalSchema.Schema;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaspberryLEDCube.CubeControl.Controllers
{
    public class LEDCubeController : ILEDCubeController
    {
        struct ColorPart
        {
            public System.Drawing.Color Color { get; set; }
            public double Part { get; set; }
        }

        private readonly LEDController _ledController;
        private readonly ProtocolColorBuffer[] _cubeColorBuffers;
        private readonly ProtocolBulkColorBuffer _cubeColorBuffer;
        private readonly Dictionary<int, Dictionary<int, Dictionary<int, List<ColorPart>>>> _virtualCube;

        public int ResolutionX => 8;

        public int ResolutionY => 8;

        public int ResolutionZ => 8;

        public LEDCubeController(LEDController ledController)
        {
            _ledController = ledController;

            var layers = ResolutionZ;
            var ledsPerLayer = ResolutionX*ResolutionY;

            _cubeColorBuffers = Enumerable.Range(0, layers).Select(l => new ProtocolColorBuffer(ledsPerLayer, (byte)l)).ToArray();
            _cubeColorBuffer = new ProtocolBulkColorBuffer(_cubeColorBuffers);

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
        }

        //public async Task InitializeAsync()
        //{
        //    await _psuController.InitializeAsync();
        //    await _ledController.InitializeAsync();
        //}

        //public async Task StartLEDCubeAsync()
        //{
        //    if (_psuController.IsPowerOn())
        //    {
        //        throw new InvalidOperationException("LED cube is already started.");
        //    }

        //    Clear();
        //    _psuController.TurnPowerOn();
        //    await Task.Delay(5);
        //    await DrawAsync();
        //}

        //public void ShutdownLEDCube()
        //{
        //    if (!_psuController.IsPowerOn())
        //    {
        //        throw new InvalidOperationException("LED cube is already shutdown.");
        //    }

        //    _psuController.TurnPowerOff();
        //}

        public void Clear()
        {
            Fill(Color.FromArgb(0, 0, 0));
        }

        public void Fill(Color fillColor)
        {
            foreach(var x in _virtualCube.Keys)
            {
                foreach(var y in _virtualCube[x].Keys)
                {
                    foreach(var z in _virtualCube[x][y].Keys)
                    {
                        SetLEDColorAbsolute(x, y, z, fillColor);
                    }
                }
            }
        }

        public async Task DrawAsync()
        {
            for (byte x = 0; x < ResolutionX; x++)
            {
                for (byte y = 0; y < ResolutionY; y++)
                {
                    for (byte z = 0; z < ResolutionZ; z++)
                    {
                        var color = GetLEDColorAbsolute(x, y, z);
                        int index = GetLEDBufferIndex((byte)x, (byte)y);
                        SetLEDColorInBuffer((byte)z, index, color);
                    }
                }
            }

            await _ledController.WriteBulkColorBufferAsync(_cubeColorBuffer);
        }

        public void SetLEDColor(double x, double y, double z, Color color)
        {
            var minX = (int)Math.Floor((x * ResolutionX) - 0.5);
            var maxX = (int)Math.Ceiling((x * ResolutionX) - 0.5);
            var minY = (int)Math.Floor((y * ResolutionY) - 0.5);
            var maxY = (int)Math.Ceiling((y * ResolutionY) - 0.5);
            var minZ = (int)Math.Floor((z * ResolutionZ) - 0.5);
            var maxZ = (int)Math.Ceiling((z * ResolutionZ) - 0.5);

            foreach (int ix in new[] { minX, x, maxX }.Distinct())
            {
                if (ix < -1 || ix > ResolutionX)
                {
                    continue;
                }

                foreach (int iy in new[] { minY, y, maxY }.Distinct())
                {
                    if (iy < -1 || iy > ResolutionY)
                    {
                        continue;
                    }

                    foreach (int iz in new[] { minZ, z, maxZ }.Distinct())
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
                        var d = (float)Math.Sqrt((dX * dX) + (dY * dY) + (dZ * dZ));

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

        public void SetLEDColorAbsolute(int x, int y, int z, Color color)
        {
            _virtualCube[x][y][z].Clear();
            _virtualCube[x][y][z].Add(new ColorPart() { Color = color, Part = 1 });
        }



        //public Color3 GetLEDColor(byte x, byte y, byte z)
        //{
        //    int index = GetLEDBufferIndex(x, y);
        //    return GetLEDColorFromBuffer(z, index);
        //}

        private void SetLEDColorInBuffer(byte layer, int index, Color color)
        {
            _cubeColorBuffers[layer][index].Red = color.R;
            _cubeColorBuffers[layer][index].Green = color.G;
            _cubeColorBuffers[layer][index].Blue = color.B;
        }

        //private Color3 GetLEDColorFromBuffer(byte layer, int index)
        //{
        //    Color3 color = _cubeColorBuffers[layer][index];
        //    return new Color3(color.Red, color.Green, color.Blue);
        //}

        private int GetLEDBufferIndex(byte x, byte y)
        {
            int index;

            var isYEven = (y % 2) == 0;

            if (isYEven)
            {
                index = x + (y * ResolutionX);
            }
            else
            {
                index = ResolutionX - x - 1 + (y * ResolutionX);
            }

            return index;
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

        private static System.Drawing.Color GetColor(IEnumerable<ColorPart> colors)
        {
            lock (colors)
            {
                if (!colors.Any())
                {
                    return System.Drawing.Color.FromArgb(
                        (byte)0,
                        (byte)0,
                        (byte)0
                    );
                }

                var totalParts = colors.Sum(p => p.Part);

                return System.Drawing.Color.FromArgb(
                        (byte)(colors.Sum(c => c.Color.R / 255.0 * (c.Part / totalParts)) * 255),
                        (byte)(colors.Sum(c => c.Color.G / 255.0 * (c.Part / totalParts)) * 255),
                        (byte)(colors.Sum(c => c.Color.B / 255.0 * (c.Part / totalParts)) * 255)
                    );
            }
        }

        public Color GetLEDColorAbsolute(int x, int y, int z)
        {
            return GetColor(_virtualCube[x][y][z]);
        }
    }
}
