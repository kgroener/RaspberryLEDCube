using System;
using System.Collections.Generic;
using System.Text;

namespace LEDCube.Animations.Animations.Weather.API.Models
{
    public class Conditions
    {
        public double Humidity { get; set; }
        public double Pressure { get; set; }
        public double Temp { get; set; }
        public double Temp_max { get; set; }
        public double Temp_min { get; set; }
    }
}