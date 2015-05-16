#region References

using System;
using System.Drawing;
using System.Linq;
using System.Text;
using Server.Accounting;
using Server.Engines.Conquests;
using Server.Engines.CustomTitles;
using Server.Engines.EventScores;
using Server.Mobiles;
using VitaNex.Modules.AutoPvP;
using VitaNex.SuperGumps;
using VitaNex.SuperGumps.UI;
using VitaNex.Text;

#endregion

namespace Server.Engines.CentralGump
{
    public sealed class CentralGumpUI : ConfirmDialogGump
    {
        private readonly CentralGumpProfile Profile;

        private CentralGumpType GumpType;

        private readonly string News;

        public CentralGumpUI(PlayerMobile user, CentralGumpProfile profile, CentralGumpType type)
            : base(user, null, 115, 0)
        {
            Profile = profile;

            GumpType = type;

            CanMove = true;
            Modal = false;

            var html = new StringBuilder();
            if (String.IsNullOrEmpty(News))
            {
                html.Append("<CENTER><BIG>Welcome to Ultima Online Forever!</BIG>\n");
                html.Append(
                    "<a href=\"http://www.uoforever.com\">UOForever Website</a> - <a href=\"http://www.uoforum.com\">UOForever Forums</a> - <a href=\"http://www.uoforever.com/chat/\">UOForever Chat</a>\n");
                html.Append(
                    "Forum News and Announcements - <a href=\"http://www.uoforum.com/forums/news.4/\">News and Announcements</a>\n");
                html.Append(
                    "Looking for tips or mechanic info? - <a href=\"http://www.uofwiki.com\">Check Out the UOF Wiki</a>\n");
                html.Append(
                    "Keep UO Forever the best shard in the world - <a href=\"http://www.uoforever.com/donate.php\">Donate Today</a></CENTER>");
                foreach (Message message in CentralGump.Messages.Values.OrderByDescending(x => x.Date))
                {
                    html.Append(message.Title);
                    html.Append(message.Content + "\n");
                }
            }

            News = html.ToString().ParseBBCode(Color.GhostWhite);
        }

        protected override void CompileLayout(SuperGumpLayout layout)
        {
            layout.Add(
                "background",
                () =>
                {
                    AddImageTiled(40, 80, 44, 458, 202);
                    AddImage(40, 39, 206);
                    AddImageTiled(80, 38, 422, 45, 201);
                    AddImage(40, 538, 204);
                    AddImageTiled(495, 71, 44, 469, 203);
                    AddImage(496, 39, 207);
                    AddImageTiled(84, 539, 417, 43, 233);
                    AddImageTiled(75, 82, 446, 459, 200);
                    AddImage(171, 16, 1419);
                    AddImage(248, -1, 1417);
                    AddImage(257, 8, 5545);
                    AddImage(496, 538, 205);
                });

            layout.Add("SideButtons",
                () =>
                {
                    if (GumpType == CentralGumpType.News)
                    {
                        AddButton(0, 87, 223, 223, b =>
                        {
                            GumpType = CentralGumpType.News;
                            Refresh(true);
                        });
                    }
                    else
                    {
                        AddButton(0, 87, 224, 223, b =>
                        {
                            GumpType = CentralGumpType.News;
                            Refresh(true);
                        });
                    }

                    if (GumpType == CentralGumpType.Links)
                    {
                        AddButton(0, 154, 221, 221, b =>
                        {
                            GumpType = CentralGumpType.Links;
                            Refresh(true);
                        });
                    }
                    else
                    {
                        AddButton(0, 154, 222, 221, b =>
                        {
                            GumpType = CentralGumpType.Links;
                            Refresh(true);
                        });
                    }

                    if (GumpType == CentralGumpType.Information)
                    {
                        AddButton(0, 286, 219, 219, b =>
                        {
                            GumpType = CentralGumpType.Information;
                            Refresh(true);
                        });
                    }
                    else
                    {
                        AddButton(0, 286, 220, 219, b =>
                        {
                            GumpType = CentralGumpType.Information;
                            Refresh(true);
                        });
                    }

                    if (GumpType == CentralGumpType.Commands)
                    {
                        AddButton(0, 352, 217, 217, b =>
                        {
                            GumpType = CentralGumpType.Commands;
                            Refresh(true);
                        });
                    }
                    else
                    {
                        AddButton(0, 352, 218, 217, b =>
                        {
                            GumpType = CentralGumpType.Commands;
                            Refresh(true);
                        });
                    }

                    if (GumpType == CentralGumpType.Profile)
                    {
                        AddButton(525, 87, 229, 229, b =>
                        {
                            GumpType = CentralGumpType.Profile;
                            Refresh(true);
                        });
                    }
                    else
                    {
                        AddButton(525, 87, 230, 229, b =>
                        {
                            GumpType = CentralGumpType.Profile;
                            Refresh(true);
                        });
                    }

                    if (GumpType == CentralGumpType.Options)
                    {
                        AddButton(525, 154, 231, 231, b =>
                        {
                            GumpType = CentralGumpType.Options;
                            Refresh(true);
                        });
                    }
                    else
                    {
                        AddButton(525, 154, 232, 231, b =>
                        {
                            GumpType = CentralGumpType.Options;
                            Refresh(true);
                        });
                    }

                    if (GumpType == CentralGumpType.Events)
                    {
                        AddButton(525, 286, 227, 227, b =>
                        {
                            GumpType = CentralGumpType.Events;
                            Refresh(true);
                        });
                    }
                    else
                    {
                        AddButton(525, 286, 228, 227, b =>
                        {
                            GumpType = CentralGumpType.Events;
                            Refresh(true);
                        });
                    }

                        AddButton(525, 352, 235, 234, b =>
                        {
                            new CentralGumpUIList(User, Profile, X, Y).Send();
                        });
                });

            switch (GumpType)
            {
                case CentralGumpType.News:
                    layout.Add("News",
                        () =>
                        {
                            AddImageTiled(81, 92, 420, 12, 50);
                            AddHtml(126, 74, 325, 17,
                                String.Format("<BIG>{0}</BIG>", "Ultima Online: Forever News & Patch Notes")
                                    .WrapUOHtmlColor(KnownColor.GhostWhite), false, false);

                            AddImageTiled(81, 104, 404, 413, 2624);
                            AddHtml(86, 103, 414, 413, News, false, true);

                            AddImageTiled(81, 518, 420, 12, 50);

                            AddImage(97, 530, 9004);
                            AddHtml(180, 541, 129, 18,
                                String.Format("<BIG>{0}</BIG>", "Receive on login?")
                                    .WrapUOHtmlColor(KnownColor.GhostWhite),
                                false, false);
                            AddCheck(147, 536, 2152, 2153, Profile.LoginGump, (b, t) => { Profile.LoginGump = t; });


                            if (User.AccessLevel >= AccessLevel.GameMaster)
                            {
                                AddHtml(385, 541, 113, 18,
                                    String.Format("<BIG>{0}</BIG>", "Modify News?")
                                        .WrapUOHtmlColor(KnownColor.GhostWhite),
                                    false, false);
                                AddButton(351, 541, 4005, 4006, b => { Send(new NewsMessageListGump(User, Hide())); });
                            }
                        });
                    break;
                case CentralGumpType.Links:
                    layout.Add("Links",
                        () =>
                        {
                            AddImageTiled(81, 92, 420, 12, 50);
                            AddHtml(134, 74, 313, 18,
                                String.Format("<BIG>{0}</BIG>", "Ultima Online: Forever - Important Links")
                                    .WrapUOHtmlColor(KnownColor.GhostWhite), false, false);

                            AddHtml(135, 115, 400, 107,
                                String.Format("<BIG>{0}</BIG>", "Ultima Online Forever Homepage")
                                    .WrapUOHtmlColor(KnownColor.GhostWhite), false, false);
                            AddButton(99, 115, 4005, 4006, b =>
                            {
                                User.LaunchBrowser("http://www.uoforever.com");
                                Refresh();
                            });

                            AddHtml(135, 160, 400, 107,
                                String.Format("<BIG>{0}</BIG>", "Ultima Online Forever Donation Page")
                                    .WrapUOHtmlColor(KnownColor.GhostWhite), false, false);
                            AddButton(99, 160, 4005, 4006, b =>
                            {
                                User.LaunchBrowser("http://uoforever.com/donate.php");
                                Refresh();
                            });

                            AddHtml(135, 205, 400, 107,
                                String.Format("<BIG>{0}</BIG>", "Ultima Online Forever Forums")
                                    .WrapUOHtmlColor(KnownColor.GhostWhite), false, false);
                            AddButton(99, 205, 4005, 4006, b =>
                            {
                                User.LaunchBrowser("http://www.uoforum.com/");
                                Refresh();
                            });

                            AddHtml(135, 250, 400, 107,
                                String.Format("<BIG>{0}</BIG>", "Ultima Online Forever Chatroom [IRC]")
                                    .WrapUOHtmlColor(KnownColor.GhostWhite), false, false);
                            AddButton(99, 250, 4005, 4006, b =>
                            {
                                User.LaunchBrowser("http://uoforever.com/chat/");
                                Refresh();
                            });

                            AddHtml(135, 295, 400, 107,
                                String.Format("<BIG>{0}</BIG>", "Ultima Online Forever Wiki")
                                    .WrapUOHtmlColor(KnownColor.GhostWhite), false, false);

                            AddButton(99, 295, 4005, 4006, b =>
                            {
                                User.LaunchBrowser("http://www.uofwiki.com");
                                Refresh();
                            });

                            AddHtml(135, 340, 400, 107,
                                String.Format("<BIG>{0}</BIG>", "Ultima Online Forever Frequently Asked Questions")
                                    .WrapUOHtmlColor(KnownColor.GhostWhite), false, false);
                            AddButton(99, 340, 4005, 4006, b =>
                            {
                                User.LaunchBrowser("http://www.uoforum.com/faq/");
                                Refresh();
                            });

                            AddHtml(135, 385, 400, 107,
                                String.Format("<BIG>{0}</BIG>", "Ultima Online Forever: Legends")
                                    .WrapUOHtmlColor(KnownColor.GhostWhite), false, false);
                            AddButton(99, 385, 4005, 4006, b =>
                            {
                                User.LaunchBrowser("http://legends.uoforever.com");
                                Refresh();
                            });

                            AddHtml(135, 430, 400, 107,
                                String.Format("<BIG>{0}</BIG>", "Ultima Online Forever Facebook Page")
                                    .WrapUOHtmlColor(KnownColor.GhostWhite), false, false);

                            AddButton(99, 430, 4005, 4006, b =>
                            {
                                User.LaunchBrowser("https://www.facebook.com/UltimaOnlineForever");
                                Refresh();
                            });

                            AddHtml(135, 475, 400, 107,
                                String.Format("<BIG>{0}</BIG>", "Ultima Online Forever Action Camera")
                                    .WrapUOHtmlColor(KnownColor.GhostWhite), false, false);
                            AddButton(99, 475, 4005, 4006, b =>
                            {
                                User.LaunchBrowser("http://live.uoforever.com");
                                Refresh();
                            });

                            AddImageTiled(81, 518, 420, 12, 50);
                        });
                    break;
                case CentralGumpType.Information:
                    layout.Add("Information",
                        () =>
                        {
                            AddImageTiled(81, 92, 420, 12, 50);


                            AddHtml(124, 74, 513, 18,
                                String.Format("<BIG>{0}</BIG>", "Ultima Online: Forever - Information and Options")
                                    .WrapUOHtmlColor(KnownColor.GhostWhite), false, false);

                            AddLabel(85, 109, 2049, "Young Program");


                            AddLabel(120, 130, 2049, "General Information");
                            AddButton(85, 130, 4005, 4007, b =>
                            {
                                new CentralGumpInformationUI(User, Hide(), CentralGump.ConstructYoungBenefitsString())
                                    .Send();
                            });

                            AddLabel(120, 155, 2049, "Leaving the Program");
                            AddButton(85, 155, 4005, 4007, b =>
                            {
                                new CentralGumpInformationUI(User, Hide(), CentralGump.ConstructYoungLeaveString()).Send
                                    ();
                            });

                            AddLabel(120, 180, 2049, "The Defiled Crypt");
                            AddButton(85, 180, 4005, 4007, b =>
                            {
                                new CentralGumpInformationUI(User, Hide(), CentralGump.ConstructYoungDungeonString())
                                    .Send();
                            });

                            AddLabel(120, 205, 2049, "Companion Program");
                            AddButton(85, 205, 4005, 4007, b =>
                            {
                                new CentralGumpInformationUI(User, Hide(), CentralGump.ConstructYoungDungeonString())
                                    .Send();
                            });


                            AddLabel(85, 228, 2049, "Shopkeeper/NPC Information");


                            AddLabel(120, 250, 2049, "Banks");
                            AddButton(85, 250, 4005, 4007,
                                b =>
                                {
                                    new CentralGumpInformationUI(User, Hide(), CentralGump.ConstructBankString()).Send();
                                });

                            AddLabel(120, 275, 2049, "Bardic Guild");
                            AddButton(85, 275, 4005, 4007, b =>
                            {
                                new CentralGumpInformationUI(User, Hide(), CentralGump.ConstructBardicGuildString())
                                    .Send();
                            });


                            AddLabel(120, 300, 2049, "The Blacksmith");
                            AddButton(85, 300, 4005, 4007, b =>
                            {
                                new CentralGumpInformationUI(User, Hide(), CentralGump.ConstructBlacksmithGuildString())
                                    .Send();
                            });


                            AddLabel(120, 325, 2049, "Carpenter's Guild");
                            AddButton(85, 325, 4005, 4007, b =>
                            {
                                new CentralGumpInformationUI(User, Hide(), CentralGump.ConstructLumberjackGuildString())
                                    .Send();
                            });

                            AddLabel(120, 350, 2049, "The Docks");
                            AddButton(85, 350, 4005, 4007,
                                b =>
                                {
                                    new CentralGumpInformationUI(User, Hide(), CentralGump.ConstructDocksString()).Send();
                                });


                            AddLabel(120, 375, 2049, "Healers");
                            AddButton(85, 375, 4005, 4007, b =>
                            {
                                new CentralGumpInformationUI(User, Hide(), CentralGump.ConstructHealersGuildString())
                                    .Send();
                            });


                            AddLabel(120, 400, 2049, "Inns");
                            AddButton(85, 400, 4005, 4007,
                                b =>
                                {
                                    new CentralGumpInformationUI(User, Hide(), CentralGump.ConstructInnGuildString())
                                        .Send();
                                });


                            AddLabel(120, 425, 2049, "Mage's Guild");
                            AddButton(85, 425, 4005, 4007,
                                b =>
                                {
                                    new CentralGumpInformationUI(User, Hide(), CentralGump.ConstructMageGuildString())
                                        .Send();
                                });


                            AddLabel(120, 450, 2049, "Miner's Guild");
                            AddButton(85, 450, 4005, 4007, b =>
                            {
                                new CentralGumpInformationUI(User, Hide(), CentralGump.ConstructMinersGuildString())
                                    .Send();
                            });


                            AddLabel(120, 475, 2049, "Stables");
                            AddButton(85, 475, 4005, 4007, b =>
                            {
                                new CentralGumpInformationUI(User, Hide(), CentralGump.ConstructStablesGuildString())
                                    .Send();
                            });


                            AddLabel(120, 500, 2049, "Tailor Shop");
                            AddButton(85, 500, 4005, 4007, b =>
                            {
                                new CentralGumpInformationUI(User, Hide(), CentralGump.ConstructTailorGuildString())
                                    .Send();
                            });

                            AddLabel(315, 109, 2049, "General Systems Information");

                            AddLabel(350, 130, 2049, "Battles System");
                            AddButton(315, 130, 4005, 4007,
                                b =>
                                {
                                    new CentralGumpInformationUI(User, Hide(), CentralGump.ConstructBattlesString())
                                        .Send();
                                });

                            AddLabel(350, 155, 2049, "Conquest System");
                            AddButton(315, 155, 4005, 4007, b =>
                            {
                                new CentralGumpInformationUI(User, Hide(), CentralGump.ConstructYoungDungeonString())
                                    .Send();
                            });

                            AddLabel(350, 180, 2049, "Event Scoring System");
                            AddButton(315, 180, 4005, 4007, b =>
                            {
                                new CentralGumpInformationUI(User, Hide(), CentralGump.ConstructYoungDungeonString())
                                    .Send();
                            });

                            AddLabel(350, 205, 2049, "Invasion System");
                            AddButton(315, 205, 4005, 4007, b =>
                            {
                                new CentralGumpInformationUI(User, Hide(), CentralGump.ConstructYoungDungeonString())
                                    .Send();
                            });

                            AddLabel(350, 230, 2049, "Player Scoring System");
                            AddButton(315, 230, 4005, 4007, b =>
                            {
                                new CentralGumpInformationUI(User, Hide(), CentralGump.ConstructYoungDungeonString())
                                    .Send();
                            });

                            AddLabel(350, 255, 2049, "Portals System");
                            AddButton(315, 255, 4005, 4007, b =>
                            {
                                new CentralGumpInformationUI(User, Hide(), CentralGump.ConstructYoungDungeonString())
                                    .Send();
                            });

                            AddLabel(315, 280, 2049, "Housing Information");

                            AddLabel(350, 300, 2049, "Classic Houses");
                            AddButton(315, 300, 4005, 4007, b =>
                            {
                                new CentralGumpInformationUI(User, Hide(), CentralGump.ConstructYoungDungeonString())
                                    .Send();
                            });

                            AddLabel(350, 325, 2049, "Custom Houses");
                            AddButton(315, 325, 4005, 4007, b =>
                            {
                                new CentralGumpInformationUI(User, Hide(), CentralGump.ConstructYoungDungeonString())
                                    .Send();
                            });

                            AddLabel(350, 350, 2049, "House Ownership");
                            AddButton(315, 350, 4005, 4007, b =>
                            {
                                new CentralGumpInformationUI(User, Hide(), CentralGump.ConstructYoungDungeonString())
                                    .Send();
                            });

                            AddLabel(350, 375, 2049, "Player Run Vendors");
                            AddButton(315, 375, 4005, 4007, b =>
                            {
                                new CentralGumpInformationUI(User, Hide(), CentralGump.ConstructYoungDungeonString())
                                    .Send();
                            });

                            AddLabel(350, 475, User.GuildMessageHue, "Guild Chat Color");
                            AddButton(315, 475, 4005, 4007,
                                b => { new CentralGump.GuildChatColorPicker(User, Profile).SendTo(User.NetState); });

                            AddLabel(350, 500, User.AllianceMessageHue, "Alliance Chat Color");
                            AddButton(315, 500, 4005, 4007,
                                b => { new CentralGump.AllianceChatColorPicker(User, Profile).SendTo(User.NetState); });


                            AddImageTiled(81, 523, 420, 12, 50);
                        });
                    break;
                case CentralGumpType.Commands:
                    layout.Add("Commands",
                        () =>
                        {
                            AddHtml(81, 74, 513, 18,
                                String.Format("<BIG>{0}</BIG>", "Ultima Online: Forever - PvP Commands and Notifications")
                                    .WrapUOHtmlColor(KnownColor.GhostWhite), false, false);

                            AddImageTiled(81, 92, 420, 12, 50);

                            AddCheck(60, 120, 2152, 2153, Profile.BuyHead,
                                (b, t) => { Profile.BuyHead = t; });
                            AddHtml(95, 125, 221, 18,
                                String.Format("<BIG>{0}</BIG>", "Buy head for 30k?")
                                    .WrapUOHtmlColor(KnownColor.GhostWhite), false, false);

                            AddCheck(283, 120, 2152, 2153, Profile.FactionPoint,
                                (b, t) => { Profile.FactionPoint = t; });
                            AddHtml(318, 125, 204, 18,
                                String.Format("<BIG>{0}</BIG>", "Ignore Faction Notifications?")
                                    .WrapUOHtmlColor(KnownColor.GhostWhite), false, false);

                            AddImageTiled(81, 518, 420, 12, 50);
                        });
                    break;
                case CentralGumpType.Profile:
                    layout.Add("Profile",
                        () =>
                        {
                            AddImageTiled(81, 92, 420, 12, 50);
                            AddHtml(140, 74, 313, 18,
                                String.Format("<BIG>{0}</BIG>", "Ultima Online: Forever - Player Profile")
                                    .WrapUOHtmlColor(KnownColor.GhostWhite), false, false);

                            AddHtml(79, 110, 186, 18,
                                String.Format("<BIG>{0}</BIG>", "Total Account Play Time")
                                    .WrapUOHtmlColor(KnownColor.GhostWhite), false, false);

                            TimeSpan totaltime = TimeSpan.Zero;
                            var account = User.Account as Account;

                            if (account != null)
                            {
                                totaltime =
                                    account.Mobiles.Cast<PlayerMobile>()
                                        .Where(player => player != null)
                                        .Aggregate(totaltime, (current, player) => current + player.GameTime);
                            }

                            AddHtml(79, 135, 186, 18,
                                String.Format("<BIG>{0} Days</BIG>", (int) Math.Ceiling(totaltime.TotalDays))
                                    .WrapUOHtmlColor(KnownColor.GhostWhite), false, false);

                            AddHtml(345, 110, 175, 18,
                                String.Format("<BIG>{0}</BIG>", "Character Play Time")
                                    .WrapUOHtmlColor(KnownColor.GhostWhite), false, false);

                            AddHtml(345, 135, 130, 18,
                                String.Format("<BIG>{0} Days</BIG>", (int) Math.Ceiling(User.GameTime.TotalDays))
                                    .WrapUOHtmlColor(KnownColor.GhostWhite), false, false);

                            AddHtml(79, 180, 105, 107,
                                String.Format("<BIG>{0}</BIG>", "Current Title")
                                    .WrapUOHtmlColor(KnownColor.GhostWhite), false, false);

                            Title t;
                            TitleHue h;
                            CustomTitles.CustomTitles.GetTitle(User, out t);
                            CustomTitles.CustomTitles.GetTitleHue(User, out h);

                            if (t != null)
                            {
                                /*AddHtml(79, 205, 105, 107,
                                    String.Format("<BIG>{0}</BIG>", User.Female ? t.FemaleTitle : t.MaleTitle)
                                        .WrapUOHtmlColor(KnownColor.GhostWhite), false, false);*/
                                AddLabel(79, 205, h != null ? h.Hue-1 : 0, User.Female ? t.FemaleTitle : t.MaleTitle);
                            }
                            else
                            {
                                AddHtml(79, 205, 105, 107,
                                    String.Format("<BIG>{0}</BIG>", "No title to display.")
                                        .WrapUOHtmlColor(KnownColor.GhostWhite), false, false);
                            }

                            AddHtml(345, 180, 138, 17,
                                String.Format("<BIG>{0}</BIG>", "Current Title Hue")
                                    .WrapUOHtmlColor(KnownColor.GhostWhite), false, false);

                            if (t != null && h != null)
                            {
                                /*AddHtml(345, 205, 105, 107,
                                    String.Format("<BIG>{0}</BIG>", h.Hue)
                                        .WrapUOHtmlColor(KnownColor.GhostWhite), false, false);*/
                                AddLabel(345, 205, h.Hue-1, h.Hue.ToString());
                            }
                            else
                            {
                                AddHtml(345, 205, 105, 107,
                                    String.Format("<BIG>{0}</BIG>", "No title hue to display.")
                                        .WrapUOHtmlColor(KnownColor.GhostWhite), false, false);
                            }

                            AddHtml(79, 250, 121, 18,
                                String.Format("<BIG>{0}</BIG>", "Conquest Points")
                                    .WrapUOHtmlColor(KnownColor.GhostWhite), false, false);

                            ConquestProfile conquestprofile = Conquests.Conquests.EnsureProfile(User);
                            long points = conquestprofile.GetPointsTotal();

                            if (points == 0)
                            {
                                AddHtml(79, 275, 171, 107,
                                    String.Format("<BIG>{0}</BIG>", "You have not completed any conquests.")
                                        .WrapUOHtmlColor(KnownColor.GhostWhite), false, false);
                            }
                            else
                            {
                                AddHtml(79, 275, 121, 18,
                                    String.Format("<BIG>{0}</BIG>", points)
                                        .WrapUOHtmlColor(KnownColor.GhostWhite), false, false);
                            }

                            AddHtml(345, 250, 171, 107,
                                String.Format("<BIG>{0}</BIG>", "Invasion Points")
                                    .WrapUOHtmlColor(KnownColor.GhostWhite), false, false);

                            AddHtml(79, 320, 171, 107,
                                String.Format("<BIG>{0}</BIG>", "Event Points")
                                    .WrapUOHtmlColor(KnownColor.GhostWhite), false, false);

                            PlayerEventScoreProfile eventprofile = EventScores.EventScores.EnsureProfile(User);
                            points = eventprofile.OverallScore;

                            if (points == 0)
                            {
                                AddHtml(79, 345, 171, 107,
                                    String.Format("<BIG>{0}</BIG>", "You have not competed in any events.")
                                        .WrapUOHtmlColor(KnownColor.GhostWhite), false, false);
                            }
                            else
                            {
                                AddHtml(79, 345, 121, 18,
                                    String.Format("<BIG>{0}</BIG>", points)
                                        .WrapUOHtmlColor(KnownColor.GhostWhite), false, false);
                            }

                            AddHtml(345, 320, 111, 17,
                                String.Format("<BIG>{0}</BIG>", "Portals Points")
                                    .WrapUOHtmlColor(KnownColor.GhostWhite), false, false);

                            AddImageTiled(81, 393, 420, 12, 50);

                            AddButton(79, 415, 4005, 4006,
                                b => { Send(new PvPProfileOverviewGump(User, AutoPvP.EnsureProfile(User), Hide())); });

                            AddHtml(117, 415, 111, 17,
                                String.Format("<BIG>{0}</BIG>", "Battles Profile")
                                    .WrapUOHtmlColor(KnownColor.GhostWhite), false, false);

                            AddButton(79, 460, 4005, 4006,
                                b => { Send(new ConquestStatesGump(User, Hide(), conquestprofile)); });

                            AddHtml(117, 460, 111, 17,
                                String.Format("<BIG>{0}</BIG>", "Conquests Profile")
                                    .WrapUOHtmlColor(KnownColor.GhostWhite), false, false);

                            AddButton(79, 505, 4005, 4006, b => { Send(new BattlesTUI(User, Hide(), eventprofile)); });

                            AddHtml(117, 505, 111, 17,
                                String.Format("<BIG>{0}</BIG>", "Events Profile")
                                    .WrapUOHtmlColor(KnownColor.GhostWhite), false, false);

                            AddButton(345, 415, 4005, 4006, b => { Refresh(); });

                            AddHtml(383, 415, 111, 17,
                                String.Format("<BIG>{0}</BIG>", "Invasions Profile")
                                    .WrapUOHtmlColor(KnownColor.GhostWhite), false, false);

                            AddButton(345, 460, 4005, 4006, b =>
                            {
                                User.LaunchBrowser("http://uoforever.com/legends/?action=player&s=" + User.Serial.Value);
                                Refresh();
                            });

                            AddHtml(383, 460, 111, 17,
                                String.Format("<BIG>{0}</BIG>", "Legends Profile")
                                    .WrapUOHtmlColor(KnownColor.GhostWhite), false, false);

                            AddButton(345, 505, 4005, 4006, b => { Refresh(); });

                            AddHtml(383, 505, 111, 17,
                                String.Format("<BIG>{0}</BIG>", "Portals Profile")
                                    .WrapUOHtmlColor(KnownColor.GhostWhite), false, false);
                        });
                    break;
                case CentralGumpType.Options:
                    layout.Add("Options",
                        () =>
                        {
                            AddImageTiled(81, 92, 420, 12, 50);


                            AddHtml(119, 74, 513, 18,
                                String.Format("<BIG>{0}</BIG>", "Ultima Online: Forever - Player Profile Options")
                                    .WrapUOHtmlColor(KnownColor.GhostWhite), false, false);


                            AddCheck(60, 120, 2152, 2153, Profile.IgnoreBattles,
                                (b, t) => { Profile.IgnoreBattles = t; });
                            AddHtml(95, 125, 221, 18,
                                String.Format("<BIG>{0}</BIG>", "Ignore Battles Prompts?")
                                    .WrapUOHtmlColor(KnownColor.GhostWhite), false, false);

                            AddCheck(283, 120, 2152, 2153, Profile.IgnoreNotifies,
                                (b, t) => { Profile.IgnoreNotifies = t; });
                            AddHtml(318, 125, 204, 18,
                                String.Format("<BIG>{0}</BIG>", "Ignore Staff Notifications?")
                                    .WrapUOHtmlColor(KnownColor.GhostWhite), false, false);

                            AddCheck(60, 190, 2152, 2153, Profile.IgnoreTournament,
                                (b, t) => { Profile.IgnoreTournament = t; });
                            AddHtml(95, 195, 256, 18,
                                String.Format("<BIG>{0}</BIG>", "Ignore Tourny Prompts?")
                                    .WrapUOHtmlColor(KnownColor.GhostWhite), false, false);

                            AddCheck(283, 190, 2152, 2153, Profile.IgnoreConquests,
                                (b, t) => { Profile.IgnoreConquests = t; });
                            AddHtml(318, 195, 232, 18,
                                String.Format("<BIG>{0}</BIG>", "Ignore Conquest Notification?")
                                    .WrapUOHtmlColor(KnownColor.GhostWhite), false, false);

                            AddCheck(60, 260, 2152, 2153, Profile.IgnoreMoongates,
                                (b, t) => { Profile.IgnoreMoongates = t; });
                            AddHtml(95, 265, 202, 18,
                                String.Format("<BIG>{0}</BIG>", "Ignore Moongate Prompt?")
                                    .WrapUOHtmlColor(KnownColor.GhostWhite), false, false);

                            AddCheck(283, 260, 2152, 2153, Profile.PublicLegends,
                                (b, t) => { Profile.PublicLegends = t; });
                            AddHtml(318, 265, 202, 18,
                                String.Format("<BIG>{0}</BIG>", "Show UOF Legends?")
                                    .WrapUOHtmlColor(KnownColor.GhostWhite), false, false);

                            /*AddCheck(283, 260, 2152, 2153, Profile.IgnoreMoongates,
                                (b, t) => { Profile.IgnoreMoongates = t; });
                            AddHtml(318, 265, 250, 18,
                                String.Format("<BIG>{0}</BIG>", "Auto Buy Head for 30k?")
                                    .WrapUOHtmlColor(KnownColor.GhostWhite), false, false);*/

                            AddCheck(60, 330, 2152, 2153, Profile.FameTitle, (b, t) => { Profile.FameTitle = t; });
                            AddHtml(95, 335, 155, 18,
                                String.Format("<BIG>{0}</BIG>", "Display Fame Title?")
                                    .WrapUOHtmlColor(KnownColor.GhostWhite), false, false);

                            AddCheck(283, 330, 2152, 2153, User.KarmaLocked, (b, t) => { User.KarmaLocked = t; });
                            AddHtml(318, 335, 147, 20,
                                String.Format("<BIG>{0}</BIG>", "Lock Your Karma?")
                                    .WrapUOHtmlColor(KnownColor.GhostWhite), false, false);

                            AddCheck(60, 400, 2152, 2153, User.DisplayChampionTitle,
                                (b, t) => { User.DisplayChampionTitle = t; });
                            AddHtml(95, 405, 186, 18,
                                String.Format("<BIG>{0}</BIG>", "Display Champion Title?")
                                    .WrapUOHtmlColor(KnownColor.GhostWhite), false, false);

                            AddCheck(283, 400, 2152, 2153, Profile.MiniGump, (b, t) => { Profile.MiniGump = t; });
                            AddHtml(318, 405, 186, 18,
                                String.Format("<BIG>{0}</BIG>", "Display Mini Score Gump")
                                    .WrapUOHtmlColor(KnownColor.GhostWhite), false, false);

                            AddCheck(60, 470, 2152, 2153, Profile.DisablePvPTemplate,
                                (b, t) => { Profile.DisablePvPTemplate = t; });
                            AddHtml(95, 475, 183, 18,
                                String.Format("<BIG>{0}</BIG>", "Disable PvP Templates?")
                                    .WrapUOHtmlColor(KnownColor.GhostWhite), false, false);

                            AddCheck(283, 470, 2152, 2153, Profile.DisableTemplateEquipment,
                                (b, t) => { Profile.DisableTemplateEquipment = t; });
                            AddHtml(318, 475, 255, 18,
                                String.Format("<BIG>{0}</BIG>", "Disable Template Equipment?")
                                    .WrapUOHtmlColor(KnownColor.GhostWhite), false, false);


                            AddImageTiled(81, 518, 420, 12, 50);
                        });
                    break;

                case CentralGumpType.Events:
                    layout.Add("Events",
                        () =>
                        {
                            AddHtml(154, 74, 513, 18,
                                String.Format("<BIG>{0}</BIG>", "Ultima Online: Forever - Events Page")
                                    .WrapUOHtmlColor(KnownColor.GhostWhite), false, false);

                            AddImageTiled(81, 92, 420, 12, 50);
                            AddHtml(85, 109, 325, 17,
                                String.Format("<BIG>{0}</BIG>", "There are currently no events at this time.")
                                    .WrapUOHtmlColor(KnownColor.GhostWhite), false, false);

                            AddImageTiled(81, 518, 420, 12, 50);
                        });
                    break;
            }
        }
    }
}