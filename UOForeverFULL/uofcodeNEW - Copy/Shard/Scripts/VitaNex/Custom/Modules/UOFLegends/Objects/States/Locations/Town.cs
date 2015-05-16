using System;
using System.Collections.Generic;

using Server.Factions;

namespace VitaNex.Modules.UOFLegends
{
	public sealed class TownLegendState : LegendState<Town>
	{
		public override string TableName { get { return "towns"; } }

		protected override void OnCompile(Town t, IDictionary<string, SimpleType> data)
		{
			if (t == null)
			{
				data.Clear();
				return;
			}

			data.Add("serial", Town.Towns.IndexOf(t));
			data.Add("typeof", t.GetType().Name);
			data.Add("name", t.Definition.TownName.GetString());
			data.Add("faction", t.Owner != null ? Faction.Factions.IndexOf(t.Owner) : -1);
			data.Add("silver", t.Silver);

			data.Add("income", t.DailyIncome);
			data.Add("incomechanged", t.LastIncome);

			data.Add("tax", t.Tax);
			data.Add("taxchanged", t.LastTaxChange);

			data.Add("netcashflow", t.NetCashFlow);

			data.Add("monolith", t.Monolith != null ? t.Monolith.Serial.Value : -1);

			data.Add("finance", t.Finance != null ? t.Finance.Serial.Value : -1);
			data.Add("financeupkeep", t.FinanceUpkeep);

			data.Add("sheriff", t.Sheriff != null ? t.Sheriff.Serial.Value : -1);
			data.Add("sheriffupkeep", t.SheriffUpkeep);
		}
	}
}