using System;
using Server;
using Server.Gumps;
using Server.Items;
using Server.Network;
using Server.Mobiles;

namespace Server.Gumps
{
    public class XmasDeedGump : Gump
    {
        private Mobile m_Mobile;
        private XmasDeed m_Deed;
        private int m_Year = DateTime.UtcNow.Year;

        public XmasDeedGump(Mobile from, XmasDeed deed)
            : base(20, 30)
        {
            m_Mobile = from;
            m_Deed = deed;

            this.Closable = false;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;
            this.AddPage(0);
            this.AddBackground(-2, 0, 500, 490, 9380);
            this.AddLabel(160, 40, 33, @"Merry Christmas From");
            this.AddLabel(170, 60, 33, @"The Forever Team");
            this.AddLabel(130, 110, 977, @"You have been given a Christmas gift.");
            this.AddLabel(180, 130, 977, @"Choose your present:");

            this.AddButton(50, 185, 2117, 2118, (int)Buttons.Snowglobe, GumpButtonType.Reply, 0);
            this.AddLabel(70, 185, 1413, @"A Snowglobe");

            this.AddButton(50, 220, 2117, 2118, (int)Buttons.Statuette, GumpButtonType.Reply, 0);
            this.AddLabel(70, 220, 1413, @"A Holiday Statuette");

            this.AddButton(50, 255, 2117, 2118, (int)Buttons.Snow, GumpButtonType.Reply, 0);
            this.AddLabel(70, 255, 1413, @"A Pile Of Snow");

            this.AddButton(50, 290, 2117, 2118, (int)Buttons.Tree, GumpButtonType.Reply, 0);
            this.AddLabel(70, 290, 1413, @"A Holiday Tree");

            this.AddButton(50, 325, 2117, 2118, (int)Buttons.Bell, GumpButtonType.Reply, 0);
            this.AddLabel(70, 325, 1413, @"A Holiday Bell");

            this.AddButton(50, 360, 2117, 2118, (int)Buttons.Log, GumpButtonType.Reply, 0);
            this.AddLabel(70, 360, 1413, @"A Yule Log");

            this.AddButton(50, 395, 2117, 2118, (int)Buttons.Candle, GumpButtonType.Reply, 0);
            this.AddLabel(70, 395, 1413, @"A Christmas Candle");

            this.AddButton(250, 185, 2117, 2118, (int)Buttons.SnowPile, GumpButtonType.Reply, 0);
            this.AddLabel(270, 185, 1413, @"A Snow Mound");

            this.AddButton(250, 220, 2117, 2118, (int)Buttons.Snowman, GumpButtonType.Reply, 0);
            this.AddLabel(270, 220, 1413, @"A Snowman");

            this.AddButton(250, 255, 2117, 2118, (int)Buttons.Mistletoe, GumpButtonType.Reply, 0);
            this.AddLabel(270, 255, 1413, @"Mistletoe");

            this.AddButton(250, 290, 2117, 2118, (int)Buttons.Wreath, GumpButtonType.Reply, 0);
            this.AddLabel(270, 290, 1413, @"A Wreath");

            this.AddButton(250, 325, 2117, 2118, (int)Buttons.Plants, GumpButtonType.Reply, 0);
            this.AddLabel(270, 325, 1413, @"A Holiday Plant");

            this.AddButton(250, 360, 2117, 2118, (int)Buttons.Garland, GumpButtonType.Reply, 0);
            this.AddLabel(270, 360, 1413, @"A Holiday Garland");

            this.AddButton(250, 395, 2117, 2118, (int)Buttons.Snowflake, GumpButtonType.Reply, 0);
            this.AddLabel(270, 395, 1413, @"A Snowflake");

            this.AddButton(50, 455, 2119, 2120, (int)Buttons.Cancel, GumpButtonType.Reply, 0);
            this.AddLabel(55, 435, 977, @"I Will Choose Later");

        }

        public enum Buttons
        {
            Snowglobe,
            Statuette,
            Snow,
            Tree,
            Bell,
            Log,
            Candle,
            SnowPile,
            Snowman,
            Mistletoe,
            Wreath,
            Plants,
            Garland,
            Snowflake,
            Cancel,
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile from = state.Mobile;

            switch (info.ButtonID)
            {
                case (int)Buttons.Cancel:
                    {
                        from.CloseGump(typeof(XmasDeedGump));
                        break;
                    }

                case (int)Buttons.Snowglobe:
                    {
                        from.AddToBackpack(new HolidaySnowglobe(m_Year));
                        from.CloseGump(typeof(XmasDeedGump));
                        from.SendMessage(0x35, "Merry Christmas. Enjoy your snowglobe.");
                        m_Deed.Delete();
                        break;
                    }

                case (int)Buttons.Snow:
                    {
                        if (0.1 > Utility.RandomDouble())
                        {
                            from.AddToBackpack(new GlacialSnow(m_Year));
                        }

                        else
                        {
                            from.AddToBackpack(new XmasSnowPile(m_Year));
                        }

                        from.CloseGump(typeof(XmasDeedGump));
                        from.SendMessage(0x35, "Merry Christmas. Enjoy your snow pil.");
                        m_Deed.Delete();
                        break;
                    }

                case (int)Buttons.Statuette:
                    {
                        switch (Utility.Random(2))
                        {
                            case 0: from.AddToBackpack(new HolidayStatuette(m_Year)); break;
                            case 1: from.AddToBackpack(new HolidayBust(m_Year)); break;
                        }

                        from.CloseGump(typeof(XmasDeedGump));
                        from.SendMessage(0x35, "Merry Christmas. Enjoy your Bust.");
                        m_Deed.Delete();
                        break;
                    }

                case (int)Buttons.Tree:
                    {
                        from.AddToBackpack(new HolidayTreeDeed());
                        from.CloseGump(typeof(XmasDeedGump));
                        from.SendMessage(0x35, "Merry Christmas. Enjoy your Tree.");
                        m_Deed.Delete();
                        break;
                    }

                case (int)Buttons.Bell:
                    {
                        from.AddToBackpack(new HolidayBell(m_Year));
                        from.CloseGump(typeof(XmasDeedGump));
                        from.SendMessage(0x35, "Merry Christmas. Enjoy your Bell.");
                        m_Deed.Delete();
                        break;
                    }

                case (int)Buttons.Log:
                    {
                        from.AddToBackpack(new YuleLog(m_Year));
                        from.CloseGump(typeof(XmasDeedGump));
                        from.SendMessage(0x35, "Merry Christmas. Enjoy your yule log.");
                        m_Deed.Delete();
                        break;
                    }

                case (int)Buttons.Candle:
                    {
                        from.AddToBackpack(new HolidayCandle(m_Year));
                        from.CloseGump(typeof(XmasDeedGump));
                        from.SendMessage(0x35, "Merry Christmas. Enjoy your candle.");
                        m_Deed.Delete();
                        break;
                    }

                case (int)Buttons.SnowPile:
                    {
                        switch (Utility.Random(5))
                        {
                            case 0: from.AddToBackpack(new SnowPile1(m_Year)); break;
                            case 1: from.AddToBackpack(new SnowPile2(m_Year)); break;
                            case 2: from.AddToBackpack(new SnowPile3(m_Year)); break;
                            case 3: from.AddToBackpack(new SnowPile4(m_Year)); break;
                            case 4: from.AddToBackpack(new SnowPile5(m_Year)); break;
                        }
                        from.CloseGump(typeof(XmasDeedGump));
                        from.SendMessage(0x35, "Merry Christmas. Enjoy your Snow Pile.");
                        m_Deed.Delete();
                        break;
                    }

                 case (int)Buttons.Snowman:
                    {
                        from.AddToBackpack(new HolidaySnowman(m_Year));
                        from.CloseGump(typeof(XmasDeedGump));
                        from.SendMessage(0x35, "Merry Christmas. Enjoy your Snowman.");
                        m_Deed.Delete();
                        break;
                    }

                    case (int)Buttons.Mistletoe:
                    {
                        from.AddToBackpack(new HolidayMistletoeDeed(m_Year));
                        from.CloseGump(typeof(XmasDeedGump));
                        from.SendMessage(0x35, "Merry Christmas. Enjoy your Mistletoe.");
                        m_Deed.Delete();
                        break;
                    }

                    case (int)Buttons.Wreath:
                    {
                        from.AddToBackpack(new HolidayWreathDeed(m_Year));
                        from.CloseGump(typeof(XmasDeedGump));
                        from.SendMessage(0x35, "Merry Christmas. Enjoy your Wreath.");
                        m_Deed.Delete();
                        break;
                    }

                    case (int)Buttons.Plants:
                    {
                        switch (Utility.Random(4))
                        {
                            case 0: from.AddToBackpack(new HolidayPoinsettia(m_Year)); break;
                            case 1: from.AddToBackpack(new HolidaySnowyTree(m_Year)); break;
                            case 2: from.AddToBackpack(new HolidayDecorativeTopiary(m_Year)); break;
                            case 3: from.AddToBackpack(new HolidayFestiveCactus(m_Year)); break;
                        }
                        from.CloseGump(typeof(XmasDeedGump));
                        from.SendMessage(0x35, "Merry Christmas. Enjoy your Plant.");
                        m_Deed.Delete();
                        break;
                    }

                    case (int)Buttons.Garland:
                    {
                        from.AddToBackpack(new HolidayGarland(m_Year));
                        from.CloseGump(typeof(XmasDeedGump));
                        from.SendMessage(0x35, "Merry Christmas. Enjoy your Garland.");
                        m_Deed.Delete();


                        break;
                    }

                    case (int)Buttons.Snowflake:
                    {
                        from.AddToBackpack(new HolidaySnowflake(m_Year));
                        from.CloseGump(typeof(XmasDeedGump));
                        from.SendMessage(0x35, "Merry Christmas. Enjoy your Snowflake.");
                        m_Deed.Delete();
                        break;
                    }
            }
        }
    }
}