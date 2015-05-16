// **********
// UOForever - DaviesLockerGump.cs
// **********

#region References

using System;
using Server.Items;
using Server.Multis;
using Server.Network;
using Server.Targeting;

#endregion

namespace Server.Gumps
{
    public class DaviesLockerGump : Gump
    {
        private DaviesLocker m_Table;

        public DaviesLocker Table
        {
            get { return m_Table; }
        }

        public int GetMapHue(Map map)
        {
            if (map == Map.Trammel)
                return 10;
            if (map == Map.Felucca)
                return 81;
            if (map == Map.Ilshenar)
                return 1102;
            if (map == Map.Malas)
                return 1102;

            return 0;
        }


        public string GetName(string name)
        {
            if (name == null || (name = name.Trim()).Length <= 0)
                return "(indescript)";

            return name;
        }

        private void AddBackground()
        {
            AddPage(0);

            AddImage(90, 95, 1473); // Background Image

            AddLabel(99, 100, 2041, @"Davies Locker"); // Davies Locker Label
            AddLabel(130, 135, 2041, @"Get"); // Get Label
            AddLabel(195, 135, 2041, @"Facet"); // Facet Label
            AddLabel(320, 135, 2041, @"Level"); // Level Label
            AddLabel(460, 135, 2041, @"Coords"); // Coords Label
            AddLabel(570, 135, 2041, @"Status"); //Status Label
        }


        private void AddDetails(int index, int count)
        {
            int btn = (index*2) + 1;


            if (index < m_Table.Entries.Count)
            {
                DaviesLockerEntry e = (DaviesLockerEntry) m_Table.Entries[index];
                int hue = GetMapHue(e.Map);

                AddLabel(110, 520, 2041, String.Format("Maps: {0} of 500", m_Table.Entries.Count)); // # of Maps

                AddButton(133, 170 + (count*35), 2117, 2118, btn, GumpButtonType.Reply, 0); // Get Button

                AddLabel(174, 167 + (count*35), hue, String.Format("{0}", e.Map)); // Facet

                if (e.type == 3)
                {
                    AddLabel(290, 167 + (count*35), hue, String.Format("M-I-B")); //Level - MIB 
                    AddLabel(417, 167 + (count*35), hue, String.Format("Unknown")); //Coords - MIB
                    AddLabel(550, 167 + (count*35), hue, String.Format("Unopened")); //Status - MIB
                }
                else if (e.type == 2)
                {
                    AddLabel(290, 167 + (count*35), hue, String.Format("S-O-S")); //Level - SOS (not shown on SOS)
                    AddLabel(417, 167 + (count*35), hue, String.Format("{0}", e.Location3d)); //Coords - SOS
                    AddLabel(550, 167 + (count*35), hue, String.Format("Opened")); //Status - SOS
                }
                else
                {
                    string leveldesc;
                    if (e.Level == 1)
                        leveldesc = "Plainly";
                    else if (e.Level == 2)
                        leveldesc = "Expertly";
                    else if (e.Level == 3)
                        leveldesc = "Adeptly";
                    else if (e.Level == 4)
                        leveldesc = "Cleverly";
                    else if (e.Level == 5)
                        leveldesc = "Deviously";
                    else if (e.Level == 6)
                        leveldesc = "Ingeniously";
                    else if (e.Level == 7)
                        leveldesc = "Diabolically";
                    else
                        leveldesc = "Unknown";
                    AddLabel(290, 167 + (count*35), hue, String.Format("{0} - {1}", e.Level, leveldesc)); //Level - TMAP

                    if (e.Decoder == null)
                    {
                        AddLabel(417, 167 + (count*35), hue, String.Format("Unknown")); //Coords - TMAP 
                        AddLabel(550, 167 + (count*35), hue, String.Format("Not Decoded")); //Status - TMAP
                    }
                    else
                    {
                        AddLabel(417, 167 + (count*35), hue,
                            e.Mapnumber == "Unknown Spot"
                                ? String.Format("{0}", e.Location2d)
                                : String.Format("{0}", e.Mapnumber));

                        AddLabel(550, 167 + (count*35), hue, String.Format("Decoded ")); //Status - TMAP
                    }
                }
            }
        }

        public DaviesLockerGump(DaviesLocker Table) : base(0, 0)
        {
            m_Table = Table;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddBackground();


            for (int page = 0; page < 500; ++page)
            {
                AddPage(1 + page);

                int pageplus = (m_Table.Entries.Count/10) + 1;
                if (pageplus > 50)
                    pageplus = 50;
                AddLabel(110, 545, 2041, String.Format("Page {0} of {1}", page + 1, pageplus)); // # of Pages

                if (page < 50)
                {
                    AddButton(625, 545, 4005, 4006, 0, GumpButtonType.Page, 2 + page); // next button
                }
                AddLabel(550, 545, 2041, @"PAGE"); // Page Label
                AddButton(500, 545, 4014, 4015, 0, GumpButtonType.Page, page); // prev button


                AddLabel(550, 520, 2041, @"ADD MAPS"); // Add Map Label
                AddButton(500, 520, 4011, 4012, 1000, GumpButtonType.Reply, 0); //Add Map button

                for (int count = 0; count < 10; ++count)
                {
                    AddDetails((page*10) + count, count);
                }
            }
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile from = state.Mobile;

            if (m_Table.Deleted || !from.InRange(m_Table.GetWorldLocation(), 3) || !DesignContext.Check(from))
                return;

            int buttonID = info.ButtonID;

            int index = (buttonID/2);
            int drp = buttonID%2;

            if (index >= 0 && index < m_Table.Entries.Count && drp == 1)
            {
                DaviesLockerEntry e = (DaviesLockerEntry) m_Table.Entries[index];

                if (m_Table.CheckAccess(from))
                {
                    m_Table.DropTMap(from, e, index);
                    from.CloseGump(typeof (DaviesLockerGump));
                    from.SendGump(new DaviesLockerGump(m_Table));
                }
                else
                {
                    from.SendLocalizedMessage(502413); // That cannot be done while the Table is locked down.
                }
            }

            else if (index >= 1 && index < m_Table.Entries.Count + 1 && drp == 0)
            {
                index = index - 1;

                DaviesLockerEntry e = (DaviesLockerEntry) m_Table.Entries[index];
                if (m_Table.CheckAccess(from))
                {
                    from.CloseGump(typeof (DaviesLockerGump));
                    from.SendGump(new DaviesLockerGump(m_Table));
                    m_Table.ViewMap(from, e, index);
                }
                else
                {
                    from.SendLocalizedMessage(502413); // That cannot be done while the Table is locked down.
                }
            }

            else if (buttonID == 1000)
            {
                from.SendMessage("Target a Treasure Map or S.O.S.");
                from.Target = new DaviesLockerTarget(m_Table);
            }
        }

        private class DaviesLockerTarget : Target
        {
            private DaviesLocker m_Table;

            public DaviesLockerTarget(DaviesLocker table) : base(18, false, TargetFlags.None)
            {
                m_Table = table;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Table.Deleted)
                    return;

                if (!from.InRange(m_Table, 5))
                {
                    from.SendMessage("You must be within 5 spaces of the box to use it.");
                    return;
                }

                if (targeted is Item)
                {
                    @from.SendMessage(m_Table.OnDragDrop(@from, targeted as Item)
                        ? "Added, please choose another."
                        : "Invalid item, please try another.");

                    from.Target = new DaviesLockerTarget(m_Table);
                }
            }

            protected override void OnTargetCancel(Mobile from, TargetCancelType cancelType)
            {
                //from.SendGump( new DaviesLockerGump( from, m_Table ) );
            }
        }
    }
}