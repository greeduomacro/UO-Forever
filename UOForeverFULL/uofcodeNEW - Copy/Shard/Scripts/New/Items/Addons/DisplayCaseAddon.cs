using Server.Multis;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{
	public class DisplayCaseAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new DisplayCaseAddonDeed(); } }

		[Constructable]
		public DisplayCaseAddon( bool east )
		{
			if ( east )
			{
				AddonComponent comp = new AddonComponent( 0xB06 );
				comp.Name = "wooden display case";
				AddComponent( comp, 0, 1, 0 );

				comp = new AddonComponent( 0xB07 );
				comp.Name = "wooden display case";
				AddComponent( comp, 0, 0, 0 );

				comp = new AddonComponent( 0xB08 );
				comp.Name = "wooden display case";
				AddComponent( comp, 0, -1, 0 );

				comp = new AddonComponent( 0xB03 );
				comp.Name = "wooden display case";
				AddComponent( comp, 0, 1, 3 );

				comp = new AddonComponent( 0xB04 );
				comp.Name = "wooden display case";
				AddComponent( comp, 0, 0, 3 );

				comp = new AddonComponent( 0xB05 );
				comp.Name = "wooden display case";
				AddComponent( comp, 0, -1, 3 );

			}
			else
			{
				AddonComponent comp = new AddonComponent( 0xB02 );
				comp.Name = "wooden display case";
				AddComponent( comp, -1, 0, 0 );

				comp = new AddonComponent( 0xB01 );
				comp.Name = "wooden display case";
				AddComponent( comp, 0, 0, 0 );

				comp = new AddonComponent( 0xB00 );
				comp.Name = "wooden display case";
				AddComponent( comp, 1, 0, 0 );

				comp = new AddonComponent( 0xAFF );
				comp.Name = "wooden display case";
				AddComponent( comp, -1, 0, 3 );

				comp = new AddonComponent( 0xAFE );
				comp.Name = "wooden display case";
				AddComponent( comp, 0, 0, 3 );

				comp = new AddonComponent( 0xAFD );
				comp.Name = "wooden display case";
				AddComponent( comp, 1, 0, 3 );
			}
		}

		public DisplayCaseAddon( Serial serial ) : base( serial )
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

	public class DisplayCaseAddonDeed : BaseAddonDeed, IRewardOption
	{
		public bool m_East;

		public override BaseAddon Addon{ get{ return new DisplayCaseAddon( m_East ); } }
		public override string DefaultName{ get{ return "a deed for a wooden display case"; } }

		[Constructable]
		public DisplayCaseAddonDeed()
		{
            LootType = LootType.Blessed;
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
			list.Add( 0, "Wooden Display Case (South)" );
			list.Add( 1, "Wooden Display Case (East)" );
		}

		public void OnOptionSelected( Mobile from, int choice )
		{
			m_East = choice == 1;

			if ( !Deleted )
				base.OnDoubleClick( from );
		}

		public DisplayCaseAddonDeed( Serial serial ) : base( serial )
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