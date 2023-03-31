using SpaceZ.DSN.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceZ.DSN.ViewModel
{
    public class SpacecraftViewModel : NotifyPropertyChanged
    {
        private string _name;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }


        public virtual void Stop()
        {

        }
    }
}
