using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceZ.IPC
{
    [Flags]
    public enum IpcMessageCommandType: short
    {
        SendAck = 1,
        SendCommand = 1 << 1,
        SendData = 1 << 2,
        Response = 1 << 3,
        Unknow = short.MaxValue,
    }
}
