using SpaceZ.IPC;
using SpaceZ.Model;
using SpaceZ.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceZ.LaunchVehicle
{
    internal class DSNService
    {
        private string _name;

        private int _orbit;

        private int _timeToOrbit;

        private PayloadConfig _payloadConfig;

        private bool _isReached = false;


        private CommandPipeClient _commandClient;

        private DataPipeServer _telemetryServer;


        public bool IsRunning = false;


        internal DSNService(LauchVehicleConfig config)
        {
            _name = config.Name;
            _orbit = config.OrBit;
            _payloadConfig = config.PayloadConfig;
            _timeToOrbit = (_orbit / 3600) + 10;

            _commandClient = new CommandPipeClient(_name);
            _telemetryServer = new DataPipeServer($"{_name}-{CommonSettings.TelemetrySuffix}");
        }


        internal void Start()
        {
            _commandClient.DoCommand = DoCommnad;
            var success = _commandClient.Start();
            if (!success)
                return;

            // Debug.WriteLine($"[{_commandClient.Name}][{DateTime.Now:G}] Client Connect Success");

            IsRunning = true;
            Task.Delay(_timeToOrbit * 1000)
                .ContinueWith(_ => _isReached = true);

            _commandClient.StartListen();
        }

        internal void Stop()
        {
            try
            {
                IsRunning = false;
                _commandClient.Stop();
                _telemetryServer.Stop();
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        #region create random data

        protected TelemetryData CreateTelemetryData()
            => TelemetryData.RandomTelemetry(_timeToOrbit);

        #endregion

        /// <summary>
        /// deploy payload
        /// </summary>
        /// <returns></returns>
        internal bool DeployPayload()
        {
            // Debugger.Launch();
            if (!_isReached)
                return false;

            var process = new Process();
            process.StartInfo.FileName = CommonSettings.PayloadFilePath;
            var args = _payloadConfig.ToArgs();
            args = $"{args} -tto {_timeToOrbit}";
            process.StartInfo.Arguments = args;
            process.StartInfo.CreateNoWindow = !CommonSettings.OpenConsole;
            process.StartInfo.RedirectStandardOutput = true;

            // ProcessManager.Register(process);
            var success = process.Start();
            // process.BeginOutputReadLine();

            return success;
        }

        internal bool DoCommnad(string command)
        {
            switch (command)
            {
                case LaunchVehicleCommand.DeployPayload:
                    return DeployPayload();
                case LaunchVehicleCommand.Deorbit:
                    Stop();
                    break;
                case LaunchVehicleCommand.StartTelemetry:
                    if (!_telemetryServer.IsConnected)
                        if (!_telemetryServer.Start())
                            return false;

                    return _telemetryServer.StartData(CreateTelemetryData, 1000);
                case LaunchVehicleCommand.StopTelemetry:
                    return _telemetryServer.StopData();
                default:
                    return false;
            }

            return true;
        }


    }
}
