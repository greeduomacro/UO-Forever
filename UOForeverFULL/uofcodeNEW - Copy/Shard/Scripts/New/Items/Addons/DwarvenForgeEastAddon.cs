using System;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Multis;

namespace Server.Items
{
	public class DwarvenForgeAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new DwarvenForgeAddonDeed(); } }

		[Constructable]
		public DwarvenForgeAddon( bool east )
		{
			if ( east )
			{
				AddonComponent ac = new AddonComponent( 6546 );
				ac.Name = "dwarven forge";
				AddComponent( ac, 0, 1, 0 );
				ac = new AddonComponent( 6554 );
				ac.Name = "dwarven forge";
				AddComponent( ac, 0, 0, 0 );
			}
			else
			{
				AddonComponent ac = new AddonComponent( 6558 );
				ac.Name = "dwarven forge";
				AddComponent( ac, 1, 0, 0 );
				ac = new AddonComponent( 6566 );
				ac.Name = "dwarven forge";
				AddComponent( ac, 0, 0, 0 );
			}
		}

		public DwarvenForgeAddon( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();
		}
	}

	public class DwarvenForgeAddonDeed : BaseAddonDeed, IRewardOption
	{
		public bool m_East;

		public override BaseAddon Addon{ get{ return new DwarvenForgeAddon( m_East ); } }
		public override string DefaultName{ get{ return "a dwarven forge deed"; } }

		[Constructable]
		public DwarvenForgeAddonDeed()
		{
			//LootType = LootType.Blessed;
		}

		public override void OnDoubleClick( Mobile from )
		{
			if (IsChildOf( from.Backpack ))
			{
				BaseHouse house = BaseHouse.FindHouseAt( from );

				if (house != null && house.IsOwner( from ))
				{
					from.CloseGump( typeof( RewardOptionGump ) );
					from.SendGump( new RewardOptionGump( this ) );
				}
				else
					from.SendLocalizedMessage( 502092 ); // You must be in your house to do this.
			}
			else
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
		}

		public void GetOptions( RewardOptionList list )
		{
			list.Add( 0, "Dwarven Forge (South)" );
			list.Add( 1, "Dwarven Forge (East)" );
		}

		public void OnOptionSelected( Mobile from, int choice )
		{
			m_East = choice == 1;

			if ( !Deleted )
				base.OnDoubleClick( from );
		}

		public DwarvenForgeAddonDeed( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int)0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}