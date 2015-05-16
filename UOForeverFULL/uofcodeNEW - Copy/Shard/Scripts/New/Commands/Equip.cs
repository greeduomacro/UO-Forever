using System;
using Server;
using Server.Network;
using Server.Mobiles;

namespace Server.Commands
{
	public class EquipCommand
	{
		public static void Initialize()
		{
			CommandSystem.Register( "equip", AccessLevel.Player, new CommandEventHandler( Equip_OnCommand ) );
		}

		public static void Equip_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;

			Type type = ScriptCompiler.FindTypeByName( e.GetString( 0 ).Trim().ToLower(), true );

			if ( type != null && typeof( Item ).IsAssignableFrom( type ) )
			{
				Item item = from.Backpack.FindItemByType( type, true );

				if ( item != null && !item.Deleted )
				{
					from.DropHolding();

					bool rejected;
					LRReason reject;

					from.Lift( item, item.Amount, out rejected, out reject );

					if ( !rejected )
					{
						from.Holding = null;

						if ( !from.EquipItem( item ) )
							item.Bounce( from );
					}

					item.ClearBounce();
				}
			}
			else
				from.SendMessage( "That item type does not exist." );
		}
	}
}