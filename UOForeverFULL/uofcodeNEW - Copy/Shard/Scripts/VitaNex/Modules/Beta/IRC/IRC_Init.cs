#region Header
//   Vorspire    _,-'/-'/  IRC_Init.cs
//   .      __,-; ,'( '/
//    \.    `-.__`-._`:_,-._       _ , . ``
//     `:-._,------' ` _,`--` -: `_ , ` ,' :
//        `---..__,,--'  (C) 2014  ` -'. -'
//        #  Vita-Nex [http://core.vita-nex.com]  #
//  {o)xxx|===============-   #   -===============|xxx(o}
//        #        The MIT License (MIT)          #
#endregion

namespace VitaNex.Modules.IRC
{
	[CoreModule("IRC", "1.0.0", false, TaskPriority.Highest)]
	public static partial class IRC
	{
		static IRC()
		{
			CMOptions = new IRCOptions();
		}

		private static void CMConfig()
		{ }

		private static void CMEnabled()
		{ }

		private static void CMDisabled()
		{ }

		private static void CMInvoke()
		{ }

		private static void CMSave()
		{ }

		private static void CMLoad()
		{ }

		private static void CMDisposed()
		{ }
	}
}