using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;

namespace Aragas.Network.Data
{
    /// <summary>
    /// Encoded Int64. Optimal for negative values. Using zig-zag encoding. 
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct VarZLong : IVariant, IEquatable<VarZLong>
    {
        public int Size => IVariant.VariantSize(IVariant.ZigZagEncode(_value));


        private readonly long _value;


        public VarZLong(long value) { _value = value; }


        public Span<byte> Encode() => Encode(this);


        public override string ToString() => _value.ToString(CultureInfo.InvariantCulture);

        public static VarZLong Parse(string str) => new VarZLong(long.Parse(str, CultureInfo.InvariantCulture));

        public static Span<byte> Encode(VarZLong value) => IVariant.Encode(IVariant.ZigZagEncode(value._value));
        public static int Encode(VarZLong value, in Span<byte> buffer)
        {
            Span<byte> encoded = Encode(value);
            encoded.CopyTo(buffer.Slice(0, encoded.Length));
            return encoded.Length;
        }
        public static int Encode(VarZLong value, Stream stream)
        {
            Span<byte> encoded = Encode(value);
            stream.Write(encoded);
            return encoded.Length;
        }

        public static VarZLong Decode(in ReadOnlySpan<byte> buffer) => new VarZLong(IVariant.ZigZagDecode(IVariant.Decode(in buffer)));
        public static VarZLong Decode(Stream stream) => new VarZLong(IVariant.ZigZagDecode(IVariant.Decode(stream)));
        public static int Decode(Stream stream, out VarZLong result)
        {
            result = Decode(stream);
            return result.Size;
        }

        public static explicit operator VarZLong(short value) => new VarZLong(value);
        public static explicit operator VarZLong(int value) => new VarZLong(value);
        public static explicit operator VarZLong(long value) => new VarZLong(value);

        public static implicit operator short(VarZLong value) => (short) value._value;
        public static implicit operator int(VarZLong value) => (int) value._value;
        public static implicit operator long(VarZLong value) => value._value;
        public static implicit operator VarZLong(Enum value) => new VarZLong(Convert.ToInt64(value, CultureInfo.InvariantCulture));


        public static bool operator !=(VarZLong a, VarZLong b) => !a.Equals(b);
        public static bool operator ==(VarZLong a, VarZLong b) => a.Equals(b);

        public bool Equals(VarZLong other) => other._value.Equals(_value);
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj.GetType() != GetType())
                return false;

            return Equals((VarZLong) obj);
        }
        public override int GetHashCode() => _value.GetHashCode();
    }
}