#region References

using System;
using System.Collections.Generic;
using System.Globalization;
using Server.Gumps;
using Server.Mobiles;
using VitaNex.SuperGumps;
using VitaNex.SuperGumps.UI;

#endregion

namespace Server.Engines.EventInvasions
{
    public sealed class EditLevelUI : ListGump<Type>
    {
        public Level Level { get; set; }

        public string InvaderTitles { get; set; }
        public int MaxKills { get; set; }
        public int ConcurrentInvaders { get; set; }
        public int Plat { get; set; }
        public TimeSpan TimeLimit { get; set; }

        public double DropChance { get; set; }

        public Action<GumpButton> AcceptHandler { get; set; }
        public Action<GumpButton> CancelHandler { get; set; }

        public EditLevelUI(PlayerMobile user, Gump parent = null, Level level = null,
            Action<GumpButton> onAccept = null, Action<GumpButton> onCancel = null)
            : base(user, parent, 0, 0)
        {
            EntriesPerPage = 8;
            CanDispose = true;
            CanMove = true;
            Modal = false;
            ForceRecompile = true;

            AcceptHandler = onAccept;
            CancelHandler = onCancel;

            if (level == null)
            {
                Level = new Level(TimeSpan.Zero, 0, 0, null, 0);
            }
            else
            {
                Level = level;

                MaxKills = Level.KillAmount;
                InvaderTitles = Level.InvaderTitles;
                ConcurrentInvaders = Level.SpawnAmount;
                TimeLimit = Level.TimeLimit;
                Plat = Level.Plat;

                DropChance = Level.DropChance;
            }
        }

        protected override void Compile()
        {
            base.Compile();
        }

        protected override void CompileList(List<Type> list)
        {
            list.Clear();

            list.TrimExcess();

            if (Level != null)
            {
                if (Level.Creatures != null)
                {
                    list.AddRange(Level.Creatures);
                }
                if (Level.RewardItems != null)
                {
                    list.AddRange(Level.RewardItems);
                }
            }

            base.CompileList(list);
        }

        protected override void CompileLayout(SuperGumpLayout layout)
        {
            layout.Add(
                "background",
                () =>
                {
                    AddBackground(0, 40, 694, 360, 2600);
                    AddImage(242, 17, 1419);
                    AddImage(319, -1, 1417);
                    AddImage(328, 8, 9012);
                    AddBackground(50, 85, 591, 264, 9270);
                    AddBackground(63, 96, 566, 240, 9200);
                    AddLabel(53, 64, 0, @"Level Editor");

                    AddLabel(57, 360, 0, @"Accept?");
                    AddButton(118, 360, 2074, 2076, OnAccept);
                });

            layout.Add(
                "MaxKills",
                () =>
                {
                    AddBackground(71, 124, 121, 28, 9350);
                    AddLabel(71, 105, 0, @"Max Kills for Level");

                    AddTextEntryLimited(72, 128, 116, 20, TextHue, MaxKills.ToString(CultureInfo.InvariantCulture),
                        4,
                        (b, t) =>
                        {
                            int sMobs;

                            if (Int32.TryParse(t, out sMobs))
                            {
                                MaxKills = Math.Max(0, sMobs);
                            }
                        });
                });

            layout.Add(
                "TimeLimit",
                () =>
                {
                    AddBackground(71, 169, 121, 28, 9350);
                    AddLabel(71, 150, 0, @"Time Limit for Level");

                    AddTextEntryLimited(72, 173, 116, 20, TextHue, TimeLimit.TotalHours.ToString(),
                        4,
                        (b, t) =>
                        {
                            double sTime;

                            if (double.TryParse(t, out sTime))
                            {
                                TimeLimit = TimeSpan.FromHours(sTime);
                            }
                        });
                });

            layout.Add(
                "ConcurrentInvaders",
                () =>
                {
                    AddLabel(71, 195, 0, @"Concurrent Invaders");
                    AddBackground(71, 214, 121, 28, 9350);

                    AddTextEntryLimited(72, 218, 116, 20, TextHue,
                        ConcurrentInvaders.ToString(CultureInfo.InvariantCulture),
                        4,
                        (b, t) =>
                        {
                            int sMobs;

                            if (Int32.TryParse(t, out sMobs))
                            {
                                ConcurrentInvaders = Math.Max(1, sMobs);
                            }
                        });
                });

            layout.Add(
                "InvaderTitles",
                () =>
                {
                    AddBackground(71, 259, 143, 28, 9350);
                    AddLabel(71, 240, 0, @"Invader Titles");

                    AddTextEntryLimited(72, 263, 139, 20, TextHue, InvaderTitles, 20, (b, t) => InvaderTitles = t);
                });

            layout.Add(
                "Platinum",
                () =>
                {
                    AddBackground(71, 304, 121, 28, 9350);
                    AddLabel(71, 285, 0, @"Platinum");

                    AddTextEntryLimited(72, 308, 116, 20, TextHue, Plat.ToString(), 4, (b, t) =>
                    {
                        int sPlat;

                        if (Int32.TryParse(t, out sPlat))
                        {
                            Plat = Math.Max(0, sPlat);
                        }
                    });
                });

            layout.Add(
                "Creatures",
                () =>
                {
                    AddLabel(256, 105, 0, @"Creatures");

                    AddLabel(256, 315, 0, @"Add Creature?");
                    AddButton(356, 315, 2074, 2076, b => Send(new CreatureTypesGump(User, Level, Hide())));

                    if (PageCount - 1 > Page)
                    {
                        AddButton(387, 300, 2224, 2224, NextPage);
                    }

                    if (Page > 0)
                    {
                        AddButton(358, 300, 2223, 2223, PreviousPage);
                    }
                });

            layout.Add(
                "regularMobItems",
                () =>
                {
                    AddLabel(427, 105, 0, @"Items");

                    AddLabel(427, 315, 0, @"Add Item?");
                    AddButton(495, 315, 2074, 2076, b => Send(new ItemTypesGump(User, Level, Hide())));
                });

            layout.Add(
                "DropChance",
                () =>
                {
                    AddBackground(546, 124, 65, 28, 9350);
                    AddLabel(546, 105, 0, @"Drop Chance");

                    AddTextEntryLimited(547, 128, 60, 20, TextHue, DropChance.ToString(CultureInfo.InvariantCulture),
                        4,
                        (b, t) =>
                        {
                            double chance;

                            if (Double.TryParse(t, out chance))
                            {
                                DropChance = Math.Max(0, chance);
                            }
                        });
                });

            Dictionary<int, Type> range = GetListRange();

            if (range.Count > 0)
            {
                CompileEntryLayout(layout, range);
            }
        }

        protected override void CompileEntryLayout(
            SuperGumpLayout layout, int length, int index, int pIndex, int yOffset, Type entry)
        {
            yOffset = 127 + pIndex * 25;

            if (Level.Creatures != null && Level.Creatures.Contains(entry))
            {
                layout.Add(
                    "entry" + index,
                    () =>
                    {
                        AddLabel(256, yOffset, 0, (index + 1) + ")");
                        AddLabel(256 + 16, yOffset, 0, entry.Name);
                        AddButton(256 + 120, yOffset - 1, 4017, 4018, b =>
                        {
                            if (Level.Creatures != null && Level.Creatures.Contains(entry))
                            {
                                if (Level.Creatures.Count - 1 > 0)
                                {
                                    Level.Creatures.Remove(entry);
                                    Refresh(true);
                                }
                                else
                                {
                                    Send(
                                        new NoticeDialogGump(User, Hide(true))
                                        {
                                            Title = "Specify Invaders",
                                            Html = "You must specify at least one creature as an invader.",
                                            AcceptHandler = b1 => Refresh(true)
                                        });
                                }
                            }
                        });
                    });
            }

            if (Level.RewardItems != null && Level.RewardItems.Contains(entry))
            {
                layout.Add(
                    "entry" + index,
                    () =>
                    {
                        AddLabel(427, 127, 0, (index + 1) + ")");
                        AddLabel(427 + 16, 127, 0, entry.Name);
                        AddButton(427 + 90, (127 - 1), 4017, 4018, b =>
                        {
                                if (Level.RewardItems != null && Level.RewardItems.Contains(entry))
                                {
                                    Level.RewardItems.Remove(entry);
                                    Refresh(true);
                                }
                        });
                    });
            }
        }

        private void OnAccept(GumpButton button)
        {
            if (MaxKills <= 0 && TimeLimit.TotalHours <= 0)
            {
                Send(
                    new NoticeDialogGump(User, Hide(true))
                    {
                        Title = "Specify Level Conclusion Conditions",
                        Html = "You must specify either a kill amount, time limit or both to conclude the level.",
                        AcceptHandler = b => Refresh(true)
                    });
                return;
            }

            if (ConcurrentInvaders <= 0)
            {
                Send(
                    new NoticeDialogGump(User, Hide(true))
                    {
                        Title = "Specify an Invader Amount",
                        Html = "You must specify the amount of invaders to be spawned at any given time.",
                        AcceptHandler = b => Refresh(true)
                    });
                return;
            }

            if (Level.Creatures.Count <= 0)
            {
                Send(
                    new NoticeDialogGump(User, Hide(true))
                    {
                        Title = "Specify Invaders",
                        Html = "You must specify at least one creature as an invader.",
                        AcceptHandler = b => Refresh(true)
                    });
                return;
            }

            Level.InvaderTitles = InvaderTitles;
            Level.KillAmount = MaxKills;
            Level.SpawnAmount = ConcurrentInvaders;
            Level.TimeLimit = TimeLimit;
            Level.Plat = Plat;
            Level.DropChance = DropChance;

            if (AcceptHandler != null)
            {
                AcceptHandler(button);
            }

            Close();
        }
    }
}