#region References
using System;
using System.Collections.Generic;

using Server.Items;
#endregion

namespace VitaNex.Modules.UOFLegends
{
	public sealed class JewelleryLegendState : ItemLegendState<BaseJewel>
	{
		public override string TableName { get { return "items_jewels"; } }

		protected override void OnCompile(BaseJewel o, IDictionary<string, SimpleType> data)
		{
			if (o == null || o.Deleted)
			{
				data.Clear();
				return;
			}

			base.OnCompile(o, data);

			data.Add("rarity", o.ArtifactRarity);

			data.Add("gemtype", o.GemType.ToString());
			data.Add("resource", o.Resource.ToString());

			data.Add("hits", o.HitPoints);
			data.Add("hitsmax", o.MaxHitPoints);
		}
	}
}