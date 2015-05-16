using System;
using System.Collections.Generic;
using Server.Items;

namespace Server
{
	public class HealLootBagSet : HealRegsLootBagSet
	{
		private double m_HealChance;

		public HealLootBagSet() : this( 30, 0.40 )
		{
		}

		public HealLootBagSet( double healChance ) : base( 30 )
		{
			m_HealChance = healChance;
		}

		public HealLootBagSet( int amount ) : this( amount, 0.40 )
		{
		}

		public HealLootBagSet( int amount, double healChance ) : base( amount, amount )
		{
			m_HealChance = healChance;
		}

		public HealLootBagSet( int mina, int maxa ) : this( mina, maxa, 0.40 )
		{
		}

		public HealLootBagSet( int mina, int maxa, double healChance ) : base( mina, maxa )
		{
			m_HealChance = healChance;
		}

		public override void AddContents( BaseContainer cont, Mobile creature, out int contentValue )
		{
			base.AddContents( cont, creature, out contentValue );

			if ( m_HealChance > Utility.RandomDouble() )
				cont.DropItem( new Bandage( Utility.RandomMinMax( MinAmount / 2, MaxAmount ) ) );

			int potioncount = Utility.RandomMinMax( MinAmount / 25, MaxAmount / 10 );

			for ( int i = 0; i < potioncount; i++ )
				if ( (m_HealChance / 2.0) > Utility.RandomDouble() )
					cont.DropItem( Utility.RandomBool() ? (Item)new LesserHealPotion() : (Item)new HealPotion() );

			int scrollcount = Utility.RandomMinMax( MinAmount / 12, MaxAmount / 5 );

			for ( int i = 0; i < scrollcount; i++ )
			{
				if ( (m_HealChance / 3.0) > Utility.RandomDouble() )
				{
					Item item = null;
					switch ( Utility.Random( 4 ) )
					{
						default:
						case 0: item = new HealScroll(); break;
						case 1: item = new GreaterHealScroll(); break;
						case 2: item = new CureScroll(); break;
						case 3: item = new ArchCureScroll(); break;
					}

					cont.DropItem( item );
				}
			}
		}
	}
}