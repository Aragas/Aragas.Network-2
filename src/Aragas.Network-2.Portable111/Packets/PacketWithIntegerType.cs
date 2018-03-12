using System;

using Aragas.Network.Data;
using Aragas.Network.IO;

namespace Aragas.Network.Packets
{
    public abstract class PacketWithIntegerType<TIntegerType, TSerializer, TDeserializer> : Packet<TIntegerType, TSerializer, TDeserializer> where TSerializer : PacketSerializer where TDeserializer : PacketDeserialiser
    {
        private static bool _isInizialized = false;
        protected PacketWithIntegerType()
        {
            // Avoididing static constructor
            if (!_isInizialized)
            {
                lock (this)
                {
                    if (!_isInizialized)
                    {
                        _isInizialized = true;

                        var type = typeof(TIntegerType);

                        if (type == typeof(Byte)) { }
                        else if (type == typeof(SByte)) { }
                        else if (type == typeof(UInt16)) { }
                        else if (type == typeof(Int16)) { }
                        else if (type == typeof(UInt32)) { }
                        else if (type == typeof(Int32)) { }
                        else if (type == typeof(UInt64)) { }
                        else if (type == typeof(Int64)) { }

                        else if (type == typeof(VarShort)) { }
                        else if (type == typeof(VarZShort)) { }

                        else if (type == typeof(VarInt)) { }
                        else if (type == typeof(VarZInt)) { }

                        else if (type == typeof(VarLong)) { }
                        else if (type == typeof(VarZLong)) { }

                        else
                            throw new NotSupportedException($"TIntegerType Type {type.FullName} is not supported!");
                    }
                }
            }
        }

        protected TIntegerType Value { get; private set; }
        protected void SetValue(long id)
        {
            switch (typeof(TIntegerType).Name)
            {
                case "Byte":
                case "SByte":
                case "UInt16":
                case "Int16":
                case "UInt32":
                case "Int32":
                case "UInt64":
                case "Int64":
                    Value = (TIntegerType) Convert.ChangeType(id, typeof(TIntegerType));
                    break;

                case "VarShort":
                    Value = (TIntegerType) Convert.ChangeType(new VarShort((short) id), typeof(TIntegerType));
                    break;
                case "VarZShort":
                    Value = (TIntegerType) Convert.ChangeType(new VarZShort((short) id), typeof(TIntegerType));
                    break;

                case "VarInt":
                    Value = (TIntegerType) Convert.ChangeType(new VarInt((int) id), typeof(TIntegerType));
                    break;
                case "VarZInt":
                    Value = (TIntegerType) Convert.ChangeType(new VarZInt((int) id), typeof(TIntegerType));
                    break;

                case "VarLong":
                    Value = (TIntegerType) Convert.ChangeType(new VarLong(id), typeof(TIntegerType));
                    break;
                case "VarZLong":
                    Value = (TIntegerType) Convert.ChangeType(new VarZLong(id), typeof(TIntegerType));
                    break;

                default:
                    throw new NotSupportedException("TIntegerType Type is not supported!");

            }
        }
    }
}