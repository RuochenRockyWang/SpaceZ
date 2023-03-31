using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceZ.DSN.ViewModel
{
    [Flags]
    public enum RunningState : short
    {
        Sleep = 0,
        Active = 1,
        SendingData = 1 << 1,
        SendingTelemetry = 1 << 2,
        // Sending = SendingData | SendingTelemetry,
        Flying = 1 << 3,
        Unkown = short.MaxValue,
    }
}
