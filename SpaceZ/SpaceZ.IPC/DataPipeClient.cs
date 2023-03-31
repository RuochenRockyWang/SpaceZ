using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceZ.IPC
{
    public class DataPipeClient : NamedPipeClient
    {
        public DataPipeClient(string name) 
            : base(name)
        {
        }


        public async Task RecieveData<T>(Action<T> onSuccess)
        {
            while (IsConnected)
            {
                WriteLog($"Wait data.");
                var result = await ReceiveData<T>();
                if (!result.Success)
                    break;

                WriteLog($"Receive data.");
                onSuccess(result.Result);
            }

            WriteLog($"Stop Receive data.");
        }

        public async Task RecieveData(Action<byte[]> onSuccess)
        {
            while (IsConnected)
            {
                WriteLog($"Wait bytes data.");
                var result = await ReceiveBytes();
                if (!result.Success)
                    break;

                WriteLog($"Receive bytes data.");
                onSuccess(result.Result);
            }

            WriteLog($"Stop Receive bytes data.");
        }
    }
}
