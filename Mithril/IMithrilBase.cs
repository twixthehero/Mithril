namespace Mithril
{
	public interface IMithrilBase
	{
		void FireConnectEvent(int connectionId);
		void FireReceiveEvent(int connectionId, Buffer buffer);
		void FireDisconnectEvent(int connectionId, byte reason);
	}
}
