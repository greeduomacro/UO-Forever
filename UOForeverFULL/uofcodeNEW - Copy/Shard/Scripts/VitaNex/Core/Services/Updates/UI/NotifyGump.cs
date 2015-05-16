using Server.Mobiles;

using VitaNex.Notify;

namespace VitaNex.Updates
{
	public sealed class UpdatesNotifyGump : NotifyGump
	{
		private static void InitSettings(NotifySettings settings)
		{
			settings.Name = "Vita-Nex: Core Updates";
			settings.Access = UpdateService.CSOptions.NotifyAccess;
		}

		public UpdatesNotifyGump(PlayerMobile user, string html)
			: base(user, html)
		{ }
	}
}