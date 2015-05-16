using System;
using System.Collections.Generic;
using Server.Items;

namespace Server
{
	public class LootCollection
	{
		private LootPackDice m_Gold;
		private int m_TotalValue;
		private List<Tuple<BaseLootSet,double>> m_LootSets;

		public LootPackDice Gold{ get{ return m_Gold; } set{ m_Gold = value; } }
		public int TotalValue{ get{ return m_TotalValue; } set{ m_TotalValue = value; } }

		public LootCollection( string gold )
		{
			m_Gold = new LootPackDice( gold );
			m_LootSets = new List<Tuple<BaseLootSet,double>>();
		}

		public void AddLoot( BaseLootTemplate template )
		{
			AddLoot( template, template.TotalValue );
		}

		public void AddLoot( BaseLootTemplate template, int value )
		{
			AddLoot( template, 1.0, value );
		}

		public void AddLoot( BaseLootTemplate template, double scalar )
		{
			m_TotalValue += template.TotalValue;

			for ( int i = 0;i < template.LootSets.Count; i++ )
				AddLoot( template.LootSets[i], scalar );
		}

		public void AddLoot( BaseLootTemplate template, double scalar, int value )
		{
			//m_TotalValue += template.TotalValue;
			m_TotalValue += value;

			for ( int i = 0;i < template.LootSets.Count; i++ )
				AddLoot( template.LootSets[i], scalar );
		}

		public void AddLoot( BaseLootSet set )
		{
			AddLoot( set, 1.0 );
		}

		public void AddLoot( BaseLootSet set, double chance )
		{
			AddLoot( set, chance, set.BaseValue );
		}

		public void AddLoot( BaseLootSet set, double chance, int value )
		{
			chance = Math.Min( Math.Max( chance, 0.0 ), 1.0 );

			AddLoot( new Tuple<BaseLootSet,double>( set, chance ), value );
		}

		public void AddLoot( Tuple<BaseLootSet,double> tuple )
		{
			AddLoot( tuple, 1.0 );
		}

		public void AddLoot( Tuple<BaseLootSet,double> tuple, int value )
		{
			AddLoot( tuple, 1.0, value );
		}

		public void AddLoot( Tuple<BaseLootSet,double> tuple, double scalar )
		{
			AddLoot( tuple, scalar, (int)(tuple.Item1.BaseValue * scalar) );
		}

		public void AddLoot( Tuple<BaseLootSet,double> tuple, double scalar, int value )
		{
			m_TotalValue += value;

			Tuple<BaseLootSet,double> t = tuple;

			if ( scalar != 1.0 )
				t = new Tuple<BaseLootSet,double>( tuple.Item1, tuple.Item2 * scalar );

			m_LootSets.Add( t );
		}

		public void GenerateGold( Mobile creature, Container c )
		{
			c.TryDropItem( creature, new Gold( m_Gold.Roll() ), false, false );
		}

		public void GenerateLoot( Mobile creature, Container c )
		{
			int value = m_TotalValue;

			//do
			//{
				for ( int i = 0; value > 0 && i < m_LootSets.Count; i++ )
				{
					Tuple<BaseLootSet,double> set = m_LootSets[i];

					if ( set.Item2 > Utility.RandomDouble() )
					{
						Tuple<Item[],int> lootitem = set.Item1.GenerateLootItem( creature );

						if ( value - lootitem.Item2 >= 0 )
						{
							for ( int j = 0;j < lootitem.Item1.Length; j++ )
								c.DropItem( lootitem.Item1[j] );

							value -= lootitem.Item2;
						}
						else
						{
							for ( int j = 0;j < lootitem.Item1.Length; j++ )
								lootitem.Item1[j].Delete();
						}
					}
				}
			//}
			//while ( ((double)value / m_TotalValue / 2.0) > Utility.RandomDouble() );

			if ( LootSystem.ExtraLootAsGold && (value = Utility.RandomMinMax( value / 10, value / 5 )) > 0 ) //We have some left, lets give it as partial gold
				c.TryDropItem( creature, new Gold( value ), false, false );
		}
	}
}