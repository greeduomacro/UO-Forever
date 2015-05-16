#region References
using Server.Regions;
#endregion

namespace VitaNex.Modules.UOFLegends
{
	public sealed class CraftShopRegionLegendState : RegionLegendState<CraftShopRegion>
	{
		public override string TableName { get { return "regions_shops"; } }
	}
}