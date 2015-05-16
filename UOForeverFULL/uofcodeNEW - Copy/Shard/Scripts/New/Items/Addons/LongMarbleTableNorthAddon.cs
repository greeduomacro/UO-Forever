using System;
using Server;

namespace Server.Items
{
	public class LongMarbleTableNorthAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new LongMarbleTableNorthDeed(); } }

		[Constructable]
		public LongMarbleTableNorthAddon()
		{
			AddComponent( new AddonComponent( 0x1DC5 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 0x1DC6 ), 1, 0, 0 );
			AddComponent( new AddonComponent( 0x1DC4 ), 2, 0, 0 );
		}

		public LongMarbleTableNorthAddon( Serial serial ) : base( serial )
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

	public class LongMarbleTableNorthDeed : BaseAddonDeed
	{
		public override BaseAddon Addon{ get{ return new LongMarbleTableNorthAddon(); } }
		public override string DefaultName{ get{ return "long marble table (north)"; } }

		[Constructable]
		public LongMarbleTableNorthDeed()
		{
		}

		public LongMarbleTableNorthDeed( Serial serial ) : base( serial )
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