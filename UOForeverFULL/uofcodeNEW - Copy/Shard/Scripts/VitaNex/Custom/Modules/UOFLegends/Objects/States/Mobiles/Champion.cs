#region References
using System;
using System.Collections.Generic;
using System.Linq;

using Server.Mobiles;
#endregion

namespace VitaNex.Modules.UOFLegends
{
	public sealed class ChampionLegendState : CreatureLegendState<BaseChampion>
	{
		public override string TableName { get { return "mobiles_champions"; } }

		protected override void OnCompile(BaseChampion o, IDictionary<string, SimpleType> data)
		{
			if (o == null || o.Deleted)
			{
				data.Clear();
				return;
			}

			base.OnCompile(o, data);

			data.Add("spawn", o.ChampSpawn != null ? o.ChampSpawn.Serial.Value : -1);
			data.Add("skull", o.SkullType.ToString());
			
			var scores = o.UseScores ? o.Scores : null;

			data.Add(
				"scores", scores != null ? JoinData(scores.Select(kv => JoinSubData(kv.Key.Serial.Value, kv.Value))) : String.Empty);
		}
	}
}