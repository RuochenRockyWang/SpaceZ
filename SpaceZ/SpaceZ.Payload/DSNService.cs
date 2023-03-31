using SpaceZ.IPC;
using SpaceZ.Model;
using SpaceZ.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceZ.Payload
{
    internal class DSNService
    {
        private string _name;

        private PayloadType _type;

        private int _timeToOrbit;


        private CommandPipeClient _commandClient;

        private DataPipeServer _telemetryServer;

        private DataPipeServer _dataServer;

        public bool IsRunning = false;

        internal DSNService(PayloadConfig payloadConfig, int timeToOrbit)
        {
            _name = payloadConfig.Name;
            _type = payloadConfig.Type;
            _timeToOrbit = timeToOrbit;
            _commandClient = new CommandPipeClient(_name);
            _telemetryServer = new DataPipeServer($"{_name}-{CommonSettings.TelemetrySuffix}");
            _dataServer = new DataPipeServer($"{_name}-{CommonSettings.DataSuffix}");
        }


        internal void Start()
        {
            _commandClient.DoCommand = DoCommnad;
            var success = _commandClient.Start();
            if (!success)
                return;

            // Debug.WriteLine($"[{_commandClient.Name}][{DateTime.Now:G}] Client Connect Success");
            
            IsRunning = true;
            _commandClient.StartListen();
        }

        internal void Stop()
        {
            try
            {
                IsRunning = false;
                _commandClient.Stop();
                _telemetryServer.Stop();
                _dataServer.Stop();
                Environment.Exit(0);
            }
            catch(Exception ex) 
            {
                Debug.WriteLine(ex.Message);
            }
        }

        #region create random data

        protected TelemetryData CreateTelemetryData()
            => TelemetryData.RandomTelemetry(_timeToOrbit);

        protected byte[] GetImages()
        {
            var index = (new Random()).Next(0, 4);
            var imagePath = @$"Resources/Images/planet{index}.png";
            return File.ReadAllBytes(imagePath);
        }

        #endregion

        protected bool StartData()
        {
            if (!_dataServer.IsConnected)
                if (!_dataServer.Start())
                    return false;

            return (_type) switch
            {
                PayloadType.Scientific => _dataServer.StartData(WeatherData.RandomWeatherData, 60000),
                PayloadType.Communication => _dataServer.StartData(CommData.RandomCommData, 5000),
                PayloadType.Spy => _dataServer.StartBytes(GetImages, 10000),
                _ => false,
            };
        }

        protected bool DoCommnad(string command)
        {
            switch (command) 
            {
                case PayloadCommand.StartData:
                    return StartData();
                case PayloadCommand.StopData:
                    return _dataServer.StopData();
                case PayloadCommand.Decommission:
                    Stop();
                    return true;
                case PayloadCommand.StartTelemetry:
                    if (!_telemetryServer.IsConnected)
                        if (!_telemetryServer.Start())
                            return false;

                    return _telemetryServer.StartData(CreateTelemetryData, 1000);
                case PayloadCommand.StopTelemetry:
                    return _telemetryServer.StopData();
                default: 
                    return false;
            }
        }

    }
}
