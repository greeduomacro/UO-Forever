using System;
using System.Collections.Generic;
using Server.Network;

namespace Server.Commands
{
	public class GotoRandomStaffCommand
	{
		public static void Initialize()
		{
			CommandSystem.Register( "GRS", AccessLevel.EventMaster, new CommandEventHandler( GRS_OnCommand ) );
		}

		[Usage( "GRS" )]
		[Description( "Go to a random online staff (below your accesslevel)." )]
		private static void GRS_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			List<Mobile> mobiles = new List<Mobile>();

			foreach ( NetState ns in NetState.Instances )
			{
				Mobile m = ns.Mobile;
				if ( m != null && m != from && m.AccessLevel != AccessLevel.Player && from.AccessLevel > m.AccessLevel )
					mobiles.Add( m );
			}

			if ( mobiles.Count == 0 )
				from.SendMessage("No staff online that has a lower accesslevel.");
			else
			{
				Mobile target = mobiles[Utility.Random( mobiles.Count )];
				from.Hidden = true; //hide the staffer
				from.MoveToWorld( target.Location, target.Map );
			}
		}
	}
}