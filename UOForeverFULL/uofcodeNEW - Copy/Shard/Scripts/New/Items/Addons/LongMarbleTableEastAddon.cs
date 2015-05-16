using System;
using Server;

namespace Server.Items
{
	public class LongMarbleTableEastAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new LongMarbleTableEastDeed(); } }

		[Constructable]
		public LongMarbleTableEastAddon()
		{
			AddComponent( new AddonComponent( 0x1DC2 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 0x1DC3 ), 0, 1, 0 );
			AddComponent( new AddonComponent( 0x1DC1 ), 0, 2, 0 );
		}

		public LongMarbleTableEastAddon( Serial serial ) : base( serial )
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

	public class LongMarbleTableEastDeed : BaseAddonDeed
	{
		public override BaseAddon Addon{ get{ return new LongMarbleTableEastAddon(); } }
		public override string DefaultName{ get{ return "long marble table (east)"; } }

		[Constructable]
		public LongMarbleTableEastDeed()
		{
		}

		public LongMarbleTableEastDeed( Serial serial ) : base( serial )
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