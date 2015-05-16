using System;
using Server;
using Server.Engines.XmlSpawner2;
using System.Collections;

namespace Server.Commands
{
	public class DoubleClickCommand
	{
		public static void Initialize()
		{
			CommandSystem.Register( "doubleclick", AccessLevel.Player, new CommandEventHandler( DoubleClick_OnCommand ) );
			CommandSystem.Register( "dclick", AccessLevel.Player, new CommandEventHandler( DoubleClick_OnCommand ) );
		}

        public static void CommandUseReq(Mobile from, Item item)
        {
            if (from == null || item == null || item.Deleted) return;
            
            if (from.AccessLevel >= AccessLevel.GameMaster || DateTime.UtcNow >= from.NextActionTime)
            {
                bool blockdefaultonuse = false;


                blockdefaultonuse = (XmlScript.HasTrigger(from, TriggerName.onUse) && UberScriptTriggers.Trigger(from, from, TriggerName.onUse, item))
                    || (XmlScript.HasTrigger(item, TriggerName.onUse) && UberScriptTriggers.Trigger(item, from, TriggerName.onUse));
                
                // need to check the item again in case it was modified in the OnUse or OnUser method
                if (!blockdefaultonuse && item != null && !item.Deleted)
                    from.Use(item);
                
                from.NextActionTime = DateTime.UtcNow + Mobile.ServerWideObjectDelay;
            }
            else
            {
                from.SendActionMessage();
            }
        }

		public static void DoubleClick_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;

			Type type = ScriptCompiler.FindTypeByName( e.GetString( 0 ).Trim().ToLower(), true );

			if ( type != null && typeof( Item ).IsAssignableFrom( type ) )
			{
				if ( from.AccessLevel >= AccessLevel.GameMaster || DateTime.UtcNow >= from.NextActionTime )
				{
					Item item = from.Backpack.FindItemByType( type, true );

					DoubleClickCommand.CommandUseReq(from, item );
				}
				else
					from.SendActionMessage();
			}
			else
				from.SendMessage( "That item type does not exist." );
		}
	}
}