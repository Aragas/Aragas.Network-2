using System;
using System.Collections.Generic;
using System.IO;

namespace Aragas.Network.IO
{
    public abstract class PacketDeserialiser : IDisposable
    {
        public static TDeserializer Create<TDeserializer>(byte [] data) where TDeserializer : PacketDeserialiser, new() => new TDeserializer() { Stream = new MemoryStream(data) };

        public Stream Stream { get; internal set; }

        private PacketDeserialiser() { }
        protected PacketDeserialiser(byte[] data) { Stream = new MemoryStream(data); }
        protected PacketDeserialiser(Stream stream) { Stream = stream; }

        #region ExtendRead

        private static readonly Dictionary<int, Func<PacketDeserialiser, int, object>> ReadExtendedList = new Dictionary<int, Func<PacketDeserialiser, int, object>>();

        public static void ExtendRead<T>(Func<PacketDeserialiser, int, T> func)
        {
            if (func != null)
                ReadExtendedList.Add(typeof(T).GetHashCode(), Transform(func));
        }
        private static Func<PacketDeserialiser, int, object> Transform<T>(Func<PacketDeserialiser, int, T> action) => (reader, length) => action(reader, length);

        protected static bool ExtendReadContains<T>() => ExtendReadContains(typeof(T));
        protected static bool ExtendReadContains(Type type) => ReadExtendedList.ContainsKey(type.GetHashCode());

        protected static T ExtendReadExecute<T>(PacketDeserialiser reader, int length = 0) => ExtendReadContains<T>() ? (T)ReadExtendedList[typeof(T).GetHashCode()](reader, length) : default(T);
        protected static bool ExtendReadTryExecute<T>(PacketDeserialiser reader, int length, out T value)
        {
            Func<PacketDeserialiser, int, object> func;
            var exist = ReadExtendedList.TryGetValue(typeof(T).GetHashCode(), out func);
            value = exist ? (T)func.Invoke(reader, length) : default(T);

            return exist;
        }

        #endregion ExtendRead


        public abstract T Read<T>(T value = default(T), int length = 0);

        public virtual int BytesLeft() => (int) (Stream.Length - Stream.Position);


        public virtual void Dispose()
        {
            Stream.Dispose();
        }
    }
}