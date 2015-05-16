#region References
using System;
using System.Collections.Generic;
using System.Linq;

using Server;
using Server.Factions;
#endregion

namespace VitaNex.Modules.UOFLegends
{
	public sealed class FactionLegendState : LegendState<Faction>
	{
		public override string TableName { get { return "factions"; } }

		protected override void OnCompile(Faction f, IDictionary<string, SimpleType> data)
		{
			if (f == null)
			{
				data.Clear();
				return;
			}

			data.Add("serial", Faction.Factions.IndexOf(f));
			data.Add("typeof", f.GetType().Name);
			data.Add("name", f.Definition.FriendlyName ?? String.Empty);
			data.Add("abbr", f.Definition.Abbreviation ?? String.Empty);
			data.Add("about", f.Definition.About.GetString());
			data.Add("hue1", f.Definition.HuePrimary);
			data.Add("hue2", f.Definition.HueSecondary);
			data.Add("stronghold", f.StrongholdRegion != null ? f.StrongholdRegion.GetSerial().ValueHash : -1);
			data.Add("commander", f.Commander != null ? f.Commander.Serial.Value : -1);

			var towns = Town.Towns.Where(t => t != null && t.Owner == f).ToArray();

			data.Add("towncount", towns.Length);
			data.Add("towns", JoinData(towns.Select(t => Town.Towns.IndexOf(t))));

			data.Add("silver", f.Silver);
			data.Add("membercount", f.Members.Count(s => s != null && s.Mobile != null));
			data.Add("members", JoinData(f.Members.Where(s => s != null && s.Mobile != null).Select(s => s.Mobile.Serial.Value)));
		}
	}
}