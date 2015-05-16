#region References
using Server.Regions;
#endregion

namespace VitaNex.Modules.UOFLegends
{
	public sealed class HouseRegionLegendState : RegionLegendState<HouseRegion>
	{
		public override string TableName { get { return "regions_houses"; } }

		protected override void OnCompile(HouseRegion o, System.Collections.Generic.IDictionary<string, System.SimpleType> data)
		{
			if (o == null || !o.Registered)
			{
				data.Clear();
				return;
			}

			base.OnCompile(o, data);

			data.Add("house", o.House != null ? o.House.Serial.Value : -1);
		}
	}
}