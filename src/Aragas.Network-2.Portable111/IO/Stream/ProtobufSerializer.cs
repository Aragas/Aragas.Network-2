using System;
using System.Text;

using Aragas.Network.Data;

namespace Aragas.Network.IO
{
    /// <summary>
    /// Stream that uses variant for length encoding.
    /// </summary>
    public class ProtobufSerializer : StreamSerializer
    {
        private Encoding Encoding { get; } = Encoding.UTF8;
        protected byte[] _buffer;

        public override byte[] GetBuffer() => _buffer;


        #region Write

        // -- Anything
        public override void Write<T>(in T value = default, bool writeDefaultLength = true)
        {
            var type = value.GetType();

            if (type == typeof (string))
                WriteString(value as string);

            else if (type == typeof (VarShort))
                WriteVarShort((VarShort) (object) value);
            else if (type == typeof (VarZShort))
                WriteVarZShort((VarZShort) (object) value);
            else if (type == typeof (VarInt))
                WriteVarInt((VarInt) (object) value);
            else if (type == typeof (VarZInt))
                WriteVarZInt((VarZInt) (object) value);
            else if (type == typeof (VarLong))
                WriteVarLong((VarLong) (object) value);
            else if (type == typeof (VarZLong[]))
                WriteVarZLong((VarZLong) (object) value);


            else if (type == typeof (bool))
                WriteBoolean((bool) (object) value);

            else if (type == typeof (sbyte))
                WriteSByte((sbyte) (object) value);
            else if (type == typeof (byte))
                WriteUByte((byte) (object) value);

            else if (type == typeof (short))
                WriteShort((short) (object) value);
            else if (type == typeof (ushort))
                WriteUShort((ushort) (object) value);

            else if (type == typeof (int))
                WriteInt((int) (object) value);
            else if (type == typeof (uint))
                WriteUInt((uint) (object) value);

            else if (type == typeof (long))
                WriteLong((long) (object) value);
            else if (type == typeof (ulong))
                WriteULong((ulong) (object) value);

            else if (type == typeof (float))
                WriteFloat((float) (object) value);

            else if (type == typeof (double))
                WriteDouble((double) (object) value);


            else if (ExtendWriteContains(type))
                ExtendWriteExecute(this, in value, writeDefaultLength);


            else if (type == typeof (string[]))
                WriteStringArray(value as string[], writeDefaultLength);
            else if (type == typeof (int[]))
                WriteIntArray(value as int[], writeDefaultLength);
            else if (type == typeof (byte[]))
                WriteByteArray(value as byte[], writeDefaultLength);
            else if (type == typeof (VarShort[]))
                WriteVarShortArray(value as VarShort[], writeDefaultLength);
            else if (type == typeof (VarZShort[]))
                WriteVarZShortArray(value as VarZShort[], writeDefaultLength);
            else if (type == typeof (VarInt[]))
                WriteVarIntArray(value as VarInt[], writeDefaultLength);
            else if (type == typeof (VarZInt[]))
                WriteVarZIntArray(value as VarZInt[], writeDefaultLength);
            else if (type == typeof (VarLong[]))
                WriteVarLongArray(value as VarLong[], writeDefaultLength);
            else if (type == typeof (VarZLong[]))
                WriteVarZLongArray(value as VarZLong[], writeDefaultLength);
        }

        // -- String
        private void WriteString(string value, int length = 0)
        {
            if (length == 0)
                length = value.Length;

            var lengthBytes = new VarInt(value.Length).Encode();
            var final = new byte[value.Length + lengthBytes.Length];
            

            Buffer.BlockCopy(lengthBytes, 0, final, 0, lengthBytes.Length);
            Buffer.BlockCopy(Encoding.GetBytes(value), 0, final, lengthBytes.Length, length);

            ToBuffer(final);
        }

        // -- Variants
        private void WriteVarShort(in VarShort value) { ToBuffer(value.Encode()); }
        private void WriteVarZShort(in VarZShort value) { ToBuffer(value.Encode()); }

        private void WriteVarInt(in VarInt value) { ToBuffer(value.Encode()); }
        private void WriteVarZInt(in VarZInt value) { ToBuffer(value.Encode()); }

        private void WriteVarLong(in VarLong value) { ToBuffer(value.Encode()); }
        private void WriteVarZLong(in VarZLong value) { ToBuffer(value.Encode()); }

        // -- Boolean
        private void WriteBoolean(bool value) { WriteUByte(Convert.ToByte(value)); }

        // -- SByte & Byte
        private void WriteSByte(sbyte value) { WriteUByte(unchecked((byte) value)); }
        private void WriteUByte(byte value) { ToBuffer(new[] { value }); }

        // -- Short & UShort
        private void WriteShort(short value)
        {
            var bytes = BitConverter.GetBytes(value);
            Array.Reverse(bytes);

            ToBuffer(bytes);
        }
        private void WriteUShort(ushort value)
        {
            ToBuffer(new[]
            {
                (byte) ((value & 0xFF00) >> 8),
                (byte) ((value & 0xFF))
            });
        }

        // -- Int & UInt
        private void WriteInt(int value)
        {
            var bytes = BitConverter.GetBytes(value);
            Array.Reverse(bytes);

            ToBuffer(bytes);
        }
        private void WriteUInt(uint value)
        {
            ToBuffer(new[]
            {
                (byte) ((value & 0xFF000000) >> 24),
                (byte) ((value & 0xFF0000) >> 16),
                (byte) ((value & 0xFF00) >> 8),
                (byte) ((value & 0xFF))
            });
        }

        // -- Long & ULong
        private void WriteLong(long value)
        {
            var bytes = BitConverter.GetBytes(value);
            Array.Reverse(bytes);

            ToBuffer(bytes);
        }
        private void WriteULong(ulong value)
        {
            ToBuffer(new[]
            {
                (byte) ((value & 0xFF00000000000000) >> 56),
                (byte) ((value & 0xFF000000000000) >> 48),
                (byte) ((value & 0xFF0000000000) >> 40),
                (byte) ((value & 0xFF00000000) >> 32),
                (byte) ((value & 0xFF000000) >> 24),
                (byte) ((value & 0xFF0000) >> 16),
                (byte) ((value & 0xFF00) >> 8),
                (byte) ((value & 0xFF))
            });
        }

        // -- Float
        private void WriteFloat(float value)
        {
            var bytes = BitConverter.GetBytes(value);
            Array.Reverse(bytes);

            ToBuffer(bytes);
        }

        // -- Double
        private void WriteDouble(double value)
        {
            var bytes = BitConverter.GetBytes(value);
            Array.Reverse(bytes);

            ToBuffer(bytes);
        }

        // -- StringArray
        private void WriteStringArray(string[] value, bool writeDefaultLength)
        {
            if(writeDefaultLength)
                WriteVarInt(new VarInt(value.Length));

            for (var i = 0; i < value.Length; i++)
                WriteString(value[i]);
        }

        // -- Variable Array
        private void WriteVarShortArray(in VarShort[] value, bool writeDefaultLength)
        {
            if (writeDefaultLength)
                WriteVarInt(new VarInt(value.Length));

            for (var i = 0; i < value.Length; i++)
                WriteVarShort(in value[i]);
        }
        private void WriteVarZShortArray(in VarZShort[] value, bool writeDefaultLength)
        {
            if (writeDefaultLength)
                WriteVarInt(new VarInt(value.Length));

            for (var i = 0; i < value.Length; i++)
                WriteVarZShort(in value[i]);
        }

        private void WriteVarIntArray(in VarInt[] value, bool writeDefaultLength)
        {
            if (writeDefaultLength)
                WriteVarInt(new VarInt(value.Length));

            for (var i = 0; i < value.Length; i++)
                WriteVarInt(in value[i]);
        }
        private void WriteVarZIntArray(in VarZInt[] value, bool writeDefaultLength)
        {
            if (writeDefaultLength)
                WriteVarInt(new VarInt(value.Length));

            for (var i = 0; i < value.Length; i++)
                WriteVarZInt(in value[i]);
        }

        private void WriteVarLongArray(in VarLong[] value, bool writeDefaultLength)
        {
            if (writeDefaultLength)
                WriteVarInt(new VarInt(value.Length));

            for (var i = 0; i < value.Length; i++)
                WriteVarLong(in value[i]);
        }
        private void WriteVarZLongArray(in VarZLong[] value, bool writeDefaultLength)
        {
            if (writeDefaultLength)
                WriteVarInt(new VarInt(value.Length));

            for (var i = 0; i < value.Length; i++)
                WriteVarZLong(in value[i]);
        }

        // -- IntArray
        private void WriteIntArray(int[] value, bool writeDefaultLength)
        {
            if (writeDefaultLength)
                WriteVarInt(new VarInt(value.Length));

            for (var i = 0; i < value.Length; i++)
                WriteInt(value[i]);
        }

        // -- ByteArray
        private void WriteByteArray(byte[] value, bool writeDefaultLength)
        {
            if (writeDefaultLength)
                WriteVarInt(new VarInt(value.Length));

            ToBuffer(value);
        }


        private void ToBuffer(byte[] value)
        {
            if (_buffer != null)
            {
                Array.Resize(ref _buffer, _buffer.Length + value.Length);
                Array.Copy(value, 0, _buffer, _buffer.Length - value.Length, value.Length);
            }
            else
                _buffer = value;
        }

        #endregion Write


        public override void Dispose()
        {
            _buffer = null;
        }
    }
}