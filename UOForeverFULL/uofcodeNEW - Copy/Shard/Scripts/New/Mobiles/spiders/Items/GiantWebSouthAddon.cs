using System;
using Server;

namespace Server.Items
{
	public class GiantWebSouthAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new GiantWebSouthDeed(); } }

		[Constructable]
		public GiantWebSouthAddon()
		{
			AddComponent( new AddonComponent( 0x10CA ), 0, 0, 0 );
			AddComponent( new AddonComponent( 0x10CB ), 1, -1, 0 );
			AddComponent( new AddonComponent( 0x10CC ), 2, -2, 0 );
			AddComponent( new AddonComponent( 0x10CD ), 3, -3, 0 );
		}

		public GiantWebSouthAddon( Serial serial ) : base ( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class GiantWebSouthDeed : BaseAddonDeed
	{
		public override BaseAddon Addon{ get{ return new GiantWebSouthAddon(); } }
		public override int LabelNumber{ get{ return 0; } } // large forge (south)
		public override string DefaultName{ get{ return "a giant web deed (south)"; } }

		[Constructable]
		public GiantWebSouthDeed()
		{
		}

		public GiantWebSouthDeed( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}
