#region References

using System;
using Server.Engines.CustomTitles;
using Server.Engines.EventScores;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using VitaNex.Items;

#endregion

namespace VitaNex.SuperGumps.UI
{
    public class BattlesRewardsUI : DialogGump
    {
        private readonly PlayerEventScoreProfile UserProfile;

        public BattlesRewardsUI(
            PlayerMobile user,
            PlayerEventScoreProfile profile,
            Gump parent = null,
            int? x = null,
            int? y = null,
            string title = null,
            string html = null,
            int icon = 7004,
            Action<GumpButton> onAccept = null,
            Action<GumpButton> onCancel = null)
            : base(user, parent, 400, 400, title, html, icon, onAccept, onCancel)
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
                    AddImage(708, 0, 10441, 1160);
                    AddImage(342, 19, 9000, 1160);
                    AddItem(337, 68, 9935, 1161);
                    AddItem(360, 68, 9934, 1161);
                    AddImageTiled(43, 151, 325, 1, 5410);
                    AddLabel(195, 135, 137, @"Battles Tournament Rewards");
                    AddLabel(49, 135, 1258, @"Ultima Online Forever:");

                    AddLabel(528, 69, 137, @"Battle Tournament Points");
                    AddImageTiled(530, 85, 161, 1, 5410);

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
                    AddLabel(168, 166, 1258, @"[25 Pts]");

                    AddLabel(36, 193, 0, @"Battleborn Skin Dye: Red");
                    AddItem(212, 193, 2540, 1157);
                    AddButton(253, 192, 4005, 4006, b => new DialogGump(User, this, 0, 0, null, null, 7004, a =>
                    {
                        if (UserProfile.SpendablePoints >= 25)
                        {
                            UserProfile.SpendablePoints -= 25;
                            var dye = new BattlesPaint(1157);
                            if (User.BankBox != null)
                            {
                                User.BankBox.DropItem(dye);
                            }
                            User.SendMessage(54, "Your purchase has been deposted to your bankbox.");
                            Refresh(true);
                        }
                        else
                        {
                            User.SendMessage(54,
                                "You do not have enough spendable battle points to purchase this item.");
                            Refresh(true);
                        }
                    })
                    {
                        Title = "Confirm Purchase",
                        Html = "Are you sure you wish to purchase: Battleborn Skin Dye: Red for 25 Points?",
                        HtmlColor = DefaultHtmlColor
                    }.Send());

                    AddLabel(36, 216, 0, @"Battleborn Skin Dye: Shadow");
                    AddItem(212, 217, 2540, 2019);
                    AddButton(253, 216, 4005, 4006, b => new DialogGump(User, this, 0, 0, null, null, 7004, a =>
                    {
                        if (UserProfile.SpendablePoints >= 25)
                        {
                            UserProfile.SpendablePoints -= 25;
                            var dye = new BattlesPaint(1175);
                            if (User.BankBox != null)
                            {
                                User.BankBox.DropItem(dye);
                            }
                            User.SendMessage(54, "Your purchase has been deposted to your bankbox.");
                            Refresh(true);
                        }
                        else
                        {
                            User.SendMessage(54,
                                "You do not have enough spendable battle points to purchase this item.");
                            Refresh(true);
                        }
                    })
                    {
                        Title = "Confirm Purchase",
                        Html = "Are you sure you wish to purchase: Battleborn Skin Dye: Shadow for 25 Points?",
                        HtmlColor = DefaultHtmlColor
                    }.Send());

                    AddImageTiled(35, 262, 207, 1, 5410);
                    AddLabel(36, 245, 1258, @"2nd Tier Rewards");
                    AddLabel(168, 244, 1258, @"[75 Pts]");

                    AddLabel(36, 269, 1194, @"Title Hue: #1195");
                    AddItem(212, 271, 10289, 1195);
                    AddButton(253, 269, 4005, 4006, b => new DialogGump(User, this, 0, 0, null, null, 7004, a =>
                    {
                        if (UserProfile.SpendablePoints >= 75)
                        {
                            UserProfile.SpendablePoints -= 75;
                            var scroll = new HueScroll(1195);
                            if (User.BankBox != null)
                            {
                                User.BankBox.DropItem(scroll);
                            }
                            User.SendMessage(54, "Your purchase has been deposted to your bankbox.");
                            Refresh(true);
                        }
                        else
                        {
                            User.SendMessage(54,
                                "You do not have enough spendable battle points to purchase this item.");
                            Refresh(true);
                        }
                    })
                    {
                        Title = "Confirm Purchase",
                        Html = "Are you sure you wish to purchase: Title Hue: #1195 for 75 Points?",
                        HtmlColor = DefaultHtmlColor
                    }.Send());

                    AddLabel(36, 296, 1281, @"Title Hue: #1282");
                    AddItem(212, 298, 10289, 1195);
                    AddButton(253, 296, 4005, 4006, b => new DialogGump(User, this, 0, 0, null, null, 7004, a =>
                    {
                        if (UserProfile.SpendablePoints >= 75)
                        {
                            UserProfile.SpendablePoints -= 75;
                            var scroll = new HueScroll(1282);
                            if (User.BankBox != null)
                            {
                                User.BankBox.DropItem(scroll);
                            }
                            User.SendMessage(54, "Your purchase has been deposted to your bankbox.");
                            Refresh(true);
                        }
                        else
                        {
                            User.SendMessage(54,
                                "You do not have enough spendable battle points to purchase this item.");
                            Refresh(true);
                        }
                    })
                    {
                        Title = "Confirm Purchase",
                        Html = "Are you sure you wish to purchase: Title Hue: #1282 for 75 Points?",
                        HtmlColor = DefaultHtmlColor
                    }.Send());

                    AddImageTiled(35, 340, 207, 1, 5410);
                    AddLabel(35, 323, 1258, @"3rd Tier Rewards");
                    AddLabel(168, 322, 1258, @"[150 Pts]");

                    AddLabel(35, 348, 0, @"Title: The Gladiator");
                    AddItem(212, 349, 10289, 1195);
                    AddButton(253, 347, 4005, 4006, b => new DialogGump(User, this, 0, 0, null, null, 7004, a =>
                    {
                        if (UserProfile.SpendablePoints >= 150)
                        {
                            UserProfile.SpendablePoints -= 150;
                            var scroll = new TitleScroll("The Gladiator");
                            if (User.BankBox != null)
                            {
                                User.BankBox.DropItem(scroll);
                            }
                            User.SendMessage(54, "Your purchase has been deposted to your bankbox.");
                            Refresh(true);
                        }
                        else
                        {
                            User.SendMessage(54,
                                "You do not have enough spendable battle points to purchase this item.");
                            Refresh(true);
                        }
                    })
                    {
                        Title = "Confirm Purchase",
                        Html = "Are you sure you wish to purchase: Title: The Gladiator for 150 Points?",
                        HtmlColor = DefaultHtmlColor
                    }.Send());

                    AddLabel(35, 375, 0, @"Title: The Conqueror");
                    AddItem(212, 377, 10289, 1195);
                    AddButton(253, 374, 4005, 4006, b => new DialogGump(User, this, 0, 0, null, null, 7004, a =>
                    {
                        if (UserProfile.SpendablePoints >= 150)
                        {
                            UserProfile.SpendablePoints -= 150;
                            var scroll = new TitleScroll("The Conqueror");
                            if (User.BankBox != null)
                            {
                                User.BankBox.DropItem(scroll);
                            }
                            User.SendMessage(54, "Your purchase has been deposted to your bankbox.");
                            Refresh(true);
                        }
                        else
                        {
                            User.SendMessage(54,
                                "You do not have enough spendable battle points to purchase this item.");
                            Refresh(true);
                        }
                    })
                    {
                        Title = "Confirm Purchase",
                        Html = "Are you sure you wish to purchase: Title: The Conqueror for 150 Points?",
                        HtmlColor = DefaultHtmlColor
                    }.Send());

                    AddImageTiled(34, 420, 207, 1, 5410);
                    AddLabel(35, 403, 1258, @"4th Tier Rewards");
                    AddLabel(168, 402, 1258, @"[300 Pts]");

                    AddLabel(36, 425, 0, @"Battleborn Hair Dye: Red");
                    AddItem(221, 428, 6195);
                    AddButton(253, 425, 4005, 4006, b => new DialogGump(User, this, 0, 0, null, null, 7004, a =>
                    {
                        if (UserProfile.SpendablePoints >= 300)
                        {
                            UserProfile.SpendablePoints -= 300;
                            var reward = new BattleBornHairDye();
                            if (User.BankBox != null)
                            {
                                User.BankBox.DropItem(reward);
                            }
                            User.SendMessage(54, "Your purchase has been deposted to your bankbox.");
                            Refresh(true);
                        }
                        else
                        {
                            User.SendMessage(54,
                                "You do not have enough spendable battle points to purchase this item.");
                            Refresh(true);
                        }
                    })
                    {
                        Title = "Confirm Purchase",
                        Html = "Are you sure you wish to purchase: Battleborn Hair Dye: Red for 300 Points?",
                        HtmlColor = DefaultHtmlColor
                    }.Send());

                    AddLabel(36, 452, 0, @"Battleborn Beard Dye: Red");
                    AddItem(203, 454, 6191);
                    AddButton(253, 452, 4005, 4006, b => new DialogGump(User, this, 0, 0, null, null, 7004, a =>
                    {
                        if (UserProfile.SpendablePoints >= 300)
                        {
                            UserProfile.SpendablePoints -= 300;
                            var reward = new BattleBornBeardDye();
                            if (User.BankBox != null)
                            {
                                User.BankBox.DropItem(reward);
                            }
                            User.SendMessage(54, "Your purchase has been deposted to your bankbox.");
                            Refresh(true);
                        }
                        else
                        {
                            User.SendMessage(54,
                                "You do not have enough spendable battle points to purchase this item.");
                            Refresh(true);
                        }
                    })
                    {
                        Title = "Confirm Purchase",
                        Html = "Are you sure you wish to purchase: Battleborn Beard Dye: Red for 300 Points?",
                        HtmlColor = DefaultHtmlColor
                    }.Send());

                    AddImageTiled(35, 496, 207, 1, 5410);
                    AddLabel(35, 479, 1258, @"Fifth Tier Rewards");
                    AddLabel(168, 478, 1258, @"[350 Pts]");

                    AddLabel(35, 502, 0, @"Mungbat[Pet]: Red");
                    AddItem(193, 499, 8441, 1157);
                    AddButton(253, 502, 4005, 4006, b => new DialogGump(User, this, 0, 0, null, null, 7004, a =>
                    {
                        if (UserProfile.SpendablePoints >= 350)
                        {
                            if (User.Followers == 0)
                            {
                                UserProfile.SpendablePoints -= 350;
                                var mungbat = new Mungbat
                                {
                                    ControlMaster = User,
                                    Controlled = true,
                                    IsBonded = true,
                                    Hue = 1157
                                };
                                mungbat.MoveToWorld(User.Location, User.Map);
                                Refresh(true);
                            }
                            else
                            {
                                User.SendMessage(54, "You must stable your pets before claiming this prize.");
                            }

                            Refresh(true);
                        }
                        else
                        {
                            User.SendMessage(54,
                                "You do not have enough spendable battle points to purchase this pet.");
                            Refresh(true);
                        }
                    })
                    {
                        Title = "Confirm Purchase",
                        Html = "Are you sure you wish to purchase: Mungbat[Pet]: Red for 350 Points?",
                        HtmlColor = DefaultHtmlColor
                    }.Send());

                    AddLabel(35, 528, 0, @"Mungbat[Pet]: Shadow");
                    AddItem(221, 525, 8441, 1175);
                    AddButton(253, 530, 4005, 4006, b => new DialogGump(User, this, 0, 0, null, null, 7004, a =>
                    {
                        if (UserProfile.SpendablePoints >= 350)
                        {
                            if (User.Followers == 0)
                            {
                                UserProfile.SpendablePoints -= 350;
                                var mungbat = new Mungbat
                                {
                                    ControlMaster = User,
                                    Controlled = true,
                                    IsBonded = true,
                                    Hue = 1175
                                };
                                mungbat.MoveToWorld(User.Location, User.Map);
                                Refresh(true);
                            }
                            else
                            {
                                User.SendMessage(54, "You must stable your pets before claiming this prize.");
                            }

                            Refresh(true);
                        }
                        else
                        {
                            User.SendMessage(54,
                                "You do not have enough spendable battle points to purchase this pet.");
                            Refresh(true);
                        }
                    })
                    {
                        Title = "Confirm Purchase",
                        Html = "Are you sure you wish to purchase: Mungbat[Pet]: Shadow for 350 Points?",
                        HtmlColor = DefaultHtmlColor
                    }.Send());


                    AddImageTiled(431, 184, 207, 1, 5410);
                    AddLabel(431, 167, 1258, @"6th Tier Rewards");
                    AddLabel(544, 166, 1258, @"[450 Pts]");

                    AddLabel(431, 193, 0, @"Bloodstained Gloves");
                    AddItem(604, 192, 5062, 1157);
                    AddButton(652, 194, 4005, 4006, b => new DialogGump(User, this, 0, 0, null, null, 7004, a =>
                    {
                        if (UserProfile.SpendablePoints >= 450)
                        {
                            UserProfile.SpendablePoints -= 450;
                            var reward = new BloodstainedGloves();
                            if (User.BankBox != null)
                            {
                                User.BankBox.DropItem(reward);
                            }
                            User.SendMessage(54, "Your purchase has been deposted to your bankbox.");
                            Refresh(true);
                        }
                        else
                        {
                            User.SendMessage(54,
                                "You do not have enough spendable battle points to purchase this item.");
                            Refresh(true);
                        }
                    })
                    {
                        Title = "Confirm Purchase",
                        Html = "Are you sure you wish to purchase: Bloodstained Gloves for 450 Points?",
                        HtmlColor = DefaultHtmlColor
                    }.Send());

                    AddLabel(431, 216, 0, @"Bloodied Axe");
                    AddItem(570, 213, 11560, 1157);
                    AddButton(652, 228, 4005, 4006, b => new DialogGump(User, this, 0, 0, null, null, 7004, a =>
                    {
                        if (UserProfile.SpendablePoints >= 450)
                        {
                            UserProfile.SpendablePoints -= 450;
                            var reward = new BloodiedAxe();
                            if (User.BankBox != null)
                            {
                                User.BankBox.DropItem(reward);
                            }
                            User.SendMessage(54, "Your purchase has been deposted to your bankbox.");
                            Refresh(true);
                        }
                        else
                        {
                            User.SendMessage(54,
                                "You do not have enough spendable battle points to purchase this item.");
                            Refresh(true);
                        }
                    })
                    {
                        Title = "Confirm Purchase",
                        Html = "Are you sure you wish to purchase: Bloodied Axe for 450 Points?",
                        HtmlColor = DefaultHtmlColor
                    }.Send());


                    AddImageTiled(431, 262, 207, 1, 5410);
                    AddLabel(431, 245, 1258, @"7th Tier Rewards");
                    AddLabel(544, 244, 1258, @"[550 Pts]");

                    AddLabel(431, 269, 0, @"Binding of Battle: Red");
                    AddItem(604, 274, 4230, 1157);
                    AddButton(652, 269, 4005, 4006, b => new DialogGump(User, this, 0, 0, null, null, 7004, a =>
                    {
                        if (UserProfile.SpendablePoints >= 550)
                        {
                            UserProfile.SpendablePoints -= 550;
                            var reward = new BindingofBattle {Hue = 1157};
                            if (User.BankBox != null)
                            {
                                User.BankBox.DropItem(reward);
                            }
                            User.SendMessage(54, "Your purchase has been deposted to your bankbox.");
                            Refresh(true);
                        }
                        else
                        {
                            User.SendMessage(54,
                                "You do not have enough spendable battle points to purchase this item.");
                            Refresh(true);
                        }
                    })
                    {
                        Title = "Confirm Purchase",
                        Html = "Are you sure you wish to purchase: Binding of Battle: Red for 550 Points?",
                        HtmlColor = DefaultHtmlColor
                    }.Send());

                    AddLabel(431, 296, 0, @"Binding of Battle: Shadow");
                    AddItem(604, 301, 4230, 1175);
                    AddButton(652, 296, 4005, 4006, b => new DialogGump(User, this, 0, 0, null, null, 7004, a =>
                    {
                        if (UserProfile.SpendablePoints >= 550)
                        {
                            UserProfile.SpendablePoints -= 550;
                            var reward = new BindingofBattle {Hue = 1175};
                            if (User.BankBox != null)
                            {
                                User.BankBox.DropItem(reward);
                            }
                            User.SendMessage(54, "Your purchase has been deposted to your bankbox.");
                            Refresh(true);
                        }
                        else
                        {
                            User.SendMessage(54,
                                "You do not have enough spendable battle points to purchase this item.");
                            Refresh(true);
                        }
                    })
                    {
                        Title = "Confirm Purchase",
                        Html = "Are you sure you wish to purchase: Binding of Battle: Shadow for 550 Points?",
                        HtmlColor = DefaultHtmlColor
                    }.Send());

                    AddImageTiled(431, 340, 207, 1, 5410);
                    AddLabel(431, 323, 1258, @"8th Tier Rewards");
                    AddLabel(544, 322, 1258, @"[700 Pts]");

                    AddLabel(431, 348, 0, @"Dragon Egg: Red");
                    AddItem(604, 345, 18406, 1157);
                    AddButton(652, 347, 4005, 4006, b => new DialogGump(User, this, 0, 0, null, null, 7004, a =>
                    {
                        if (UserProfile.SpendablePoints >= 700)
                        {
                            UserProfile.SpendablePoints -= 700;
                            var reward = new EvolutionEgg(1157);
                            if (User.BankBox != null)
                            {
                                User.BankBox.DropItem(reward);
                            }
                            User.SendMessage(54, "Your purchase has been deposted to your bankbox.");
                            Refresh(true);
                        }
                        else
                        {
                            User.SendMessage(54,
                                "You do not have enough spendable battle points to purchase this item.");
                            Refresh(true);
                        }
                    })
                    {
                        Title = "Confirm Purchase",
                        Html = "Are you sure you wish to purchase: Dragon Egg: Red for 700 Points?",
                        HtmlColor = DefaultHtmlColor
                    }.Send());

                    AddLabel(431, 375, 0, @"Dragon Egg: Shadow");
                    AddItem(604, 384, 18406, 1175);
                    AddButton(651, 374, 4005, 4006, b => new DialogGump(User, this, 0, 0, null, null, 7004, a =>
                    {
                        if (UserProfile.SpendablePoints >= 700)
                        {
                            UserProfile.SpendablePoints -= 700;
                            var reward = new EvolutionEgg(1175);
                            if (User.BankBox != null)
                            {
                                User.BankBox.DropItem(reward);
                            }
                            User.SendMessage(54, "Your purchase has been deposted to your bankbox.");
                            Refresh(true);
                        }
                        else
                        {
                            User.SendMessage(54,
                                "You do not have enough spendable battle points to purchase this item.");
                            Refresh(true);
                        }
                    })
                    {
                        Title = "Confirm Purchase",
                        Html = "Are you sure you wish to purchase: Dragon Egg: Shadow for 700 Points?",
                        HtmlColor = DefaultHtmlColor
                    }.Send());

                    AddImageTiled(431, 420, 207, 1, 5410);
                    AddLabel(431, 403, 1258, @"9th Tier Rewards");
                    AddLabel(544, 402, 1258, @"[800 Pts]");

                    AddLabel(431, 425, 0, @"Decorative Sword: Red");
                    AddItem(604, 428, 9934, 1157);
                    AddButton(652, 425, 4005, 4006, b => new DialogGump(User, this, 0, 0, null, null, 7004, a =>
                    {
                        if (UserProfile.SpendablePoints >= 800)
                        {
                            UserProfile.SpendablePoints -= 800;
                            var reward = new DecorativeSword {Hue = 1157};
                            if (User.BankBox != null)
                            {
                                User.BankBox.DropItem(reward);
                            }
                            User.SendMessage(54, "Your purchase has been deposted to your bankbox.");
                            Refresh(true);
                        }
                        else
                        {
                            User.SendMessage(54,
                                "You do not have enough spendable battle points to purchase this item.");
                            Refresh(true);
                        }
                    })
                    {
                        Title = "Confirm Purchase",
                        Html = "Are you sure you wish to purchase: Decorative Sword: Red for 800 Points?",
                        HtmlColor = DefaultHtmlColor
                    }.Send());

                    AddLabel(431, 452, 0, @"Decorative Sword: Shadow");
                    AddItem(604, 457, 9934, 1175);
                    AddButton(652, 452, 4005, 4006, b => new DialogGump(User, this, 0, 0, null, null, 7004, a =>
                    {
                        if (UserProfile.SpendablePoints >= 800)
                        {
                            UserProfile.SpendablePoints -= 800;
                            var reward = new DecorativeSword {Hue = 1175};
                            if (User.BankBox != null)
                            {
                                User.BankBox.DropItem(reward);
                            }
                            User.SendMessage(54, "Your purchase has been deposted to your bankbox.");
                            Refresh(true);
                        }
                        else
                        {
                            User.SendMessage(54,
                                "You do not have enough spendable battle points to purchase this item.");
                            Refresh(true);
                        }
                    })
                    {
                        Title = "Confirm Purchase",
                        Html = "Are you sure you wish to purchase: Decorative Sword: Shadow for 800 Points?",
                        HtmlColor = DefaultHtmlColor
                    }.Send());

                    AddImageTiled(431, 496, 207, 1, 5410);
                    AddLabel(431, 479, 1258, @"10th Tier Reward");
                    AddLabel(544, 478, 1258, @"[1500 Pts]");

                    AddLabel(431, 502, 0, @"Flesh-Bound Tome");
                    AddItem(604, 503, 3834, 137);
                    AddButton(652, 502, 4005, 4006, b => new DialogGump(User, this, 0, 0, null, null, 7004, a =>
                    {
                        if (UserProfile.SpendablePoints >= 1500)
                        {
                            UserProfile.SpendablePoints -= 1500;
                            var reward = new FleshboundTome();
                            if (User.BankBox != null)
                            {
                                User.BankBox.DropItem(reward);
                            }
                            User.SendMessage(54, "Your purchase has been deposted to your bankbox.");
                            Refresh(true);
                        }
                        else
                        {
                            User.SendMessage(54,
                                "You do not have enough spendable battle points to purchase this item.");
                            Refresh(true);
                        }
                    })
                    {
                        Title = "Confirm Purchase",
                        Html = "Are you sure you wish to purchase: Flesh-Bound Tome for 1500 Points?",
                        HtmlColor = DefaultHtmlColor
                    }.Send());
                }
                );
        }
    }
}