#region References
using Server.Regions;
#endregion

namespace VitaNex.Modules.UOFLegends
{
	public sealed class TownRegionLegendState : RegionLegendState<TownRegion>
	{
		public override string TableName { get { return "regions_towns"; } }
	}
}