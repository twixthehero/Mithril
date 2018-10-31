using System.Net;
using System.Threading;

namespace Mithril
{
	public sealed class MithrilClient : MithrilBase
	{
		public int Connect(string to, int port)
		{
			if (IPAddress.TryParse(to, out IPAddress iPAddress))
			{
				IPEndPoint endPoint = new IPEndPoint(iPAddress, port);
				int connectionId = GetNextConnectionId();
				Connection connection = new Connection(this, connectionId, endPoint, Socket);
				Connections.Add(connectionId, connection);
				EndPointIds.Add(endPoint, connectionId);

				if (!IsReceiving)
				{
					Thread.Start();
				}

				connection.InitializeHandshake();

				return connectionId;
			}
			else
			{
				return -1;
			}
		}
	}
}
