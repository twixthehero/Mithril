using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mithril;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace MithrilTests
{
	[TestClass]
	public class HandshakeTests
	{
		[TestMethod]
		public void TestInitiate()
		{
			MithrilClient client = new MithrilClient();

			Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			Buffer buffer = new Buffer();

			socket.Bind(new IPEndPoint(IPAddress.Any, 0));
			int serverPort = ((IPEndPoint)socket.LocalEndPoint).Port;
			EndPoint fromEp = new IPEndPoint(IPAddress.Any, 0);

			int connectionId = client.Connect("127.0.0.1", serverPort);
			Assert.AreNotEqual(-1, connectionId);

			int numBytes = socket.ReceiveFrom(buffer.Data, buffer.Size, SocketFlags.None, ref fromEp);
			buffer.Set(numBytes);

			Assert.AreEqual(client.Port, ((IPEndPoint)fromEp).Port);
			Assert.AreEqual(Common.ICHI, buffer.ReadByte());
			int responseValue = buffer.ReadInt() + 10;

			buffer.Wipe();
			buffer.WriteByte(Common.NI);
			buffer.WriteInt(responseValue);
			buffer.WriteInt(1327);
			socket.SendTo(buffer.Data, buffer.Count, SocketFlags.None, fromEp);

			buffer.Wipe();
			numBytes = socket.ReceiveFrom(buffer.Data, buffer.Size, SocketFlags.None, ref fromEp);
			buffer.Set(numBytes);

			Assert.AreEqual(client.Port, ((IPEndPoint)fromEp).Port);
			Assert.AreEqual(Common.SAN, buffer.ReadByte());
			Assert.AreEqual(1337, buffer.ReadInt());

			responseValue = buffer.ReadInt() + 10;
			buffer.Wipe();
			buffer.WriteByte(Common.YON);
			buffer.WriteInt(responseValue);
			int numSent = socket.SendTo(buffer.Data, buffer.Count, SocketFlags.None, fromEp);

			Thread.Sleep(1);

			Assert.IsTrue(client.IsConnected(connectionId));
			Assert.IsTrue(client.IsConnected());

			client.Shutdown();

			buffer.Wipe();
			numBytes = socket.ReceiveFrom(buffer.Data, buffer.Size, SocketFlags.None, ref fromEp);
			buffer.Set(numBytes);

			Assert.AreEqual(client.Port, ((IPEndPoint)fromEp).Port);
			Assert.AreEqual(Common.DISCONNECT, buffer.ReadByte());
			Assert.AreEqual(Common.R_DISCONNECT, buffer.ReadByte());
		}

		[TestMethod]
		public void TestReceive()
		{
			MithrilServer server = new MithrilServer();
			IPEndPoint serverEp = new IPEndPoint(IPAddress.Parse("127.0.0.1"), server.Port);

			Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			socket.Bind(new IPEndPoint(IPAddress.Any, 0));
			EndPoint fromEp = new IPEndPoint(IPAddress.Any, 0);

			Buffer buffer = new Buffer();
			buffer.WriteByte(Common.ICHI);
			buffer.WriteInt(410);

			socket.SendTo(buffer.Data, buffer.Count, SocketFlags.None, serverEp);
			buffer.Wipe();
			int numBytes = socket.ReceiveFrom(buffer.Data, buffer.Size, SocketFlags.None, ref fromEp);
			buffer.Set(numBytes);

			Assert.AreEqual(server.Port, ((IPEndPoint)fromEp).Port);
			Assert.AreEqual(Common.NI, buffer.ReadByte());
			Assert.AreEqual(420, buffer.ReadInt());

			int responseValue = buffer.ReadInt() + 10;
			buffer.Wipe();
			buffer.WriteByte(Common.SAN);
			buffer.WriteInt(responseValue);
			buffer.WriteInt(59);
			socket.SendTo(buffer.Data, buffer.Count, SocketFlags.None, serverEp);

			buffer.Wipe();
			numBytes = socket.ReceiveFrom(buffer.Data, buffer.Size, SocketFlags.None, ref fromEp);
			buffer.Set(numBytes);

			Assert.AreEqual(server.Port, ((IPEndPoint)fromEp).Port);
			Assert.AreEqual(Common.YON, buffer.ReadByte());
			Assert.AreEqual(69, buffer.ReadInt());

			Assert.IsTrue(server.IsConnected());

			server.Shutdown();

			buffer.Wipe();
			numBytes = socket.ReceiveFrom(buffer.Data, buffer.Size, SocketFlags.None, ref fromEp);
			buffer.Set(numBytes);

			Assert.AreEqual(server.Port, ((IPEndPoint)fromEp).Port);
			Assert.AreEqual(Common.DISCONNECT, buffer.ReadByte());
			Assert.AreEqual(Common.R_DISCONNECT, buffer.ReadByte());
		}
	}
}
