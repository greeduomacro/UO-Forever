#region References
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Server.Engines.Conquests;

using VitaNex.Modules.AutoPvP;
#endregion

namespace VitaNex.Modules.UOFLegends
{
	public sealed class PvPProfileLegendState : LegendState<PvPProfile>
	{
		public override string TableName { get { return "battles_profiles"; } }

		protected override void OnCompile(PvPProfile o, IDictionary<string, SimpleType> data)
		{
			if (o == null || o.Owner == null || o.Owner.Deleted)
			{
				data.Clear();
				return;
			}

			data.Add("serial", o.Owner.Serial.Value);
			data.Add("typeof", o.GetType().Name);
			data.Add("points", o.Points);
			data.Add("pointsgained", o.TotalPointsGained);
			data.Add("pointslost", o.TotalPointsLost);
			data.Add("wins", o.TotalWins);
			data.Add("losses", o.TotalLosses);
			data.Add("kills", o.TotalKills);
			data.Add("deaths", o.TotalDeaths);
			data.Add("resurrections", o.TotalResurrections);
			data.Add("damagedone", o.TotalDamageDone);
			data.Add("damagetaken", o.TotalDamageTaken);
			data.Add("healingdone", o.TotalHealingDone);
			data.Add("healingtaken", o.TotalHealingTaken);
			data.Add("attended", o.TotalBattles);
			data.Add(
				"misc",
				JoinData(
					o.GetMiscStatisticTotals().Select(kv => JoinSubData(kv.Key, kv.Value.ToString(CultureInfo.InvariantCulture)))));

			data.Add("history", JoinData(o.Select(h => h.UID.ValueHash)));
		}
	}
}