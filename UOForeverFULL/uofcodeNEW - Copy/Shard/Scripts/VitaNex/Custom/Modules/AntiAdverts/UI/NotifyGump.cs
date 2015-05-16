#region References
using Server.Mobiles;

using VitaNex.Notify;
#endregion

namespace Server.Misc
{
	public sealed class AntiAdvertNotifyGump : NotifyGump
	{
		private static void InitSettings(NotifySettings settings)
		{
			settings.Name = "Advertising Reports";
			settings.CanIgnore = true;
			settings.Access = AntiAdverts.Access;
		}

		public AntiAdvertNotifyGump(PlayerMobile user, string html)
			: base(user, html)
		{ }
	}
}