using System;

namespace Server.Items
{
	[FlipableAttribute( 0x2DD5, 0x2DD6 )]
	[Server.Engines.Craft.Anvil]
	public class ElvenAnvil : Item
	{
		[Constructable]
		public ElvenAnvil() : base( 0x2DD5 )
		{
			Movable = false;
		}

		public ElvenAnvil( Serial serial ) : base( serial )
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