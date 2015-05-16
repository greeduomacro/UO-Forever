using System;
using Server;
using Server.Items;
using Server.Network;

namespace Server.Mobiles
{
	public class Librarian : BaseCreature
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
		public Librarian() : base( AIType.AI_Melee, FightMode.None, 10, 1, 0.5, 2.0 )
		{
			Title = "the librarian";
			Female = true;
			Body = 0x191;
			Name = NameList.RandomName( "female" );

			int hairHue = Utility.RandomHairHue();
			Utility.AssignRandomHair( this, hairHue );
			Utility.AssignRandomFacialHair( this, hairHue );

			switch ( Utility.Random( 2 ) )
			{
				case 0: AddItem( new Skirt( GetRandomHue() ) ); break;
				case 1: AddItem( new Kilt( GetRandomHue() ) ); break;
			}

			switch ( Utility.Random( 4 ) )
			{
				case 0: AddItem( new FancyDress( GetRandomHue() ) ); break;
				case 1: AddItem( new PlainDress( GetRandomHue() ) ); break;
				case 2: AddItem( new FancyShirt( GetRandomHue() ) ); break;
				case 3: AddItem( new Shirt( GetRandomHue() ) ); break;
			}

			AddItem( new Sandals( Utility.RandomBool() ? 0 : GetRandomHue() ) );
		}

		public Librarian( Serial serial ) : base( serial )
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