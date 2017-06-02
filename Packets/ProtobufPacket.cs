using Aragas.Network.Data;
using Aragas.Network.IO;

namespace Aragas.Network.Packets
{
    public abstract class ProtobufPacket : Packet<VarInt, ProtobufSerializer, ProtobufDeserialiser>
    {

    }
}