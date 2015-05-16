using Server.Mobiles;

using VitaNex.Notify;

namespace Server.Engines.Conquests
{
	public class ConquestNotifyGump : NotifyGump
	{
		private static void InitSettings(NotifySettings settings)
		{
			settings.Name = "Conquests";
			settings.CanIgnore = true;
		}

		public ConquestNotifyGump(PlayerMobile user, string html)
			: base(user, html)
		{ }
	}
}