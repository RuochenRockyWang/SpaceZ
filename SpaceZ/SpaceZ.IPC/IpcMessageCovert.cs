using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceZ.IPC
{
    internal static class IpcMessageCovert
    {
        /// <summary>
        /// Write Message
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static async Task WriteAsync(Stream stream, IpcMessage message)
            => await WriteAsync(stream, message.Ack, message.CommandType, message.Content);

        /// <summary>
        /// Write message
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="ack"></param>
        /// <param name="commandType"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static async Task WriteAsync(Stream stream, UInt64 ack, 
                          IpcMessageCommandType commandType, byte[] content)
        {
            /* 
             * Int16 command type
             * UInt64 ack
             * Int32 content length
             * byte[] content
            */

            var headByteCount =
                // Int16 Command Type
                +sizeof(UInt16)
                // UInt64 Ack
                + sizeof(UInt64)
                // Int32 Content Length
                + sizeof(UInt32);
            
            var head = new byte[headByteCount];
            using var memoeryStream = new MemoryStream(head);
            using var binaryWritter = new BinaryWriter(memoeryStream);

            binaryWritter.Write((ushort)commandType);
            binaryWritter.Write(ack);
            binaryWritter.Write(content.Length);

            await stream.WriteAsync(head, 0, headByteCount).ConfigureAwait(false);
            await stream.WriteAsync(content, 0, content.Length).ConfigureAwait(false);
            await stream.FlushAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Read Message
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>

        public static async Task<IpcMessage> ReadAsync(Stream stream)
        {
            /* 
             * Int16 command type
             * UInt64 ack
             * Int32 content length
             * byte[] content
            */
            var headByteCount =
                // Int16 Command Type
                +sizeof(UInt16)
                // UInt64 Ack
                + sizeof(UInt64)
                // Int32 Content Length
                + sizeof(UInt32);

            var head = new byte[headByteCount];
            await stream.ReadAsync(head, 0, headByteCount).ConfigureAwait(false);
            using var memoeryStream = new MemoryStream(head);
            using var binaryReader = new BinaryReader(memoeryStream);
           
            var commandType = (IpcMessageCommandType)binaryReader.ReadInt16();
            var ack = binaryReader.ReadUInt64();
            var contentLength = binaryReader.ReadInt32();

            var content = new byte[contentLength];
            await stream.ReadAsync(content, 0, contentLength).ConfigureAwait(false);

            return new IpcMessage
            {
                CommandType = commandType,
                Ack = ack,
                ContentLength = contentLength,
                Content = content,
            };
        }
    }
}
