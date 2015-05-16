using System;

namespace Server.Items
{
	public class KarmaIonStone : Item
	{
		public override string DefaultName{ get{ return "dreaded ion stone"; } }

		[Constructable]
		public KarmaIonStone() : base( 0x2809 )
		{
			Stackable = true;
			Weight = 1.0;
			Hue = 2106;
			LootType = LootType.Cursed;
		}

		public KarmaIonStone( Serial serial ) : base( serial )
		{
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( IsChildOf( from.Backpack ) )
			{
				from.Karma -= 1000;
				from.SendMessage( 0x5, "Anger and hostility courses through your veins!" );
				Consume();
			}
			else
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}


