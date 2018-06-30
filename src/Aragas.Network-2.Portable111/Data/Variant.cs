using System.IO;

namespace Aragas.Network.Data
{
    internal static class Variant
    {
        internal static int VariantSize(ulong value)
        {
            var outputSize = 0;
            while (value > 0b_01111111UL)
            {
                outputSize++;
                value >>= 7;
            }
            return ++outputSize;
        }
        internal static int VariantSize(long value)
        {
            var outputSize = 0;
            while (value > 0b_01111111L)
            {
                outputSize++;
                value >>= 7;
            }
            return ++outputSize;
        }

        internal static byte[] Encode(ulong value)
        {
            var size = VariantSize(value);
            var array = new byte[size];

            for (var i = 0; i < array.Length - 1; i++)
            {
                array[i] = (byte) (value & 0b_01111111U | 0b_10000000U);
                value >>= 7;
            }
            array[array.Length - 1] = (byte) value;

            return array;
        }

        internal static ulong Decode(byte[] buffer, int offset)
        {
            ulong decodedValue = 0;
            int index = 0, shiftAmount = 0;
            byte currByte;
            do
            {
                currByte = buffer[offset + index++];
                decodedValue |= currByte & 0b_01111111U << 7 * shiftAmount++;
            } while ((currByte & 0b_10000000U) != 0);

            return decodedValue;
        }
        internal static ulong Decode(Stream stream)
        {
            ulong decodedValue = 0;
            int shiftAmount = 0;
            byte currByte;
            do
            {
                currByte = (byte) stream.ReadByte();
                decodedValue |= currByte & 0b_01111111U << 7 * shiftAmount++;
            } while ((currByte & 0b_10000000U) != 0);

            return decodedValue;
        }

        internal static long ZigZagEncode(long value, int k = 64) => (value << 1) ^ (value >> k - 1);
        internal static long ZigZagDecode(long value, int k = 64) => 
            ((((value << k - 1) >> k - 1) ^ value) >> 1) ^ (value & (1L << k - 1));
    }
}