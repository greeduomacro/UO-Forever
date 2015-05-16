using System;
using Server;

namespace Server.Items
{
	[FlipableAttribute( 0x1E66, 0x1E6D )]
	public class TrollHead : Item
	{
		[Constructable]
		public TrollHead( int hue ) : base( 0x1E66 )
		{
			Hue = hue;
		}

		[Constructable]
		public TrollHead() : this( 0 )
		{
		}

		public TrollHead( Serial serial ) : base( serial )
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