#region References
using Server.Items;
#endregion

namespace VitaNex.Modules.UOFLegends
{
	public sealed class ArmorLegendState : ItemLegendState<BaseArmor>
	{
		public override string TableName { get { return "items_armor"; } }

		protected override void OnCompile(BaseArmor o, System.Collections.Generic.IDictionary<string, System.SimpleType> data)
		{
			if (o == null || o.Deleted)
			{
				data.Clear();
				return;
			}

			base.OnCompile(o, data);

			data.Add("resource", o.Resource.ToString());
			data.Add("quality", o.Quality.ToString());
			data.Add("durability", o.Durability.ToString());

			data.Add("crafter", o.Crafter != null ? o.Crafter.Serial.Value : -1);

			data.Add("identified", o.Identified);

			data.Add("rarity", o.ArtifactRarity);

			data.Add("slayers", JoinData(o.Slayer, o.Slayer2));
			
			data.Add("armorbase", o.ArmorBase);

			data.Add("hits", o.HitPoints);
			data.Add("hitsmax", o.MaxHitPoints);

			data.Add("reqstr", o.StrRequirement);
			data.Add("reqdex", o.DexRequirement);
			data.Add("reqint", o.IntRequirement);
			data.Add("reqrace", o.RequiredRace != null ? o.RequiredRace.RaceID : -1);

			data.Add("bonusstr", o.StrBonus);
			data.Add("bonusdex", o.DexBonus);
			data.Add("bonusint", o.IntBonus);
			data.Add("bonusluck", o.GetLuckBonus());
			data.Add("bonusdurability", o.GetDurabilityBonus());
		}
	}
}