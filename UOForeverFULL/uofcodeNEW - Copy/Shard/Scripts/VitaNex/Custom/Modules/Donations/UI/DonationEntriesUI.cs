#region References

using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Globalization;
using Server.Engines.EventInvasions;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using VitaNex.SuperGumps;
using VitaNex.SuperGumps.UI;

#endregion

namespace Server.Engines.DonationsTracker
{
    public sealed class DonationsEntriesUI : ListGump<DonationEntry>
    {
        public DonationProfile SelectedDonationProfile { get; set; }

        public DonationsEntriesUI(PlayerMobile user, Gump parent = null, DonationProfile donationprofile = null)
            : base(user, parent, 0, 0)
        {
            CanDispose = true;
            CanMove = true;
            Modal = false;
            ForceRecompile = true;

            CanSearch = true;

            EntriesPerPage = 6;

            SelectedDonationProfile = donationprofile;
        }

        protected override void CompileList(List<DonationEntry> list)
        {
            list.Clear();

            list.AddRange(SelectedDonationProfile.DonationEntries);

            base.CompileList(list);
        }

        protected override void CompileLayout(SuperGumpLayout layout)
        {
            layout.Add(
                "background",
                () =>
                {
                    AddBackground(0, 40, 617, 313, 2600);
                    AddBackground(50, 85, 517, 224, 9270);
                    AddBackground(63, 97, 492, 200, 9200);
                    AddBackground(69, 117, 482, 176, 9350);
                    AddLabel(52, 67, 0, @"Donation Entries for " + SelectedDonationProfile.Email);
                    AddLabel(74, 99, 0, @"Date");
                    AddLabel(175, 99, 0, @"Character Name");
                    AddLabel(324, 99, 0, @"Amount Donated");
                    AddLabel(460, 99, 0, @"Coins Given");

                    if (PageCount - 1 > Page)
                    {
                        AddButton(524, 278, 2224, 2224, NextPage);
                    }

                    if (Page > 0)
                    {
                        AddButton(493, 278, 2223, 2223, PreviousPage);
                    }
                });

            Dictionary<int, DonationEntry> range = GetListRange();

            if (range.Count > 0)
            {
                CompileEntryLayout(layout, range);
            }
        }

        protected override void CompileEntryLayout(
            SuperGumpLayout layout, int length, int index, int pIndex, int yOffset, DonationEntry entry)
        {
            yOffset = 125 + pIndex * 26;

            layout.Add(
                "entry" + index,
                () =>
                {
                    AddBackground(71, yOffset-4, 476, 25, 9200);

                    AddLabel(74, yOffset, 0, entry.Date.ToShortDateString());
                    AddLabel(175, yOffset, 0, entry.Character);
                    AddLabel(324, yOffset, 0, entry.AmountDonated.ToString("C2"));
                    AddLabel(460, yOffset, 0, entry.CoinsGiven.ToString());
                });
        }
    }
}