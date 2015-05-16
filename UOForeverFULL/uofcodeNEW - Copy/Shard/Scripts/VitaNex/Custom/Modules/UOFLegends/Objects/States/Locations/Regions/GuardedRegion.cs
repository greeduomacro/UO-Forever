#region References
using Server.Regions;
#endregion

namespace VitaNex.Modules.UOFLegends
{
	public sealed class GuardedRegionLegendState : RegionLegendState<GuardedRegion>
	{
		public override string TableName { get { return "regions_guarded"; } }

		protected override void OnCompile(GuardedRegion o, System.Collections.Generic.IDictionary<string, System.SimpleType> data)
		{
			if (o == null || !o.Registered)
			{
				data.Clear();
				return;
			}

			base.OnCompile(o, data);

			data.Add("guarded", !o.Disabled);
			data.Add("allowreds", o.AllowReds);
		}
	}
}