using System;
using Server;
using Server.Items;
using Server.Network;
using Server.Factions;

namespace Server.Mobiles
{
	public class Commoner : BaseCreature
	{
		/*private static string[] Titles = new string[]
		{
			"the commoner",
			"the noble",
			"the artist",
			"the bard",
			"the beggar",
			"the thief"
		};*/

		public virtual int GetRandomHue()
		{
			switch ( Utility.Random( 6 ) )
			{
				default:
				case 0: return Utility.RandomBlueHue();
				case 1: return Utility.RandomGreenHue();
				case 2: return Utility.RandomRedHue();
				case 3: return Utility.RandomYellowHue();
				case 4: return Utility.RandomNeutralHue();
				case 5: return Utility.RandomGreyHue();
			}
		}

		public override int FactionSilverWorth{ get{ return 0; } }

		[Constructable]
		public Commoner() : base( AIType.AI_Melee, FightMode.None, 10, 1, 0.5, 2.0 )
		{
			SetStr( 10, 30 );
			SetDex( 10, 30 );
			SetInt( 10, 30 );

			Fame = 50;
			Karma = 50;

			DressCommoner();

			PackItem( new Gold( 10, 50 ) );
		}

		public Commoner( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}

		public void DressCommoner()
		{
			SpeechHue = Utility.RandomDyedHue();
			Hue = Utility.RandomSkinHue();

			if ( Female = Utility.RandomBool() )
			{
				Body = 0x191;
				Name = NameList.RandomName( "female" );

				switch ( Utility.Random( 5 ) )
				{
					case 0: AddItem( new FloppyHat( Utility.RandomNeutralHue() ) ); break;
					case 1: AddItem( new FeatheredHat( Utility.RandomNeutralHue() ) ); break;
					case 2: AddItem( new Bonnet() ); break;
					case 3: AddItem( new Cap( Utility.RandomNeutralHue() ) ); break;
				}

				switch ( Utility.Random( 4 ) )
				{
					case 0: AddItem( new ShortPants( GetRandomHue() ) ); break;
					case 1: AddItem( new LongPants( GetRandomHue() ) ); break;
					case 2: AddItem( new Skirt( GetRandomHue() ) ); break;
					case 3: AddItem( new Kilt( GetRandomHue() ) ); break;
				}

				switch ( Utility.Random( 7 ) )
				{
					case 0: AddItem( new Doublet( GetRandomHue() ) ); break;
					case 1: AddItem( new Surcoat( GetRandomHue() ) ); break;
					case 2: AddItem( new Tunic( GetRandomHue() ) ); break;
					case 3: AddItem( new FancyDress( GetRandomHue() ) ); break;
					case 4: AddItem( new PlainDress( GetRandomHue() ) ); break;
					case 5: AddItem( new FancyShirt( GetRandomHue() ) ); break;
					case 6: AddItem( new Shirt( GetRandomHue() ) ); break;
				}
			}
			else
			{
				Body = 0x190;
				Name = NameList.RandomName( "male" );

				switch ( Utility.Random( 7 ) )
				{
					case 0: AddItem( new SkullCap( GetRandomHue() ) ); break;
					case 1: AddItem( new Bandana( GetRandomHue() ) ); break;
					case 2: AddItem( new WideBrimHat() ); break;
					case 3: AddItem( new TallStrawHat( Utility.RandomNeutralHue() ) ); break;
					case 4: AddItem( new StrawHat( Utility.RandomNeutralHue() ) ); break;
					case 5: AddItem( new TricorneHat( Utility.RandomNeutralHue() ) ); break;
				}

				switch ( Utility.Random( 3 ) )
				{
					case 0: AddItem( new ShortPants( GetRandomHue() ) ); break;
					case 1: AddItem( new LongPants( GetRandomHue() ) ); break;
					case 2: AddItem( new Kilt( GetRandomHue() ) ); break;
				}

				switch ( Utility.Random( 5 ) )
				{
					case 0: AddItem( new Doublet( GetRandomHue() ) ); break;
					case 1: AddItem( new Surcoat( GetRandomHue() ) ); break;
					case 2: AddItem( new Tunic( GetRandomHue() ) ); break;
					case 3: AddItem( new FancyShirt( GetRandomHue() ) ); break;
					case 4: AddItem( new Shirt( GetRandomHue() ) ); break;
				}
			}

			int hairHue = Utility.RandomHairHue();
			Utility.AssignRandomHair( this, hairHue );
			Utility.AssignRandomFacialHair( this, hairHue );

			switch ( Utility.Random( 5 ) )
			{
				case 0: AddItem( new Boots( Utility.RandomNeutralHue() ) ); break;
				case 1: AddItem( new Shoes( Utility.RandomNeutralHue() ) ); break;
				case 2: AddItem( new Sandals( Utility.RandomNeutralHue() ) ); break;
				case 3: AddItem( new ThighBoots( Utility.RandomNeutralHue() ) ); break;
			}
		}
	}
}