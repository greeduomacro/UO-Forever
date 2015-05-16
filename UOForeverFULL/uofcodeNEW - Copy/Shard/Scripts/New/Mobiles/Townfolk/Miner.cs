using System;
using Server;
using Server.Items;
using Server.Network;

namespace Server.Mobiles
{
	public class CommonMiner : BaseCreature
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
		public CommonMiner() : base( AIType.AI_Melee, FightMode.None, 10, 1, 0.5, 2.0 )
		{
			SetStr( 75, 100 );
			SetDex( 10, 75 );
			SetInt( 10 );

			Fame = 50;
			Karma = 50;

			SpeechHue = Utility.RandomDyedHue();
			Title = "a miner";
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
			AddItem( new HalfApron( GetRandomHue() ) );

			switch ( Utility.Random( 2 ) )
			{
				case 0: AddItem( new Boots( Utility.RandomNeutralHue() ) ); break;
				case 1: AddItem( new Shoes( Utility.RandomNeutralHue() ) ); break;
			}

			double random = Utility.RandomDouble();

			if ( 0.25 > random )
				AddItem( new SturdyPickaxe() );
			else if ( 0.05 > random )
				new GargoylesPickaxe();
			else
				AddItem( new Pickaxe() );

			PackItem( new Gold( 10, 15 ) );

			if ( 0.10 > Utility.RandomDouble() )
				PackItem( new IronIngot( Utility.Random( 5, 15 ) ) );
			else
				PackItem( new IronOre( Utility.Random( 5, 15 ) ) );
		}

		public CommonMiner( Serial serial ) : base( serial )
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