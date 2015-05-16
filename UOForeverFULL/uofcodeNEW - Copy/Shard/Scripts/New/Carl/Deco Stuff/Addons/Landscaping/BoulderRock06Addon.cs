/////////////////////////////////////////////////
//
// Automatically generated by the
// AddonGenerator script by Arya
//
/////////////////////////////////////////////////
using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class BoulderRock06Addon : BaseAddon
	{
		public override BaseAddonDeed Deed
		{
			get
			{
				return new BoulderRock06AddonDeed();
			}
		}

		[ Constructable ]
		public BoulderRock06Addon()
		{
			AddonComponent ac = null;
			ac = new AddonComponent( 4956 );
			AddComponent( ac, 0, 1, 0 );
			ac = new AddonComponent( 4957 );
			AddComponent( ac, 1, 1, 0 );
			ac = new AddonComponent( 4958 );
			AddComponent( ac, 1, 0, 0 );

		}

		public BoulderRock06Addon( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class BoulderRock06AddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new BoulderRock06Addon();
			}
		}

		[Constructable]
		public BoulderRock06AddonDeed()
		{
			Name = "BoulderRock06";
		}

		public BoulderRock06AddonDeed( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( 0 ); // Version
		}

		public override void	Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}