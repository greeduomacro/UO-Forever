using System;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Multis;
using Server.Network;

namespace Server.Items
{
	public class DecoOreMiningCart : BaseAddon
	{
		public override BaseAddonDeed Deed
		{
			get
			{
				DecoOreMiningCartDeed deed = new DecoOreMiningCartDeed();
				return deed;
			}
		}
		[Constructable]
		public DecoOreMiningCart( bool east ) : base()
		{
			if ( east )
			{
				AddComponent( new AddonComponent( 0x1A88 ), 0, 0, 0 );
				AddComponent( new AddonComponent( 0x1A87 ), 1, 0, 0 );
				AddComponent( new AddonComponent( 0x1A8B ), -1, 0, 0 );
			}
			else
			{
				AddComponent( new AddonComponent( 0x1A83 ), 0, 0, 0 );
				AddComponent( new AddonComponent( 0x1A82 ), 0, 1, 0 );
				AddComponent( new AddonComponent( 0x1A86 ), 0, -1, 0 );
			}


			/* Gems
					AddComponent( new LocalizedAddonComponent( 0x1A83, 1080388 ), 0, 0, 0 );
					AddComponent( new LocalizedAddonComponent( 0x1A82, 1080388 ), 0, 1, 0 );
					AddComponent( new LocalizedAddonComponent( 0x1A86, 1080388 ), 0, -1, 0 );

					AddComponent( new AddonComponent( 0xF2C ), 0, 0, 6 );
					AddComponent( new AddonComponent( 0xF1D ), 0, 0, 5 );
					AddComponent( new AddonComponent( 0xF2B ), 0, 0, 2 );
					AddComponent( new AddonComponent( 0xF21 ), 0, 0, 1 );
					AddComponent( new AddonComponent( 0xF22 ), 0, 0, 4 );
					AddComponent( new AddonComponent( 0xF2F ), 0, 0, 5 );
					AddComponent( new AddonComponent( 0xF26 ), 0, 0, 6 );
					AddComponent( new AddonComponent( 0xF27 ), 0, 0, 3 );
					AddComponent( new AddonComponent( 0xF29 ), 0, 0, 0 );


					East:

					AddComponent( new LocalizedAddonComponent( 0x1A88, 1080388 ), 0, 0, 0 );
					AddComponent( new LocalizedAddonComponent( 0x1A87, 1080388 ), 1, 0, 0 );
					AddComponent( new LocalizedAddonComponent( 0x1A8B, 1080388 ), -1, 0, 0 );

					AddComponent( new AddonComponent( 0xF2E ), 0, 0, 6 );
					AddComponent( new AddonComponent( 0xF12 ), 0, 0, 3 );
					AddComponent( new AddonComponent( 0xF29 ), 0, 0, 1 );
					AddComponent( new AddonComponent( 0xF24 ), 0, 0, 5 );
					AddComponent( new AddonComponent( 0xF21 ), 0, 0, 1 );
					AddComponent( new AddonComponent( 0xF2B ), 0, 0, 3 );
					AddComponent( new AddonComponent( 0xF2F ), 0, 0, 4 );
					AddComponent( new AddonComponent( 0xF23 ), 0, 0, 3 );
					AddComponent( new AddonComponent( 0xF27 ), 0, 0, 3 );
			*/
		}

		public DecoOreMiningCart( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class DecoOreMiningCartDeed : BaseAddonDeed, IRewardOption
	{
		public override string DefaultName{ get{ return "a deed for a decorative mining cart"; } }

		public bool m_East;

		public override BaseAddon Addon
		{
			get
			{
				DecoOreMiningCart addon = new DecoOreMiningCart( m_East );
				return addon;
			}
		}

		private bool m_IsRewardItem;

		[CommandProperty( AccessLevel.GameMaster )]
		public bool IsRewardItem
		{
			get{ return m_IsRewardItem; }
			set{ m_IsRewardItem = value; InvalidateProperties(); }
		}

		[Constructable]
		public DecoOreMiningCartDeed() : base()
		{
		}

		public DecoOreMiningCartDeed( Serial serial ) : base( serial )
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
			list.Add( 0, 1080391 );
			list.Add( 1, 1080390 );
		}

		public void OnOptionSelected( Mobile from, int choice )
		{
			m_East = choice == 1;

			if ( !Deleted )
				base.OnDoubleClick( from );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version

			writer.Write( (bool) m_IsRewardItem );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();

			m_IsRewardItem = reader.ReadBool();
		}
	}
}