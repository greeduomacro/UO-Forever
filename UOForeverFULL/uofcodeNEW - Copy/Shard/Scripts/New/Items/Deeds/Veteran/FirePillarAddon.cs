using System;
using Server;
using Server.Engines.VeteranRewards;

namespace Server.Items
{
	public class FirePillarAddon : BaseAddon, IRewardItem
	{
		public override BaseAddonDeed Deed
		{
			get
			{
				FirePillarAddonDeed deed = new FirePillarAddonDeed();
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
		public FirePillarAddon()
		{
			AddComponent( new AddonComponent( 7978 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 7979 ), 0, 0, 7 );
			AddonComponent ac = new AddonComponent( 6571 );
			ac.Light = LightType.ArchedWindowEast;
			AddComponent( ac, 0, 0, 10 );
		}

		public FirePillarAddon( Serial serial ) : base( serial )
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

	public class FirePillarAddonDeed : BaseAddonDeed, IRewardItem
	{
		public override BaseAddon Addon
		{
			get
			{
				FirePillarAddon addon = new FirePillarAddon();
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
		public FirePillarAddonDeed()
		{
			LootType = LootType.Blessed;
			Name = "deed for a pillar of fire";
		}

		public FirePillarAddonDeed( Serial serial ) : base( serial )
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