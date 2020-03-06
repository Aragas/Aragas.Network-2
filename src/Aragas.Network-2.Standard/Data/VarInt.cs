using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;

namespace Aragas.Network.Data
{
    /// <summary>
    /// Encoded Int32. Not optimal for negative values.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct VarInt : IVariant, IEquatable<VarInt>
    {
        public int Size => IVariant.VariantSize(_value);

        private readonly uint _value;

        public VarInt(uint value) { _value = value; }
        public VarInt(int value) { _value = (uint) value; }

        public Span<byte> Encode() => Encode(this);

        public override string ToString() => _value.ToString(CultureInfo.InvariantCulture);

        public static VarInt Parse(string str) => new VarInt(uint.Parse(str, CultureInfo.InvariantCulture));

        public static Span<byte> Encode(VarInt value) => IVariant.Encode(value._value);
        public static int Encode(VarInt value, in Span<byte> buffer)
        {
            Span<byte> encoded = Encode(value);
            encoded.CopyTo(buffer.Slice(0, encoded.Length));
            return encoded.Length;
        }
        public static int Encode(VarInt value, Stream stream)
        {
            Span<byte> encoded = Encode(value);
            stream.Write(encoded);
            return encoded.Length;
        }

        public static VarInt Decode(in ReadOnlySpan<byte> buffer) => new VarInt((uint) IVariant.Decode(in buffer));
        public static VarInt Decode(Stream stream) => new VarInt((uint) IVariant.Decode(stream));
        public static int Decode(Stream stream, out VarInt result)
        {
            result = Decode(stream);
            return result.Size;
        }


        public static implicit operator VarInt(ushort value) => new VarInt(value);
        public static implicit operator VarInt(uint value) => new VarInt(value);
        //public static implicit operator VarInt(int value) => new VarInt(value);


        public static implicit operator ushort(VarInt value) => (ushort) value._value;
        public static implicit operator uint(VarInt value) => value._value;
        public static implicit operator ulong(VarInt value) => value._value;
        public static implicit operator VarInt(Enum value) => new VarInt(Convert.ToUInt32(value, CultureInfo.InvariantCulture));


        public static bool operator !=(VarInt a, VarInt b) => !a.Equals(b);
        public static bool operator ==(VarInt a, VarInt b) => a.Equals(b);

        public bool Equals(VarInt other) => _value.Equals(other._value);
        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;

            if (obj.GetType() != GetType())
                return false;

            return Equals((VarInt) obj);
        }
        public override int GetHashCode() => _value.GetHashCode();
    }
}