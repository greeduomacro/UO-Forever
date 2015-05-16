using System;
using Server.Network;
using Server.Prompts;
using Server.Items;
using Server.Targeting;
using Server.Gumps;
using Server.Mobiles;
using Server.Engines.VeteranRewards;

namespace Server.Items
{
	public class EtherealMountDeed : Item, IRewardOption
	{
		private Mobile m_Owner;

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile Owner
		{
			get{ return m_Owner; }
			set{ m_Owner = value; }
		}

		public override string DefaultName{ get{ return "a deed for an ethereal mount"; } }

		[Constructable]
		public EtherealMountDeed() : base( 0x14F0 )
		{
			Weight = 6.0;
			Hue = 2212;
			LootType = LootType.Blessed;
		}

		public EtherealMountDeed( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version
			writer.Write( (Mobile) m_Owner );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();

			switch ( version )
			{
				case 0:
				{
					m_Owner = reader.ReadMobile();
					break;
				}
			}
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( IsChildOf( from.Backpack ) )
			{
				from.CloseGump( typeof( RewardOptionGump ) );
				from.SendGump( new RewardOptionGump( this ) );
			}
			else
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
		}

		public void GetOptions( RewardOptionList list )
		{
			list.Width = 400;

			list.Add( 1, "Donation Ethereal Horse (no veterancy required)" );
			list.Add( 2, "Donation Ethereal Llama (no veterancy required)" );
			list.Add( 3, "Donation Ethereal Desert Ostard (no veterancy required)" );
			list.Add( 4, "Donation Ethereal Forest Ostard (no veterancy required)" );
			list.Add( 5, "Donation Ethereal Frenzied Ostard (no veterancy required)" );
		}

		public void OnOptionSelected( Mobile from, int option )
		{
			if ( !Deleted )
			{
				Item item = null;

				switch ( option )
				{
					case 1: item = new EtherealHorse(); break;
					case 2: item = new EtherealLlama(); break;
					case 3: item = new EtherealOstard(); break;
					case 4: item = new EtherealForestOstard(); break;
					case 5: item = new EtherealFrenziedOstard(); break;
				}

				//Donation item has been claimed
				from.Backpack.DropItem( item );
				Consume();
			}
		}
	}
}