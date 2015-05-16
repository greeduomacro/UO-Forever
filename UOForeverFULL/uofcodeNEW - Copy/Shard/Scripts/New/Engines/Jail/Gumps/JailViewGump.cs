using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Gumps;

namespace Server.Engines.Jail
{
	/// <summary>
	/// This gump displays a jailing entry
	/// </summary>
	public class JailViewGump : Gump
	{
		private const int LabelHue = 1152;
		private const int GreenHue = 64;
		private const int RedHue = 32;

		private Mobile m_User;
		private JailEntry m_Jail;
		private JailGumpCallback m_Callback;

		public JailViewGump( Mobile user, JailEntry jail, JailGumpCallback callback ) : base( 50, 50 )
		{
			user.CloseGump( typeof( JailViewGump ) );

			m_User = user;
			m_Jail = jail;
			m_Callback = callback;

			MakeGump();
		}

		private void MakeGump()
		{
			this.Closable=true;
			this.Disposable=true;
			this.Dragable=true;
			this.Resizable=false;

			bool active = JailSystem.IsActive( m_Jail );

			this.AddPage(0);

			this.AddBackground(0, 0, 600, 400, 9250);
			this.AddAlphaRegion(15, 15, 570, 370);

			// Title
			this.AddLabel(20, 20, RedHue, @"Jail Record Review");

			// Status
			this.AddLabel(290, 20, LabelHue, @"Status:");
			int statusHue = GreenHue;
			string status = "Active";

			if ( m_Jail.Account == null )
			{
				statusHue = RedHue;
				status = "Account Deleted";
			}
			else if ( m_Jail.Account.Banned )
			{
				statusHue = RedHue;
				status = "Account Banned";
			}
			else if ( m_Jail.Expired )
			{
				statusHue = GreenHue;
				status = "Expired";
			}

			this.AddLabel(350, 20, statusHue, status);

			// History Record
			this.AddImageTiled(19, 39, 562, 52, 3604);
			this.AddAlphaRegion(20, 40, 560, 50);
			this.AddHtml( 20, 40, 560, 50, string.Format( "<basefont color=#CCCCCC>{0}", m_Jail.HistoryRecord ), false, false);

			// Player
			this.AddLabel(40, 90, LabelHue, @"Player:");

			if ( m_Jail.Mobile != null )
			{
				// Player Props: Button 1
                if (m_User.AccessLevel >= JailSystem.m_JailLevel)
                {
                    if ( m_User.AccessLevel >= AccessLevel.Lead )
                        this.AddButton(19, 93, 5601, 5605, 1, GumpButtonType.Reply, 0);
                    this.AddLabel(90, 90, GreenHue, m_Jail.Mobile.Name);
                }
			}
			else
				this.AddLabel(90, 90, RedHue, "Not Available" );

			// Account
			this.AddLabel(310, 90, LabelHue, @"Account:");

			if ( m_Jail.Account != null )
			{
				// Account Information: Button 2
				if ( m_User.AccessLevel >= AccessLevel.Administrator )
					this.AddButton(290, 93, 5601, 5605, 2, GumpButtonType.Reply, 0);

				this.AddLabel(370, 90, GreenHue, m_Jail.Account.Username );
			}
			else
				this.AddLabel(370, 90, RedHue, "Deleted");

			if ( active )
			{
				// Jail Duration
				this.AddLabel(20, 120, LabelHue, "Jail Duration:");

				// Days: Text 0
				this.AddLabel(160, 120, LabelHue, @"Days");
				this.AddImageTiled(114, 119, 42, 22, 5154);
				this.AddAlphaRegion(115, 120, 40, 20);
				this.AddTextEntry(125, 120, 20, 20, LabelHue, 0, m_Jail.Duration.Days.ToString() );

				// Hours: Text 1
				this.AddLabel(240, 120, LabelHue, @"Hours");
				this.AddImageTiled(194, 119, 42, 22, 5154);
				this.AddAlphaRegion(195, 120, 40, 20);
				this.AddTextEntry(205, 120, 20, 20, LabelHue, 1, m_Jail.Duration.Hours.ToString() );

				// Update Jail Duration: Button 7
                if ( m_User.AccessLevel >= JailSystem.m_JailLevel )
                {
                    this.AddButton(290, 120, 4005, 4006, 7, GumpButtonType.Reply, 0);
                    this.AddLabel(330, 120, LabelHue, @"Update jail duration");
                }

				// Auto Release
                this.AddLabel(20, 150, LabelHue, @"Auto Release:");
                this.AddLabel(160, 150, m_Jail.AutoRelease ? GreenHue : RedHue, m_Jail.AutoRelease ? "Enabled" : "Disabled");

				// Toggle Auto Release: Button 3
                if ( m_User.AccessLevel >= JailSystem.m_JailLevel )
                {
                    this.AddButton(290, 150, 4005, 4006, 3, GumpButtonType.Reply, 0);
                    this.AddLabel(330, 150, LabelHue, string.Format("Turn {0} auto release", m_Jail.AutoRelease ? "off" : "on"));
                }

				// Full Account Jail
                this.AddLabel(20, 180, LabelHue, @"Full Account Jail:");
                this.AddLabel(160, 180, m_Jail.FullJail ? GreenHue : RedHue, m_Jail.FullJail ? "Enabled" : "Disabled");

				// Full Account Jail toggle: Button 4
				if ( m_Jail.Mobile != null || ( !m_Jail.FullJail ) )
				{
                    if ( m_User.AccessLevel >= JailSystem.m_JailLevel )
                    {
                        this.AddLabel(330, 180, LabelHue, string.Format("Turn {0} full account jail", m_Jail.FullJail ? "off" : "on"));
                        this.AddButton(290, 180, 4005, 4006, 4, GumpButtonType.Reply, 0);
                    }
				}

				// New comment: Text 2
				this.AddLabel(290, 210, LabelHue, @"New comment");
				this.AddImageTiled(289, 229, 292, 102, 5154);
				this.AddAlphaRegion(290, 230, 290, 100);
				this.AddTextEntry(290, 230, 290, 100, LabelHue, 2, @"");

				// Add comment: Button 5
				this.AddLabel(330, 335, LabelHue, @"Add comment");
				this.AddButton(290, 335, 4011, 4012, 5, GumpButtonType.Reply, 0);

				// Unjail: Button 6
                if ( m_User.AccessLevel >= JailSystem.m_JailLevel )
                {
                    this.AddLabel(330, 360, LabelHue, @"Unjail");
                    this.AddButton(290, 360, 4002, 4003, 6, GumpButtonType.Reply, 0);
                }
			}
			else
				this.AddLabel(20, 120, LabelHue, "This jailing has expired and cannot be modified" );

			// Comments
			string html = "";

			if ( m_Jail.Comments.Count == 0 )
				html = "There are no comments";
			else
			{
				foreach( string comment in m_Jail.Comments )
					html += string.Format( "{0}<br>", comment );
			}

			this.AddLabel(20, 210, LabelHue, @"Comments:");
			this.AddImageTiled(19, 229, 262, 152, 3604);
			this.AddAlphaRegion(20, 230, 260, 150);
			this.AddHtml( 20, 230, 260, 150, html, false, true);

			if ( m_User.AccessLevel >= JailSystem.m_HistoryLevel && m_Jail.Account != null )
			{
				// History: Button 8
				this.AddButton(450, 335, 4029, 4030, 8, GumpButtonType.Reply, 0);
				this.AddLabel(490, 335, LabelHue, @"History");
			}

			// Close: button 0
			this.AddButton(450, 360, 4023, 4024, 0, GumpButtonType.Reply, 0);
			this.AddLabel(490, 360, LabelHue, @"Close");
		}

		public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
		{
			switch ( info.ButtonID )
			{
				case 0: // Close the gump
				{
					if ( m_Callback != null )
					{
						try
						{
							m_Callback.DynamicInvoke( new object[] { m_User } );
						}
						catch {}
					}
					break;
				}
				case 1: // Display player props
				{
                    if (sender.Mobile.AccessLevel >= JailSystem.m_JailLevel)
					{
						m_User.SendGump( new JailViewGump( m_User, m_Jail, m_Callback ) );

						if ( m_User.AccessLevel >= AccessLevel.Lead && m_Jail.Mobile != null )
							m_User.SendGump( new PropertiesGump( m_User, m_Jail.Mobile ) );
					}
					break;
				}
				case 2: // Display account admin
				{
                    if ( sender.Mobile.AccessLevel >= AccessLevel.Administrator )
					{
						m_User.SendGump( new JailViewGump( m_User, m_Jail, m_Callback ) );

						if ( m_Jail.Account != null )
							m_User.SendGump( new AdminGump( m_User, AdminGumpPage.AccountDetails_Information, 0, null, "Jailed account information", m_Jail.Account ) );
					}
					break;
				}
				case 3: // AutoRelease toggle
				{
                    if (sender.Mobile.AccessLevel >= JailSystem.m_JailLevel)
					{
						m_Jail.AutoRelease = !m_Jail.AutoRelease;
						m_User.SendGump( new JailViewGump( m_User, m_Jail, m_Callback ) );
					}
					break;
				}
				case 4: // FullJail toggle
				{
                    if (sender.Mobile.AccessLevel >= JailSystem.m_JailLevel)
					{
						m_Jail.FullJail = !m_Jail.FullJail;
						m_User.SendGump( new JailViewGump( m_User, m_Jail, m_Callback ) );
					}
					break;
				}
				case 5: // Add comment
				{
					TextRelay comment = info.GetTextEntry( 2 );

					if ( comment == null || String.IsNullOrEmpty( comment.Text ) )
						m_User.SendMessage( "Can't add an empty comment" );
					else
						m_Jail.AddComment( m_User, comment.Text );

					m_User.SendGump( new JailViewGump( m_User, m_Jail, m_Callback ) );
					break;
				}
				case 6: // Unjail
				{
                    if (sender.Mobile.AccessLevel >= JailSystem.m_JailLevel)
					{
						JailSystem.Release(m_Jail, m_User);
						m_User.SendGump( new JailViewGump( m_User, m_Jail, m_Callback ) );
					}
					break;
				}
				case 7: // Update duration
				{
                    if ( sender.Mobile.AccessLevel >= JailSystem.m_JailLevel )
					{
						TimeSpan duration = TimeSpan.Zero;

						TextRelay text = info.GetTextEntry( 0 );
						if ( text != null && !String.IsNullOrEmpty( text.Text ) )
							duration += TimeSpan.FromDays( Utility.ToInt32( text.Text ) );

						text = info.GetTextEntry( 1 );
						if ( text != null && !String.IsNullOrEmpty( text.Text ) )
							duration += TimeSpan.FromHours( Utility.ToInt32( text.Text ) );

						if ( duration == TimeSpan.Zero )
							m_User.SendMessage( "Invalid duration specified. If you wish to release the player, use the unjail button." );
						else
							m_Jail.Duration = duration;

						m_User.SendGump( new JailViewGump( m_User, m_Jail, m_Callback ) );
					}
					break;
				}
				case 8: // History
				{
                    if (sender.Mobile.AccessLevel >= JailSystem.m_HistoryLevel)
					{
						m_User.SendGump(new JailViewGump(m_User, m_Jail, m_Callback));

						List<JailEntry> history = JailSystem.SearchHistory( m_Jail.Account );

						if ( history.Count > 0 )
							m_User.SendGump( new JailListingGump( m_User, history, null ) );
					}
					break;
				}
			}
		}

		private void JailViewCallback( Mobile user )
		{
			user.SendGump( new JailViewGump( user, m_Jail, m_Callback ) );
		}

	}
}