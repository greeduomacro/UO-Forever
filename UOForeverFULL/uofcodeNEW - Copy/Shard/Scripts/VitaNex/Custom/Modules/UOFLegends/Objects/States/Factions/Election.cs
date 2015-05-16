using System;
using System.Collections.Generic;
using System.Linq;

using Server.Factions;

namespace VitaNex.Modules.UOFLegends
{
	public sealed class FactionElectionLegendState : LegendState<Election>
	{
		public override string TableName { get { return "faction_elections"; } }

		protected override void OnCompile(Election e, IDictionary<string, SimpleType> data)
		{
			if (e == null || e.Faction == null)
			{
				data.Clear();
				return;
			}

			data.Add("serial", Faction.Factions.IndexOf(e.Faction));

			data.Add(
				"candidates",
				JoinData(
					e.Candidates.Where(c => c != null && c.Mobile != null).Select(c => JoinSubData(c.Mobile.Serial.Value, c.Votes))));

			data.Add(
				"votes",
				JoinData(
					e.Candidates.Where(c => c != null && c.Mobile != null)
					 .SelectMany(c => c.Voters.Where(v => v != null && v.From != null))
					 .Select(v => JoinSubData(v.From.Serial.Value, v.Candidate.Serial.Value))));
		}
	}
}