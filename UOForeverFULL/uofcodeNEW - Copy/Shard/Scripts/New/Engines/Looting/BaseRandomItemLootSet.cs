using System;
using Server.Items;

namespace Server
{
	public abstract class BaseRandomItemLootSet : BasicLootSet
	{
		private int m_MinAmount;
		public int MinAmount{ get{ return m_MinAmount; } }

		private int m_MaxAmount;
		public int MaxAmount{ get{ return m_MaxAmount; } }

		private Type[] m_ItemTypes;

		public BaseRandomItemLootSet( int min, int max, Type[] itemTypes ) : base()
		{
			m_MinAmount = min;
			m_MaxAmount = max;
			m_ItemTypes = itemTypes;
		}

		public virtual Type GetRandomType()
		{
			return m_ItemTypes[Utility.Random(m_ItemTypes.Length)];
		}

		public virtual Item GenerateItem( Type type )
		{
			if ( typeof( Item ).IsAssignableFrom( type ) )
				return Utility.CreateInstance<Item>( type );
			else
				throw new Exception( String.Format( "Type {0} is not assignable to type 'Server.Item'", type.FullName ) );
		}

		public override Item GenerateItem()
		{
			return GenerateItem(GetRandomType());
		}

		public override Tuple<Item[],int> GenerateLootItem( Mobile creature )
		{
			int amount = Utility.RandomMinMax( m_MinAmount, m_MaxAmount );

			Item[] items = new Item[amount];

			for ( int i = 0; i < amount; i++ )
				items[i] = GenerateItem();

			return new Tuple<Item[],int>( items, BaseValue * amount );
		}
	}
}