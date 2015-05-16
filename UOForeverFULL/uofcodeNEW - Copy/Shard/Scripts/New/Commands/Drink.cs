using System;
using Server;
using Server.Items;

namespace Server.Commands
{
   public class DrinkCommand
   {
		public static void Initialize()
		{
			CommandSystem.Register( "Drink", AccessLevel.Player, new CommandEventHandler( Drink_OnCommand ) );
		}

		[Usage( "Drink <potion_type>" )]
		[Description( "Drinks a potion if available." )]
		public static void Drink_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;

			Type type = ScriptCompiler.FindTypeByName( e.GetString( 0 ).Trim().ToLower(), true );

			if ( type != null && typeof( BasePotion ).IsAssignableFrom( type ) )
			{
				BasePotion potion = from.Backpack.FindItemByType( type, true ) as BasePotion;

				if ( potion != null )
                    DoubleClickCommand.CommandUseReq(from, potion);
				else
					from.SendMessage( "You do not have any of those potions." );
			}
			else
				from.SendMessage( "That is not a type of potion." );
		}
	}
}