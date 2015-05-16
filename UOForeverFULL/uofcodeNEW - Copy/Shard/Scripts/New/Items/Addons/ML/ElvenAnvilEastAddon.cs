using System;
using Server;

namespace Server.Items
{
	public class ElvenAnvilEastAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new ElvenAnvilEastDeed(); } }

		[Constructable]
		public ElvenAnvilEastAddon()
		{
			AddComponent( new AnvilComponent( 0x2DD5 ), 0, 0, 0 );
		}

		public ElvenAnvilEastAddon( Serial serial ) : base( serial )
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

	public class ElvenAnvilEastDeed : BaseAddonDeed
	{
		public override BaseAddon Addon{ get{ return new ElvenAnvilEastAddon(); } }
		//public override int LabelNumber{ get{ return 1044333; } } // elven anvil (east)

		[Constructable]
		public ElvenAnvilEastDeed()
		{
		}

		public ElvenAnvilEastDeed( Serial serial ) : base( serial )
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