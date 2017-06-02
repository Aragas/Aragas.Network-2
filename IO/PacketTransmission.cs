using System;

using Aragas.Network.Packets;

namespace Aragas.Network.IO
{
    public abstract class PacketTransmission<TPacketType, TPacketIDType, TSerializer, TDeserializer> : IDisposable where TPacketType : Packet<TPacketIDType, TSerializer, TDeserializer> where TSerializer : PacketSerializer where TDeserializer : PacketDeserialiser
    {
        public abstract bool IsConnected { get; }

        public string Host { get; protected set; }
        public ushort Port { get; protected set; }

        protected PacketFactory<TPacketType, TPacketIDType, TSerializer, TDeserializer> Factory { get; }

        protected PacketTransmission(Type packetEnumType = null)
        {
            Factory = new PacketFactory<TPacketType, TPacketIDType, TSerializer, TDeserializer>(packetEnumType);
        }

        public virtual void Connect(string ip, ushort port) { Host = ip; Port = port; }
        public abstract void Disconnect();


        public abstract void SendPacket(TPacketType packet);
        public abstract TPacketType ReadPacket();

        public abstract void Dispose();
    }
}