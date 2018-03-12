using System.IO;

using Aragas.Network.Packets;

namespace Aragas.Network.IO
{
    public class StandardTransmission<TStandardPacketType> : SocketPacketTransmission<TStandardPacketType, int, ProtobufSerializer, ProtobufDeserialiser> where TStandardPacketType : Packet<int, ProtobufSerializer, ProtobufDeserialiser>
    {
#if !(NETSTANDARD2_0 || NET45)
        public StandardTransmission(PCLExt.Network.ISocketClient socket, Stream socketStream = null, BasePacketFactory<TStandardPacketType, int, ProtobufSerializer, ProtobufDeserialiser> factory = null) : base(socket, socketStream, factory) { }
#else
        public StandardTransmission(System.Net.Sockets.Socket socket, Stream socketStream = null, BasePacketFactory<TStandardPacketType, int, ProtobufSerializer, ProtobufDeserialiser> factory = null) : base(socket, socketStream, factory) { }
#endif
    }
}