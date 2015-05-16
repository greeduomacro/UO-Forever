#region References
using System;
using System.Collections.Generic;
using System.Linq;

using Server.Engines.Conquests;
#endregion

namespace VitaNex.Modules.UOFLegends
{
	public sealed class ConquestProfileLegendState : LegendState<ConquestProfile>
	{
		public override string TableName { get { return "conquest_profiles"; } }

		protected override void OnCompile(ConquestProfile o, IDictionary<string, SimpleType> data)
		{
			if (o == null || o.Owner == null || o.Owner.Deleted)
			{
				data.Clear();
				return;
			}

			data.Add("serial", o.Owner.Serial.Value);
			data.Add("typeof", o.GetType().Name);
			data.Add("points", o.GetPointsTotal());
			data.Add("conquestcount", o.Count);
			data.Add("conquests", JoinData(o.Select(s => s.UID.ValueHash)));
		}
	}
}