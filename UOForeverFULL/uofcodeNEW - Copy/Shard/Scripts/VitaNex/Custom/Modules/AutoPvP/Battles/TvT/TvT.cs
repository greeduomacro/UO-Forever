#region Header
//   Vorspire    _,-'/-'/  TvT.cs
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

using Server;
using Server.Mobiles;
using VitaNex.Schedules;
using VitaNex.SuperGumps.UI;

#endregion

namespace VitaNex.Modules.AutoPvP.Battles
{
	public class UOF_TvTBattle : UOF_PvPBattle
	{
		public UOF_TvTBattle()
		{
			Name = "Team vs Team";
			Category = "Team vs Team";
			Description = "The last team alive wins!";

			AddTeam(NameList.RandomName("daemon"), 1, 1, 0x22);
			AddTeam(NameList.RandomName("daemon"), 1, 1, 0x55);

			Schedule.Info.Months = ScheduleMonths.All;
			Schedule.Info.Days = ScheduleDays.All;
			Schedule.Info.Times = ScheduleTimes.EveryQuarterHour;

			Options.Timing.PreparePeriod = TimeSpan.FromMinutes(2.0);
			Options.Timing.RunningPeriod = TimeSpan.FromMinutes(8.0);
			Options.Timing.EndedPeriod = TimeSpan.FromMinutes(1.0);
		}

		public UOF_TvTBattle(GenericReader reader)
			: base(reader)
		{ }

		public override int CompareTeamRank(PvPTeam a, PvPTeam b)
		{
			int result = 0;

			if (a.CompareNull(b, ref result))
			{
				return result;
			}

			if (a.Deleted && b.Deleted)
			{
				return 0;
			}

			if (a.Deleted)
			{
				return 1;
			}

			if (b.Deleted)
			{
				return -1;
			}

			if (!a.RespawnOnDeath && !b.RespawnOnDeath)
			{
				if (a.Dead.Count > b.Dead.Count)
				{
					return 1;
				}

				if (a.Dead.Count < b.Dead.Count)
				{
					return -1;
				}

				return 0;
			}

			return base.CompareTeamRank(a, b);
		}

		public override void OnTeamWin(PvPTeam team)
		{
			WorldBroadcast("Team {0} has won {1}!", team.Name, Name);

			base.OnTeamWin(team);
		}

		public override GlobalJoinGump CreateJoinGump(PlayerMobile pm)
		{
			return new GlobalJoinGump(pm, this, 7025);
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			int version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
					break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.GetVersion();

			switch (version)
			{
				case 0:
					break;
			}
		}
	}
}