using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SpaceZ.IPC
{
    public abstract class NamedPipeBase
    {
        public string Name { get; protected set; }

        protected PipeStream Pipe;

        protected ulong Ack = 1;

        protected string PipeType = "PIPE";


        public bool IsConnected => Pipe.IsConnected;

        #region IPC message method

        private bool Verify(IpcMessage message, IpcMessageCommandType commandType) 
        {
            // had received message
            // No Use
            //if (Ack >= message.Ack)
            //    return false;

            if (message.CommandType != commandType)
                return false;

            return true;
        }

        /// <summary>
        /// send ack
        /// </summary>
        /// <returns></returns>
        public async Task SendAck()
        {
            Ack++;
            var message = new IpcMessage
            {
                CommandType = IpcMessageCommandType.SendAck,
                Ack = Ack,
                ContentLength = 0,
                Content = new byte[0],
            };

            await IpcMessageCovert.WriteAsync(Pipe, message);
        }

        /// <summary>
        /// recieve ack
        /// </summary>
        /// <returns></returns>
        public async Task<bool> RecieveAck()
        {
            var message = await IpcMessageCovert.ReadAsync(Pipe);

            if (message.CommandType != IpcMessageCommandType.SendAck)
                return false;

            Ack = message.Ack + 1;

            return true;
        }

        public async Task SendResponce(bool success)
        {
            var buffer = JsonSerializer.SerializeToUtf8Bytes(success);
            var message = new IpcMessage
            {
                CommandType = IpcMessageCommandType.Response,
                Ack = Ack,
                ContentLength = buffer.Length,
                Content = buffer,
            };

            await IpcMessageCovert.WriteAsync(Pipe, message);
        }

        public async Task<bool> ReceiveResponce()
        {
            var message = await IpcMessageCovert.ReadAsync(Pipe);

            if (!Verify(message,IpcMessageCommandType.Response))
                return false;

            Ack = message.Ack + 1;
            bool success = JsonSerializer.Deserialize<bool>(message.Content);

            return success;
        }


        /// <summary>
        /// send command
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public async Task SendCommand(string command)
        {
            var buffer = JsonSerializer.SerializeToUtf8Bytes(command);
            var message = new IpcMessage
            {
                CommandType = IpcMessageCommandType.SendCommand,
                Ack = Ack,
                ContentLength = buffer.Length,
                Content = buffer,
            };

            await IpcMessageCovert.WriteAsync(Pipe, message);
        }

        /// <summary>
        /// receive command
        /// </summary>
        /// <returns></returns>
        public async Task<IpcMessageResult<string>> ReceiveCommand()
        {
            var message = await IpcMessageCovert.ReadAsync(Pipe);

            if(!Verify(message, IpcMessageCommandType.SendCommand))
                return new IpcMessageResult<string>(success: false);

            Ack = message.Ack + 1;
            var command = JsonSerializer.Deserialize<string>(message.Content);
            
            return new IpcMessageResult<string>(success: true, command);
        }


        /// <summary>
        /// send data
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>

        public async Task SendData<T>(T data)
        {
            var buffer = JsonSerializer.SerializeToUtf8Bytes(data);
            var message = new IpcMessage
            {
                CommandType = IpcMessageCommandType.SendData,
                Ack = Ack,
                ContentLength = buffer.Length,
                Content = buffer,
            };

            await IpcMessageCovert.WriteAsync(Pipe, message);
        }

        /// <summary>
        /// receive data
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<IpcMessageResult<T>> ReceiveData<T>()
        {
            var message = await IpcMessageCovert.ReadAsync(Pipe);

            if (!Verify(message, IpcMessageCommandType.SendData))
                return new IpcMessageResult<T>(success: false);

            Ack = message.Ack + 1;
            var data = JsonSerializer.Deserialize<T>(message.Content);

            return new IpcMessageResult<T>(success: true, data);
        }

        /// <summary>
        /// send bytes
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>

        public async Task SendBytes(byte[] buffer)
        {
            var message = new IpcMessage
            {
                CommandType = IpcMessageCommandType.SendData,
                Ack = Ack,
                ContentLength = buffer.Length,
                Content = buffer,
            };

            await IpcMessageCovert.WriteAsync(Pipe, message);
        }

        /// <summary>
        /// receive bytes
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<IpcMessageResult<byte[]>> ReceiveBytes()
        {
            var message = await IpcMessageCovert.ReadAsync(Pipe);

            if (!Verify(message, IpcMessageCommandType.SendData))
                return new IpcMessageResult<byte[]>(success: false);

            Ack = message.Ack + 1;

            return new IpcMessageResult<byte[]>(success: true, message.Content);
        }

        #endregion
    
        public virtual void Stop()
        {
            // maybe useful
            Pipe.WaitForPipeDrain();
            Pipe.Close();
        }

        public void WriteLog(string message)
        {
            if (!Directory.Exists("Log"))
                Directory.CreateDirectory("Log");

            string logPath = Path.Combine("Log", $"{Name}_{PipeType}_log.txt");
            if (!File.Exists(logPath))
                File.CreateText(logPath).Close();

            var log = $"[{DateTime.Now:G}][{PipeType}][{Name}]: {message}";
            Debug.WriteLine(log);
            File.AppendAllLines(logPath, new string[] { log });
        }
    }
}
