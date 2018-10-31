using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mithril;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;

namespace MithrilTests
{
	[TestClass]
	public class OrderingTest
	{
		private Queue<int> data = new Queue<int>();

		[TestMethod]
		public void TestOrdering()
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

			buffer.Wipe();
			buffer.WriteByte(Common.RELIABLE);
			buffer.WriteByte(2);
			buffer.WriteInt(2);
			socket.SendTo(buffer.Data, buffer.Count, SocketFlags.None, serverEp);

			buffer.Wipe();
			buffer.WriteByte(Common.RELIABLE);
			buffer.WriteByte(1);
			buffer.WriteInt(1);
			socket.SendTo(buffer.Data, buffer.Count, SocketFlags.None, serverEp);

			Thread.Sleep(100);

			Assert.AreEqual(0, data.Dequeue());
			Assert.AreEqual(1, data.Dequeue());
			Assert.AreEqual(2, data.Dequeue());

			buffer.Wipe();
			buffer.WriteByte(Common.RELIABLE);
			buffer.WriteByte(3);
			buffer.WriteInt(3);
			socket.SendTo(buffer.Data, buffer.Count, SocketFlags.None, serverEp);

			buffer.Wipe();
			buffer.WriteByte(Common.RELIABLE);
			buffer.WriteByte(5);
			buffer.WriteInt(5);
			socket.SendTo(buffer.Data, buffer.Count, SocketFlags.None, serverEp);

			Thread.Sleep(2_001);

			Assert.IsTrue(data.Count == 2);
			Assert.AreEqual(3, data.Dequeue());
			Assert.AreEqual(5, data.Dequeue());

			server.Shutdown();
		}

		private void OnReceive(int connectionId, Mithril.Buffer buffer)
		{
			data.Enqueue(buffer.ReadInt());
		}
	}
}
