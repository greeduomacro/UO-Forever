using System;
using Server;

namespace Server.Items
{
	[FlipableAttribute( 0x224E, 0x224F )]
	public class DemonCowSkull : Item
	{
		[Constructable]
		public DemonCowSkull() : base( 0x224E )
		{
		}

		public DemonCowSkull( Serial serial ) : base( serial )
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