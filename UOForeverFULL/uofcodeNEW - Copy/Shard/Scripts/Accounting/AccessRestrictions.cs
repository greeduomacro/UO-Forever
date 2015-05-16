#region References
using System;
using System.Net;

using Server.Commands;
using Server.Misc;
#endregion

namespace Server
{
	public static class AccessRestrictions
	{
		public static void Initialize()
		{
			EventSink.SocketConnect += OnSocketConnect;
		}

		private static void OnSocketConnect(SocketConnectEventArgs e)
		{
			try
			{
				IPAddress ip = ((IPEndPoint)e.Socket.RemoteEndPoint).Address;

				if (Firewall.IsBlocked(ip))
				{
					Console.WriteLine("Client: {0}: Firewall blocked connection attempt.", ip);
					e.AllowConnection = false;
					return;
				}

				if (!IPLimiter.SocketBlock || IPLimiter.Verify(ip))
				{
					return;
				}

				LoggingCustom.Log(
					"LOG_IPLimits.log",
					String.Format("{0}\tPast IP limit threshold\t{1}", ip, DateTime.Now.ToSimpleString("t d-m-y N")));

				Console.WriteLine("Client: {0}: Past IP limit threshold", ip);

				e.AllowConnection = false;
			}
			catch
			{
				e.AllowConnection = false;
			}
		}
	}
}