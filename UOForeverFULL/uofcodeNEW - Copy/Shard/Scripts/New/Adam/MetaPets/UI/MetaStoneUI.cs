#region References

using System;
using System.Collections.Generic;
using Server.Engines.EventScores;
using Server.Gumps;
using Server.Items;
using Server.Mobiles.MetaPet.Skills;
using VitaNex.SuperGumps;
using VitaNex.SuperGumps.UI;

#endregion

namespace Server.Mobiles.MetaPet
{
    public sealed class MetaStoneUI : ListGump<BaseMetaSkill>
    {
        public BaseMetaPet MetaPet { get; set; }

        public EventType EventType { get; set; }

        public MetaStone MetaStone { get; set; }

        public MetaStoneUI(PlayerMobile user, BaseMetaPet pet, MetaStone stone, Gump parent = null)
            : base(user, parent, 0, 0)
        {
            CanDispose = true;
            CanMove = true;

            EntriesPerPage = 4;

            MetaPet = pet;

            MetaStone = stone;

            AutoRefresh = true;

            ForceRecompile = true;

            AutoRefreshRate = TimeSpan.FromSeconds(5.0);
        }

        protected override void CompileList(List<BaseMetaSkill> list)
        {
            list.Clear();

            list.AddRange(MetaPet.Metaskills.Values);
            base.CompileList(list);
        }

        protected override void CompileLayout(SuperGumpLayout layout)
        {
            layout.Add(
                "background",
                () =>
                {
                    AddBackground(0, 0, 426, 338, 9380);
                    AddBackground(15, 30, 397, 283, 3500);
                });

            layout.Add(
                "PetIcon",
                () =>
                {
                    AddLabel(40, 50, 2059, @"Meta-Pet");
                    if (MetaPet is MetaDragon)
                    {
                        AddItem(40, 70, 8406, MetaPet.Hue);
                    }
                    AddLabel(85, 85, 1258, MetaPet.SpecialTitle);
                });

            layout.Add(
                "Stage",
                () =>
                {
                    AddLabel(40, 115, 0, @"Stage");
                    AddLabel(40, 135, 0, MetaPet.Stage + "/" + MetaPet.MaxStage);

                    const int width = 109;

                    var wOffset = (int) Math.Ceiling(width * (MetaPet.Stage / (double) MetaPet.MaxStage));

                    if (wOffset >= width)
                    {
                        AddImageTiled(35, 155, width, 17, 2062);
                    }
                    else
                    {
                        AddImageTiled(35, 155, width, 17, 2061);

                        if (wOffset > 0)
                        {
                            AddImageTiled(35, 155, wOffset, 17, 2062);
                        }
                    }
                });

            layout.Add(
                "Progress",
                () =>
                {
                    AddLabel(40, 180, 0, @"Current Progress");
                    if (MetaPet.Stage != MetaPet.MaxStage)
                    {
                        AddLabel(40, 200, 0, MetaPet.EvolutionPoints + "/" + MetaPet.NextEvolution);

                        const int width = 109;

                        var wOffset =
                            (int) Math.Ceiling(width * (MetaPet.EvolutionPoints / (double) MetaPet.NextEvolution));

                        if (wOffset >= width)
                        {
                            AddImageTiled(35, 220, width, 17, 2062);
                        }
                        else
                        {
                            AddImageTiled(35, 220, width, 17, 2061);

                            if (wOffset > 0)
                            {
                                AddImageTiled(35, 220, wOffset, 17, 2062);
                            }
                        }
                    }
                    else
                    {
                        AddLabel(40, 200, 0, "MAX");
                        AddImageTiled(35, 220, 109, 17, 2062);
                    }
                });

            layout.Add(
                "AbilitySlots",
                () =>
                {
                    AddLabel(40, 245, 0, @"Ability Slots");
                    if (MetaPet.MaxAbilities > 0)
                    {
                        AddLabel(40, 265, 0, MetaPet.CurrentAbilities + "/" + MetaPet.MaxAbilities);
                    }
                    else
                    {
                        AddLabel(40, 265, 0, "No ability slots available.");
                    }
                });

            layout.Add(
                "Skills",
                () =>
                {
                    AddLabel(205, 50, 2059, @"Meta-Skills");
                    AddLabel(355, 49, 2059, @"Level");
                    AddLabel(235, 283, 1258, @"Apply Relic?");
                    AddButton(319, 282, 247, 248, b =>
                    {
                        MetaStone.GetTargetRelic(User);
                        Refresh(true);
                    });

                    if (MetaPet.Metaskills.Count == 0)
                    {
                        AddLabel(205, 80, 0, "No learned skills.");
                    }
                });


            Dictionary<int, BaseMetaSkill> range = GetListRange();


            if (range.Count > 0)
            {
                CompileEntryLayout(layout, range);
            }
        }

        protected override void CompileEntryLayout(
            SuperGumpLayout layout, int length, int index, int pIndex, int yOffset, BaseMetaSkill entry)
        {
            yOffset = 80 + pIndex * 50;

            layout.Add(
                "entry" + index,
                () =>
                {
                    AddLabel(205, yOffset, 0, entry.Name);
                    AddButton(309, yOffset, 56, 56, b => new DialogGump(User, this, 0, 0, null, null, 7004, a =>
                    {
                        if (MetaPet != null && MetaPet.Metaskills.ContainsKey(entry.MetaSkillType))
                        {
                            MetaPet.Metaskills.Remove(entry.MetaSkillType);
                            MetaPet.CurrentAbilities--;
                            Refresh(true);
                        }
                    }, c => { Refresh(true); })
                    {
                        Title = "Confirm Skill Deletion",
                        Html = "Are you sure you wish to delete this skill?  All progress in the skill will be lost.",
                        HtmlColor = DefaultHtmlColor
                    }.Send());
                    AddLabel(355, yOffset, 0, entry.Level + "/" + entry.MaxLevel);
                    AddImageTiled(215, yOffset + 20, 111, 11, 2053);

                    const int width = 111;

                    var wOffset = (int) Math.Ceiling(width * (entry.Experience / (double) entry.NextLevelExperience));

                    if (wOffset >= width)
                    {
                        AddImageTiled(215, yOffset + 20, width, 11, 2057);
                    }
                    else
                    {
                        AddImageTiled(215, yOffset + 20, width, 11, 2053);

                        if (wOffset > 0)
                        {
                            AddImageTiled(215, yOffset + 20, wOffset, 11, 2056);
                        }
                    }
                });
        }
    }
}