using System;
using System.IO;
using System.Net;

using Aragas.Network.Data;
using Aragas.Network.Packets;

namespace Aragas.Network.IO
{
    public class SocketPacketTransmission<TPacketType, TPacketIDType, TSerializer, TDeserializer> : PacketTransmission<TPacketType, TPacketIDType, TSerializer, TDeserializer> 
        where TPacketType : Packet<TPacketIDType, TSerializer, TDeserializer>         where TSerializer : StreamSerializer, new() where TDeserializer : StreamDeserializer, new()
    {
        protected Stream SocketStream { get; }

        protected byte[] Buffer;

#if !(NETSTANDARD2_0 || NET45)
        protected PCLExt.Network.ISocketClient Socket { get; }

        public override string Host => Socket.RemoteEndPoint.IP;
        public override ushort Port => Socket.RemoteEndPoint.Port;
        public override bool IsConnected => Socket?.IsConnected == true;

        protected SocketPacketTransmission(PCLExt.Network.ISocketClient socket, Stream socketStream = null, BasePacketFactory<TPacketType, TPacketIDType, TSerializer, TDeserializer> factory = null) : base(factory)
        {
            Socket = socket;
            SocketStream = socketStream ?? new SocketClientStream(Socket);
        }
    
        public override void Connect(string ip, ushort port) { base.Connect(ip, port); Socket.Connect(ip, port); }
        public override void Disconnect() { Socket.Disconnect(); }
#else
        protected System.Net.Sockets.Socket Socket { get; }

        public override string Host => (Socket.RemoteEndPoint as IPEndPoint)?.Address.ToString() ?? string.Empty;
        public override ushort Port => (ushort) ((Socket.RemoteEndPoint as IPEndPoint)?.Port ?? 0);
        public override bool IsConnected => Socket?.Connected == true;

        protected SocketPacketTransmission(System.Net.Sockets.Socket socket, Stream socketStream = null, BasePacketFactory<TPacketType, TPacketIDType, TSerializer, TDeserializer> factory = null) : base(factory)
        {
            Socket = socket;
            SocketStream = socketStream ?? new SocketClientStream(Socket);
        }

        public override void Connect(string ip, ushort port) { base.Connect(ip, port); Socket.Connect(ip, port); }
        public override void Disconnect() { Socket.Disconnect(false); }
#endif




        protected virtual void Send(byte[] buffer)
        {
            SocketStream.Write(buffer, 0, buffer.Length);
        }
        protected virtual byte[] Receive(long length)
        {
            var buffer = new byte[length];
            SocketStream.Read(buffer, 0, buffer.Length);
            return buffer;
        }

        protected virtual void Purge()
        {
            var lenBytes = new VarInt(Buffer.Length).Encode();
            var tempBuff = new byte[Buffer.Length + lenBytes.Length];

            Array.Copy(lenBytes, 0, tempBuff, 0, lenBytes.Length);
            Array.Copy(Buffer, 0, tempBuff, lenBytes.Length, Buffer.Length);

            Send(tempBuff);

            Buffer = null;
        }

        protected virtual long ReadPacketLength() => VarInt.Decode(SocketStream);

        public override void SendPacket(TPacketType packet)
        {
            using (var serializer = new TSerializer())
            {
                serializer.Write(packet.ID);
                packet.Serialize(serializer);
                Buffer = serializer.GetBuffer();
            }

            Purge();
        }

        public override TPacketType ReadPacket()
        {
#if !(NETSTANDARD2_0 || NET45)
            if (Socket.DataAvailable > 0)
#else
            if (Socket.Available > 0)
#endif

            {
                var dataLength = ReadPacketLength();
                if (dataLength != 0)
                {
                    var data = Receive(dataLength);
                    using (var reader = StreamDeserializer.Create<TDeserializer>(data))
                    {
                        var id = reader.Read<TPacketIDType>();
                        var packet = Factory.Create(id);
                        if (packet != null)
                        {
                            packet.Deserialize(reader);
                            return packet;
                        }
                    }
                }
            }

            return null;
        }

        public override void Dispose()
        {
            Buffer = null;
        }
    }
}