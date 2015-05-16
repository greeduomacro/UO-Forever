#region References
using System;
using System.Collections.Generic;

using Server;
#endregion

namespace VitaNex.Modules.UOFLegends
{
	public sealed class LandDataLegendState : LegendState<LandData>
	{
		public override string TableName { get { return "data_land"; } }

		protected override void OnCompile(LandData o, IDictionary<string, SimpleType> data)
		{
			data.Add("serial", TileData.LandTable.IndexOf(o));
			data.Add("name", o.Name);
			data.Add("flags", (int)o.Flags);
		}
	}
}