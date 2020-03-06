using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Aragas.Network.Data
{
    /// <summary>
    /// Encoded String. Using VarLong as length.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct VarString : IVariant, IEquatable<VarString>
    {
        public int Size => _length.Size + _value.Length;

        private readonly VarInt _length;
        private readonly string _value;

        public VarString(string value) { _value = value; _length = new VarInt(_value.Length); }

        public Span<byte> Encode() => Encode(in this);

        public override string ToString() => _value;

        public static Span<byte> Encode(in VarString value)
        {
            Span<byte> lengthArray = value._length.Encode();
            Span<byte> stringArray = Encoding.UTF8.GetBytes(value._value);
            Span<byte> result = new byte[lengthArray.Length + stringArray.Length];

            lengthArray.CopyTo(result.Slice(0, lengthArray.Length));
            stringArray.CopyTo(result.Slice(lengthArray.Length, stringArray.Length));

            return result;
        }
        public static int Encode(in VarString value, in Span<byte> buffer)
        {
            Span<byte> encoded = Encode(in value);
            encoded.CopyTo(buffer.Slice(0, encoded.Length));
            return encoded.Length;
        }
        public static int Encode(in VarString value, Stream stream)
        {
            Span<byte> encoded = Encode(in value);
            stream.Write(encoded);
            return encoded.Length;
        }

        public static VarString Decode(in ReadOnlySpan<byte> buffer)
        {
            var stringDataLength = VarLong.Decode(in buffer);
            var stringData = buffer.Slice(stringDataLength.Size, stringDataLength);
            return new VarString(Encoding.UTF8.GetString(stringData));
        }
        public static VarString Decode(Stream stream)
        {
            var stringDataLength = VarInt.Decode(stream);
            Span<byte> stringData = stackalloc byte[stringDataLength];
            stream.Read(stringData);
            return new VarString(Encoding.UTF8.GetString(stringData));
        }
        public static int Decode(Stream stream, out VarString result)
        {
            result = Decode(stream);
            return result.Size;
        }


        public static implicit operator VarString(string value) => new VarString(value);
        public static implicit operator string(VarString value) => value._value;


        public static bool operator !=(VarString a, VarString b) => !a.Equals(b);
        public static bool operator ==(VarString a, VarString b) => a.Equals(b);

        public bool Equals(VarString other) => other._value.Equals(_value);
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj.GetType() != GetType())
                return false;

            return Equals((VarString) obj);
        }
        public override int GetHashCode() => _value.GetHashCode();
    }
}