using System;
using System.Collections.Generic;
using Server;
using Server.Items;

namespace Server.Commands
{
    public class BlowPouch
    {
        public static void Initialize()
        {
            CommandSystem.Register("BlowPouch", AccessLevel.Player, new CommandEventHandler(BlowPouch_OnCommand));
            CommandSystem.Register("PopPouch", AccessLevel.Player, new CommandEventHandler(BlowPouch_OnCommand));
            CommandSystem.Register("Pouch", AccessLevel.Player, new CommandEventHandler(BlowPouch_OnCommand));
        }

        [Usage("BlowPouch")]
        [Description("Blows a trapped pouch if any available.")]
        public static void BlowPouch_OnCommand(CommandEventArgs e)
        {
            bool nopouch = true;
            Mobile from = e.Mobile;

            if (from.Backpack != null)
            {
                List<Pouch> pouches = from.Backpack.FindItemsByType<Pouch>();

                for (int i = 0; nopouch && i < pouches.Count; ++i)
                {
                    Pouch pouch = pouches[i];

                    if (pouch.TrapType == TrapType.MagicTrap)
                    {
                        DoubleClickCommand.CommandUseReq(from, pouch);
                        nopouch = false;
                    }
                }
                List<TPouch> pouches2 = from.Backpack.FindItemsByType<TPouch>();

                for (int i = 0; nopouch && i < pouches2.Count; ++i)
                {
                    TPouch pouch2 = pouches2[i];

                    if (pouch2.TrapType == TrapType.MagicTrap)
                    {
                        DoubleClickCommand.CommandUseReq(from, pouch2);
                        nopouch = false;
                    }
                }

                List<ChargeableTrapPouch> pouches3 = from.Backpack.FindItemsByType<ChargeableTrapPouch>();

                for (int i = 0; nopouch && i < pouches3.Count; ++i)
                {
                    ChargeableTrapPouch pouch3 = pouches3[i];

                    if (pouch3.TrapType == TrapType.MagicTrap)
                    {
                        DoubleClickCommand.CommandUseReq(from, pouch3);
                        nopouch = false;
                    }
                }

                if (nopouch)
                    from.SendMessage("You do not have any magically trapped pouches.");
            }
        }
    }
}
