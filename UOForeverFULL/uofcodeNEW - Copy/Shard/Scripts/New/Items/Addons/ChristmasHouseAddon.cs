using System;
using Server;
using Server.Gumps;

namespace Server.Items
{
	public class ChristmasHouseAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new ChristmasHouseAddonDeed(); } }

		[Constructable]
		public ChristmasHouseAddon()
		{
			AddonComponent comp = new AddonComponent( 0x2BE5 );
			comp.Name = "gingerbread house";
			AddComponent( comp, 0, 0, 0 );

			comp = new AddonComponent( 0x2BE6 );
			comp.Name = "gingerbread house";
			AddComponent( comp, 1, 0, 0 );

			comp = new AddonComponent( 0x2BE7 );
			comp.Name = "gingerbread house";
			AddComponent( comp, 1, -1, 0 );
		}

		public ChristmasHouseAddon( Serial serial ) : base( serial )
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

	public class ChristmasHouseAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon{ get{ return new ChristmasHouseAddon(); } }
		public override string DefaultName{ get{ return "a gingerbread house deed"; } }

		[Constructable]
		public ChristmasHouseAddonDeed()
		{
		}

		public ChristmasHouseAddonDeed( Serial serial ) : base( serial )
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