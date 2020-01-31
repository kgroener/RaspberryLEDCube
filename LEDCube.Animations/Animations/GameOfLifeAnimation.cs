using LEDCube.Animations.Helpers;
using LEDCube.CanonicalSchema.Contract;
using System;
using System.Diagnostics;
using System.Drawing;

namespace LEDCube.Animations.Animations
{
    public class GameOfLifeAnimation : ILEDCubeAnimation
    {
        private const int OVERPOPULATION_THRESHOLD = 5;
        private const int REPRODUCTION_THRESHOLD = 4;
        private const int UNDERPOPULATION_THRESHOLD = 3;
        private Color _color;
        private Color _deathColor;
        private TimeSpan _fadeSpeed;
        private bool _hasBeenCleared;
        private bool _isRunning;
        private TimeSpan? _timeRemaining;
        private TimeSpan _timeSinceLastUpdate;
        private TimeSpan _updateFrequency;
        public bool AutomaticSchedulingAllowed => true;
        public bool IsFinished => !_isRunning;

        public bool IsFinite => false;
        public bool IsStopping { get; private set; }
        public TimeSpan PrefferedDuration => TimeSpan.FromSeconds(30);

        public void Cleanup()
        {
            _isRunning = false;
            IsStopping = false;
        }

        public void Prepare()
        {
            _updateFrequency = TimeSpan.FromSeconds(0.10);
            _timeSinceLastUpdate = TimeSpan.FromSeconds(0);
            _isRunning = true;
            _timeRemaining = null;
            _hasBeenCleared = false;
            _fadeSpeed = TimeSpan.FromSeconds(0.5);
            _color = RandomNumber.GetRandomItem(new[]
            {
                Color.FromArgb(255,0,0),
                Color.FromArgb(0,255,0),
                Color.FromArgb(0,0,255),
                Color.FromArgb(127,127,0),
                Color.FromArgb(0,127,127),
                Color.FromArgb(127,0,127),
            });

            var deathColorDelta = RandomNumber.GetRandomItem(new[]
            {
                Color.FromArgb(80,80,0),
                Color.FromArgb(0,80,80),
                Color.FromArgb(80,0,80),
            });
            _deathColor = Color.FromArgb(
                (_color.R + deathColorDelta.R).Clip(0, 255),
                (_color.G + deathColorDelta.G).Clip(0, 255),
                (_color.B + deathColorDelta.B).Clip(0, 255));
        }

        public void RequestStop(TimeSpan timeout)
        {
            _timeRemaining = timeout;
            IsStopping = true;
        }

        public void Update(ILEDCube cube, TimeSpan updateInterval)
        {
            if (!_hasBeenCleared)
            {
                cube.Clear();
                _hasBeenCleared = true;
            }

            if (_timeRemaining.HasValue)
            {
                _timeRemaining -= updateInterval;
            }

            _timeSinceLastUpdate += updateInterval;
            bool shouldUpdate = _timeSinceLastUpdate > _updateFrequency;
            if (shouldUpdate)
            {
                _timeSinceLastUpdate = TimeSpan.FromSeconds(0);
            }

            int numberOfLEDSAlive = 0;

            var aliveLEDS = new bool[cube.ResolutionX, cube.ResolutionY, cube.ResolutionZ];

            int reproductionsFailed = 0;

            for (int x = 0; x < cube.ResolutionX; x++)
            {
                for (int y = 0; y < cube.ResolutionY; y++)
                {
                    for (int z = 0; z < cube.ResolutionZ; z++)
                    {
                        aliveLEDS[x, y, z] = IsLEDAlive(cube, x, y, z);

                        var neighbourCount = GetAliveNeighbourCount(cube, x, y, z);

                        if (shouldUpdate)
                        {
                            if (aliveLEDS[x, y, z])
                            {
                                if (neighbourCount <= UNDERPOPULATION_THRESHOLD
                                    || neighbourCount >= OVERPOPULATION_THRESHOLD)
                                {
                                    aliveLEDS[x, y, z] = false;
                                }
                            }
                            else if (neighbourCount == REPRODUCTION_THRESHOLD)
                            {
                                if (_timeRemaining.HasValue && (reproductionsFailed < 3 || _timeRemaining <= _fadeSpeed))
                                {
                                    var t = Math.Max(0, Math.Ceiling(_timeRemaining.Value.TotalMilliseconds / _fadeSpeed.TotalMilliseconds) - 1);
                                    var r = RandomNumber.GetRandomInteger((int)t);
                                    var canReproduce = r == 0;

                                    reproductionsFailed++;
                                    aliveLEDS[x, y, z] = canReproduce;
                                }
                                else
                                {
                                    aliveLEDS[x, y, z] = true;
                                }
                            }
                        }

                        if (aliveLEDS[x, y, z])
                        {
                            numberOfLEDSAlive++;
                        }
                    }
                }
            }

            bool anyDying = false;
            for (int x = 0; x < cube.ResolutionX; x++)
            {
                for (int y = 0; y < cube.ResolutionY; y++)
                {
                    for (int z = 0; z < cube.ResolutionZ; z++)
                    {
                        bool alive = aliveLEDS[x, y, z];

                        if (alive)
                        {
                            cube.SetLEDColorAbsolute(x, y, z, _color);
                        }
                        else
                        {
                            //cube.SetLEDColorAbsolute(x, y, z, Color.FromArgb(0, 0, 0, 0));
                            var currentColor = cube.GetLEDColorAbsolute(x, y, z);
                            if (currentColor == _color)
                            {
                                currentColor = _deathColor;
                            }

                            if (currentColor.R > 0 || currentColor.G > 0 || currentColor.B > 0)
                            {
                                anyDying = true;

                                var delta = (updateInterval.TotalSeconds / _fadeSpeed.TotalSeconds);

                                var dimColor = ColorHelper.DimColor(_deathColor, currentColor, delta);
                                cube.SetLEDColorAbsolute(x, y, z, dimColor);
                            }
                        }
                    }
                }
            }

            if (!IsStopping)
            {
                var repopulationCount = numberOfLEDSAlive == 0 ? 20 : 2;

                var ox = RandomNumber.GetRandomInteger(2, cube.ResolutionX - 3);
                var oy = RandomNumber.GetRandomInteger(2, cube.ResolutionY - 3);
                var oz = RandomNumber.GetRandomInteger(2, cube.ResolutionZ - 3);

                for (int i = 0; i < RandomNumber.GetRandomInteger(1, repopulationCount); i++)
                {
                    var x = ox + RandomNumber.GetRandomInteger(-1, 1);
                    var y = oy + RandomNumber.GetRandomInteger(-1, 1);
                    var z = oz + RandomNumber.GetRandomInteger(-1, 1);

                    cube.SetLEDColorAbsolute(x, y, z, _color);
                }
            }
            else
            {
                if (numberOfLEDSAlive == 0 && !anyDying)
                {
                    _isRunning = false;
                }
            }
        }

        private int GetAliveNeighbourCount(ILEDCube cube, int x, int y, int z)
        {
            int count = 0;

            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    for (int dz = -1; dz <= 1; dz++)
                    {
                        if ((dx == 0 && dy == 0 && dz == 0)
                         || (x + dx < 0) || (x + dx > cube.ResolutionX)
                         || (y + dy < 0) || (y + dy > cube.ResolutionY)
                         || (z + dz < 0) || (z + dz > cube.ResolutionZ))
                        {
                            continue;
                        }

                        if (IsLEDAlive(cube, x + dx, y + dy, z + dz))
                        {
                            count++;
                        }
                    }
                }
            }

            return count;
        }

        private bool IsLEDAlive(ILEDCube cube, int x, int y, int z)
        {
            var color = cube.GetLEDColorAbsolute(x, y, z);
            return (color == _color);
        }
    }
}