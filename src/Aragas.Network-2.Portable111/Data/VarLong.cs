using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Aragas.Network.Data
{
    /// <summary>
    /// Encoded Int64. Not optimal for negative values.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct VarLong : IEquatable<VarLong>
    {
        public int Size => Variant.VariantSize(_value);


        private readonly ulong _value;


        public VarLong(ulong value) { _value = value; }
        public VarLong(long value) { _value = (ulong) value; }


        public byte[] Encode() => Encode(in this);


        public override string ToString() => _value.ToString();

        public static VarLong Parse(string str) => new VarLong(ulong.Parse(str));

        public static byte[] Encode(in VarLong value) => Variant.Encode(value._value);
        public static int Encode(in VarLong value, byte[] buffer, int offset)
        {
            var encoded = value.Encode();
            Buffer.BlockCopy(encoded, 0, buffer, offset, encoded.Length);
            return encoded.Length;
        }
        public static int Encode(in VarLong value, Stream stream)
        {
            var encoded = value.Encode();
            stream.Write(encoded, 0, encoded.Length);
            return encoded.Length;
        }

        public static VarLong Decode(byte[] buffer, int offset) => new VarLong(Variant.Decode(buffer, offset));
        public static VarLong Decode(Stream stream) => new VarLong(Variant.Decode(stream));
        public static int Decode(byte[] buffer, int offset, out VarLong result)
        {
            result = Decode(buffer, offset);
            return result.Size;
        }
        public static int Decode(Stream stream, out VarLong result)
        {
            result = Decode(stream);
            return result.Size;
        }


        public static explicit operator VarLong(ushort value) => new VarLong(value);
        public static explicit operator VarLong(uint value) => new VarLong(value);
        public static explicit operator VarLong(ulong value) => new VarLong(value);

        public static implicit operator ushort(in VarLong value) => (ushort) value._value;
        public static implicit operator uint(in VarLong value) => (uint) value._value;
        public static implicit operator ulong(in VarLong value) => value._value;
        public static implicit operator VarLong(Enum value) => new VarLong(Convert.ToUInt64(value));


        public static bool operator !=(in VarLong a, in VarLong b) => !a.Equals(b);
        public static bool operator ==(in VarLong a, in VarLong b) => a.Equals(b);

        public bool Equals(VarLong value) => value._value.Equals(_value);
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj.GetType() != GetType())
                return false;

            return Equals((VarLong) obj);
        }
        public override int GetHashCode() => _value.GetHashCode();
    }
}