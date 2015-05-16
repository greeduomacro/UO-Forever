#region References

using Server.Gumps;
using Server.Network;

#endregion

namespace Server.Items
{
    public class ForeverBeardDye : Item
    {
        public override string DefaultName { get { return "Forever Beard Dye"; } }

        [Constructable]
        public ForeverBeardDye()
            : base(3627)
        {
            Weight = 1.0;
        }

        public ForeverBeardDye(Serial serial)
            : base(serial)
        { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(GetWorldLocation(), 1))
            {
                from.CloseGump(typeof(ForeverBeardDyeGump));
                from.SendGump(new ForeverBeardDyeGump(this));
            }
            else
            {
                from.LocalOverheadMessage(MessageType.Regular, 906, 1019045); // I can't reach that.
            }
        }
    }

    public class ForeverBeardDyeGump : Gump
    {
        private ForeverBeardDye m_ForeverBeardDye;

        private class ForeverBeardDyeEntry
        {
            private string m_Name;
            private int m_HueStart;
            private int m_HueCount;

            public string Name { get { return m_Name; } }

            public int HueStart { get { return m_HueStart; } }

            public int HueCount { get { return m_HueCount; } }

            public ForeverBeardDyeEntry(string name, int hueStart, int hueCount)
            {
                m_Name = name;
                m_HueStart = hueStart;
                m_HueCount = hueCount;
            }
        }

        private static ForeverBeardDyeEntry[] m_Entries =
        {
            new ForeverBeardDyeEntry("NeonPink", 2727, 1),
            new ForeverBeardDyeEntry("Fallon", 1266, 1),
            new ForeverBeardDyeEntry("UltraGreen", 1167, 1),
            new ForeverBeardDyeEntry("NeonGreen", 2966, 1),
            new ForeverBeardDyeEntry("NeonOrange", 1258, 1),
            new ForeverBeardDyeEntry("DeepViolet", 1363, 1),
            new ForeverBeardDyeEntry("Vesper", 1159, 1),
            new ForeverBeardDyeEntry("Shadow", 1175, 1)
        };

        public ForeverBeardDyeGump(ForeverBeardDye dye)
            : base(0, 0)
        {
            m_ForeverBeardDye = dye;

            AddPage(0);
            AddBackground(150, 60, 350, 358, 2600);
            AddBackground(170, 104, 110, 270, 5100);
            AddHtmlLocalized(230, 75, 200, 20, 1013006, false, false); // Hair Color Selection Menu
            AddHtmlLocalized(235, 380, 300, 20, 1013007, false, false); // Dye my hair this color!
            AddButton(200, 380, 0xFA5, 0xFA7, 1, GumpButtonType.Reply, 0); // DYE HAIR

            for (int i = 0; i < m_Entries.Length; ++i)
            {
                AddLabel(180, 109 + (i * 22), m_Entries[i].HueStart - 1, m_Entries[i].Name);
                AddButton(257, 110 + (i * 22), 5224, 5224, 0, GumpButtonType.Page, i + 1);
            }

            for (int i = 0; i < m_Entries.Length; ++i)
            {
                ForeverBeardDyeEntry e = m_Entries[i];

                AddPage(i + 1);

                for (int j = 0; j < e.HueCount; ++j)
                {
                    AddLabel(328 + ((j / 16) * 80), 102 + ((j % 16) * 17), e.HueStart + j - 1, "*****");
                    AddRadio(310 + ((j / 16) * 80), 102 + ((j % 16) * 17), 210, 211, false, (i * 100) + j);
                }
            }
        }

        public override void OnResponse(NetState from, RelayInfo info)
        {
            if (m_ForeverBeardDye.Deleted)
            {
                return;
            }

            Mobile m = from.Mobile;
            int[] switches = info.Switches;

            if (!m_ForeverBeardDye.IsChildOf(m.Backpack))
            {
                m.SendLocalizedMessage(1042010); //You must have the object in your backpack to use it.
                return;
            }

            if (info.ButtonID != 0 && switches.Length > 0)
            {
                if (m.FacialHairItemID == 0)
                {
                    m.SendLocalizedMessage(502619); // You have no hair to dye and cannot use this
                }
                else
                {
                    // To prevent this from being exploited, the hue is abstracted into an internal list

                    int entryIndex = switches[0] / 100;
                    int hueOffset = switches[0] % 100;

                    if (entryIndex >= 0 && entryIndex < m_Entries.Length)
                    {
                        ForeverBeardDyeEntry e = m_Entries[entryIndex];

                        if (hueOffset >= 0 && hueOffset < e.HueCount)
                        {
                            m_ForeverBeardDye.Delete();

                            int hue = e.HueStart + hueOffset;

                            m.FacialHairHue = hue;

                            m.SendLocalizedMessage(502618); // You dye your hair
                            m.PlaySound(0x4E);
                        }
                    }
                }
            }
            else
            {
                m.SendLocalizedMessage(501200); // You decide not to dye your hair
            }
        }
    }
}