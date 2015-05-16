using System;
using Server.Items;

namespace Server
{
	public class BaseSpellbookLootSet : BaseLootSet
	{
		private int m_MinIntensity;
		private int m_MaxIntensity;

		public override int BaseValue{ get{ return GetMaxValue(); } }

		public BaseSpellbookLootSet( int minInt, int maxInt ) : base()
		{
			m_MinIntensity = minInt;
			m_MaxIntensity = maxInt;
		}

		public override Tuple<Item[],int> GenerateLootItem( Mobile creature )
		{
			Spellbook book = new Spellbook();

			int value = 100;

			int rnd = Utility.RandomMinMax( m_MinIntensity, m_MaxIntensity );

			int circle = (int)((rnd / 12.5)+1.0);
			if ( circle >= 8 && 0.10 > Utility.RandomDouble() )
				book.Content = ulong.MaxValue;
			else
			{
				circle = Math.Min( circle, 8 );
				//do we fill this circle?
				for ( int i = 0; i < circle; i++ )
					if ( Utility.RandomBool() )
						book.Content |= (ulong)Utility.Random( 0x100 ) << (i*8);
			}

			if ( 0.03 > Utility.RandomDouble() && GetBonus() >= 4 )
			{
				//mark for blessing
				book.SetSavedFlag( 0x02, true );
				//book.LootType = LootType.Blessed;
				value += 100;
			}
			
			if ( 0.01 > Utility.RandomDouble() && GetBonus() == 5 )
			{
				book.Dyable = true;
				value += 100;
			}

			if ( 0.01 > Utility.RandomDouble() )
			{
				value += 250;

				int bonus = GetBonus();

				if ( bonus == 5 )
				{
					if ( 0.01 > Utility.RandomDouble() )
						book.Hue = GetOreHue();
					else if ( 0.01 > Utility.RandomDouble() )
						book.Hue = 1072; //Iron
					else
						book.Hue = GetRareHue();

					value += 500;
				}
				else if ( bonus >= 3 )
					book.Hue = GetCommonHue();
			}

			return new Tuple<Item[],int>( new Item[]{ book }, value );
		}

		private int GetRareHue()
		{
			return 1102 + (Utility.Random( 6 ) * Utility.Random( 4 ));
		}

		private int GetCommonHue()
		{
			return 1801 + (Utility.Random( 12 ) * Utility.Random( 4 ));
		}

		private int GetOreHue()
		{
			int rnd = Utility.Random( 1000 );
			CraftResource resource = CraftResource.None;

			if ( rnd >= 995 ) // 5/1000 -> 1/200
				resource = CraftResource.Valorite;
			else if ( rnd >= 985 ) // 10/1000 -> 1/100
				resource = CraftResource.Verite;
			else if ( rnd >= 965 ) // 20/1000 -> 1/50
				resource = CraftResource.Agapite;
			else if ( rnd >= 925 ) // 40/1000 -> 1/25
				resource = CraftResource.Gold;
			else if ( rnd >= 845 ) // 80/1000 -> 1/12.5
				resource = CraftResource.Bronze;
			else if ( rnd >= 685 ) // 160/1000 -> 1/6.25
				resource = CraftResource.Copper;
			else if ( rnd >= 320 ) // 320/1000 -> 1/3.125
				resource = CraftResource.ShadowIron;
			else
				resource = CraftResource.DullCopper;

			return CraftResources.GetInfo( resource ).Hue;
		}

		private int GetMaxValue()
		{
			if ( m_MaxIntensity > 98 )
				return 450;
			else if ( m_MaxIntensity > 88 )
				return 350;
			else if ( m_MaxIntensity > 74 )
				return 250;
			else if ( m_MaxIntensity > 49 )
				return 150;
			else
				return 100;
		}

		private int GetCircle()
		{
			if ( m_MinIntensity == 0 && m_MaxIntensity == 0 )
				return 1;
			else if ( m_MinIntensity == 100 && m_MaxIntensity == 100 )
				return 8;

			int rnd = Utility.RandomMinMax( m_MinIntensity, m_MaxIntensity );

			if ( rnd > 98 )
				return 8;
			else if ( rnd > 94 )
				return 7;
			else if ( rnd > 85 )
				return 6;
			else if ( rnd > 78 )
				return 5;
			else if ( rnd > 70 )
				return 4;
			else if ( rnd > 60 )
				return 3;
			else if ( rnd > 30 )
				return 2;
			else
				return 1;
		}

		private int GetBonus()
		{
			if ( m_MinIntensity == 0 && m_MaxIntensity == 0 )
				return 0;

			int rnd = Utility.RandomMinMax( m_MinIntensity, m_MaxIntensity );

			if ( rnd > 98 )
				return 5;
			else if ( rnd > 90 )
				return 4;
			else if ( rnd > 75 )
				return 3;
			else if ( rnd > 50 )
				return 2;
			else
				return 1;
		}
	}

	public class MagicSpellbookLootSets : MagicLootSets<BaseSpellbookLootSet>
	{
		public static void Configure()
		{
			GenerateLootSets();
		}
	}
}