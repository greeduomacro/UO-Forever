#region Header
//   Vorspire    _,-'/-'/  FixMeOverride.cs
//   .      __,-; ,'( '/
//    \.    `-.__`-._`:_,-._       _ , . ``
//     `:-._,------' ` _,`--` -: `_ , ` ,' :
//        `---..__,,--'  (C) 2014  ` -'. -'
//        #  Vita-Nex [http://core.vita-nex.com]  #
//  {o)xxx|===============-   #   -===============|xxx(o}
//        #        The MIT License (MIT)          #
#endregion

namespace VitaNex.Commands
{
	public static class FixMeCommandOverride
	{
		public static void Initialize()
		{
			FixMeCommand.DisabledFlags.Add(FixMeFlags.Pets);
		}
	}
}