using System;
using Server;
using Server.Gumps;

namespace Server.Engines.Jail
{
	/// <summary>
	/// Describes all the commands available in the jail system
	/// </summary>
	public class JailHelpGump : Gump
	{
		private static string html =
@"<basefont color=#FFFFFF>All jailing operation is managed through the <basefont color=#3300CC>Jail<basefont color=#FFFFFF> command. The command supports a number of additional switches:

<basefont color=#3300CC><u>Jail</u><basefont color=#FFFFFF>: When used without any switches, the command will bring up a target to jail a player.
<basefont color=#3300CC><u>Jail Release</u><basefont color=#FFFFFF>: Will release the targeted player from jail by closing their jail record. If the player received a full account jail, all of the characters on the account will be released.
<basefont color=#3300CC><u>Jail View</u><basefont color=#FFFFFF>: Displays the list of all current jail records. This list doesn't include jailings who have expired or that have been canceled manually.
<basefont color=#3300CC><u>Jail Info</u><basefont color=#FFFFFF>: Displays the current jail report for the targeted player
<basefont color=#3300CC><u>Jail History</u><basefont color=#FFFFFF>: Displays the jailing history for the targeted player
<basefont color=#3300CC><u>Jail Admin</u><basefont color=#FFFFFF>: Displays the jailing administration menu which allows to view the complete history as well as purge it
<basefont color=#3300CC><u>Jail Help</u><basefont color=#FFFFFF>: Displays this description
<basefont color=#3300CC><u>Jail Account [account]</u><basefont color=#FFFFFF>: Searches the accounts list for an account that matches [account] (useful for offline jailing)
<basefont color=#3300CC><u>Jail Player [name]</u><basefont color=#FFFFFF>: Searches the players list for a mobile whose name matches [name] (useful for offline jailing)";

		public JailHelpGump( Mobile user ) : base( 100, 100 )
		{
			user.CloseGump( typeof( JailHelpGump ) );

			this.Closable=true;
			this.Disposable=true;
			this.Dragable=true;
			this.Resizable=false;

			this.AddPage(0);
			this.AddBackground(0, 0, 500, 300, 9250);
			this.AddImageTiled(15, 40, 470, 245, 2624);
			this.AddAlphaRegion(15, 15, 470, 20);
			this.AddLabel(185, 15, 0x480, @"Jail System Help");
			this.AddButton(455, 15, 4017, 4018, 0, GumpButtonType.Reply, 0);
			this.AddAlphaRegion(15, 40, 470, 245);
			this.AddHtml( 15, 40, 470, 245, html, (bool)false, (bool)true);
		}
	}
}