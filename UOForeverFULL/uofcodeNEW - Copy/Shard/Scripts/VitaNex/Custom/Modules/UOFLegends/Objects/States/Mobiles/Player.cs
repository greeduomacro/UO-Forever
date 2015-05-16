#region References
using System;
using System.Collections.Generic;
using System.Linq;

using Server;
using Server.Factions;
using Server.Mobiles;
#endregion

namespace VitaNex.Modules.UOFLegends
{
	public sealed class PlayerLegendState : MobileLegendState<PlayerMobile>
	{
		public override string TableName { get { return "mobiles_players"; } }

		protected override void OnCompile(PlayerMobile o, IDictionary<string, SimpleType> data)
		{
			if (o == null || o.Deleted)
			{
				data.Clear();
				return;
			}

			base.OnCompile(o, data);

			data.Add("account", o.Account != null ? o.Account.Username ?? String.Empty : String.Empty);
			data.Add("online", o.NetState != null && o.NetState.Socket != null);
			data.Add(
				"ipaddress", o.NetState != null && o.NetState.Address != null ? o.NetState.Address.ToString() : String.Empty);

			data.Add("lastonline", o.LastOnline);
			data.Add("gametime", o.GameTime);
			data.Add("young", o.Young);

			data.Add("language", o.GetLanguage().ToString());

			data.Add("companion", o.Companion);
			data.Add("stafftitle", o.StaffTitle ?? String.Empty);

			data.Add("valorpoints", o.ValorPoints);
			data.Add("valorrank", o.ValorRank);
			data.Add("valortitle", o.ValorTitle ?? String.Empty);
			data.Add("valortitles", JoinData(o.GetValorTitles()));

			data.Add("npcguild", o.NpcGuild.ToString());

			data.Add("factionpoints", o.FactionPoints);

			data.Add("lastkiller", o.LastKiller != null ? o.LastKiller.Serial.Value : -1);
			data.Add("corpse", o.Corpse != null ? o.Corpse.Serial.Value : -1);

			var mount = o.Mount as IEntity;

			data.Add("mount", mount != null ? mount.Serial.Value : -1);

			data.Add("bankcount", o.BankBoxes.Count(b => b != null));
			data.Add("banks", JoinData(o.BankBoxes.Where(b => b != null).Select(b => b.Serial.Value)));

			data.Add("packcount", o.Backpacks.Count(p => p != null));
			data.Add("packs", JoinData(o.Backpacks.Where(p => p != null).Select(p => p.Serial.Value)));

			data.Add("followers", o.Followers);
			data.Add("followersmax", o.FollowersMax);

			data.Add("activepetcount", o.AllFollowers.Count(c => c != null));
			data.Add("activepets", JoinData(o.AllFollowers.Where(c => c != null).Select(c => c.Serial.Value)));

			data.Add("stabledpetcount", o.Stabled.Count(c => c != null));
			data.Add("stabledpets", JoinData(o.Stabled.Where(c => c != null).Select(c => c.Serial.Value)));

			data.Add("partycount", o.Party != null ? o.PartyMembers.Count(p => p != null) : 0);
			data.Add(
				"party", o.Party != null ? JoinData(o.PartyMembers.Where(p => p != null).Select(p => p.Serial.Value)) : String.Empty);

            data.Add("showlegends", o.PublicLegends);

            data.Add("customtitle", o.CustomTitle);
		}
	}
}