using System;
using Server;

namespace Server.Items
{
	public class DSwordEastAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new DSwordEastDeed(); } }

		[Constructable]
		public DSwordEastAddon()
		{
			AddComponent( new AddonComponent( 0x1567 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 0x1566 ), 0, 1, 0 );
		}

		public DSwordEastAddon( Serial serial ) : base( serial )
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

	public class DSwordEastDeed : BaseAddonDeed
	{
		public override BaseAddon Addon{ get{ return new DSwordEastAddon(); } }
		public override string DefaultName{ get{ return "decorative swords (east)"; } }

		[Constructable]
		public DSwordEastDeed()
		{
		}

		public DSwordEastDeed( Serial serial ) : base( serial )
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