#region References
using System;
using System.Collections.Generic;
using Server.Engines.EventInvasions;
using Server.Gumps;
using Server.Mobiles;

using VitaNex.SuperGumps.UI;
#endregion

namespace Server.Engines.EventInvasions
{
	public class ItemTypesGump : TypeListGump
	{
		public Level Level { get; set; }

        public ItemTypesGump(PlayerMobile user, Level l, Gump parent = null)
			: base(user, parent, list: l.RewardItems, title: "Regular Mob Drops List", emptyText: "No Items to display.")
		{
            Level = l;
		}

		public override List<Type> GetExternalList()
		{
            return Level.RewardItems;
		}
	}
}