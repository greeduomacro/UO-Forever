using System;
using Server;

namespace Server.Items
{
	public class DSwordNorthAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new DSwordNorthDeed(); } }

		[Constructable]
		public DSwordNorthAddon()
		{
			AddComponent( new AddonComponent( 0x1564 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 0x1565 ), 1, 0, 0 );
		}

		public DSwordNorthAddon( Serial serial ) : base( serial )
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

	public class DSwordNorthDeed : BaseAddonDeed
	{
		public override BaseAddon Addon{ get{ return new DSwordNorthAddon(); } }
		public override string DefaultName{ get{ return "decorative swords (north)"; } }

		[Constructable]
		public DSwordNorthDeed()
		{
		}

		public DSwordNorthDeed( Serial serial ) : base( serial )
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