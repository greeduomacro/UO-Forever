#region References
using System;
using System.Collections.Generic;
using System.Linq;

using Server;
using Server.Multis;
#endregion

namespace VitaNex.Modules.UOFLegends
{
	public sealed class HouseLegendState : ItemLegendState<BaseHouse>
	{
		public override string TableName { get { return "items_houses"; } }

		protected override void OnCompile(BaseHouse o, IDictionary<string, SimpleType> data)
		{
			if (o == null || o.Deleted)
			{
				data.Clear();
				return;
			}

			base.OnCompile(o, data);

			data.Add("owner", o.Owner != null ? o.Owner.Serial.Value : -1);
			data.Add("sign", o.Sign != null ? o.Sign.Serial.Value : -1);

			data.Add("public", o.Public);

			data.Add("builton", o.BuiltOn);
			data.Add("price", o.Price);

			data.Add("decaytype", o.DecayType.ToString());
			data.Add("decaylevel", o.DecayLevel.ToString());
			data.Add("decaynext", o.NextDecayStage);
			data.Add("decaytime", o.TimeToDecay);

			data.Add("region", o.Region != null ? o.Region.GetSerial().ValueHash : -1);
			data.Add("area", JoinData(o.Area.Select(b => JoinSubData(b.Start.X, b.Start.Y, b.Width, b.Height))));

			data.Add("banloc", JoinData(o.BanLocation.X, o.BanLocation.Y, o.BanLocation.Z));

			data.Add("addons", JoinData(o.Addons.Where(a => a != null).Select(a => a.Serial.Value)));

			data.Add("vendors", JoinData(o.PlayerVendors.Where(v => v != null).Select(v => v.Serial.Value)));
			data.Add("contracts", JoinData(o.VendorRentalContracts.Where(c => c != null).Select(c => c.Serial.Value)));

			data.Add("securecount", o.SecureCount);
			data.Add("securesmax", o.MaxSecures);
			data.Add(
				"secures", JoinData(o.Secures.Where(s => s != null).Select(s => JoinSubData(s.Item.Serial.Value, (int)s.Level))));

			data.Add("lockdowncount", o.LockDownCount);
			data.Add("lockdownmax", o.MaxLockDowns);
			data.Add("lockdowns", JoinData(o.LockDowns.Where(l => l != null).Select(l => l.Serial.Value)));

			data.Add("bans", JoinData(o.Bans.Where(m => m != null).Select(m => m.Serial.Value)));
			data.Add("access", JoinData(o.Access.Where(m => m != null).Select(m => m.Serial.Value)));
			data.Add("friends", JoinData(o.Friends.Where(m => m != null).Select(m => m.Serial.Value)));
			data.Add("coowners", JoinData(o.CoOwners.Where(m => m != null).Select(m => m.Serial.Value)));
		}
	}
}