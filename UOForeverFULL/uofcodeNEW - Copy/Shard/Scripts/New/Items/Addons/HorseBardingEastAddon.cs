using System;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Multis;

namespace Server.Items
{
	public class HorseBardingAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new HorseBardingDeed(); } }

		[Constructable]
		public HorseBardingAddon( bool east )
		{
			if ( east )
			{
				AddComponent( new AddonComponent( 0x1379 ), 0, 0, 0 );
				AddComponent( new AddonComponent( 0x1378 ), 0, 1, 0 );
			}
			else
			{
				AddComponent( new AddonComponent( 0x1376 ), 0, 0, 0 );
				AddComponent( new AddonComponent( 0x1377 ), 1, 0, 0 );
			}
		}

		public HorseBardingAddon( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class HorseBardingDeed : BaseAddonDeed, IRewardOption
	{
		public bool m_East;

		public override BaseAddon Addon{ get{ return new HorseBardingAddon( m_East ); } }
		public override string DefaultName{ get{ return "a horse barding deed"; } }

		[Constructable]
		public HorseBardingDeed()
		{
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
			list.Add( 0, "Horse Barding (South)" );
			list.Add( 1, "Horse Barding (East)" );
		}

		public void OnOptionSelected( Mobile from, int choice )
		{
			m_East = choice == 1;

			if ( !Deleted )
				base.OnDoubleClick( from );
		}

		public HorseBardingDeed( Serial serial ) : base( serial )
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