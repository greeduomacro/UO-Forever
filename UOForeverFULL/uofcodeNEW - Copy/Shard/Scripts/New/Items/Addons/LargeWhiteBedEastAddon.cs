using System;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Multis;

namespace Server.Items
{
	public class LargeWhiteBedAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new LargeWhiteBedDeed(); } }

		[Constructable]
		public LargeWhiteBedAddon( bool east )
		{
			if ( east )
			{
				AddComponent( new AddonComponent( 0xA85 ), 0, 0, 0 );
				AddComponent( new AddonComponent( 0xA84 ), 0, 1, 0 );
				AddComponent( new AddonComponent( 0xA87 ), 1, 0, 0 );
				AddComponent( new AddonComponent( 0xA86 ), 1, 1, 0 );
			}
			else
			{
				AddComponent( new AddonComponent( 0xA89 ), 0, 0, 0 );
				AddComponent( new AddonComponent( 0xA8B ), 0, 1, 0 );
				AddComponent( new AddonComponent( 0xA88 ), 1, 0, 0 );
				AddComponent( new AddonComponent( 0xA8A ), 1, 1, 0 );
			}
		}

		public LargeWhiteBedAddon( Serial serial ) : base( serial )
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

	public class LargeWhiteBedDeed : BaseAddonDeed, IRewardOption
	{
		public bool m_East;

		public override BaseAddon Addon{ get{ return new LargeWhiteBedAddon( m_East ); } }
		public override string DefaultName{ get{ return "a large white bed deed"; } }

		[Constructable]
		public LargeWhiteBedDeed()
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
			list.Add( 0, "Large White Bed (South)" );
			list.Add( 1, "Large White Bed (East)" );
		}

		public void OnOptionSelected( Mobile from, int choice )
		{
			m_East = choice == 1;

			if ( !Deleted )
				base.OnDoubleClick( from );
		}

		public LargeWhiteBedDeed( Serial serial ) : base( serial )
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