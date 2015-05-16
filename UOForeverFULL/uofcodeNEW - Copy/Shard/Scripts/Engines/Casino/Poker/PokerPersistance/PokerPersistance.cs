#region References
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Server.Mobiles;
using Server.Network;
using Server.Regions;

using VitaNex.Modules.AutoPvP;
using VitaNex.Notify;
using VitaNex.SuperGumps;
#endregion

namespace Server.Twitch
{
    public static partial class PokerPersistance
	{
		private static readonly FileInfo _PersistenceFile;

		public static int HandID { get; set; }
        public static int ActionID { get; set; }
	}
}