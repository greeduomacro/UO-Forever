using System;
using Server;

namespace Server.Items
{
	public class SacrificialAltarSouth : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new SacrificialAltarSouthDeed(); } }
		public override bool RetainDeedHue{ get{ return true; } }
		[Constructable]
		public SacrificialAltarSouth()
		{
			AddComponent( new AddonComponent( 10907 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 10906 ), 1, 0, 0 );
		}

		public SacrificialAltarSouth( Serial serial ) : base( serial )
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

	public class SacrificialAltarSouthDeed : BaseAddonDeed
	{
		public override BaseAddon Addon{ get{ return new SacrificialAltarSouth(); } }

		[Constructable]
		public SacrificialAltarSouthDeed()
		{
			Name = "deed for a sacrificial altar (south)";
		}

		public SacrificialAltarSouthDeed( Serial serial ) : base( serial )
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