using LEDCube.Animations.Animations.Weather.API.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace LEDCube.Animations.Animations.Weather.API
{
    internal static class WeatherAPI
    {
        private const string CURRENT_WEATHER_API = "https://api.openweathermap.org/data/2.5/weather?q=Enschede,NL&appid=1a2e90e119511cf9b30edcfa3a0515d9";
        private static readonly RestClient _client;

        static WeatherAPI()
        {
            _client = new RestClient("https://api.openweathermap.org/data/2.5/");
        }

        public enum WeatherConditions
        {
            Unknown = -1,
            Thunderstorm,
            ThunderstormDrizzle,
            ThunderstormRain,
            Drizzle,
            Rain,
            RainHeavy,
            RainFreezing,
            Snow,
            SnowHeavy,
            SnowyRain,
            SnowyRainHeavy,
            Fog,
            Haze,
            Storm,
            Clear,
            CloudsFew,
            CloudsOvercast
        }

        public static CurrentWeatherResult GetCurrentWeather()
        {
            var request = new RestRequest("weather", Method.GET, DataFormat.Json);
            request.AddParameter("q", "Enschede,NL");
            request.AddParameter("appid", "1a2e90e119511cf9b30edcfa3a0515d9");

            var response = _client.Get<CurrentWeatherResult>(request);

            if (response.IsSuccessful)
            {
                return response.Data;
            }

            return null;
        }

        public static WeatherConditions GetWeatherCondition(Models.Weather weather)
        {
            switch (weather.ID)
            {
                case 200:
                case 201:
                case 202:
                    return WeatherConditions.ThunderstormRain;

                case 210:
                case 211:
                case 212:
                case 221:
                    return WeatherConditions.Thunderstorm;

                case 230:
                case 231:
                case 232:
                    return WeatherConditions.ThunderstormDrizzle;

                case 300:
                case 301:
                case 302:
                case 310:
                case 311:
                case 312:
                case 313:
                case 314:
                case 321:
                    return WeatherConditions.Drizzle;

                case 500:
                case 501:
                case 520:
                case 521:
                    return WeatherConditions.Rain;

                case 502:
                case 503:
                case 522:
                case 531:
                    return WeatherConditions.RainHeavy;

                case 511:
                    return WeatherConditions.RainFreezing;

                case 600:
                case 601:
                    return WeatherConditions.Snow;

                case 602:
                    return WeatherConditions.SnowHeavy;

                case 611:
                case 612:
                case 615:
                case 616:
                case 620:
                case 621:
                    return WeatherConditions.SnowyRain;

                case 613:
                case 622:
                    return WeatherConditions.SnowyRainHeavy;

                case 701:
                case 711:
                case 741:
                    return WeatherConditions.Fog;

                case 721:
                case 731:
                case 751:
                case 761:
                case 762:
                    return WeatherConditions.Haze;

                case 771:
                case 781:
                    return WeatherConditions.Storm;

                case 800:
                    return WeatherConditions.Clear;

                case 801:
                case 802:
                    return WeatherConditions.CloudsFew;

                case 803:
                case 804:
                    return WeatherConditions.CloudsOvercast;

                default:
                    return WeatherConditions.Unknown;
            }
        }
    }
}