#region References

using Server.Engines.CustomTitles;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Scripts.New.Adam.Easter.Items;
using VitaNex.Items;

#endregion

namespace Server.Gumps
{
    public class EasterItemsUI : Gump
    {
        public EasterItemsUI(PlayerMobile pm)
            : base(0, 0)
        {
            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            if (EasterEventController.Instance == null)
                return;

            AddBackground(26, 83, 424, 485, 9200);
            AddImageTiled(30, 85, 21, 480, 10464);
            AddImageTiled(426, 87, 21, 480, 10464);
            AddImage(417, 26, 10441, 1165);
            AddImage(207, 44, 9000, 1265);

            AddImageTiled(320, 110, 92, 1, 5410);
            AddLabel(323, 94, 2049, @"Easter Points");

            if (pm != null && EasterEventController.PointList != null && EasterEventController.PointList.ContainsKey(pm))
            {
                AddLabel(357, 117, 1265, EasterEventController.PointList[pm].ToString());
            }
            else
            {
                AddLabel(357, 117, 1265, "0");              
            }



            AddImageTiled(77, 176, 325, 1, 5410);
            AddLabel(75, 160, 1258, "Ultima Online Forever:");
            AddLabel(217, 160, 2049, "A Very Wretched Easter 2014");
            AddItem(219, 93, 18406);

            AddLabel(62, 192, 2049, @"First Tier Rewards");
            AddLabel(194, 191, 2049, "["+ EasterEventController.FirstTierCost + " Points]");

            AddButton(60, 224, 4005, 4006, 1, GumpButtonType.Reply, 0);
            AddLabel(95, 225, 2049, @"Random Firework");
            AddItem(226, 218, 6202, 38);


            AddButton(263, 224, 4005, 4006, 2, GumpButtonType.Reply, 0);
            AddLabel(298, 225, 2049, @"Easter Eggs");
            AddItem(393, 230, 2485, 88);

            AddLabel(62, 270, 2049, @"Second Tier Rewards");
            AddLabel(194, 269, 2049, "[" + EasterEventController.SecondTierCost + " Points]");

            AddButton(60, 302, 4005, 4006, 3, GumpButtonType.Reply, 0);
            AddLabel(95, 303, 2049, @"Title: The Gatherer");
            AddItem(226, 305, 10289, 1195);

            AddButton(263, 302, 4005, 4006, 4, GumpButtonType.Reply, 0);
            AddLabel(298, 303, 1165, @"Title Hue: 1166");
            AddItem(395, 305, 10289, 1195);

            AddLabel(61, 348, 2049, @"Third Tier Rewards");
            AddLabel(194, 347, 2049, "[" + EasterEventController.ThirdTierCost + " Points]");

            AddButton(60, 380, 4005, 4006, 5, GumpButtonType.Reply, 0);
            AddLabel(95, 381, 2049, @"Herald of Easter");
            AddItem(226, 385, 8485, 1166);

            AddButton(263, 380, 4005, 4006, 6, GumpButtonType.Reply, 0);
            AddLabel(298, 381, 2049, @"Wretched Rabbit");
            AddItem(395, 385, 8485, 1157);

            AddLabel(61, 426, 2049, @"Fourth Tier Rewards");
            AddLabel(194, 425, 2049, "[" + EasterEventController.FourthTierCost + " Points]");

            AddButton(60, 458, 4005, 4006, 7, GumpButtonType.Reply, 0);
            AddLabel(95, 459, 2049, @"Easter Bracelet");
            AddItem(226, 465, 4230, 1166);

            AddButton(263, 458, 4005, 4006, 8, GumpButtonType.Reply, 0);
            AddLabel(298, 459, 2049, @"Easter Bracelet");
            AddItem(395, 465, 4230, 1266);

            AddLabel(61, 504, 2049, @"Fifth Tier Rewards");
            AddLabel(194, 503, 2049, "[" + EasterEventController.FifthTierCost + " Points]");

            AddButton(60, 536, 4005, 4006, 9, GumpButtonType.Reply, 0);
            AddLabel(95, 537, 2049, @"Title: Herald of Easter");
            AddItem(226, 538, 10289, 1195);

            AddButton(263, 536, 4005, 4006, 10, GumpButtonType.Reply, 0);
            AddLabel(298, 537, 1265, @"Title Hue: 1266");
            AddItem(395, 538, 10289, 1195);


            AddImageTiled(61, 209, 207, 1, 5410);
            AddImageTiled(61, 287, 207, 1, 5410);
            AddImageTiled(61, 365, 207, 1, 5410);
            AddImageTiled(61, 443, 207, 1, 5410);
            AddImageTiled(61, 521, 207, 1, 5410);
        }


        public override void OnResponse(NetState sender, RelayInfo info)
        {
            var pm = sender.Mobile as PlayerMobile;

            if (pm == null)
            {
                return;
            }
            if (EasterEventController.Instance == null)
            {
                return;
            }
            if (EasterEventController.PointList == null)
            {
                return;
            }
            if (!EasterEventController.PointList.ContainsKey(pm))
            {
                pm.SendMessage(54,"You have not turned in any purified Easter eggs!");
                return;
            }

            int cost = 0;

            switch (info.ButtonID)
            {
                case 0:
                {
                    pm.SendGump(new EasterRewardsUI(pm));
                    break;
                }
                case 1:
                {
                    if (EasterEventController.PointList[pm] >= EasterEventController.FirstTierCost && EasterEventController.FirstTierCost > 0)
                    {
                        int rand = Utility.Random(6);
                        switch (rand)
                        {
                            case 0:
                                pm.BankBox.DropItem(new BigBettyRocket());
                                break;
                            case 1:
                                pm.BankBox.DropItem(new BlockBusterRocket());
                                break;
                            case 2:
                                pm.BankBox.DropItem(new BottleRocket());
                                break;
                            case 3:
                                pm.BankBox.DropItem(new LittleBoyRocket());
                                break;
                            case 4:
                                pm.BankBox.DropItem(new PenetratorRocket());
                                break;
                            case 5:
                                pm.BankBox.DropItem(new SkyShieldRocket());
                                break;
                        }
                        cost = EasterEventController.FirstTierCost;
                    }

                    break;
                }
                case 2:
                {
                    if (EasterEventController.PointList[pm] >= EasterEventController.FirstTierCost && EasterEventController.FirstTierCost > 0)
                    {
                        pm.BankBox.DropItem(new EasterEggsEvent());
                        cost = EasterEventController.FirstTierCost;
                    }
                    break;
                }
                case 3:
                {
                    if (EasterEventController.PointList[pm] >= EasterEventController.SecondTierCost && EasterEventController.SecondTierCost > 0)
                    {
                        pm.BankBox.DropItem(new TitleScroll("The Gatherer"));
                        cost = EasterEventController.SecondTierCost;
                    }
                    break;
                }
                case 4:
                {
                    if (EasterEventController.PointList[pm] >= EasterEventController.SecondTierCost && EasterEventController.SecondTierCost > 0)
                    {
                        pm.BankBox.DropItem(new HueScroll(1166));
                        cost = EasterEventController.SecondTierCost;
                    }
                    break;
                }
                case 5:
                {
                    if (EasterEventController.PointList[pm] >= EasterEventController.ThirdTierCost && EasterEventController.ThirdTierCost > 0)
                    {
                        if (pm.Followers < pm.FollowersMax)
                        {
                            var bunny = new HeraldofEasterPet() {ControlMaster = pm, Controlled = true, IsBonded = true};
                            bunny.MoveToWorld(pm.Location, pm.Map);
                            cost = EasterEventController.ThirdTierCost;
                        }
                        else
                        {
                            pm.SendMessage(54, "You must stable your pets before claiming this prize.");
                        }
                    }
                    break;
                }
                case 6:
                {
                    if (EasterEventController.PointList[pm] >= EasterEventController.ThirdTierCost && EasterEventController.ThirdTierCost > 0)
                    {
                        if (pm.Followers < pm.FollowersMax)
                        {
                            var bunny = new WretchedRabbitPet() { ControlMaster = pm, Controlled = true, IsBonded = true };
                            bunny.MoveToWorld(pm.Location, pm.Map);
                            cost = EasterEventController.ThirdTierCost;
                        }
                        else
                        {
                            pm.SendMessage(54, "You must stable your pets before claiming this prize.");
                        }
                    }
                    break;
                }
                case 7:
                {
                    if (EasterEventController.PointList[pm] >= EasterEventController.FourthTierCost && EasterEventController.FourthTierCost > 0)
                    {
                        pm.BankBox.DropItem(new EasterBracelet(1166));
                        cost = EasterEventController.FourthTierCost;
                    }
                    break;
                }
                case 8:
                {
                    if (EasterEventController.PointList[pm] >= EasterEventController.FourthTierCost && EasterEventController.FourthTierCost > 0)
                    {
                        pm.BankBox.DropItem(new EasterBracelet(1266));
                        cost = EasterEventController.FourthTierCost;
                    }
                    break;
                }
                case 9:
                {
                    if (EasterEventController.PointList[pm] >= EasterEventController.FifthTierCost && EasterEventController.FifthTierCost > 0)
                    {
                        pm.BankBox.DropItem(new TitleScroll("Herald of Easter"));
                        cost = EasterEventController.FifthTierCost;
                    }
                    break;
                }
                case 10:
                {
                    if (EasterEventController.PointList[pm] >= EasterEventController.FifthTierCost && EasterEventController.FifthTierCost > 0)
                    {
                        pm.BankBox.DropItem(new HueScroll(1266));
                        cost = EasterEventController.FifthTierCost;
                    }
                    break;
                }
            }
            if (cost > 0)
            {
                pm.SendMessage(54,
                    "The item has been placed in your bank box and {0} points have been deducted from your earned Event Points.", cost);
                EasterEventController.PointList[pm] -= cost;
                pm.SendGump(new EasterItemsUI(pm));
            }
            else if (info.ButtonID > 0)
            {
                pm.SendMessage(54,"You did not have enough points to purchase that item.");
                pm.SendGump(new EasterItemsUI(pm));
            }
        }
    }
}