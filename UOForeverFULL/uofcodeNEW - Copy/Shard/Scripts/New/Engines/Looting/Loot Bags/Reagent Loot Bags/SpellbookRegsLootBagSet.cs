using System;
using System.Collections.Generic;
using Server.Items;

namespace Server
{
	public class SpellbookRegsLootBagSet : AllRegsLootBagSet
	{
		private MagicGrade m_SpellbookGrade;
		private double m_BookChance;

		public SpellbookRegsLootBagSet() : this( 0.25, MagicGrade.NonMagical )
		{
		}

		public SpellbookRegsLootBagSet( double bookChance, MagicGrade grade ) : base( 60 )
		{
			m_SpellbookGrade = grade;
			m_BookChance = bookChance;
		}

		public SpellbookRegsLootBagSet( int amount ) : this( amount, 0.25, MagicGrade.NonMagical )
		{
		}

		public SpellbookRegsLootBagSet( int amount, double bookChance, MagicGrade grade ) : base( amount, amount )
		{
			m_SpellbookGrade = grade;
			m_BookChance = bookChance;
		}

		public SpellbookRegsLootBagSet( int mina, int maxa ) : this( mina, maxa, 0.25, MagicGrade.NonMagical )
		{
		}

		public SpellbookRegsLootBagSet( int mina, int maxa, double bookChance, MagicGrade grade ) : base( mina, maxa )
		{
			m_SpellbookGrade = grade;
			m_BookChance = bookChance;
		}

		public SpellbookRegsLootBagSet( int minc, int maxc, bool random, int mina, int maxa ) : this( minc, maxc, random, mina, maxa, 0.25, MagicGrade.NonMagical )
		{
		}

		public SpellbookRegsLootBagSet( int minc, int maxc, bool random, int mina, int maxa, double bookChance, MagicGrade grade ) : base( minc, maxc, random, mina, maxa )
		{
			m_SpellbookGrade = grade;
			m_BookChance = bookChance;
		}

		public override void AddContents( BaseContainer cont, Mobile creature, out int contentValue )
		{
			base.AddContents( cont, creature, out contentValue );

			if ( m_BookChance > Utility.RandomDouble() )
			{
				BaseSpellbookLootSet spellbookloot = MagicSpellbookLootSets.LootSet( m_SpellbookGrade );

				if ( spellbookloot != null ) //sanity check
				{
					Tuple<Item[], int> spelltuple = spellbookloot.GenerateLootItem( creature );
					if ( spelltuple.Item1 != null ) // another sanity check
						for ( int i = 0;i < spelltuple.Item1.Length; i++ )
							if ( spelltuple.Item1[i] != null ) // more sanity checks
								cont.DropItem( spelltuple.Item1[i] );

					contentValue += spelltuple.Item2;
				}
			}
		}
	}
}