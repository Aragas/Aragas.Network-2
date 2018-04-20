using System;
using System.Collections.Generic;

namespace Aragas.Network.IO
{
    public abstract class PacketSerializer : IDisposable
    {
        #region ExtendWrite

        private static readonly Dictionary<int, Action<StreamSerializer, object, bool>> WriteExtendedList = new Dictionary<int, Action<StreamSerializer, object, bool>>();

        public static void ExtendWrite<T>(Action<StreamSerializer, T, bool> action)
        {
            if (action != null)
                WriteExtendedList.Add(typeof(T).GetHashCode(), Transform(action));
        }

        private static Action<StreamSerializer, object, bool> Transform<T>(Action<StreamSerializer, T, bool> action) => (stream, value, writedef) => action(stream, (T)value, writedef);

        protected static bool ExtendWriteContains<T>() => ExtendWriteContains(typeof(T));
        protected static bool ExtendWriteContains(Type type) => WriteExtendedList.ContainsKey(type.GetHashCode());

        protected static void ExtendWriteExecute<T>(StreamSerializer stream, T value, bool writeDefaultLength = true)
        {
            if (WriteExtendedList.TryGetValue(typeof(T).GetHashCode(), out var action))
                action.Invoke(stream, value, writeDefaultLength);
        }

        #endregion ExtendWrite

        public abstract void Write<TDataType>(TDataType value = default(TDataType), bool writeDefaultLength = true);

        public abstract void Dispose();
    }
}