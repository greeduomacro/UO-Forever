using System;
using Server;

namespace Server.Items
{
	public class MarbleTableNorthAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new MarbleTableNorthDeed(); } }

		[Constructable]
		public MarbleTableNorthAddon()
		{
			AddComponent( new AddonComponent( 0x1DC5 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 0x1DC4 ), 1, 0, 0 );
		}

		public MarbleTableNorthAddon( Serial serial ) : base( serial )
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

	public class MarbleTableNorthDeed : BaseAddonDeed
	{
		public override BaseAddon Addon{ get{ return new MarbleTableNorthAddon(); } }
		public override string DefaultName{ get{ return "marble table (north)"; } }

		[Constructable]
		public MarbleTableNorthDeed()
		{
		}

		public MarbleTableNorthDeed( Serial serial ) : base( serial )
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