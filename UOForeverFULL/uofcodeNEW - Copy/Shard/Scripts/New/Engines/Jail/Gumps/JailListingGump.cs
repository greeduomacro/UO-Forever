using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Gumps;

namespace Server.Engines.Jail
{
	/// <summary>
	/// This is the main jail listing gump
	/// </summary>
	public class JailListingGump : Gump
	{
		private const int LabelHue = 0x480;
		private const int GreenHue = 0x40;
		private const int RedHue = 0x20;

		private List<JailEntry> m_MasterList;
		private JailEntry[] m_Jailings;
		private int m_Page;
		private Mobile m_User;
		private JailGumpCallback m_Callback;

		public JailListingGump( Mobile user, List<JailEntry> list, int page, JailGumpCallback callback ) : base( 100, 100 )
		{
			user.CloseGump( typeof( JailListingGump ) );

			m_Callback = callback;
			m_User = user;
			m_MasterList = list;
			m_Page = page;
			m_Jailings = list.ToArray();
			MakeGump();
		}

		public JailListingGump( Mobile user, List<JailEntry> list, JailGumpCallback callback ) : this( user, list, 0, callback )
		{
		}

		private void MakeGump()
		{
			this.Closable=true;
			this.Disposable=true;
			this.Dragable=true;
			this.Resizable=false;

			this.AddPage(0);
			this.AddBackground(0, 0, 420, 315, 9250);

			this.AddImageTiled( 15, 15, 16, 285, 2624 );

			// Title
			this.AddAlphaRegion(35, 15, 370, 20);
			this.AddLabel(40, 15, GreenHue, string.Format( "Jail Listing: Displaying {0} records", m_MasterList.Count ) );

			// Table headers: Account
			this.AddImageTiled(35, 40, 110, 20, 9304);
			this.AddAlphaRegion(35, 40, 110, 20);
			this.AddLabel(40, 40, LabelHue, @"Account");

			// Player
			this.AddImageTiled( 150, 40, 110, 20, 9304 );
			this.AddAlphaRegion(150, 40, 110, 20);
			this.AddLabel(155, 40, LabelHue, @"Player");

			// Reason
			this.AddImageTiled(265, 40, 140, 20, 9304);
			this.AddAlphaRegion(265, 40, 140, 20);
			this.AddLabel(270, 40, LabelHue, @"Reason");

			// Table Background
			this.AddAlphaRegion(35, 65, 110, 200);

			this.AddImageTiled(150, 65, 110, 200, 3604);
			this.AddAlphaRegion(150, 65, 110, 200);

			this.AddAlphaRegion(265, 65, 140, 200);

			this.AddAlphaRegion(35, 270, 370, 30);

			// Close: Button 0
			this.AddButton(45, 275, 4017, 4018, 0, GumpButtonType.Reply, 0);
			this.AddLabel(80, 275, LabelHue, @"Close");

			if ( m_MasterList.Count == 0 )
				return;

			// Data Section. Verify bounds
			if ( m_Page * 10 >= m_MasterList.Count )
			{
				// Show last page
				m_Page = ( m_MasterList.Count -1 ) / 10;
			}

			if ( m_Page > 0 )
			{
				// Prev Page: Button 1
				this.AddButton(335, 275, 4014, 4015, 1, GumpButtonType.Reply, 0);
			}

			if ( m_Page < ( m_MasterList.Count - 1 ) / 10 )
			{
				// Next Page: Button 2
				this.AddButton(370, 275, 4005, 4006, 2, GumpButtonType.Reply, 0);
			}

			for ( int i = m_Page * 10; i < m_Page * 10 + 10 && i < m_MasterList.Count; i++ )
			{
				int index = i - m_Page * 10;

				JailEntry jail = m_MasterList[i];

				// Jail Details: Buttons from 10 to 19
				this.AddButton(15, 67 + index * 20, 5601, 5605, 10 + index, GumpButtonType.Reply, 0);

				// Account details: Buttons from 20 to 29
				if ( jail.Account != null && m_User.AccessLevel == AccessLevel.Administrator )
					this.AddButton(35, 69 + index * 20, 2224, 2224, 20 + index, GumpButtonType.Reply, 0);

				// Account name
				if ( jail.Account != null )
					this.AddLabelCropped( 55, 65 + index * 20, 90, 20, LabelHue, jail.Account.Username );
				else
					this.AddLabelCropped( 55, 65 + index * 20, 90, 20, RedHue, "Deleted" );

				// Player name
				if ( jail.Mobile != null )
				{
					// Player props: Buttons from 30 to 39
                    if (m_User.AccessLevel >= AccessLevel.GameMaster)
					    this.AddButton(150, 69 + index * 20, 2224, 2224, 30 + index, GumpButtonType.Reply, 0);
					this.AddLabelCropped(170, 65 + index * 20, 90, 20, LabelHue, jail.Mobile.Name );
				}
				else
					this.AddLabelCropped(170, 65 + index * 20, 90, 20, LabelHue, "N/A" );

				// Reason
				this.AddLabelCropped(270, 65 + index * 20, 140, 20, LabelHue, jail.Reason );
			}
		}

		public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
		{
			switch ( info.ButtonID )
			{
				case 0: // Close

					if ( m_Callback != null )
					{
						try
						{
							m_Callback.DynamicInvoke( new object[] { m_User } );
						}
						catch {}
					}
					break;

				case 1: // Previous Page
					m_User.SendGump( new JailListingGump( m_User, m_MasterList, m_Page - 1, m_Callback ) );
					break;

				case 2: // Next Page
					m_User.SendGump( new JailListingGump( m_User, m_MasterList, m_Page + 1, m_Callback ) );
					break;

				default:

					int type = info.ButtonID / 10;
					int index = info.ButtonID % 10;

					JailEntry jail = m_Jailings[index];

					if ( jail != null )
					{
						switch ( type )
						{
							case 1: // View Information
							{
								m_User.SendGump( new JailViewGump( m_User, jail, new JailGumpCallback( JailListingCallback ) ) );
								break;
							}
							case 2: // View Account details
							{
								m_User.SendGump( new JailListingGump( m_User, m_MasterList, m_Page, m_Callback ) );

								if ( jail.Account != null )
									m_User.SendGump( new AdminGump( m_User, AdminGumpPage.AccountDetails_Information, 0, null, "Requested by the Jail System", jail.Account ) );

								break;
							}
							case 3: // View Player props
							{
								m_User.SendGump( new JailListingGump( m_User, m_MasterList, m_Page, m_Callback ) );

                                if ( m_User.AccessLevel < AccessLevel.Lead)
									return;

                                if ( jail.Mobile != null )
									m_User.SendGump( new PropertiesGump( m_User, jail.Mobile ) );

								break;
							}
						}
					}
					break;
			}
		}

		/// <summary>
		/// The callback used when viewing an entry
		/// </summary>
		private void JailListingCallback( Mobile from )
		{
			from.SendGump( new JailListingGump( from, m_MasterList, m_Page, m_Callback ) );
		}
	}
}