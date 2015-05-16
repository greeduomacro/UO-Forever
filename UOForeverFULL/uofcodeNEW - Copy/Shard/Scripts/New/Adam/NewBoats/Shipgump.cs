using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Network;
using Server.Prompts;
using Server.Multis;
using Server.Multis.Deeds;
using Server.Items;

namespace Server.Gumps
{
    public class BoatListGump : Gump
    {
        private BaseShip m_Ship;

        public BoatListGump(List<Mobile> list, BaseShip ship, bool accountOf) : base(20, 30)
        {
            if (ship.Deleted)
                return;

            m_Ship = ship;

            AddPage(0);

            AddBackground(0, 0, 420, 430, 5054);
            AddBackground(10, 10, 400, 410, 3000);

            AddButton(20, 388, 4005, 4007, 0, GumpButtonType.Reply, 0);
            AddHtmlLocalized(55, 388, 300, 20, 1011104, false, false); // Return to previous menu

            AddLabel(20, 20, 0, "Officers of the Ship");

            if (list != null)
            {
                for (int i = 0; i < list.Count; ++i)
                {
                    if ((i % 16) == 0)
                    {
                        if (i != 0)
                        {
                            // Next button
                            AddButton(370, 20, 4005, 4007, 0, GumpButtonType.Page, (i / 16) + 1);
                        }

                        AddPage((i / 16) + 1);

                        if (i != 0)
                        {
                            // Previous button
                            AddButton(340, 20, 4014, 4016, 0, GumpButtonType.Page, i / 16);
                        }
                    }

                    Mobile m = list[i];

                    string name;

                    if (m == null || (name = m.Name) == null || (name = name.Trim()).Length <= 0)
                        continue;

                    AddLabel(55, 55 + ((i % 16) * 20), 0,
                        accountOf && m.Player && m.Account != null ? String.Format("Account of {0}", name) : name);
                }
            }
        }
    }

    public class BoatGump : Gump
    {
        private BaseShip m_ship;

        private List<string> Wrap(string value)
        {
            if (value == null || (value = value.Trim()).Length <= 0)
                return null;

            string[] values = value.Split(' ');
            List<string> list = new List<string>();
            string current = "";

            for (int i = 0; i < values.Length; ++i)
            {
                string val = values[i];

                string v = current.Length == 0 ? val : current + ' ' + val;

                if (v.Length < 10)
                {
                    current = v;
                }
                else if (v.Length == 10)
                {
                    list.Add(v);

                    if (list.Count == 6)
                        return list;

                    current = "";
                }
                else if (val.Length <= 10)
                {
                    list.Add(current);

                    if (list.Count == 6)
                        return list;

                    current = val;
                }
                else
                {
                    while (v.Length >= 10)
                    {
                        list.Add(v.Substring(0, 10));

                        if (list.Count == 6)
                            return list;

                        v = v.Substring(10);
                    }

                    current = v;
                }
            }

            if (current.Length > 0)
                list.Add(current);

            return list;
        }

        public BoatGump(Mobile from, BaseShip ship)
            : base(20, 30)
        {
            if (ship.Deleted)
                return;

            m_ship = ship;

            from.CloseGump(typeof(HouseGump));
            from.CloseGump(typeof(HouseListGump));
            from.CloseGump(typeof(HouseRemoveGump));

            bool isOwner = m_ship.Owner == from;

            AddPage(0);


            AddBackground(0, 0, 420, 430, 5054);
            AddBackground(10, 10, 400, 410, 3000);

            AddImage(130, 0, 100);

            List<string> lines = Wrap(m_ship.ShipName);

            if (lines != null)
            {
                for (int i = 0, y = (101 - (lines.Count * 14)) / 2; i < lines.Count; ++i, y += 14)
                {
                    string s = lines[i];

                    AddLabel(130 + ((143 - (s.Length * 8)) / 2), y, 0, s);
                }
            }

            AddLabel(55, 103, 0, "Ship Info"); // INFO
            AddButton(20, 103, 4005, 4007, 0, GumpButtonType.Page, 1);

            AddLabel(170, 103, 0, "Ownership"); // FRIENDS
            AddButton(135, 103, 4005, 4007, 0, GumpButtonType.Page, 2);

            AddLabel(295, 103, 0, "Options"); // OPTIONS
            AddButton(260, 103, 4005, 4007, 0, GumpButtonType.Page, 3);

            AddLabel(55, 390, 0, "Dock Vessel"); //dock
            AddButton(20, 390, 4005, 4007, 1, GumpButtonType.Reply, 0);

            AddLabel(200, 390, 0, "Rename Vessel"); //rename
            AddButton(165, 390, 4005, 4007, 2, GumpButtonType.Reply, 0);

            AddLabel(355, 390, 0, "Exit"); // EXIT
            AddButton(320, 390, 4005, 4007, 0, GumpButtonType.Reply, 0);

            // Info page
            AddPage(1);

            AddHtmlLocalized(150, 135, 100, 20, 1011242, false, false); // Owned by:
            AddHtml(220, 135, 100, 20, m_ship.GetOwnerName(), false, false);

            AddLabel(20, 250, 0, "Ship's Status: ");

            if (m_ship.Status == ShipStatus.Full)
            {
                AddLabel(110, 250, 1372-1, "Maximum Strength");
            }
            else if (m_ship.Status == ShipStatus.Half)
            {
                AddLabel(110, 250, 1357-1, "Half Strength");
            }
            else if (m_ship.Status == ShipStatus.Low)
            {
                AddLabel(110, 250, 133-1, "BOARDABLE!");
            }

            AddLabel(20, 275, 0, "Ship's Hull: ");

            AddLabel(110, 275, 0, m_ship.HullDurability + "/" + m_ship.MaxHullDurability);

            var percentage = (int)Math.Round(((double)m_ship.HullDurability / m_ship.MaxHullDurability) * 100);

            AddLabel(180, 275, 0, "(" + percentage + "%)");

            AddLabel(20, 300, 0, "Ship's Sails: ");

            AddLabel(110, 300, 0, m_ship.SailDurability + "/" + m_ship.MaxSailDurability);

            percentage = (int)Math.Round(((double)m_ship.SailDurability / m_ship.MaxSailDurability) * 100);

            AddLabel(180, 300, 0, "(" + percentage + "%)");

            AddLabel(55, 325, 0, "Embark");
            AddButton(20, 325, 4005, 4007, 3, GumpButtonType.Page, 1);

            AddLabel(255, 325, 0, "Disembark");
            AddButton(220, 325, 4005, 4007, 4, GumpButtonType.Page, 1);

            AddLabel(55, 350, 0, "Embark All Followers");
            AddButton(20, 350, 4005, 4007, 5, GumpButtonType.Page, 1);

            AddLabel(255, 350, 0, "Disembark All Followers");
            AddButton(220, 350, 4005, 4007, 6, GumpButtonType.Page, 1);



            // Friends page
            AddPage(2);

            AddLabel(45, 130, 0, "View Officers"); // List of captains
            AddButton(20, 130, 2714, 2715, 7, GumpButtonType.Reply, 0);

            AddLabel(45, 150, 0, "Add an Officer"); // Add a captains
            AddButton(20, 150, 2714, 2715, 8, GumpButtonType.Reply, 0);

            AddLabel(45, 170, 0, "Remove an Officer"); // Remove a captains
            AddButton(20, 170, 2714, 2715, 9, GumpButtonType.Reply, 0);

            AddLabel(45, 190, 0, "Clear all Officers"); // Clear captains list
            AddButton(20, 190, 2714, 2715, 10, GumpButtonType.Reply, 0);

            AddLabel(225, 130, 0, "View Deckhands"); // List of Deckhands
            AddButton(200, 130, 2714, 2715, 11, GumpButtonType.Reply, 0);

            AddLabel(225, 150, 0, "Add a Deckhand"); // Add a deckhand
            AddButton(200, 150, 2714, 2715, 12, GumpButtonType.Reply, 0);

            AddLabel(225, 170, 0, "Remove a Deckhand"); // Remove a deckhand
            AddButton(200, 170, 2714, 2715, 13, GumpButtonType.Reply, 0);

            AddLabel(225, 190, 0, "Clear all Deckhands"); // Clear deckhands list
            AddButton(200, 190, 2714, 2715, 14, GumpButtonType.Reply, 0);

            AddPage(3);

            AddLabel(45, 130, 0, "Scuttle Ship"); // List of captains
            AddButton(20, 130, 2714, 2715, 15, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            bool isOwner = m_ship.Owner == sender.Mobile;

            if (m_ship.Deleted)
                return;

            Mobile from = sender.Mobile;

            if (m_ship == null || from.Map != m_ship.Map || !from.InRange(m_ship.GetWorldLocation(), 18))
                return;

            switch (info.ButtonID)
            {
                case 1: // Dock boat
                {
                    if (m_ship is BaseGalleon)
                    {
                        var galleon = m_ship as BaseGalleon;
                        galleon.BeginDryDock(from);
                    }
                        break;
                    }
                case 2: // Rename boat
                    {
                        from.Prompt = new ShipRenamePrompt(m_ship);
                        from.SendLocalizedMessage(501302); // What dost thou wish the sign to say?

                        break;
                    }
                case 7: // list officers
                {
                    if (m_ship.PlayerAccess != null)
                    {
                        List<Mobile> list = m_ship.PlayerAccess.Where(x => x.Value == 3).Select(item => item.Key as Mobile).ToList();
                        from.CloseGump(typeof(BoatGump));
                        from.CloseGump(typeof(BoatListGump));
                        from.SendGump(new BoatListGump(list, m_ship, false));
                    }

                    break;
                }
                case 8: // add officer
                {
                    if (isOwner)
                    {
                        from.SendMessage(61, "Target the individual you would like to make an officer of your ship.");
                        from.Target = new BaseShip.OfficerTarget(true, m_ship);
                    }
                    else
                    {
                        from.SendMessage(61, "You do not have access to this function.");
                    }

                    break;
                }
                case 9: // remove officer
                {
                    if (isOwner)
                    {
                        from.SendMessage(61, "Target the individual you would like to remove as an officer of your ship.");
                        from.Target = new BaseShip.OfficerTarget(false, m_ship);
                    }
                    else
                    {
                        from.SendMessage(61, "You do not have access to this function.");
                    }

                    break;
                }
                case 10: // clear officers
                {
                    if (isOwner)
                    {
                        if (m_ship.PlayerAccess != null)
                        {
                            m_ship.PlayerAccess.RemoveValueRange(x => x == 3);
                        }
                    }
                    else
                    {
                        from.SendMessage(61, "You do not have access to this function.");
                    }

                    break;
                }
                case 15: // scuttle boat
                {
                    from.SendMessage(61, "You have scuttled the " + m_ship.ShipName + ".");
                    m_ship.HullDurability = 0;

                    break;
                }
            }
        }
    }
}

namespace Server.Prompts
{
    public class ShipRenamePrompt : Prompt
    {
        private BaseShip m_ship;

        public ShipRenamePrompt(BaseShip ship)
        {
            m_ship = ship;
        }

        public override void OnResponse(Mobile from, string text)
        {
            m_ship.ShipName = text;

            from.SendMessage("Vessel name changed.");
        }
    }
}