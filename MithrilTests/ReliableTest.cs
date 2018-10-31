using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mithril;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System;

namespace MithrilTests
{
	[TestClass]
	public class ReliableTest
	{
		private HashSet<int> awaitingAck = new HashSet<int>();

		[TestMethod]
		public void TestReliable()
		{
			MithrilServer server = new MithrilServer();
			server.Receive += OnReceive;
			IPEndPoint serverEp = new IPEndPoint(IPAddress.Parse("127.0.0.1"), server.Port);

			Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			socket.Bind(new IPEndPoint(IPAddress.Any, 0));
			EndPoint fromEp = new IPEndPoint(IPAddress.Any, 0);

			Mithril.Buffer buffer = new Mithril.Buffer();
			buffer.WriteByte(Common.ICHI);
			buffer.WriteInt(410);

			socket.SendTo(buffer.Data, buffer.Count, SocketFlags.None, serverEp);
			buffer.Wipe();
			int numBytes = socket.ReceiveFrom(buffer.Data, buffer.Size, SocketFlags.None, ref fromEp);
			buffer.Set(numBytes);

			int responseValue = buffer.ReadInt() + 10;
			buffer.Wipe();
			buffer.WriteByte(Common.SAN);
			buffer.WriteInt(responseValue);
			buffer.WriteInt(59);
			socket.SendTo(buffer.Data, buffer.Count, SocketFlags.None, serverEp);

			buffer.Wipe();
			numBytes = socket.ReceiveFrom(buffer.Data, buffer.Size, SocketFlags.None, ref fromEp);
			buffer.Set(numBytes);

			buffer.Wipe();
			buffer.WriteByte(Common.RELIABLE);
			buffer.WriteByte(0);
			buffer.WriteInt(0);
			socket.SendTo(buffer.Data, buffer.Count, SocketFlags.None, serverEp);
			awaitingAck.Add(0);

			buffer.Wipe();
			buffer.WriteByte(Common.RELIABLE);
			buffer.WriteByte(1);
			buffer.WriteInt(1);
			socket.SendTo(buffer.Data, buffer.Count, SocketFlags.None, serverEp);
			awaitingAck.Add(1);

			buffer.Wipe();
			buffer.WriteByte(Common.RELIABLE);
			buffer.WriteByte(2);
			buffer.WriteInt(2);
			socket.SendTo(buffer.Data, buffer.Count, SocketFlags.None, serverEp);
			awaitingAck.Add(2);

			buffer.Wipe();
			numBytes = socket.ReceiveFrom(buffer.Data, buffer.Size, SocketFlags.None, ref fromEp);
			buffer.Set(numBytes);
			Assert.AreEqual(Common.ACK, buffer.ReadByte());
			Assert.IsTrue(awaitingAck.Remove(buffer.ReadByte()));

			buffer.Wipe();
			numBytes = socket.ReceiveFrom(buffer.Data, buffer.Size, SocketFlags.None, ref fromEp);
			buffer.Set(numBytes);
			Assert.AreEqual(Common.ACK, buffer.ReadByte());
			Assert.IsTrue(awaitingAck.Remove(buffer.ReadByte()));

			buffer.Wipe();
			numBytes = socket.ReceiveFrom(buffer.Data, buffer.Size, SocketFlags.None, ref fromEp);
			buffer.Set(numBytes);
			Assert.AreEqual(Common.ACK, buffer.ReadByte());
			Assert.IsTrue(awaitingAck.Remove(buffer.ReadByte()));

			Assert.IsTrue(awaitingAck.Count == 0);

			buffer.Wipe();
			buffer.WriteInt(69);
			server.SendReliable(1, buffer);

			buffer.Wipe();
			numBytes = socket.ReceiveFrom(buffer.Data, buffer.Size, SocketFlags.None, ref fromEp);
			buffer.Set(numBytes);
			Assert.AreEqual(Common.RELIABLE, buffer.ReadByte());
			byte id = buffer.ReadByte();
			Assert.AreEqual(69, buffer.ReadInt());

			buffer.Wipe();
			buffer.WriteByte(Common.ACK);
			buffer.WriteByte(id);
			socket.SendTo(buffer.Data, buffer.Count, SocketFlags.None, serverEp);

			buffer.Wipe();
			buffer.WriteInt(420);
			server.SendReliable(1, buffer);

			for (int i = 0; i < 10; i++)
			{
				buffer.Wipe();
				numBytes = socket.ReceiveFrom(buffer.Data, buffer.Size, SocketFlags.None, ref fromEp);
				buffer.Set(numBytes);
				Assert.AreEqual(Common.RELIABLE, buffer.ReadByte());
				Assert.AreEqual(2, buffer.ReadByte()); // expected packet id
				Assert.AreEqual(420, buffer.ReadInt());
			}

			buffer.Wipe();
			buffer.WriteInt(1337);
			server.SendReliable(1, buffer);

			buffer.Wipe();
			numBytes = socket.ReceiveFrom(buffer.Data, buffer.Size, SocketFlags.None, ref fromEp);
			buffer.Set(numBytes);
			Assert.AreEqual(Common.RELIABLE, buffer.ReadByte());
			Assert.AreEqual(3, buffer.ReadByte());
			Assert.AreEqual(1337, buffer.ReadInt());

			server.Shutdown();
		}

		private void OnReceive(int connectionId, Mithril.Buffer buffer)
		{
			Console.WriteLine(buffer.ReadInt());
		}
	}
}
