#region References

using System;
using Server.Engines.CustomTitles;
using Server.Engines.EventScores;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using VitaNex.Items;
using VitaNex.SuperGumps;
using VitaNex.SuperGumps.UI;

#endregion

namespace Server.Engines.ZombieEvent
{
    public class ZombieEventRewardsUI : DialogGump
    {
        private readonly PlayerZombieProfile UserProfile;

        public ZombieEventRewardsUI(
            PlayerMobile user,
            PlayerZombieProfile profile,
            Gump parent = null,
            int? x = null,
            int? y = null,
            string title = null,
            string html = null,
            int icon = 7004,
            Action<GumpButton> onAccept = null,
            Action<GumpButton> onCancel = null)
            : base(user, parent, 0, 0, title, html, icon, onAccept, onCancel)
        {
            CanMove = true;
            Closable = true;
            Modal = false;
            UserProfile = profile;
        }

        protected override void CompileLayout(SuperGumpLayout layout)
        {
            layout.Add(
                "background",
                () =>
                {
                    AddBackground(0, 57, 740, 513, 9200);
                    AddImageTiled(4, 60, 21, 507, 10464);
                    AddImageTiled(715, 61, 21, 507, 10464);
                    AddImage(708, 0, 10441, 60);
                    AddImage(342, 19, 9000, 60);
                    AddItem(360, 65, 8428, 0);
                    AddImageTiled(43, 151, 275, 1, 5410);
                    AddLabel(195, 135, 60, @"Zombieland Rewards");
                    AddLabel(49, 135, 1258, @"Ultima Online Forever:");

                    AddLabel(542, 69, 60, @"Zombieland Points");
                    AddImageTiled(537, 85, 113, 1, 5410);

                    if (UserProfile.SpendablePoints.ToString().Length == 1)
                    {
                        AddLabel(608, 96, 2049, UserProfile.SpendablePoints.ToString());
                    }
                    else if (UserProfile.SpendablePoints.ToString().Length == 2)
                    {
                        AddLabel(605, 96, 2049, UserProfile.SpendablePoints.ToString());
                    }
                    else if (UserProfile.SpendablePoints.ToString().Length == 3)
                    {
                        AddLabel(602, 96, 2049, UserProfile.SpendablePoints.ToString());
                    }
                    else if (UserProfile.SpendablePoints.ToString().Length == 4)
                    {
                        AddLabel(599, 96, 2049, UserProfile.SpendablePoints.ToString());
                    }
                    else
                    {
                        AddLabel(596, 96, 2049, UserProfile.SpendablePoints.ToString());
                    }
                });

            layout.Add(
                "tierrewards",
                () =>
                {
                    AddImageTiled(35, 184, 207, 1, 5410);
                    AddLabel(36, 167, 1258, @"1st Tier Rewards");
                    AddLabel(168, 166, 1258, @"[150 Pts]");

                    AddLabel(36, 193, 2049, @"Zombieland Skin Dye[Booger]");
                    AddItem(205, 193, 5163, 61);
                    AddButton(253, 192, 4005, 4006, b => new DialogGump(User, this, 0, 0, null, null, 7004, a =>
                    {
                        if (UserProfile.Tier1Cap < 5)
                        {
                            if (UserProfile.SpendablePoints >= 150)
                            {
                                UserProfile.SpendablePoints -= 150;
                                var scroll = new ZombieSkinDye(61);
                                if (User.BankBox != null)
                                {
                                    User.BankBox.DropItem(scroll);
                                }
                                User.SendMessage(54, "Your purchase has been deposted to your bankbox.");
                                UserProfile.Tier1Cap++;
                                Refresh(true);
                            }
                            else
                            {
                                User.SendMessage(54,
                                    "You do not have enough points to purchase this item.");
                                Refresh(true);
                            }
                        }
                        else
                        {
                            User.SendMessage(54,
                                "You have reached the purchase cap for this item.");
                        }
                    })
                    {
                        Title = "Confirm Purchase",
                        Html = "Are you sure you wish to purchase",
                        HtmlColor = DefaultHtmlColor
                    }.Send());

                    AddLabel(36, 216, 2049, @"Zombieland Skin Dye[Vesper]");
                    AddItem(205, 217, 5163, 1159);
                    AddButton(253, 216, 4005, 4006, b => new DialogGump(User, this, 0, 0, null, null, 7004, a =>
                    {
                        if (UserProfile.Tier1Cap < 5)
                        {
                            if (UserProfile.SpendablePoints >= 150)
                            {
                                UserProfile.SpendablePoints -= 150;
                                var scroll = new ZombieSkinDye(1159);
                                if (User.BankBox != null)
                                {
                                    User.BankBox.DropItem(scroll);
                                }
                                User.SendMessage(54, "Your purchase has been deposted to your bankbox.");
                                UserProfile.Tier1Cap++;
                                Refresh(true);
                            }
                            else
                            {
                                User.SendMessage(54,
                                    "You do not have enough points to purchase this item.");
                                Refresh(true);
                            }
                        }
                        else
                        {
                            User.SendMessage(54,
                                "You have reached the purchase cap for this item.");
                        }
                    })
                    {
                        Title = "Confirm Purchase",
                        Html = "Are you sure you wish to purchase",
                        HtmlColor = DefaultHtmlColor
                    }.Send());

                    AddImageTiled(35, 262, 207, 1, 5410);
                    AddLabel(36, 245, 1258, @"2nd Tier Rewards");
                    AddLabel(168, 244, 1258, @"[500 Pts]");

                    AddLabel(36, 269, 2116, @"Title Hue: #2117");
                    AddItem(212, 271, 10289, 1195);
                    AddButton(253, 269, 4005, 4006, b => new DialogGump(User, this, 0, 0, null, null, 7004, a =>
                    {
                        if (UserProfile.Tier2Cap < 3)
                        {
                            if (UserProfile.SpendablePoints >= 500)
                            {
                                UserProfile.SpendablePoints -= 500;
                                var scroll = new HueScroll(2117);
                                if (User.BankBox != null)
                                {
                                    User.BankBox.DropItem(scroll);
                                }
                                User.SendMessage(54, "Your purchase has been deposted to your bankbox.");
                                UserProfile.Tier2Cap++;
                                Refresh(true);
                            }
                            else
                            {
                                User.SendMessage(54,
                                    "You do not have enough points to purchase this item.");
                                Refresh(true);
                            }
                        }
                        else
                        {
                            User.SendMessage(54,
                                "You have reached the purchase cap for this item.");
                        }
                    })
                    {
                        Title = "Confirm Purchase",
                        Html = "Are you sure you wish to purchase",
                        HtmlColor = DefaultHtmlColor
                    }.Send());

                    AddLabel(36, 296, 60, @"Title Hue: #61 [Booger]");
                    AddItem(212, 298, 10289, 1195);
                    AddButton(253, 296, 4005, 4006, b => new DialogGump(User, this, 0, 0, null, null, 7004, a =>
                    {
                        if (UserProfile.Tier2Cap < 3)
                        {
                            if (UserProfile.SpendablePoints >= 500)
                            {
                                UserProfile.SpendablePoints -= 500;
                                var scroll = new HueScroll(61);
                                if (User.BankBox != null)
                                {
                                    User.BankBox.DropItem(scroll);
                                }
                                User.SendMessage(54, "Your purchase has been deposted to your bankbox.");
                                UserProfile.Tier2Cap++;
                                Refresh(true);
                            }
                            else
                            {
                                User.SendMessage(54,
                                    "You do not have enough points to purchase this item.");
                                Refresh(true);
                            }
                        }
                        else
                        {
                            User.SendMessage(54,
                                "You have reached the purchase cap for this item.");
                        }
                    })
                    {
                        Title = "Confirm Purchase",
                        Html = "Are you sure you wish to purchase",
                        HtmlColor = DefaultHtmlColor
                    }.Send());

                    AddImageTiled(35, 340, 207, 1, 5410);
                    AddLabel(35, 323, 1258, @"3rd Tier Rewards");
                    AddLabel(168, 322, 1258, @"[1000 Pts]");

                    AddLabel(35, 348, 2049, @"Title: The Gravedigger");
                    AddItem(212, 349, 10289, 1195);
                    AddButton(253, 347, 4005, 4006, b => new DialogGump(User, this, 0, 0, null, null, 7004, a =>
                    {
                        if (UserProfile.Tier3Cap < 3)
                        {
                            if (UserProfile.SpendablePoints >= 1000)
                            {
                                UserProfile.SpendablePoints -= 1000;
                                var scroll = new TitleScroll("The Gravedigger");
                                if (User.BankBox != null)
                                {
                                    User.BankBox.DropItem(scroll);
                                }
                                User.SendMessage(54, "Your purchase has been deposted to your bankbox.");
                                UserProfile.Tier3Cap++;
                                Refresh(true);
                            }
                            else
                            {
                                User.SendMessage(54,
                                    "You do not have enough points to purchase this item.");
                                Refresh(true);
                            }
                        }
                        else
                        {
                            User.SendMessage(54,
                                "You have reached the purchase cap for this item.");
                        }
                    })
                    {
                        Title = "Confirm Purchase",
                        Html = "Are you sure you wish to purchase",
                        HtmlColor = DefaultHtmlColor
                    }.Send());

                    AddLabel(35, 375, 2049, @"Title: Scourge of the Dead");
                    AddItem(212, 377, 10289, 1195);
                    AddButton(253, 374, 4005, 4006, b => new DialogGump(User, this, 0, 0, null, null, 7004, a =>
                    {
                        if (UserProfile.Tier3Cap < 3)
                        {
                            if (UserProfile.SpendablePoints >= 1000)
                            {
                                UserProfile.SpendablePoints -= 1000;
                                var scroll = new TitleScroll("Scourge of the Dead");
                                if (User.BankBox != null)
                                {
                                    User.BankBox.DropItem(scroll);
                                }
                                User.SendMessage(54, "Your purchase has been deposted to your bankbox.");
                                UserProfile.Tier3Cap++;
                                Refresh(true);
                            }
                            else
                            {
                                User.SendMessage(54,
                                    "You do not have enough points to purchase this item.");
                                Refresh(true);
                            }
                        }
                        else
                        {
                            User.SendMessage(54,
                                "You have reached the purchase cap for this item.");
                        }
                    })
                    {
                        Title = "Confirm Purchase",
                        Html = "Are you sure you wish to purchase",
                        HtmlColor = DefaultHtmlColor
                    }.Send());

                    AddImageTiled(34, 420, 207, 1, 5410);
                    AddLabel(35, 403, 1258, @"4th Tier Rewards");
                    AddLabel(168, 402, 1258, @"[1500 Pts]");

                    AddLabel(36, 425, 2049, @"Zombieland Hair Dye[Booger]");
                    AddItem(221, 428, 6195, 61);
                    AddButton(253, 425, 4005, 4006, b => new DialogGump(User, this, 0, 0, null, null, 7004, a =>
                    {
                        if (UserProfile.Tier4Cap < 2)
                        {
                            if (UserProfile.SpendablePoints >= 1500)
                            {
                                UserProfile.SpendablePoints -= 1500;
                                var reward = new ZombieHairDye();
                                if (User.BankBox != null)
                                {
                                    User.BankBox.DropItem(reward);
                                }
                                User.SendMessage(54, "Your purchase has been deposted to your bankbox.");
                                UserProfile.Tier4Cap++;
                                Refresh(true);
                            }
                            else
                            {
                                User.SendMessage(54,
                                    "You do not have enough points to purchase this item.");
                                Refresh(true);
                            }
                        }
                        else
                        {
                            User.SendMessage(54,
                                "You have reached the purchase cap for this item.");
                        }
                    })
                    {
                        Title = "Confirm Purchase",
                        Html = "Are you sure you wish to purchase",
                        HtmlColor = DefaultHtmlColor
                    }.Send());

                    AddLabel(36, 452, 2049, @"Zombieland Beard Dye[Booger]");
                    AddItem(203, 454, 6191, 61);
                    AddButton(253, 452, 4005, 4006, b => new DialogGump(User, this, 0, 0, null, null, 7004, a =>
                    {
                        if (UserProfile.Tier4Cap < 2)
                        {
                            if (UserProfile.SpendablePoints >= 1500)
                            {
                                UserProfile.SpendablePoints -= 1500;
                                var reward = new ZombieBeardDye();
                                if (User.BankBox != null)
                                {
                                    User.BankBox.DropItem(reward);
                                }
                                User.SendMessage(54, "Your purchase has been deposted to your bankbox.");
                                UserProfile.Tier4Cap++;
                                Refresh(true);
                            }
                            else
                            {
                                User.SendMessage(54,
                                    "You do not have enough points to purchase this item.");
                                Refresh(true);
                            }
                        }
                        else
                        {
                            User.SendMessage(54,
                                "You have reached the purchase cap for this item.");
                        }
                    })
                    {
                        Title = "Confirm Purchase",
                        Html = "Are you sure you wish to purchase",
                        HtmlColor = DefaultHtmlColor
                    }.Send());

                    AddImageTiled(35, 496, 207, 1, 5410);
                    AddLabel(35, 479, 1258, @"Fifth Tier Rewards");
                    AddLabel(168, 478, 1258, @"[2000 Pts]");

                    AddLabel(35, 502, 2049, @"Slimer[Pet][Booger]");
                    AddItem(165, 494, 8424, 61);
                    AddButton(253, 502, 4005, 4006, b => new DialogGump(User, this, 0, 0, null, null, 7004, a =>
                    {
                        if (UserProfile.Tier5Cap < 2)
                        {
                            if (UserProfile.SpendablePoints >= 2000)
                            {
                                if (User.Followers == 0)
                                {
                                    UserProfile.SpendablePoints -= 2000;
                                    var slimer = new Slimer
                                    {
                                        ControlMaster = User,
                                        Controlled = true,
                                        IsBonded = true,
                                        Hue = 61
                                    };
                                    slimer.MoveToWorld(User.Location, User.Map);
                                    UserProfile.Tier5Cap++;
                                    Refresh(true);
                                }
                                else
                                {
                                    User.SendMessage(54, "You must stable your pets before claiming this prize.");
                                }
                            }
                            else
                            {
                                User.SendMessage(54,
                                    "You do not have enough points to purchase this item.");
                                Refresh(true);
                            }
                        }
                        else
                        {
                            User.SendMessage(54,
                                "You have reached the purchase cap for this item.");
                        }
                    })
                    {
                        Title = "Confirm Purchase",
                        Html = "Are you sure you wish to purchase?",
                        HtmlColor = DefaultHtmlColor
                    }.Send());

                    AddLabel(35, 528, 2049, @"Slimer[Pet][Vesper]");
                    AddItem(190, 527, 8424, 1159);
                    AddButton(253, 530, 4005, 4006, b => new DialogGump(User, this, 0, 0, null, null, 7004, a =>
                    {
                        if (UserProfile.Tier5Cap < 2)
                        {
                            if (UserProfile.SpendablePoints >= 2000)
                            {
                                if (User.Followers == 0)
                                {
                                    UserProfile.SpendablePoints -= 2000;
                                    var slimer = new Slimer
                                    {
                                        ControlMaster = User,
                                        Controlled = true,
                                        IsBonded = true,
                                        Hue = 1159
                                    };
                                    slimer.MoveToWorld(User.Location, User.Map);
                                    UserProfile.Tier5Cap++;
                                    Refresh(true);
                                }
                                else
                                {
                                    User.SendMessage(54, "You must stable your pets before claiming this prize.");
                                }
                            }
                            else
                            {
                                User.SendMessage(54,
                                    "You do not have enough points to purchase this item.");
                                Refresh(true);
                            }
                        }
                        else
                        {
                            User.SendMessage(54,
                                "You have reached the purchase cap for this item.");
                        }
                    })
                    {
                        Title = "Confirm Purchase",
                        Html = "Are you sure you wish to purchase",
                        HtmlColor = DefaultHtmlColor
                    }.Send());


                    AddImageTiled(431, 184, 207, 1, 5410);
                    AddLabel(431, 167, 1258, @"6th Tier Rewards");
                    AddLabel(544, 166, 1258, @"[4500 Pts]");

                    AddLabel(431, 208, 2049, @"An Abomination Statue");
                    AddItem(556, 183, 11650, 0);
                    AddButton(652, 211, 4005, 4006, b => new DialogGump(User, this, 0, 0, null, null, 7004, a =>
                    {
                        if (UserProfile.Tier6Cap < 2)
                        {
                            if (UserProfile.SpendablePoints >= 4500)
                            {
                                UserProfile.SpendablePoints -= 4500;
                                var reward = new VitriolStatue();
                                if (User.BankBox != null)
                                {
                                    User.BankBox.DropItem(reward);
                                }
                                User.SendMessage(54, "Your purchase has been deposted to your bankbox.");
                                UserProfile.Tier6Cap++;
                                Refresh(true);
                            }
                            else
                            {
                                User.SendMessage(54,
                                    "You do not have enough points to purchase this item.");
                                Refresh(true);
                            }
                        }
                        else
                        {
                            User.SendMessage(54,
                                "You have reached the purchase cap for this item.");
                        }
                    })
                    {
                        Title = "Confirm Purchase",
                        Html = "Are you sure you wish to purchase",
                        HtmlColor = DefaultHtmlColor
                    }.Send());

                    AddImageTiled(431, 262, 207, 1, 5410);
                    AddLabel(431, 245, 1258, @"7th Tier Rewards");
                    AddLabel(544, 244, 1258, @"[5000 Pts]");

                    AddLabel(431, 285, 2049, @"Wheel of Corptune");
                    AddItem(612, 275, 8428, 0);
                    AddButton(652, 285, 4005, 4006, b => new DialogGump(User, this, 0, 0, null, null, 7004, a =>
                    {
                        if (UserProfile.Tier7Cap < 2)
                        {
                            if (UserProfile.SpendablePoints >= 5000)
                            {
                                UserProfile.SpendablePoints -= 5000;
                                var reward = new WheelofCorptune();
                                if (User.BankBox != null)
                                {
                                    User.BankBox.DropItem(reward);
                                }
                                User.SendMessage(54, "Your purchase has been deposted to your bankbox.");
                                UserProfile.Tier7Cap++;
                                Refresh(true);
                            }
                            else
                            {
                                User.SendMessage(54,
                                    "You do not have enough points to purchase this item.");
                                Refresh(true);
                            }
                        }
                        else
                        {
                            User.SendMessage(54,
                                "You have reached the purchase cap for this item.");
                        }
                    })
                    {
                        Title = "Confirm Purchase",
                        Html = "Are you sure you wish to purchase",
                        HtmlColor = DefaultHtmlColor
                    }.Send());

                    AddImageTiled(431, 340, 207, 1, 5410);
                    AddLabel(431, 323, 1258, @"8th Tier Rewards");
                    AddLabel(544, 322, 1258, @"[6000 Pts]");

                    AddLabel(431, 348, 2049, @"Dragon Egg[Red]");
                    AddItem(604, 345, 18406, 2117);
                    AddButton(652, 347, 4005, 4006, b => new DialogGump(User, this, 0, 0, null, null, 7004, a =>
                    {
                        if (UserProfile.Tier8Cap == 0)
                        {
                            if (UserProfile.SpendablePoints >= 6000)
                            {
                                UserProfile.SpendablePoints -= 6000;
                                var reward = new EvolutionEgg(2117);
                                if (User.BankBox != null)
                                {
                                    User.BankBox.DropItem(reward);
                                }
                                User.SendMessage(54, "Your purchase has been deposted to your bankbox.");
                                UserProfile.Tier8Cap++;
                                Refresh(true);
                            }
                            else
                            {
                                User.SendMessage(54,
                                    "You do not have enough points to purchase this item.");
                                Refresh(true);
                            }
                        }
                        else
                        {
                            User.SendMessage(54,
                                "You have reached the purchase cap for this item.");
                        }
                    })
                    {
                        Title = "Confirm Purchase",
                        Html = "Are you sure you wish to purchase this item?",
                        HtmlColor = DefaultHtmlColor
                    }.Send());

                    AddLabel(431, 375, 2049, @"Dragon Egg[Booger]");
                    AddItem(604, 384, 18406, 61);
                    AddButton(651, 374, 4005, 4006, b => new DialogGump(User, this, 0, 0, null, null, 7004, a =>
                    {
                        if (UserProfile.Tier8Cap == 0)
                        {
                            if (UserProfile.SpendablePoints >= 6000)
                            {
                                UserProfile.SpendablePoints -= 6000;
                                var reward = new EvolutionEgg(61);
                                if (User.BankBox != null)
                                {
                                    User.BankBox.DropItem(reward);
                                }
                                User.SendMessage(54, "Your purchase has been deposted to your bankbox.");
                                UserProfile.Tier8Cap++;
                                Refresh(true);
                            }
                            else
                            {
                                User.SendMessage(54,
                                    "You do not have enough points to purchase this item.");
                                Refresh(true);
                            }
                        }
                        else
                        {
                            User.SendMessage(54,
                                "You have reached the purchase cap for this item.");
                        }
                    })
                    {
                        Title = "Confirm Purchase",
                        Html = "Are you sure you wish to purchase",
                        HtmlColor = DefaultHtmlColor
                    }.Send());

                    AddImageTiled(431, 420, 207, 1, 5410);
                    AddLabel(431, 403, 1258, @"9th Tier Rewards");
                    AddLabel(544, 402, 1258, @"[7000 Pts]");

                    AddLabel(431, 427, 2049, @"Putrid Cuirass[Booger]");
                    AddItem(586, 419, 5199, 61);
                    AddButton(652, 426, 4005, 4006, b => new DialogGump(User, this, 0, 0, null, null, 7004, a =>
                    {
                        if (UserProfile.Tier9Cap == 0)
                        {
                            if (UserProfile.SpendablePoints >= 7000)
                            {
                                UserProfile.SpendablePoints -= 7000;
                                var reward = new PutridCuirass(61);
                                if (User.BankBox != null)
                                {
                                    User.BankBox.DropItem(reward);
                                }
                                User.SendMessage(54, "Your purchase has been deposted to your bankbox.");
                                UserProfile.Tier9Cap++;
                                Refresh(true);
                            }
                            else
                            {
                                User.SendMessage(54,
                                    "You do not have enough points to purchase this item.");
                                Refresh(true);
                            }
                        }
                        else
                        {
                            User.SendMessage(54,
                                "You have reached the purchase cap for this item.");
                        }
                    })
                    {
                        Title = "Confirm Purchase",
                        Html = "Are you sure you wish to purchase",
                        HtmlColor = DefaultHtmlColor
                    }.Send());

                    AddLabel(431, 453, 2049, @"Putrid Cuirass[Vesper]");
                    AddItem(601, 449, 5199, 1159);
                    AddButton(652, 454, 4005, 4006, b => new DialogGump(User, this, 0, 0, null, null, 7004, a =>
                    {
                        if (UserProfile.Tier9Cap == 0)
                        {
                            if (UserProfile.SpendablePoints >= 7000)
                            {
                                UserProfile.SpendablePoints -= 7000;
                                var reward = new PutridCuirass(1159);
                                if (User.BankBox != null)
                                {
                                    User.BankBox.DropItem(reward);
                                }
                                User.SendMessage(54, "Your purchase has been deposted to your bankbox.");
                                UserProfile.Tier9Cap++;
                                Refresh(true);
                            }
                            else
                            {
                                User.SendMessage(54,
                                    "You do not have enough points to purchase this item.");
                                Refresh(true);
                            }
                        }
                        else
                        {
                            User.SendMessage(54,
                                "You have reached the purchase cap for this item.");
                        }
                    })
                    {
                        Title = "Confirm Purchase",
                        Html = "Are you sure you wish to purchase",
                        HtmlColor = DefaultHtmlColor
                    }.Send());

                    AddImageTiled(431, 496, 207, 1, 5410);
                    AddLabel(431, 479, 1258, @"10th Tier Reward");
                    AddLabel(544, 478, 1258, @"[10000 Pts]");

                    AddLabel(431, 518, 2049, @"Putrid Steed");
                    AddLabel(444, 548, 2049, @"*Not an ethereal mount");

                    AddItem(606, 508, 8413, 61);
                    AddButton(652, 515, 4005, 4006, b => new DialogGump(User, this, 0, 0, null, null, 7004, a =>
                    {
                        if (UserProfile.Tier10Cap == 0)
                        {
                            if (UserProfile.SpendablePoints >= 10000)
                            {
                                if (User.Followers == 0)
                                {
                                    UserProfile.SpendablePoints -= 10000;
                                    var slimer = new PutridHorse
                                    {
                                        ControlMaster = User,
                                        Controlled = true,
                                        IsBonded = true
                                    };
                                    slimer.MoveToWorld(User.Location, User.Map);
                                    UserProfile.Tier10Cap++;
                                    Refresh(true);
                                }
                                else
                                {
                                    User.SendMessage(54, "You must stable your pets before claiming this prize.");
                                }
                            }
                            else
                            {
                                User.SendMessage(54,
                                    "You do not have enough points to purchase this item.");
                                Refresh(true);
                            }
                        }
                        else
                        {
                            User.SendMessage(54,
                                "You have reached the purchase cap for this item.");
                        }
                    })
                    {
                        Title = "Confirm Purchase",
                        Html = "Are you sure you wish to purchase:",
                        HtmlColor = DefaultHtmlColor
                    }.Send());
                }
                );
        }
    }
}