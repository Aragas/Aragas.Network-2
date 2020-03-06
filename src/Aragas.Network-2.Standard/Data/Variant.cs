using System;
using System.Buffers;
using System.IO;
using System.Runtime.CompilerServices;

namespace Aragas.Network.Data
{
    public interface IVariant
    {
        protected static int VariantSize(ulong value)
        {
            var outputSize = 0;
            do
            {
                value >>= 7;
                outputSize++;
            } while (value != 0);
            return outputSize;
        }
        /// <summary>
        /// This will be used for Int8, Int16 and Int32
        /// </summary>
        protected static int VariantSize(int value)
        {
            var unsignedValue = (uint) value;
            var outputSize = 0;
            do
            {
                unsignedValue >>= 7;
                outputSize++;
            } while (unsignedValue != 0);
            return outputSize;
        }
        /// <summary>
        /// This will be used for Int64
        /// </summary>
        protected static int VariantSize(long value)
        {
            var unsignedValue = (ulong) value;
            var outputSize = 0;
            do
            {
                unsignedValue >>= 7;
                outputSize++;
            } while (unsignedValue != 0);
            return outputSize;
        }

        protected static Span<byte> Encode(in long value) => Encode((ulong) value);
        protected static int Encode(in Span<byte> buffer, ulong value)
        {
            var size = VariantSize(value);
            if (buffer.Length < size)
                return -1;

            for (var i = 0; i < size - 1; i++)
            {
                buffer[i] = (byte) ((value & 127) | 128u);
                value >>= 7;
            }
            buffer[size - 1] = (byte) value;

            return size;
        }
        protected static Span<byte> Encode(ulong value)
        {
            var size = VariantSize(value);
            Span<byte> buffer = new byte[size];

            for (var i = 0; i < buffer.Length - 1; i++)
            {
                buffer[i] = (byte) ((value & 127) | 128u);
                value >>= 7;
            }
            buffer[size - 1] = (byte) value;

            return buffer;
        }
        protected static int Encode(byte[] buffer, int offset, ulong value)
        {
            var size = VariantSize(value);

            for (var i = 0; i < size - 1; i++)
            {
                buffer[offset + i] = (byte)((value & 127) | 128u);
                value >>= 7;
            }
            buffer[offset + size - 1] = (byte) value;

            return size;
        }
        protected static int Encode(Stream stream, ulong value)
        {
            var size = VariantSize(value);

            for (var i = 0; i < size - 1; i++)
            {
                stream.WriteByte((byte) ((value & 127) | 128u));
                value >>= 7;
            }
            stream.WriteByte((byte) value);

            return size;
        }

        protected static ulong? Decode(in ReadOnlySequence<byte> sequence)
        {
            if(sequence.IsSingleSegment)
                return Decode(sequence.First.Span);

            int sequenceIndex = 0;
            ulong decodedValue = 0;
            foreach (var memory in sequence)
            {
                sequenceIndex += memory.Length;

                var span = memory.Span;
                int index = 0, shiftAmount = 0;
                byte currByte = 0;
                do
                {
                    if (index >= span.Length)
                    {
                        if(sequence.End.Equals(sequence.GetPosition(sequenceIndex)))
                        {
                            return null; // End of sequence, data not sufficient
                        }

                        continue; // Continue reading from next sequence
                    }

                    currByte = span[index++];
                    ulong lowByte = currByte & 127u;
                    decodedValue |= lowByte << shiftAmount++ * 7;
                } while ((currByte & 128u) != 0);
            }
            return decodedValue;
        }
        protected static ulong Decode(in ReadOnlySpan<byte> buffer)
        {
            ulong decodedValue = 0;
            int index = 0, shiftAmount = 0;
            byte currByte;
            do
            {
                currByte = buffer[index++];
                ulong lowByte = currByte & 127u;
                decodedValue |= lowByte << shiftAmount++ * 7;
            } while ((currByte & 128u) != 0);

            return decodedValue;
        }
        protected static ulong Decode(byte[] buffer, int offset)
        {
            ulong decodedValue = 0;
            int index = 0, shiftAmount = 0;
            byte currByte;
            do
            {
                currByte = buffer[offset + index++];
                ulong lowByte = currByte & 127u;
                decodedValue |= lowByte << shiftAmount++ * 7;
            } while ((currByte & 128u) != 0);

            return decodedValue;
        }
        protected static ulong Decode(Stream stream)
        {
            int iterations = 0;
            ulong decodedValue = 0;
            int shiftAmount = 0;
            byte currByte;
            do
            {
                currByte = (byte) stream.ReadByte();
                ulong lowByte = currByte & 127u;
                decodedValue |= lowByte << shiftAmount++ * 7;

                if (iterations > 10)
                    throw new Exception();
            } while ((currByte & 128u) != 0);

            return decodedValue;
        }

        protected static ulong ZigZagEncode(long value)
        {
            var signed = (value << 1) ^ (value >> 63);
            return Unsafe.As<long, ulong>(ref signed);
        }

        protected static long ZigZagDecode(ulong value)
        {
            var unsigned = Unsafe.As<ulong, long>(ref value);
            //if ((value & 0x1) == 0x1)
            //    return (-1 * ((unsigned >> 1) + 1));
            //return (unsigned >> 1);

            var temp = (((unsigned << 63) >> 63) ^ unsigned) >> 1;
            return temp ^ (unsigned & (1L << 63));
        }

        int Size { get; }
        Span<byte> Encode();
    }
}