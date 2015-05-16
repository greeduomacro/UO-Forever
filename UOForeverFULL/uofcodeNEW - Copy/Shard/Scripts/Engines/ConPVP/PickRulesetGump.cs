using System;
using Server.Gumps;
using Server.Network;
using Server.Scripts.Engines.ConPVP;

namespace Server.Engines.ConPVP
{
    public class PickRulesetGump : Gump
    {
        private readonly DuelContext m_Context;
        private readonly Ruleset[] m_Defaults;
        private readonly Ruleset[] m_Flavors;
        private readonly Mobile m_From;
        private readonly Ruleset m_Ruleset;
        private Ruleset[] m_Eras;

        public PickRulesetGump(Mobile from, DuelContext context, Ruleset ruleset) : base(50, 50)
        {
            m_From = from;
            m_Context = context;
            m_Ruleset = ruleset;
            m_Defaults = ruleset.Layout.Defaults;
            m_Flavors = ruleset.Layout.Flavors;
            m_Eras = ruleset.Layout.Eras;

            int height = 25 + 20 + ((m_Defaults.Length + 1) * 22) + 6 + 20 + (m_Flavors.Length * 22) + 125;

            AddPage(0);

            AddBackground(0, 0, 260, height, 9250);
            AddBackground(10, 10, 240, height - 20, 0xDAC);

            int y = 25;

            AddHtml(35, y, 190, 20, Center("Era"), false, false);

            y += 20;

            AddHtml(35 + 14, y, 176, 20, "Ultima Online: Renaissance", false, false);

            if (ruleset.Era == Era.UOR)
            {
                AddImage(35, y + 4, 0x939);
            }
            else
            {
                AddButton(35, y + 4, 0x938, 0x939, 99, GumpButtonType.Reply, 0);
            }

            y += 22;

            AddHtml(35 + 14, y, 176, 20, "Ultima Online: T2A", false, false);

            if (ruleset.Era == Era.T2A)
            {
                AddImage(35, y + 4, 0x939);
            }
            else
            {
                AddButton(35, y + 4, 0x938, 0x939, 100, GumpButtonType.Reply, 0);
            }

            y += 22;

            AddHtml(35 + 14, y, 176, 20, "Ultima Online: Arcade", false, false);

            if (ruleset.Era == Era.Arcade)
            {
                AddImage(35, y + 4, 0x939);
            }
            else
            {
                AddButton(35, y + 4, 0x938, 0x939, 101, GumpButtonType.Reply, 0);
            }

            y += 22;
            y += 6;

            AddHtml(35, y, 190, 20, Center("Rules"), false, false);

            y += 20;

            for (int i = 0; i < m_Defaults.Length; ++i)
            {
                Ruleset cur = m_Defaults[i];

                AddHtml(35 + 14, y, 176, 20, cur.Title, false, false);

                if (ruleset.Base == cur && !ruleset.Changed)
                {
                    AddImage(35, y + 4, 0x939);
                }
                else if (ruleset.Base == cur)
                {
                    AddButton(35, y + 4, 0x93A, 0x939, 2 + i, GumpButtonType.Reply, 0);
                }
                else
                {
                    AddButton(35, y + 4, 0x938, 0x939, 2 + i, GumpButtonType.Reply, 0);
                }

                y += 22;
            }

            AddHtml(35 + 14, y, 176, 20, "Custom", false, false);
            AddButton(35, y + 4, ruleset.Changed ? 0x939 : 0x938, 0x939, 1, GumpButtonType.Reply, 0);

            y += 22;
            y += 6;

            AddHtml(35, y, 190, 20, Center("Flavors"), false, false);
            y += 20;

            for (int i = 0; i < m_Flavors.Length; ++i)
            {
                Ruleset cur = m_Flavors[i];

                AddHtml(35 + 14, y, 176, 20, cur.Title, false, false);

                if (ruleset.Flavors.Contains(cur))
                {
                    AddButton(35, y + 4, 0x939, 0x938, 2 + m_Defaults.Length + i, GumpButtonType.Reply, 0);
                }
                else
                {
                    AddButton(35, y + 4, 0x938, 0x939, 2 + m_Defaults.Length + i, GumpButtonType.Reply, 0);
                }

                y += 22;
            }
        }

        public string Center(string text)
        {
            return String.Format("<CENTER>{0}</CENTER>", text);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Context != null && !m_Context.Registered)
            {
                return;
            }

            switch (info.ButtonID)
            {
                case 0: // closed
                {
                    if (m_Context != null)
                    {
                        m_From.SendGump(new DuelContextGump(m_From, m_Context));
                    }

                    break;
                }
                case 1: // customize
                {
                    m_From.SendGump(new RulesetGump(m_From, m_Ruleset, m_Ruleset.Layout, m_Context));
                    break;
                }
                case 99: // Set UOR
                {
                    m_Ruleset.Era = Era.UOR;
                    m_From.SendGump(new PickRulesetGump(m_From, m_Context, m_Ruleset));
                    break;
                }
                case 100: // Set T2A
                {
                    m_Ruleset.Era = Era.T2A;
                    m_From.SendGump(new PickRulesetGump(m_From, m_Context, m_Ruleset));
                    break;
                }
                case 101: // Set Arcade
                {
                    m_Ruleset.Era = Era.Arcade;
                    m_From.SendGump(new PickRulesetGump(m_From, m_Context, m_Ruleset));
                    break;
                }
                default:
                {
                    int idx = info.ButtonID - 2;

                    if (idx >= 0 && idx < m_Defaults.Length)
                    {
                        m_Ruleset.ApplyDefault(m_Defaults[idx]);
                        m_From.SendGump(new PickRulesetGump(m_From, m_Context, m_Ruleset));
                    }
                    else
                    {
                        idx -= m_Defaults.Length;

                        if (idx >= 0 && idx < m_Flavors.Length)
                        {
                            if (m_Ruleset.Flavors.Contains(m_Flavors[idx]))
                            {
                                m_Ruleset.RemoveFlavor(m_Flavors[idx]);
                            }
                            else
                            {
                                m_Ruleset.AddFlavor(m_Flavors[idx]);
                            }

                            m_From.SendGump(new PickRulesetGump(m_From, m_Context, m_Ruleset));
                        }
                    }

                    break;
                }
            }
        }
    }
}