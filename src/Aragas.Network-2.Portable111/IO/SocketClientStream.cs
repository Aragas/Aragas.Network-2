using System;
using System.IO;

namespace Aragas.Network.IO
{ 
    public class SocketClientStream : Stream
    {
        public override bool CanRead { get; }
        public override bool CanSeek { get; }
        public override bool CanWrite { get; }
        public override long Length { get; }
        public override long Position { get; set; }

#if !(NETSTANDARD2_0 || NET45)
        private readonly PCLExt.Network.ISocketClient _client;

        public SocketClientStream(PCLExt.Network.ISocketClient client) { _client = client; }

        public override void Write(byte[] buffer, int offset, int count) { _client.Write(buffer, offset, count); }
        public override int Read(byte[] buffer, int offset, int count) { return _client.Read(buffer, offset, count); }
#else
        private readonly System.Net.Sockets.Socket _client;
        
        public SocketClientStream(System.Net.Sockets.Socket client) { _client = client; }  
        
        public override void Write(byte[] buffer, int offset, int count) { _client.Send(buffer, offset, count, System.Net.Sockets.SocketFlags.None); }
        public override int Read(byte[] buffer, int offset, int count) { return _client.Receive(buffer, offset, count, System.Net.Sockets.SocketFlags.None); }
#endif

        public override void Flush() { throw new NotImplementedException(); }
        public override long Seek(long offset, SeekOrigin origin) { throw new NotImplementedException(); }
        public override void SetLength(long value) { throw new NotImplementedException(); }
    }
}