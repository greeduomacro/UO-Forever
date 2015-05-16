using System;
using Server;

namespace Server.Items
{
	public class SmallWebBlack : Item
	{
		[Constructable]
		public SmallWebBlack() : base( 0x10D7 )
		{
			Hue = 1899;
		}

		public SmallWebBlack( Serial serial ) : base( serial )
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