using System.Diagnostics;
using System.IO.Pipes;

namespace SpaceZ.IPC
{
    public class NamedPipeServer : NamedPipeBase
    {
        protected NamedPipeServerStream Server;


        public NamedPipeServer(string name)
        {
            Name = name;
            Server = new NamedPipeServerStream(name, PipeDirection.InOut);
            Pipe = Server;
            Ack = 100;
            PipeType = "SERVER";
        }


        public bool Start()
        {
            try
            {
                WriteLog($"Wait Connect.");
                Server.WaitForConnection();
                WriteLog($"Connect Success.");
                return true;
            }
            catch(Exception ex)
            {
                WriteLog($"Connect Failed. exception:{ex.Message}");
                return false;
            }
        }

        public async Task<bool> StartAsync()
        {
            try
            {
                await Server.WaitForConnectionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Debug.Write(ex);
                return false;
            }
        }


        //public async Task<bool> DoCommand(string command)
        //{
        //    await SendCommand(command);

        //    return await ReceiveResponce();
        //}


        //public async Task<T> ReceiveDatas<T>()
        //{
        //    var result = await ReceiveData<T>();

        //    if(!result.Success)
        //        return default(T);

        //    return result.Result;
        //}
    }
}