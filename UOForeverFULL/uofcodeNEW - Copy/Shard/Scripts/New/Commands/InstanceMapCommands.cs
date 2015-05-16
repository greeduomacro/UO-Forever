using System;
using System.Collections.Generic;
using Server;
using Server.Regions;

namespace Server.Commands
{
	public class MapCommands
	{
		public static void Initialize()
		{
			CommandSystem.Register( "CreateMap", AccessLevel.Lead, new CommandEventHandler( CreateMap_OnCommand ) );
			CommandSystem.Register( "DeleteMap", AccessLevel.Lead, new CommandEventHandler( DeleteMap_OnCommand ) );
		}

		private static void CreateMap_OnCommand( CommandEventArgs e )
		{
			if ( e.Arguments.Length == 2 )
			{
				try
				{
					Map map = Map.Parse( e.GetString( 0 ) );
                    if (map != null)
                    {
                        string name = e.GetString(1);

                        BaseInstanceMap basemap = new BaseInstanceMap(map, name, MapRules.FeluccaRules);
                    }
                    else
                    {
                        e.Mobile.SendMessage("Usage: [createmap mapToCopy newMapName");
                    }
				}
				catch
				{
                    e.Mobile.SendMessage("Usage: [createmap mapToCopy newMapName");
				}
			}
		}

		private static void DeleteMap_OnCommand( CommandEventArgs e )
		{
			try
			{
				BaseInstanceMap basemap = Map.Parse( e.GetString( 0 ) ) as BaseInstanceMap;

                if (basemap != null)
                {
                    List<Item> items = new List<Item>();
                    List<Mobile> mobiles = new List<Mobile>();

                    foreach (Item item in World.Items.Values)
                        if (item.Map == basemap && item.Parent == null)
                            items.Add(item);

                    for (int i = items.Count - 1; i >= 0; i--)
                        items[i].Delete();

                    foreach (Mobile m in World.Mobiles.Values)
                        if (!m.Player && m.Map == basemap)
                            mobiles.Add(m);

                    for (int i = mobiles.Count - 1; i >= 0; i--)
                        mobiles[i].Delete();

                    basemap.Delete();
                }
                else
                {
                    e.Mobile.SendMessage("Map " + e.GetString(0) + " was not found.  Usage: [createmap mapToDelete");
                }
			}
			catch
			{
                e.Mobile.SendMessage("Usage: [createmap mapToDelete");
			}
		}
	}
}