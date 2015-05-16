#region References
using Server.Regions;
#endregion

namespace VitaNex.Modules.UOFLegends
{
	public sealed class CustomRegionLegendState : RegionLegendState<CustomRegion>
	{
		public override string TableName { get { return "regions_custom"; } }
	}
}