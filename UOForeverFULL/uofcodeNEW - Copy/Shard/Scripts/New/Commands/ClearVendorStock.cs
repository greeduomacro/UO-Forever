using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Commands
{
	public class ClearVendorCommand
	{
		public static void Initialize()
		{
			CommandSystem.Register( "ClearVendorStock", AccessLevel.Administrator, new CommandEventHandler( ClearContainer_OnCommand ) );
		}

		[Usage( "ClearVendorStock" )]
		[Description( "Clears vendors with excess buypack items." )]
		public static void ClearContainer_OnCommand( CommandEventArgs e )
		{
			foreach ( Mobile m in World.Mobiles.Values )
			{
				if ( m is BaseVendor )
				{
					BaseVendor vendor = m as BaseVendor;
					Container buypack = vendor.BuyPack;
					if ( buypack != null && buypack.Items.Count > 0 )
					{
						List<Item> newitemslist = new List<Item>( buypack.Items );
						buypack.Items.Clear();

						for ( int i = newitemslist.Count - 1;i >= 0; i-- )
						{
							Item item = newitemslist[i] as Item;
							if ( item != null )
								item.Delete();
						}

						buypack.UpdateTotals();
					}
				}
			}
		}
   }
}