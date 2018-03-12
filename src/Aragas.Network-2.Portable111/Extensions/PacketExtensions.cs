using System;

using Aragas.Network.Data;
using Aragas.Network.IO;

using static Aragas.Network.IO.PacketSerializer;
using static Aragas.Network.IO.PacketDeserialiser;

namespace Aragas.Network.Extensions
{
    public static class PacketExtensions
    {
        private static void Extend<T>(Func<PacketDeserialiser, int, T> readFunc, Action<PacketSerializer, T, bool> writeAction)
        {
            ExtendRead(readFunc);
            ExtendWrite(writeAction);
        }

        public static void Init()
        {
            Extend<TimeSpan>(ReadTimeSpan, WriteTimeSpan);
            Extend<DateTime>(ReadDateTime, WriteDateTime);
            Extend<Vector2>(ReadVector2, WriteVector2);
            Extend<Vector3>(ReadVector3, WriteVector3);
        }

        private static void WriteTimeSpan(PacketSerializer serializer, TimeSpan value, bool writeDefaultLength = true) => serializer.Write(value.Ticks);
        private static TimeSpan ReadTimeSpan(PacketDeserialiser deserialiser, int length = 0) => new TimeSpan(deserialiser.Read<long>());

        private static void WriteDateTime(PacketSerializer serializer, DateTime value, bool writeDefaultLength = true) => serializer.Write(value.Ticks);
        private static DateTime ReadDateTime(PacketDeserialiser deserialiser, int length = 0) => new DateTime(deserialiser.Read<long>());

        private static void WriteVector2(PacketSerializer serializer, Vector2 value, bool writeDefaultLength = true)
        {
            serializer.Write(value.X);
            serializer.Write(value.Y);
        }
        private static Vector2 ReadVector2(PacketDeserialiser deserialiser, int length = 0) => new Vector2(deserialiser.Read<float>(), deserialiser.Read<float>());

        private static void WriteVector3(PacketSerializer serializer, Vector3 value, bool writeDefaultLength = true)
        {
            serializer.Write(value.X);
            serializer.Write(value.Y);
            serializer.Write(value.Z);
        }
        private static Vector3 ReadVector3(PacketDeserialiser deserialiser, int length = 0) => new Vector3(deserialiser.Read<float>(), deserialiser.Read<float>(), deserialiser.Read<float>());
    }
}