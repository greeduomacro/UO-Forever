using System;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Engines.VeteranRewards;

namespace Server.Items
{
	public class CatStatueEastAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new CatStatueEastDeed(); } }

		[Constructable]
		public CatStatueEastAddon() : base()
		{
			AddComponent( new AddonComponent( 0x1948 ), 0, 0, 0 );
		}

		public CatStatueEastAddon( Serial serial ) : base( serial )
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

	public class CatStatueSouthAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new CatStatueSouthDeed(); } }

		[Constructable]
		public CatStatueSouthAddon() : base()
		{
			AddComponent( new AddonComponent( 0x1947 ), 0, 0, 0 );
		}

		public CatStatueSouthAddon( Serial serial ) : base( serial )
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

	public class CatStatueEast2Addon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new CatStatueEast2Deed(); } }

		[Constructable]
		public CatStatueEast2Addon() : base()
		{
			AddComponent( new AddonComponent( 0x194A ), 0, 0, 0 );
		}

		public CatStatueEast2Addon( Serial serial ) : base( serial )
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

	public class CatStatueSouth2Addon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new CatStatueSouth2Deed(); } }

		[Constructable]
		public CatStatueSouth2Addon() : base()
		{
			AddComponent( new AddonComponent( 0x1949 ), 0, 0, 0 );
		}

		public CatStatueSouth2Addon( Serial serial ) : base( serial )
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

	public class CatStatueEastDeed : BaseAddonDeed
	{
		public override string DefaultName{ get{ return "a deed for a cat statue (east)"; } }

		public override BaseAddon Addon{ get{ return new CatStatueEastAddon(); } }

		[Constructable]
		public CatStatueEastDeed() : base()
		{
		}

		public CatStatueEastDeed( Serial serial ) : base( serial )
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

	public class CatStatueSouthDeed : BaseAddonDeed
	{
		public override string DefaultName{ get{ return "a deed for a cat statue (south)"; } }

		public override BaseAddon Addon{ get{ return new CatStatueSouthAddon(); } }

		[Constructable]
		public CatStatueSouthDeed() : base()
		{
		}

		public CatStatueSouthDeed( Serial serial ) : base( serial )
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

	public class CatStatueEast2Deed : BaseAddonDeed
	{
		public override string DefaultName{ get{ return "a deed for a cat statue of good fortune (east)"; } }

		public override BaseAddon Addon{ get{ return new CatStatueEastAddon(); } }

		[Constructable]
		public CatStatueEast2Deed() : base()
		{
		}

		public CatStatueEast2Deed( Serial serial ) : base( serial )
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

	public class CatStatueSouth2Deed : BaseAddonDeed
	{
		public override string DefaultName{ get{ return "a deed for a cat statue of good fortune (south)"; } }

		public override BaseAddon Addon{ get{ return new CatStatueSouth2Addon(); } }

		[Constructable]
		public CatStatueSouth2Deed() : base()
		{
		}

		public CatStatueSouth2Deed( Serial serial ) : base( serial )
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