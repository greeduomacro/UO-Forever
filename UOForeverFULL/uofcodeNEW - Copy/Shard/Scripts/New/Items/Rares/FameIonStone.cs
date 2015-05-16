using System;

namespace Server.Items
{
	public class FameIonStone : Item
	{
		public override string DefaultName{ get{ return "valorous ion stone"; } }

		[Constructable]
		public FameIonStone() : base( 0x2809 )
		{
			Stackable = true;
			Weight = 1.0;
			Hue = 1265;
			LootType = LootType.Cursed;
		}

		public FameIonStone( Serial serial ) : base( serial )
		{
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( IsChildOf( from.Backpack ) )
			{
				from.Fame += 1000;
				from.SendMessage( 0x5, "You gain a feeling of prestige and influence!" );
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


