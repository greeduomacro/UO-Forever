using System;
using Server;

namespace Server.Commands
{
	public class SingleClickCommand
	{
		public static void Initialize()
		{
			CommandSystem.Register( "singleclick", AccessLevel.Player, new CommandEventHandler( SingleClick_OnCommand ) );
			CommandSystem.Register( "sclick", AccessLevel.Player, new CommandEventHandler( SingleClick_OnCommand ) );
		}

		public static void SingleClick_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;

			Type type = ScriptCompiler.FindTypeByName( e.GetString( 0 ).Trim().ToLower(), true );

			if ( type != null && typeof( Item ).IsAssignableFrom( type ) )
			{
				Item item = from.Backpack.FindItemByType( type, true );

				if ( item != null && !item.Deleted && from.CanSee( item ) && Utility.InUpdateRange( from.Location, item.GetWorldLocation() ) && from.Region.OnSingleClick( from, item ) )
				{
					if ( item.Parent is Item )
						((Item)item.Parent).OnSingleClickContained( from, item );

					item.OnSingleClick( from );
				}
			}
			else
				from.SendMessage( "That item type does not exist." );
		}
	}
}