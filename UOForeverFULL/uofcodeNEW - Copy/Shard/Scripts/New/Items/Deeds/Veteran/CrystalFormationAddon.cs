using System;
using Server;
using Server.Engines.VeteranRewards;

namespace Server.Items
{
	public class CrystalFormationAddon : BaseAddon, IRewardItem
	{
		public override BaseAddonDeed Deed
		{
			get
			{
				CrystalFormationAddonDeed deed = new CrystalFormationAddonDeed();
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
		public CrystalFormationAddon()
		{
			AddComponent( new AddonComponent( 8743 ), -1, 0, 0 );
			AddComponent( new AddonComponent( 12253 ), -1, -1, 0 );
			AddComponent( new AddonComponent( 8738 ), 0, -1, 0 );
			AddComponent( new AddonComponent( 8770 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 8769 ), -1, -2, 0 );
			AddComponent( new AddonComponent( 8768 ), 0, 1, 0 );
			AddComponent( new AddonComponent( 8767 ), 0, 2, 0 );
			AddComponent( new AddonComponent( 8766 ), -1, 1, 0 );
			AddComponent( new AddonComponent( 8765 ), 1, 0, 0 );
			AddComponent( new AddonComponent( 8764 ), 0, 1, 0 );
			AddComponent( new AddonComponent( 8736 ), 0, -2, 0 );

		}

		public CrystalFormationAddon( Serial serial ) : base( serial )
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

	public class CrystalFormationAddonDeed : BaseAddonDeed, IRewardItem
	{
		public override BaseAddon Addon
		{
			get
			{
				CrystalFormationAddon addon = new CrystalFormationAddon();
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
		public CrystalFormationAddonDeed()
		{
            LootType = LootType.Blessed;
			Name = "deed for a crystal formation";
		}

		public CrystalFormationAddonDeed( Serial serial ) : base( serial )
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