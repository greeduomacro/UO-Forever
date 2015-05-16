#region References

using System.Collections.Generic;
using System.Linq;
using Server.Engines.EventInvasions;
using Server.Gumps;
using Server.Mobiles;
using VitaNex.SuperGumps;
using VitaNex.SuperGumps.UI;

#endregion

namespace Server
{
    public class InvasionScoresOverviewGump : ListGump<KeyValuePair<PlayerMobile, int>>
    {
        public Dictionary<PlayerMobile, int> PlayerScores { get; set; }

        public Invasion Invasion { get; set; }

        public InvasionScoresOverviewGump(
            PlayerMobile user,
            Invasion invasion,
            Gump parent = null)
            : base(user, parent, 150, 100, emptyText: "No Scores to Display.", title: "")
        {
            PlayerScores = invasion.ParticipantsScores;
            Invasion = invasion;

            EntriesPerPage = 10;

            Modal = false;
            CanClose = true;
            CanMove = true;
            //ForceRecompile = true;
        }

        protected override void CompileList(List<KeyValuePair<PlayerMobile, int>> list)
        {
            list.Clear();
            list.TrimExcess();
            list.AddRange(PlayerScores);

            list.Sort(SortByValue);

            base.CompileList(list);
        }

        protected override void CompileMenuOptions(MenuGumpOptions list)
        {}

        protected override void CompileLayout(SuperGumpLayout layout)
        {
            if (IsDisposed)
            {
                return;
            }

            layout.Add(
                "background/body",
                () =>
                {
                    AddBackground(0, 0, 457, 478, 9380);
                    AddBackground(28, 41, 401, 401, 3500);
                });

            layout.Add(
                "images/body",
                () =>
                {
                    AddImageTiled(49, 406, 359, 37, 93);
                    AddImage(407, 404, 94);
                    AddImageTiled(66, 81, 328, 1, 5174);
                    AddImage(48, 54, 113);
                    AddImage(378, 54, 113);
                });

            layout.Add(
                "labels/body",
                () =>
                {
                    AddLabel(89, 63, 2049, @"Points Scoreboard for:");
                    if (Invasion != null)
                    {
                        AddLabelCropped(243, 63, 155, 16, 1287, Invasion.InvasionName);
                    }
                });

            layout.Add(
                "buttons/body",
                () =>
                {
                    AddButton(412, 1, 4017, 4018, 0, GumpButtonType.Reply, 0); //exit

                    if (PageCount - 1 > Page)
                    {
                        AddLabel(337, 409, 0, @"Next");
                        AddButton(371, 409, 4005, 4006, NextPage); //right
                    }

                    if (Page > 0)
                    {
                        AddLabel(91, 411, 0, @"Previous");
                        AddButton(57, 409, 4014, 4015, 2, PreviousPage); //left
                    }
                });


            Dictionary<int, KeyValuePair<PlayerMobile, int>> range = GetListRange();

            CompileEntryLayout(layout, range);
        }

        protected override void CompileEntryLayout(
            SuperGumpLayout layout,
            int length,
            int index,
            int pIndex,
            int yOffset,
            KeyValuePair<PlayerMobile, int> entry)
        {
            //base.CompileEntryLayout(layout, length, index, pIndex, yOffset, entry);
            layout.Add(
                "images/entry/" + index,
                () =>
                {
                    AddImage(79, 107 + 30 * (pIndex), 97);
                    AddImageTiled(87, 124 + 30 * (pIndex), 149, 1, 96);
                    AddImage(235, 108 + 30 * (pIndex), 95);
                });

            layout.Add(
                "labels/entry/" + index,
                () =>
                {
                    AddLabel(55, 86, 1258, @"Names");
                    AddLabel(256, 86, 1258, @"Overall Score");
                    AddLabel(55, 107 + 30 * (pIndex), 2049, (index + 1) + ")");
                    AddLabel(90, 107 + 30 * (pIndex), 1287, entry.Key.Name);
                    AddLabel(255, 107 + 30 * (pIndex), 1287, entry.Value.ToString());
                });
        }

        static int SortByValue(KeyValuePair<PlayerMobile, int> a, KeyValuePair<PlayerMobile, int> b)
        {
            return -1*a.Value.CompareTo(b.Value);
        }

    }
}