#region References
using System;

using Server;
using Server.Mobiles;

using VitaNex.Schedules;
using VitaNex.SuperGumps.UI;

#endregion

namespace VitaNex.Modules.AutoPvP.Battles
{
	public class UOF_FFABattle : UOF_PvPBattle
	{
		public UOF_FFABattle()
		{
			Name = "Free For All";
			Category = "Free For All";
			Description = "The last participant alive wins!";

			AddTeam(NameList.RandomName("daemon"), 5, 40, 85);

			Schedule.Info.Months = ScheduleMonths.All;
			Schedule.Info.Days = ScheduleDays.All;
			Schedule.Info.Times = ScheduleTimes.EveryHour;

			Options.Timing.PreparePeriod = TimeSpan.FromMinutes(5.0);
			Options.Timing.RunningPeriod = TimeSpan.FromMinutes(15.0);
			Options.Timing.EndedPeriod = TimeSpan.FromMinutes(5.0);

			Options.Rules.CanDamageOwnTeam = true;
			Options.Rules.CanHealOwnTeam = false;
		}

		public UOF_FFABattle(GenericReader reader)
			: base(reader)
		{ }

		protected override void OnWin(PlayerMobile pm)
		{
			WorldBroadcast("{0} has won {1}!", pm.RawName, Name);

			base.OnWin(pm);
		}

		public override GlobalJoinGump CreateJoinGump(PlayerMobile pm)
		{
			return new GlobalJoinGump(pm, this, 7040);
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