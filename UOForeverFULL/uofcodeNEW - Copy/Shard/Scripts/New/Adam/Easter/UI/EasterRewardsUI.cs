#region References

using System.Collections.Generic;
using System.Linq;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using VitaNex.Modules.AutoPvP;

#endregion

namespace Server.Gumps
{
    public class EasterRewardsUI : Gump
    {
        public EasterRewardsUI(PlayerMobile pm)
            : base(0, 0)
        {
            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddPage(0);
            AddBackground(27, 83, 424, 357, 9200);
            AddImageTiled(30, 85, 21, 349, 10464);
            AddImageTiled(426, 87, 21, 349, 10464);
            AddImage(417, 26, 10441, 1165);
            AddImage(207, 44, 9000, 1265);
            AddImageTiled(77, 176, 325, 1, 5410);
            AddLabel(217, 160, 2049, "A Very Wretched Easter 2014");
            AddItem(219, 93, 18406);
            AddHtml(56, 184, 366, 168, pm.Name + @"!  You must help us!

The Wretched Rabbit has returned from his banishment a millennia ago.  The circumstances are dire for Easter.The Spirit of Easter's eggs have been stolen from our holy grove located in the Hedge Maze.  Without them, the Sosarian Easter cycle cannot be completed!  Please, we beg of you, help us in Sosaria's hour of need. 

We require you to adventure throughout Sosaria in search of beings who have fallen under the corrupting influence of the Wretched Rabbit.  Slay them and retrieve our eggs.  The Wretched Rabbit has most likely corrupted them to use towards his nefarious ends.  After collecting them, you must return to our holy grove in the hedge maze and purify them in the healing waters of the holy fountain of Easter.

Return them to us after you have done this deed and we shall reward you handsomely.

Help us, " + pm.Name + ".  You're our only hope.", true, true);
            AddLabel(98, 385, 2049, @"Turn in Purified Easter Eggs");
            AddLabel(98, 415, 2049, @"Purchase Rewards");
            AddLabel(98, 355, 2049, @"View Top Scores");
            AddButton(63, 355, 4005, 4006, 1, GumpButtonType.Reply, 0);
            AddButton(63, 385, 4005, 4006, 2, GumpButtonType.Reply, 0);
            AddButton(63, 415, 4005, 4006, 3, GumpButtonType.Reply, 0);
            AddLabel(75, 160, 1258, "Ultima Online Forever:");
            AddImageTiled(303, 371, 115, 1, 5410);
            AddLabel(301, 355, 2049, "Easter Event Points");
            if (EasterEventController.Instance != null && EasterEventController.PointList != null &&
                EasterEventController.PointList.ContainsKey(pm))
            {
                AddLabel(349, 377, 1265, EasterEventController.PointList[pm].ToString());
            }
            else
            {
                AddLabel(349, 377, 1265, "0");
            }
        }


        public override void OnResponse(NetState sender, RelayInfo info)
        {
            var pm = sender.Mobile as PlayerMobile;

            switch (info.ButtonID)
            {
                case 1:
                {
                    if (pm == null || EasterEventController.ParticipantList == null)
                        return;

                    pm.SendGump(new EasterScoreBoard(pm, EasterEventController.ParticipantList));
                    break;
                }
                case 2:
                {
                    if (pm == null)
                    {
                        return;
                    }
                    int count = 0;

                    if (EasterEventController.Instance != null)
                    {
                        if (EasterEventController.ParticipantList == null)
                            EasterEventController.ParticipantList = new Dictionary<PlayerMobile, int>();
                        if (EasterEventController.PointList == null)
                            EasterEventController.PointList = new Dictionary<PlayerMobile, int>();
                        if (!EasterEventController.ParticipantList.ContainsKey(pm))
                            EasterEventController.ParticipantList.Add(pm, 0);
                        if (!EasterEventController.PointList.ContainsKey(pm))
                            EasterEventController.PointList.Add(pm, 0);
                    }

                    foreach (Item egg in pm.Backpack.Items.Where(i => i is EasterEggsPurified).ToArray())
                    {
                        egg.Delete();
                        if (egg.Amount > 0)
                            count += egg.Amount;
                        else
                        {
                            count++;
                        }
                    }

                    foreach (Item egg in pm.BankBox.Items.Where(i => i is EasterEggsPurified).ToArray())
                    {
                        egg.Delete();
                        if (egg.Amount > 0)
                            count += egg.Amount;
                        else
                        {
                            count++;
                        }
                    }

                    if (EasterEventController.Instance != null)
                    {
                        EasterEventController.ParticipantList[pm] += count;
                        EasterEventController.PointList[pm] += count;
                    }

                    if (EasterEventController.Instance != null && count > 0)
                    {
                        pm.SendMessage(54,
                            "You have turned in {0} purified Easter eggs.  You have now have {1} Easter event point(s) to spend.",
                            count, EasterEventController.PointList[pm]);
                    }
                    else
                    {
                        pm.SendMessage(54, "You did not have any purified Easter eggs to turn in.");
                    }

                    pm.SendGump(new EasterRewardsUI(pm));
                    break;
                }

                case 3:
                {
                    if (pm == null || EasterEventController.PointList == null)
                        return;

                    pm.SendGump(new EasterItemsUI(pm));
                    break;
                }
            }

        }
    }
}