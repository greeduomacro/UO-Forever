using System;
using Server;
using Server.Items;
using Server.Network;

namespace Server.Mobiles
{
	public class Lumberjack : BaseCreature
	{

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

		[Constructable]
		public Lumberjack() : base( AIType.AI_Melee, FightMode.None, 10, 1, 0.5, 2.0 )
		{
			SetStr( 75, 100 );
			SetDex( 10, 75 );
			SetInt( 10 );

			Fame = 50;
			Karma = 50;

			SpeechHue = Utility.RandomDyedHue();
			Title = "a lumberjack";
			Hue = Utility.RandomSkinHue();

			Body = 0x190;
			Name = NameList.RandomName( "male" );

			int hairHue = Utility.RandomHairHue();
			Utility.AssignRandomHair( this, hairHue );
			Utility.AssignRandomFacialHair( this, hairHue );

			switch ( Utility.Random( 5 ) )
			{
				case 0: AddItem( new FloppyHat( Utility.RandomNeutralHue() ) ); break;
				case 1: AddItem( new SkullCap( Utility.RandomNeutralHue() ) ); break;
				case 2: AddItem( new StrawHat() ); break;
				case 3: AddItem( new TallStrawHat() ); break;
				default: break;
			}

			switch ( Utility.Random( 2 ) )
			{
				case 0: AddItem( new ShortPants( GetRandomHue() ) ); break;
				case 1: AddItem( new LongPants( GetRandomHue() ) ); break;
			}

			AddItem( new Shirt( GetRandomHue() ) );

			switch ( Utility.Random( 2 ) )
			{
				case 0: AddItem( new Boots( Utility.RandomNeutralHue() ) ); break;
				case 1: AddItem( new Shoes( Utility.RandomNeutralHue() ) ); break;
			}

			switch ( Utility.Random( 4 ) )
			{
				case 0: AddItem( new Hatchet() ); break;
				case 1: AddItem( new Axe() ); break;
				case 2: AddItem( new DoubleAxe() ); break;
				case 3: AddItem( new TwoHandedAxe() ); break;
			}

			PackItem( new Gold( 10, 15 ) );
			PackItem( new Log( Utility.Random( 5, 15 ) ) );
		}

		public Lumberjack( Serial serial ) : base( serial )
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
	}
}