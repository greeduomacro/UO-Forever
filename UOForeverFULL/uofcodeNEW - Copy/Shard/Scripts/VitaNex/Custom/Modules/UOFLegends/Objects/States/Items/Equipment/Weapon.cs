#region References
using System;
using System.Collections.Generic;

using Server.Items;
#endregion

namespace VitaNex.Modules.UOFLegends
{
	public sealed class WeaponLegendState : ItemLegendState<BaseWeapon>
	{
		public override string TableName { get { return "items_weapons"; } }

		protected override void OnCompile(BaseWeapon o, IDictionary<string, SimpleType> data)
		{
			if (o == null || o.Deleted)
			{
				data.Clear();
				return;
			}

			base.OnCompile(o, data);

			data.Add("resource", o.Resource.ToString());
			data.Add("quality", o.Quality.ToString());
			data.Add("durability", o.DurabilityLevel.ToString());
			data.Add("accuracy", o.AccuracyLevel.ToString());
			data.Add("damage", o.DamageLevel.ToString());

			data.Add("type", o.Type.ToString());
			data.Add("skill", (int)o.Skill);
			data.Add("accuracyskill", (int)o.AccuracySkill);

			data.Add("crafter", o.Crafter != null ? o.Crafter.Serial.Value : -1);

			data.Add("identified", o.Identified);

			data.Add("rarity", o.ArtifactRarity);

			List<string> abilities = new List<string>();

			if (o.PrimaryAbility != null)
			{
				abilities.Add(o.PrimaryAbility.GetType().Name);
			}

			if (o.SecondaryAbility != null)
			{
				abilities.Add(o.SecondaryAbility.GetType().Name);
			}

			data.Add("abilities", JoinData(abilities));

			data.Add("slayers", JoinData(o.Slayer, o.Slayer2));

			data.Add("damagemin", o.DamageMin);
			data.Add("damagemax", o.DamageMax);

			data.Add("hits", o.HitPoints);
			data.Add("hitsmax", o.MaxHitPoints);

			data.Add("reqstr", o.StrRequirement);
			data.Add("reqdex", o.DexRequirement);
			data.Add("reqint", o.IntRequirement);
			data.Add("reqrace", o.RequiredRace != null ? o.RequiredRace.RaceID : -1);

			data.Add("bonusluck", o.GetLuckBonus());
			data.Add("bonusdurability", o.GetDurabilityBonus());
		}
	}
}