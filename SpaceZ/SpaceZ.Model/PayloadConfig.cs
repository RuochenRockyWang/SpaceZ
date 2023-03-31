using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceZ.Model
{
    public class PayloadConfig
    {
        public string Name { get; set; }

        public PayloadType Type { get; set; }

        /// <summary>
        /// Payload Start Arguments
        /// </summary>
        /// <returns></returns>
        public string ToArgs()
            => $"-n {Name} -t {Type}";


        public static PayloadConfig? TryParseFromArgs(string[] args)
        {
            if (args.Length < 4)
                return null;

            var config = new PayloadConfig();
            for(int i = 0; i < args.Length; i += 2)
            {
                if (args[i] == "-n")
                    config.Name = args[i + 1];
                else if (args[i] == "-t")
                    config.Type = Enum.Parse<PayloadType>(args[i + 1]);
            }

            return config;
        }
    }
}
