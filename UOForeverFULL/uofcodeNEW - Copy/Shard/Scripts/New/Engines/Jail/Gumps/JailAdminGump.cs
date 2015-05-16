using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Accounting;
using Server.Mobiles;

namespace Server.Engines.Jail
{
	/// <summary>
	/// Gump used to view/purge jail history
	/// </summary>
	public class JailAdminGump : Gump
	{
		private const int LabelHue = 0x480;
		private const int GreenHue = 0x40;
		private const int RedHue = 0x20;

		private bool m_Banned;
		private bool m_Deleted;
		private bool m_Old;
		private int m_Months;

		public JailAdminGump( Mobile user ) : this( user, false, true, false, 12 )
		{
		}

		public JailAdminGump( Mobile user, bool banned, bool deleted, bool old, int months ) : base( 100, 100 )
		{
			user.CloseGump( typeof( JailAdminGump ) );
			MakeGump();
		}

		private void MakeGump()
		{
			this.Closable=true;
			this.Disposable=true;
			this.Dragable=true;
			this.Resizable=false;

			this.AddPage(0);
			this.AddBackground(0, 0, 240, 375, 9250);

			this.AddAlphaRegion(15, 15, 210, 45);
			this.AddLabel(20, 15, GreenHue, @"Jail Administration");

			// View Full History: Button 1
			this.AddButton(20, 35, 4029, 4030, 1, GumpButtonType.Reply, 0);
			this.AddLabel(55, 35, LabelHue, @"View Full History");

			this.AddAlphaRegion(15, 65, 210, 100);

			this.AddLabel(20, 70, LabelHue, @"Search:");

			// Search: Text 0
			this.AddImageTiled(24, 89, 192, 22, 5154);
			this.AddAlphaRegion(25, 90, 190, 20);
			this.AddTextEntry(25, 90, 190, 20, LabelHue, 0, @"");

			// Search for players: Button 2
			this.AddLabel(60, 115, LabelHue, @"For Players");
			this.AddButton(25, 115, 4005, 4006, 2, GumpButtonType.Reply, 0);

			// Search for accounts: Button 3
			this.AddLabel(60, 140, LabelHue, @"For Accounts");
			this.AddButton(25, 140, 4005, 4006, 3, GumpButtonType.Reply, 0);

			this.AddAlphaRegion(15, 170, 210, 155);
			this.AddLabel(20, 175, LabelHue, @"Purge History");

			// Deleted accounts: Switch 0
			this.AddCheck(25, 200, 9010, 9008, m_Deleted, 0);
			this.AddLabel(45, 195, LabelHue, @"Deleted Accounts");

			// Banned accounts: Switch 1
			this.AddCheck(25, 225, 9010, 9008, m_Banned, 1);
			this.AddLabel(45, 220, LabelHue, @"Banned Accounts");

			// Old entries: Switch 2
			this.AddCheck(25, 250, 9010, 9008, m_Old, 2);
			this.AddLabel(45, 245, LabelHue, @"Entries older than:");

			// Months: Text 1
			this.AddImageTiled(44, 269, 42, 22, 5154);
			this.AddAlphaRegion(45, 270, 40, 20);
			this.AddTextEntry(55, 270, 20, 20, LabelHue, 1, @"");
			this.AddLabel(95, 270, LabelHue, @"Months");

			// Perform Purge: Button 4
			this.AddButton(25, 300, 4005, 4006, 4, GumpButtonType.Reply, 0);
			this.AddLabel(60, 300, LabelHue, @"Perform Purge");

			this.AddAlphaRegion(15, 330, 210, 30);

			// View Jail: Button 5
			this.AddLabel(60, 335, LabelHue, @"View Jail");
			this.AddButton(25, 335, 4008, 4009, 5, GumpButtonType.Reply, 0);

			// Close: Button 0
			this.AddButton(185, 335, 4017, 4018, 0, GumpButtonType.Reply, 0);
		}

		public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
		{
			// Get information first
			m_Banned = false;
			m_Deleted = false;
			m_Old = false;

			foreach( int flag in info.Switches )
			{
				switch ( flag )
				{
					case 0: // Banned
						m_Banned = true;
						break;
					case 1: // Deleted
						m_Deleted = true;
						break;
					case 2: // Old
						m_Old = true;
						break;
				}
			}

			TextRelay text = info.GetTextEntry( 1 );
			if ( text != null && !String.IsNullOrEmpty( text.Text ) )
			{
				int months = Utility.ToInt32( info.GetTextEntry( 1 ).Text );
				if ( months > 0 && months <= 12 )
					m_Months = months;
			}

			switch ( info.ButtonID )
			{
				case 1: // View History
				{
					List<JailEntry> history = JailSystem.GetFullHistory();

					if ( history.Count > 0 )
					{
						sender.Mobile.SendGump( new JailListingGump( sender.Mobile, JailSystem.GetFullHistory(), new JailGumpCallback( JailAdminGumpCallback ) ) );
					}
					else
					{
						sender.Mobile.SendMessage( "The history is empty" );
						sender.Mobile.SendGump( new JailAdminGump( sender.Mobile, m_Banned, m_Deleted, m_Old, m_Months ) );
					}
					break;
				}
				case 2: // Search for players
				case 3: // Search for accounts
				{
					text = info.GetTextEntry( 0 );
					if ( text != null && !String.IsNullOrEmpty( text.Text ) )
					{
						List<PlayerMobile> mobmatches = new List<PlayerMobile>();
						List<Account> accmatches = new List<Account>();

						if ( info.ButtonID == 2 )
							mobmatches = JailSystem.SearchForPlayers( text.Text );
						else
							accmatches = JailSystem.SearchForAccounts( text.Text );

						if ( mobmatches.Count > 0 || accmatches.Count > 0 )
							sender.Mobile.SendGump( new JailSearchGump( mobmatches, accmatches, sender.Mobile, new JailGumpCallback( JailAdminGumpCallback ) ) );
						else
						{
							sender.Mobile.SendMessage( "No matches found" );
							sender.Mobile.SendGump( new JailAdminGump( sender.Mobile, m_Banned, m_Deleted, m_Old, m_Months ) );
						}
					}
					else
					{
						sender.Mobile.SendMessage( "Invalid search" );
						sender.Mobile.SendGump( new JailAdminGump( sender.Mobile, m_Banned, m_Deleted, m_Old, m_Months ) );
					}
					break;
				}
				case 4: // Purge
				{
					if ( ! ( m_Deleted || m_Banned || m_Old ) )
					{
						sender.Mobile.SendMessage( "Invalid purge options. Please correct and try again." );
						sender.Mobile.SendGump( new JailAdminGump( sender.Mobile, m_Banned, m_Deleted, m_Old, m_Months ) );
					}
					else
					{
						JailPurge purge = new JailPurge( m_Banned, m_Deleted, m_Old, m_Months );
						JailSystem.PurgeHistory( sender.Mobile, purge );
						sender.Mobile.SendGump( new JailAdminGump( sender.Mobile ) );
					}
					break;
				}
				case 5: // View Jail
				{
					sender.Mobile.SendGump( new JailListingGump( sender.Mobile, JailSystem.Jailings, new JailGumpCallback( JailAdminGumpCallback ) ) );
					break;
				}
			}
		}

		private static void JailAdminGumpCallback( Mobile user )
		{
			user.SendGump( new JailAdminGump( user ) );
		}
	}
}