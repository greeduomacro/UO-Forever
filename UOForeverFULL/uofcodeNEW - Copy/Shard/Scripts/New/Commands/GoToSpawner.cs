using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Commands
{
	public class GoToSpawner
	{
		public static void Initialize()
		{
			CommandSystem.Register( "GoToSpawner", AccessLevel.GameMaster, new CommandEventHandler( GoToSpawner_OnCommand ) );
		}

		[Usage( "GoToSpawner" )]
		[Description( "Teleports you to the mobile or item's spawner." )]
		private static void GoToSpawner_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;

			from.Target = new GoToSpawnerTarget();
		}

		private class GoToSpawnerTarget : Target
		{
			public GoToSpawnerTarget() : base( 15, false, TargetFlags.None )
			{
			}

			protected override void OnTarget( Mobile from, object targ )
			{
				ISpawner spawner = null;

				if ( targ is Item )
					spawner = ((Item)targ).Spawner;
				else if ( targ is Mobile )
					spawner = ((Mobile)targ).Spawner;

				if ( spawner is Spawner )
				{
					Spawner spawnitem = (Spawner)spawner;
					if ( spawnitem.Map == null || spawnitem.Map == Map.Internal )
						from.SendMessage( "That spawner is in the internal map." );
					else if ( spawnitem.RootParent is Mobile )
					{
						from.MoveToWorld( spawnitem.GetWorldLocation(), spawnitem.Map );
						from.SendMessage( "The spawner is on a mobile." );
					}
					else
						from.MoveToWorld( spawnitem.Location, spawnitem.Map );
				}
			}
		}
	}
}