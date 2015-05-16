using System;
using Server;
using Server.Gumps;
using Server.Guilds;
using Server.Mobiles;
using Server.Network;
using System.Collections;
using System.Collections.Generic;

namespace Server.Engines.MurderSystem
{
    public class MurderBoardGump : Gump
    {
        private List<Mobile> m_List;
        private int m_ListPage;
        private int m_check;
        private List<Mobile> m_CountList;

        public MurderBoardGump(Mobile from, int listPage, List<Mobile> list, List<Mobile> count, int check)
            : base(50, 50)
        {
            m_List = list;
            m_ListPage = listPage;
            m_CountList = count;
            m_check = check;


            Closable = false;
            Disposable = false;
            Dragable = true;
            Resizable = false;
            AddPage(0);
            AddBackground(16, 12,355, 303, 5120);
            AddImageTiled(26, 21, 340, 26, 5154);
            AddAlphaRegion(26, 21, 340, 26);
            AddButton(28, 50, 4005, 4006, 1, GumpButtonType.Reply, 0);
            AddButton(202, 50, 4005, 4006, 2, GumpButtonType.Reply, 0);
            AddImageTiled(20, 80, 347, 10, 5121);
            AddLabel(61, 50, 1149, @"Sort by Name");
            AddLabel(236, 49, 1149, @"Sort by Total Kills");
            AddImageTiled(20, 280, 347, 10, 5121);
            AddImageTiled(26, 100, 199, 23, 5174);
            AddImageTiled(232, 100, 42, 23, 5174);
            AddImageTiled(281, 100, 81, 23, 5174);
            AddImageTiled(26, 130, 199, 134, 5174);
            AddImageTiled(232, 130, 42, 134, 5174);
            AddImageTiled(281, 130, 81, 134, 5174);
            AddAlphaRegion(26, 100, 199, 23);
            AddAlphaRegion(232, 100, 42, 23);;
            AddAlphaRegion(281, 100, 81, 23);
            AddLabel(30, 101, 1149, @"Name");
            AddLabel(236, 101, 1149, @"Kills");
            AddLabel(285, 101, 1149, @"Guild Abbr");
            AddAlphaRegion(26, 130, 199, 134);
            AddAlphaRegion(232, 130, 42, 134);
            AddAlphaRegion(281, 130, 81, 134);

            AddButton(284, 285, 4017, 4018, 7, GumpButtonType.Reply, 0);
            AddLabel(319, 286, 1149, @"Close");

            if (m_List == null)
            {
                List<Mobile> a = new List<Mobile>();

                foreach (Mobile person in World.Mobiles.Values)
                {
                    if (person is PlayerMobile)
                    {
                        PlayerMobile gather = (PlayerMobile)person;
                        a.Add(gather);
                    }
                }
                m_List = a;
            }
            if (m_check == 0)
            {
                m_List.Sort(new NameComparer());
            }
            else
                m_List.Sort(new KillComparer());

            if (listPage > 0)
            {
                AddButton(26, 285, 4014, 4015, 9, GumpButtonType.Reply, 0);
                AddLabel(60, 286, 1149, @"Last Page");
            }

            if ((listPage + 1) * 6 < m_List.Count)
            {
                AddButton(146, 285, 4005, 4006, 10, GumpButtonType.Reply, 0);
                AddLabel(179, 286, 1149, @"Next Page");
            }

            AddLabel(30, 25, 32, "Murderer Board");
			
      		int k = 0;

            for (int i = 0, j = 0, index=((listPage*6)+k) ; i < 6 && index < m_List.Count && j >= 0; ++j, i++, ++index)
            {
                Mobile mob = m_List[index] as Mobile;
                if (mob is PlayerMobile)
                {
                    PlayerMobile m = (PlayerMobile)mob;

                    int offset = 138 + (i * 20);

                    if (m.AccessLevel != AccessLevel.Player)
                      --i;
                    else
                    {
                        AddLabel(30, offset, 1149, m.Name.ToString());
                        AddLabel(236, offset, 1149, m.Kills.ToString());
                        Guild g = m.Guild as Guild;

                        if (g != null)
                        {
                           string abb;

                           abb = "[" + g.Abbreviation + "]";

                           AddLabel(285, offset, 1149, abb);
                         }
                         else
                            AddLabel(285, offset, 1149, @"No Guild");
                     }
                }
            }
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile from = state.Mobile;

            if (from == null)
                return;
            if (info.ButtonID == 1) // Sort by Name
            {
                m_check = 0;
                from.SendGump(new MurderBoardGump(from, m_ListPage, m_List, m_CountList, m_check));
            }
            if (info.ButtonID == 2) // Sort by Kills
            {
                m_check = 1;
                from.SendGump(new MurderBoardGump(from, m_ListPage, m_List, m_CountList, m_check));
            }

            if (info.ButtonID == 9) // Previous page
            {
                if (m_ListPage > 0)
                    from.SendGump(new MurderBoardGump(from, m_ListPage - 1, m_List, m_CountList, m_check));
            }

            if (info.ButtonID == 10) // Next page
            {
                if ((m_ListPage + 1) * 6 < m_List.Count)
                    from.SendGump(new MurderBoardGump(from, m_ListPage + 1, m_List, m_CountList, m_check));
            }
        }
    }

    public class KillComparer : IComparer<Mobile>
    {
        public int Compare(Mobile m1, Mobile m2)
        {
            return m1.Kills.CompareTo(m2.Kills)*-1;
        }
    }

    public class NameComparer : IComparer<Mobile>
    {
        public int Compare(Mobile m1, Mobile m2)
        {
			int res = 0;

			if (m1.CompareNull(m2, ref res))
			{
				return res;
			}

			return Insensitive.Compare(m1.Name, m2.Name);
        }
    }

}