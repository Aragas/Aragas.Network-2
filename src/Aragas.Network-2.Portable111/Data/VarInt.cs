﻿using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Aragas.Network.Data
{
    /// <summary>
    /// Encoded Int32. Not optimal for negative values.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct VarInt : IEquatable<VarInt>
    {
        public int Size => Variant.VariantSize(_value);


        private readonly uint _value;


        public VarInt(uint value) { _value = value; }
        public VarInt(int value) { _value = (uint) value; }


        public byte[] Encode() => Encode(in this);


        public override string ToString() => _value.ToString();

        public static VarInt Parse(string str) => new VarInt(uint.Parse(str));
        
        public static byte[] Encode(in VarInt value) => Variant.Encode(value._value);
        public static int Encode(in VarInt value, byte[] buffer, int offset)
        {
            var encoded = value.Encode();
            Buffer.BlockCopy(encoded, 0, buffer, offset, encoded.Length);
            return encoded.Length;
        }
        public static int Encode(in VarInt value, Stream stream)
        {
            var encoded = value.Encode();
            stream.Write(encoded, 0, encoded.Length);
            return encoded.Length;
        }

        public static VarInt Decode(byte[] buffer, int offset) => new VarInt((uint) Variant.Decode(buffer, offset));
        public static VarInt Decode(Stream stream) => new VarInt((uint) Variant.Decode(stream));
        public static int Decode(byte[] buffer, int offset, out VarInt result)
        {
            result = Decode(buffer, offset);
            return result.Size;
        }
        public static int Decode(Stream stream, out VarInt result)
        {
            result = Decode(stream);
            return result.Size;
        }


        public static implicit operator VarInt(ushort value) => new VarInt(value);
        public static implicit operator VarInt(uint value) => new VarInt(value);

        public static implicit operator ushort(in VarInt value) => (ushort) value._value;
        public static implicit operator uint(in VarInt value) => value._value;
        public static implicit operator ulong(in VarInt value) => value._value;
        public static implicit operator VarInt(Enum value) => new VarInt(Convert.ToUInt32(value));


        public static bool operator !=(in VarInt a, in VarInt b) => !a.Equals(b);
        public static bool operator ==(in VarInt a, in VarInt b) => a.Equals(b);

        public bool Equals(VarInt value) => _value.Equals(value._value);
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj.GetType() != GetType())
                return false;

            return Equals((VarInt) obj);
        }
        public override int GetHashCode() => _value.GetHashCode();
    }
}