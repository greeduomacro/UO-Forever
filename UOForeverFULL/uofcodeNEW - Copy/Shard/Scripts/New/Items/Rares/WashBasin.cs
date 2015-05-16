using System;

namespace Server.Items
{
	public class WashBasin : Item
	{
		[Constructable]
		public WashBasin() : base( 0x1008 )
		{
		}

		public WashBasin( Serial serial ) : base( serial )
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