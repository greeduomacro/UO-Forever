using System;
using Server;
using Server.Engines.VeteranRewards;

namespace Server.Items
{
	public class LilacBushAddon : BaseAddon, IRewardItem
	{
		public override BaseAddonDeed Deed
		{
			get
			{
				LilacBushAddonDeed deed = new LilacBushAddonDeed();
				deed.IsRewardItem = m_IsRewardItem;
				return deed;
			}
		}

		private bool m_IsRewardItem;

		[CommandProperty( AccessLevel.GameMaster )]
		public bool IsRewardItem
		{
			get { return m_IsRewardItem; }
			set { m_IsRewardItem = value; }
		}

		[Constructable]
		public LilacBushAddon()
		{
			AddComponent( new AddonComponent( 3230 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 3146 ), 0, 1, 21 );
			AddComponent( new AddonComponent( 3146 ), 1, 0, 20 );
			AddComponent( new AddonComponent( 3146 ), 1, 1, 20 );
			AddComponent( new AddonComponent( 3146 ), 1, 1, 31 );
			AddComponent( new AddonComponent( 3145 ), -1, 1, 10 );
			AddComponent( new AddonComponent( 3142 ), 1, 0, 15 );
		}

		public LilacBushAddon( Serial serial ) : base( serial )
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

	public class LilacBushAddonDeed : BaseAddonDeed, IRewardItem
	{
		public override BaseAddon Addon
		{
			get
			{
				LilacBushAddon addon = new LilacBushAddon();
				addon.IsRewardItem = m_IsRewardItem;
				return addon;
			}
		}

		private bool m_IsRewardItem;

		[CommandProperty( AccessLevel.GameMaster )]
		public bool IsRewardItem
		{
			get { return m_IsRewardItem; }
			set { m_IsRewardItem = value; }
		}

		[Constructable]
		public LilacBushAddonDeed()
		{
            LootType = LootType.Blessed;
			Name = "deed for a lilac bush";
		}

		public LilacBushAddonDeed( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( (int)0 ); // version

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