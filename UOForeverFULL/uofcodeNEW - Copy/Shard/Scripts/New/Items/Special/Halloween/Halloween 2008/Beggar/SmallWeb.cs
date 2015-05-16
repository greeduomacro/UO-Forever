using System;
using Server;

namespace Server.Items
{
	[FlipableAttribute( 0x10D6, 0x10D7 )]
	public class SmallWeb : Item
	{
		[Constructable]
		public SmallWeb() : base( 0x10D6 )
		{
		}

		public SmallWeb( Serial serial ) : base( serial )
		{
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