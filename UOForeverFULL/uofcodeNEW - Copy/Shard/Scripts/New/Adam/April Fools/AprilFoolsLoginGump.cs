using System.Collections.Generic;
using Server.Mobiles;
using Server.Network;
using Server.Commands;
using Server.Scripts.New.Adam.NewGuild;

namespace Server.Gumps
{
    public class AprilFoolsGump1 : Gump
    {
        public AprilFoolsGump1()
            : base(200, 100)
        {
            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;

            AddPage(0);
            AddBackground(27, 83, 424, 357, 9200);
            AddImageTiled(30, 85, 21, 349, 10464);
            AddImageTiled(426, 87, 21, 349, 10464);
            AddImage(417, 27, 10441);
            AddImage(207, 44, 9000);
            AddButton(209, 396, 247, 248, 1, GumpButtonType.Reply, 0);
            AddImageTiled(116, 176, 253, 1, 5410);
            AddLabel(198, 151, 1160, @"YOU'VE WON!");
            AddLabel(59, 185, 0, @"You were randomly selected as this months raffle winner!");
            AddLabel(106, 212, 0, @"This months prize is:");
            AddLabel(258, 212, 52, @"an ethereal boura");
            AddItem(218, 242, 18169);
            AddLabel(75, 283, 0, @"To claim your prize, all you need to do is answer this");
            AddLabel(121, 307, 0, @"skill testing question: 2 × 4 + 10 × 3");
            AddImageTiled(185, 340, 129, 24, 3504);
            AddTextEntry(190, 342, 124, 20, 0, 0, @"Answer Here");
            AddLabel(112, 342, 0, @"Answer:");
        }



        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            switch (info.ButtonID)
            {
                case 0:
                    {

                        break;
                    }
                case 1:
                    {
                        TextRelay entry1 = info.GetTextEntry(0);
                        string cat = (entry1 == null ? "" : entry1.Text.Trim());
                        int value;
                        if (int.TryParse(cat, out value))
                        {
                            from.SendGump(new AprilFoolsLoseGump(value));
                            if (AprilFoolsLogin.UsedGump != null && from is PlayerMobile)
                                AprilFoolsLogin.UsedGump.Add(from as PlayerMobile);
                            else if (from is PlayerMobile)
                            {
                                AprilFoolsLogin.UsedGump = new List<PlayerMobile> { @from as PlayerMobile };
                            }
                        }
                        else
                        {
                            from.SendMessage("You must enter a valid number!  Try again.");
                            from.SendGump(new AprilFoolsGump1());
                        }
                        break;
                    }

            }
        }
    }

    public class AprilFoolsLoseGump : Gump
    {
        public AprilFoolsLoseGump(int num)
            : base(200, 100)
        {
            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;

            AddPage(0);
            AddBackground(27, 83, 424, 357, 9200);
            AddImageTiled(30, 85, 21, 349, 10464);
            AddImageTiled(426, 87, 21, 349, 10464);
            AddImage(417, 27, 10441);
            AddImage(207, 44, 9000);
            AddButton(210, 375, 247, 248, 0, GumpButtonType.Reply, 0);
            AddImageTiled(116, 176, 253, 1, 5410);
            AddLabel(173, 151, 37, @"INCORRECT ANSWER");
            AddLabel(140, 185, 0, @"Sorry, that answer is incorrect!");
            AddLabel(145, 239, 0, @"The correct answer was: ");
            AddLabel(310, 239, 52, (num == 38 ? "54" : "38"));
            AddLabel(190, 308, 0, @"Thanks for playing!");



        }



        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            switch (info.ButtonID)
            {
                case 0:
                    {

                        break;
                    }

            }
        }
    }
}
