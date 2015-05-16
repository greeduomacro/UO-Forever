using System;
using Server;
using Server.Engines.VeteranRewards;

namespace Server.Items
{
	public class AmethystTreeAddon : BaseAddon, IRewardItem
	{
        private bool m_IsRewardItem;

		[CommandProperty( AccessLevel.GameMaster )]
		public bool IsRewardItem
		{
			get { return m_IsRewardItem; }
			set { m_IsRewardItem = value; }
		}

		public override BaseAddonDeed Deed
		{
			get
			{
				AmethystTreeAddonDeed deed = new AmethystTreeAddonDeed();
				deed.IsRewardItem = m_IsRewardItem;
				return deed;
			}
		}

		[ Constructable ]
		public AmethystTreeAddon()
		{
			AddComponent( new AddonComponent( 3302 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 3207 ), -1, 1, 20 );
			AddComponent( new AddonComponent( 3207 ), 1, -1, 18 );
			AddComponent( new AddonComponent( 3207 ), 1, 1, 33 );
			AddComponent( new AddonComponent( 3207 ), -1, 1, 10 );
			AddComponent( new AddonComponent( 3207 ), 1, -1, 10 );
			AddComponent( new AddonComponent( 3207 ), 1, 1, 20 );

		}

		public AmethystTreeAddon( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( (int)0 ); // Version

			writer.Write( m_IsRewardItem );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();

			m_IsRewardItem = reader.ReadBool();
		}
	}

	public class AmethystTreeAddonDeed : BaseAddonDeed, IRewardItem
	{
        private bool m_IsRewardItem;

		[CommandProperty( AccessLevel.GameMaster )]
		public bool IsRewardItem
		{
			get { return m_IsRewardItem; }
			set { m_IsRewardItem = value; }
		}

		public override BaseAddon Addon
		{
			get
			{
				AmethystTreeAddon addon = new AmethystTreeAddon();
				addon.IsRewardItem = m_IsRewardItem;
				return addon;
			}
		}

		[Constructable]
		public AmethystTreeAddonDeed()
		{
            LootType = LootType.Blessed;
			Name = "deed for an amethyst tree";
		}

		public AmethystTreeAddonDeed( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( (int)0 ); // Version

			writer.Write( m_IsRewardItem );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();

			m_IsRewardItem = reader.ReadBool();
		}
	}
}