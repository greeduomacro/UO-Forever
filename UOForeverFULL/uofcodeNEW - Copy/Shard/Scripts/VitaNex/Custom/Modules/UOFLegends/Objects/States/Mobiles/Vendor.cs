#region References
using System;
using System.Collections.Generic;

using Server.Factions;
using Server.Mobiles;
#endregion

namespace VitaNex.Modules.UOFLegends
{
	public sealed class VendorLegendState : MobileLegendState<BaseVendor>
	{
		public override string TableName { get { return "mobiles_vendors"; } }

		protected override void OnCompile(BaseVendor v, IDictionary<string, SimpleType> data)
		{
			if (v == null || v.Deleted)
			{
				data.Clear();
				return;
			}

			base.OnCompile(v, data);
			
			data.Add("npcguild", v.NpcGuild.ToString());
		}
	}
}