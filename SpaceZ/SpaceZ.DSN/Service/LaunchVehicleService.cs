using SpaceZ.IPC;
using SpaceZ.Model;
using SpaceZ.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceZ.DSN.Service
{
    internal class LaunchVehicleService
    {
        private string _name;

        private int _orbit;

        private int _timeToOrbit;

        private PayloadConfig _payloadConfig;

        private CommandPipeServer _commandPipeServer;
        private DataPipeClient _telemetryClient;

        internal int TimeToOrbi => _timeToOrbit;


        internal LaunchVehicleService(LauchVehicleConfig config)
        {
            _name = config.Name;
            _orbit = config.OrBit;
            _payloadConfig = config.PayloadConfig;
            _timeToOrbit = (_orbit / 3600) + 10;

            _commandPipeServer = new CommandPipeServer(_name);
            _telemetryClient = new DataPipeClient($"{_name}-{CommonSettings.TelemetrySuffix}");
        }

        internal void Start()
        {
            var success = _commandPipeServer.Start();
            if (!success)
                return;

            // Debug.WriteLine($"[{_commandPipeServer.Name}][{DateTime.Now:G}] Server Connect Success");
        }


        internal async void Stop()
        {
            try
            {
                await _commandPipeServer.DoCommand(LaunchVehicleCommand.Deorbit);
                _commandPipeServer.Stop();
                _telemetryClient.Stop();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }


        internal async Task<bool> DoCommand(string command, Action? onSuccess = null)
        {
            bool success = false;

            switch (command)
            {
                case LaunchVehicleCommand.DeployPayload:
                case LaunchVehicleCommand.StopTelemetry:
                    success = await _commandPipeServer.DoCommand(command);
                    break;
                case LaunchVehicleCommand.StartTelemetry:
                    if (!_telemetryClient.IsConnected)
                        _telemetryClient.StartAsync();

                    success = await _commandPipeServer.DoCommand(command);
                    break;
                case LaunchVehicleCommand.Deorbit:
                    await _commandPipeServer.DoCommand(command);
                    success = !_commandPipeServer.IsConnected;
                    return success;
                default:
                    return false;
            }

            if (success)
                onSuccess?.Invoke();

            return success;
        }

        internal async void RecieveTelemetry(Action<TelemetryData> onSuccess)
            => await _telemetryClient.RecieveData(onSuccess);
    }
}
