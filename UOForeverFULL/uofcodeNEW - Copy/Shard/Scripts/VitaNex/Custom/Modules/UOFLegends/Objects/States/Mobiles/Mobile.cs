#region References
using System;
using System.Collections.Generic;
using System.Linq;

using Server;
using Server.Factions;
using Server.Items;
#endregion

namespace VitaNex.Modules.UOFLegends
{
	public abstract class MobileLegendState<TMobile> : LegendState<TMobile>
		where TMobile : Mobile
	{
		protected override void OnCompile(TMobile o, IDictionary<string, SimpleType> data)
		{
			if (o == null || o.Deleted)
			{
				data.Clear();
				return;
			}

			data.Add("serial", o.Serial.Value);
			data.Add("typeof", o.GetType().Name);

			data.Add("access", o.AccessLevel.ToString());

			data.Add("name", o.RawName ?? String.Empty);
			data.Add("namehue", o.NameHue);

			data.Add("assoc", o.Association);

			data.Add("title", o.Title ?? String.Empty);
			data.Add("profile", o.Profile ?? String.Empty);

			data.Add("race", o.Race != null ? o.Race.RaceID : -1);
			data.Add("body", o.BodyValue);
			data.Add("hue", o.Hue);
			data.Add("solidhue", o.SolidHueOverride);

			data.Add("hairid", o.HairItemID);
			data.Add("hairhue", o.HairHue);
			data.Add("beardid", o.FacialHairItemID);
			data.Add("beardhue", o.FacialHairHue);

			data.Add("female", o.Female);
			data.Add("criminal", o.Criminal);
			data.Add("blessed", o.Blessed);
			data.Add("poisoned", o.Poisoned);
			data.Add("hidden", o.Hidden);
			data.Add("alive", o.Alive);

			var f = Faction.Find(o);

			data.Add("faction", f != null ? Faction.Factions.IndexOf(f) : -1);

			data.Add("guild", o.Guild != null ? o.Guild.Id : -1);
			data.Add("guildtitle", o.GuildTitle ?? String.Empty);
			data.Add("guildfealty", o.GuildFealty != null ? o.GuildFealty.Serial.Value : -1);
			
			data.Add("fame", o.Fame);
			data.Add("karma", o.Karma);
			data.Add("kills", o.Kills);

			data.Add("strraw", o.RawStr);
			data.Add("dexraw", o.RawDex);
			data.Add("intraw", o.RawInt);

			data.Add("str", o.Str);
			data.Add("dex", o.Dex);
			data.Add("int", o.Int);

			data.Add("hits", o.Hits);
			data.Add("stam", o.Stam);
			data.Add("mana", o.Mana);

			data.Add("hitsmax", o.HitsMax);
			data.Add("stammax", o.StamMax);
			data.Add("manamax", o.ManaMax);

			data.Add("skillstotal", o.SkillsTotal / 10.0);
			data.Add("skillscap", o.SkillsCap / 10.0);

			data.Add("skills", JoinData(o.Skills.Where(s => s != null).Select(s => JoinSubData(s.SkillID, s.Value, s.Cap))));

			data.Add("totalgold", o.TotalGold);
			data.Add("totalitems", o.TotalItems);
			data.Add("totalweight", o.TotalWeight);

			data.Add("itemcount", o.Items.Count(i => i != null));
			data.Add("items", JoinData(o.Items.Where(i => i != null).Select(i => i.Serial.Value)));

			BankBox bank = o.FindBankNoCreate();
			Container pack = o.Backpack;

			data.Add("bank", bank != null ? bank.Serial.Value : -1);
			data.Add("pack", pack != null ? pack.Serial.Value : -1);

			data.Add("expansion", o.Expansion.ToString());
			data.Add("location", JoinData(o.X, o.Y, o.Z));
			data.Add("region", o.Region != null ? o.Region.GetSerial().ValueHash : -1);
			data.Add("map", o.Map != null ? o.Map.MapIndex : -1);
		}
	}

	public sealed class MobileLegendState : MobileLegendState<Mobile>
	{
		public override string TableName { get { return "mobiles"; } }
	}
}