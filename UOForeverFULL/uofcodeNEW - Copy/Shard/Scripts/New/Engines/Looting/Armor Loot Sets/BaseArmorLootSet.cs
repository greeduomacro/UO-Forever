using System;
using Server.Items;

namespace Server
{
	public class BaseArmorLootSet : BaseLootSet
	{
		private int m_MinIntensity;
		private int m_MaxIntensity;
		private Type[] m_ArmorTypes;

		public override int BaseValue{ get{ return GetMaxValue(); } }

		public BaseArmorLootSet( int minInt, int maxInt, params Type[] weapTypes ) : base()
		{
			m_MinIntensity = minInt;
			m_MaxIntensity = maxInt;
			m_ArmorTypes = weapTypes;
		}

		public override Tuple<Item[],int> GenerateLootItem( Mobile creature )
		{
			Type type = m_ArmorTypes[Utility.Random(m_ArmorTypes.Length)];
			BaseArmor armor = Activator.CreateInstance( type ) as BaseArmor;

			if ( armor == null )
				throw new Exception( String.Format( "Type {0} is not BaseArmor or could not be instantiated.", type ) );

			int value = 100;

			if ( 0.45 > Utility.RandomDouble() )
			{
				int bonus = GetBonus();
				armor.ProtectionLevel = (ArmorProtectionLevel)bonus;
				value += 40 * bonus;
			}

			if ( 0.25 > Utility.RandomDouble() )
			{
				int bonus = GetBonus();

				armor.Durability = (ArmorDurabilityLevel)bonus;
				value += 40 * bonus;
			}

			if ( (armor.Hue == 0 || armor is StuddedCap) && 0.01 > Utility.RandomDouble() )
			{
				if ( armor.Resource == CraftResource.Iron && GetBonus() == 5 && 0.01 > Utility.RandomDouble() )
				{
					if ( Utility.RandomBool() )
						MakeBloodRockArmor( armor );
					else
						MakeBlackRockArmor( armor );

					value += 1500;
				}
			}

			return new Tuple<Item[], int>( new Item[]{ armor }, value );
		}

		public void MakeBloodRockArmor( BaseArmor armor )
		{
			armor.Resource = CraftResource.BloodRock;
		}

		public void MakeBlackRockArmor( BaseArmor armor )
		{
			armor.Resource = CraftResource.BlackRock;
		}

		public int GetLowRareHue()
		{
			int hue = -1;

			switch ( Utility.Random( 5 ) )
			{
				case 0: hue = 1401 + (9 * Utility.Random( 6 )) + Utility.Random( 3 ); break;
				case 1: hue = 1801 + (9 * Utility.Random( 12 )) + Utility.Random( 2 ); break;
				case 2: hue = 2301 + (9 * Utility.Random( 3 )); break;
				case 3: hue = 2401 + (9 * Utility.Random( 5 )) + Utility.Random( 2 ); break;
				case 4: hue = 1134 + (9 * Utility.Random( 2 )) + Utility.Random( 3 ); break;
			}

			return hue; //These are not reserved for armors.
		}

		private int GetMaxValue()
		{
			if ( m_MaxIntensity > 96 )
				return 500;
			else if ( m_MaxIntensity > 88 )
				return 400;
			else if ( m_MaxIntensity > 74 )
				return 300;
			else if ( m_MaxIntensity > 49 )
				return 200;
			else
				return 100;
		}

		private int GetBonus()
		{
			if ( m_MinIntensity == 0 && m_MaxIntensity == 0 )
				return 0;

			int rnd = Utility.RandomMinMax( m_MinIntensity, m_MaxIntensity );

			if ( rnd > 97 )
				return 5;
			else if ( rnd > 89 )
				return 4;
			else if ( rnd > 75 )
				return 3;
			else if ( rnd > 50 )
				return 2;
			else
				return 1;
		}
	}
}