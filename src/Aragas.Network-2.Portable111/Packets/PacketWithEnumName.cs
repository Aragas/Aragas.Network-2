using Aragas.Network.IO;

namespace Aragas.Network.Packets
{
    /// <summary>
    /// A <see cref="Packet"/> with <see langword="int"/> <seealso cref="Packet{TPacketType, TReader, TWriter}.ID"/>. Must have an <seealso cref="System.Enum"/>.
    /// </summary>
    /// <typeparam name="TDeserializer"><see cref="StandardDeserialiser"/>. You can create a custom one or use <see cref="ProtobufDeserialiser"/> and <see cref="ProtobufDeserialiser"/></typeparam>
    /// <typeparam name="TSerializer"><see cref="StandardSerializer"/>. You can create a custom one or use <see cref="ProtobufSerializer"/> and <see cref="System.Enum"/></typeparam>
    /// <typeparam name="TIntegerType">Any integer type. See <see cref="byte"/>, <see cref="short"/>, <see cref="int"/>, <see cref="long"/>, any <see cref="Aragas.Network.Data.Variant"/></typeparam>
    public abstract class PacketWithEnumName<TIntegerType, TSerializer, TDeserializer> : PacketWithIntegerType<TIntegerType, TSerializer, TDeserializer> where TSerializer : PacketSerializer where TDeserializer : PacketDeserialiser
    {
        internal void SetIntegerType(long id) => SetValue(id);
        public sealed override TIntegerType ID => Value;
    }
}