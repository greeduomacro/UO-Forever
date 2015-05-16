using System;
using Server;
using Server.Misc;
using Server.Mobiles;
using Server.Network;
using Server.Commands;

namespace Server.Gumps
{
	public class NameChangeGump : Gump
	{
		Mobile m_From;

		public NameChangeGump( Mobile from ) : this( from, NameResultMessage.Allowed, from.Name )
		{
		}

		public NameChangeGump( Mobile from, NameResultMessage result, string name ) : base( 150, 100 )
		{
			m_From = from;

			from.CloseGump( typeof( NameChangeGump ) );
			from.Hidden = true;
			from.CantWalk = true;
			from.Blessed = true;
			from.Squelched = true;

			Closable = false;
			Disposable = false;
			Dragable = false;
			Resizable = false;

			AddPage(0);

			AddBackground( 0, 0, 400, 350, 0x24A4 ); //9270
			//AddAlphaRegion( 13, 13, 473, 49 );
			//AddAlphaRegion( 13, 68, 473, 244 );

			AddHtml( 10, 35, 400, 20, Color( Center("Unique & Appropriate Name Required"), 0x111111 ), false, false );

			string message = String.Format( "Thank you for playing {0}. Please choose a unique player name with the following constraints:", ShardInfo.DisplayName );

			AddHtml( 35, 70, 350, 60, Color(  message, 0x7E1E00 ), false, false );

			AddHtml( 35, 130, 350, 20, Color( "* Name must be between 2 and 16 characters.", 0x666666 ), false, false );
			AddHtml( 35, 155, 350, 20, Color( "* Name must not include numbers.", 0x666666 ), false, false );
			AddHtml( 35, 180, 350, 20, Color( "* Name may include, but not start with, one (1) symbol.", 0x666666 ), false, false );
			AddHtml( 35, 205, 350, 20, Color( "* All player names are verified for appropriateness.", 0x666666 ), false, false );

			AddTextField( 50, 245, 250, 20, 1 );
			AddButton( 320, 244, 0xFB7, 0xFB8, 2, GumpButtonType.Reply, 0 );

			if ( result != NameResultMessage.Allowed )
			{
				string error = String.Empty;

				switch ( result )
				{
					default:
					case NameResultMessage.NotAllowed: error = String.Format( "The name {0} is not allowed.", name ); break;
					case NameResultMessage.InvalidCharacter: error = String.Format( "The name {0} contains invalid characters.", name ); break;
					case NameResultMessage.TooFewCharacters: case NameResultMessage.TooManyCharacters: error = "The name must be between 2 and 16 characters."; break;
					case NameResultMessage.AlreadyExists: error = String.Format( "A player with the name {0} already exists.", name ); break;
				}

				AddHtml( 55, 270, 350, 40, Color( String.Format( "Error: {0}", error ), 0xFF0000 ), false, false );
			}
		}

		public string Center( string text )
		{
			return String.Format( "<CENTER>{0}</CENTER>", text );
		}

		public string Color( string text, int color )
		{
			return String.Format( "<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", color, text );
		}

		public void AddTextField( int x, int y, int width, int height, int index )
		{
			AddTextField( x, y, width, height, index, String.Empty );
		}

		public void AddTextField( int x, int y, int width, int height, int index, string text )
		{
			AddBackground( x - 2, y - 2, width + 4, height + 4, 0x2486 );
			AddTextEntry( x + 2, y + 2, width - 4, height - 4, 0, index, text );
		}

        public override void OnResponse( NetState sender, RelayInfo info )
        {
			int val = info.ButtonID;

			if ( val == 2 )
			{
				TextRelay first = info.GetTextEntry(1);
				string name = first != null ? first.Text.Trim() : String.Empty;

				string lowername = name != null ? name.ToLower() : String.Empty;
				NameResultMessage result = NameVerification.ValidatePlayerName( lowername, 2, 16, true, false, true, 1, NameVerification.SpaceDashPeriodQuote );

				if ( result == NameResultMessage.Allowed )
					AllowName( name );
				else
					m_From.SendGump( new NameChangeGump( m_From, result, name ) );
			}
		}

		private void AllowName( string name )
		{
			m_From.RawName = name;
			m_From.Hidden = false;
			m_From.CantWalk = false;
			m_From.Blessed = false;
			m_From.Squelched = false;
			m_From.SendMessage( String.Format( "Your name has been changed to {0}.  Please re-open your paperdoll.", name ) );
		}

		public static void Initialize()
		{
			EventSink.Login += new LoginEventHandler( OnLogin );
		}

		private static void OnLogin( LoginEventArgs e )
		{
			PlayerMobile mob = e.Mobile as PlayerMobile;
            if (mob == null) return; // pseudoseer controlled basecreature
			if ( AccessLevelToggler.GetRawAccessLevel( mob ) == AccessLevel.Player ) //Incase [staff was used
			{
				NameResultMessage result = NameVerification.Validate( mob.RawName, 2, 16, true, false, true, 1, NameVerification.SpaceDashPeriodQuote );
				if ( result != NameResultMessage.Allowed )
					mob.SendGump( new NameChangeGump( mob ) );
			}
		}
	}
}