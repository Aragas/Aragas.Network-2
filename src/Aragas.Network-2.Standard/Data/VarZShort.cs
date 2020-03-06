using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;

namespace Aragas.Network.Data
{
    /// <summary>
    /// Encoded Int16. Optimal for negative values. Using zig-zag encoding.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct VarZShort : IVariant, IEquatable<VarZShort>
    {
        public int Size => IVariant.VariantSize(IVariant.ZigZagEncode(_value));


        private readonly short _value;


        public VarZShort(short value) { _value = value; }


        public Span<byte> Encode() => Encode(this);


        public override string ToString() => _value.ToString(CultureInfo.InvariantCulture);

        public static VarZShort Parse(string str) => new VarZShort(short.Parse(str, CultureInfo.InvariantCulture));

        public static Span<byte> Encode(VarZShort value) => IVariant.Encode(IVariant.ZigZagEncode(value._value));
        public static int Encode(VarZShort value, in Span<byte> buffer)
        {
            Span<byte> encoded = Encode(value);
            encoded.CopyTo(buffer.Slice(0, encoded.Length));
            return encoded.Length;
        }
        public static int Encode(VarZShort value, Stream stream)
        {
            Span<byte> encoded = Encode(value);
            stream.Write(encoded);
            return encoded.Length;
        }

        public static VarZShort Decode(in ReadOnlySpan<byte> buffer) => new VarZShort((short) IVariant.ZigZagDecode(IVariant.Decode(in buffer)));
        public static VarZShort Decode(Stream stream) => new VarZShort((short) IVariant.ZigZagDecode(IVariant.Decode(stream)));
        public static int Decode(Stream stream, out VarZShort result)
        {
            result = Decode(stream);
            return result.Size;
        }


        public static explicit operator VarZShort(short value) => new VarZShort(value);

        public static implicit operator short(VarZShort value) => value._value;
        public static implicit operator int(VarZShort value) => value._value;
        public static implicit operator long(VarZShort value) => value._value;
        public static implicit operator VarZShort(Enum value) => new VarZShort(Convert.ToInt16(value, CultureInfo.InvariantCulture));


        public static bool operator !=(VarZShort a, VarZShort b) => !a.Equals(b);
        public static bool operator ==(VarZShort a, VarZShort b) => a.Equals(b);

        public bool Equals(VarZShort other) => other._value.Equals(_value);
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj.GetType() != GetType())
                return false;

            return Equals((VarZShort) obj);
        }
        public override int GetHashCode() => _value.GetHashCode();
    }
}