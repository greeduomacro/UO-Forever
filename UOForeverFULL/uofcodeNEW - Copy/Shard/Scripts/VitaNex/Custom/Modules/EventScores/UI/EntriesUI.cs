#region References

using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Globalization;
using System.Linq;
using Server.Engines.EventInvasions;
using Server.Engines.EventScores;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using VitaNex.SuperGumps;
using VitaNex.SuperGumps.UI;

#endregion

namespace Server.Engines.DonationsTracker
{
    public sealed class EntriesUI : ListGump<EventObject>
    {
        public PlayerEventScoreProfile SelecteProfile { get; set; }

        public EventType EventType { get; set; }

        public EntriesUI(PlayerMobile user, EventType type, Gump parent = null, PlayerEventScoreProfile profile = null)
            : base(user, parent, 0, 0)
        {
            CanDispose = true;
            CanMove = true;
            Modal = false;
            ForceRecompile = true;

            CanSearch = true;

            EntriesPerPage = 6;

            SelecteProfile = profile;

            EventType = type;
        }

        protected override void CompileList(List<EventObject> list)
        {
            list.Clear();

            list.TrimExcess();
            list.AddRange(SelecteProfile.Events.Where(x => x.EventType == EventType));
            base.CompileList(list);
        }

        protected override void CompileLayout(SuperGumpLayout layout)
        {
            layout.Add(
                "background",
                () =>
                {
                    AddBackground(0, 40, 467, 313, 2600);
                    AddBackground(50, 85, 367, 224, 9270);
                    AddBackground(63, 97, 342, 200, 9200);
                    AddBackground(69, 117, 332, 176, 9350);
                    AddLabel(52, 67, 0, EventType.ToString() + " Entries for " + SelecteProfile.DisplayCharacter.RawName);
                    AddLabel(74, 99, 0, @"Date Awarded");
                    AddLabel(184, 99, 0, @"Event Name");
                    AddLabel(345, 99, 0, @"Score");

                    if (PageCount - 1 > Page)
                    {
                        AddButton(380, 278, 2224, 2224, NextPage);
                    }

                    if (Page > 0)
                    {
                        AddButton(355, 278, 2223, 2223, PreviousPage);
                    }
                });

            Dictionary<int, EventObject> range = GetListRange();

            if (range.Count > 0)
            {
                CompileEntryLayout(layout, range);
            }
        }

        protected override void CompileEntryLayout(
            SuperGumpLayout layout, int length, int index, int pIndex, int yOffset, EventObject entry)
        {
            yOffset = 125 + pIndex * 26;

            layout.Add(
                "entry" + index,
                () =>
                {
                    AddBackground(71, yOffset-4, 326, 25, 9200);

                    AddLabel(74, yOffset, 0, entry.TimeAwarded.ToShortDateString());
                    AddLabel(184, yOffset, 0, entry.EventName);
                    AddLabel(345, yOffset, 0, entry.PointsGained.ToString());

                });
        }
    }
}