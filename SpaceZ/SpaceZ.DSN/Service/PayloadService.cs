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
    internal class PayloadService
    {
        private string _name;
        private PayloadType _type;

        private CommandPipeServer _commandPipeServer;
        private DataPipeClient _telemetryClient;
        private DataPipeClient _dataClient;


        internal PayloadService(PayloadConfig config)
        {
            _name = config.Name;
            _type = config.Type;

            _commandPipeServer = new CommandPipeServer(_name);
            _telemetryClient = new DataPipeClient($"{_name}-{CommonSettings.TelemetrySuffix}");
            _dataClient = new DataPipeClient($"{_name}-{CommonSettings.DataSuffix}");
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
                await _commandPipeServer.DoCommand(PayloadCommand.Decommission);
                _commandPipeServer.Stop();
                _telemetryClient.Stop();
                _dataClient.Stop();
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
                case PayloadCommand.StartData:
                    if (!_dataClient.IsConnected)
                        _dataClient.StartAsync();

                    success = await _commandPipeServer.DoCommand(command);
                    break;
                case PayloadCommand.StartTelemetry:
                    if (!_telemetryClient.IsConnected)
                        _telemetryClient.StartAsync();

                    success = await _commandPipeServer.DoCommand(command);
                    break;
                case PayloadCommand.StopData:
                case PayloadCommand.StopTelemetry:
                    success = await _commandPipeServer.DoCommand(command);
                    break;
                case PayloadCommand.Decommission:
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

        internal async void ReivieData(Action<byte[]> onSuccess)
            => await _dataClient.RecieveData(onSuccess);

        internal async void RecieveData<T>(Action<T> onSuccess)
            => await _dataClient.RecieveData(onSuccess);

        internal async void RecieveTelemetry(Action<TelemetryData> onSuccess)
            => await _telemetryClient.RecieveData(onSuccess);
    }
}
