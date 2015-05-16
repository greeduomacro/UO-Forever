using System;
using Server;

namespace Server.Items
{
	[FlipableAttribute( 0x469B, 0x469C )]
	public class PumpkinScarecrow : Item
	{
		[Constructable]
		public PumpkinScarecrow() : base( 0x469B )
		{
		}

		public PumpkinScarecrow( Serial serial ) : base( serial )
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