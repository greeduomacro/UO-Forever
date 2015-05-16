using System;
using System.Collections.Generic;
using Server.Network;

namespace Server.Commands
{
	public class GotoRandomPlayerCommand
	{
		public static void Initialize()
		{
			CommandSystem.Register( "GRP", AccessLevel.Seer, new CommandEventHandler( GRP_OnCommand ) );
		}

		[Usage( "GRP" )]
		[Description( "Go to a random online player." )]
		private static void GRP_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			List<Mobile> mobiles = new List<Mobile>();

			foreach ( NetState ns in NetState.Instances )
			{
				Mobile m = ns.Mobile;
				if ( m != null && m.AccessLevel == AccessLevel.Player )
					mobiles.Add( m );
			}

			if ( mobiles.Count == 0 )
				from.SendMessage("No players online.");
			else
			{
				Mobile target = mobiles[Utility.Random( mobiles.Count )];
				from.Hidden = true; //hide the staffer
				from.MoveToWorld( target.Location, target.Map );
			}
		}
	}
}