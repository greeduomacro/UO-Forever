using System;
using System.Collections.Generic;

namespace Server
{
	public abstract class BaseLootTemplate
	{
		private int m_TotalValue;
		public int TotalValue{ get{ return m_TotalValue; } set{ m_TotalValue = value; } }

		public abstract List<Tuple<BaseLootSet,double>> LootSets{ get; set; } //This should reference a static list object.
		public abstract void InitializeTemplate();

		public void AddLootSet( BaseLootSet set, double chance )
		{
			LootSets.Add( new Tuple<BaseLootSet,double>( set, chance ) );
		}

		public BaseLootTemplate( int totalvalue )
		{
			m_TotalValue = totalvalue;

			if ( LootSets == null || LootSets.Count == 0 )
			{
				LootSets = new List<Tuple<BaseLootSet,double>>();
				InitializeTemplate();
			}
		}
	}
}