#region References
using System;
using System.Collections.Generic;
using System.Linq;

using Server.Engines.Conquests;
#endregion

namespace VitaNex.Modules.UOFLegends
{
	public class ConquestLegendState : LegendState<Conquest>
	{
		public override string TableName { get { return "conquests"; } }

		protected override void OnCompile(Conquest o, IDictionary<string, SimpleType> data)
		{
			if (o == null || o.Deleted)
			{
				data.Clear();
				return;
			}

			data.Add("serial", o.UID.ValueHash);
			data.Add("typeof", o.GetType().Name);
			data.Add("name", o.Name ?? String.Empty);
			data.Add("desc", o.Desc ?? String.Empty);
			data.Add("category", o.Category ?? ConquestCategory.Default);
			data.Add("enabled", o.Enabled);
			data.Add("accountbound", o.AccountBound);
			data.Add("timeout", o.TimeoutReset);
			data.Add("secret", o.Secret);
			data.Add("young", o.Young);
			data.Add("icon", o.ItemID);
			data.Add("hue", o.Hue);
			data.Add("color", String.Format("#{0:X}", o.Color.ToColor().ToArgb()));
			data.Add("points", o.Points);
			data.Add("progressmax", o.ProgressMax);
			data.Add("tiermax", o.TierMax);

			var rewards =
				o.Rewards.Select(t => ConquestRewardInfo.EnsureInfo(o, t))
				 .Where(r => r != null)
				 .Select(r => JoinSubData(r.Name ?? String.Empty, r.Amount, r.ItemID, r.Hue))
				 .ToArray();

			data.Add("rewardcount", rewards.Length);
			data.Add("rewards", JoinData(rewards));
		}
	}
}