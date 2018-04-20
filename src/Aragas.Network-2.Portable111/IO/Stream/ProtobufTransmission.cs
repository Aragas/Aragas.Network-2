using System.IO;

using Aragas.Network.Data;
using Aragas.Network.Packets;

namespace Aragas.Network.IO
{
    public class ProtobufTransmission<TProtobufPacketType> : SocketPacketTransmission<TProtobufPacketType, VarInt, ProtobufSerializer, ProtobufDeserialiser> 
        where TProtobufPacketType : Packet<VarInt, ProtobufSerializer, ProtobufDeserialiser>
    {
#if !(NETSTANDARD2_0 || NET45)
        public ProtobufTransmission(PCLExt.Network.ISocketClient socket, Stream socketStream = null, BasePacketFactory<TProtobufPacketType, VarInt, ProtobufSerializer, ProtobufDeserialiser> factory = null) 
            : base(socket, socketStream, factory) { }
#else
        public ProtobufTransmission(System.Net.Sockets.Socket socket, Stream socketStream = null, BasePacketFactory<TProtobufPacketType, VarInt, ProtobufSerializer, ProtobufDeserialiser> factory = null) 
            : base(socket, socketStream, factory) { }
#endif
    }
}