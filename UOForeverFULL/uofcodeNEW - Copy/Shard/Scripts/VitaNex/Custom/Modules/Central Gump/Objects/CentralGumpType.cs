#region References

using System;
using System.Collections.Generic;
using System.Linq;
using Server.Commands.Generic;
using Server.Items;
using Server.Mobiles.MetaSkills;
using Server.Network;
using VitaNex;
using VitaNex.FX;
using VitaNex.Network;

#endregion

namespace Server.Engines.CentralGump
{
    public enum CentralGumpType
    {
        News = 0,
        Information,
        Events,
        Links,
        Options,
        Profile,
        Young,
        Commands
    }
}