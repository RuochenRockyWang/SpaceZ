using SpaceZ.IPC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceZ.IPC
{
    public class CommandPipeServer : NamedPipeServer
    {

        public CommandPipeServer(string name)
            : base(name)
        {
        }

        
        public async Task<bool> DoCommand(string command)
        {
            if (!IsConnected)
                return false;

            WriteLog($"Send Command {command}.");
            await SendCommand(command);

            var success = await ReceiveResponce();
            WriteLog($"Client Responce: {success}");
            return success;
        }
    }
}
