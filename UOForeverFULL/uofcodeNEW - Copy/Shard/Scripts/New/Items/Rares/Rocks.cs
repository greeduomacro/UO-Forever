using System;

namespace Server.Items
{
	public class Rocks : Item
	{
		[Constructable]
		public Rocks() : base( 0x1367 )
		{
		}

		public Rocks( Serial serial ) : base( serial )
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