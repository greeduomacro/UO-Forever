#region References
using System;
using System.Collections.Generic;
using System.Linq;

using Server.Factions;
using Server.Mobiles;
#endregion

namespace VitaNex.Modules.UOFLegends
{
	public abstract class CreatureLegendState<TCreature> : MobileLegendState<TCreature>
		where TCreature : BaseCreature
	{
		protected override void OnCompile(TCreature o, IDictionary<string, SimpleType> data)
		{
			if (o == null || o.Deleted)
			{
				data.Clear();
				return;
			}

			base.OnCompile(o, data);

			data.Add("ai", o.AI.ToString());
			data.Add("fightmode", o.FightMode.ToString());
			data.Add("alignment", o.Alignment.ToString());

			data.Add("loyalty", o.Loyalty);
			data.Add("slots", o.ControlSlots);

			data.Add("stabled", o.IsStabled);
			data.Add("bonded", o.IsBonded);
			data.Add("summoned", o.Summoned);
			data.Add("controlled", o.Controlled);

			data.Add("tamable", o.Tamable);
			data.Add("tameskill", o.MinTameSkill);

			data.Add("mastercount", o.Owners != null ? o.Owners.Count(m => m != null) : 0);
			data.Add(
				"masters", o.Owners != null ? JoinData(o.Owners.Where(m => m != null).Select(m => m.Serial.Value)) : String.Empty);

			data.Add("friendcount", o.Friends != null ? o.Friends.Count(m => m != null) : 0);
			data.Add(
				"friends", o.Friends != null ? JoinData(o.Friends.Where(m => m != null).Select(m => m.Serial.Value)) : String.Empty);

			var master = o.GetMaster();

			data.Add("master", master != null ? master.Serial.Value : -1);
			data.Add("lastmaster", o.LastOwner != null ? o.LastOwner.Serial.Value : -1);

			data.Add("damagemin", o.DamageMin);
			data.Add("damagemax", o.DamageMax);
		}
	}

	public sealed class CreatureLegendState : MobileLegendState<BaseCreature>
	{
		public override string TableName { get { return "mobiles_creatures"; } }
	}
}