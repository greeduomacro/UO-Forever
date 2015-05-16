using System;
using System.Collections.Generic;
using Server;
using Server.Multis;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;
using Server.Commands;

namespace Server.Commands
{
	public class RemoveDupesCommand
	{
		public static void Initialize()
		{
			CommandSystem.Register( "RemoveDupeDeco", AccessLevel.Owner, new CommandEventHandler( RemoveDupes_OnCommand ) );
		}

		[Usage( "RemoveDupeDeco" )]
		[Description( "Removes duplicate decorations created by unscrupulous staff members." )]
		private static void RemoveDupes_OnCommand(CommandEventArgs e)
		{
			List<Item> m_Deco = new List<Item>();

			foreach ( Item item in World.Items.Values )
				if ( !item.Movable && !(item is Spawner) && !BaseHouse.CheckLockedDownOrSecured( item ) && ( item.RootParent == null || item.RootParent == item ) && item.Map != null && item.Map != Map.Internal )
					m_Deco.Add( item );

			int origc = m_Deco.Count;

			Console.WriteLine( String.Format( "Decoration Removal: {0} items acquired..", m_Deco.Count ) );

			for ( int i = 0;i < m_Deco.Count; i++ )
			{
				Item deco = m_Deco[i];
				int count = 0;

				for ( int j = i+1;j < m_Deco.Count; j++ )
				{
					Item check = m_Deco[j];

					if ( deco.GetType() == check.GetType() && deco.Location == check.Location && deco.Map == check.Map && deco.Hue == check.Hue && deco.ItemID == check.ItemID )
					{
						//if ( !(check is AddonComponent) || ((AddonComponent)check).Addon.GetType() == ((AddonComponent)deco).Addon.GetType() )
						//{
							m_Deco.RemoveAt( j );
							j--;
							count++;
							check.Delete();
						//}
					}
				}

				//if ( count > 0 )
					//Console.WriteLine( String.Format( "Deleted {0} items of {1} ({3}) [{2}]", count, deco.GetType(), i, deco.Location ) );
			}

			Console.WriteLine( String.Format( "Decoration Removal: {0} items deleted.", origc - m_Deco.Count ) );
		}
	}
}