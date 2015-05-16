#region References
using System;
using System.Collections.Generic;

using Server;
#endregion

namespace VitaNex.Modules.UOFLegends
{
	public sealed class ItemDataLegendState : LegendState<ItemData>
	{
		public override string TableName { get { return "data_statics"; } }

		protected override void OnCompile(ItemData o, IDictionary<string, SimpleType> data)
		{
			data.Add("serial", TileData.ItemTable.IndexOf(o));
			data.Add("name", o.Name);
			data.Add("impassable", o.Impassable);
			data.Add("surface", o.Surface);
			data.Add("bridge", o.Bridge);
			data.Add("weight", o.Weight);
			data.Add("height", o.CalcHeight);
			data.Add("quality", o.Quality);
			data.Add("quantity", o.Quantity);
			data.Add("value", o.Value);
			data.Add("flags", (int)o.Flags);
		}
	}
}