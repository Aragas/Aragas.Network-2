using System.Reflection;

using Aragas.Network.Attributes;
using Aragas.Network.IO;

namespace Aragas.Network.Packets
{
    /// <summary>
    /// A <see cref="Packet"/> with <see langword="int"/> <seealso cref="Packet{TPacketType, TReader, TWriter}.ID"/>. Must have a <seealso cref="PacketAttribute"/>.
    /// </summary>
    /// <typeparam name="TDeserializer"><see cref="StreamDeserializer"/>. You can create a custom one or use <see cref="StandardDeserialiser"/> and <see cref="ProtobufDeserialiser"/></typeparam>
    /// <typeparam name="TSerializer"><see cref="StreamSerializer"/>. You can create a custom one or use <see cref="StandardSerializer"/> and <see cref="ProtobufSerializer"/></typeparam>
    /// <typeparam name="TIntegerType">Any integer type. See <see cref="byte"/>, <see cref="short"/>, <see cref="int"/>, <see cref="long"/>, any <see cref="Aragas.Network.Data.Variant"/></typeparam>
    public abstract class PacketAttribute<TIntegerType, TSerializer, TDeserializer> : Packet<TIntegerType, TSerializer, TDeserializer> where TIntegerType : struct where TSerializer : PacketSerializer where TDeserializer : PacketDeserializer
    {
        private TIntegerType? _id;
        public sealed override TIntegerType ID
        {
            get
            {
                if(_id == null)
                    // casting to dynamic will allow us to use the explicit casting overrides
                    _id = (TIntegerType) (dynamic) GetType().GetTypeInfo().GetCustomAttribute<PacketAttribute>().ID;
                return _id.Value;
            }
        }
    }
}