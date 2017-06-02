using System;

using Aragas.Network.Data;
using Aragas.Network.Packets;

using PCLExt.Network;

namespace Aragas.Network.IO
{
    public class ProtobufTransmission : SocketPacketTransmission<ProtobufPacket, VarInt, ProtobufSerializer, ProtobufDeserialiser>
    {
        public ProtobufTransmission(ISocketClient socket, Type packetEnumType = null) : base(socket, packetEnumType) { }
    }
}