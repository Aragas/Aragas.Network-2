using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;

namespace Aragas.Network.Data
{
    /// <summary>
    /// Encoded Int64. Not optimal for negative values.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct VarLong : IVariant, IEquatable<VarLong>
    {
        public int Size => IVariant.VariantSize(_value);


        private readonly ulong _value;


        public VarLong(ulong value) { _value = value; }
        public VarLong(long value) { _value = (ulong) value; }


        public Span<byte> Encode() => Encode(this);


        public override string ToString() => _value.ToString(CultureInfo.InvariantCulture);

        public static VarLong Parse(string str) => new VarLong(ulong.Parse(str, CultureInfo.InvariantCulture));

        public static Span<byte> Encode(VarLong value) => IVariant.Encode(value._value);
        public static int Encode(VarLong value, in Span<byte> buffer)
        {
            Span<byte> encoded = Encode(value);
            encoded.CopyTo(buffer.Slice(0, encoded.Length));
            return encoded.Length;
        }
        public static int Encode(VarLong value, Stream stream)
        {
            Span<byte> encoded = Encode(value);
            stream.Write(encoded);
            return encoded.Length;
        }

        public static VarLong Decode(in ReadOnlySpan<byte> buffer) => new VarLong(IVariant.Decode(in buffer));
        public static VarLong Decode(Stream stream) => new VarLong(IVariant.Decode(stream));
        public static int Decode(Stream stream, out VarLong result)
        {
            result = Decode(stream);
            return result.Size;
        }


        public static explicit operator VarLong(ushort value) => new VarLong(value);
        public static explicit operator VarLong(uint value) => new VarLong(value);
        public static explicit operator VarLong(ulong value) => new VarLong(value);

        public static implicit operator ushort(VarLong value) => (ushort) value._value;
        public static implicit operator uint(VarLong value) => (uint) value._value;
        public static implicit operator ulong(VarLong value) => value._value;
        public static implicit operator VarLong(Enum value) => new VarLong(Convert.ToUInt64(value, CultureInfo.InvariantCulture));


        public static bool operator !=(VarLong a, VarLong b) => !a.Equals(b);
        public static bool operator ==(VarLong a, VarLong b) => a.Equals(b);

        public bool Equals(VarLong other) => other._value.Equals(_value);
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