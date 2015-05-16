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

    public class CtfDungeonCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("CTFDungeon", AccessLevel.GameMaster, new CommandEventHandler(Staff_OnCommand));
        }


        public static void Staff_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            Map map = from.Map;

            from.MoveToWorld(new Point3D(5957, 436, 44), Map.Felucca);
            from.SendMessage(68, "Arrived at the Ctf Dungeon");
        }
    }
}
