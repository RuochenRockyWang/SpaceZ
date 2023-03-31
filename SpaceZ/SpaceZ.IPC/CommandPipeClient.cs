using SpaceZ.IPC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceZ.IPC
{
    public class CommandPipeClient : NamedPipeClient
    {
        public Func<string, bool> DoCommand { get; set; }

        public CommandPipeClient(string name) 
            : base(name)
        {
        }

        public void StartListen()
        {
            WriteLog($"Start Listen.");
            Task.Run(() => ListenCommand());
        }

        
        private async Task ListenCommand()
        {
            while(IsConnected)
            {
                WriteLog($"Wait Command.");
                var result = await ReceiveCommand();
                var success = result.Success;

                WriteLog($"Received Command {result.Result}.");
                if (success)
                    success &= DoCommand(result.Result);

                await SendResponce(success);
                WriteLog($"Send Responce: {success}");
                await Task.Delay(200);
            }
        }
    }
}
