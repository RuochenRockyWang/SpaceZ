using SpaceZ.DSN.Common;
using SpaceZ.DSN.Service;
using SpaceZ.Model;
using SpaceZ.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SpaceZ.DSN.ViewModel
{
    public class LaunchVehicleViewModel : SpacecraftViewModel
    {

        private LaunchVehicleService _service;

        public Action OnActive;

        public Action OnDeorbit;

        public Action<PayloadConfig> OnDeployPayloaded;

        public Func<bool> IsExistPayload;

        public LauchVehicleConfig Config { get; set; }


        public int TimeToOrbit => _service.TimeToOrbi;

        #region Property

        private int _orbit;

        public int Orbit
        {
            get => _orbit;
            set => SetProperty(ref _orbit, value);
        }


        private RunningState _state;

        public RunningState State
        {
            get => _state;
            set
            {
                SetProperty(ref _state, value);
                OnPropertyChanged(nameof(TelemetryVisibity));
            }
        }


        private TelemetryData _telemetryData;

        public TelemetryData TelemetryData
        {
            get => _telemetryData;
            set => SetProperty(ref _telemetryData, value);
        }


        public Visibility TelemetryVisibity
            => State.HasFlag(RunningState.SendingTelemetry)
            ? Visibility.Visible
            : Visibility.Collapsed;
        #endregion

        #region Command

        private RelayCommand<string> _sendCommand;

        public RelayCommand<string> SendCommand
             => _sendCommand ??= new RelayCommand<string>(
                 async command => await DoCommandAsync(command),  
                 CanDoCommand);


        private RelayCommand<object> _launchCommand;

        public RelayCommand<object> LaunchCommand
            => _launchCommand ??= new(_ => Launch());

        #endregion


        public LaunchVehicleViewModel(LauchVehicleConfig config)
        {
            Config = config;
            Name = config.Name;
            Orbit = config.OrBit;
            State = RunningState.Sleep;
            _service = new LaunchVehicleService(config);
        }

        ~LaunchVehicleViewModel() 
        {
            _service.Stop();
        }

        public void Start()
        {
            _service.Start();
            State = RunningState.Active;
            OnActive?.Invoke();
        }

        public override void Stop() => 
            _service?.Stop();

        private void Launch()
        {
            // Debugger.Launch();

            var process = new Process();
            process.StartInfo.FileName = CommonSettings.LaunchVehiclePath;
            process.StartInfo.Arguments = Config.ToArgs();
            process.StartInfo.CreateNoWindow = !CommonSettings.OpenConsole; 
            process.StartInfo.RedirectStandardOutput = true;

            var success = process.Start();
            // process.BeginOutputReadLine();

            if (!success)
            {
                MessageBox.Show($"Launch {Name} Failed.");
                return;
            }

            ProcessManager.Register(process);
            Start();

            State = RunningState.Flying;
            //State = RunningState.Active;

            Task.Run(() =>
            {
                Task.Delay(TimeToOrbit * 1000)
                .ContinueWith(_ => State = RunningState.Active);
            });

        }

        private bool CanDoCommand(string command)
        {
            return command switch
            {
                LaunchVehicleCommand.DeployPayload => !(IsExistPayload?.Invoke() ?? false),
                LaunchVehicleCommand.StartTelemetry => !State.HasFlag(RunningState.SendingTelemetry),
                LaunchVehicleCommand.StopTelemetry => State.HasFlag(RunningState.SendingTelemetry),
                _ => true,
            };
        }


        private async Task DoCommandAsync(string command)
        {
            if (State.HasFlag(RunningState.Flying))
            {
                MessageBox.Show($"{Name} is flying.");
                return;
            }

            bool success = false;

            switch (command)
            {
                case LaunchVehicleCommand.DeployPayload:
                    success = await _service.DoCommand(command);

                    if (success)
                        OnDeployPayloaded?.Invoke(Config.PayloadConfig);

                    break;
                case LaunchVehicleCommand.StartTelemetry:
                    success = await _service.DoCommand(command);
                    if (success)
                    {
                        State |= RunningState.SendingTelemetry;
                        _service.RecieveTelemetry(data => TelemetryData = data);
                    }

                    break;
                case LaunchVehicleCommand.StopTelemetry:
                    success = await _service.DoCommand(command);
                    if (success)
                        State ^= RunningState.SendingTelemetry;

                    break;
                case LaunchVehicleCommand.Deorbit:
                    success = await _service.DoCommand(command);
                    if (success)
                    {
                        State = RunningState.Sleep;
                        OnDeorbit?.Invoke();
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
