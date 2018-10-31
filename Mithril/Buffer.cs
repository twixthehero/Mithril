using System;
using System.Diagnostics;
using System.Text;

namespace Mithril
{
    public class Buffer
    {
        public const int DEFAULT_SIZE = 1450;

        private const int BYTE_SIZE = sizeof(byte);
        private const int SHORT_SIZE = sizeof(short);
        private const int INT_SIZE = sizeof(int);
        private const int LONG_SIZE = sizeof(long);
        private const int FLOAT_SIZE = sizeof(float);
        private const int DOUBLE_SIZE = sizeof(double);

        /// <summary>
        /// Number of bytes in the Buffer
        /// </summary>
        public int Count { get; private set; } = 0;

        /// <summary>
        /// Buffer's byte array
        /// </summary>
        public byte[] Data { get; private set; }

        /// <summary>
        /// Checks if the Buffer is full
        /// </summary>
        public bool IsFull { get { return Count >= Size; } }

        public int Position { get { return index; } }

        /// <summary>
        /// Max number of bytes in this Buffer
        /// </summary>
        public int Size { get; private set; }

        private int index = 0;

        public Buffer() : this(DEFAULT_SIZE) { }
        public Buffer(int size) : this(new byte[size]) { }

        public Buffer(byte[] data)
        {
            Data = data;
            Size = Data.Length;
        }

        /// <summary>
        /// Sets the Buffer index to 0
        /// </summary>
        public void Reset()
        {
            index = 0;
        }

        /// <summary>
        /// Sets Count to the value of count
        /// </summary>
        public void Set(int count)
        {
            Debug.Assert(count < Size, $"Tried to set Count > Size! {count} > {Size}");

            Count = count;
        }

        /// <summary>
        /// Sets the Buffer index to 0 and writes 0 to the entire Buffer
        /// </summary>
        public void Wipe()
        {
            index = 0;
            Count = 0;

            for (int i = 0; i < Data.Length; i++)
                Data[i] = 0;
        }

        public byte this[int index]
        {
            get { return Data[index]; }
        }
		
		public void Write(Buffer buffer, bool wholeBuffer = true)
		{
			if (wholeBuffer)
			{
				Array.Copy(buffer.Data, 0, Data, index, buffer.Count);
				index += buffer.Count;
				Count += buffer.Count;
			}
			else
			{
				Array.Copy(buffer.Data, buffer.Position, Data, index, buffer.Count);
				index += buffer.Count;
				Count += buffer.Count;
			}
		}

        #region Peek

        public bool PeekBoolean(int offset = 0)
        {
            Debug.Assert(index + offset < Size, "Tried to read past end of buffer!");

            return Data[index + offset] == 1;
        }

        public byte PeekByte(int offset = 0)
        {
            Debug.Assert(index + offset < Size, "Tried to read past end of buffer!");

            return Data[index + offset];
        }

        public short PeekShort(int offset = 0)
        {
            Debug.Assert(index + SHORT_SIZE + offset <= Size, "Tried to read past end of buffer!");

            short value = BitConverter.ToInt16(Data, index + offset);

            return value;
        }

        public int PeekInt(int offset = 0)
        {
            Debug.Assert(index + INT_SIZE + offset <= Size, "Tried to read past end of buffer!");

            int value = BitConverter.ToInt32(Data, index + offset);

            return value;
        }

        public long PeekLong(int offset = 0)
        {
            Debug.Assert(index + LONG_SIZE + offset <= Size, "Tried to read past end of buffer!");

            long value = BitConverter.ToInt64(Data, index + offset);

            return value;
        }

        public float PeekFloat(int offset = 0)
        {
            Debug.Assert(index + FLOAT_SIZE + offset <= Size, "Tried to read past end of buffer!");

            float value = BitConverter.ToSingle(Data, index + offset);

            return value;
        }

        public double PeekDouble(int offset = 0)
        {
            Debug.Assert(index + DOUBLE_SIZE + offset <= Size, "Tried to read past end of buffer!");

            double value = BitConverter.ToDouble(Data, index + offset);

            return value;
        }

        public string PeekString(int offset = 0)
        {
            short length = PeekShort(offset);
            string value = Encoding.UTF8.GetString(Data, index + SHORT_SIZE + offset, length);

            return value;
        }

        #endregion

        #region Read

        public bool ReadBoolean()
        {
            Debug.Assert(index < Size, "Tried to read past end of buffer!");

            Count--;
            return Data[index++] == 1;
        }

        private bool ReadBooleanNoAssert()
        {
            Count--;
            return Data[index++] == 1;
        }

        public byte ReadByte()
        {
            Debug.Assert(index < Size, "Tried to read past end of buffer!");

            Count--;
            return Data[index++];
        }

        private byte ReadByteNoAssert()
        {
            Count--;
            return Data[index++];
        }

        public short ReadShort()
        {
            Debug.Assert(index + SHORT_SIZE <= Size, "Tried to read past end of buffer!");

            short value = BitConverter.ToInt16(Data, index);
            index += SHORT_SIZE;
            Count -= SHORT_SIZE;

            return value;
        }

        private short ReadShortNoAssert()
        {
            short value = BitConverter.ToInt16(Data, index);
            index += SHORT_SIZE;
            Count -= SHORT_SIZE;

            return value;
        }

        public int ReadInt()
        {
            Debug.Assert(index + INT_SIZE <= Size, "Tried to read past end of buffer!");

            int value = BitConverter.ToInt32(Data, index);
            index += INT_SIZE;
            Count -= INT_SIZE;

            return value;
        }

        private int ReadIntNoAssert()
        {
            int value = BitConverter.ToInt32(Data, index);
            index += INT_SIZE;
            Count -= INT_SIZE;

            return value;
        }

        public long ReadLong()
        {
            Debug.Assert(index + LONG_SIZE <= Size, "Tried to read past end of buffer!");

            long value = BitConverter.ToInt64(Data, index);
            index += LONG_SIZE;
            Count -= LONG_SIZE;

            return value;
        }

        private long ReadLongNoAssert()
        {
            long value = BitConverter.ToInt64(Data, index);
            index += LONG_SIZE;
            Count -= LONG_SIZE;

            return value;
        }

        public float ReadFloat()
        {
            Debug.Assert(index + FLOAT_SIZE <= Size, "Tried to read past end of buffer!");

            float value = BitConverter.ToSingle(Data, index);
            index += FLOAT_SIZE;
            Count -= FLOAT_SIZE;

            return value;
        }

        private float ReadFloatNoAssert()
        {
            float value = BitConverter.ToSingle(Data, index);
            index += FLOAT_SIZE;
            Count -= FLOAT_SIZE;

            return value;
        }

        public double ReadDouble()
        {
            Debug.Assert(index + DOUBLE_SIZE <= Size, "Tried to read past end of buffer!");

            double value = BitConverter.ToDouble(Data, index);
            index += DOUBLE_SIZE;
            Count -= DOUBLE_SIZE;

            return value;
        }

        private double ReadDoubleNoAssert()
        {
            double value = BitConverter.ToDouble(Data, index);
            index += DOUBLE_SIZE;
            Count -= DOUBLE_SIZE;

            return value;
        }

        public string ReadString()
        {
            short length = ReadShort();
            string value = Encoding.UTF8.GetString(Data, index, length);
            index += length;
            Count -= length;

            return value;
        }

        private string ReadStringNoAssert()
        {
            short length = ReadShortNoAssert();
            string value = Encoding.UTF8.GetString(Data, index, length);
            index += length;
            Count -= length;

            return value;
        }

        #endregion

        #region Read Arrays

        public bool[] ReadBooleanArray()
        {
            bool[] values = new bool[ReadByte()];

            for (int i = 0; i < values.Length; i++)
                values[i] = ReadBoolean();

            return values;
        }

        public byte[] ReadByteArray()
        {
            byte[] values = new byte[ReadByte()];

            for (int i = 0; i < values.Length; i++)
                values[i] = ReadByte();

            return values;
        }

        public short[] ReadShortArray()
        {
            short[] values = new short[ReadByte()];

            for (int i = 0; i < values.Length; i++)
                values[i] = ReadShort();

            return values;
        }

        public int[] ReadIntArray()
        {
            int[] values = new int[ReadByte()];

            for (int i = 0; i < values.Length; i++)
                values[i] = ReadInt();

            return values;
        }

        public long[] ReadLongArray()
        {
            long[] values = new long[ReadByte()];

            for (int i = 0; i < values.Length; i++)
                values[i] = ReadLong();

            return values;
        }

        public float[] ReadFloatArray()
        {
            float[] values = new float[ReadByte()];

            for (int i = 0; i < values.Length; i++)
                values[i] = ReadFloat();

            return values;
        }

        public double[] ReadDoubleArray()
        {
            double[] values = new double[ReadByte()];

            for (int i = 0; i < values.Length; i++)
                values[i] = ReadDouble();

            return values;
        }

        public string[] ReadStringArray()
        {
            string[] values = new string[ReadByte()];

            for (int i = 0; i < values.Length; i++)
                values[i] = ReadString();

            return values;
        }

        #endregion

        #region Write

        public void WriteBoolean(bool value)
        {
            Debug.Assert(index < Size, "Tried to write past end of buffer!");

            Data[index++] = value ? (byte)1 : (byte)0;
            Count++;
        }

        public void WriteByte(byte value)
        {
            Debug.Assert(index < Size, "Tried to write past end of buffer!");

            Data[index++] = value;
            Count++;
        }

        public unsafe void WriteShort(short value)
        {
            Debug.Assert(index + SHORT_SIZE <= Size, $"Tried to write past end of buffer!");

            void* ptr = &value;
            for (int i = 0; i < SHORT_SIZE; i++)
                Data[index++] = (byte)(*(ulong*)ptr >> (i * 8));

            Count += SHORT_SIZE;
        }

        public unsafe void WriteInt(int value)
        {
            Debug.Assert(index + INT_SIZE <= Size, "Tried to write past end of buffer!");

            void* ptr = &value;
            for (int i = 0; i < INT_SIZE; i++)
                Data[index++] = (byte)(*(ulong*)ptr >> (i * 8));

            Count += INT_SIZE;
        }

        public unsafe void WriteLong(long value)
        {
            Debug.Assert(index + LONG_SIZE <= Size, "Tried to write past end of buffer!");

            void* ptr = &value;
            for (int i = 0; i < LONG_SIZE; i++)
                Data[index++] = (byte)(*(ulong*)ptr >> (i * 8));

            Count += LONG_SIZE;
        }

        public unsafe void WriteFloat(float value)
        {
            Debug.Assert(index + FLOAT_SIZE <= Size, "Tried to write past end of buffer!");

            void* ptr = &value;
            for (int i = 0; i < FLOAT_SIZE; i++)
                Data[index++] = (byte)(*(ulong*)ptr >> (i * 8));

            Count += FLOAT_SIZE;
        }

        public unsafe void WriteDouble(double value)
        {
            Debug.Assert(index + DOUBLE_SIZE <= Size, "Tried to write past end of buffer!");

            void* ptr = &value;
            for (int i = 0; i < DOUBLE_SIZE; i++)
                Data[index++] = (byte)(*(ulong*)ptr >> (i * 8));

            Count += DOUBLE_SIZE;
        }

        public void WriteString(string value)
        {
            WriteShort((short)value.Length);

            byte[] byteData = Encoding.UTF8.GetBytes(value);

            Debug.Assert(index + byteData.Length <= Size, "String too long to write to buffer!");

            for (int i = 0; i < byteData.Length; i++)
                Data[index++] = byteData[i];

            Count += byteData.Length;
        }

        #endregion

        #region Write Arrays

        public void WriteBooleanArray(bool[] values)
        {
            Debug.Assert(values.Length < byte.MaxValue, $"Array is too big! Length must be < {byte.MaxValue}");
            Debug.Assert(index + values.Length + BYTE_SIZE <= Size, $"Too much data! Must be under {Size} bytes! ({index + values.Length + BYTE_SIZE})");

            WriteByte((byte)values.Length);

            foreach (bool value in values)
                WriteBoolean(value);
        }

        public void WriteByteArray(byte[] values)
        {
            Debug.Assert(values.Length < byte.MaxValue, $"Array is too big! Length must be < {byte.MaxValue}");
            Debug.Assert(index + values.Length + BYTE_SIZE <= Size, $"Too much data! Must be under {Size} bytes! ({index + values.Length + BYTE_SIZE})");

            WriteByte((byte)values.Length);

            foreach (byte value in values)
                WriteByte(value);
        }

        public void WriteShortArray(short[] values)
        {
            Debug.Assert(values.Length < byte.MaxValue, $"Array is too big! Length must be < {byte.MaxValue}");
            Debug.Assert(index + values.Length * SHORT_SIZE + BYTE_SIZE <= Size, $"Too much data! Must be under {Size} bytes! ({index + values.Length * SHORT_SIZE + BYTE_SIZE})");

            WriteByte((byte)values.Length);

            foreach (short value in values)
                WriteShort(value);
        }

        public void WriteIntArray(int[] values)
        {
            Debug.Assert(values.Length < byte.MaxValue, $"Array is too big! Length must be < {byte.MaxValue}");
            Debug.Assert(index + values.Length * INT_SIZE + BYTE_SIZE <= Size, $"Too much data! Must be under {Size} bytes! ({index + values.Length * INT_SIZE + BYTE_SIZE})");

            WriteByte((byte)values.Length);

            foreach (int value in values)
                WriteInt(value);
        }

        public void WriteLongArray(long[] values)
        {
            Debug.Assert(values.Length < byte.MaxValue, $"Array is too big! Length must be < {byte.MaxValue}");
            Debug.Assert(index + values.Length * LONG_SIZE + BYTE_SIZE <= Size, $"Too much data! Must be under {Size} bytes! ({index + values.Length * LONG_SIZE + BYTE_SIZE})");

            WriteByte((byte)values.Length);

            foreach (long value in values)
                WriteLong(value);
        }

        public void WriteFloatArray(float[] values)
        {
            Debug.Assert(values.Length < byte.MaxValue, $"Array is too big! Length must be < {byte.MaxValue}");
            Debug.Assert(index + values.Length * FLOAT_SIZE + BYTE_SIZE <= Size, $"Too much data! Must be under {Size} bytes! ({index + values.Length * FLOAT_SIZE + BYTE_SIZE})");

            WriteByte((byte)values.Length);

            foreach (float value in values)
                WriteFloat(value);
        }

        public void WriteDoubleArray(double[] values)
        {
            Debug.Assert(values.Length < byte.MaxValue, $"Array is too big! Length must be < {byte.MaxValue}");
            Debug.Assert(index + values.Length * DOUBLE_SIZE + BYTE_SIZE <= Size, $"Too much data! Must be under {Size} bytes! ({index + values.Length * DOUBLE_SIZE + BYTE_SIZE})");

            WriteByte((byte)values.Length);

            foreach (double value in values)
                WriteDouble(value);
        }

        #endregion

		public string GetContent()
		{
			StringBuilder s = new StringBuilder("Buffer");
			for (int i = 0; i < Count + Position; i++)
			{
				s.Append($"[{Data[i]}]");
			}

			return s.ToString();
		}

		public override string ToString()
		{
			return $"Buffer[Count={Count},Position={Position},Data={GetContent()}]";
		}
	}
}
