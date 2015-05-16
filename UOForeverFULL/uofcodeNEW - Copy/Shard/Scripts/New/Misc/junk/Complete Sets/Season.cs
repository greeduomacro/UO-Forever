using System;
using Server;
using Server.Network;
using Server.Mobiles;
using Server.Commands;
using System.IO;
using System.Text;
using System.Collections;
using System.Net;
using Server.Accounting;
using Server.Items;
using Server.Menus;
using Server.Menus.Questions;
using Server.Menus.ItemLists;
using Server.Spells;
using Server.Targeting;
using Server.Targets;
using Server.Gumps;

namespace Server.Commands
{
	public class SetSeason
	{
		public static void Initialize()
		{
			CommandSystem.Register("Season", AccessLevel.Administrator, new CommandEventHandler(Season_OnCommand));
		}

		public static void Season_OnCommand(CommandEventArgs e)
		{
			Map map;

			if(e.Length == 1)
			{
				for(int i = 1; i < 5; i++)
				{
					map = Map.AllMaps[i];
					map.Season = (e.GetInt32(0));

					foreach(NetState state in NetState.Instances)
					{
						Mobile m = state.Mobile;
						if(m != null)
						{
							state.Send(SeasonChange.Instantiate(m.GetSeason(), true));
							m.SendEverything();
						}
					}
				}
			}
		}
	}
}
