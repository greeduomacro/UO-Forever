using System;
using Server;

namespace Server.Items
{
	[FlipableAttribute( 0x4699, 0x469A )]
	public class RuinedTapestry : Item
	{
		[Constructable]
		public RuinedTapestry() : base( 0x4699 )
		{
		}

		public RuinedTapestry( Serial serial ) : base( serial )
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