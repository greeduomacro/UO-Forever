using System;
using System.Collections.Generic;
using Server;
using Server.Multis;
using Server.Gumps;

namespace Server.Items
{
	public class ElvenAnvilAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new ElvenAnvilDeed(); } }

		[Constructable]
		public ElvenAnvilAddon( bool east )
		{
			if ( east )
				AddComponent( new AnvilComponent( 0x2DD5 ), 0, 0, 0 );
			else
				AddComponent( new AnvilComponent( 0x2DD6 ), 0, 0, 0 );
		}

		public ElvenAnvilAddon( Serial serial ) : base( serial )
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

	public class ElvenAnvilDeed : BaseAddonDeed, IRewardOption
	{
		public bool m_East;

		public override BaseAddon Addon{ get{ return new ElvenAnvilAddon( m_East ); } }
		public override string DefaultName{ get{ return "a deed for an elven anvil"; } }

		[Constructable]
		public ElvenAnvilDeed()
		{
		}

		public override void OnDoubleClick( Mobile from )
		{
			if (IsChildOf( from.Backpack ))
			{
				BaseHouse house = BaseHouse.FindHouseAt( from );

				if ( house != null && house.IsOwner( from ) )
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
			list.Add( 0, "Elven Anvil (South)" );
			list.Add( 1, "Elven Anvil (East)" );
		}

		public void OnOptionSelected( Mobile from, int choice )
		{
			m_East = choice == 1;

			if ( !Deleted )
				base.OnDoubleClick( from );
		}

		public ElvenAnvilDeed( Serial serial ) : base( serial )
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
}