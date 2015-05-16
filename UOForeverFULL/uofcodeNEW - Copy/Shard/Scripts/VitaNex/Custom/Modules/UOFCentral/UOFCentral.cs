#region References
using System;
using System.IO;
using System.Linq;

using Server;
using Server.Commands;
using Server.Mobiles;
using Server.Network;

using VitaNex.SuperGumps;
#endregion

namespace VitaNex.Modules.UOFCentral
{
	public static partial class UOFCentral
	{
		public const AccessLevel Access = AccessLevel.Seer;

		public static Type[] EntryTypes { get; private set; }

		public static UOFCentralOptions CMOptions { get; private set; }

		public static FileInfo StateFile { get; private set; }

		public static State State { get; private set; }

		public static void OpenAll()
		{
			foreach (PlayerMobile m in
				NetState.Instances.AsParallel()
						.Where(ns => ns != null && ns.Mobile != null)
						.Select(ns => ns.Mobile)
						.OfType<PlayerMobile>())
			{
				SendCentralGump(m);
			}
		}

		public static void CloseAll()
		{
			foreach (PlayerMobile m in
				NetState.Instances.AsParallel()
						.Where(ns => ns != null && ns.Mobile != null)
						.Select(ns => ns.Mobile)
						.OfType<PlayerMobile>())
			{
				CloseCentralGump(m);
			}
		}

		public static void SendCentralGump(this PlayerMobile user)
		{
			if (user != null && !user.Deleted && user.NetState != null && CMOptions.ModuleEnabled)
			{
				SuperGump.Send(new UOFCentralGump(user, State.Root));
			}
		}

		public static void CloseCentralGump(this PlayerMobile user)
		{
			foreach (UOFCentralGump g in SuperGump.GetInstances<UOFCentralGump>(user))
			{
				g.Close(true);
			}
		}

		public static void HandlePopupCommand(CommandEventArgs e)
		{
			SendCentralGump(e.Mobile as PlayerMobile);
		}

		private static void OnLogin(LoginEventArgs e)
		{
			if (CMOptions.LoginPopup)
			{
				SendCentralGump(e.Mobile as PlayerMobile);
			}
		}

		/*private static void OnVirtueGumpRequest(VirtueGumpRequestEventArgs e)
		{
			if (e.Beheld != null && e.Beheld == e.Beholder && CMOptions.VirtuePopup)
			{
				SendCentralGump(e.Beheld as PlayerMobile);
			}
		}*/
	}
}