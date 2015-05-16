#region Header
//   Vorspire    _,-'/-'/  Sandbox.cs
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
#endregion

namespace VitaNex.Sandbox
{
	public static partial class Sandbox
	{
		public static CoreServiceOptions CSOptions { get; private set; }

		public static void SafeInvoke(Action func, ISandboxTest test)
		{
			if (func == null || test == null)
			{
				return;
			}

			VitaNexCore.TryCatch(
				() =>
				{
					func();
					test.OnSuccess();
				},
				test.OnException);
		}
	}
}