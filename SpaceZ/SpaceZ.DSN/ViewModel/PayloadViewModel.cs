using SpaceZ.DSN.Common;
using SpaceZ.DSN.Service;
using SpaceZ.IPC;
using SpaceZ.Model;
using SpaceZ.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace SpaceZ.DSN.ViewModel
{
    public class PayloadViewModel : SpacecraftViewModel
    {
        private PayloadConfig _config;

        private PayloadService _service;

        public Action OnDecommission;

        #region Property
        

        private PayloadType _type;

        public PayloadType Type
        {
            get => _type;
            set => SetProperty(ref _type, value);
        }


        private RunningState _state;

        public RunningState State
        {
            get => _state;
            set
            {
                SetProperty(ref _state, value);
                OnPropertyChanged(nameof(TelemetryVisibity));
                OnPropertyChanged(nameof(DataVisibility));
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


        private WeatherData _weatherData;

        public WeatherData WeatherData
        {
            get => _weatherData;
            set => SetProperty(ref _weatherData, value);
        }


        private CommData _commData;

        public CommData CommData
        {
            get => _commData;
            set => SetProperty(ref _commData, value);
        }

        public BitmapImage SpyImage { get; private set; }


        public Visibility DataVisibility
            => State.HasFlag(RunningState.SendingData)
            ? Visibility.Visible
            : Visibility.Collapsed;


        public Visibility ScientificVisibility
            => Type == PayloadType.Scientific
            ? Visibility.Visible
            : Visibility.Collapsed;

        public Visibility CommunicationVisibility
            => Type == PayloadType.Communication
            ? Visibility.Visible 
            : Visibility.Collapsed;

        public Visibility SpyVisibility
            => Type == PayloadType.Spy
            ? Visibility.Visible 
            : Visibility.Collapsed;
        #endregion

        #region Command

        private RelayCommand<string> _sendCommand;

        public RelayCommand<string> SendCommand
            => _sendCommand ??= new(
                async command => await DoCommandAsync(command),
                CanDoCommand);

        #endregion

        public PayloadViewModel(PayloadConfig config)
        {
            _config = config;
            Name = config.Name;
            Type = config.Type;
            State = RunningState.Sleep;
            _service = new PayloadService(config);
        }


        ~PayloadViewModel()
        {
            _service.Stop();
        }

        public void Start()
        {
            _service.Start();
            State = RunningState.Active;
        }

        public override void Stop()
            => _service.Stop();

        private bool CanDoCommand(string command) 
        {
            return command switch
            {
                PayloadCommand.StartData => !State.HasFlag(RunningState.SendingData),
                PayloadCommand.StopData => State.HasFlag(RunningState.SendingData),
                PayloadCommand.StartTelemetry => !State.HasFlag(RunningState.SendingTelemetry),
                PayloadCommand.StopTelemetry => State.HasFlag(RunningState.SendingTelemetry),
                _ => true,
            };
        }

        private async Task DoCommandAsync(string command)
        {
            bool success = false;

            switch (command)
            {
                case PayloadCommand.StartData:
                    if(State.HasFlag(RunningState.SendingData))
                    {
                        MessageBox.Show("Data being sent...");
                        return;
                    }

                    success = await _service.DoCommand(command);
                    if (success)
                    {
                        State |= RunningState.SendingData;
                        switch (_type)
                        {
                            case PayloadType.Scientific:
                                _service.RecieveData<WeatherData>(data => WeatherData = data);
                                break;
                            case PayloadType.Communication:
                                _service.RecieveData<CommData>(data => CommData = data);
                                break;
                            case PayloadType.Spy:
                                _service.ReivieData(UpdateImage);
                                break;
                        };

                    }

                    break;
                case PayloadCommand.StopData:
                    if (!State.HasFlag(RunningState.SendingData))
                        return;

                    success = await _service.DoCommand(command);
                    if (success)
                        State ^= RunningState.SendingData;

                    break;
                case PayloadCommand.StartTelemetry:
                    if (State.HasFlag(RunningState.SendingTelemetry))
                    {
                        MessageBox.Show("Telemetry being sent...");
                        return;
                    }

                    success = await _service.DoCommand(command);
                    if (success)
                    {
                        State |= RunningState.SendingTelemetry;
                        _service.RecieveTelemetry(data => TelemetryData = data);
                    }

                    break;
                case PayloadCommand.StopTelemetry:
                    if (!State.HasFlag(RunningState.SendingTelemetry))
                        return;

                    success = await _service.DoCommand(command);
                    if (success)
                        State ^= RunningState.SendingTelemetry;

                    break;
                case PayloadCommand.Decommission:
                    success = await _service.DoCommand(command);
                    if (success)
                    {
                        State = RunningState.Sleep;
                        OnDecommission?.Invoke();
                    }
                    break;
                default:
                    break;
            }
        }

        private void UpdateImage(byte[] bytes)
        {
            using var ms = new MemoryStream(bytes);
            SpyImage = new BitmapImage();
            SpyImage.BeginInit();
            SpyImage.StreamSource = ms;
            SpyImage.CacheOption = BitmapCacheOption.OnLoad;
            SpyImage.EndInit();

            OnPropertyChanged(nameof(SpyImage));
        }

    }
}
