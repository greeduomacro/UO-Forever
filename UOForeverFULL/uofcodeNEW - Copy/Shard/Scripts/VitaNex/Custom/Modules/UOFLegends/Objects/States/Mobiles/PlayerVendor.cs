#region References
using System;
using System.Collections.Generic;

using Server.Factions;
using Server.Mobiles;
#endregion

namespace VitaNex.Modules.UOFLegends
{
	public sealed class PlayerVendorLegendState : MobileLegendState<PlayerVendor>
	{
		public override string TableName { get { return "mobiles_player_vendors"; } }

		protected override void OnCompile(PlayerVendor v, IDictionary<string, SimpleType> data)
		{
			if (v == null || v.Deleted)
			{
				data.Clear();
				return;
			}

			base.OnCompile(v, data);
		}
	}
}