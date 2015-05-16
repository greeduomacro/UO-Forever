#region References
using System.Net;
using System.Net.Sockets;

using Server.Network;
#endregion

namespace Server
{
	public class SocketOptions
	{
		public static bool NagleEnabled = false; // Should the Nagle algorithm be enabled? This may reduce performance
		public static int CoalesceBufferSize = 512; // MSS that the core will use when buffering packets
		//public static int PooledSockets = 32; // The number of sockets to initially pool. Ideal value is expected client count.

		private static readonly IPEndPoint[] m_ListenerEndPoints = new[] {//
			new IPEndPoint(IPAddress.Any, 5555), //
			new IPEndPoint(IPAddress.Any, ShardInfo.IsTestCenter ? 2598 : 2599) //
		};

		public static void Initialize()
		{
			SendQueue.CoalesceBufferSize = CoalesceBufferSize;
			//SocketPool.InitialCapacity = PooledSockets;

			EventSink.SocketConnect += EventSink_SocketConnect;

			Listener.EndPoints = m_ListenerEndPoints;
		}

		private static void EventSink_SocketConnect(SocketConnectEventArgs e)
		{
			if (e.AllowConnection && !NagleEnabled)
			{
				e.Socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, 1); // RunUO uses its own algorithm
			}
		}
	}
}