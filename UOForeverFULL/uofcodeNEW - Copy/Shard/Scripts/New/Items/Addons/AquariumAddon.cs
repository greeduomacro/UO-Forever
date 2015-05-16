using Server.Multis;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{
	public class AquariumAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new AquariumAddonDeed(); } }

		[Constructable]
		public AquariumAddon( bool east )
		{
			if ( east )
			{
				AddonComponent comp = new AddonComponent( 0x3062 );
				comp.Name = "aquarium";
				AddComponent( comp, 0, 0, 0 );

				comp = new AddonComponent( 0x3063 );
				comp.Name = "aquarium";
				AddComponent( comp, 0, -1, 0 );
			}
			else
			{
				AddonComponent comp = new AddonComponent( 0x3060 );
				comp.Name = "aquarium";
				AddComponent( comp, 0, 0, 0 );

				comp = new AddonComponent( 0x3061 );
				comp.Name = "aquarium";
				AddComponent( comp, -1, 0, 0 );
			}
		}

		public AquariumAddon( Serial serial ) : base( serial )
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

	public class AquariumAddonDeed : BaseAddonDeed, IRewardOption
	{
		public bool m_East;

		public override BaseAddon Addon{ get{ return new AquariumAddon( m_East ); } }
		public override string DefaultName{ get{ return "an aquarium deed"; } }

		[Constructable]
		public AquariumAddonDeed()
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
			list.Add( 0, "Aquarium (South)" );
			list.Add( 1, "Aquarium (East)" );
		}

		public void OnOptionSelected( Mobile from, int choice )
		{
			m_East = choice == 1;

			if ( !Deleted )
				base.OnDoubleClick( from );
		}

		public AquariumAddonDeed( Serial serial ) : base( serial )
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