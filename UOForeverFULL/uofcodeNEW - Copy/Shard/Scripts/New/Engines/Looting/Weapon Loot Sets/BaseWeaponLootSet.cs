using System;
using Server.Items;
using Server.Games;

namespace Server
{
	public class BaseWeaponLootSet : BaseLootSet
	{
		private int m_MinIntensity;
		private int m_MaxIntensity;
		private Type[] m_WeaponTypes;

		public override int BaseValue{ get{ return GetMaxValue(); } }

		public BaseWeaponLootSet( int minInt, int maxInt, params Type[] weapTypes ) : base()
		{
			m_MinIntensity = minInt;
			m_MaxIntensity = maxInt;
			m_WeaponTypes = weapTypes;
		}

		public override Tuple<Item[],int> GenerateLootItem( Mobile creature )
		{
			Type type = m_WeaponTypes[Utility.Random(m_WeaponTypes.Length)];
			BaseWeapon weapon = Activator.CreateInstance( type ) as BaseWeapon;

			if ( weapon == null )
				throw new Exception( String.Format( "Type {0} is not BaseWeapon or could not be instantiated.", type ) );

			int value = 100;

			if ( 0.40 > Utility.RandomDouble() )
			{
				int bonus = GetBonus();
				weapon.AccuracyLevel = (WeaponAccuracyLevel)bonus;
				value += 20 * bonus;
			}

			if ( 0.30 > Utility.RandomDouble() )
			{
				int bonus = GetBonus();
                if (PseudoSeerStone.Instance != null && PseudoSeerStone.Instance._HighestDamageLevelSpawn < bonus)
                {
                    bonus = PseudoSeerStone.Instance._HighestDamageLevelSpawn;
                }
				weapon.DamageLevel = (WeaponDamageLevel)bonus;
				value += 20 * bonus;
			}

			if ( 0.25 > Utility.RandomDouble() )
			{
				int bonus = GetBonus();
				weapon.DurabilityLevel = (WeaponDurabilityLevel)bonus;
				value += 20 * bonus;
			}

			if ( GetBonus() > 2 )
			{
				if ( 0.10 > Utility.RandomDouble() )
				{
					weapon.Slayer = SlayerName.Silver;
					value += 100;
				}

				if ( 0.01 > Utility.RandomDouble() || ( weapon.AccuracyLevel == 0 && weapon.DamageLevel == 0 && weapon.DurabilityLevel == 0 && weapon.Slayer == SlayerName.None && 0.15 > Utility.RandomDouble() ) )
				{
					if ( creature != null )
						weapon.Slayer = SlayerGroup.GetLootSlayerType( creature.GetType() );
					else
						weapon.Slayer = BaseRunicTool.GetRandomSlayer();

					value += 100;
				}
			}

			if ( weapon.Hue == 0 && 0.01 > Utility.RandomDouble() )
			{
				int bonus = GetBonus();

				if ( bonus > 2 && 0.045 > Utility.RandomDouble() )
				{
					weapon.Hue = Math.Max( 0, GetLowRareHue() );
					value += 600;
				}
				else if ( bonus > 3 && 0.005 > Utility.RandomDouble() )
				{
					weapon.Hue = Math.Max( 0, GetHighRareHue() );
					value += 1200;
				}
				else if ( weapon.Resource != CraftResource.RegularWood && bonus == 5 && 0.001 > Utility.RandomDouble() )
				{
					weapon.Hue = Math.Max( 0, GetVeryRareHue() );
					value += 1550;
				}
			}

			return new Tuple<Item[],int>( new Item[]{ weapon }, value );
		}

		public int GetHighRareHue()
		{
			int hue = -1;

			switch ( Utility.Random( 5 ) )
			{
				case 0: hue = 1405 + (9 * Utility.Random( 6 )) + Utility.Random( 3 ); break;
				case 1: hue = 1805 + (9 * Utility.Random( 12 )) + Utility.Random( 2 ); break;
				case 2: hue = 2305 + (9 * Utility.Random( 3 )); break;
				case 3: hue = 2405 + (9 * Utility.Random( 5 )) + Utility.Random( 2 ); break;
				case 4: hue = 1138 + (9 * Utility.Random( 2 )) + Utility.Random( 3 ); break;
			}

			return hue; //These are not reserved for weapons.
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

			return hue; //These are not reserved for weapons.
		}

		public int GetVeryRareHue()
		{
			int hue = -1;

			switch ( Utility.Random( 3 ) )
			{
				case 0: hue = 2958 + Utility.Random( 5 ); break;
				case 1: hue = 2949; break;
				case 2: hue = 2953; break;
			}

			return hue; //These are not reserved for weapons.
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