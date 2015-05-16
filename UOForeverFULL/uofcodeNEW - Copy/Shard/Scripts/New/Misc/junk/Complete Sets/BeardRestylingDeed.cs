using System;
using Server.Mobiles;
using Server.Network;
using Server.Prompts;
using Server.Items;
using Server.Targeting;
using Server.Gumps;

namespace Server.Items
{
	public class BeardRestylingDeed : Item
	{

		[Constructable]
		public BeardRestylingDeed() : base( 0x14F0 )
		{
			Name = "a coupon for a free beard restyling";
			Weight = 1.0;
			LootType = LootType.Blessed;
		}

		public BeardRestylingDeed( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( from.Race == Race.Elf )
			{
				from.SendMessage( "This isn't implemented for elves yet.  Sorry!" );
				return;
			}

			/*if (from.Female == true)
			{
				from.SendMessage( "Only males can use this!");
				return;
			}*/

			if ( !IsChildOf( from.Backpack ) )
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			else
				from.SendGump( new InternalGump( from, this ) );
		}

		private class InternalGump : Gump
		{
			private Mobile m_From;
			private BeardRestylingDeed m_Deed;

			public InternalGump( Mobile from, BeardRestylingDeed deed ) : base( 50, 50 )
			{
				m_From = from;
				m_Deed = deed;

				from.CloseGump( typeof( InternalGump ) );

				AddBackground( 100, 10, 400, 385, 0xA28 );

				AddHtml( 100, 25, 400, 35, "<CENTER>BEARD SELECTION MENU</CENTER>", false, false ); //localized is not cookie cutter style of hair restyle :P
				
				AddButton( 175, 340, 0xFA5, 0xFA7, 0x0, GumpButtonType.Reply, 0 ); // CANCEL
				AddHtmlLocalized( 210, 342, 90, 35, 3000091, false, false);

				AddBackground( 220, 60, 50, 50, 0xA3C );
				AddBackground( 220, 115, 50, 50, 0xA3C );
				AddBackground( 220, 170, 50, 50, 0xA3C );
				AddBackground( 220, 225, 50, 50, 0xA3C );
				AddBackground( 425, 60, 50, 50, 0xA3C );
				AddBackground( 425, 115, 50, 50, 0xA3C );
				AddBackground( 425, 170, 50, 50, 0xA3C );
				AddBackground( 425, 225, 50, 50, 0xA3C );

				AddHtmlLocalized( 150, 75, 80, 35, 1011060, false, false );  // Short Beard
				AddHtmlLocalized( 150, 130, 80, 35, 1011061, false, false ); // Long Beard
				AddHtmlLocalized( 150, 185, 80, 35, 1011062, false, false ); // Mustache
				AddHtmlLocalized( 150, 240, 80, 35, 1015323, false, false ); // Goatee
				
				AddHtmlLocalized( 355, 75, 80, 35, 1011401, false, false );  // Vandyke
				AddHtmlLocalized( 355, 130, 80, 35, 1015321, false, false ); // Short Beard w/Mustache
				AddHtmlLocalized( 355, 185, 80, 35, 1015322, false, false ); // Long Beard w/ Mustache
				AddHtmlLocalized( 355, 240, 80, 35, 1075521, false, false ); // Clean Shaven

				AddImage( 153, 20,  0xC672 ); 
				AddImage( 153, 65,  0xC671 );
				AddImage( 153, 120, 0xC673 );
				AddImage( 153, 185, 0xC670 );
				AddImage( 358, 18,  0xC674 );
				AddImage( 358, 75,  0xC676 ); 
				AddImage( 358, 120, 0xC675 ); 

				AddButton( 118,  73, 0xFA5, 0xFA7, 2, GumpButtonType.Reply, 0 ); 
				AddButton( 118, 128, 0xFA5, 0xFA7, 3, GumpButtonType.Reply, 0 );
				AddButton( 118, 183, 0xFA5, 0xFA7, 4, GumpButtonType.Reply, 0 );
				AddButton( 118, 238, 0xFA5, 0xFA7, 5, GumpButtonType.Reply, 0 ); 
				
				AddButton( 323,  73, 0xFA5, 0xFA7, 6, GumpButtonType.Reply, 0 );
				AddButton( 323, 128, 0xFA5, 0xFA7, 7, GumpButtonType.Reply, 0 ); 
				AddButton( 323, 183, 0xFA5, 0xFA7, 8, GumpButtonType.Reply, 0 ); 
				AddButton( 323, 238, 0xFA5, 0xFA7, 1, GumpButtonType.Reply, 0 );
			}

			public override void OnResponse( NetState sender, RelayInfo info )
			{
				if ( m_Deed.Deleted )
					return;

				if ( info.ButtonID > 0 )
				{
					int itemID = 0;

				switch ( info.ButtonID )
				{
						case 2: itemID = 0x203F;	break;
						case 3: itemID = 0x203E;	break;
						case 4: itemID = 0x2041;	break;
						case 5: itemID = 0x2040;	break;
						case 6: itemID = 0x204D;	break;
						case 7: itemID = 0x204B;	break;
						case 8: itemID = 0x204C;	break;
				}

				if ( m_From is PlayerMobile )
				{
					PlayerMobile pm = (PlayerMobile)m_From;

					pm.SetHairMods( -1, -1 ); // clear any hair mods (disguise kit, incognito)
				}

					m_From.FacialHairItemID = itemID;
					m_Deed.Delete();
				}
			}
		}
	}
}