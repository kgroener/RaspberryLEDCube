using LEDCube.Animations.Animations.Abstracts;
using LEDCube.Animations.Animations.Text;
using LEDCube.Animations.Animations.Text.Data;
using LEDCube.Animations.Animations.Weather.API;
using LEDCube.Animations.Animations.Weather.API.Models;
using LEDCube.Animations.Animations.Weather.ConditionsAnimations;
using LEDCube.Animations.Animations.Weather.ConditionsAnimations.Contracts;
using LEDCube.Animations.Animations.Weather.Sprites;
using LEDCube.Animations.Helpers;
using LEDCube.Animations.Models;
using LEDCube.CanonicalSchema.Contract;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using static LEDCube.Animations.Animations.Weather.API.WeatherAPI;

namespace LEDCube.Animations.Animations.Weather
{
    public class CurrentWeatherAnimation : AggregateAnimation
    {
        private static readonly IWeatherConditionsAnimation _clearWeatherAnimation = new ClearWeatherAnimation();
        private static readonly IWeatherConditionsAnimation _cloudsAnimation = new CloudsAnimation();
        private static readonly IWeatherConditionsAnimation _rainAnimation = new RainAnimation();
        private static readonly IWeatherConditionsAnimation _temperatureAnimation = new TemperatureAnimation();

        private static readonly ReadOnlyDictionary<WeatherConditions, IWeatherConditionsAnimation[]> _weatherConditionsAnimations = new ReadOnlyDictionary<WeatherConditions, IWeatherConditionsAnimation[]>(new Dictionary<WeatherConditions, IWeatherConditionsAnimation[]>()
        {
            { WeatherConditions.Drizzle, new[]{ _rainAnimation } },
            { WeatherConditions.Rain, new[]{ _rainAnimation } },
            { WeatherConditions.RainHeavy, new[]{ _rainAnimation } },
            { WeatherConditions.Snow, new[]{ _rainAnimation } },
            { WeatherConditions.SnowHeavy, new[]{ _rainAnimation } },
            { WeatherConditions.Clear, new[]{ _clearWeatherAnimation }  },
            { WeatherConditions.CloudsFew, new[]{ _clearWeatherAnimation, _cloudsAnimation } },
            { WeatherConditions.CloudsOvercast, new[]{ _cloudsAnimation } },
        });

        public CurrentWeatherAnimation()
        {
        }

        public override bool AutomaticSchedulingAllowed => true;
        public override bool IsFinite => false;

        public override TimeSpan PrefferedDuration => TimeSpan.FromSeconds(20);

        protected override IEnumerable<ILEDCubeAnimation> GetAnimations()
        {
            var weatherResult = WeatherAPI.GetCurrentWeather();
            var condition = WeatherAPI.GetWeatherCondition(weatherResult.Weather.First());

            var animations = new List<IWeatherConditionsAnimation>();
            if (_weatherConditionsAnimations.ContainsKey(condition))
            {
                animations.AddRange(_weatherConditionsAnimations[condition]);
            }
            animations.Add(_temperatureAnimation);

            foreach (var animation in animations)
            {
                animation.PrepareForWeather(weatherResult);
            }

            return animations;
        }
    }
}