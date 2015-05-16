#region References

using System;
using System.Collections.Generic;
using System.Drawing;
using Server.Ethics;
using Server.Ethics.Hero;
using Server.Gumps;
using Server.Mobiles;
using VitaNex.SuperGumps;

#endregion

namespace Server.Engines.Ethics
{
    public class EthicSpellbookUI : SuperGumpList<Power>
    {
        public Ethic Ethic { get; set; }
        public Player PlayerState { get; set; }

        public EthicSpellbookUI(
            PlayerMobile user, Gump parent = null, Ethic ethic = null)
            : base(user, parent)
        {
            Ethic = ethic;

            PlayerState = Player.Find(user);

            ForceRecompile = true;

            CanMove = true;

            Sorted = true;

            EntriesPerPage = 2;
        }

        protected override void CompileList(List<Power> list)
        {
            list.Clear();

            list.AddRange(Ethic.Definition.Powers);

            list.Sort((x, y) => x.Definition.Name.ToString().ToLower().CompareTo(x.Definition.Name.String.ToLower()));

            base.CompileList(list);
        }

        public override Dictionary<int, Power> GetListRange()
        {
            return GetListRange((Page - 1) * EntriesPerPage, EntriesPerPage);
        }

        public override void InvalidatePageCount()
        {
            if (List.Count > EntriesPerPage)
            {
                if (EntriesPerPage > 0)
                {
                    PageCount = (int) Math.Ceiling(List.Count / (double) EntriesPerPage);
                    PageCount = Math.Max(1, PageCount) + 1;
                }
                else
                {
                    PageCount = 1;
                }
            }
            else
            {
                PageCount = 1;
            }

            Page = Math.Max(0, Math.Min(PageCount - 1, Page));
        }

        protected override void CompileLayout(SuperGumpLayout layout)
        {
            base.CompileLayout(layout);

            layout.Add(
                "image/spellbook",
                () => { AddImage(0, 0, Ethic is HeroEthic ? 11009 : 11008); });

            if (Page == 0)
            {
                int count = 0;
                foreach (Power power in Ethic.Definition.Powers)
                {
                    int count1 = count;
                    int xoffset;
                    if (count <= 3)
                    {
                        xoffset = 57;
                    }
                    else
                    {
                        count1 = count1 - 4;
                        xoffset = 225;
                    }
                    Power power1 = power;
                    layout.Add(
                        "power/spellbook" + count,
                        () =>
                        {
                            AddLabel(xoffset + 19, 33 + 33 * count1, 0,
                                power1.Definition.Name);
                            AddButton(xoffset, 36 + 33 * count1, 1209, 1210, btn =>
                            {
                                if (power1.CheckInvoke(PlayerState))
                                {
                                    User.Say(true, power1.Definition.Phrase);
                                    power1.BeginInvoke(PlayerState);
                                }
                            });
                        });
                    count++;
                }
            }

            if (Page == 0)
            {
                layout.Add(
                    "label/lifeforce",
                    () =>
                    {
                        AddLabel(59, 158, 1258, "Life Force:");
                        AddLabel(59, 176, Ethic is HeroEthic ? 2049 : 1100, PlayerState.Power + "/100");
                    });
                layout.Add(
                    "label/rank",
                    () =>
                    {
                        AddLabel(227, 158, 1258, "Current Rank:");
                        AddLabel(227, 176, Ethic is HeroEthic ? 2049 : 1100,
                            Ethic.Definition.Ranks[PlayerState.Rank].Title);
                    });
            }

            if (PageCount > 0 && Page + 1 != PageCount)
            {
                layout.Add(
                    "button/next",
                    () => { AddButton(321, 9, 2236, 2236, NextPage); });
            }

            if (PageCount > 0 && Page != 0)
            {
                layout.Add(
                    "button/previous",
                    () => { AddButton(50, 9, 2235, 2235, PreviousPage); });
            }


            if (Page != 0)
            {
                Dictionary<int, Power> range = GetListRange();
                CompileSpellLayout(layout, range);
            }
        }

        protected virtual void CompileSpellLayout(SuperGumpLayout layout, Dictionary<int, Power> range)
        {
            range.For((i, kv) => CompileSpellLayout(layout, range.Count, kv.Key, i, 138 + (i * 91), kv.Value));
        }

        protected virtual void CompileSpellLayout(
            SuperGumpLayout layout, int length, int index, int pIndex, int yOffset, Power entry)
        {
            int xoffset;
            if (index % 2 == 0)
            {
                xoffset = 59;
            }
            else
            {
                xoffset = 225;
            }
            layout.Add(
                "button/icon" + index,
                () =>
                {
                    AddButton(xoffset, 37, entry.Definition.Icon, entry.Definition.Icon, btn =>
                    {
                        if (entry.CheckInvoke(PlayerState))
                        {
                            User.Say(true, entry.Definition.Phrase);
                            entry.BeginInvoke(PlayerState);
                        }
                    });
                });

            layout.Add(
                "label/name" + index,
                () => { AddLabel(xoffset + 1, 84, 0, entry.Definition.Name); });

            layout.Add(
                "label/phrase" + index,
                () => { AddLabel(xoffset + 46, 36, Ethic is HeroEthic ? 2049 : 1100, entry.Definition.Phrase); });

            layout.Add(
                "label/cost" + index,
                () => AddLabel(xoffset + 46, 61, 0, entry.Definition.Power + " LF"));

            layout.Add(
                "html/description" + index,
                () =>
                {
                    AddHtml(xoffset, 102, 132, 86,
                        entry.Definition.Description.ToString().WrapUOHtmlColor(Color.DarkSlateBlue, false),
                        false, false);
                });
        }
    }
}