#region References

using System;
using System.Collections.Generic;
using System.Linq;
using Server.Engines.DonationsTracker;
using Server.Gumps;
using Server.Mobiles;
using VitaNex.SuperGumps;
using VitaNex.SuperGumps.UI;

#endregion

namespace Server.Engines.EventScores
{
    public sealed class BattlesTUI : ListGump<PlayerEventScoreProfile>
    {
        public PlayerEventScoreProfile SelectedProfile { get; set; }

        public string SearchEmail { get; set; }

        public List<PlayerEventScoreProfile> Profiles;

        public Action<GumpButton> AcceptHandler { get; set; }
        public Action<GumpButton> CancelHandler { get; set; }

        public BattlesTUI(PlayerMobile user, Gump parent = null, PlayerEventScoreProfile battleprofile = null,
            Action<GumpButton> onAccept = null, Action<GumpButton> onCancel = null)
            : base(user, parent, 0, 0)
        {
            CanDispose = true;
            CanMove = true;
            Modal = false;
            ForceRecompile = true;

            AcceptHandler = onAccept;
            CancelHandler = onCancel;

            CanSearch = true;

            EntriesPerPage = 10;

            SelectedProfile = battleprofile;

            AutoRefreshRate = TimeSpan.FromSeconds(60);
            AutoRefresh = true;
        }

        public override string GetSearchKeyFor(PlayerEventScoreProfile key)
        {
            if (key != null)
            {
                return key.DisplayCharacter.Name;
            }

            return base.GetSearchKeyFor(key);
        }

        protected override void CompileList(List<PlayerEventScoreProfile> list)
        {
            list.Clear();

            list.TrimExcess();
            if (Profiles == null)
            {
                Profiles = new List<PlayerEventScoreProfile>();
                Profiles.AddRange(EventScores.SortedProfiles());
            }
            else
            {
                Profiles.Clear();
                Profiles.TrimExcess();
                Profiles.AddRange(EventScores.SortedProfiles());
            }

            if (Profiles.Count == 0)
            {
                Profiles.AddRange(EventScores.SortedProfiles());
            }

            list.AddRange(Profiles);

            base.CompileList(list);
        }

        protected override void CompileLayout(SuperGumpLayout layout)
        {
            layout.Add(
                "background",
                () =>
                {
                    AddBackground(0, 40, 775, 525, 2600);
                    AddImage(269, 18, 1419);
                    AddImage(346, 0, 1417);
                    AddImage(355, 9, 9012, 1360);
                    AddBackground(35, 100, 703, 413, 9270);
                    AddBackground(49, 112, 676, 389, 9200);
                    AddBackground(56, 119, 404, 377, 9260);
                    AddBackground(466, 119, 253, 377, 9260);

                    AddLabel(47, 80, 137, @"1st Annual Battles Tournament");

                    AddItem(245, 65, 9934, 1360);
                    AddItem(225, 65, 9935, 1360);

                    AddItem(350, 150, 11009);
                });

            layout.Add(
                "Search",
                () =>
                {
                    AddLabel(72, 128, 2049, @"Search");
                    AddLabel(72, 146, 1258, @"Enter Character Name");
                    AddBackground(70, 164, 139, 29, 3000);

                    AddTextEntryLimited(73, 170, 161, 24, TextHue, String.Empty, 20, (b, t) => SearchEmail = t);

                    AddButton(213, 168, 4023, 4025, b =>
                    {
                        SearchText = SearchEmail;
                        Page = 0;
                        Refresh(true);
                    });
                });

            layout.Add(
                "Profiles",
                () =>
                {
                    AddLabel(73, 203, 1258, @"Tournament Profiles");
                    AddLabel(230, 203, 1258, @"Rank");
                    AddLabel(300, 203, 1258, @"Score");
                    AddLabel(370, 203, 1258, @"View Profile");

                    AddBackground(71, 222, 377, 232, 3000);

                    if (PageCount - 1 > Page)
                    {
                        AddButton(427, 461, 5601, 5605, NextPage);
                    }

                    if (Page > 0)
                    {
                        AddButton(74, 461, 5603, 5607, PreviousPage);
                    }
                });

            layout.Add(
                "SelectedProfile",
                () =>
                {
                    if (SelectedProfile != null)
                    {
                        AddLabel(482, 131, 1258, @"Character Name");
                        AddLabel(484, 151, 2049, SelectedProfile.DisplayCharacter.RawName);

                        var Rank = EventScores.SortedProfiles().IndexOf(SelectedProfile) + 1;
                        AddLabel(660, 131, 1258, @"Rank");
                        if (Rank.ToString().Length == 1)
                        {
                            AddLabel(668, 151, 2049, Rank.ToString());
                        }
                        else if (Rank.ToString().Length == 2)
                        {
                            AddLabel(665, 151, 2049, Rank.ToString());
                        }
                        else
                        {
                            AddLabel(661, 151, 2049, Rank.ToString());
                        }

                        if (User.AccessLevel == AccessLevel.Player)
                        {
                            AddLabel(482, 181, 1258, @"Total Points Accumulated");
                            AddLabel(484, 201, 2049, SelectedProfile.OverallScore.ToString());
                        }
                        else
                        {
                            AddLabel(482, 181, 1258, @"Spendable Points");
                            AddLabel(484, 201, 2049, SelectedProfile.SpendablePoints.ToString());
                        }

                        AddLabel(482, 221, 1258, @"Contribution From Battles");
                        AddLabel(484, 241, 2049, SelectedProfile.Events.Where(i => i.EventType == EventType.Battles).Sum(i => i.PointsGained).ToString());

                        AddLabel(482, 261, 1258, @"Contribution From Invasions");
                        AddLabel(484, 281, 2049, SelectedProfile.Events.Where(i => i.EventType == EventType.Invasions).Sum(i => i.PointsGained).ToString());

                        AddLabel(482, 301, 1258, @"Contribution from Tournaments");
                        AddLabel(484, 321, 2049, SelectedProfile.Events.Where(i => i.EventType == EventType.Tournaments).Sum(i => i.PointsGained).ToString());

                        AddLabel(482, 341, 1258, @"Contribution from Scav. Hunts");
                        AddLabel(484, 361, 2049, SelectedProfile.Events.Where(i => i.EventType == EventType.Scavenger).Sum(i => i.PointsGained).ToString());

                        AddLabel(482, 382, 137, @"Battles History");
                        AddButton(628, 382, 4005, 4007, b =>
                        {
                            new EntriesUI(User, EventType.Battles, Refresh(true), SelectedProfile).Send();
                        });

                        AddLabel(482, 407, 137, @"Invasion History");
                        AddButton(628, 407, 4005, 4007, b =>
                        {
                            new EntriesUI(User, EventType.Invasions, Refresh(true), SelectedProfile).Send();
                        });

                        AddLabel(482, 432, 137, @"Tournament History");
                        AddButton(628, 432, 4005, 4007, b =>
                        {
                            new EntriesUI(User, EventType.Tournaments, Refresh(true), SelectedProfile).Send();
                        });

                        AddLabel(482, 457, 137, @"Scav. Hunt History");
                        AddButton(628, 457, 4005, 4007, b =>
                        {
                            new EntriesUI(User, EventType.Scavenger, Refresh(true), SelectedProfile).Send();
                        });
                    }
                });

            Dictionary<int, PlayerEventScoreProfile> range = GetListRange();

            if (range.Count > 0)
            {
                CompileEntryLayout(layout, range);
            }
        }

        protected override void CompileEntryLayout(
            SuperGumpLayout layout, int length, int index, int pIndex, int yOffset, PlayerEventScoreProfile entry)
        {
            yOffset = 226 + pIndex * 23;

            layout.Add(
                "entry" + index,
                () =>
                {
                    AddLabel(78, yOffset, 2049, entry.DisplayCharacter.Name);
                    AddLabel(230, yOffset, 2049, (EventScores.SortedProfiles().IndexOf(entry) + 1).ToString());
                    AddLabel(300, yOffset, 2049, entry.OverallScore.ToString());
                    AddButton(388, yOffset - 2, 4023, 4024, b =>
                    {
                        SelectedProfile = entry;
                        Refresh(true);
                    });
                });
        }

        private static int SortByValue(PlayerEventScoreProfile a, PlayerEventScoreProfile b)
        {
            return -1 * a.OverallScore.CompareTo(b.OverallScore);
        }
    }
}