#region References
using Server.Items;
#endregion

namespace VitaNex.Modules.UOFLegends
{
	public sealed class ClothingLegendState : ItemLegendState<BaseClothing>
	{
		public override string TableName { get { return "items_clothing"; } }

		protected override void OnCompile(BaseClothing o, System.Collections.Generic.IDictionary<string, System.SimpleType> data)
		{
			if (o == null || o.Deleted)
			{
				data.Clear();
				return;
			}

			base.OnCompile(o, data);

			data.Add("resource", o.Resource.ToString());
			data.Add("quality", o.Quality.ToString());

			data.Add("crafter", o.Crafter != null ? o.Crafter.Serial.Value : -1);

			data.Add("identified", o.Identified);

			data.Add("rarity", o.ArtifactRarity);

			data.Add("hits", o.HitPoints);
			data.Add("hitsmax", o.MaxHitPoints);

			data.Add("reqstr", o.StrRequirement);
			data.Add("reqrace", o.RequiredRace != null ? o.RequiredRace.RaceID : -1);

			data.Add("bonusstr", o.BaseStrBonus);
			data.Add("bonusdex", o.BaseDexBonus);
			data.Add("bonusint", o.BaseIntBonus);
		}
	}
}