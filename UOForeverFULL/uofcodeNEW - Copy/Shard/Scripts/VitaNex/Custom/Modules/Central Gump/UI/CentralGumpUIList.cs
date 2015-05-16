#region References

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Server.Accounting;
using Server.Engines.Help;
using Server.Engines.PartySystem;
using Server.Guilds;
using Server.Gumps;
using Server.Mobiles;
using VitaNex.SuperGumps;
using VitaNex.SuperGumps.UI;
using HelpGump = Server.Mobiles.HelpGump;

#endregion

namespace Server.Engines.CentralGump
{
    public sealed class CentralGumpUIList : ListGump<PlayerMobile>
    {
        private readonly CentralGumpProfile Profile;

        public CentralGumpUIList(PlayerMobile user, CentralGumpProfile profile, int x, int y)
            : base(user, null, x, y)
        {
            CanMove = true;
            Modal = false;
            EntriesPerPage = 8;
            ForceRecompile = true;
            Profile = profile;
        }

        protected override void CompileList(List<PlayerMobile> list)
        {
            list.Clear();

            list.TrimExcess();

            list.AddRange(CentralGump.Adventurers);

            base.CompileList(list);
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

                    AddImageTiled(81, 92, 420, 12, 50);
                    AddImageTiled(81, 518, 420, 12, 50);
                });

            layout.Add("SideButtons",
                () =>
                {
                    AddButton(0, 87, 224, 223, b =>
                    {
                        new CentralGumpUI(User, CentralGump.EnsureProfile(User),
                            CentralGumpType.News).Send();
                    });

                    AddButton(0, 154, 222, 221, b =>
                    {
                        new CentralGumpUI(User, CentralGump.EnsureProfile(User),
                            CentralGumpType.Links).Send();
                    });


                    AddButton(0, 286, 220, 219, b =>
                    {
                        new CentralGumpUI(User, CentralGump.EnsureProfile(User),
                            CentralGumpType.Information).Send();
                    });


                    AddButton(0, 352, 218, 217, b =>
                    {
                        new CentralGumpUI(User, CentralGump.EnsureProfile(User),
                            CentralGumpType.Commands).Send();
                    });


                    AddButton(525, 87, 230, 229, b =>
                    {
                        new CentralGumpUI(User, CentralGump.EnsureProfile(User),
                            CentralGumpType.Profile).Send();
                    });


                    AddButton(525, 154, 232, 231, b =>
                    {
                        new CentralGumpUI(User, CentralGump.EnsureProfile(User),
                            CentralGumpType.Options).Send();
                    });


                    AddButton(525, 286, 228, 227, b =>
                    {
                        new CentralGumpUI(User, CentralGump.EnsureProfile(User),
                            CentralGumpType.Events).Send();
                    });

                    AddButton(525, 352, 234, 234, b => { Refresh(true); });
                });

            var acct = User.Account as Account;
            if (acct != null && acct.Young || User.Companion)
            {
                layout.Add("Young",
                    () =>
                    {
                        if (acct != null && acct.Young || User.AccessLevel >= AccessLevel.GameMaster)
                        {
                            AddHtml(115, 74, 513, 18,
                                String.Format("<BIG>{0}</BIG>", "Ultima Online: Forever - Young Player Program")
                                    .WrapUOHtmlColor(KnownColor.GhostWhite), false, false);

                            AddHtml(95, 112, 155, 17,
                                String.Format("<BIG>{0}</BIG>", "Time Left as Young")
                                    .WrapUOHtmlColor(KnownColor.GhostWhite), false, false);

                            AddHtml(95, 135, 165, 17,
                                String.Format("<BIG>{0}</BIG>",
                                    ((int) Math.Ceiling(acct.GetYoungTime().TotalHours) + "/" +
                                     (int) Math.Ceiling(Account.YoungDuration.TotalHours) + " Hours Left"))
                                    .WrapUOHtmlColor(KnownColor.GhostWhite), false, false);

                            AddHtml(345, 112, 155, 17,
                                String.Format("<BIG>{0}</BIG>", "Skill Points Left")
                                    .WrapUOHtmlColor(KnownColor.GhostWhite), false, false);

                            AddHtml(345, 135, 200, 25,
                                String.Format("<BIG>{0}</BIG>",
                                    600 - User.SkillsTotal / 10 + "/600" + " Skill Points")
                                    .WrapUOHtmlColor(KnownColor.GhostWhite), false, false);

                            AddHtml(95, 175, 390, 80,
                                String.Format(
                                    "<BIG>You have {0} hours left as a young player.  You will also reach mature status if you gain {1} more skill points.</BIG>",
                                    (int) Math.Ceiling(acct.GetYoungTime().TotalHours), 600 - (User.SkillsTotal / 10))
                                    .WrapUOHtmlColor(KnownColor.GhostWhite), false, false);


                            if (User.Guild == null)
                            {
                                AddButton(305, 260, 4005, 4007, b =>
                                {
                                    var g = (Guild) BaseGuild.FindByAbbrev("New");
                                    if (g != null && User.Guild == null)
                                    {
                                        Refresh(true);
                                        User.SendGump(new NewPlayerGuildJoinGump(g, User));
                                    }
                                });

                                AddLabel(340, 260, 2049, "Join the new player guild?");
                            }


                            AddButton(305, 285, 4005, 4007, b =>
                            {
                                if (Profile.LastBritainTeleport <= DateTime.UtcNow && User.PokerGame == null)
                                {
                                    User.Map = Map.Felucca;
                                    User.MoveToWorld(new Point3D(1438, 1690, 0), Map.Felucca);
                                    User.SendMessage(54, "You have been teleported to the West Britain Bank.");
                                    Profile.LastBritainTeleport = DateTime.UtcNow + TimeSpan.FromMinutes(5);
                                }
                                else if (User.PokerGame == null)
                                {
                                    User.SendMessage(54,
                                        "You cannot teleport to West Britain Bank for another " +
                                        (int)
                                            Math.Ceiling(
                                                (Profile.LastBritainTeleport - DateTime.UtcNow).TotalMinutes) +
                                        " minute(s).");
                                }
                                else
                                {
                                    User.SendMessage(61, "You cannot use this while playing a poker game.");
                                }
                            });


                            AddLabel(340, 285, 2049, "Travel to Britain?");

                            AddButton(305, 310, 4005, 4007, b =>
                            {
                                if (Profile.LastDungeonTeleport <= DateTime.UtcNow && User.PokerGame == null)
                                {
                                    User.Map = Map.Ilshenar;
                                    User.MoveToWorld(new Point3D(2182, 18, -32), Map.Ilshenar);
                                    User.SendMessage(54, "You have been teleported to the New Player Dungeon.");
                                    Profile.LastDungeonTeleport = DateTime.UtcNow + TimeSpan.FromMinutes(5);
                                }
                                else if (User.PokerGame == null)
                                {
                                    User.SendMessage(54,
                                        "You cannot teleport to the New Player Dungeon for another " +
                                        (int) Math.Ceiling((Profile.LastDungeonTeleport - DateTime.UtcNow).TotalMinutes) +
                                        " minute(s).");
                                }
                                else
                                {
                                    User.SendMessage(61, "You cannot use this while playing a poker game.");
                                }
                            });

                            AddLabel(340, 310, 2049, "Travel to Young Dungeon?");

                            AddButton(305, 335, 4005, 4007);

                            AddLabel(340, 335, 2049, "Page a Companion?");

                            AddButton(305, 360, 4005, 4007, b =>
                            {
                                Refresh(true);
                                if (User.NetState.Gumps.OfType<HelpGump>().Any())
                                {
                                    return;
                                }

                                if (!PageQueue.CheckAllowedToPage(User))
                                {
                                    return;
                                }

                                if (PageQueue.Contains(User))
                                {
                                    User.SendMenu(new ContainedMenu(User));
                                }
                                else
                                {
                                    User.SendGump(new Help.HelpGump(User));
                                }
                            });

                            AddLabel(340, 360, 2049, "Page a GM?");

                            AddButton(305, 480, 4005, 4007,
                                b => new DialogGump(User, this, 0, 0, null, null, 7004, a =>
                                {
                                    acct.Young = false;
                                    User.SendMessage(54, "You have successfully left the Young Player Program.");
                                    CentralGump.Adventurers.Remove(User);
                                }, c => { Refresh(true); })
                                {
                                    Title = "Leave the Young Player Program?",
                                    Html = "Are you sure you wish to leave the Young Player Program?",
                                    HtmlColor = DefaultHtmlColor
                                }.Send()
                                );

                            AddLabel(340, 480, 2049, "Leave the Young Player");

                            AddLabel(340, 495, 2049, "Program?");
                        }

                        if (User.Companion && acct != null && !acct.Young)
                        {
                            AddHtml(109, 74, 513, 18,
                                String.Format("<BIG>{0}</BIG>", "Ultima Online: Forever - Companion Control Panel")
                                    .WrapUOHtmlColor(KnownColor.GhostWhite), false, false);

                            AddBackground(61, 107, 454, 125, 3500);

                            AddLabel(86, 114, 2049, "Companion Message of the Day");

                            AddButton(305, 250, 4005, 4007);

                            AddLabel(340, 250, 2049, "Set MotD?");

                            AddButton(305, 280, 4005, 4007);

                            AddLabel(340, 280, 2049, "Answer Companion Pages?");
                        }
                        AddBackground(61, 250, 245, 267, 3500);

                        AddLabel(186, 260, 2049, "Join");

                        AddLabel(85, 260, 2049, "Name");

                        AddLabel(67, 233, 2049, "Adventuring Partners");

                        AddLabel(230, 260, 2049, "Members");

                        if (!CentralGump.Adventurers.Contains(User))
                        {
                            AddButton(305, 420, 4005, 4007, b =>
                            {
                                if (User.Party == null || Party.Get(User).Leader == User)
                                {
                                    CentralGump.Adventurers.Add(User);
                                    User.SendMessage(54, "You have signed up to start an adventure.");
                                    Refresh(true);
                                }
                                else
                                {
                                    User.SendMessage(54, "Only the leader of your party may sign up for an adventure.");
                                }
                            });

                            AddLabel(340, 420, 2049, "Join the adventuring");

                            AddLabel(340, 435, 2049, "list?");
                        }
                        else
                        {
                            AddButton(305, 420, 4005, 4007, b =>
                            {
                                CentralGump.Adventurers.Remove(User);
                                User.SendMessage(54, "You have removed yourself from the adventuring list.");
                                Refresh(true);
                            });

                            AddLabel(340, 420, 2049, "Remove yourself from the");

                            AddLabel(340, 435, 2049, "adventuring list?");
                        }

                        if (PageCount - 1 > Page)
                        {
                            AddButton(266, 485, 5601, 5605, NextPage);
                        }

                        if (Page > 0)
                        {
                            AddButton(240, 485, 5603, 5607, PreviousPage);
                        }
                    })
                    ;
                Dictionary<int, PlayerMobile> range = GetListRange();

                if (range.Count > 0)
                {
                    CompileEntryLayout(layout, range);
                }
            }
            else
            {
                AddHtml(116, 74, 513, 18,
                    String.Format("<BIG>{0}</BIG>", "Ultima Online: Forever - Companion Application")
                        .WrapUOHtmlColor(KnownColor.GhostWhite), false, false);                
            }
        }

        protected override void CompileEntryLayout(
            SuperGumpLayout layout, int length, int index, int pIndex, int yOffset, PlayerMobile entry)
        {
            if (entry.NetState == null)
            {
                CentralGump.Adventurers.Remove(entry);
                return;
            }
            yOffset = 280 + pIndex * 25;

            layout.Add(
                "entry" + index,
                () =>
                {
                    AddLabel(85, yOffset, 2049, entry.RawName);
                    Party myparty = Party.Get(User);
                    Party theirparty = Party.Get(entry);
                    if (User != entry)
                    {
                        AddButton(185, yOffset - 2, 4008, 4009, b =>
                        {
                            if (entry.NetState == null)
                            {
                                CentralGump.Adventurers.Remove(entry);
                                User.SendMessage(54, "This player has logged off.");
                                return;
                            }
                            if (Profile.LastPartyJoin <= DateTime.UtcNow)
                            {
                                if (myparty == null)
                                {
                                    if (theirparty != null &&
                                        (theirparty.Members.Count + theirparty.Candidates.Count) >= Party.Capacity)
                                    {
                                        User.SendMessage(54,
                                            "That player's adventuring party is already at full capacity.");
                                    }
                                    else
                                    {
                                        if (theirparty == null)
                                        {
                                            theirparty = new Party(entry);
                                            entry.Party = theirparty = new Party(entry);
                                        }
                                        User.Party = entry;
                                        CentralGump.Adventurers.Remove(User);
                                        theirparty.OnAccept(User);
                                        User.MoveToWorld(entry.Location, entry.Map);
                                        User.SendMessage(54, "You have joined " + entry.Name + " 's adventuring party.");
                                        entry.SendMessage(54, User.Name + " has joined your adventuring party.");
                                        Profile.LastPartyJoin = DateTime.UtcNow + TimeSpan.FromMinutes(5);
                                    }
                                }
                                else
                                {
                                    User.SendMessage(54,
                                        "You are already in a party!  Leave your current party to adventure with this player.");
                                }
                            }
                            else
                            {
                                User.SendMessage(54,
                                    "You cannot join another adventure for " +
                                    (int) Math.Ceiling((Profile.LastPartyJoin - DateTime.UtcNow).TotalMinutes) +
                                    " more minute(s).");
                            }
                        });
                    }
                    if (theirparty != null)
                    {
                        AddLabel(242, yOffset, 2049, theirparty.Members.Count + theirparty.Candidates.Count + "/10");
                    }
                    else
                    {
                        AddLabel(242, yOffset, 2049, "1/10");
                    }
                });
        }
    }
}