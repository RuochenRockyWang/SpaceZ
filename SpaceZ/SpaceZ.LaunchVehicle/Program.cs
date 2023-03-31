using SpaceZ.Model;
using SpaceZ.Util;
using System.Diagnostics;

namespace SpaceZ.LaunchVehicle
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // AppDomain.CurrentDomain.ProcessExit += (sender, e) => ProcessManager.ExitAll();

                //if (!Debugger.IsAttached)
                //    Debugger.Launch();

                var config = LauchVehicleConfig.TryParseFromArgs(args);
                if (config == null)
                    throw new Exception("not enough args");

                var service = new DSNService(config);
                service.Start();

                // service.DeployPayload();

                while(service.IsRunning)
                {
                    Task.Delay(5000);
                }
            }
            catch(Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                Environment.Exit(1);
            }
        }
    }
}