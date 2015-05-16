#region References

using System;
using System.Linq;
using Server;
using Server.Gumps;
using Server.Network;

#endregion

namespace Knives.TownHouses
{
    public class GumpResponse
    {
        public static void Initialize()
        {
            Timer.DelayCall(TimeSpan.Zero, AfterInit);
        }

        private static void AfterInit()
        {
            PacketHandlers.Register(0xB1, 0, true, DisplayGumpResponse);
        }

        private static void DisplayGumpResponse(NetState state, PacketReader pvSrc)
        {
            int serial = pvSrc.ReadInt32();
            int typeID = pvSrc.ReadInt32();
            int buttonID = pvSrc.ReadInt32();

            var gumps = state.Gumps.ToList();

            for (int i = 0; i < gumps.Count; ++i)
            {
                Gump gump = gumps[i];
                if (gump == null)
                {
                    continue;
                }

                if (gump.Serial != serial || gump.TypeID != typeID)
                {
                    continue;
                }

                int switchCount = pvSrc.ReadInt32();

                if (switchCount < 0)
                {
                    Console.WriteLine("Client: {0}: Invalid gump response, disconnecting...", state);
                    state.Dispose();
                    return;
                }

                var switches = new int[switchCount];

                for (int j = 0; j < switches.Length; ++j)
                {
                    switches[j] = pvSrc.ReadInt32();
                }

                int textCount = pvSrc.ReadInt32();

                if (textCount < 0)
                {
                    Console.WriteLine("Client: {0}: Invalid gump response, disconnecting...", state);
                    state.Dispose();
                    return;
                }

                var textEntries = new TextRelay[textCount];

                for (int j = 0; j < textEntries.Length; ++j)
                {
                    int entryID = pvSrc.ReadUInt16();
                    int textLength = pvSrc.ReadUInt16();

                    if (textLength > 239)
                    {
                        return;
                    }

                    string text = pvSrc.ReadUnicodeStringSafe(textLength);
                    textEntries[j] = new TextRelay(entryID, text);
                }

                state.RemoveGump(i);

                if (!CheckResponse(gump, state.Mobile, buttonID))
                {
                    return;
                }

                gump.OnResponse(state, new RelayInfo(buttonID, switches, textEntries));

                return;
            }

            if (typeID == 461) // Virtue gump
            {
                int switchCount = pvSrc.ReadInt32();

                if (buttonID == 1 && switchCount > 0)
                {
                    Mobile beheld = World.FindMobile(pvSrc.ReadInt32());

                    if (beheld != null)
                    {
                        EventSink.InvokeVirtueGumpRequest(new VirtueGumpRequestEventArgs(state.Mobile, beheld));
                    }
                }
                else
                {
                    Mobile beheld = World.FindMobile(serial);

                    if (beheld != null)
                    {
                        EventSink.InvokeVirtueItemRequest(new VirtueItemRequestEventArgs(state.Mobile, beheld, buttonID));
                    }
                }
            }
        }

        private static bool CheckResponse(Gump gump, Mobile m, int id)
        {
            if (m == null || !m.Player)
            {
                return true;
            }

            var list = m.GetItemsInRange(20).OfType<TownHouse>().Cast<Item>().ToList();

            TownHouse th = list.Cast<TownHouse>().FirstOrDefault(t => t != null && t.Owner == m);

            if (th == null || th.ForSaleSign == null)
            {
                return true;
            }

            if (gump is HouseGumpAOS)
            {
                int val = id - 1;

                if (val < 0)
                {
                    return true;
                }

                int type = val%15;
                int index = val/15;

                if (th.ForSaleSign.ForcePublic && type == 3 && index == 12 && th.Public)
                {
                    m.SendMessage("This house cannot be private.");
                    m.SendGump(gump);
                    return false;
                }

                if (th.ForSaleSign.ForcePrivate && type == 3 && index == 13 && !th.Public)
                {
                    m.SendMessage("This house cannot be public.");
                    m.SendGump(gump);
                    return false;
                }

                if (!th.ForSaleSign.NoTrade || type != 6 || index != 1)
                {
                    return true;
                }
                m.SendMessage("This house cannot be traded.");
                m.SendGump(gump);
                return false;
            }

            if (!(gump is HouseGump))
            {
                return true;
            }

            if (th.ForSaleSign.ForcePublic && id == 17 && th.Public)
            {
                m.SendMessage("This house cannot be private.");
                m.SendGump(gump);
                return false;
            }

            if (th.ForSaleSign.ForcePrivate && id == 17 && !th.Public)
            {
                m.SendMessage("This house cannot be public.");
                m.SendGump(gump);
                return false;
            }

            if (!th.ForSaleSign.NoTrade || id != 14)
            {
                return true;
            }
            m.SendMessage("This house cannot be traded.");
            m.SendGump(gump);
            return false;
        }
    }
}