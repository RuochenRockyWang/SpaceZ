using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceZ.Util
{
    public static class ProcessManager
    {
        private static List<Process> _processes = new List<Process>();
            
        public static void Register(Process process)
        {
            _processes.Add(process);
        }

        public static void ExitAll()
        {
            TryExitAll(_processes);
            _processes.Clear();

            TryExitAll(Process.GetProcessesByName("SpaceZ.Payload"));
            TryExitAll(Process.GetProcessesByName("SpaceZ.LaunchVehicle"));
        }

        private static void TryExitAll(IEnumerable<Process> processes)
        {
            foreach(var process in processes) 
            {
                try
                {
                    if (process.HasExited)
                        continue;

                    if (!process.WaitForExit(200))
                        process.Kill();

                    process.Close();
                }
                catch
                {
                }
            }
        }
    }
}
