using System;
using Server;

namespace Server.Items
{
	public class SacrificialAltarEast : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new SacrificialAltarEastDeed(); } }
		public override bool RetainDeedHue{ get{ return true; } }
		[Constructable]
		public SacrificialAltarEast()
		{
			AddComponent( new AddonComponent( 10909 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 10908 ), 0, 1, 0 );
		}

		public SacrificialAltarEast( Serial serial ) : base( serial )
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

	public class SacrificialAltarEastDeed : BaseAddonDeed
	{
		public override BaseAddon Addon{ get{ return new SacrificialAltarEast(); } }

		[Constructable]
		public SacrificialAltarEastDeed()
		{
			Name = "deed for a sacrificial altar (east)";
		}

		public SacrificialAltarEastDeed( Serial serial ) : base( serial )
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