#region References
using System.Linq;

using Server.Mobiles;

using VitaNex.IO;
#endregion

namespace Server.PvPTemplates
{
	public static partial class PvPTemplates
	{
		public const AccessLevel Access = AccessLevel.Player;

		public static PvPTemplatesOptions CSOptions { get; private set; }

		public static BinaryDataStore<PlayerMobile, TemplateProfile> Templates { get; private set; }

		private static void OnLogin(LoginEventArgs e)
		{
			var user = e.Mobile as PlayerMobile;

			if(user != null)
			{
				FetchProfile(user).ClearDelta();
			}
		}

		public static TemplateProfile FetchProfile(PlayerMobile pm, bool replace = false)
		{
			if(pm == null)
			{
				return null;
			}

			if(!Templates.ContainsKey(pm))
			{
				Templates.Add(pm, new TemplateProfile(pm));
			}
			else if(replace || Templates[pm] == null)
			{
				Templates[pm] = new TemplateProfile(pm);
			}

			return Templates[pm];
		}

		public static Template FindTemplate(TemplateSerial uid)
		{
			Template template = null;

			return Templates.Values.Any(profile => profile.TryFind(uid, out template)) ? template : null;
		}

		public static bool TryFindTemplate(TemplateSerial uid, out Template template)
		{
			return (template = FindTemplate(uid)) != null;
		}

		public static void DisplayManagerGump(PlayerMobile user)
		{
			if(user != null)
			{
				new TemplateManagerGump(user, null, FetchProfile(user)).Send();
			}
		}
	}
}