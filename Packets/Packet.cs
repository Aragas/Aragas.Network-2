using Aragas.Network.IO;

namespace Aragas.Network.Packets
{
    public abstract class Packet { }
    public abstract class Packet<TPacketIDType, TSerializer, TDeserializer> : Packet where TSerializer : PacketSerializer where TDeserializer : PacketDeserialiser
    {
        public abstract TPacketIDType ID { get; }

        public abstract void Deserialize(TDeserializer deserialiser);

        public abstract void Serialize(TSerializer serializer);
    }
}