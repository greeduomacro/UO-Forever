using Server.Mobiles;

using VitaNex.Notify;

namespace VitaNex.Modules.AutoPvP
{
	public class BattleNotifyGump : NotifyGump
	{
		private static void InitSettings(NotifySettings settings)
		{
			settings.Name = "PvP Battles";
			settings.CanIgnore = true;
		}

		public BattleNotifyGump(PlayerMobile user, string html)
			: base(user, html)
		{ }
	}
}