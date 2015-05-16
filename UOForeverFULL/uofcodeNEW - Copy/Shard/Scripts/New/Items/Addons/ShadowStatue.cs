using System;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Engines.VeteranRewards;

namespace Server.Items
{
	public class ShadowStatueEastAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new ShadowStatueEastDeed(); } }

		[Constructable]
		public ShadowStatueEastAddon() : base()
		{
			AddComponent( new AddonComponent( 0x369B ), 0, 0, 0 );
		}

		public ShadowStatueEastAddon( Serial serial ) : base( serial )
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

	public class ShadowStatueSouthAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new ShadowStatueSouthDeed(); } }

		[Constructable]
		public ShadowStatueSouthAddon() : base()
		{
			AddComponent( new AddonComponent( 0x364B ), 0, 0, 0 );
		}

		public ShadowStatueSouthAddon( Serial serial ) : base( serial )
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

	public class ShadowStatueEastDeed : BaseAddonDeed
	{
		public override string DefaultName{ get{ return "a deed for a shadow statue (east)"; } }

		public override BaseAddon Addon{ get{ return new ShadowStatueEastAddon(); } }

		[Constructable]
		public ShadowStatueEastDeed() : base()
		{
		}

		public ShadowStatueEastDeed( Serial serial ) : base( serial )
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

	public class ShadowStatueSouthDeed : BaseAddonDeed
	{
		public override string DefaultName{ get{ return "a deed for a shadow statue (south)"; } }

		public override BaseAddon Addon{ get{ return new ShadowStatueSouthAddon(); } }

		[Constructable]
		public ShadowStatueSouthDeed() : base()
		{
		}

		public ShadowStatueSouthDeed( Serial serial ) : base( serial )
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