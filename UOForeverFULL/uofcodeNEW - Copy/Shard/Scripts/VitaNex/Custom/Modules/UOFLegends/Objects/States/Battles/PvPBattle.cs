#region References
using System;
using System.Collections.Generic;
using System.Linq;

using Server;

using VitaNex.Modules.AutoPvP;
#endregion

namespace VitaNex.Modules.UOFLegends
{
	public class PvPBattleLegendState : LegendState<PvPBattle>
	{
		public override string TableName { get { return "battles"; } }

		protected override void OnCompile(PvPBattle o, IDictionary<string, SimpleType> data)
		{
			if (o == null || o.Deleted || !o.Validate())
			{
				data.Clear();
				return;
			}

			data.Add("serial", o.Serial.ValueHash);
			data.Add("typeof", o.GetType().Name);
			data.Add("name", o.Name ?? String.Empty);
			data.Add("desc", o.Description ?? String.Empty);
			data.Add("category", o.Category ?? String.Empty);
			data.Add("state", o.State.ToString());
			data.Add("hidden", o.Hidden);
			data.Add("ranked", o.Ranked);
			data.Add("capacitymin", o.MinCapacity);
			data.Add("capacitymax", o.MaxCapacity);
			data.Add("rankpoints", o.PointsRankFactor);
			data.Add("basepoints", o.PointsBase);
			data.Add("killpoints", o.KillPoints);

			data.Add("teamcount", o.Teams.Count(t => t != null));
			data.Add("teams", JoinData(o.Teams.Where(t => t != null).Select(t => t.Serial.ValueHash)));
			data.Add("doorcount", o.Doors.Count(d => d != null));
			data.Add("doors", JoinData(o.Doors.Where(d => d != null).Select(d => d.Serial.Value)));

			data.Add(
				"spectate",
				JoinData(o.Options.Locations.SpectateJoin.X, o.Options.Locations.SpectateJoin.Y, o.Options.Locations.SpectateJoin.Z));
			data.Add("spectategate", o.Gate != null ? o.Gate.Serial.Value : -1);
			data.Add("spectateregion", o.SpectateRegion != null ? o.SpectateRegion.GetSerial().ValueHash : -1);
			data.Add("battleregion", o.BattleRegion != null ? o.BattleRegion.GetSerial().ValueHash : -1);

			data.Add("eject", JoinData(o.Options.Locations.Eject.X, o.Options.Locations.Eject.Y, o.Options.Locations.Eject.Z));
			data.Add("ejectmap", o.Map != null ? o.Options.Locations.Eject.Map.MapIndex : -1);
			data.Add("map", o.Map != null ? o.Map.MapIndex : -1);

			data.Add("preptime", o.Options.Timing.PreparePeriod.TotalSeconds);
			data.Add("runtime", o.Options.Timing.RunningPeriod.TotalSeconds);
			data.Add("endtime", o.Options.Timing.EndedPeriod.TotalSeconds);

			data.Add("scheduled", o.Schedule != null && o.Schedule.Enabled);

			string months = String.Empty, days = String.Empty, times = String.Empty;

			if (o.Schedule != null && o.Schedule.Enabled)
			{
				months = o.Schedule.Info.Months.ToString();

				switch (months)
				{
					case "All":
						months = JoinData(o.Schedule.Info.Months.GetValues<string>().Not(m => m == "All" || m == "None"));
						break;
					case "None":
						months = String.Empty;
						break;
					default:
						months = JoinData(months.Split(' ').Not(m => m == "All" || m == "None"));
						break;
				}

				days = o.Schedule.Info.Days.ToString();

				switch (days)
				{
					case "All":
						days = JoinData(o.Schedule.Info.Days.GetValues<string>().Not(d => d == "All" || d == "None"));
						break;
					case "None":
						days = String.Empty;
						break;
					default:
						days = JoinData(days.Split(' ').Not(d => d == "All" || d == "None"));
						break;
				}

				times = JoinData(o.Schedule.Info.Times.Select(t => t.TotalSeconds));
			}

			data.Add("months", months);
			data.Add("days", days);
			data.Add("times", times);
		}
	}
}