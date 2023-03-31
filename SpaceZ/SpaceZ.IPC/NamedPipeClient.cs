using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace SpaceZ.IPC
{
    public class NamedPipeClient : NamedPipeBase
    {
        protected NamedPipeClientStream Client;

        public NamedPipeClient(string name)
        {
            Name = name;
            Client = new NamedPipeClientStream(".", name, PipeDirection.InOut,
                                            PipeOptions.None, TokenImpersonationLevel.Impersonation);

            Pipe = Client;
            Ack = 0;
            PipeType = "CLIENT";
        }


        public bool Start()
        {
            try
            {
                WriteLog($"Wait Connect.");
                Client.Connect(10000);
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
           return await Task.Run(() => {
                try
                {
                    Client.Connect(10000);
                    WriteLog($"Connect Success.");
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    WriteLog($"Connect Failed. exception:{ex.Message}");
                    return false;
                }

            });
        }
    }
}
