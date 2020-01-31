using LEDCube.Animations.Animations.Weather.API.Models;
using LEDCube.CanonicalSchema.Contract;
using System;
using System.Collections.Generic;
using System.Text;

namespace LEDCube.Animations.Animations.Weather.ConditionsAnimations.Contracts
{
    interface IWeatherConditionsAnimation : ILEDCubeAnimation
    {
        void PrepareForWeather(CurrentWeatherResult currentWeather);
    }
}
