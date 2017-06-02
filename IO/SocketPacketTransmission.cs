using System;
using System.IO;

using Aragas.Network.Data;
using Aragas.Network.Packets;

using PCLExt.Network;

namespace Aragas.Network.IO
{
    public class SocketPacketTransmission<TPacketType, TPacketIDType, TSerializer, TDeserializer> : PacketTransmission<TPacketType, TPacketIDType, TSerializer, TDeserializer> where TPacketType : Packet<TPacketIDType, TSerializer, TDeserializer> where TSerializer : PacketSerializer, new() where TDeserializer : PacketDeserialiser, new()
    {
        public override bool IsConnected => Socket.IsConnected;

        protected ISocketClient Socket { get; }
        protected Stream SocketStream { get; }

        protected byte[] Buffer;


        protected SocketPacketTransmission(ISocketClient socket, Type packetEnumType = null) : base(packetEnumType)
        {
            Socket = socket;
            SocketStream = new SocketClientStream(Socket);
        }


        public override void Connect(string ip, ushort port) { base.Connect(ip, port); Socket.Connect(ip, port); }
        public override void Disconnect() { Socket.Disconnect(); }

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
            }

            Purge();
        }

        public override TPacketType ReadPacket()
        {
            if (Socket.DataAvailable > 0)
            {
                var dataLength = ReadPacketLength();
                if (dataLength != 0)
                {
                    var data = Receive(dataLength);
                    using (var reader = PacketDeserialiser.Create<TDeserializer>(data))
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