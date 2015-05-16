#region References

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Server.Engines.DonationsTracker;
using Server.Engines.ZombieEvent;
using Server.Gumps;
using Server.Misc;
using Server.Mobiles;
using VitaNex;
using VitaNex.SuperGumps;
using VitaNex.SuperGumps.UI;

#endregion

namespace Server.Engines.ZombieEvent
{
    public sealed class ZombieEventUI : ListGump<PlayerZombieProfile>
    {
        public PlayerZombieProfile SelectedProfile { get; set; }

        public PlayerZombieProfile UserProfile { get; set; }

        public ZombieAvatar Avatar { get; set; }

        public ZombieInstance ZEvent { get; set; }

        public string SearchEmail { get; set; }

        public List<PlayerZombieProfile> Profiles;

        public Action<GumpButton> AcceptHandler { get; set; }
        public Action<GumpButton> CancelHandler { get; set; }

        public ZombieEventUI(PlayerMobile user, ZombieAvatar avatar, ZombieInstance zevent = null, PlayerZombieProfile zprofile = null,
            Action<GumpButton> onAccept = null, Action<GumpButton> onCancel = null)
            : base(user, null, 0, 0)
        {
            CanDispose = true;
            CanMove = true;
            Modal = false;
            ForceRecompile = true;

            AcceptHandler = onAccept;
            CancelHandler = onCancel;

            CanSearch = true;

            EntriesPerPage = 11;

            SelectedProfile = zprofile;
            ZEvent = zevent;
            Avatar = avatar;

            UserProfile = ZombieEvent.EnsureProfile(user);

            AutoRefreshRate = TimeSpan.FromSeconds(60);
            AutoRefresh = true;
        }

        public override string GetSearchKeyFor(PlayerZombieProfile key)
        {
            if (key != null)
            {
                return key.Owner.Name;
            }

            return base.GetSearchKeyFor(key);
        }

        public override SuperGump Send()
        {
            if (IsDisposed)
            {
                return this;
            }

            return VitaNexCore.TryCatchGet(
                () =>
                {
                    if (IsOpen)
                    {
                        InternalClose(this);
                    }

                    Compile();
                    Clear();

                    CompileLayout(Layout);
                    Layout.ApplyTo(this);

                    InvalidateOffsets();
                    InvalidateSize();

                    Compiled = true;

                    if (Modal && ModalSafety && Buttons.Count == 0 && TileButtons.Count == 0)
                    {
                        CanDispose = true;
                        CanClose = true;
                    }

                    OnBeforeSend();

                    Initialized = true;
                    IsOpen = Avatar != null ? Avatar.SendGump(this, false) : User.SendGump(this, false);
                    Hidden = false;

                    if (IsOpen)
                    {
                        OnSend();
                    }
                    else
                    {
                        OnSendFail();
                    }

                    return this;
                },
                e =>
                {
                    Console.WriteLine("SuperGump '{0}' could not be sent, an exception was caught:", GetType().FullName);
                    VitaNexCore.ToConsole(e);
                    IsOpen = false;
                    Hidden = false;
                    OnSendFail();
                }) ?? this;
        }

        protected override void CompileList(List<PlayerZombieProfile> list)
        {
            list.Clear();

            list.TrimExcess();
            if (Profiles == null)
            {
                Profiles = new List<PlayerZombieProfile>();
                Profiles.AddRange(ZombieEvent.SortedProfiles());
            }
            else
            {
                Profiles.Clear();
                Profiles.TrimExcess();
                Profiles.AddRange(ZombieEvent.SortedProfiles());
            }

            if (Profiles.Count == 0)
            {
                Profiles.AddRange(ZombieEvent.SortedProfiles());
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
                    AddBackground(0, 40, 775, 553, 2600);
                    AddImage(269, 18, 1419);
                    AddImage(346, 0, 1417);
                    AddImage(355, 9, 9012, 60);
                    AddBackground(35, 100, 703, 444, 9270);
                    AddBackground(49, 112, 676, 420, 9200);
                    AddBackground(56, 119, 362, 409, 9260);
                    AddBackground(423, 119, 296, 409, 9260);

                    AddLabel(47, 80, 60, @"Zombieland!!");
                    AddItem(146, 58, 9685);

                    AddLabel(528, 55, 2049, @"Current Participants:");
                    AddLabel(670, 55, 60, ZombieEvent.GetParticipantCount().ToString());

                    AddLabel(528, 75, 2049, @"Zombieland Rewards");
                    AddButton(660, 75, 247, 248, b =>
                    {
                        if (Avatar == null)
                            new ZombieEventRewardsUI(User, UserProfile).Send();
                        else
                        {
                            Avatar.SendMessage(61, "You cannot view rewards while participating in the event.  Please log back into your normal character first.");
                        }
                    });

                    AddItem(351, 130, 4476);
                    AddItem(324, 156, 7397, 1270);
                    AddItem(333, 154, 7393, 1270);
                    AddItem(329, 162, 3795);

                    var zinstance = ZombieEvent.GetInstance();

                    if (User.AccessLevel >= AccessLevel.GameMaster)
                    {
                        if (zinstance == null)
                        {
                            AddLabel(77, 551, 1270, "Start Event?");
                            AddButton(173, 549, 247, 248, b =>
                            {
                                var instance = new ZombieInstance();
                                ZombieEvent.ZombieEvents.Add(instance.Uid, instance);
                                instance.init();
                            });
                        }
                        else
                        {
                            AddLabel(77, 551, 1270, "Stop Event?");
                            AddButton(173, 549, 247, 248, b =>
                            {
                                zinstance.Stop();
                            });
                        }
                    }

                    if (zinstance != null && !UserProfile.Active)
                    {
                        AddLabel(529, 551, 1270, "Join the Event?");
                        AddButton(638, 549, 247, 248, b =>
                        {
                            if (User.Alive)
                                zinstance.JoinZombieInstance(User);
                            else
                            {
                                User.SendMessage(54, "You must be alive to join the event!");
                            }
                        });
                    }
                    else if (zinstance != null && UserProfile.Active)
                    {
                        AddLabel(529, 551, 1270, "Leave the Event?");
                        AddButton(638, 549, 247, 248, b =>
                        {
                            zinstance.LeaveZombieInstance(UserProfile);
                            /*if (UserProfile.LeaveEventTimer == null || !UserProfile.LeaveEventTimer.Running)
                                ZombieEvent.LeaveEvent(User);
                            else
                            {
                                new DialogGump(User, this, 0, 0, null, null, 7004, a =>
                                {
                                    if (User.Map == Map.ZombieLand)
                                    {
                                        if (ZombieEvent.GetInstance() != null)
                                        {
                                            var zevent = ZombieEvent.GetInstance();
                                            if (zevent != null && UserProfile.LeaveEventTimer != null && UserProfile.LeaveEventTimer.Running)
                                            {
                                                User.SendMessage(54, "You have succesfully left the event.");
                                                UserProfile.LeaveEventTimer.Stop();
                                                zevent.HandlePlayerDeathLeave(User, null);
                                            }
                                        }
                                    }
                                })
                                {
                                    Title = "Leave Event?",
                                    Html = "Are you sure you wish to leave the event before the leave timer finishes?  You will lose all your items and your current location will not be saved.",
                                    HtmlColor = DefaultHtmlColor
                                }.Send();*/
                        });
                    }

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
                    AddLabel(73, 203, 1258, @"Zombieland Profiles");
                    AddLabel(219, 203, 1258, @"Rank");
                    AddLabel(270, 203, 1258, @"Score");
                    AddLabel(326, 203, 1258, @"View Profile");

                    AddBackground(71, 222, 335, 259, 3000);

                    if (PageCount - 1 > Page)
                    {
                        AddButton(384, 491, 5601, 5605, NextPage);
                    }

                    if (Page > 0)
                    {
                        AddButton(75, 491, 5603, 5607, PreviousPage);
                    }
                });

            layout.Add(
                "SelectedProfile",
                () =>
                {
                    if (SelectedProfile != null && ZEvent != null)
                    {
                        AddLabel(440, 131, 1258, @"Character Name");
                        AddLabel(445, 150, 2049, SelectedProfile.Owner.RawName);

                        AddLabel(570, 131, 1258, @"Murders");
                        AddHtml(569, 151, 46, 22, ("<p align=\"center\">" + SelectedProfile.Kills).WrapUOHtmlColor(KnownColor.White), false, false);

                        AddLabel(654, 131, 1258, @"Deaths");
                        AddHtml(651, 151, 46, 22, ("<p align=\"center\">" + SelectedProfile.Deaths).WrapUOHtmlColor(KnownColor.White), false, false);

                        AddLabel(440, 179, 1258, @"Creature Kills");

                        AddLabel(460, 278, 1258, @"Tentacle");
                        AddHtml(463, 293, 46, 22, ("<p align=\"left\">" + SelectedProfile.GetTentacleKills(ZEvent.Uid)).WrapUOHtmlColor(KnownColor.White), false, false);
                        AddItem(445, 240, 9672);

                        AddLabel(439, 360, 1258, @"Zombie");
                        AddHtml(440, 375, 46, 22, ("<p align=\"left\">" + SelectedProfile.GetZombieKills(ZEvent.Uid)).WrapUOHtmlColor(KnownColor.White), false, false);
                        AddItem(450, 315, 9685);

                        AddLabel(630, 440, 1258, @"Dk. Creeper");
                        AddHtml(650, 455, 46, 22, ("<p align=\"right\">" + SelectedProfile.GetGoreFiendKills(ZEvent.Uid)).WrapUOHtmlColor(KnownColor.White), false, false);
                        AddItem(639, 394, 9773);

                        AddHtml(548, 497, 46, 22, ("<p align=\"center\">" + SelectedProfile.GetVitriolKills(ZEvent.Uid)).WrapUOHtmlColor(KnownColor.White), false, false);
                        AddItem(528, 431, 11650);
                        AddLabel(545, 482, 1258, @"Abomination");

                        AddLabel(529, 360, 137, @"Chaos Dragon");
                        AddHtml(548, 375, 46, 22, ("<p align=\"center\">" + SelectedProfile.DragonBossDamage).WrapUOHtmlColor(KnownColor.White), false, false);
                        AddItem(545, 302, 9780);

                        AddLabel(630, 281, 1258, @"Treefellow");
                        AddHtml(643, 294, 46, 22, ("<p align=\"right\">" + SelectedProfile.GetTreefellowKills(ZEvent.Uid)).WrapUOHtmlColor(KnownColor.White), false, false);
                        AddItem(639, 243, 9761);

                        AddLabel(661, 360, 1258, @"Daemon");
                        AddHtml(658, 374, 46, 22, ("<p align=\"right\">" + SelectedProfile.GetDaemonKills(ZEvent.Uid)).WrapUOHtmlColor(KnownColor.White), false, false);
                        AddItem(669, 315, 8403);

                        AddLabel(455, 440, 1258, @"Fey Warrior");
                        AddHtml(456, 455, 46, 22, ("<p align=\"left\">" + SelectedProfile.GetFeyKills(ZEvent.Uid)).WrapUOHtmlColor(KnownColor.White), false, false);
                        AddItem(454, 397, 9609);

                        AddLabel(528, 234, 1258, @"Zombie Spider");
                        AddHtml(548, 250, 46, 22, ("<p align=\"center\">" + SelectedProfile.GetSpiderKills(ZEvent.Uid)).WrapUOHtmlColor(KnownColor.White), false, false);
                        AddItem(536, 196, 9668);
                    }
                });

            Dictionary<int, PlayerZombieProfile> range = GetListRange();

            if (range.Count > 0 && ZEvent != null)
            {
                CompileEntryLayout(layout, range);
            }
        }

        protected override void CompileEntryLayout(
            SuperGumpLayout layout, int length, int index, int pIndex, int yOffset, PlayerZombieProfile entry)
        {
            yOffset = 226 + pIndex * 23;

            layout.Add(
                "entry" + index,
                () =>
                {
                    AddLabel(78, yOffset, 2049, entry.Owner.RawName);
                    AddLabel(219, yOffset, 2049, (ZombieEvent.SortedProfiles().IndexOf(entry) + 1).ToString());
                    AddLabel(270, yOffset, 2049, entry.OverallScore.ToString());
                    AddButton(346, yOffset - 2, 4023, 4024, b =>
                    {
                        SelectedProfile = entry;
                        Refresh(true);
                    });
                });
        }
    }
}