using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;

namespace Aragas.Network.Data
{
    /// <summary>
    /// Encoded Int32. Optimal for negative values. Using zig-zag encoding. 
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct VarZInt : IVariant, IEquatable<VarZInt>
    {
        public int Size => IVariant.VariantSize(IVariant.ZigZagEncode(_value));


        private readonly int _value;


        public VarZInt(int value) { _value = value; }


        public Span<byte> Encode() => Encode(this);


        public override string ToString() => _value.ToString(CultureInfo.InvariantCulture);

        public static VarZInt Parse(string str) => new VarZInt(int.Parse(str, CultureInfo.InvariantCulture));

        public static Span<byte> Encode(VarZInt value) => IVariant.Encode(IVariant.ZigZagEncode(value._value));
        public static int Encode(VarZInt value, in Span<byte> buffer)
        {
            Span<byte> encoded = Encode(value);
            encoded.CopyTo(buffer.Slice(0, encoded.Length));
            return encoded.Length;
        }
        public static int Encode(VarZInt value, Stream stream)
        {
            Span<byte> encoded = Encode(value);
            stream.Write(encoded);
            return encoded.Length;
        }

        public static VarZInt Decode(in ReadOnlySpan<byte> buffer) => new VarZInt((int) IVariant.ZigZagDecode(IVariant.Decode(in buffer)));
        public static VarZInt Decode(Stream stream) => new VarZInt((int) IVariant.ZigZagDecode(IVariant.Decode(stream)));
        public static int Decode(Stream stream, out VarZInt result)
        {
            result = Decode(stream);
            return result.Size;
        }

        public static explicit operator VarZInt(short value) => new VarZInt(value);
        public static explicit operator VarZInt(int value) => new VarZInt(value);

        public static implicit operator short(VarZInt value) => (short) value._value;
        public static implicit operator int(VarZInt value) => value._value;
        public static implicit operator long(VarZInt value) => value._value;
        public static implicit operator VarZInt(Enum value) => new VarZInt(Convert.ToInt32(value, CultureInfo.InvariantCulture));


        public static bool operator !=(VarZInt a, VarZInt b) => !a.Equals(b);
        public static bool operator ==(VarZInt a, VarZInt b) => a.Equals(b);

        public bool Equals(VarZInt other) => other._value.Equals(_value);
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj.GetType() != GetType())
                return false;

            return Equals((VarZInt) obj);
        }
        public override int GetHashCode() => _value.GetHashCode();
    }
}