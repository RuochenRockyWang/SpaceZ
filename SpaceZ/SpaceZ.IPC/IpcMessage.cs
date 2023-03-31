using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceZ.IPC
{
    /// <summary>
    /// IPC Message
    /// </summary>
    public struct IpcMessage
    {
        public IpcMessageCommandType CommandType { get; set; }

        public UInt64 Ack { get; set; }

        public int ContentLength { get; set; }

        public byte[] Content { get; set; }
    }
}
