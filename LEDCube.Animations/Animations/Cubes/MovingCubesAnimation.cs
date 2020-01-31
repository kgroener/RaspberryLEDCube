using LEDCube.Animations.Helpers;
using LEDCube.Animations.Models;
using LEDCube.CanonicalSchema.Contract;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace LEDCube.Animations.Animations.Cubes
{
    public class MovingCubesAnimation : ILEDCubeAnimation
    {
        private readonly List<Cube> _cubes;

        private TimeSpan _timeSinceLastUpdate;
        private TimeSpan _updateFrequency;

        public MovingCubesAnimation()
        {
            _cubes = new List<Cube>();
        }

        public bool AutomaticSchedulingAllowed => true;

        public bool IsFinished { get; private set; }

        public bool IsFinite => false;

        public bool IsStopping { get; private set; }

        public TimeSpan PrefferedDuration => TimeSpan.FromSeconds(30);

        public void Cleanup()
        {
            IsFinished = true;
            _cubes.Clear();
        }

        public void Prepare()
        {
            IsFinished = false;
            _updateFrequency = TimeSpan.FromSeconds(1);
            _timeSinceLastUpdate = TimeSpan.FromSeconds(0);

            var colors = new[] { Color.Red, Color.Blue, Color.Green };

            int numberOfCubes = RandomNumber.GetRandomInteger(3, 10);

            for (int i = 0; i < numberOfCubes; i++)
            {
                AbsoluteCoordinate coordinate;
                do
                {
                    coordinate = new AbsoluteCoordinate()
                    {
                        X = RandomNumber.GetRandomInteger(3) * 2,
                        Y = RandomNumber.GetRandomInteger(3) * 2,
                        Z = RandomNumber.GetRandomInteger(3) * 2,
                    };
                } while (_cubes.Any(c => c.CurrentLocation == coordinate));

                _cubes.Add(new Cube(coordinate)
                {
                    Color = RandomNumber.GetRandomItem(colors),
                });
            }

            UpdateCubeLocations();
        }

        public void RequestStop(TimeSpan timeout)
        {
            IsStopping = true;
        }

        public void Update(ILEDCube ledCube, TimeSpan updateInterval)
        {
            _timeSinceLastUpdate += updateInterval;
            bool shouldUpdate = _timeSinceLastUpdate > _updateFrequency;
            if (shouldUpdate)
            {
                _timeSinceLastUpdate = TimeSpan.FromSeconds(0);
            }

            double distanceFraction = 0;

            if (shouldUpdate)
            {
                UpdateCubeLocations();
            }
            else
            {
                distanceFraction = _timeSinceLastUpdate.TotalSeconds / _updateFrequency.TotalSeconds;
            }

            ledCube.Clear();
            foreach (var cube in _cubes)
            {
                if (cube.CurrentLocation.Equals(cube.DesiredLocation))
                {
                    cube.Color = ColorHelper.ShiftHue(cube.Color, updateInterval.TotalSeconds * 60);
                }

                DrawCube(ledCube, cube, distanceFraction);
            }
        }

        private void DrawCube(ILEDCube ledCube, Cube cube, double fraction)
        {
            var ox = cube.CurrentLocation.X + ((cube.DesiredLocation.X - cube.CurrentLocation.X) * fraction);
            var oy = cube.CurrentLocation.Y + ((cube.DesiredLocation.Y - cube.CurrentLocation.Y) * fraction);
            var oz = cube.CurrentLocation.Z + ((cube.DesiredLocation.Z - cube.CurrentLocation.Z) * fraction);

            for (int nx = 0; nx < 2; nx++)
            {
                for (int ny = 0; ny < 2; ny++)
                {
                    for (int nz = 0; nz < 2; nz++)
                    {
                        ledCube.SetLEDColor(
                            (ox + nx) / (ledCube.ResolutionX - 1),
                            (oy + ny) / (ledCube.ResolutionY - 1),
                            (oz + nz) / (ledCube.ResolutionZ - 1),
                            cube.Color);
                    }
                }
            }
        }

        private IEnumerable<AbsoluteCoordinate> GetPossibleLocations(Cube cube)
        {
            var currentLocation = cube.CurrentLocation;

            if (cube.CurrentLocation.X >= 2
                && !_cubes.Any(c => IsOccupyingLocation(c, currentLocation.X - 2, currentLocation.Y, currentLocation.Z)))
            {
                yield return new AbsoluteCoordinate(currentLocation.X - 2, currentLocation.Y, currentLocation.Z);
            }
            if (cube.CurrentLocation.Y >= 2
                && !_cubes.Any(c => IsOccupyingLocation(c, currentLocation.X, currentLocation.Y - 2, currentLocation.Z)))
            {
                yield return new AbsoluteCoordinate(currentLocation.X, currentLocation.Y - 2, currentLocation.Z);
            }
            if (cube.CurrentLocation.Z >= 2
                && !_cubes.Any(c => IsOccupyingLocation(c, currentLocation.X, currentLocation.Y, currentLocation.Z - 2)))
            {
                yield return new AbsoluteCoordinate(currentLocation.X, currentLocation.Y, currentLocation.Z - 2);
            }

            if (cube.CurrentLocation.X <= 4
                && !_cubes.Any(c => IsOccupyingLocation(c, currentLocation.X + 2, currentLocation.Y, currentLocation.Z)))
            {
                yield return new AbsoluteCoordinate(currentLocation.X + 2, currentLocation.Y, currentLocation.Z);
            }
            if (cube.CurrentLocation.Y <= 4
                && !_cubes.Any(c => IsOccupyingLocation(c, currentLocation.X, currentLocation.Y + 2, currentLocation.Z)))
            {
                yield return new AbsoluteCoordinate(currentLocation.X, currentLocation.Y + 2, currentLocation.Z);
            }
            if (cube.CurrentLocation.Z <= 4
                && !_cubes.Any(c => IsOccupyingLocation(c, currentLocation.X, currentLocation.Y, currentLocation.Z + 2)))
            {
                yield return new AbsoluteCoordinate(currentLocation.X, currentLocation.Y, currentLocation.Z + 2);
            }
        }

        private bool IsOccupyingLocation(Cube cube, int x, int y, int z)
        {
            return cube.CurrentLocation.Equals(x, y, z) || cube.DesiredLocation.Equals(x, y, z);
        }

        private void UpdateCubeLocations()
        {
            foreach (var cube in _cubes)
            {
                cube.CurrentLocation = cube.DesiredLocation;
                var possibleNextLocations = GetPossibleLocations(cube).ToArray();
                if (possibleNextLocations.Any())
                {
                    cube.DesiredLocation = RandomNumber.GetRandomItem(possibleNextLocations);
                }
            }
        }

        private class Cube
        {
            public Cube(AbsoluteCoordinate coordinate)
            {
                CurrentLocation = coordinate;
                DesiredLocation = coordinate;
            }

            public Color Color { get; set; }
            public AbsoluteCoordinate CurrentLocation { get; set; }
            public AbsoluteCoordinate DesiredLocation { get; set; }
        }
    }
}