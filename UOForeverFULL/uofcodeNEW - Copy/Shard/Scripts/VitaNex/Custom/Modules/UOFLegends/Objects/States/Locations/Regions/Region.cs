#region References
using System;
using System.Collections.Generic;
using System.Linq;

using Server;
using Server.Regions;
#endregion

namespace VitaNex.Modules.UOFLegends
{
	public abstract class RegionLegendState<TRegion> : LegendState<TRegion>
		where TRegion : Region
	{
		protected override void OnCompile(TRegion o, IDictionary<string, SimpleType> data)
		{
			if (o == null || !o.Registered)
			{
				data.Clear();
				return;
			}

			data.Add("serial", o.GetSerial().ValueHash);
			data.Add("typeof", o.GetType().Name);
			data.Add("mapid", o.Map.MapIndex);
			data.Add("name", o.Name ?? String.Empty);

			data.Add("go", JoinData(o.GoLocation.X, o.GoLocation.Y, o.GoLocation.Z));

			data.Add("dynamic", o.Dynamic);
			data.Add("priority", o.Priority);
			data.Add("childlevel", o.ChildLevel);

			data.Add("parent", o.Parent != null ? o.Parent.GetSerial().ValueHash : -1);

			data.Add("childcount", o.Children.Count(c => c != null));
			data.Add("children", JoinData(o.Children.Where(c => c != null).Select(c => c.GetSerial().ValueHash)));

			data.Add("areacount", o.Area.Length);
			data.Add(
				"area", JoinData(o.Area.Select(b => JoinSubData(b.Start.X, b.Start.Y, b.Start.Z, b.Width, b.Height, b.Depth))));
		}
	}

	public sealed class RegionLegendState : RegionLegendState<Region>
	{
		public override string TableName { get { return "regions"; } }
	}
}