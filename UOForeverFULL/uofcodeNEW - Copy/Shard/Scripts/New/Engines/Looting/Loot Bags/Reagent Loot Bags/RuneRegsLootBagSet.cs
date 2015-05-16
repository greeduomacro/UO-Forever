using System;
using System.Collections.Generic;
using Server.Items;

namespace Server
{
	public class RuneRegsLootBagSet : RecallRegsLootBagSet
	{
		private double m_RuneChance;

		public RuneRegsLootBagSet() : this( 30, 0.50 )
		{
		}

		public RuneRegsLootBagSet( double runeChance ) : base( 30 )
		{
			m_RuneChance = runeChance;
		}

		public RuneRegsLootBagSet( int amount ) : this( amount, 0.50 )
		{
		}

		public RuneRegsLootBagSet( int amount, double runeChance ) : base( amount, amount )
		{
			m_RuneChance = runeChance;
		}

		public RuneRegsLootBagSet( int mina, int maxa ) : this( mina, maxa, 0.50 )
		{
		}

		public RuneRegsLootBagSet( int mina, int maxa, double runeChance ) : base( mina, maxa )
		{
			m_RuneChance = runeChance;
		}

		public override void AddContents( BaseContainer cont, Mobile creature, out int contentValue )
		{
			base.AddContents( cont, creature, out contentValue );

			if ( m_RuneChance > Utility.RandomDouble() )
				cont.DropItem( new RecallRune() );

			int scrollcount = Utility.RandomMinMax( MinAmount / 25, MaxAmount / 8 );

			for ( int i = 0; i < scrollcount; i++ )
			{
				if ( (m_RuneChance / 3.0) > Utility.RandomDouble() )
				{
					Item item = null;
					switch ( Utility.Random( 5 ) )
					{
						default:
						case 0: case 1: case 2: item = new RecallScroll(); break;
						case 3: item = new MarkScroll(); break;
						case 4: item = new GateTravelScroll(); break;
					}

					cont.DropItem( item );
				}
			}
		}
	}
}