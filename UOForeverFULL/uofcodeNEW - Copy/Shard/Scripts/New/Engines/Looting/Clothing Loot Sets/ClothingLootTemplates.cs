using System;
using System.Collections.Generic;
using Server.Items;

namespace Server
{
	public class AllClothingLootTemplate : BaseLootTemplate
	{
		private static List<Tuple<BaseLootSet,double>> m_LootSets;
		public override List<Tuple<BaseLootSet,double>> LootSets{ get{ return m_LootSets; } set{ m_LootSets = value; } }

		public override void InitializeTemplate()
		{
		}

		public AllClothingLootTemplate() : base( 1750 )
		{
		}
	}
}