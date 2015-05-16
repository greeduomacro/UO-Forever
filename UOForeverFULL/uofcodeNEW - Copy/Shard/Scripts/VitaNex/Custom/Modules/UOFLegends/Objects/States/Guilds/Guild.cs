#region References
using System;
using System.Collections.Generic;
using System.Linq;

using Server.Commands;
using Server.Guilds;
#endregion

namespace VitaNex.Modules.UOFLegends
{
	public sealed class GuildLegendState : LegendState<Guild>
	{
		public override string TableName { get { return "guilds"; } }

		protected override void OnCompile(Guild o, IDictionary<string, SimpleType> data)
		{
			if (o == null)
			{
				data.Clear();
				return;
			}

			data.Add("serial", o.Id);
			data.Add("typeof", o.GetType().Name);
			data.Add("disbanded", o.Disbanded);
			data.Add("name", o.Name ?? String.Empty);
			data.Add("abbr", o.Abbreviation ?? String.Empty);
			data.Add("type", o.Type.ToString());
			data.Add("website", o.Website ?? String.Empty);
			data.Add("charter", o.Charter ?? String.Empty);
			data.Add("leader", o.Leader != null ? o.Leader.Serial.Value : -1);

			data.Add("membercount", o.Members.Count(m => m != null));
			data.Add("members", JoinData(o.Members.Where(m => m != null).Select(m => m.Serial.Value)));

			data.Add("allycount", o.Allies.Count(m => m != null));
			data.Add("allies", JoinData(o.Allies.Where(a => a != null).Select(a => a.Id)));

			data.Add("enemycount", o.Enemies.Count(m => m != null));
			data.Add("enemies", JoinData(o.Enemies.Where(e => e != null).Select(e => e.Id)));
		}
	}
}