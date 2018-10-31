using System.Net.Sockets;

namespace Mithril
{
	public sealed class MithrilServer : MithrilBase
	{
		public MithrilServer()
		{
			Thread.Start();
		}

		protected override void ReceiveAsync(object sender, SocketAsyncEventArgs e)
		{
			if (e.BytesTransferred > 0)
			{
				Buffer buffer = new Buffer(e.Buffer);
				buffer.Set(e.BytesTransferred);

				// new connection to server
				if (!EndPointIds.ContainsKey(e.RemoteEndPoint) && buffer.PeekByte() == Common.ICHI)
				{
					int connectionId = GetNextConnectionId();
					Connection connection = new Connection(this, connectionId, e.RemoteEndPoint, Socket);
					Connections.Add(connectionId, connection);
					EndPointIds.Add(e.RemoteEndPoint, connectionId);
				}

				if (EndPointIds.ContainsKey(e.RemoteEndPoint))
				{
					Connections[EndPointIds[e.RemoteEndPoint]].OnReceive(buffer);
				}
			}

			Handle.Set();
		}
	}
}
