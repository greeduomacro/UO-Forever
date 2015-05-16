using System;
using Server;

namespace Server.Items
{
	public class BedOfNailsSouth : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new BedOfNailsSouthDeed(); } }
		public override bool RetainDeedHue{ get{ return true; } }

		[Constructable]
		public BedOfNailsSouth()
		{
			AddComponent( new AddonComponent( 10885 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 10886 ), 1, 0, 0 );
		}

		public BedOfNailsSouth( Serial serial ) : base( serial )
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

	public class BedOfNailsSouthDeed : BaseAddonDeed
	{
		public override BaseAddon Addon{ get{ return new BedOfNailsSouth(); } }

		[Constructable]
		public BedOfNailsSouthDeed()
		{
			Name = "deed for a bed of nails (south)";
		}

		public BedOfNailsSouthDeed( Serial serial ) : base( serial )
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