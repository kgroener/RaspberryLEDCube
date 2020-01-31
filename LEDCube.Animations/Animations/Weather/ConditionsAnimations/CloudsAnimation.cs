using LEDCube.Animations.Animations.Weather.API;
using LEDCube.Animations.Animations.Weather.API.Models;
using LEDCube.Animations.Animations.Weather.ConditionsAnimations.Contracts;
using LEDCube.Animations.Animations.Weather.Sprites;
using LEDCube.Animations.Helpers;
using LEDCube.Animations.Models;
using LEDCube.CanonicalSchema.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LEDCube.Animations.Animations.Weather.ConditionsAnimations
{
    internal class CloudsAnimation : IWeatherConditionsAnimation
    {
        private readonly List<Coordinate> _clouds;
        private double _scale;
        private Coordinate _speed;

        public CloudsAnimation()
        {
            _clouds = new List<Coordinate>();
        }

        public bool AutomaticSchedulingAllowed => false;
        public bool IsFinished { get; private set; }

        public bool IsFinite => false;
        public bool IsStopping { get; private set; }
        public TimeSpan PrefferedDuration => TimeSpan.FromSeconds(60);

        public void Cleanup()
        {
            _clouds.Clear();
        }

        public void Prepare()
        {
        }

        public void PrepareForWeather(CurrentWeatherResult currentWeather)
        {
            _speed = new Coordinate(currentWeather.Wind.Speed / 100, 0, 0);
            var condition = WeatherAPI.GetWeatherCondition(currentWeather.Weather.First());

            //switch (condition)
            //{
            //    case WeatherAPI.WeatherConditions.CloudsFew:
            //        _scale = 0.5;
            //        _clouds.Add(new Coordinate(0, RandomNumber.GetRandomNumber(0.25, 0.5), RandomNumber.GetRandomNumber(0.1, 0.5)));
            //        _clouds.Add(new Coordinate(1, RandomNumber.GetRandomNumber(0.25, 0.5), RandomNumber.GetRandomNumber(0.1, 0.5)));
            //        break;

            //    default:
            _scale = 1;
            _clouds.Add(new Coordinate(0, RandomNumber.GetRandomNumber(0.25, 0.3), RandomNumber.GetRandomNumber(0.1, 0.4)));
            _clouds.Add(new Coordinate(1, RandomNumber.GetRandomNumber(0.25, 0.3), RandomNumber.GetRandomNumber(0.1, 0.4)));
            //break;
            //}
        }

        public void RequestStop(TimeSpan timeout)
        {
            IsStopping = true;
        }

        public void Update(ILEDCube cube, TimeSpan updateInterval)
        {
            cube.Clear();

            //cube.DrawSpriteAbsolute(WeatherSprites.GetSprite("Cloud"), new AbsoluteCoordinate(0, 2, 0));

            foreach (var cloud in _clouds)
            {
                if (cloud.X < -0.5)
                {
                    cloud.X = 1.5;
                }
                if (cloud.X > 1.5)
                {
                    cloud.X = -0.5;
                }

                if (cloud.Y < -0.5)
                {
                    cloud.Y = 1.5;
                }
                if (cloud.Y > 1.5)
                {
                    cloud.Y = -0.5;
                }

                if (cloud.Z < -0.5)
                {
                    cloud.Z = 1.5;
                }
                if (cloud.Z > 1.5)
                {
                    cloud.Z = -0.5;
                }

                cube.DrawSprite(WeatherSprites.GetSprite("Cloud"), _scale, cloud, new Mathematics.RotationMatrix(0, 0, 0));

                cloud.X += _speed.X * updateInterval.TotalSeconds;
                cloud.Y += _speed.Y * updateInterval.TotalSeconds;
                cloud.Z += _speed.Z * updateInterval.TotalSeconds;
            }
        }
    }
}