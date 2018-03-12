using System.Reflection;

using Aragas.Network.Attributes;
using Aragas.Network.IO;

namespace Aragas.Network.Packets
{
    /// <summary>
    /// A <see cref="Packet"/> with <see langword="int"/> <seealso cref="Packet{TPacketType, TReader, TWriter}.ID"/>. Must have a <seealso cref="PacketAttribute"/>.
    /// </summary>
    /// <typeparam name="TDeserializer"><see cref="PacketDeserialiser"/>. You can create a custom one or use <see cref="StandardDeserialiser"/> and <see cref="ProtobufDeserialiser"/></typeparam>
    /// <typeparam name="TSerializer"><see cref="PacketSerializer"/>. You can create a custom one or use <see cref="StandardSerializer"/> and <see cref="ProtobufSerializer"/></typeparam>
    /// <typeparam name="TIntegerType">Any integer type. See <see cref="byte"/>, <see cref="short"/>, <see cref="int"/>, <see cref="long"/>, any <see cref="Aragas.Network.Data.Variant"/></typeparam>
    public abstract class PacketWithAttribute<TIntegerType, TSerializer, TDeserializer> : PacketWithIntegerType<TIntegerType, TSerializer, TDeserializer> where TSerializer : PacketSerializer where TDeserializer : PacketDeserialiser
    {
        public sealed override TIntegerType ID
        {
            get
            {
                if (!Value.Equals(default(TIntegerType)))
                    return Value;

                SetValue(GetType().GetTypeInfo().GetCustomAttribute<PacketAttribute>().ID);
                return Value;
            }
        }
    }
}