using LEDCube.Animations.Animations.Text.Data;
using LEDCube.Animations.Helpers;
using LEDCube.CanonicalSchema.Contract;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LEDCube.Animations.Animations.Text.Abstracts
{
    public abstract class ScrollingTextAnimation : ILEDCubeAnimation
    {
        private readonly IEnumerable<(int x, int z)> _scrollPath = new[]
        {
            (0, 0),
            (1, 0),
            (2, 0),
            (3, 0),
            (4, 0),
            (5, 0),
            (6, 0),
            (7, 0),
            (7, 1),
            (7, 2),
            (7, 3),
            (7, 4),
            (7, 5),
            (7, 6),
            (7, 7),
            (6, 7),
            (5, 7),
            (4, 7),
            (3, 7),
            (2, 7),
            (1, 7),
            (0, 7),
            (0, 6),
            (0, 5),
            (0, 4),
            (0, 3),
            (0, 2),
            (0, 1),
        };

        private int _index;
        private PixelizedString _text;
        private TimeSpan _timeSinceLastUpdate;

        public abstract bool AutomaticSchedulingAllowed { get; }
        public bool IsFinished { get; private set; }

        public bool IsFinite => !Repeat;
        public bool IsStopping { get; private set; }
        public abstract TimeSpan PrefferedDuration { get; }

        protected abstract bool Repeat { get; }

        public void Cleanup()
        {
            IsStopping = false;
        }

        public void Prepare()
        {
            _text = GetText();

            _index = 0;
            _timeSinceLastUpdate = TimeSpan.Zero;
            IsFinished = false;
        }

        public void RequestStop(TimeSpan timeout)
        {
            IsStopping = true;
        }

        public void Update(ILEDCube cube, TimeSpan updateInterval)
        {
            _timeSinceLastUpdate += updateInterval;

            if (_timeSinceLastUpdate.TotalSeconds > 0.1)
            {
                _timeSinceLastUpdate = TimeSpan.Zero;

                cube.Clear();

                var leds =
                    Enumerable.Repeat(PixelizedCharacter.EmptyColumn, _scrollPath.Count())
                    .Concat(_text.Columns)
                    .Skip(_index)
                    .Zip(_scrollPath, (column, coordinate) => new { X = coordinate.x, Z = coordinate.z, Column = column });

                if (!leds.Any())
                {
                    if (!Repeat)
                    {
                        IsFinished = true;
                    }
                    else
                    {
                        _index = 0;
                    }
                }

                foreach (var led in leds)
                {
                    foreach (var coordinate in led.Column.Pixels.Select((v, i) => new { Y = i, Value = v }).Where(v => v.Value))
                    {
                        cube.SetLEDColorAbsolute(led.X, coordinate.Y, led.Z, led.Column.Color);
                    }
                }

                _index++;
            }
        }

        protected abstract PixelizedString GetText();
    }
}