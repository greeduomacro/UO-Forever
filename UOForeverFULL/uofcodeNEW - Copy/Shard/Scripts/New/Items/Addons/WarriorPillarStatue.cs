using System;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Engines.VeteranRewards;

namespace Server.Items
{
	public class WarriorPillarStatueEastAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new WarriorPillarStatueEastDeed(); } }

		[Constructable]
		public WarriorPillarStatueEastAddon() : base()
		{
			AddComponent( new AddonComponent( 0x12D8 ), 0, 0, 0 );
		}

		public WarriorPillarStatueEastAddon( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

        public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class WarriorPillarStatueSouthAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new WarriorPillarStatueSouthDeed(); } }

		[Constructable]
		public WarriorPillarStatueSouthAddon() : base()
		{
			AddComponent( new AddonComponent( 0x12D9 ), 0, 0, 0 );
		}

		public WarriorPillarStatueSouthAddon( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

        public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class WarriorPillarStatueEastDeed : BaseAddonDeed
	{
		public override string DefaultName{ get{ return "a deed for a pillar statue of a warrior (east)"; } }

		public override BaseAddon Addon{ get{ return new WarriorPillarStatueEastAddon(); } }

		[Constructable]
		public WarriorPillarStatueEastDeed() : base()
		{
		}

		public WarriorPillarStatueEastDeed( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class WarriorPillarStatueSouthDeed : BaseAddonDeed
	{
		public override string DefaultName{ get{ return "a deed for a pillar statue of a warrior (south)"; } }

		public override BaseAddon Addon{ get{ return new WarriorPillarStatueSouthAddon(); } }

		[Constructable]
		public WarriorPillarStatueSouthDeed() : base()
		{
		}

		public WarriorPillarStatueSouthDeed( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}