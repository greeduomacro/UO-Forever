#region Header
//   Vorspire    _,-'/-'/  AutoPvPExtension.cs
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
using System.Linq;

using VitaNex.Modules.AutoPvP.Battles;
#endregion

namespace VitaNex.Modules.AutoPvP
{
	public static class AutoPvPExtension
	{
		private static readonly Type TypeOfAutoPvP = typeof(AutoPvP);
		private static readonly Type TypeOfIUOFBattle = typeof(IUOFBattle);

		public static bool Configured { get; private set; }

		public static void Configure()
		{
			if (Configured)
			{
				return;
			}

			Configured = true;

			VitaNexCore.OnModuleConfigured += cmi =>
			{
				if (cmi.TypeOf.IsEqual(TypeOfAutoPvP))
				{
					AutoPvP.BattleTypes = AutoPvP.BattleTypes.Where(t => t != null && t.HasInterface(TypeOfIUOFBattle)).ToArray();
				}
			};

			VitaNexCore.OnModuleInvoked += cmi =>
			{
				if (cmi.TypeOf.IsEqual(TypeOfAutoPvP))
				{
					AutoPvP.Scenarios = AutoPvP.Scenarios.Where(s => s != null && s.TypeOf.HasInterface(TypeOfIUOFBattle)).ToArray();
				}
			};
		}
	}
}