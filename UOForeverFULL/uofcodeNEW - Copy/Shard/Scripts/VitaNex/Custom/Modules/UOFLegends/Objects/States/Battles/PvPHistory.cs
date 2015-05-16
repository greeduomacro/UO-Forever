#region References
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using VitaNex.Modules.AutoPvP;
#endregion

namespace VitaNex.Modules.UOFLegends
{
	public sealed class PvPHistoryLegendState : LegendState<PvPProfileHistoryEntry>
	{
		public override string TableName { get { return "battle_profiles_history"; } }

		protected override void OnCompile(PvPProfileHistoryEntry h, IDictionary<string, SimpleType> data)
		{
			if (h == null)
			{
				data.Clear();
				return;
			}

			data.Add("serial", h.UID.ValueHash);
			data.Add("typeof", h.GetType().Name);
			data.Add("season", h.Season);
			data.Add("points", h.PointsGained - h.PointsLost);
			data.Add("pointsgained", h.PointsGained);
			data.Add("pointslost", h.PointsLost);
			data.Add("wins", h.Wins);
			data.Add("losses", h.Losses);
			data.Add("kills", h.Kills);
			data.Add("deaths", h.Deaths);
			data.Add("resurrections", h.Resurrections);
			data.Add("damagedone", h.DamageDone);
			data.Add("damagetaken", h.DamageTaken);
			data.Add("healingdone", h.HealingDone);
			data.Add("healingtaken", h.HealingTaken);
			data.Add("attended", h.Battles);
			data.Add("misc", JoinData(h.MiscStats.Select(kv => JoinSubData(kv.Key, kv.Value))));
		}
	}
}