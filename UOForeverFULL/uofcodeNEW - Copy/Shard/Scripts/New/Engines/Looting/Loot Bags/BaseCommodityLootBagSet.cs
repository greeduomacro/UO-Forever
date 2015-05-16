using System;
using System.Collections.Generic;
using Server.Items;

namespace Server
{
	public abstract class BaseCommodityLootBagSet : BaseLootBagSet
	{
		private Type[] m_CommodityTypes;
		private int m_MinCount;
		private int m_MaxCount;
		private int m_MinAmount;
		private int m_MaxAmount;
		private bool m_Random; //Otherwise in order

		public int MinCount{ get{ return m_MinCount; } }
		public int MaxCount{ get{ return m_MaxCount; } }
		public int MinAmount{ get{ return m_MinAmount; } }
		public int MaxAmount{ get{ return m_MaxAmount; } }

		public override int BaseValue{ get{ return 100; } }

		public BaseCommodityLootBagSet( int count, bool random, int amount, Type[] comTypes ) : this( count, count, random, amount, amount, comTypes )
		{
		}

		public BaseCommodityLootBagSet( int count, bool random, int amount, Type[] comTypes, Type[] contTypes ) : this( count, count, random, amount, amount, comTypes, contTypes )
		{
		}

		public BaseCommodityLootBagSet( int count, bool random, int mina, int maxa, Type[] comTypes ) : this( count, count, random, mina, maxa, comTypes )
		{
		}

		public BaseCommodityLootBagSet( int minc, int maxc, bool random, int amount, Type[] comTypes ) : this( minc, maxc, random, amount, amount, comTypes, BasicBags )
		{
		}

		public BaseCommodityLootBagSet( int minc, int maxc, bool random, int mina, int maxa, Type[] comTypes ) : this( minc, maxc, random, mina, maxa, comTypes, BasicBags )
		{
		}

		public BaseCommodityLootBagSet( int count, bool random, int mina, int maxa, Type[] comTypes, Type[] contTypes ) : this( count, count, random, mina, maxa, comTypes, contTypes )
		{
		}

		public BaseCommodityLootBagSet( int minc, int maxc, bool random, int amount, Type[] comTypes, Type[] contTypes ) : this( minc, maxc, random, amount, amount, comTypes, contTypes )
		{
		}

		public BaseCommodityLootBagSet( int minc, int maxc, bool random, int mina, int maxa, Type[] comTypes, Type[] contTypes ) : base( contTypes )
		{
			m_MinCount = minc;
			m_MaxCount = maxc;
			m_MinAmount = mina;
			m_MaxAmount = maxa;
			m_Random = random;

			m_CommodityTypes = comTypes;
		}

		public override void AddContents( BaseContainer cont, Mobile creature, out int contentValue )
		{
			contentValue = 0;

			int count = Utility.RandomMinMax( m_MinCount, m_MaxCount );

			for ( int i = 0; i < count; i++ )
			{
				Type type = null;

				if ( m_Random )
					type = m_CommodityTypes[Utility.Random(m_CommodityTypes.Length)];
				else
					type = m_CommodityTypes[i % m_CommodityTypes.Length];

				Item com = Activator.CreateInstance( m_CommodityTypes[i] ) as Item;

				if ( com == null )
					throw new NullReferenceException( String.Format( "Type {0} is not an Item or could not be instantiated.", type.ToString() ) );
				else
				{
					if ( com.Stackable )
						com.Amount = Utility.RandomMinMax( m_MinAmount, m_MaxAmount );

					cont.DropItem( com );
				}
			}
		}
	}
}