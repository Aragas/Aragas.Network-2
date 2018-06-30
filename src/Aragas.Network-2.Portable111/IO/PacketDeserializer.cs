using System;
using System.Collections.Generic;

namespace Aragas.Network.IO
{
    public abstract class PacketDeserializer : IDisposable
    {
        #region ExtendRead

        private static readonly Dictionary<int, Func<StreamDeserializer, int, object>> ReadExtendedList = new Dictionary<int, Func<StreamDeserializer, int, object>>();

        public static void ExtendRead<T>(Func<StreamDeserializer, int, T> func)
        {
            if (func != null) ReadExtendedList.Add(typeof(T).GetHashCode(), Transform(func));
        }

        private static Func<StreamDeserializer, int, object> Transform<T>(Func<StreamDeserializer, int, T> action) => (reader, length) => action(reader, length);

        protected static bool ExtendReadContains<T>() => ExtendReadContains(typeof(T));
        protected static bool ExtendReadContains(Type type) => ReadExtendedList.ContainsKey(type.GetHashCode());

        protected static T ExtendReadExecute<T>(StreamDeserializer reader, int length = 0) => ExtendReadContains<T>() ? (T) ReadExtendedList[typeof(T).GetHashCode()](reader, length) : default;

        protected static bool ExtendReadTryExecute<T>(StreamDeserializer reader, int length, out T value)
        {
            var exist = ReadExtendedList.TryGetValue(typeof(T).GetHashCode(), out var func);
            value = exist ? (T)func.Invoke(reader, length) : default;

            return exist;
        }

        #endregion ExtendRead

        public abstract T Read<T>(in T value = default, int length = 0);

        public abstract int BytesLeft();

        public abstract void Dispose();
    }
}