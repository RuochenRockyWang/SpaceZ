using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceZ.IPC
{
    public class DataPipeServer : NamedPipeServer
    {
        public bool IsSendingData { get; set; } = false;

        public DataPipeServer(string name)
            : base(name)
        {

        }

        public override void Stop()
        {
            IsSendingData = false;

            base.Stop();
        }


        public bool StartData<T>(Func<T> getData, int delay)
        {
            WriteLog($"Preparing Info, IsConnect:{IsConnected}, IsSendingData: {IsSendingData}, delay:{delay / 1000}s.");

            if (!IsConnected || IsSendingData)
                return false;

            IsSendingData = true;
            Task.Run(() => SendData(getData, delay));

            return true;
        }

        public bool StopData()
        {
            if (!IsConnected || !IsSendingData)
                return false;

            IsSendingData = false;
            return true;
        }

        private async Task SendData<T>(Func<T> getData, int delay)
        {
            while (IsSendingData)
            {
                Ack++;
                await SendData(getData());
                WriteLog($"Send data.");
                await Task.Delay(delay);
            }

            WriteLog($"Stop send data.");
            if (!IsConnected)
                return;

            await SendResponce(false);
        }

        public bool StartBytes(Func<byte[]> getData, int delay)
        {
            WriteLog($"Preparing Info, IsConnect:{IsConnected}, IsSendingData: {IsSendingData}, delay:{delay / 1000}s.");

            if (!IsConnected || IsSendingData)
                return false;

            IsSendingData = true;
            var task = Task.Run(() => SendBytes(getData, delay));

            return true;
        }

        public bool StopBytes()
        {
            if (!IsConnected || !IsSendingData)
                return false;

            IsSendingData = false;
            return true;
        }


        private async Task SendBytes(Func<byte[]> getData, int delay)
        {
            while (IsSendingData)
            {
                Ack++;
                await SendBytes(getData());
                WriteLog($"Send bytes data.");
                await Task.Delay(delay);
            }

            WriteLog($"Stop send bytes data.");
            if (!IsConnected)
                return;

            await SendResponce(false);
        }
    }
}
