using System;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Engines.VeteranRewards;

namespace Server.Items
{
	public class CrystalStatueEastAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new CrystalStatueEastDeed(); } }

		[Constructable]
		public CrystalStatueEastAddon() : base()
		{
			AddComponent( new AddonComponent( 0x35F9 ), 0, 0, 0 );
		}

		public CrystalStatueEastAddon( Serial serial ) : base( serial )
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

	public class CrystalStatueEast2Addon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new CrystalStatueEast2Deed(); } }

		[Constructable]
		public CrystalStatueEast2Addon() : base()
		{
			AddComponent( new AddonComponent( 0x35FA ), 0, 0, 0 );
		}

		public CrystalStatueEast2Addon( Serial serial ) : base( serial )
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

	public class CrystalStatueEast3Addon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new CrystalStatueEast3Deed(); } }

		[Constructable]
		public CrystalStatueEast3Addon() : base()
		{
			AddComponent( new AddonComponent( 0x35FC ), 0, 0, 0 );
		}

		public CrystalStatueEast3Addon( Serial serial ) : base( serial )
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

	public class CrystalStatueSouthAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new CrystalStatueSouthDeed(); } }

		[Constructable]
		public CrystalStatueSouthAddon() : base()
		{
			AddComponent( new AddonComponent( 0x35F8 ), 0, 0, 0 );
		}

		public CrystalStatueSouthAddon( Serial serial ) : base( serial )
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

	public class CrystalStatueSouth2Addon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new CrystalStatueSouth2Deed(); } }

		[Constructable]
		public CrystalStatueSouth2Addon() : base()
		{
			AddComponent( new AddonComponent( 0x35FB ), 0, 0, 0 );
		}

		public CrystalStatueSouth2Addon( Serial serial ) : base( serial )
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

	public class CrystalStatueSouth3Addon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new CrystalStatueSouth3Deed(); } }

		[Constructable]
		public CrystalStatueSouth3Addon() : base()
		{
			AddComponent( new AddonComponent( 0x35FD ), 0, 0, 0 );
		}

		public CrystalStatueSouth3Addon( Serial serial ) : base( serial )
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

	public class CrystalStatueEastDeed : BaseAddonDeed
	{
		public override string DefaultName{ get{ return "a deed for crystal statue (east)"; } }

		public override BaseAddon Addon{ get{ return new CrystalStatueEastAddon(); } }

		[Constructable]
		public CrystalStatueEastDeed() : base()
		{
		}

		public CrystalStatueEastDeed( Serial serial ) : base( serial )
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

	public class CrystalStatueEast2Deed : BaseAddonDeed
	{
		public override string DefaultName{ get{ return "a deed for crystal statue (east)"; } }

		public override BaseAddon Addon{ get{ return new CrystalStatueEast2Addon(); } }

		[Constructable]
		public CrystalStatueEast2Deed() : base()
		{
		}

		public CrystalStatueEast2Deed( Serial serial ) : base( serial )
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

	public class CrystalStatueEast3Deed : BaseAddonDeed
	{
		public override string DefaultName{ get{ return "a deed for crystal statue (east)"; } }

		public override BaseAddon Addon{ get{ return new CrystalStatueEast3Addon(); } }

		[Constructable]
		public CrystalStatueEast3Deed() : base()
		{
		}

		public CrystalStatueEast3Deed( Serial serial ) : base( serial )
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

	public class CrystalStatueSouthDeed : BaseAddonDeed
	{
		public override string DefaultName{ get{ return "a deed for a crystal statue (south)"; } }

		public override BaseAddon Addon{ get{ return new CrystalStatueSouthAddon(); } }

		[Constructable]
		public CrystalStatueSouthDeed() : base()
		{
		}

		public CrystalStatueSouthDeed( Serial serial ) : base( serial )
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

	public class CrystalStatueSouth2Deed : BaseAddonDeed
	{
		public override string DefaultName{ get{ return "a deed for a crystal statue (south)"; } }

		public override BaseAddon Addon{ get{ return new CrystalStatueSouth2Addon(); } }

		[Constructable]
		public CrystalStatueSouth2Deed() : base()
		{
		}

		public CrystalStatueSouth2Deed( Serial serial ) : base( serial )
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

	public class CrystalStatueSouth3Deed : BaseAddonDeed
	{
		public override string DefaultName{ get{ return "a deed for a crystal statue (south)"; } }

		public override BaseAddon Addon{ get{ return new CrystalStatueSouth3Addon(); } }

		[Constructable]
		public CrystalStatueSouth3Deed() : base()
		{
		}

		public CrystalStatueSouth3Deed( Serial serial ) : base( serial )
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