using System;
using System.Text;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{
	public class BlackHairDye : Item
	{
		[Constructable]
		public BlackHairDye() : base( 0xEFC )
		{
			Weight = 1.0;
			Name = "a black hair dye";
			Hue = 1175;
		}

		public BlackHairDye( Serial serial ) : base( serial )
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

		public override void OnDoubleClick( Mobile from )
		{
			if ( IsChildOf( from.Backpack ) )
			{
				from.CloseGump( typeof( BlackHairDyeGump ) );
				from.SendGump( new BlackHairDyeGump( this ) );
			}
			else
				from.SendLocalizedMessage( 1042010 );
		}
	}

	public class BlackHairDyeGump : Gump
	{
		BlackHairDye m_BlackHairDye;

		public BlackHairDyeGump( BlackHairDye dye ) : base( 0, 0 )
		{
			m_BlackHairDye = dye;

			AddPage( 0 );
			AddBackground( 150, 60, 350, 138, 2600 );
			AddBackground( 170, 104, 110, 40, 5100 );
			AddHtmlLocalized( 230, 75, 200, 20, 1011013, false, false );		// Hair Color Selection Menu
			AddHtmlLocalized( 235, 150, 300, 20, 1011014, false, false );		// Dye my hair this color!
			AddButton( 200, 150, 0xFA5, 0xFA7, 1, GumpButtonType.Reply, 0 );        // DYE HAIR

			AddLabel( 180, 109, 0, "*****" );
			AddButton( 257, 110, 5224, 5224, 0, GumpButtonType.Page, 1 );

			AddPage( 1 );

			AddLabel( 335, 102, 0, "*****" );
			AddRadio( 310, 102, 210, 211, false, 0 );
		}

		public override void OnResponse( NetState from, RelayInfo info )
		{
			if ( m_BlackHairDye == null || m_BlackHairDye.Deleted )
				return;

			Mobile m = from.Mobile;

			if ( m_BlackHairDye.IsChildOf( m.Backpack ) )
			{
				if ( info.ButtonID != 0 )
				{
					if ( m.HairItemID > 0 )
					{
						m_BlackHairDye.Consume();

						m.HairHue = 1;

						m.SendLocalizedMessage( 501199 );  // You dye your hair
						m.PlaySound( 0x4E );
					}
					else
						m.SendLocalizedMessage( 502623 );	// You have no hair to dye and cannot use this
				}
				else
					m.SendLocalizedMessage( 1042010 ); //You must have the object in your backpack to use it.
			}
			else
				m.SendLocalizedMessage( 501200 ); // You decide not to dye your hair
		}
	}
}