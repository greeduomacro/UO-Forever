using Server.Multis;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{
	public class ShadowAltarAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new ShadowAltarAddonDeed(); } }

		[Constructable]
		public ShadowAltarAddon( bool east )
		{
			if ( east )
			{
				AddonComponent comp = new AddonComponent( 0x2A9C );
				comp.Name = "shadow altar";
				AddComponent( comp, 0, 0, 0 );

				comp = new AddonComponent( 0x2A9D );
				comp.Name = "shadow altar";
				AddComponent( comp, 0, -1, 0 );
			}
			else
			{
				AddonComponent comp = new AddonComponent( 0x2A9A );
				comp.Name = "shadow altar";
				AddComponent( comp, 0, 0, 0 );

				comp = new AddonComponent( 0x2A9B );
				comp.Name = "shadow altar";
          		AddComponent( comp, -1, 0, 0 );
			}
		}

		public ShadowAltarAddon( Serial serial ) : base( serial )
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

	public class ShadowAltarAddonDeed : BaseAddonDeed, IRewardOption
	{
		public bool m_East;

		public override BaseAddon Addon{ get{ return new ShadowAltarAddon( m_East ); } }
		public override string DefaultName{ get{ return "a deed for a shadow altar"; } }

		[Constructable]
		public ShadowAltarAddonDeed()
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
			list.Add( 0, "Shadow Altar (South)" );
			list.Add( 1, "Shadow Altar (East)" );
		}

		public void OnOptionSelected( Mobile from, int choice )
		{
			m_East = choice == 1;

			if ( !Deleted )
				base.OnDoubleClick( from );
		}

		public ShadowAltarAddonDeed( Serial serial ) : base( serial )
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