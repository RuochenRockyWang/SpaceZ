using SpaceZ.DSN.Common;
using SpaceZ.Model;
using SpaceZ.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace SpaceZ.DSN.ViewModel
{
    public class MainViewModel : NotifyPropertyChanged
    {

        public ObservableCollection<LaunchVehicleViewModel> SleepLaunchVehicles { get; set; }

        public ObservableCollection<SpacecraftViewModel> ActiveSpacecrafts { get; set; }

        #region Property

        private SpacecraftViewModel? _selected;

        /// <summary>
        /// selected Spacecrft
        /// </summary>
        public SpacecraftViewModel? Selected
        {
            get => _selected; 
            set => SetProperty(ref _selected, value);
        }

        #endregion


        public MainViewModel()
        {
            SleepLaunchVehicles = new ();
            ActiveSpacecrafts = new ();

            Init();
        }


        public void Init()
        {
            foreach(var config in ReadFromConfigs())
            {
                try
                {

                    CreateLaunchVehicle(config);
                }
                catch(Exception ex) 
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        public void Stop()
        {
            foreach(var spacecraft in ActiveSpacecrafts)
            {
                spacecraft.Stop();
            }
        }

        #region Spacecraft Data Method

        private IEnumerable<LauchVehicleConfig> ReadFromConfigs()
        {
            var dir = new DirectoryInfo("Config");
            var files = dir.GetFiles();

            foreach(var file in files) 
            {
                if (file.Extension != ".json")
                    continue;

                var text = File.ReadAllText(file.FullName);
                yield return JsonSerializer.Deserialize<LauchVehicleConfig>(text);
            }
        }


        private LaunchVehicleViewModel CreateLaunchVehicle(LauchVehicleConfig config)
        {
            var launch = new LaunchVehicleViewModel(config);
            launch.OnActive = () =>
            {
                SleepLaunchVehicles.Remove(launch);
                ActiveSpacecrafts.Add(launch);
            };

            launch.OnDeorbit = () =>
            {
                ActiveSpacecrafts.Remove(launch);
                SleepLaunchVehicles.Add(launch);
            };

            launch.OnDeployPayloaded = payloadConfig =>
            {
                var payload = CreatePayload(payloadConfig);
                payload.Start();

                ActiveSpacecrafts.Add(payload);
            };

            launch.IsExistPayload = () 
                => ActiveSpacecrafts.Any(space => space.Name == launch.Config.PayloadConfig.Name);

            SleepLaunchVehicles.Add (launch);
            return launch;
        }

        private PayloadViewModel CreatePayload(PayloadConfig config)
        {
            var payload = new PayloadViewModel(config);
            payload.OnDecommission = () => ActiveSpacecrafts.Remove(payload);

            return payload;
        }


        /// <summary>
        /// For Test
        /// </summary>
        /// <returns></returns>
        private IEnumerable<LauchVehicleConfig> TestDatas()
        {
            yield return new LauchVehicleConfig()
            {
                Name = "Bird-9",
                OrBit = 600,
                PayloadConfig = new PayloadConfig()
                {
                    Name = "GPM",
                    Type = PayloadType.Scientific,
                },
            };

            yield return new LauchVehicleConfig()
            {
                Name = "Bird-Heavy",
                OrBit = 36000,
                PayloadConfig = new PayloadConfig()
                {
                    Name = "TDRS-11",
                    Type = PayloadType.Communication,
                },
            };

            yield return new LauchVehicleConfig()
            {
                Name = "Hawk-Heavy",
                OrBit = 3000,
                PayloadConfig = new PayloadConfig()
                {
                    Name = "RO-245",
                    Type = PayloadType.Spy,
                },
            };
        }

        /// <summary>
        /// For Test
        /// </summary>
        /// <returns></returns>
        private void SaveConfig(LauchVehicleConfig config)
        {
            var path = @$"Config/{config.Name}.json";
            File.WriteAllText(path, JsonSerializer.Serialize(config));
        }

        #endregion  
        
    }
}
