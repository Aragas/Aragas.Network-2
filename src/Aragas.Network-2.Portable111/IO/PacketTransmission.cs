using System;

using Aragas.Network.Packets;

namespace Aragas.Network.IO
{
    public abstract class PacketTransmission<TPacketType, TPacketIDType, TSerializer, TDeserializer> : IDisposable
        where TPacketType : Packet<TPacketIDType, TSerializer, TDeserializer> where TSerializer : PacketSerializer where TDeserializer : PacketDeserializer
    {
        public abstract string Host { get; }
        public abstract ushort Port { get; }
        public abstract bool IsConnected { get; }

        protected BasePacketFactory<TPacketType, TPacketIDType, TSerializer, TDeserializer> Factory { get; }

        protected PacketTransmission(BasePacketFactory<TPacketType, TPacketIDType, TSerializer, TDeserializer> factory = null) 
            => Factory = factory ?? new DefaultPacketFactory<TPacketType, TPacketIDType, TSerializer, TDeserializer>();

        public virtual void Connect(string ip, ushort port) { }
        public abstract void Disconnect();


        public abstract void SendPacket(TPacketType packet);
        public abstract TPacketType ReadPacket();

        public abstract void Dispose();
    }
}