using System;

namespace Server.Items
{
	public class ThreeAleBottles : Item
	{
		[Constructable]
		public ThreeAleBottles() : base( 0x9A1 )
		{
		}

		public ThreeAleBottles( Serial serial ) : base( serial )
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