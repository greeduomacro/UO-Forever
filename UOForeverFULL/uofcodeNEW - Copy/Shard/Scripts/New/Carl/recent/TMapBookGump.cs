  //
 //  Written by Haazen May 2006
//
using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Network;
using Server.Prompts;
using Server.Multis;

namespace Server.Gumps
{
	public class TMapBookGump : Gump
	{
		private TMapBook m_Book;

		public TMapBook Book{ get{ return m_Book; } }

		public int GetMapHue( Map map )
		{
			if ( map == Map.Trammel )
				return 10;
			else if ( map == Map.Felucca )
				return 81;
			else if ( map == Map.Ilshenar )
				return 1102;
			else if ( map == Map.Malas )
				return 1102;

			return 0;
		}


		public string GetName( string name )
		{
			if ( name == null || (name = name.Trim()).Length <= 0 )
				return "(indescript)";

			return name;
		}

		private void AddBackground()
		{
			AddPage( 0 );

			AddImage( 100, 10, 2200 ); // background 

			for ( int i = 0; i < 2; ++i ) // page separators
			{
				int xOffset = 125 + (i * 165);

				AddImage( xOffset, 105, 57 );
				xOffset += 20;

				for ( int j = 0; j < 6; ++j, xOffset += 15 )
					AddImage( xOffset, 105, 58 );

				AddImage( xOffset - 5, 105, 59 );
			} 

			//  page buttons
			for ( int i = 0, xOffset = 130, gumpID = 2225; i < 4; ++i, xOffset += 35, ++gumpID )
				AddButton( xOffset, 187, gumpID, gumpID, 0, GumpButtonType.Page, 1 + i );


			for ( int i = 0, xOffset = 300, gumpID = 2229; i < 4; ++i, xOffset += 35, ++gumpID )
				AddButton( xOffset, 187, gumpID, gumpID, 0, GumpButtonType.Page, 5 + i ); 


		}

		private void AddDetails( int index, int half, int tb )
		{
			int hue;

			if ( index < m_Book.Entries.Count )
			{
				int btn;
				btn = (index * 2) + 1;
		
				TMapBookEntry e = (TMapBookEntry)m_Book.Entries[index];
				hue = GetMapHue( e.Map );

					AddLabel( 135 + (half * 160), 40 + (tb * 80), hue, String.Format( "Level {0}  {1}", e.Level, e.Map ) );

				if( e.Decoder == null )
					AddLabel( 135 + (half * 160), 55 + (tb * 80), hue, String.Format( "Not Decoded" ) );
				else
				{
					AddLabel( 135 + (half * 160), 55 + (tb * 80), hue, String.Format( "Decoder  {0}", e.Decoder.Name ) );
				AddButton( 135 + (half * 160), 89 + (tb * 80), 216, 216, btn + 1, GumpButtonType.Reply, 0 );
				
				AddHtml( 150 + (half * 160), 87 + (tb * 80), 100, 18, "View", false, false );
				}

				// buttons

				AddButton( 135 + (half * 160), 75 + (tb * 80), 2437, 2438, btn, GumpButtonType.Reply, 0 );
				
				AddHtml( 150 + (half * 160), 73 + (tb * 80), 100, 18, "Drop TMap", false, false );
			}
		}

		public TMapBookGump( Mobile from, TMapBook book ) : base( 150, 200 )
		{
			m_Book = book;

			AddBackground();

			for ( int page = 0; page < 8; ++page )
			{
				AddPage( 1 + page );
				
				if ( page > 0 ) //0
				AddButton( 125, 14, 2205, 2205, 0, GumpButtonType.Page, page );

				if ( page < 7 )
				AddButton( 393, 14, 2206, 2206, 0, GumpButtonType.Page, 2 + page );

				if ( page < 4 )
				AddImage ( 135 + ( page * 35), 190, 36 );
				if ( page > 3 )
				AddImage ( 305 + (( page - 4) * 35), 190, 36 );

				for ( int half = 0; half < 2; ++half )
				{
					int tb = 0;
					AddDetails( (page * 4) + (half * 2), half, tb );
					tb = 1;
					AddDetails( (page * 4) + (half * 2) + 1, half, tb );
				}
			}
		}

		public override void OnResponse( NetState state, RelayInfo info )
		{
			Mobile from = state.Mobile;

			if ( m_Book.Deleted || !from.InRange( m_Book.GetWorldLocation(), 1 ) || !Multis.DesignContext.Check( from ) )
				return;

			int buttonID = info.ButtonID;

			int index = (buttonID / 2);
			int drp = buttonID % 2;

			if ( index >= 0 && index < m_Book.Entries.Count && drp == 1 )
			{
				TMapBookEntry e = (TMapBookEntry)m_Book.Entries[index];

				if ( m_Book.CheckAccess( from ) )
				{
					m_Book.DropTMap( from, e, index );
					from.CloseGump( typeof( TMapBookGump ) );
					from.SendGump( new TMapBookGump( from, m_Book ) );
				}
				else
				{
					from.SendLocalizedMessage( 502413 ); // That cannot be done while the book is locked down.
				}

			}
		// ViewMap
			 else	if (index >= 1 && index < m_Book.Entries.Count +1 && drp == 0)
			{

				index = index - 1;
				
				TMapBookEntry e = (TMapBookEntry)m_Book.Entries[index];
				if ( m_Book.CheckAccess( from ) )
				{

					from.CloseGump( typeof( TMapBookGump ) );
					from.SendGump( new TMapBookGump( from, m_Book ) );
					m_Book.ViewMap( from, e, index );
				}
				else
				{
					from.SendLocalizedMessage( 502413 ); // That cannot be done while the book is locked down.
				}

			}  // end view
		}
	}
}