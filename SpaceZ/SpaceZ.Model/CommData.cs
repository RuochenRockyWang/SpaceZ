using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceZ.Model
{
    public class CommData
    {
        /// <summary>
        /// Uplink in MBps
        /// </summary>
        public int Uplink { get; set; }


        /// <summary>
        /// Downlink in MBps
        /// </summary>
        public int Downlink { get; set; }


        /// <summary>
        /// Active Transponders
        /// </summary>
        public int ActiveTransponders { get; set; }


        public static CommData RandomCommData()
        {
            var random = new Random();
            var comm = new CommData();
            comm.Uplink = random.Next(0, 101);
            comm.Downlink = random.Next(40, 100) * 100;
            comm.ActiveTransponders = random.Next(0, 101);

            return comm;
        }
    }
}
