#region References

using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Globalization;
using Server.Engines.EventInvasions;
using Server.Engines.Help;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using VitaNex.SuperGumps;
using VitaNex.SuperGumps.UI;

#endregion

namespace Server.Engines.DonationsTracker
{
    public sealed class DonationsUI : ListGump<DonationProfile>
    {
        public DonationProfile SelectedDonationProfile { get; set; }

        public string NewProfileEmail { get; set; }
        public string InvasionRegion { get; set; }

        public string SearchEmail { get; set; }

        public List<DonationProfile> DonationProfiles;

        public string CharacterName { get; set; }

        public int CashAmount { get; set; }

        public int CointAmount { get; set; }

        public Action<GumpButton> AcceptHandler { get; set; }
        public Action<GumpButton> CancelHandler { get; set; }

        public DonationsUI(PlayerMobile user, Gump parent = null, DonationProfile donationprofile = null,
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

            EntriesPerPage = 7;

            SelectedDonationProfile = donationprofile;

            if (DonationProfiles == null)
            {
                DonationProfiles = new List<DonationProfile>();
                DonationProfiles.AddRange(DonationsTracker.DonationProfiles.Values);
            }
        }

        public override string GetSearchKeyFor(DonationProfile key)
        {
            if (key != null)
            {
                return key.Email;
            }

            return base.GetSearchKeyFor(key);
        }

        protected override void CompileList(List<DonationProfile> list)
        {
            list.Clear();

            if (DonationProfiles.Count == 0)
            {
                DonationProfiles.AddRange(DonationsTracker.DonationProfiles.Values);
            }

            list.AddRange(DonationProfiles);

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
                    AddBackground(56, 119, 261, 377, 9260);
                    AddBackground(322, 119, 397, 377, 9260);
                });

            layout.Add(
                "Search",
                () =>
                {
                    AddLabel(72, 128, 0, @"Search");
                    AddLabel(72, 146, 0, @"Enter Email");
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
                "CreateNewProfile",
                () =>
                {
                    AddLabel(72, 196, 0, @"Create a new Profile?");
                    AddLabel(72, 214, 0, @"Enter Email");
                    AddBackground(70, 232, 139, 29, 3000);

                    AddTextEntryLimited(73, 238, 161, 24, TextHue, String.Empty, 100, (b, t) => NewProfileEmail = t);

                    AddButton(213, 236, 4023, 4025, b =>
                    {
                        if (String.IsNullOrWhiteSpace(NewProfileEmail))
                        {
                            Send(
                                new NoticeDialogGump(User, Hide(true))
                                {
                                    Title = "You Must Enter a Profile Email!",
                                    Html = "Your new donation profile must have a valid email!",
                                    AcceptHandler = a => Refresh(true)
                                });
                        }
                        if (String.IsNullOrWhiteSpace(NewProfileEmail))
                        {
                            Send(
                                new NoticeDialogGump(User, Hide(true))
                                {
                                    Title = "You Must Enter a Profile Email!",
                                    Html = "Your new donation profile must have a valid email!",
                                    AcceptHandler = a => Refresh(true)
                                });
                        }
                        else if (DonationsTracker.DonationProfiles.ContainsKey(NewProfileEmail))
                        {
                            Send(
                                new NoticeDialogGump(User, Hide(true))
                                {
                                    Title = "This Email Already Exists!",
                                    Html = "A donation profile with this email already exists!",
                                    AcceptHandler = a => Refresh(true)
                                });
                        }
                        else
                        {
                            SelectedDonationProfile = DonationsTracker.EnsureProfile(NewProfileEmail);
                            Refresh(true);
                        }
                    });
                });

            layout.Add(
                "DonationProfiles",
                () =>
                {
                    AddLabel(73, 269, 0, @"Donation Profiles");
                    AddBackground(71, 290, 228, 192, 3000);

                    if (PageCount - 1 > Page)
                    {
                        AddButton(278, 461, 5601, 5605, NextPage);
                    }

                    if (Page > 0)
                    {
                        AddButton(259, 461, 5603, 5607, PreviousPage);
                    }
                });


            layout.Add(
                "SelectedDonationProfile",
                () =>
                {
                    if (SelectedDonationProfile != null)
                    {
                        AddLabel(338, 128, 0, SelectedDonationProfile.Email);

                        AddLabel(338, 157, 0, @"Donation History");
                        AddButton(450, 157, 4005, 4007, b =>
                        {
                            new DonationsEntriesUI(User, Refresh(true), SelectedDonationProfile).Send();
                        });

                        AddLabel(337, 189, 0, @"Current Amount Donated: ");
                        AddLabel(502, 189, 0, SelectedDonationProfile.TotalDonations.ToString("C2"));

                        AddLabel(338, 223, 0, @"Grant Donation Coins?");
                        AddLabel(338, 245, 0, @"Character Name");
                        AddBackground(337, 266, 164, 29, 3000);
                        AddTextEntryLimited(340, 272, 161, 24, TextHue, String.Empty, 100, (b, t) => CharacterName = t);

                        AddLabel(515, 245, 0, @"Amount Donated");
                        AddBackground(514, 266, 164, 29, 3000);
                        AddTextEntryLimited(517, 272, 161, 24, TextHue,
                            String.Empty, 100, (b, t) =>
                            {
                                int scash;

                                if (Int32.TryParse(t, out scash))
                                {
                                    CashAmount = Math.Max(0, scash);
                                }
                            });

                        AddLabel(338, 299, 0, @"Amount of Coins to Give");
                        AddBackground(337, 319, 164, 29, 3000);
                        AddTextEntryLimited(340, 325, 161, 24, TextHue,
                            String.Empty, 100, (b, t) =>
                            {
                                int scoins;

                                if (Int32.TryParse(t, out scoins))
                                {
                                    CointAmount = Math.Max(0, scoins);
                                }
                            });

                        AddButton(505, 323, 4023, 4025, b =>
                        {
                            if (String.IsNullOrWhiteSpace(CharacterName))
                            {
                                Send(
                                    new NoticeDialogGump(User, Hide(true))
                                    {
                                        Title = "You Must Enter a Valid Character Name",
                                        Html = "You must enter a valid character name!",
                                        AcceptHandler = a => Refresh(true)
                                    });
                                return;
                            }
                            if (CointAmount == 0)
                            {
                                Send(
                                    new NoticeDialogGump(User, Hide(true))
                                    {
                                        Title = "You Must Enter a Valid Amount of Coins to Give",
                                        Html = "Please enter a valid amount of coins in an integer form.",
                                        AcceptHandler = a => Refresh(true)
                                    });
                                return;
                            }
                            if (CashAmount == 0)
                            {
                                Send(
                                    new NoticeDialogGump(User, Hide(true))
                                    {
                                        Title = "You Must Enter a Valid Amount Donated",
                                        Html = "Please enter a valid number for Amount Donated.  The amount donated is the actual dollar amount of the donation.  Enter this in an integer form, IE: 100.  No decimals!",
                                        AcceptHandler = a => Refresh(true)
                                    });
                                return;
                            }

                            Mobile player = null;
                            List<Mobile> mobs = new List<Mobile>(World.Mobiles.Values);
                            foreach (Mobile m in mobs)
                                if (m is PlayerMobile && !String.IsNullOrEmpty(m.RawName) &&
                                    m.RawName.Trim().ToLower() == CharacterName.ToLower())
                                {
                                    player = m;
                                }

                            if (player == null)
                            {
                                Send(
                                    new NoticeDialogGump(User, Hide(true))
                                    {
                                        Title = "Character does not exist!",
                                        Html = "A character with this name does not exist.",
                                        AcceptHandler = a => Refresh(true)
                                    });
                            }
                            else
                            {
                                player.SendGump(new MessageSentGump(player, "Shane", CointAmount + " donation coins have been placed in your bank box. Thank you very much for donating to Ultima Online Forever!"));
                                player.BankBox.AddItem(new DonationCoin{Amount = CointAmount});
                                SelectedDonationProfile.TotalDonations += CashAmount;
                                SelectedDonationProfile.DonationEntries.Add(new DonationEntry(CharacterName, CashAmount, CointAmount, DateTime.Now));
                                Refresh(true);
                                if (CashAmount == 0)
                                {
                                    Send(
                                        new NoticeDialogGump(User, Hide(true))
                                        {
                                            Title = "Donation Was Successful!",
                                            Html = "The donation was successful.  The specified character has been given their donation coins.",
                                            AcceptHandler = a => Refresh(true)
                                        });
                                }
                            }
                        });

                        AddTextEntryLimited(73, 238, 161, 24, TextHue, NewProfileEmail, 100,
                            (b, t) => NewProfileEmail = t);


                        AddLabel(338, 355, 0, @"Donation Tiers");
                    }
                });


            Dictionary<int, DonationProfile> range = GetListRange();

            if (range.Count > 0)
            {
                CompileEntryLayout(layout, range);
            }
        }

        protected override void CompileEntryLayout(
            SuperGumpLayout layout, int length, int index, int pIndex, int yOffset, DonationProfile entry)
        {
            yOffset = 293 + pIndex * 23;

            layout.Add(
                "entry" + index,
                () =>
                {
                    AddLabel(78, yOffset, 0, entry.Email);
                    AddButton(263, yOffset - 2, 4023, 4024, b =>
                    {
                        SelectedDonationProfile = entry;
                        Refresh(true);
                    });
                });
        }
    }
}