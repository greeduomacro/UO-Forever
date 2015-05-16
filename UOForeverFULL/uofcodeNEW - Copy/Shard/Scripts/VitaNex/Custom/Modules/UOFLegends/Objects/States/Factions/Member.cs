using System;
using System.Collections.Generic;

using Server.Factions;

namespace VitaNex.Modules.UOFLegends
{
	public sealed class FactionMemberLegendState : LegendState<PlayerState>
	{
		public override string TableName { get { return "faction_members"; } }

		protected override void OnCompile(PlayerState s, IDictionary<string, SimpleType> data)
		{
			if (s == null || s.Faction == null || s.Mobile == null || s.Mobile.Deleted)
			{
				data.Clear();
				return;
			}

			data.Add("serial", s.Mobile.Serial.Value);
			data.Add("faction", Faction.Factions.IndexOf(s.Faction));
			data.Add("joined", s.JoinDate);
			data.Add("active", s.IsActive);
			data.Add("leaving", s.IsLeaving);
			data.Add("points", s.KillPoints);
			data.Add("rank", s.Rank.Rank);
			data.Add("title", s.Rank.Title.GetString());
		}
	}
}