using System;
using Server.Items;
using Server.Mobiles;

namespace Server
{
	public class BaseClothingLootSet : BaseLootSet
	{
		private double m_HueChance;
		private double m_RareHueScalar;
		private Type[] m_ClothingTypes;
/*
		private static Type[] m_ClothingTypes = new Type[]
			{
				typeof( Cloak ),
				typeof( Bonnet ),               typeof( Cap ),		            typeof( FeatheredHat ),
				typeof( FloppyHat ),            typeof( JesterHat ),			typeof( Surcoat ),
				typeof( SkullCap ),             typeof( StrawHat ),	            typeof( TallStrawHat ),
				typeof( TricorneHat ),			typeof( WideBrimHat ),          typeof( WizardsHat ),
				typeof( BodySash ),             typeof( Doublet ),              typeof( Boots ),
				typeof( FullApron ),            typeof( JesterSuit ),           typeof( Sandals ),
				typeof( Tunic ),				typeof( Shoes ),				typeof( Shirt ),
				typeof( Kilt ),                 typeof( Skirt ),				typeof( FancyShirt ),
				typeof( FancyDress ),			typeof( ThighBoots ),			typeof( LongPants ),
				typeof( PlainDress ),           typeof( Robe ),					typeof( ShortPants ),
				typeof( HalfApron )
			};
*/
		public override int BaseValue{ get{ return 100; } }

		public BaseClothingLootSet( double hueChance, double rareHueScalar, params Type[] clothTypes ) : base()
		{
			m_RareHueScalar = Math.Max( rareHueScalar, 0.0 );
			m_HueChance = Math.Max( Math.Min( hueChance, 1.0 ), 0.0 );
			m_ClothingTypes = clothTypes;
		}

		public override Tuple<Item[],int> GenerateLootItem( Mobile creature )
		{
			Type type = m_ClothingTypes[Utility.Random(m_ClothingTypes.Length)];
			BaseClothing clothing = Activator.CreateInstance( type ) as BaseClothing;

			if ( clothing == null )
				throw new Exception( String.Format( "Type {0} is not BaseClothing or could not be instantiated.", type ) );

			int value = 50;

			if ( m_HueChance > Utility.RandomDouble() )
			{
				int rnd = Utility.Random( 10000 );
				double veryrare = 2 * m_RareHueScalar;
				double rare = veryrare + (299 * m_RareHueScalar);
				double uncommon = rare + (1500 * m_RareHueScalar);

				if ( veryrare >= rnd )
					clothing.Hue = VeryRareHue();
				else if ( rare >= rnd )
					clothing.Hue = RareHue();
				else if ( uncommon >= rnd )
					clothing.Hue = UncommonHue();
				else
					clothing.Hue = CommonHue();

				value += 100;
			}

			return new Tuple<Item[], int>( new Item[]{ clothing }, value );
		}

		public virtual int CommonHue()
		{
			return Utility.RandomDyedHue();
		}

		//15%
		public virtual int UncommonHue()
		{
			int hue = 0;
			do
			{
				hue = Utility.RandomList( Utility.RandomNondyedHue(), Utility.RandomNeutralHue() );
			}
			while ( BaseVendor.IsResourceHue( hue ) );

			return hue;
		}
		
		//2.99%
		public virtual int RareHue()
		{
			switch ( Utility.Random( 6 ) )
			{
				case 0: return Utility.RandomBlackHue();
				default: case 1: return Utility.RandomSnakeHue();
				case 2: return Utility.RandomBirdHue();
				case 3: return Utility.RandomSlimeHue();
				case 4: return Utility.RandomAnimalHue();
				case 5: return Utility.RandomMetalHue();
			}
		}

		//0.02% (Ore Hues)
		public virtual int VeryRareHue()
		{
			int rnd = Utility.Random( 1000 );
			CraftResource resource = CraftResource.None;

			if ( rnd >= 995 ) // 5/1000 -> 1/200
				resource = CraftResource.Valorite;
			else if ( rnd >= 985 ) // 10/1000 -> 1/100
				resource = CraftResource.Verite;
			else if ( rnd >= 965 ) // 20/1000 -> 1/50
				resource = CraftResource.Agapite;
			else if ( rnd >= 925 ) // 40/1000 -> 1/25
				resource = CraftResource.Gold;
			else if ( rnd >= 845 ) // 80/1000 -> 1/12.5
				resource = CraftResource.Bronze;
			else if ( rnd >= 685 ) // 160/1000 -> 1/6.25
				resource = CraftResource.Copper;
			else if ( rnd >= 320 ) // 320/1000 -> 1/3.125
				resource = CraftResource.ShadowIron;
			else
				resource = CraftResource.DullCopper;

			return CraftResources.GetInfo( resource ).Hue;
		}
	}
}