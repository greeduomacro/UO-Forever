using System;
using System.Collections;
using Server;
using Server.Misc;
using Server.Gumps;
using Server.Network;
using Server.Mobiles;
using Server.Items;

namespace Server.Gumps
{
	public class KillRemoveGump : Gump
	{
			//private Mobile m;
	
		public KillRemoveGump (Mobile m) : base( 0, 0 )
		{
			BuildGump();
		}

		private void BuildGump()
		{
			AddBackground( 265, 205, 320, 120, 5054 );
			Closable = false;
			Resizable = false;

			AddPage( 0 );

			AddImageTiled( 225, 175, 50, 45, 0xCE );   //Top left corner
			AddImageTiled( 267, 175, 315, 44, 0xC9 );  //Top bar
			AddImageTiled( 582, 175, 43, 45, 0xCF );   //Top right corner
			AddImageTiled( 225, 219, 44, 110, 0xCA );  //Left side
			AddImageTiled( 582, 219, 44, 110, 0xCB );  //Right side
			AddImageTiled( 225, 300, 44, 43, 0xCC );   //Lower left corner
			AddImageTiled( 267, 300, 315, 43, 0xE9 );  //Lower Bar
			AddImageTiled( 582, 300, 43, 43, 0xCD );   //Lower right corner

			AddPage( 1 );

			AddLabel( 320, 230, 1049066, "Would you like to reset your kills?"); // Would you like to report...

			AddButton( 330, 270, 0xFA5, 0xFA7, 1, GumpButtonType.Reply, 0 );
			AddHtmlLocalized( 370, 270, 300, 50, 1046362, false, false ); // Yes

			AddButton( 470, 270, 0xFA5, 0xFA7, 2, GumpButtonType.Reply, 0 );
			AddHtmlLocalized( 510, 270, 300, 50, 1046363, false, false ); // No
		}
		
		public override void OnResponse( NetState state, RelayInfo info )
		{
			PlayerMobile user = state.Mobile as PlayerMobile;
			Mobile from = state.Mobile;
			
			switch ( info.ButtonID )
			{
				case 1: 
				{
					from.ShortTermMurders = 0;
					from.Kills = 0;
					from.SendMessage("Your kills have been reset.");
					user.KMUsed = 1;
					break;
				}
				case 2: 
				{
					from.SendMessage( "You decide not to clear your kills." );
					user.KMUsed = 1;
					break;
				}
			}

		}
	}
} //end of BountyGump
