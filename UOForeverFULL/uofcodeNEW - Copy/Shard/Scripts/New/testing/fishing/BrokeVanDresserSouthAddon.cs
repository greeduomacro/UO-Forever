using System;
using Server;

namespace Server.Items
{
	public class BrokeVanDresserSouthAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new BrokeVanDresserSouthDeed(); } }

		[Constructable]
		public BrokeVanDresserSouthAddon()
		{
			AddComponent( new AddonComponent( 0xC23 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 0xC22 ), 1, 0, 0 );
			
			Name = "Broken Vanity Dresser";
		}

		public BrokeVanDresserSouthAddon( Serial serial ) : base( serial )
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

	public class BrokeVanDresserSouthDeed : BaseAddonDeed
	{
		public override BaseAddon Addon{ get{ return new BrokeVanDresserSouthAddon(); } }
		//public override int LabelNumber{ get{ return 1044323; } } // large bed (south)

		[Constructable]
		public BrokeVanDresserSouthDeed()
		{
			Name = "Broken Vanity Dresser South Deed";
		}

		public BrokeVanDresserSouthDeed( Serial serial ) : base( serial )
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

