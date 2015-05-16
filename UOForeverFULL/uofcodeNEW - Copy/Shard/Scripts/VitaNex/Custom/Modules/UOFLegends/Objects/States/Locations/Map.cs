#region References
using System;
using System.Collections.Generic;

using Server;

using VitaNex.Network;
#endregion

namespace VitaNex.Modules.UOFLegends
{
	public sealed class MapLegendState : LegendState<Map>
	{
		public override string TableName { get { return "maps"; } }

		protected override void OnCompile(Map o, IDictionary<string, SimpleType> data)
		{
			if (o == null)
			{
				data.Clear();
				return;
			}

			data.Add("serial", o.MapIndex);
			data.Add("typeof", o.GetType().Name);
			data.Add("name", o.Name);
			data.Add("mapid", o.MapID);
			data.Add("width", o.Width);
			data.Add("height", o.Height);
			data.Add("season", ((Season)o.Season).ToString());
			data.Add("expansion", o.Expansion.ToString());
		}
	}
}