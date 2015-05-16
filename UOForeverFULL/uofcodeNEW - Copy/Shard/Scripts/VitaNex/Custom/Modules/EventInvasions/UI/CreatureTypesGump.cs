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
	public class CreatureTypesGump : TypeListGump
	{
		public Level Level { get; set; }

        public CreatureTypesGump(PlayerMobile user, Level l, Gump parent = null)
			: base(user, parent, list: l.Creatures, title: "Creatures List", emptyText: "No Creatures to display.")
		{
            Level = l;
		}

		public override List<Type> GetExternalList()
		{
			return Level.Creatures;
		}
	}
}