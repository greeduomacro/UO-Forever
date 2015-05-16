using System;
using Server;

namespace Server.Items
{
	public class DAxeNorthAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new DAxeNorthDeed(); } }

		[Constructable]
		public DAxeNorthAddon()
		{
			AddComponent( new AddonComponent( 0x1568 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 0x1569 ), 1, 0, 0 );
		}

		public DAxeNorthAddon( Serial serial ) : base( serial )
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

	public class DAxeNorthDeed : BaseAddonDeed
	{
		public override BaseAddon Addon{ get{ return new DAxeNorthAddon(); } }
		public override string DefaultName{ get{ return "decorative axes (north)"; } }

		[Constructable]
		public DAxeNorthDeed()
		{
		}

		public DAxeNorthDeed( Serial serial ) : base( serial )
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