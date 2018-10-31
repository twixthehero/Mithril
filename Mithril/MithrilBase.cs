using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Mithril
{
	public abstract class MithrilBase : IMithrilBase
	{
		public IPEndPoint EndPoint { get; private set; }
		public int Port { get { return EndPoint?.Port ?? -1; } }

		protected Socket Socket { get; private set; }
		protected Thread Thread { get; private set; }

		private int nextConnectionId = 0;
		protected Dictionary<int, Connection> Connections { get; } = new Dictionary<int, Connection>();
		protected Dictionary<EndPoint, int> EndPointIds { get; } = new Dictionary<EndPoint, int>();

		protected bool IsReceiving { get; set; }
		protected ManualResetEvent Handle { get; private set; } = new ManualResetEvent(false);

		public delegate void OnNewConnection(int connectionId);
		public event OnNewConnection NewConnection;

		public delegate void OnReceive(int connectionId, Mithril.Buffer buffer);
		public event OnReceive Receive;

		public delegate void OnDisconnect(int connectionId, byte reason);
		public event OnDisconnect Disconnect;

		public MithrilBase()
		{
			Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			Socket.Bind(new IPEndPoint(IPAddress.Any, 0));
			EndPoint = (IPEndPoint)Socket.LocalEndPoint;

			Thread = new Thread(new ThreadStart(ReceiveThread));
		}

		public bool IsConnected(int connectionId = -1)
		{
			if (connectionId == -1)
			{
				foreach (Connection connection in Connections.Values)
				{
					if (connection.Connected)
					{
						return true;
					}
				}

				return false;
			}
			else
			{
				return Connections[connectionId]?.Connected ?? false;
			}
		}

		protected int GetNextConnectionId()
		{
			do
			{
				nextConnectionId++;
			} while (Connections.ContainsKey(nextConnectionId));

			return nextConnectionId;
		}

		public void Send(int connectionId, Mithril.Buffer buffer, bool wholeBuffer = true)
		{
			Connections[connectionId]?.SendUnreliable(buffer, wholeBuffer);
		}

		public void SendReliable(int connectionId, Mithril.Buffer buffer, bool wholeBuffer = true)
		{
			Connections[connectionId]?.SendReliable(buffer, wholeBuffer);
		}

		public void ReceiveThread()
		{
			IsReceiving = true;

			while (IsReceiving)
			{
				Handle.Reset();
				SocketAsyncEventArgs saea = new SocketAsyncEventArgs()
				{
					RemoteEndPoint = new IPEndPoint(IPAddress.Any, 0)
				};
				saea.Completed += ReceiveAsync;

				byte[] data = new byte[Mithril.Buffer.DEFAULT_SIZE];
				saea.SetBuffer(data, 0, data.Length);

				if (Socket.ReceiveFromAsync(saea))
				{
					Handle.WaitOne();
				}
				else
				{
					ReceiveAsync(null, saea);
				}
			}
		}

		protected virtual void ReceiveAsync(object sender, SocketAsyncEventArgs e)
		{
			if (e.BytesTransferred > 0)
			{
				Mithril.Buffer buffer = new Mithril.Buffer(e.Buffer);
				buffer.Set(e.BytesTransferred);

				if (EndPointIds.ContainsKey(e.RemoteEndPoint))
				{
					Connections[EndPointIds[e.RemoteEndPoint]].OnReceive(buffer);
				}
			}

			Handle.Set();
		}

		public void FireConnectEvent(int connectionId)
		{
			NewConnection?.Invoke(connectionId);
		}

		public void FireReceiveEvent(int connectionId, Mithril.Buffer buffer)
		{
			Receive?.Invoke(connectionId, buffer);
		}

		public void FireDisconnectEvent(int connectionId, byte reason)
		{
			Disconnect?.Invoke(connectionId, reason);
		}

		public void Shutdown()
		{
			foreach (Connection connection in Connections.Values)
			{
				connection.Disconnect();
			}

			IsReceiving = false;
			Handle.Set();
			Thread.Join();

			Socket.Close();
		}
	}
}
