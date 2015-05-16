using System;
using System.Collections.Generic;
using Server;
using Server.Network;
using Server.Mobiles;

namespace Server.Commands
{
	public class TotalRespawn
	{
		public static void Initialize()
		{
			CommandSystem.Register( "TotalRespawn", AccessLevel.Administrator, new CommandEventHandler( TotalRespawn_OnCommand ) );
		}

		[Usage( "TotalRespawn" )]
		[Description( "Completely respawns all spawners in the world." )]
		private static void TotalRespawn_OnCommand( CommandEventArgs e )
		{
			int count = 0;

			List<Spawner> spawners = new List<Spawner>();

			foreach ( Item item in World.Items.Values )
			{
				Spawner spawner = item as Spawner;
				if ( spawner != null && !spawner.Deleted && spawner.Running )
				{
					spawners.Add( spawner );
					count++;
				}
			}

			foreach ( Spawner spawner in spawners )
				spawner.Respawn();

			e.Mobile.SendMessage( "Spawners Respawned: {0}", count );
		}
	}
}