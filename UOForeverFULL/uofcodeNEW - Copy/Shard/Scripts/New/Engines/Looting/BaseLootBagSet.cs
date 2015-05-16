using System;
using System.Collections.Generic;
using Server.Items;
using Server.Multis;

namespace Server
{
	public abstract class BaseLootBagSet : BaseLootSet
	{
		private Type[] m_ContainerTypes;

		public abstract void AddContents( BaseContainer cont, Mobile creature, out int contentValue );

		public BaseLootBagSet( params Type[] contTypes ) : base()
		{
			m_ContainerTypes = contTypes;
		}

		public override Tuple<Item[],int> GenerateLootItem( Mobile creature )
		{
			Type type = m_ContainerTypes[Utility.Random( m_ContainerTypes.Length )];

			BaseContainer cont = Activator.CreateInstance( type ) as BaseContainer;
			
			if ( cont == null )
				throw new NullReferenceException( String.Format( "Type {0} is not a BaseContainer or could not be instantiated.", type.ToString() ) );
			else
			{
				int contentValue;
				AddContents( cont, creature, out contentValue );
				contentValue += BaseValue;

				return new Tuple<Item[],int>( new Item[]{ cont }, contentValue );
			}
		}

		public static Type[] BasicBags = new Type[]
			{
				typeof( Backpack ), typeof( Pouch ), typeof( Bag )
			};

		public static Type[] TreasureChests = new Type[]
			{
				typeof( MetalChest ), typeof( MetalGoldenChest ), typeof( WoodenChest )
			};

		public static Type[] Crates = new Type[]
			{
				typeof( SmallCrate ), typeof( MediumCrate ), typeof( LargeCrate )
			};

		public static Type[] Boxes = new Type[]
			{
				typeof( WoodenBox ), typeof( MetalBox )
			};

		public static Type[] AnyLockable = new Type[] // And trapable!
			{
				typeof( MetalChest ), typeof( MetalGoldenChest ), typeof( WoodenChest ),
				typeof( SmallCrate ), typeof( MediumCrate ), typeof( LargeCrate ),
				typeof( WoodenBox ), typeof( MetalBox ), typeof( LockableBarrel )
			};
	}
}