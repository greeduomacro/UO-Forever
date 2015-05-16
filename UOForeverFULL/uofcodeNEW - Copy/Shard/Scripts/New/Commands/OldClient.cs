using System;
using Server;
using Server.Accounting;
using Server.Network;
using Server.Mobiles;

namespace Server.Commands
{
	public class OldBodiesCommand
	{
		public static void Initialize()
		{
			// from Alan: WHAT IS THIS GARBAGE???????????? I'm commenting it out.
            //CommandSystem.Register( "OldClient", AccessLevel.Player, new CommandEventHandler( OldClient_OnCommand ) );
		}
        /*

		[Usage( "OldClient [<On/Off>]" )]
		[Description( "Switches old client packets on or off." )]
		public static void OldClient_OnCommand( CommandEventArgs e )
		{
			Mobile m = e.Mobile;

			if ( m.Player && m.Account != null )
			{
				IAccount acct = m.Account;
				if ( e.Arguments.Length == 0 )
					m.SendMessage( "Old style client packets is currently {0}.", acct.SFO == FeatureFlags.None ? "OFF" : "ON" );
				else
				{
					bool turnon = e.Arguments[0].ToLower() == "on";
					acct.SFO = turnon ? (FeatureFlags)0x03 : (FeatureFlags)0x0;
					m.SendMessage( "Old style client packets has been turned {0}.", turnon ? "ON" : "OFF" );
					m.Send( new MobileIncoming( m, m ) );
					m.Send( SupportedFeatures.Instantiate( m.NetState ) );
					m.Send( new MobileUpdate( m ) );
					m.Send( new MobileAttributes( m ) );
				}
			}
		}
         */
	}
}