#region References
using System;
using System.Globalization;

using Server.Gumps;
using Server.Network;
#endregion

namespace Server.Poker
{
	public class PokerBetGump : Gump
	{
		private const int COLOR_WHITE = 0xFFFFFF;
		private const int COLOR_YELLOW = 0xFFFF00;
		private const int COLOR_GOLD = 0xFFD700;
		private const int COLOR_BLACK = 0x000001;
		private const int COLOR_GREEN = 0x00FF00;
		private const int COLOR_OFFWHITE = 0xFFFACD;

		private readonly bool m_CanCall;
		private readonly PokerGame m_Game;
		private readonly PokerPlayer m_Player;

		public PokerBetGump(PokerGame game, PokerPlayer player, bool canCall)
			: base(460, 425)
		{
			m_CanCall = canCall;
			m_Game = game;
			m_Player = player;

			Closable = false;
			Disposable = false;
			Dragable = true;
			Resizable = false;
			AddPage(0);

			//this.AddImageTiled( 0, 0, 170, 165, 2624 );
			//this.AddImageTiled( 2, 2, 166, 161, 3604 );
			//this.AddImageTiled( 4, 4, 162, 157, 3504 );
			//this.AddImageTiled( 6, 6, 158, 153, 3604 );
			//this.AddAlphaRegion( 6, 6, 158, 153 );
			AddBackground(0, 0, 200, 130, 9270);

            AddRadio(14, 20, 9727, 9730, true, canCall && (m_Game.CurrentBet - m_Player.RoundBet) > 0 ? (int)Buttons.Call : (int)Buttons.Check);
			AddRadio(14, 50, 9727, 9730, false, (int)Buttons.Fold);
			AddRadio(14, 80, 9727, 9730, false, canCall ? (int)Buttons.Raise : (int)Buttons.Bet);

			AddHtml(50, 24, 60, 45, Color(canCall && (m_Game.CurrentBet - m_Player.RoundBet) > 0 ? "Call" : "Check", COLOR_WHITE), false, false);

            if (canCall && (m_Game.CurrentBet - m_Player.RoundBet) > 0)
            {
                AddHtml(
                    105,
                    24,
                    60,
                    22,
                    Color(
                            m_Game.CurrentBet - player.RoundBet >= player.Currency
                                ? "all-in"
                                : String.Format("{0:#,0}", m_Game.CurrentBet - m_Player.RoundBet),
                        COLOR_GREEN),
                    false,
                    false);
            }

            AddHtml(50, 54, 60, 45, Color("Fold", COLOR_WHITE), false, false);
            AddHtml(50, 84, 60, 45, Color(canCall ? "Raise" : "Bet", COLOR_WHITE), false, false);
            AddTextEntry(105, 84, 60, 22, 455, (int)Buttons.txtBet, game.NextRaise < game.Dealer.BigBlind ? game.Dealer.BigBlind.ToString(CultureInfo.InvariantCulture) : game.NextRaise.ToString());

			AddButton(104, 53, 247, 248, (int)Buttons.Okay, GumpButtonType.Reply, 0);
		}

		public enum Buttons
		{
			None,
			Check,
			Call,
			Fold,
			Bet,
			Raise,
			AllIn,
			txtBet,
			Okay
		}

		public string Center(string text)
		{
			return String.Format("<CENTER>{0}</CENTER>", text);
		}

		public string Color(string text, int color)
		{
			return String.Format("<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", color, text);
		}

		public override void OnResponse(NetState state, RelayInfo info)
		{
			Mobile from = state.Mobile;

			if (from == null)
			{
				return;
			}

			if (m_Game.Players.Peek() != m_Player)
			{
				return;
			}

			if ((Buttons)info.ButtonID != Buttons.Okay)
			{
				return;
			}

			if (info.IsSwitched((int)Buttons.Check))
			{
				m_Player.Action = PlayerAction.Check;
			}
            else if (info.IsSwitched((int)Buttons.Call))
            {
                if (m_Game.CurrentBet >= m_Player.Currency)
                {
                    m_Player.Forced = true;
                    m_Player.Action = PlayerAction.AllIn;
                }
                else
                {
                    m_Player.Bet = m_Game.CurrentBet - m_Player.RoundBet;
                    m_Player.Action = PlayerAction.Call;
                }
            }
			else if (info.IsSwitched((int)Buttons.Fold))
			{
				m_Player.Action = PlayerAction.Fold;
			}
            else if (info.IsSwitched((int)Buttons.Bet))
            {
                int bet;
                var t = info.GetTextEntry((int)Buttons.txtBet);

                Int32.TryParse(t.Text, out bet);

                if (bet < m_Game.Dealer.BigBlind)
                {
                    from.SendMessage(0x22, "Your must bet at least {0:#,0} {1}.", m_Game.BigBlind, m_Game.Dealer.IsDonation ? "donation coins." : "gold.");

                    from.CloseGump(typeof(PokerBetGump));
                    from.SendGump(new PokerBetGump(m_Game, m_Player, m_CanCall));
                }
                else if (bet > m_Player.Currency)
                {
                    from.SendMessage(0x22, "You cannot bet more gold than you currently have!");

                    from.CloseGump(typeof(PokerBetGump));
                    from.SendGump(new PokerBetGump(m_Game, m_Player, m_CanCall));
                }
                else if (bet == m_Player.Currency)
                {
                    m_Player.Action = PlayerAction.AllIn;
                }
                else
                {
                    m_Player.Bet = bet;
                    m_Player.Action = PlayerAction.Bet;
                }
            }
            else if (info.IsSwitched((int)Buttons.Raise)) //Same as bet, but add value to current bet
            {
                int bet;
                var t = info.GetTextEntry((int)Buttons.txtBet);

                Int32.TryParse(t.Text, out bet);

                if (bet > m_Player.Currency)
                {
                    m_Player.Action = PlayerAction.AllIn;
                }
                else if (bet < m_Game.NextRaise && m_Game.NextRaise > m_Game.Dealer.BigBlind)
                {
                    from.SendMessage(0x22, "If you are going to raise a bet, it needs to be by at least double the call amount.");

                    from.CloseGump(typeof(PokerBetGump));
                    from.SendGump(new PokerBetGump(m_Game, m_Player, m_CanCall));
                }
                else if (bet + m_Game.CurrentBet > m_Player.Currency)
                {
                    from.SendMessage(0x22, "You do not have enough {0} to raise by that much.", m_Game.Dealer.IsDonation ? "donation coins" : "gold");

                    from.CloseGump(typeof(PokerBetGump));
                    from.SendGump(new PokerBetGump(m_Game, m_Player, m_CanCall));
                }
                else if (bet + m_Game.CurrentBet == m_Player.Currency)
                {
                    m_Player.Action = PlayerAction.AllIn;
                }
                else
                {
                    m_Player.Bet = bet;
                    m_Player.Action = PlayerAction.Raise;
                }
            }
			else if (info.IsSwitched((int)Buttons.AllIn))
			{
				m_Player.Action = PlayerAction.AllIn;
			}
		}
	}
}