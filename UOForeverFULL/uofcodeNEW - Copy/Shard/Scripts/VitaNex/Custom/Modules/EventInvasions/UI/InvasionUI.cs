#region References

using System;
using System.Collections.Generic;
using Server.Gumps;
using Server.Mobiles;
using VitaNex.SuperGumps;
using VitaNex.SuperGumps.UI;

#endregion

namespace Server.Engines.EventInvasions
{
    public sealed class InvasionUI : ListGump<Invasion>
    {
        public List<Invasion> Invasions { get; set; }

        public InvasionUI(PlayerMobile user, Gump parent = null)
            : base(user, parent, 0, 0)
        {
            EntriesPerPage = 5;

            ForceRecompile = true;

            CanMove = true;
        }

        protected override void Compile()
        {
            base.Compile();
        }

        protected override void CompileList(List<Invasion> list)
        {
            list.Clear();

            list.TrimExcess();

            Invasions = new List<Invasion>(EventInvasions.Invasions.Values);

            if (Invasions != null)
            {
                list.AddRange(Invasions);
            }

            list.Sort((a, b) => b.DateStarted.CompareTo(a.DateStarted));

            base.CompileList(list);
        }

        protected override void CompileLayout(SuperGumpLayout layout)
        {
            layout.Add(
                "background/body/base",
                () =>
                {
                    AddBackground(0, 40, 775, 492, 2600);
                    AddImage(269, 18, 1419);
                    AddImage(346, 0, 1417);
                    AddImage(355, 9, 9012);
                    AddBackground(35, 100, 703, 373, 9270);
                    AddBackground(49, 112, 676, 348, 9200);

                    if (PageCount - 1 > Page)
                    {
                        AddLabel(625, 473, 0, @"Next Page");
                        AddButton(691, 477, 2224, 2224, NextPage);
                    }

                    if (Page > 0)
                    {
                        AddLabel(516, 473, 0, @"Previous Page");
                        AddButton(494, 477, 2223, 2223, PreviousPage);
                    }

                    if (User.AccessLevel >= EventInvasions.Access)
                    {
                        AddLabel(50, 493, 0, @"Create New Invasion?");

                        AddButton(197, 493, 2074, 2076, b => Send(new InvasionAdminUI(User, Hide(true))
                        {
                            AcceptHandler = a => { Refresh(true); }
                        }));
                    }
                });

            Dictionary<int, Invasion> range = GetListRange();

            if (range.Count > 0)
            {
                CompileEntryLayout(layout, range);
            }
        }

        protected override void CompileEntryLayout(
            SuperGumpLayout layout, int length, int index, int pIndex, int yOffset, Invasion entry)
        {
            yOffset = 116 + pIndex * 67;

            layout.Add(
                "entry" + index,
                () =>
                {
                    AddBackground(54, yOffset, 664, 72, 9260);

                    AddLabel(71, yOffset + 10, 0, @"Invasion Name");
                    AddLabel(71, yOffset + 38, 0, entry.InvasionName);

                    AddLabel(208, yOffset + 10, 0, @"Invasion Region");
                    AddLabel(208, yOffset + 38, 0, entry.RegionName);

                    AddLabel(335, yOffset + 10, 0, @"Date");
                    AddLabel(335, yOffset + 38, 0, entry.DateStarted.ToShortDateString());

                    AddLabel(439, yOffset + 10, 0, @"Status");
                    AddLabel(439, yOffset + 38, entry.Status.AsHue(), entry.Status.ToString());

                    AddLabel(505, yOffset + 10, 0, @"View Details");
                    AddButton(518, yOffset + 38, 2074, 2076, b =>
                    {
                        new InvasionDetailsUI(User, Hide(true), entry).
                            Send<InvasionDetailsUI>();
                    });

                    if (User.AccessLevel < EventInvasions.Access && entry.Status != InvasionStatus.Waiting)
                    {
                        AddLabel(605, yOffset + 10, 0, @"View Scoreboard");
                        AddButton(630, yOffset + 38, 2074, 2076, b => Send(new InvasionScoresOverviewGump(User, entry, Refresh(true))));
                    }

                    if (User.AccessLevel >= EventInvasions.Access)
                    {
                        if (entry.Status == InvasionStatus.Running)
                        {
                            AddLabel(600, yOffset + 10, 0, @"Cancel");
                            AddButton(590, yOffset + 38, 2071, 2073, b =>
                            {
                                if (entry.Status == InvasionStatus.Running)
                                {
                                    entry.FinishInvasion();
                                }
                                Refresh(true);
                            });
                        }

                        if (entry.Status == InvasionStatus.Waiting)
                        {
                            AddLabel(600, yOffset + 10, 0, @"Start");
                            AddButton(593, yOffset + 38, 2074, 2076, b =>
                            {
                                if (entry.Status == InvasionStatus.Waiting)
                                {
                                    entry.StartInvasion();
                                }
                                Refresh(true);
                            });
                        }

                        if (entry.Status == InvasionStatus.Finished)
                        {
                            AddLabel(600, yOffset + 10, 0, @"Restart");
                            AddButton(597, yOffset + 38, 2074, 2076, b =>
                            {
                                if (entry.Status == InvasionStatus.Finished)
                                {
                                    entry.RestartInvasion();
                                }
                                Refresh(true);
                            });
                        }

                        AddLabel(666, yOffset + 10, 0, @"Copy");
                        AddButton(657, yOffset + 38, 2074, 2076, b =>
                        {
                            EventInvasions.GenerateInvasion(entry);
                            Refresh(true);
                        });
                    }
                });
        }
    }
}