using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceZ.Model
{
    public class WeatherData
    {
        /// <summary>
        /// Rainfall in mm
        /// </summary>
        public int Rainfall { get; set; }

        /// <summary>
        /// Humidty in %
        /// </summary>
        public int Humidty { get; set; }

        /// <summary>
        /// Snow in inch
        /// </summary>
        public int Snow { get; set; }



        public static WeatherData RandomWeatherData()
        {
            var random = new Random();
            var weather = new WeatherData();
            weather.Rainfall = random.Next(0, 101);
            weather.Humidty = random.Next(0, 101);
            weather.Snow = random.Next(0, 10);

            return weather;
        }
    }
}
