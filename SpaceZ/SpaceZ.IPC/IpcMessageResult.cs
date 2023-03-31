using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceZ.IPC
{
    public class IpcMessageResult<T> 
    {
        public IpcMessageResult(bool success) 
        {
            Success = success;
        }

        public IpcMessageResult(bool success, T result)
        {
            Success = success;
            Result = result;
        }

        public bool Success { get; }

        public T Result { get; }
    }
}
