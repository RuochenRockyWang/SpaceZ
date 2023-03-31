using SpaceZ.Model;
using SpaceZ.Util;
using System.Diagnostics;

namespace SpaceZ.Payload
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Debugger.Launch();

                var payLoad = PayloadConfig.TryParseFromArgs(args);
                if (payLoad == null)
                    throw new Exception("not enough args");

                int timeToOrbit = GetTimeToOrbit(args);

                var service = new DSNService(payLoad, timeToOrbit);
                service.Start();

                while (service.IsRunning)
                {
                    Task.Delay(5000);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                Environment.Exit(1);
            }
        }

        private static int GetTimeToOrbit(string[] args)
        {
            for(int i = 0; i < args.Length; i+=2) 
            {
                if (args[i] == "-tto")
                    return int.Parse(args[i+1]);
            }

            return 0;
        }
    }
}