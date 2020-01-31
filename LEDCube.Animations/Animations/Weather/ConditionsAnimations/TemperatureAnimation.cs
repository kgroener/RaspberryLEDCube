using LEDCube.Animations.Animations.Text.Abstracts;
using LEDCube.Animations.Animations.Text.Data;
using LEDCube.Animations.Animations.Weather.API.Models;
using LEDCube.Animations.Animations.Weather.ConditionsAnimations.Contracts;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace LEDCube.Animations.Animations.Weather.ConditionsAnimations
{
    public class TemperatureAnimation : ScrollingTextAnimation, IWeatherConditionsAnimation
    {
        private string _temperatureText;

        public override bool AutomaticSchedulingAllowed => false;

        public override TimeSpan PrefferedDuration => TimeSpan.FromSeconds(20);

        protected override bool Repeat => true;

        public void PrepareForWeather(CurrentWeatherResult currentWeather)
        {
            _temperatureText = $"{currentWeather.Main.Temp - 273.15:G1}°C        {currentWeather.Weather.First().Description}        ";
        }

        protected override PixelizedString GetText()
        {
            return new PixelizedString(_temperatureText, Color.FromArgb(0, 20, 0));
        }
    }
}