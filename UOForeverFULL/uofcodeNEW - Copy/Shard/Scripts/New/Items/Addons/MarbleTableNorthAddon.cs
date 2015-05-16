using System;
using Server;

namespace Server.Items
{
	public class MarbleTableEastAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new MarbleTableEastDeed(); } }

		[Constructable]
		public MarbleTableEastAddon()
		{
			AddComponent( new AddonComponent( 0x1DC2 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 0x1DC1 ), 0, 1, 0 );
		}

		public MarbleTableEastAddon( Serial serial ) : base( serial )
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

	public class MarbleTableEastDeed : BaseAddonDeed
	{
		public override BaseAddon Addon{ get{ return new MarbleTableEastAddon(); } }
		public override string DefaultName{ get{ return "marble table (east)"; } }

		[Constructable]
		public MarbleTableEastDeed()
		{
		}

		public MarbleTableEastDeed( Serial serial ) : base( serial )
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