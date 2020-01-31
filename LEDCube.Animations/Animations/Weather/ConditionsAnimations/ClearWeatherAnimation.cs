using LEDCube.Animations.Animations.Weather.API.Models;
using LEDCube.Animations.Animations.Weather.ConditionsAnimations.Contracts;
using LEDCube.Animations.Helpers;
using LEDCube.CanonicalSchema.Contract;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace LEDCube.Animations.Animations.Weather.ConditionsAnimations
{
    internal class ClearWeatherAnimation : IWeatherConditionsAnimation
    {
        private bool _isNightTime;

        public bool AutomaticSchedulingAllowed => false;
        public bool IsFinished { get; private set; }

        public bool IsFinite => false;
        public bool IsStopping { get; private set; }
        public TimeSpan PrefferedDuration => TimeSpan.FromSeconds(30);

        public void Cleanup()
        {
        }

        public void Prepare()
        {
        }

        public void PrepareForWeather(CurrentWeatherResult currentWeather)
        {
            _isNightTime = (DateTime.Now > currentWeather.Sys.SunsetDate) || (DateTime.Now < currentWeather.Sys.SunriseDate);
        }

        public void RequestStop(TimeSpan timeout)
        {
            IsStopping = true;
        }

        public void Update(ILEDCube cube, TimeSpan updateInterval)
        {
            cube.Clear();

            if (_isNightTime)
            {
                cube.DrawSphere(new Models.Coordinate(0.4, 0.4, 0.2), 0.25, Color.FromArgb(100, 80, 50));
            }
            else
            {
                cube.DrawSphere(new Models.Coordinate(0.4, 0.4, 0.2), 0.25, Color.FromArgb(100, 60, 0));
                //TODO draw extra lines
            }
        }
    }
}