using System;
using Server;

namespace Server.Items
{
	public class BrokeVanDresserEastAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new BrokeVanDresserEastDeed(); } }

		[Constructable]
		public BrokeVanDresserEastAddon()
		{
			AddComponent( new AddonComponent( 0xC21 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 0xC20 ), 0, 1, 0 );
			
			Name = "Broken Vanity Dresser";
		}

		public BrokeVanDresserEastAddon( Serial serial ) : base( serial )
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

	public class BrokeVanDresserEastDeed : BaseAddonDeed
	{
		public override BaseAddon Addon{ get{ return new BrokeVanDresserEastAddon(); } }
		//public override int LabelNumber{ get{ return 1044323; } } // large bed (south)

		[Constructable]
		public BrokeVanDresserEastDeed()
		{
			Name = "Broken Vanity Dresser East Deed";
		}

		public BrokeVanDresserEastDeed( Serial serial ) : base( serial )
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

