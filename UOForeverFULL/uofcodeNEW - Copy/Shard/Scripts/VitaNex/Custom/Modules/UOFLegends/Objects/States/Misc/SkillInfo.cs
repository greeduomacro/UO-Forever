#region References
using System;
using System.Collections.Generic;

using Server;
#endregion

namespace VitaNex.Modules.UOFLegends
{
	public sealed class SkillInfoLegendState : LegendState<SkillInfo>
	{
		public override string TableName { get { return "skills"; } }

		protected override void OnCompile(SkillInfo o, IDictionary<string, SimpleType> data)
		{
			if (o == null)
			{
				data.Clear();
				return;
			}

			data.Add("serial", o.SkillID);
			data.Add("name", o.Name);
			data.Add("title", o.Title);
			data.Add("gainstr", o.StrGain);
			data.Add("gaindex", o.DexGain);
			data.Add("gainint", o.IntGain);
		}
	}
}