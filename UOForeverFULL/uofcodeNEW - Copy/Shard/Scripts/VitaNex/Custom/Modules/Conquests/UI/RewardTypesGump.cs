#region References
using System;
using System.Collections.Generic;

using Server.Gumps;
using Server.Mobiles;

using VitaNex.SuperGumps.UI;
#endregion

namespace Server.Engines.Conquests
{
	public class RewardTypesGump : TypeListGump
	{
		public Conquest Conquest { get; set; }

		public RewardTypesGump(PlayerMobile user, Conquest c, Gump parent = null)
			: base(user, parent, list: c.Rewards, title: c.Name + " Rewards List", emptyText: "No rewards to display.")
		{
			Conquest = c;
		}

		public override List<Type> GetExternalList()
		{
			return Conquest.Rewards;
		}
	}
}