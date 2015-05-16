using System;
using Server.Items;

namespace Server
{
	public abstract class BaseRandomItemScaledLootSet : BasicLootSet
	{
		private Tuple<Type,int>[] m_ItemTypes;
		private int m_Total;

		public BaseRandomItemScaledLootSet( params Tuple<Type,int>[] itemTypes ) : base()
		{
			m_ItemTypes = itemTypes;
			for ( int i = 0;i < m_ItemTypes.Length; i++ )
				m_Total += m_ItemTypes[i].Item2;
		}

		public virtual Type GetRandomType()
		{
			int rnd = Utility.RandomMinMax( 1, m_Total );

			for ( int i = 0; i < m_ItemTypes.Length; i++ )
			{
				int chance = m_ItemTypes[i].Item2;

				if ( rnd <= chance )
					return m_ItemTypes[i].Item1;
				else
					rnd -= chance;
			}

			return m_ItemTypes[0].Item1;
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
			return new Tuple<Item[],int>( new Item[]{ GenerateItem() }, BaseValue );
		}
	}
}