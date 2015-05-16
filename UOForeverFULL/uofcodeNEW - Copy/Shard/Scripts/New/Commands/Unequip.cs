using System;
using Server;
using Server.Network;
using Server.Mobiles;

namespace Server.Commands
{
	public class UnequipCommand
	{
		public static void Initialize()
		{
			CommandSystem.Register( "unequip", AccessLevel.Player, new CommandEventHandler( Unequip_OnCommand ) );
		}

		public static void Unequip_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;

			Layer layer;
			if ( Enum.TryParse<Layer>( e.GetString( 0 ).Trim().ToLower(), true, out layer ) )
			{
				if ( layer > Layer.Invalid && layer <= Layer.LastUserValid && layer != Layer.Backpack )
				{
					Item item = from.FindItemOnLayer( layer );

					if ( item != null && !item.Deleted )
					{
						from.DropHolding();

						bool rejected;
						LRReason reject;

						from.Lift( item, item.Amount, out rejected, out reject );

						if ( !rejected )
						{
							from.Drop( from.Backpack, new Point3D( -1, -1, 0 ) );
							from.Holding = null;
						}
					}
				}
				else
					from.SendMessage( "That layer is not accessible." );
			}
			else
				from.SendMessage( "That layer does not exist." );
		}
	}
}