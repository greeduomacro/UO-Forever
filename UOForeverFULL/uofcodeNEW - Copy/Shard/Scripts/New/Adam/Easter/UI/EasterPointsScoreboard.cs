#region References

using System.Collections.Generic;
using Server.Mobiles;
using VitaNex.SuperGumps;
using VitaNex.SuperGumps.UI;

#endregion

namespace Server.Gumps
{
    public class EasterScoreBoard : ListGump<KeyValuePair<PlayerMobile, int>>
    {
        private readonly IDictionary<PlayerMobile, int> Participants;

        public EasterScoreBoard(
            PlayerMobile user,
            IDictionary<PlayerMobile, int> participants)
            : base(user, null, 150, 100, emptyText: "No Scores to Display.", title: "")
        {
            Participants = participants;

            EntriesPerPage = 6;

            Modal = false;
            CanClose = true;
            CanMove = true;
            //ForceRecompile = true;
        }

        protected override void CompileList(List<KeyValuePair<PlayerMobile, int>> list)
        {
            list.Clear();

            list.AddRange(Participants);

            base.CompileList(list);

            list.Sort((l, r) => r.Value.CompareTo(l.Value));
        }

        protected override void CompileMenuOptions(MenuGumpOptions list)
        {}

        protected override void CompileLayout(SuperGumpLayout layout)
        {
            Dictionary<int, KeyValuePair<PlayerMobile, int>> range = GetListRange();

            layout.Add(
                "background/body/base1",
                () => { AddBackground(27, 83, 424, 357, 9200); });

            layout.Add(
                "tiledimage/body/left",
                () => AddImageTiled(30, 85, 21, 349, 10464));

            layout.Add(
                "tiledimage/body/right",
                () => AddImageTiled(426, 87, 21, 349, 10464));

            layout.Add(
                "image/body/dragon",
                () => AddImage(417, 26, 10441, 1165));

            layout.Add(
                "image/body/uosymbol",
                () => AddImage(207, 44, 9000, 1265));

            layout.Add("item/body/easteregg", () => AddItem(219, 93, 18406));

            layout.Add(
                "tiledimage/body/underline",
                () => AddImageTiled(77, 176, 325, 1, 5410));

            layout.Add("label/header/UOF", () => AddLabel(75, 160, 1258, "Ultima Online Forever:"));

            layout.Add("label/header/Easter", () => AddLabel(217, 160, 2049, "A Very Wretched Easter 2014"));

            layout.Add("label/header/EasterPoints", () => AddLabel(200, 186, 2049, "Easter Points"));

            layout.Add(
                "tiledimage/body/underline2",
                () => AddImageTiled(186, 203, 109, 1, 5410));

            layout.Add("label/header/Name", () => AddLabel(92, 216, 2049, "Name"));

            layout.Add("label/header/Score", () => AddLabel(355, 216, 2049, "Points"));

            layout.Add(
                "widget/body/scrollbar",
                () =>
                    AddScrollbarH(
                        69,
                        420,
                        PageCount,
                        Page,
                        PreviousPage,
                        NextPage,
                        new Rectangle2D(30, 0, 280, 13),
                        new Rectangle2D(0, 0, 28, 13),
                        new Rectangle2D(312, 0, 28, 13)));

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
            base.CompileEntryLayout(layout, length, index, pIndex, yOffset, entry);

            yOffset += 170;

            layout.Remove("button/list/select/" + index);
            layout.AddReplace(
                "label/list/entry/" + index,
                () => AddLabel(92, yOffset, GetLabelHue(index, pIndex, entry), entry.Key.RawName));

            layout.Add(
                "label/list/entry/points/" + index,
                () => AddLabel(355, yOffset, GetLabelHue(index, pIndex, entry), entry.Value.ToString()));

            if (pIndex < (length - 1))
            {
                layout.Remove("imagetiled/body/hsep/" + index);
            }
        }

        public override string GetSearchKeyFor(KeyValuePair<PlayerMobile, int> key)
        {
            return key.Key.RawName;
        }
    }
}