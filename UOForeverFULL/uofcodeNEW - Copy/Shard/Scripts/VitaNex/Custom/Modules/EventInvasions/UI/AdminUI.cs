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
    public sealed class InvasionAdminUI : ListGump<Level>
    {
        public string InvasionName { get; set; }
        public string InvasionRegion { get; set; }

        public bool Gates { get; set; }

        public int InvasionTime { get; set; }
        public int MaxMobs { get; set; }

        public List<Level> Levels { get; set; }

        public Level NewLevel { get; set; }

        public Action<GumpButton> AcceptHandler { get; set; }

        public InvasionAdminUI(PlayerMobile user, Gump parent = null, Action<GumpButton> onAccept = null)
            : base(user, parent, 0, 0)
        {
            CanMove = true;
            ForceRecompile = true;
            Levels = new List<Level>();

            NewLevel = new Level(TimeSpan.Zero, 0, 0, null, 0);

            AcceptHandler = onAccept;
        }

        protected override void Compile()
        {
            base.Compile();
        }

        protected override void CompileList(List<Level> list)
        {
            list.Clear();

            list.TrimExcess();

            if (Levels != null)
            {
                list.AddRange(Levels);
            }

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
                    AddImage(355, 9, 9012);
                    AddBackground(35, 100, 703, 413, 9270);
                    AddBackground(49, 112, 676, 389, 9200);
                    AddBackground(362, 119, 354, 377, 9260);

                    AddLabel(59, 463, 0, @"Generate Invasion?");
                    AddButton(185, 462, 247, 248, OnAccept);
                });

            layout.Add(
                "InvasionName",
                () =>
                {
                    AddBackground(58, 122, 195, 66, 9260);
                    AddBackground(70, 148, 171, 30, 3000);
                    AddLabel(73, 132, 0, @"Invasion Name");

                    AddTextEntryLimited(73, 155, 161, 24, TextHue, InvasionName, 30, (b, t) => InvasionName = t);
                });

            layout.Add(
                "Region",
                () =>
                {
                    AddBackground(58, 197, 195, 66, 9260);
                    AddBackground(70, 223, 171, 30, 3000);
                    AddLabel(73, 207, 0, @"Region to Invade");

                    AddTextEntryLimited(73, 231, 161, 24, TextHue, InvasionRegion, 20, (b, t) => InvasionRegion = t);
                });

            /*layout.Add(
                "InvaderTitles",
                () =>
                {
                    AddBackground(58, 272, 195, 66, 9260);
                    AddBackground(70, 299, 171, 30, 3000);
                    AddLabel(73, 283, 0, @"Invader Titles");

                    AddTextEntryLimited(73, 308, 161, 24, TextHue, InvaderTitles, 20, (b, t) => InvaderTitles = t);
                });*/

            layout.Add(
                "Gates",
                () =>
                {
                    AddLabel(60, 280, 0, @"Spawn moongates in towns?");

                    AddCheck(240, 281, 210, 211, false, (b, t) => Gates = t);
                });

            layout.Add(
                "LevelsCategory",
                () =>
                {
                    AddLabel(390, 131, 0, @"Levels");

                    AddLabel(383, 156, 0, @"Level");
                    AddLabel(428, 156, 0, @"Creatures");
                    AddLabel(508, 156, 0, @"Duration");
                    AddLabel(576, 156, 0, @"Kills");
                    AddLabel(618, 156, 0, @"Edit");
                    AddLabel(657, 156, 0, @"Delete");

                    AddLabel(382, 444, 0, @"Create New Level?");
                    AddButton(379, 462, 247, 248, b => Send(new EditLevelUI(User, Hide(true), NewLevel)
                    {
                        AcceptHandler = a =>
                        {
                            if (NewLevel != null && NewLevel.Creatures.Count > 0 && NewLevel.SpawnAmount > 0 &&
                                (NewLevel.TimeLimit.TotalHours > 0 || NewLevel.KillAmount > 0))
                            {
                                Levels.Add(NewLevel);
                                NewLevel = new Level(TimeSpan.Zero, 0, 0, null, 0);
                            }
                            Refresh(true);
                        },
                        CancelHandler = c => Refresh(true)
                    }));
                });

            Dictionary<int, Level> range = GetListRange();

            if (range.Count > 0)
            {
                CompileEntryLayout(layout, range);
            }
        }

        protected override void CompileEntryLayout(
            SuperGumpLayout layout, int length, int index, int pIndex, int yOffset, Level entry)
        {
            yOffset = 180 + pIndex * 20;

            layout.Add(
                "entry" + index,
                () =>
                {
                    AddLabel(382, yOffset, 0, "#" + (index + 1));
                    AddLabel(429, yOffset, 0, entry.Creatures.Count.ToString());
                    AddLabel(500, yOffset, 0, entry.TimeLimit.ToString());
                    AddLabel(576, yOffset, 0, entry.KillAmount.ToString());
                    AddButton(615, yOffset - 2, 4023, 4024, b => Send(new EditLevelUI(User, Hide(true), entry)
                    {
                        AcceptHandler = a => { Refresh(true); },
                        CancelHandler = c => Refresh(true)
                    }));
                    AddButton(654, yOffset + 3, 2181, 2181, b =>
                    {
                        Levels.Remove(entry);
                        Refresh(true);
                    });
                });
        }

        private void OnAccept(GumpButton button)
        {
            if (AcceptHandler != null)
            {
                AcceptHandler(button);
            }

            if (String.IsNullOrWhiteSpace(InvasionName))
            {
                Send(
                    new NoticeDialogGump(User, Hide(true))
                    {
                        Title = "Choose an Invasion Name",
                        Html = "Your invasion must have a name!",
                        AcceptHandler = b => Refresh(true)
                    });
                return;
            }

            if (Region.Regions.Find(x => x.Name == InvasionRegion) == null)
            {
                Send(
                    new NoticeDialogGump(User, Hide(true))
                    {
                        Title = "Region Does Not Exist",
                        Html = "The region you input does not exist.",
                        AcceptHandler = b => Refresh(true)
                    });
                return;
            }

            if (Levels.Count == 0)
            {
                Send(
                    new NoticeDialogGump(User, Hide(true))
                    {
                        Title = "Invalid Levels",
                        Html = "You must specify at least one valid level.",
                        AcceptHandler = b => Refresh(true)
                    });
                return;
            }

            EventInvasions.GenerateInvasion(InvasionName, InvasionRegion, Levels, Gates);

            Close();
        }
    }
}