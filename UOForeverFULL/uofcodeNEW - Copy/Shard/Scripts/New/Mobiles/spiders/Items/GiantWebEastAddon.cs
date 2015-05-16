using System;
using Server;

namespace Server.Items
{
	public class GiantWebEastAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new GiantWebEastDeed(); } }

		[Constructable]
		public GiantWebEastAddon()
		{
			AddComponent( new AddonComponent( 0x10CE ), 3, -3, 0 );
			AddComponent( new AddonComponent( 0x10CF ), 2, -2, 0 );
			AddComponent( new AddonComponent( 0x10D0 ), 1, -1, 0 );
			AddComponent( new AddonComponent( 0x10D1 ), 0, 0, 0 );
		}

		public GiantWebEastAddon( Serial serial ) : base( serial )
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

	public class GiantWebEastDeed : BaseAddonDeed
	{
		public override BaseAddon Addon{ get{ return new GiantWebEastAddon(); } }
		public override int LabelNumber{ get{ return 0; } } // custom (east)
        public override string DefaultName { get { return "a giant web deed (East)"; } }

		[Constructable]
		public GiantWebEastDeed()
		{
		}

		public GiantWebEastDeed( Serial serial ) : base( serial )
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
