using Server.Games;
using Server.Mobiles;
using Server.Network;

namespace Server.Gumps
{
    public class PossessGump : Gump
    {
        Mobile m_CurrentOwner;

        public PossessGump(Mobile owner)  : base(30, 30)
        {
            if (owner != null) { owner.CloseGump(typeof(PossessGump)); }
            m_CurrentOwner = owner;
            BuildGump();
        }

        public static void Initialize()
        {
            EventSink.Login += EventSink_Login;
        }

        static void EventSink_Login(LoginEventArgs e)
        {
            if (e.Mobile != null && CreaturePossession.HasAnyPossessPermissions(e.Mobile.NetState))
            {
                e.Mobile.SendGump(new PossessGump(e.Mobile));
            }
        }

        const int Width = 120;
        const int Height = 92;

        const int BorderMarginX = 5;
        const int BorderMarginY = 7;

        const int BodyMarginX = 9;
        const int BodyMarginY = 11;

        const int TextMarginX = 18;
        const int TextMarginY = 12;

        const int LineHeight = 18;

        enum Button
        {
            Close = -1,
            Connect = 1,
            Disconnect = 2
        }

        void BuildGump()
        {
            Closable = false;

            AddPage(0);

            AddBackground(0, 0, Width, Height, 0xA3C);

            AddImageTiled(BorderMarginX, BorderMarginY, Width-(BorderMarginX*2), Height-(BorderMarginY*2), 0xA40);
            AddAlphaRegion(BorderMarginX, BorderMarginY, Width - (BorderMarginX * 2), Height - (BorderMarginY * 2));

            AddImageTiled(BodyMarginX, BodyMarginY, Width - (BodyMarginX * 2), Height - (BodyMarginY * 2), 0xBBC);

            AddHtml(TextMarginX, TextMarginY, 65, 24, "POSSESS", false, false);
            AddButton(TextMarginX, TextMarginY + (1 * LineHeight), 0x943, 0x942, (int)Button.Connect, GumpButtonType.Reply, 0); // Connection

            if (!(m_CurrentOwner is PlayerMobile))
                AddButton(TextMarginX + 8, TextMarginY + (2 * LineHeight + 5), 0x7D9, 0x7DA, (int)Button.Disconnect, GumpButtonType.Reply, 0); // Logout
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (sender != null)
            {
                Mobile from = sender.Mobile;
                Button pressedButton = (Button)info.ButtonID;
                if (pressedButton == Button.Connect) {
                    CreaturePossession.OnPossessTargetRequest(from);
                    m_CurrentOwner.SendGump(this);
                }
                else if (pressedButton == Button.Disconnect)
                {
                    if (from != null)
                    {
                        if (!CreaturePossession.AttemptReturnToOriginalBody(from.NetState))
                        {
                            from.NetState.Dispose();
                        }
                    }
                }
                else
                {
                    m_CurrentOwner.SendGump(this);
                }
            }
        }

    }
}
