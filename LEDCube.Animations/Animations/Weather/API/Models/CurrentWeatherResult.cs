using System;
using System.Collections.Generic;
using System.Text;

namespace LEDCube.Animations.Animations.Weather.API.Models
{
    public class CurrentWeatherResult
    {
        public Dictionary<string, double> Clouds { get; set; }
        public Conditions Main { get; set; }
        public Dictionary<string, double> Rain { get; set; }
        public LocalData Sys { get; set; }
        public IEnumerable<Weather> Weather { get; set; }
        public Wind Wind { get; set; }
    }
}