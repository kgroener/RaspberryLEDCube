using LEDCube.CanonicalSchema.Contract;
using System;
using System.Diagnostics;
using System.Drawing;

namespace LEDCube.Animations.Animations
{
    public class GameOfLifeAnimation : ILEDCubeAnimation
    {
        private const int UNDERPOPULATION_THRESHOLD = 3;
        private const int OVERPOPULATION_THRESHOLD = 5;
        private const int REPRODUCTION_THRESHOLD = 4;
        private TimeSpan? _timeRemaining;
        private bool _isRunning;
        private TimeSpan _updateFrequency;
        private TimeSpan _timeSinceLastUpdate;
        private TimeSpan _fadeSpeed;

        public bool IsFinished => !_isRunning;

        public bool IsStopping { get; private set; }

        public bool IsFinite => false;

        public TimeSpan PrefferedDuration => TimeSpan.FromSeconds(30);

        public bool AutomaticSchedulingAllowed => true;

        public void Cleanup()
        {
            _isRunning = false;
        }

        public void Prepare()
        {
            _updateFrequency = TimeSpan.FromSeconds(0.1);
            _timeSinceLastUpdate = TimeSpan.FromSeconds(0);
            _isRunning = true;
            _timeRemaining = null;
            _fadeSpeed = TimeSpan.FromSeconds(0.5);
        }

        public void RequestStop(TimeSpan timeout)
        {
            _timeRemaining = timeout;
            IsStopping = true;
        }

        public void Update(ILEDCubeController cube, TimeSpan updateInterval)
        {
            if (_timeRemaining.HasValue)
            {
                _timeRemaining -= _timeSinceLastUpdate;
            }

            _timeSinceLastUpdate += updateInterval;
            bool shouldUpdate = _timeSinceLastUpdate > _updateFrequency;

            if (!shouldUpdate)
            {
                return;
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
                                var t = Math.Ceiling(_timeRemaining.Value.TotalMilliseconds / _fadeSpeed.TotalMilliseconds) - 1;
                                var r = RandomNumber.GetRandomNumber((int)t);
                                var canReproduce = r == 0;

                                reproductionsFailed++;
                                aliveLEDS[x, y, z] = canReproduce;
                            }
                            else
                            {
                                aliveLEDS[x, y, z] = true;
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
                            cube.SetLEDColorAbsolute(x, y, z, Color.FromArgb(byte.MaxValue, 0, 0));
                        }
                        else
                        {
                            //cube.SetLEDColorAbsolute(x, y, z, Color.FromArgb(0, 0, 0, 0));
                            var currentColor = cube.GetLEDColorAbsolute(x, y, z);
                            if (currentColor.R > 0)
                            {
                                anyDying = true;
                                var r = (currentColor.R - (byte.MaxValue * (_timeSinceLastUpdate.TotalSeconds / _fadeSpeed.TotalSeconds)));
                                if (r < 0)
                                {
                                    r = 0;
                                }

                                if (r > byte.MaxValue)
                                {
                                    r = byte.MaxValue;
                                }

                                cube.SetLEDColorAbsolute(x, y, z, Color.FromArgb(0, (byte)r, (byte)r/2, currentColor.B));
                            }
                        }
                    }
                }
            }

            if (!IsStopping)
            {
                var repopulationCount = numberOfLEDSAlive == 0 ? 20 : 2;

                var ox = RandomNumber.GetRandomNumber(2, cube.ResolutionX - 3);
                var oy = RandomNumber.GetRandomNumber(2, cube.ResolutionY - 3);
                var oz = RandomNumber.GetRandomNumber(2, cube.ResolutionZ - 3);

                for (int i = 0; i < RandomNumber.GetRandomNumber(1, repopulationCount); i++)
                {
                    var x = ox + RandomNumber.GetRandomNumber(-1, 1);
                    var y = oy + RandomNumber.GetRandomNumber(-1, 1);
                    var z = oz + RandomNumber.GetRandomNumber(-1, 1);

                    cube.SetLEDColorAbsolute(x, y, z, Color.FromArgb(byte.MaxValue, 0, 0));
                }
            }
            else
            {
                if (numberOfLEDSAlive == 0 && !anyDying)
                {
                    _isRunning = false;
                }
            }

            _timeSinceLastUpdate = TimeSpan.FromSeconds(0);
        }

        private bool IsLEDAlive(ILEDCubeController cube, int x, int y, int z)
        {
            var color = cube.GetLEDColorAbsolute(x, y, z);
            return (color.R == byte.MaxValue);
        }

        private int GetAliveNeighbourCount(ILEDCubeController cube, int x, int y, int z)
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
    }
}
