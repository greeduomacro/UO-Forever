#region References
using System;
#endregion

namespace Server.Engines.Harvest
{
	public class BonusHarvestResource
	{
		public Type Type { get; set; }
		public double ReqSkill { get; set; }
		public double Chance { get; set; }

		public Expansion ReqExpansion { get; set; }

		public TextDefinition SuccessMessage { get; private set; }

		public void SendSuccessTo(Mobile m)
		{
			TextDefinition.SendMessageTo(m, SuccessMessage);
		}

		public BonusHarvestResource(Expansion reqExpansion, double reqSkill, double chance, TextDefinition message, Type type)
		{
			ReqExpansion = reqExpansion;
			ReqSkill = reqSkill;

			Chance = chance;
			Type = type;
			SuccessMessage = message;
		}
	}
}