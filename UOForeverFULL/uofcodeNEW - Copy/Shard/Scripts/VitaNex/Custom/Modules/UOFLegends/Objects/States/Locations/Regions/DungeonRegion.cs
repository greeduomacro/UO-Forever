#region References
using Server.Regions;
#endregion

namespace VitaNex.Modules.UOFLegends
{
	public sealed class DungeonRegionLegendState : RegionLegendState<DungeonRegion>
	{
		public override string TableName { get { return "regions_dungeon"; } }
	}
}