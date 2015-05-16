using System;
using Server;

namespace Server.Items
{
	[Flipable( 0x2B04, 0x2B04 )]
	public class RoyalCloak : BaseCloak
	{
		public override string DefaultName{ get{ return "royal cloak"; } }

		[Constructable]
		public RoyalCloak() : this( 1636 )
		{
		}

		[Constructable]
		public RoyalCloak( int hue ) : base( 0x2B04, hue )
		{
			Weight = 4.0;
		}

		public RoyalCloak( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( (int) 0 );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();
		}
	}
}