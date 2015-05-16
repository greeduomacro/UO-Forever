#region References
using Server;
#endregion

namespace VitaNex.Modules.UOFLegends
{
	public sealed class TreasureRegionLegendState : RegionLegendState<TreasureRegion>
	{
		public override string TableName { get { return "regions_treasure"; } }
	}
}