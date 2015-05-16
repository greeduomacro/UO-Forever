using System;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using Server;
using Server.Items;
using Server.Gumps;
using Server.Spells;
using Server.Multis;
using Server.Mobiles;
using Server.Network;
using Server.Accounting;

namespace Server.Commands
{

    public class CTFHouseCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("CTFHouse", AccessLevel.GameMaster, new CommandEventHandler(Staff_OnCommand));
        }


        public static void Staff_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            Map map = from.Map;

            from.MoveToWorld(new Point3D(4425, 1150, 0), Map.Felucca);
            from.SendMessage(68, "Arrived at the Ctf house");
        }
    }
}
