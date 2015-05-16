using System;
using Server;

namespace Server.Items
{
	public class ElvenAnvilSouthAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new ElvenAnvilSouthDeed(); } }

		[Constructable]
		public ElvenAnvilSouthAddon()
		{
			AddComponent( new AnvilComponent( 0x2DD6 ), 0, 0, 0 );
		}

		public ElvenAnvilSouthAddon( Serial serial ) : base( serial )
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

	public class ElvenAnvilSouthDeed : BaseAddonDeed
	{
		public override BaseAddon Addon{ get{ return new ElvenAnvilSouthAddon(); } }
		//public override int LabelNumber{ get{ return 1044334; } } // anvil (south)

		[Constructable]
		public ElvenAnvilSouthDeed()
		{
		}

		public ElvenAnvilSouthDeed( Serial serial ) : base( serial )
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