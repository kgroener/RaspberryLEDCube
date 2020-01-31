using LEDCube.Animations.Animations.Weather.API;
using LEDCube.Animations.Animations.Weather.API.Models;
using LEDCube.Animations.Animations.Weather.ConditionsAnimations.Contracts;
using LEDCube.Animations.Animations.Weather.Sprites;
using LEDCube.Animations.Helpers;
using LEDCube.Animations.Models;
using LEDCube.CanonicalSchema.Contract;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace LEDCube.Animations.Animations.Weather.ConditionsAnimations
{
    public class RainAnimation : IWeatherConditionsAnimation
    {
        private RainIntensity _intensity;
        private TimeSpan _interval;
        private List<AbsoluteLEDCoordinate> _rain;
        private Color _rainColor;
        private TimeSpan _timeSinceLastUpdate;

        public RainAnimation()
        {
        }

        public enum RainIntensity
        {
            Low = 1,
            Normal = 3,
            High = 6
        }

        public bool AutomaticSchedulingAllowed => false;
        public bool IsFinished { get; private set; }

        public bool IsFinite => false;
        public bool IsStopping { get; private set; }
        public TimeSpan PrefferedDuration => TimeSpan.FromSeconds(30);

        public void Cleanup()
        {
            _rain.Clear();
        }

        public void Prepare()
        {
            _rain = new List<AbsoluteLEDCoordinate>();
            _timeSinceLastUpdate = TimeSpan.Zero;
        }

        public void PrepareForWeather(CurrentWeatherResult currentWeather)
        {
            switch (WeatherAPI.GetWeatherCondition(currentWeather.Weather.First()))
            {
                case WeatherAPI.WeatherConditions.Drizzle:
                    _intensity = RainIntensity.Low;
                    _rainColor = Color.FromArgb(0, 20, 20);
                    _interval = TimeSpan.FromSeconds(0.1);
                    break;

                case WeatherAPI.WeatherConditions.Rain:
                    _intensity = RainIntensity.Normal;
                    _rainColor = Color.FromArgb(0, 20, 30);
                    _interval = TimeSpan.FromSeconds(0.1);
                    break;

                case WeatherAPI.WeatherConditions.RainHeavy:
                    _intensity = RainIntensity.High;
                    _rainColor = Color.FromArgb(0, 20, 40);
                    _interval = TimeSpan.FromSeconds(0.1);
                    break;

                case WeatherAPI.WeatherConditions.Snow:
                    _intensity = RainIntensity.Low;
                    _rainColor = Color.FromArgb(20, 20, 20);
                    _interval = TimeSpan.FromSeconds(0.2);
                    break;

                case WeatherAPI.WeatherConditions.SnowHeavy:
                    _intensity = RainIntensity.High;
                    _rainColor = Color.FromArgb(20, 20, 20);
                    _interval = TimeSpan.FromSeconds(0.2);
                    break;

                default:
                    _intensity = RainIntensity.Normal;
                    _rainColor = Color.FromArgb(0, 20, 30);
                    _interval = TimeSpan.FromSeconds(0.1);
                    break;
            }
        }

        public void RequestStop(TimeSpan timeout)
        {
            IsStopping = true;
        }

        public void Update(ILEDCube cube, TimeSpan updateInterval)
        {
            _timeSinceLastUpdate += updateInterval;
            if (_timeSinceLastUpdate > _interval)
            {
                _timeSinceLastUpdate = TimeSpan.Zero;

                cube.Clear();

                cube.DrawSpriteAbsolute(WeatherSprites.GetSprite("Cloud"));

                var n = RandomNumber.GetRandomInteger((int)_intensity / 2, (int)_intensity);
                for (int i = 0; i < n; i++)
                {
                    _rain.Add(new AbsoluteLEDCoordinate()
                    {
                        Color = _rainColor,
                        Coordinate = new AbsoluteCoordinate()
                        {
                            X = RandomNumber.GetRandomInteger(1, cube.ResolutionX - 2),
                            Z = RandomNumber.GetRandomInteger(1, cube.ResolutionZ - 2),
                            Y = 4
                        }
                    });
                }

                foreach (var rainDrop in _rain.ToArray())
                {
                    if (rainDrop.Coordinate.Y >= cube.ResolutionY)
                    {
                        _rain.Remove(rainDrop);
                    }
                    else
                    {
                        cube.SetLEDColorAbsolute(rainDrop.Coordinate.X, rainDrop.Coordinate.Y, rainDrop.Coordinate.Z, rainDrop.Color);
                        rainDrop.Coordinate.Y += 1;
                    }
                }
            }
        }
    }
}