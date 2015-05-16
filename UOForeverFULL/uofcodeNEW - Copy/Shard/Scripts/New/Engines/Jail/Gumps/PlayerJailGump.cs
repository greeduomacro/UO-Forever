using System;
using Server;
using Server.Gumps;
using Server.Commands;

namespace Server.Engines.Jail
{
	/// <summary>
	/// This gump is sent to the player with the details of their sentence
	/// </summary>
	public class PlayerJailGump : Gump
	{
		private const int LabelHue = 0x480;
		private const int GreenHue = 0x40;
		private const int RedHue = 0x20;

		private JailEntry m_Jail;
		private Mobile m_Jailer;

		public PlayerJailGump( JailEntry jail, Mobile jailer ) : base( 100, 100 )
		{
			m_Jail = jail;
			m_Jailer = jailer;

			MakeGump();
		}

		private void MakeGump()
		{
			this.Closable=true;
			this.Disposable=true;
			this.Dragable=true;
			this.Resizable=false;

			this.AddPage(0);
			this.AddBackground(0, 0, 430, 155, 9250);

			this.AddImageTiled(15, 15, 400, 20, 5154);
			this.AddAlphaRegion(15, 15, 400, 20);
			this.AddLabel(20, 15, LabelHue, @"You've been jailed by");
			this.AddLabelCropped( 180, 15, 235, 20, RedHue, string.Format( "{0} ({1})", m_Jailer.Name, m_Jailer.AccessLevel.ToString() ) );

			this.AddAlphaRegion(15, 35, 400, 20);
			this.AddLabel(20, 35, LabelHue, @"Reason:");
			this.AddLabelCropped(100, 35, 315, 20, GreenHue, m_Jail.Reason);

			this.AddImageTiled(15, 55, 400, 20, 5154);
			this.AddAlphaRegion(15, 55, 400, 20);
			this.AddLabel(20, 55, LabelHue, @"Duration");

			if ( m_Jail.AutoRelease )
				this.AddLabel(100, 55, GreenHue, string.Format( "{0} days and {1} hours", m_Jail.Duration.Days, m_Jail.Duration.Hours ) );
			else
				this.AddLabel(100, 55, GreenHue, "Undetermined" );

			this.AddAlphaRegion(15, 75, 400, 40);
			this.AddLabel(20, 75, LabelHue, @"You can verify your remaining jail time by typing");
			this.AddLabel(20, 90, LabelHue, string.Format( "{0}JailInfo", CommandSystem.Prefix ) );

			this.AddButton(15, 120, 4017, 4018, 0, GumpButtonType.Reply, 0);
		}
	}
}