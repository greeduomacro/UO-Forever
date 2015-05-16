using System;
using Server;
using Server.Multis;
using Server.Gumps;

namespace Server.Items
{
	public class ChristmasCastleAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new ChristmasCastleAddonDeed(); } }

		[Constructable]
		public ChristmasCastleAddon()
		{
			AddonComponent comp = new AddonComponent( 0x2317 );
			comp.Name = "gingerbread castle";
			AddComponent( comp, 0, 0, 0 );

			comp = new AddonComponent( 0x2318 );
			comp.Name = "gingerbread castle";
			AddComponent( comp, 1, 0, 0 );

			comp = new AddonComponent( 0x2319 );
			comp.Name = "gingerbread castle";
			AddComponent( comp, 1, -1, 0 );
		}

		public ChristmasCastleAddon( Serial serial ) : base( serial )
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

	public class ChristmasCastleAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon{ get{ return new ChristmasCastleAddon(); } }
		public override string DefaultName{ get{ return "a gingerbread castle deed"; } }

		[Constructable]
		public ChristmasCastleAddonDeed()
		{
		}

		public ChristmasCastleAddonDeed( Serial serial ) : base( serial )
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