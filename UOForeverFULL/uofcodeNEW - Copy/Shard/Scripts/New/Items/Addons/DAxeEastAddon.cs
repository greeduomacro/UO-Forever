using System;
using Server;

namespace Server.Items
{
	public class DAxeEastAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new DAxeEastDeed(); } }

		[Constructable]
		public DAxeEastAddon()
		{
			AddComponent( new AddonComponent( 0x156B ), 0, 0, 0 );
			AddComponent( new AddonComponent( 0x156A ), 0, 1, 0 );
		}

		public DAxeEastAddon( Serial serial ) : base( serial )
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

	public class DAxeEastDeed : BaseAddonDeed
	{
		public override BaseAddon Addon{ get{ return new DAxeEastAddon(); } }
		public override string DefaultName{ get{ return "decorative axes (east)"; } }

		[Constructable]
		public DAxeEastDeed()
		{
		}

		public DAxeEastDeed( Serial serial ) : base( serial )
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