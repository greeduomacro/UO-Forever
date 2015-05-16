using System;
using Server;
using Server.Accounting;
using Server.Gumps;

namespace Server.Engines.Jail
{
	/// <summary>
	/// This gump is used when trying to jail a player that already has another character in jail
	/// </summary>
	public class JailAccountGump : Gump
	{
		private const int LabelHue = 0x480;
		private const int GreenHue = 0x40;

		private Mobile m_Offender;
		private Mobile m_User;
		private JailEntry m_ExistingJail;
		private Account m_Account;

		public JailAccountGump( Mobile offender, Mobile user, JailEntry jail, Account account ) : base( 100, 100 )
		{
			m_Offender = offender;
			m_User = user;
			m_ExistingJail = jail;
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
			this.AddBackground(0, 0, 320, 220, 9250);
			this.AddAlphaRegion( 20, 20, 280, 180 );
			this.AddLabel(30, 30, LabelHue, @"The following account:");
			this.AddLabel(100, 50, GreenHue, m_Account.Username );
			this.AddLabel(30, 70, LabelHue, @"already has a character jailed.");
			this.AddHtml( 30, 100, 259, 60, @"<basefont color=#FFFFFF>Do you wish to convert the existing jailing and jail the full account rather than add a new entry?", false, false);
			this.AddButton(30, 170, 4005, 4006, 1, GumpButtonType.Reply, 0); // B 1: No
			this.AddButton(220, 170, 4005, 4006, 2, GumpButtonType.Reply, 0); // B 2: Yes
			this.AddLabel(260, 170, LabelHue, @"Yes");
			this.AddLabel(70, 170, LabelHue, @"No");
		}

		public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
		{
			switch ( info.ButtonID )
			{
				case 1 : // No: make a new jail entry\
					JailSystem.InitJail( m_User, m_Offender, true );
					break;
				case 2: // Yes: set current jail entry to full account
					m_ExistingJail.FullJail = true;
					m_ExistingJail.AddComment( m_User, "Turning on full jail to jail {0}", m_Offender.Name );
					m_User.SendGump( new JailViewGump( m_User, m_ExistingJail, null ) );
					break;
			}
		}
	}
}