using System;
using System.Collections.Generic;
using System.Text;

namespace LEDCube.Animations.Animations.Weather.API.Models
{
    public class LocalData
    {
        private static readonly System.DateTime UNIX_START_DATETIME = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        public long Sunrise { get; set; }
        public DateTime SunriseDate => ConvertUnixToLocalDateTime(Sunrise);
        public long Sunset { get; set; }
        public DateTime SunsetDate => ConvertUnixToLocalDateTime(Sunset);

        private DateTime ConvertUnixToLocalDateTime(long unix)
        {
            return UNIX_START_DATETIME.AddSeconds(unix).ToLocalTime();
        }
    }
}