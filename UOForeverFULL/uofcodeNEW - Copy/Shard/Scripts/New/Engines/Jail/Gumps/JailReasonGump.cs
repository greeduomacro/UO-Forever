using System;
using Server;
using Server.Gumps;
using Server.Accounting;

namespace Server.Engines.Jail
{
	/// <summary>
	/// First gump in the jailing sequence
	/// </summary>
	public class JailReasonGump : Gump
	{
		private const int LabelHue = 0x480;
		private const int RedHue = 0x20;
		private const int GreenHue = 0x40;

		/// <summary>
		/// The player being jailed
		/// </summary>
		private Mobile m_Offender;

		/// <summary>
		/// The account of the player being jailed
		/// </summary>
		private Account m_Account;

		/// <summary>
		/// Defines the 5 common reasons supported by the gump
		/// </summary>
		private string[] m_Reasons = { "Staff Harassment", "Player Harassment", "AFK Macroing", "Quest Spoiling", "Cheating" };

		public JailReasonGump( Mobile offender ) : base( 100, 100 )
		{
			m_Offender = offender;

			if ( m_Offender.Account != null )
				m_Account = offender.Account as Account;

			MakeGump();
		}

		public JailReasonGump( Account account ) : base( 100, 100 )
		{
			m_Offender = null;
			m_Account = account;

			MakeGump();
		}

		private void MakeGump()
		{
			this.Closable=false;
			this.Disposable=true;
			this.Dragable=true;
			this.Resizable=false;
			this.AddPage(0);
			this.AddBackground(0, 0, 290, 380, 9250);
			this.AddAlphaRegion(15, 15, 260, 350);
			this.AddLabel(170, 20, RedHue, @"New Jail Record");

			// Account name
			this.AddLabel( 30, 40, LabelHue, "Account:" );

			if ( m_Account != null )
				this.AddLabel( 100, 40, GreenHue, m_Account.Username );
			else
				this.AddLabel( 100, 40, GreenHue, "N/A" );

			// Player name
			this.AddLabel(30, 60, LabelHue, "Player:" );

			if ( m_Offender != null )
				this.AddLabel(100, 60, GreenHue, m_Offender.Name );
			else
				this.AddLabel( 100, 60, GreenHue, "All characters" );

			this.AddLabel(30, 85, RedHue, @"Jailing Reason:");

			for ( int i = 0; i < 5; i++ )
			{
				AddLabel( 70, 110 + i * 30, LabelHue, m_Reasons[ i ] );
				AddButton( 30, 110 + i * 30, 4005, 4006, i + 1, GumpButtonType.Reply, 0 );
			}

			this.AddLabel(70, 260, LabelHue, @"Other:");
			this.AddImageTiled( 69, 279, 202, 22, 9384 );
			this.AddAlphaRegion(70, 280, 200, 20);
			this.AddTextEntry(70, 280, 200, 20, LabelHue, 0, @"");
			this.AddButton(30, 260, 4005, 4006, 6, GumpButtonType.Reply, 0); // Other

			this.AddLabel(70, 320, LabelHue, @"Cancel the jailing");
			this.AddLabel(70, 340, LabelHue, @"This will release the player");
			this.AddButton(30, 320, 4002, 4003, 0, GumpButtonType.Reply, 0); // Cancel
		}

		public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
		{
			string reason = null;

			switch ( info.ButtonID )
			{
				case 0: // Cancel the jailing
				{
					if ( m_Offender != null )
						JailSystem.CancelJail( m_Offender, sender.Mobile );
					else if ( m_Account != null )
						sender.Mobile.SendMessage( "You have canceled the jailing of the account {0}.", m_Account.Username );

					return;
				}
				case 1: // Predefined reasons
				case 2:
				case 3:
				case 4:
				case 5:
				{
					reason = m_Reasons[ info.ButtonID - 1 ];
					break;
				}
				case 6: // Other, check for text
				{
					TextRelay text = info.GetTextEntry( 0 );
					if ( text != null && !String.IsNullOrEmpty( text.Text ) )
						reason = text.Text;
					break;
				}
			}

			if ( String.IsNullOrEmpty( reason ) )
			{
				sender.Mobile.SendMessage( "Please specify a valid reason for the jailing." );

				if ( m_Offender != null )
					sender.Mobile.SendGump( new JailReasonGump( m_Offender ) );
				else
					sender.Mobile.SendGump( new JailReasonGump( m_Account ) );
			}
			else
				sender.Mobile.SendGump( new JailingGump( m_Offender, m_Account, reason ) );
		}

	}
}