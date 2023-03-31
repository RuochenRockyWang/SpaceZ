using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceZ.Model
{
    public class TelemetryData
    {
        /// <summary>
        /// Altitude in km
        /// </summary>
        public int Altitude { get; set; }

        /// <summary>
        /// Longitude in degrees (-90 deg (South) to +90 deg (North))
        /// </summary>
        public double Longtiude { get; set; }


        /// <summary>
        /// Latitude in degrees (-180 deg (West) to 180 deg (East))
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Temperature in kelvin
        /// </summary>
        public double Temperature { get; set; }


        /// <summary>
        /// Time to Orbit, in seconds counting down to 0
        /// </summary>
        public int TimeToOrbit { get; set; }


        public static TelemetryData RandomTelemetry(int t)
            => RandomTelemetry(t, 0, 1000);

        public static TelemetryData RandomTelemetry(int t, int minAlt, int maxAlt)
        {
            var random = new Random();
            var telemetry = new TelemetryData();
            telemetry.TimeToOrbit = t;
            telemetry.Altitude = random.Next(minAlt, maxAlt);
            telemetry.Longtiude = Math.Round((random.NextDouble() - 0.5) * 180, 2);
            telemetry.Latitude = Math.Round((random.NextDouble() - 0.5) * 360, 2);
            telemetry.Temperature = random.Next(0, 4000) / 10.0;

            return telemetry;
        }

    }
}
