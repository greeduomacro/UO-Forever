using System;
using Server;

namespace Server.Items
{
	[FlipableAttribute( 0x2234, 0x2235 )]
	public class DragonHead : Item
	{
		[Constructable]
		public DragonHead( int hue ) : base( 0x2234 )
		{
			Hue = hue;
		}

		[Constructable]
		public DragonHead() : this( 0 )
		{
		}

		public DragonHead( Serial serial ) : base( serial )
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