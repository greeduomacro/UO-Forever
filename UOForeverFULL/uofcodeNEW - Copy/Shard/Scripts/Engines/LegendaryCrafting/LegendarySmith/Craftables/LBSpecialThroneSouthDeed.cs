using System;
using Server;

namespace Server.Items
{
	public class LBSpecialThroneSouthAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new LBSpecialThroneSouthDeed(); } }


		[Constructable]
		public LBSpecialThroneSouthAddon()
		{
			AddComponent( new AddonComponent( 19419 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 19420 ), 1, 0, 0 );
			AddComponent( new AddonComponent( 19421 ), 2, 0, 0 );
		}

		public LBSpecialThroneSouthAddon( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class LBSpecialThroneSouthDeed : BaseAddonDeed
	{
		public override BaseAddon Addon{ get{ return new LBSpecialThroneSouthAddon(); } }
		public override int LabelNumber{ get{ return 1098291; } } // Lord Blackthorn's Throne 

		[Constructable]
		public LBSpecialThroneSouthDeed()
		{
		    LootType = LootType.Blessed;
		}

		public LBSpecialThroneSouthDeed( Serial serial ) : base( serial )
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