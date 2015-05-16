#region Header
//   Vorspire    _,-'/-'/  Utility.cs
//   .      __,-; ,'( '/
//    \.    `-.__`-._`:_,-._       _ , . ``
//     `:-._,------' ` _,`--` -: `_ , ` ,' :
//        `---..__,,--'  (C) 2014  ` -'. -'
//        #  Vita-Nex [http://core.vita-nex.com]  #
//  {o)xxx|===============-   #   -===============|xxx(o}
//        #        The MIT License (MIT)          #
#endregion

#region References
using System;
using System.Drawing;
using System.Text;

using Server.Commands;
#endregion

namespace Server.Engines.Conquests
{
	public static class ConquestGumpUtility
	{
		public static StringBuilder GetHelpText(Mobile m)
		{
			var help = new StringBuilder();

			help.AppendFormat("<basefont color=#{0:X6}>", Color.SkyBlue.ToArgb());
			help.AppendLine("Conquests allow you to achieve great feats and be rewarded for it!");

			help.AppendLine();
			help.AppendFormat("<basefont color=#{0:X6}>", Color.Yellow.ToArgb());
			help.AppendLine("All conquests will be tracked in your personal conquest profile.");
			help.AppendLine(
				String.Format("To view your conquest profile, use the <big>{0}Conquests</big> command.", CommandSystem.Prefix));

			help.AppendLine();
			help.AppendFormat("<basefont color=#{0:X6}>", Color.YellowGreen.ToArgb());
			help.AppendLine(
				String.Format("To view conquest profiles, use the <big>{0}ConquestProfiles</big> command.", CommandSystem.Prefix));

			if (m.AccessLevel >= Conquests.Access)
			{
				help.AppendLine("You have access to administrate the conquest system, you can also manage profiles.");

				help.AppendLine();
				help.AppendFormat("<basefont color=#{0:X6}>", Color.LimeGreen.ToArgb());
				help.AppendLine(
					String.Format(
						"To administrate the conquest system, use the <big>{0}ConquestAdmin</big> command.", CommandSystem.Prefix));
			}

			return help;
		}
	}
}