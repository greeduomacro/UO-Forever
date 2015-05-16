#region References
using System;
using System.Collections.Generic;
using System.Linq;

using Server;
using Server.Engines.Conquests;

using VitaNex.Modules.AutoPvP;
#endregion

namespace VitaNex.Modules.UOFLegends
{
	public class PvPTeamLegendState : LegendState<PvPTeam>
	{
		public override string TableName { get { return "battle_teams"; } }

		protected override void OnCompile(PvPTeam o, IDictionary<string, SimpleType> data)
		{
			if (o == null || o.Deleted)
			{
				data.Clear();
				return;
			}

			data.Add("serial", o.Serial.ValueHash);
			data.Add("typeof", o.GetType().Name);
			data.Add("battle", o.Battle != null ? o.Battle.Serial.ValueHash : -1);
			data.Add("name", o.Name ?? String.Empty);
			data.Add("hue", o.Color);
			data.Add("capacitymin", o.MinCapacity);
			data.Add("capacitymax", o.MaxCapacity);

			data.Add("home", JoinData(o.HomeBase.X, o.HomeBase.Y, o.HomeBase.Z));
			data.Add("spawn", JoinData(o.SpawnPoint.X, o.SpawnPoint.Y, o.SpawnPoint.Z));

			data.Add("joingate", o.Gate != null ? o.Gate.Serial.Value : -1);
			data.Add("map", o.Map != null ? o.Map.MapIndex : -1);
		}
	}
}