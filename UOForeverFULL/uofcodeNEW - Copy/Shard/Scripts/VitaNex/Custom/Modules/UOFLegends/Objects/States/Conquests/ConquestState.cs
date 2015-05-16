#region References
using System;
using System.Collections.Generic;

using Server.Engines.Conquests;
#endregion

namespace VitaNex.Modules.UOFLegends
{
	public sealed class ConquestStateLegendState : LegendState<ConquestState>
	{
		public override string TableName { get { return "conquest_states"; } }

		protected override void OnCompile(ConquestState o, IDictionary<string, SimpleType> data)
		{
			if (o == null || !o.ConquestExists || o.Owner == null || o.Owner.Deleted)
			{
				data.Clear();
				return;
			}

			data.Add("serial", o.UID.ValueHash);
			data.Add("typeof", o.GetType().Name);
			data.Add("owner", o.Owner.Serial.Value);
			data.Add("conquest", o.Conquest.UID.ValueHash);
			data.Add("completed", o.Completed);
			data.Add("worldfirst", o.WorldFirst);
			data.Add("date", o.CompletedDate);
			data.Add("progress", o.Progress);
			data.Add("tier", o.Tier);
		}
	}
}