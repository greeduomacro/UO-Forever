using System;
using Server.Items;

namespace Server
{
	public class StackableFoodLootSet : BaseRandomItemLootSet
	{
		public override int BaseValue{ get{ return 10; } }

		private static Type[] m_FoodTypes = new Type[]
			{
				typeof( BreadLoaf ),		typeof( Bacon ),		typeof( FishSteak ),	typeof( CheeseWheel ),	typeof( CheeseWedge ),
				typeof( FrenchBread ),		typeof( FriedEggs ),	typeof( CookedBird ),	typeof( RoastPig ),		typeof( Sausage ),
				typeof( Ham ),				typeof( Ribs ),			typeof( CheesePizza ),	typeof( SausagePizza ),	typeof( Peach ),
				typeof( HoneydewMelon ),	typeof( YellowGourd ),	typeof( GreenGourd ),	typeof( Banana ),		typeof( SplitCoconut ),
				typeof( Lemon ),			typeof( Lime ),			typeof( Coconut ),		typeof( Dates ),		typeof( Grapes ),
				typeof( Pear ),				typeof( Apple ),		typeof( Watermelon ),	typeof( Squash ),		typeof( Cantaloupe ),
				typeof( Carrot ),			typeof( Cabbage ),		typeof( Onion ),		typeof( Lettuce ),		typeof( Pumpkin ),
				typeof( LambLeg ),			typeof( ChickenLeg ),	typeof( Bananas )
			};

		public StackableFoodLootSet( int amt ) : this( amt, amt )
		{
		}

		public StackableFoodLootSet( int min, int max ) : base( min, max, m_FoodTypes )
		{
		}
	}

	public class NonStackableFoodLootSet : BaseRandomItemLootSet
	{
		public override int BaseValue{ get{ return 10; } }

		private static Type[] m_FoodTypes = new Type[]
			{
				typeof( Cake ),				typeof( Cookies ),		typeof( Muffins ),		typeof( FruitPie ),		typeof( MeatPie ),
				typeof( PumpkinPie ),		typeof( ApplePie ),		typeof( PeachCobbler ),	typeof( Quiche )
			};

		public NonStackableFoodLootSet( int amt ) : this( amt, amt )
		{
		}

		public NonStackableFoodLootSet( int min, int max ) : base( min, max, m_FoodTypes )
		{
		}
	}

	public class FoodLootSet : BaseRandomItemLootSet
	{
		public override int BaseValue{ get{ return 10; } }

		private static Type[] m_FoodTypes = new Type[]
			{
				typeof( BreadLoaf ),		typeof( Bacon ),		typeof( FishSteak ),	typeof( CheeseWheel ),	typeof( CheeseWedge ),
				typeof( FrenchBread ),		typeof( FriedEggs ),	typeof( CookedBird ),	typeof( RoastPig ),		typeof( Sausage ),
				typeof( Ham ),				typeof( Ribs ),			typeof( CheesePizza ),	typeof( SausagePizza ),	typeof( Peach ),
				typeof( HoneydewMelon ),	typeof( YellowGourd ),	typeof( GreenGourd ),	typeof( Banana ),		typeof( SplitCoconut ),
				typeof( Lemon ),			typeof( Lime ),			typeof( Coconut ),		typeof( Dates ),		typeof( Grapes ),
				typeof( Pear ),				typeof( Apple ),		typeof( Watermelon ),	typeof( Squash ),		typeof( Cantaloupe ),
				typeof( Carrot ),			typeof( Cabbage ),		typeof( Onion ),		typeof( Lettuce ),		typeof( Pumpkin ),
				typeof( LambLeg ),			typeof( ChickenLeg ),	typeof( Bananas ),		typeof( Cake ),			typeof( Cookies ),
				typeof( Muffins ),			typeof( FruitPie ),		typeof( MeatPie ),		typeof( PumpkinPie ),	typeof( ApplePie ),
				typeof( PeachCobbler ),		typeof( Quiche )
			};

		public FoodLootSet( int amt ) : this( amt, amt )
		{
		}

		public FoodLootSet( int min, int max ) : base( min, max, m_FoodTypes )
		{
		}
	}
}