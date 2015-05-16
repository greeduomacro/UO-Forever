using System;
using Server.Items;

namespace Server
{
	public class BodyClothingLootSet : BaseClothingLootSet
	{
		private static readonly Type[] ClothingTypes = new Type[]
			{
				typeof( Cloak ),				typeof( Surcoat ),				typeof( BodySash ),
				typeof( Doublet ),				typeof( FullApron ),			typeof( JesterSuit ),
				typeof( Tunic ),				typeof( Shirt ),				typeof( Kilt ),
				typeof( Skirt ),				typeof( FancyShirt ),			typeof( FancyDress ),
				typeof( LongPants ),			typeof( PlainDress ),           typeof( Robe ),
				typeof( ShortPants ),			typeof( HalfApron )
			};

		public BodyClothingLootSet( double hueChance, double rareHueScalar ) : base( hueChance, rareHueScalar, ClothingTypes )
		{
		}
	}

	public class HatLootSet : BaseClothingLootSet
	{
		private static readonly Type[] ClothingTypes = new Type[]
			{
				typeof( Bonnet ),               typeof( Cap ),		            typeof( FeatheredHat ),
				typeof( FloppyHat ),            typeof( JesterHat ),			typeof( SkullCap ),
				typeof( StrawHat ),	            typeof( TallStrawHat ),			typeof( TricorneHat ),
				typeof( WideBrimHat ),          typeof( WizardsHat )
			};

		public HatLootSet( double hueChance, double rareHueScalar ) : base( hueChance, rareHueScalar, ClothingTypes )
		{
		}
	}

	public class ShoesLootSet : BaseClothingLootSet
	{
		private static readonly Type[] ClothingTypes = new Type[]
			{
				typeof( Boots ),				typeof( Sandals ),				typeof( Shoes ),
				typeof( ThighBoots )
			};

		public ShoesLootSet( double hueChance, double rareHueScalar ) : base( hueChance, rareHueScalar, ClothingTypes )
		{
		}
	}
}