using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace MithrilTests
{
    [TestClass]
    public class BufferTests
    {
        private readonly Mithril.Buffer buffer;
        private readonly Mithril.Buffer bigBuffer;
        private const int BIG_BUFFER_SIZE = 3000;
        private readonly Random r;

        public BufferTests()
        {
            buffer = new Mithril.Buffer();
            bigBuffer = new Mithril.Buffer(BIG_BUFFER_SIZE);
            r = new Random();
        }

        [TestMethod]
        public void TestInit()
        {
            Assert.AreEqual(Mithril.Buffer.DEFAULT_SIZE, buffer.Size);
            Assert.AreEqual(0, buffer.Count);

            Assert.AreEqual(BIG_BUFFER_SIZE, bigBuffer.Size);
            Assert.AreEqual(0, bigBuffer.Count);
        }

        [TestMethod]
        public void TestClear()
        {
            byte[] expected = new byte[buffer.Size];

            for (int i = 0; i < expected.Length; i++)
            {
                expected[i] = (byte)r.Next(byte.MaxValue);
                buffer.WriteByte(expected[i]);
            }

            Assert.AreEqual(expected.Length, buffer.Count);

            buffer.Reset();
            Assert.AreEqual(0, buffer.Position);
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], buffer.Data[i]);
            }

            buffer.Wipe();
            Assert.AreEqual(0, buffer.Count);
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(0, buffer.Data[i]);
            }
        }

        [TestMethod]
        public void TestPeekBoolean()
        {
			buffer.WriteBoolean(true);
			buffer.WriteBoolean(true);

			buffer.Reset();
			Assert.AreEqual(true, buffer.PeekBoolean());
			Assert.AreEqual(true, buffer.PeekBoolean(1));

			buffer.Wipe();

			buffer.WriteBooleanArray(new bool[] { true, true, true, false, true, true, true });
			buffer.Reset();
			Assert.AreEqual(false, buffer.PeekBoolean(sizeof(byte) + 3));
			Assert.AreEqual(true, buffer.PeekBoolean(sizeof(byte) + 4));

			buffer.Wipe();
        }

        [TestMethod]
        public void TestPeekByte()
        {
			buffer.WriteByte(1);
			buffer.WriteByte(2);

			buffer.Reset();
			Assert.AreEqual(1, buffer.PeekByte());
			Assert.AreEqual(2, buffer.PeekByte(1));

			buffer.Wipe();

			buffer.WriteByteArray(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
			buffer.Reset();
			Assert.AreEqual(4, buffer.PeekByte(sizeof(byte) + 3));
			Assert.AreEqual(5, buffer.PeekByte(sizeof(byte) + 4));

			buffer.Wipe();
		}

        [TestMethod]
        public void TestPeekShort()
        {
			buffer.WriteShort(1);
			buffer.WriteShort(2);

			buffer.Reset();
			Assert.AreEqual(1, buffer.PeekShort());
			Assert.AreEqual(2, buffer.PeekShort(1 * sizeof(short)));

			buffer.Wipe();

			buffer.WriteShortArray(new short[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
			buffer.Reset();
			Assert.AreEqual(4, buffer.PeekShort(sizeof(byte) + 3 * sizeof(short)));
			Assert.AreEqual(5, buffer.PeekShort(sizeof(byte) + 4 * sizeof(short)));

			buffer.Wipe();
		}

        [TestMethod]
        public void TestPeekInt()
        {
			buffer.WriteInt(1);
			buffer.WriteInt(2);

			buffer.Reset();
			Assert.AreEqual(1, buffer.PeekInt());
			Assert.AreEqual(2, buffer.PeekInt(1 * sizeof(int)));

			buffer.Wipe();

			buffer.WriteIntArray(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
			buffer.Reset();
			Assert.AreEqual(4, buffer.PeekInt(sizeof(byte) + 3 * sizeof(int)));
			Assert.AreEqual(5, buffer.PeekInt(sizeof(byte) + 4 * sizeof(int)));

			buffer.Wipe();
		}

        [TestMethod]
        public void TestPeekLong()
        {
			buffer.WriteLong(1);
			buffer.WriteLong(2);

			buffer.Reset();
			Assert.AreEqual(1, buffer.PeekLong());
			Assert.AreEqual(2, buffer.PeekLong(1 * sizeof(long)));

			buffer.Wipe();

			buffer.WriteLongArray(new long[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
			buffer.Reset();
			Assert.AreEqual(4, buffer.PeekLong(sizeof(byte) + 3 * sizeof(long)));
			Assert.AreEqual(5, buffer.PeekLong(sizeof(byte) + 4 * sizeof(long)));

			buffer.Wipe();
		}

        [TestMethod]
        public void TestPeekFloat()
        {
			buffer.WriteFloat(1);
			buffer.WriteFloat(2);

			buffer.Reset();
			Assert.AreEqual(1, buffer.PeekFloat());
			Assert.AreEqual(2, buffer.PeekFloat(1 * sizeof(float)));

			buffer.Wipe();

			buffer.WriteFloatArray(new float[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
			buffer.Reset();
			Assert.AreEqual(4, buffer.PeekFloat(sizeof(byte) + 3 * sizeof(float)));
			Assert.AreEqual(5, buffer.PeekFloat(sizeof(byte) + 4 * sizeof(float)));

			buffer.Wipe();
		}

        [TestMethod]
        public void TestPeekDouble()
        {
			buffer.WriteDouble(1);
			buffer.WriteDouble(2);

			buffer.Reset();
			Assert.AreEqual(1, buffer.PeekDouble());
			Assert.AreEqual(2, buffer.PeekDouble(1 * sizeof(double)));

			buffer.Wipe();

			buffer.WriteDoubleArray(new double[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
			buffer.Reset();
			Assert.AreEqual(4, buffer.PeekDouble(sizeof(byte) + 3 * sizeof(double)));
			Assert.AreEqual(5, buffer.PeekDouble(sizeof(byte) + 4 * sizeof(double)));

			buffer.Wipe();
		}

        [TestMethod]
        public void TestPeekString()
        {
			buffer.WriteString("Test1");
			buffer.WriteString("Test2");

			buffer.Reset();
			Assert.AreEqual("Test1", buffer.PeekString());
			Assert.AreEqual("Test2", buffer.PeekString(sizeof(short) + "Test1".Length));

			buffer.Wipe();
        }

        [TestMethod]
        public void TestBuffer()
        {
            for (int i = 0; i < buffer.Size; i++)
            {
                buffer.WriteByte((byte)r.Next(byte.MaxValue));
            }

            bigBuffer.Write(buffer);
            bigBuffer.Reset();
            buffer.Reset();

            Assert.AreEqual(buffer.Count, bigBuffer.Count);

			int i2 = 0;
            while (bigBuffer.Count > 0)
            {
                Assert.AreEqual(buffer.PeekByte(i2), bigBuffer.ReadByte());
				i2++;
            }

            bigBuffer.Wipe();
            buffer.Reset();

			for (int i = 0; i < 10; i++)
			{
				buffer.ReadByte();
			}

            bigBuffer.Write(buffer, false);
            bigBuffer.Reset();

            Assert.AreEqual(10, buffer.Position);
            Assert.AreEqual(buffer.Count, bigBuffer.Count);

            while (bigBuffer.Count > 0)
            {
                Assert.AreEqual(buffer.ReadByte(), bigBuffer.ReadByte());
            }

			buffer.Wipe();
			bigBuffer.Wipe();
        }

        [TestMethod]
        public void TestBoolean()
        {
            bool[] expectedBools = new bool[buffer.Size];
            byte[] expectedBytes = new byte[buffer.Size];

            for (int i = 0; i < buffer.Size; i++)
            {
                expectedBools[i] = r.Next(2) == 1;
                expectedBytes[i] = (byte)(expectedBools[i] ? 1 : 0);
                buffer.WriteBoolean(expectedBools[i]);
            }

            Assert.IsTrue(buffer.IsFull);
            Assert.AreEqual(Mithril.Buffer.DEFAULT_SIZE, buffer.Count);

            for (int i = 0; i < buffer.Size; i++)
            {
                Assert.AreEqual(expectedBytes[i], buffer.Data[i]);
            }

            buffer.Reset();
            Assert.AreEqual(0, buffer.Position);

            foreach (bool expected in expectedBools)
            {
                Assert.AreEqual(expected, buffer.ReadBoolean());
            }

            Assert.AreEqual(0, buffer.Count);
            Assert.AreEqual(buffer.Size, buffer.Position);

            buffer.Wipe();
            Assert.AreEqual(0, buffer.Count);
        }

        [TestMethod]
        public void TestByte()
        {
            byte[] expected = new byte[buffer.Size];

            byte value;
            for (int i = 0; i < buffer.Size; i++)
            {
                value = (byte)r.Next(256);
                expected[i] = value;
                buffer.WriteByte(value);
            }

            Assert.IsTrue(buffer.IsFull);
            Assert.AreEqual(Mithril.Buffer.DEFAULT_SIZE, buffer.Count);

            for (int i = 0; i < buffer.Size; i++)
            {
                Assert.AreEqual(expected[i], buffer.Data[i]);
            }

            buffer.Reset();
            Assert.AreEqual(0, buffer.Position);

            foreach (byte b in expected)
            {
                Assert.AreEqual(b, buffer.ReadByte());
            }

            Assert.AreEqual(0, buffer.Count);
            Assert.AreEqual(buffer.Size, buffer.Position);

            buffer.Wipe();
            Assert.AreEqual(0, buffer.Count);
        }

        [TestMethod]
        public void TestShort()
        {
            int maxShorts = buffer.Size / sizeof(short);
            int expectedCount = maxShorts * sizeof(short);
            short[] expected = new short[maxShorts];

            for (int i = 0; i < maxShorts; i++)
            {
                expected[i] = (short)r.Next(short.MaxValue);
                buffer.WriteShort(expected[i]);
            }

            if (expectedCount == buffer.Size)
            {
                Assert.IsTrue(buffer.IsFull);
            }

            Assert.AreEqual(expectedCount, buffer.Count);

            buffer.Reset();
            Assert.AreEqual(0, buffer.Position);

            foreach (short s in expected)
            {
                Assert.AreEqual(s, buffer.ReadShort());
            }

            Assert.AreEqual(0, buffer.Count);
            Assert.AreEqual(expectedCount, buffer.Position);

            buffer.Wipe();
            Assert.AreEqual(0, buffer.Count);
        }

        [TestMethod]
        public void TestInt()
        {
            int maxInts = buffer.Size / sizeof(int);
            int expectedCount = maxInts * sizeof(int);
            int[] expected = new int[maxInts];

            for (int i = 0; i < maxInts; i++)
            {
                expected[i] = r.Next(int.MaxValue);
                buffer.WriteInt(expected[i]);
            }

            if (expectedCount == buffer.Size)
            {
                Assert.IsTrue(buffer.IsFull);
            }

            Assert.AreEqual(expectedCount, buffer.Count);

            buffer.Reset();
            Assert.AreEqual(0, buffer.Position);

            foreach (int i in expected)
            {
                Assert.AreEqual(i, buffer.ReadInt());
            }

            Assert.AreEqual(0, buffer.Count);
            Assert.AreEqual(expectedCount, buffer.Position);

            buffer.Wipe();
            Assert.AreEqual(0, buffer.Count);
        }

        [TestMethod]
        public void TestLong()
        {
            int maxLongs = buffer.Size / sizeof(long);
            int expectedCount = maxLongs * sizeof(long);
            long[] expected = new long[maxLongs];

            for (int i = 0; i < maxLongs; i++)
            {
                expected[i] = (long)(r.NextDouble() * long.MaxValue);
                buffer.WriteLong(expected[i]);
            }

            if (expectedCount == buffer.Size)
            {
                Assert.IsTrue(buffer.IsFull);
            }

            Assert.AreEqual(expectedCount, buffer.Count);

            buffer.Reset();
            Assert.AreEqual(0, buffer.Position);

            foreach (long l in expected)
            {
                Assert.AreEqual(l, buffer.ReadLong());
            }

            Assert.AreEqual(0, buffer.Count);
            Assert.AreEqual(expectedCount, buffer.Position);

            buffer.Wipe();
            Assert.AreEqual(0, buffer.Count);
        }

        [TestMethod]
        public void TestFloat()
        {
            int maxFloats = buffer.Size / sizeof(float);
            int expectedCount = maxFloats * sizeof(float);
            float[] expected = new float[maxFloats];

            for (int i = 0; i < maxFloats; i++)
            {
                expected[i] = (float)(r.NextDouble() * float.MaxValue);
                buffer.WriteFloat(expected[i]);
            }

            if (expectedCount == buffer.Size)
            {
                Assert.IsTrue(buffer.IsFull);
            }

            Assert.AreEqual(expectedCount, buffer.Count);

            buffer.Reset();
            Assert.AreEqual(0, buffer.Position);

            foreach (float f in expected)
            {
                Assert.AreEqual(f, buffer.ReadFloat());
            }

            Assert.AreEqual(0, buffer.Count);
            Assert.AreEqual(expectedCount, buffer.Position);

            buffer.Wipe();
            Assert.AreEqual(0, buffer.Count);
        }

        [TestMethod]
        public void TestDouble()
        {
            int maxDoubles = buffer.Size / sizeof(double);
            int expectedCount = maxDoubles * sizeof(double);
            double[] expected = new double[maxDoubles];

            for (int i = 0; i < maxDoubles; i++)
            {
                expected[i] = r.NextDouble() * double.MaxValue;
                buffer.WriteDouble(expected[i]);
            }

            if (expectedCount == buffer.Size)
            {
                Assert.IsTrue(buffer.IsFull);
            }

            Assert.AreEqual(expectedCount, buffer.Count);

            buffer.Reset();
            Assert.AreEqual(0, buffer.Position);

            foreach (double d in expected)
            {
                Assert.AreEqual(d, buffer.ReadDouble());
            }

            Assert.AreEqual(0, buffer.Count);
            Assert.AreEqual(expectedCount, buffer.Position);

            buffer.Wipe();
            Assert.AreEqual(0, buffer.Count);
        }

        [TestMethod]
        public void TestBooleanArray()
        {
            bool[] expectedBools = new bool[byte.MaxValue - 1];
            byte[] expectedBytes = new byte[buffer.Size];

            expectedBytes[0] = (byte)expectedBools.Length;
            for (int i = 0; i < expectedBools.Length; i++)
            {
                expectedBools[i] = r.Next(2) == 1;
                expectedBytes[i + sizeof(byte)] = (byte)(expectedBools[i] ? 1 : 0);
            }

            buffer.WriteBooleanArray(expectedBools);

            Assert.IsFalse(buffer.IsFull);
            Assert.AreEqual(byte.MaxValue, buffer.Count);

            for (int i = 0; i < buffer.Size; i++)
            {
                Assert.AreEqual(expectedBytes[i], buffer.Data[i]);
            }

            buffer.Reset();
            Assert.AreEqual(0, buffer.Position);

            bool[] actualBools = buffer.ReadBooleanArray();

            Assert.AreEqual(expectedBools.Length, actualBools.Length);

            for (int i = 0; i < expectedBools.Length; i++)
            {
                Assert.AreEqual(expectedBools[i], actualBools[i]);
            }

            buffer.Wipe();
            Assert.AreEqual(0, buffer.Count);
        }

        [TestMethod]
        public void TestByteArray()
        {
            byte[] expectedValues = new byte[byte.MaxValue - 1];
            byte[] expectedBytes = new byte[buffer.Size];

            expectedBytes[0] = (byte)expectedValues.Length;
            for (int i = 0; i < expectedValues.Length; i++)
            {
                expectedValues[i] = (byte)r.Next(byte.MaxValue);
                expectedBytes[i + sizeof(byte)] = expectedValues[i];
            }

            buffer.WriteByteArray(expectedValues);

            Assert.IsFalse(buffer.IsFull);
            Assert.AreEqual(byte.MaxValue, buffer.Count);

            for (int i = 0; i < buffer.Size; i++)
            {
                Assert.AreEqual(expectedBytes[i], buffer.Data[i]);
            }

            buffer.Reset();
            Assert.AreEqual(0, buffer.Position);

            byte[] actualBytes = buffer.ReadByteArray();

            Assert.AreEqual(expectedValues.Length, actualBytes.Length);

            for (int i = 0; i < expectedValues.Length; i++)
            {
                Assert.AreEqual(expectedValues[i], actualBytes[i]);
            }

            buffer.Wipe();
            Assert.AreEqual(0, buffer.Count);
        }

        [TestMethod]
        public void TestShortArray()
        {
            short[] expectedValues = new short[byte.MaxValue - 1];

            for (int i = 0; i < expectedValues.Length; i++)
            {
                expectedValues[i] = (short)r.Next(short.MaxValue);
            }

            buffer.WriteShortArray(expectedValues);

            Assert.IsFalse(buffer.IsFull);
            Assert.AreEqual(expectedValues.Length * sizeof(short) + 1, buffer.Count);

            buffer.Reset();
            Assert.AreEqual(0, buffer.Position);

            short[] actualValues = buffer.ReadShortArray();

            Assert.AreEqual(expectedValues.Length, actualValues.Length);

            for (int i = 0; i < expectedValues.Length; i++)
            {
                Assert.AreEqual(expectedValues[i], actualValues[i]);
            }

            buffer.Wipe();
            Assert.AreEqual(0, buffer.Count);
        }

        [TestMethod]
        public void TestIntArray()
        {
            int[] expectedValues = new int[byte.MaxValue - 1];

            for (int i = 0; i < expectedValues.Length; i++)
            {
                expectedValues[i] = r.Next(int.MaxValue);
            }

            buffer.WriteIntArray(expectedValues);

            Assert.IsFalse(buffer.IsFull);
            Assert.AreEqual(expectedValues.Length * sizeof(int) + 1, buffer.Count);

            buffer.Reset();
            Assert.AreEqual(0, buffer.Position);

            int[] actualValues = buffer.ReadIntArray();

            Assert.AreEqual(expectedValues.Length, actualValues.Length);

            for (int i = 0; i < expectedValues.Length; i++)
            {
                Assert.AreEqual(expectedValues[i], actualValues[i]);
            }

            buffer.Wipe();
            Assert.AreEqual(0, buffer.Count);
        }

        [TestMethod]
        public void TestLongArray()
        {
            long[] expectedValues = new long[byte.MaxValue - 1];

            for (int i = 0; i < expectedValues.Length; i++)
            {
                expectedValues[i] = (long)(r.NextDouble() * long.MaxValue);
            }

            bigBuffer.WriteLongArray(expectedValues);

            Assert.IsFalse(bigBuffer.IsFull);
            Assert.AreEqual(expectedValues.Length * sizeof(long) + 1, bigBuffer.Count);

            bigBuffer.Reset();
            Assert.AreEqual(0, bigBuffer.Position);

            long[] actualValues = bigBuffer.ReadLongArray();

            Assert.AreEqual(expectedValues.Length, actualValues.Length);

            for (int i = 0; i < expectedValues.Length; i++)
            {
                Assert.AreEqual(expectedValues[i], actualValues[i]);
            }

            bigBuffer.Wipe();
            Assert.AreEqual(0, bigBuffer.Count);
        }

        [TestMethod]
        public void TestFloatArray()
        {
            float[] expectedValues = new float[byte.MaxValue - 1];

            for (int i = 0; i < expectedValues.Length; i++)
            {
                expectedValues[i] = (float)(r.NextDouble() * float.MaxValue);
            }

            buffer.WriteFloatArray(expectedValues);

            Assert.IsFalse(buffer.IsFull);
            Assert.AreEqual(expectedValues.Length * sizeof(float) + 1, buffer.Count);

            buffer.Reset();
            Assert.AreEqual(0, buffer.Position);

            float[] actualValues = buffer.ReadFloatArray();

            Assert.AreEqual(expectedValues.Length, actualValues.Length);

            for (int i = 0; i < expectedValues.Length; i++)
            {
                Assert.AreEqual(expectedValues[i], actualValues[i]);
            }

            buffer.Wipe();
            Assert.AreEqual(0, buffer.Count);
        }

        [TestMethod]
        public void TestDoubleArray()
        {
            double[] expectedValues = new double[byte.MaxValue - 1];

            for (int i = 0; i < expectedValues.Length; i++)
            {
                expectedValues[i] = r.NextDouble() * double.MaxValue;
            }

            bigBuffer.WriteDoubleArray(expectedValues);

            Assert.IsFalse(bigBuffer.IsFull);
            Assert.AreEqual(expectedValues.Length * sizeof(double) + 1, bigBuffer.Count);

            bigBuffer.Reset();
            Assert.AreEqual(0, bigBuffer.Position);

            double[] actualValues = bigBuffer.ReadDoubleArray();

            Assert.AreEqual(expectedValues.Length, actualValues.Length);

            for (int i = 0; i < expectedValues.Length; i++)
            {
                Assert.AreEqual(expectedValues[i], actualValues[i]);
            }

            bigBuffer.Wipe();
            Assert.AreEqual(0, bigBuffer.Count);
        }
    }
}
