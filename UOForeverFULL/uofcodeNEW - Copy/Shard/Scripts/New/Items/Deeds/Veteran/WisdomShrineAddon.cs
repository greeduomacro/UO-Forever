using System;
using Server;
using Server.Engines.VeteranRewards;

namespace Server.Items
{
	public class WisdomShrineAddon : BaseAddon, IRewardItem
	{
		public override BaseAddonDeed Deed
		{
			get
			{
				WisdomShrineAddonDeed deed = new WisdomShrineAddonDeed();
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
		public WisdomShrineAddon()
		{
			AddComponent( new AddonComponent( 0x14C3 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 0x14D4 ), 0, 1, 0 );
			AddComponent( new AddonComponent( 0x14C6 ), 1, 0, 0 );
			AddComponent( new AddonComponent( 0x14D5 ), 1, 1, 0 );
		}

		public WisdomShrineAddon( Serial serial ) : base( serial )
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

	public class WisdomShrineAddonDeed : BaseAddonDeed, IRewardItem
	{
		public override BaseAddon Addon
		{
			get
			{
				WisdomShrineAddon addon = new WisdomShrineAddon();
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

		public override string DefaultName{ get{ return "a deed for a shrine of wisdom"; } }

		[Constructable]
		public WisdomShrineAddonDeed()
		{
			LootType = LootType.Blessed;

		}

		public WisdomShrineAddonDeed( Serial serial ) : base( serial )
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