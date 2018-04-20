﻿using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Aragas.Network.Data
{
    /// <summary>
    /// Encoded Int16. Not optimal for negative values.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VarShort : IEquatable<VarShort>
    {
        public int Size => Variant.VariantSize(_value);


        private readonly ushort _value;


        public VarShort(ushort value) { _value = value; }
        public VarShort(short value) { _value = (ushort) value; }


        public byte[] Encode() => Encode(this);


        public override string ToString() => _value.ToString();

        public static VarShort Parse(string str) => new VarShort(ushort.Parse(str));

        public static byte[] Encode(VarShort value) => Variant.Encode(value._value);
        public static int Encode(VarShort value, byte[] buffer, int offset)
        {
            var encoded = value.Encode();
            Buffer.BlockCopy(encoded, 0, buffer, offset, encoded.Length);
            return encoded.Length;
        }
        public static int Encode(VarShort value, Stream stream)
        {
            var encoded = value.Encode();
            stream.Write(encoded, 0, encoded.Length);
            return encoded.Length;
        }

        public static VarShort Decode(byte[] buffer, int offset) => new VarShort((ushort) Variant.Decode(buffer, offset));
        public static VarShort Decode(Stream stream) => new VarShort((ushort) Variant.Decode(stream));
        public static int Decode(byte[] buffer, int offset, out VarShort result)
        {
            result = Decode(buffer, offset);
            return result.Size;
        }
        public static int Decode(Stream stream, out VarShort result)
        {
            result = Decode(stream);
            return result.Size;
        }


        public static explicit operator VarShort(ushort value) => new VarShort(value);

        public static implicit operator ushort(VarShort value) => value._value;
        public static implicit operator uint(VarShort value) => value._value;
        public static implicit operator ulong(VarShort value) => value._value;
        public static implicit operator VarShort(Enum value) => new VarShort(Convert.ToUInt16(value));


        public static bool operator !=(VarShort a, VarShort b) => !a.Equals(b);
        public static bool operator ==(VarShort a, VarShort b) => a.Equals(b);

        public bool Equals(VarShort value) => value._value.Equals(_value);
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj.GetType() != GetType())
                return false;

            return Equals((VarShort) obj);
        }
        public override int GetHashCode() => _value.GetHashCode();
    }
}