using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace SpaceZ.Model
{
    public class LauchVehicleConfig
    {
        public string Name { get; set; }

        /// <summary>
        /// km
        /// </summary>
        public int OrBit { get; set; }


        /// <summary>
        /// path to Payload 
        /// </summary>
        public PayloadConfig PayloadConfig { get; set; }


        /// <summary>
        /// Payload Start Arguments
        /// </summary>
        /// <returns></returns>
        public string ToArgs()
            => $"-n {Name} -o {OrBit} -pn {PayloadConfig.Name} -pt {PayloadConfig.Type}";


        public static LauchVehicleConfig? TryParseFromArgs(string[] args)
        {
            if (args.Length < 8)
                return null;

            var config = new LauchVehicleConfig();
            config.PayloadConfig = new PayloadConfig();
            for (int i = 0; i < args.Length; i += 2)
            {
                if (args[i] == "-n")
                    config.Name = args[i + 1];
                else if (args[i] == "-o")
                    config.OrBit = int.Parse(args[i + 1]);
                else if (args[i] == "-pn")
                    config.PayloadConfig.Name = args[i + 1];
                else if (args[i] == "-pt")
                    config.PayloadConfig.Type = Enum.Parse<PayloadType>(args[i + 1]);
            }

            return config;
        }
    }
}
