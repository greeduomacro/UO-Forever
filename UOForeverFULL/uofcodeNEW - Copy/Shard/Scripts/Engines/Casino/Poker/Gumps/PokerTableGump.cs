#region References
using System;

using Server.Gumps;
#endregion

namespace Server.Poker
{
	public class PokerTableGump : Gump
	{
		private const int RED = 38;
		private const int BLACK = 0;
		private const int CARD_X = 300;
		private const int CARD_Y = 270;

		private const int COLOR_WHITE = 0xFFFFFF;
		private const int COLOR_YELLOW = 0xFFFF00;
		private const int COLOR_GOLD = 0xFFD700;
		private const int COLOR_BLACK = 0x111111;
		private const int COLOR_GREEN = 0x00FF00;
		private const int COLOR_OFF_WHITE = 0xFFFACD;
		private const int COLOR_PINK = 0xFF0099;

		private readonly PokerGame m_Game;
		private readonly PokerPlayer m_Player;

		public PokerTableGump(PokerGame game, PokerPlayer player)
			: base(0, 0)
		{
			m_Game = game;
			m_Player = player;

			Closable = false;
			Disposable = true;
			Dragable = true;
			Resizable = false;
			AddPage(0);

			if (m_Game.State > PokerGameState.PreFlop)
			{
				DrawCards();
			}

			DrawPlayers();
		}

		private void DrawPlayers()
		{
			const int RADIUS = 240;
			const int centerY = CARD_Y + RADIUS;

			int centerX = CARD_X + (m_Game.State < PokerGameState.Turn ? 15 : m_Game.State < PokerGameState.River ? 30 : 45);

            if (m_Game.State > PokerGameState.Inactive && m_Game.CommunityCurrency > 0)
            {
                AddItem(centerX+30, 340, 3823, m_Game.Dealer.IsDonation ? 1153 : 0);
                AddHtml((centerX + 30) - 78, 343, 200, 20, String.Format("<BASEFONT COLOR=#{1}><B><BIG><CENTER>{0}</CENTER></BIG></B></BASEFONT>",
                    m_Game.CommunityCurrency.ToString("#,0"), m_Game.Dealer.IsDonation ? "FFD700" : "FFFACD"), false, false);
            }

			if (m_Game.State > PokerGameState.DealHoleCards)
			{
				const int lastY = centerY - 85;

				int lastX = centerX;

				foreach (Card c in m_Player.HoleCards)
				{
					AddBackground(lastX, lastY, 71, 95, 9350);
					AddLabelCropped(lastX + 10, lastY + 5, 80, 60, c.GetSuitColor(), c.GetRankLetter());
					AddLabelCropped(lastX + 6, lastY + 25, 75, 60, c.GetSuitColor(), c.GetSuitString());

					lastX += 30;
				}
			}

			int playerIndex = m_Game.GetIndexFor(m_Player.Mobile);
			int counter = m_Game.Players.Count - 1;

			for (double i = playerIndex + 1; counter >= 0; ++i)
			{
				if (i == m_Game.Players.Count)
				{
					i = 0;
				}

				PokerPlayer current = m_Game.Players[(int)i];
				double xdist = RADIUS * Math.Sin(counter * 2.0 * Math.PI / m_Game.Players.Count);
				double ydist = RADIUS * Math.Cos(counter * 2.0 * Math.PI / m_Game.Players.Count);

				int x = centerX + (int)xdist;
				int y = CARD_Y + (int)ydist;

				AddBackground(x, y, 101, 65, 9270);
				//changed from 9200.  This is the gump that shows your name and gold left.

                if (current.HasDealerButton)
                {
                    AddHtml(
                        x,
                        y - 15,
                        101,
                        45,
                        Color(
                            Center("(Dealer Button)"),
                            COLOR_GREEN),
                        false,
                        false); // changed from COLOR_YELLOW
                }

				AddHtml(
					x,
					y + 15,
					101,
					45,
					Color(
                    Center(current.Mobile.RawName.Length > 8 ? (current.Mobile.RawName).Substring(0, 8) : current.Mobile.RawName),
						m_Game.Players.Peek() == current
							 ? COLOR_GREEN
							 : !m_Game.Players.Round.Contains(current) ? COLOR_OFF_WHITE : COLOR_PINK),
					false,
					false);
				AddHtml(
					x + 2, y + 30, 101, 45, Color(Center("(" + current.Currency.ToString("#,0") + ")"), COLOR_GOLD), false, false);

			    if (current == m_Player)
			    {
			        if (current.RoundBet > 0)
			        {
                        AddItem(x + 27, y - 40, 3823, m_Game.Dealer.IsDonation ? 1153 : 0);
			            AddHtml(x, y - 38, 100, 20,
                            String.Format("<BASEFONT COLOR=#{1}><B><BIG><CENTER>{0}</CENTER></BIG></B></BASEFONT>",
                                current.RoundBet.ToString("#,0"), m_Game.Dealer.IsDonation ? "FFD700" : "FFFACD"), false, false);
			        }
			    }
			    else
			    {
                    if (current.RoundBet > 0)
			        {
                        AddItem(x + 27, y + 70, 3823, m_Game.Dealer.IsDonation ? 1153 : 0);
			            AddHtml(x, y + 72, 100, 20,
                            String.Format("<BASEFONT COLOR=#{1}><B><BIG><CENTER>{0}</CENTER></BIG></B></BASEFONT>",
                                current.RoundBet.ToString("#,0"), m_Game.Dealer.IsDonation ? "FFD700" : "FFFACD"), false, false);
			        }
			    }
			    --counter;
			}
		}

		private void DrawCards()
		{
			const int lastY = CARD_Y;

			int lastX = CARD_X;

			foreach (Card c in m_Game.CommunityCards)
			{
				AddBackground(lastX, lastY, 71, 95, 9350);
				AddLabelCropped(lastX + 10, lastY + 5, 80, 60, c.GetSuitColor(), c.GetRankLetter());
				AddLabelCropped(lastX + 6, lastY + 25, 75, 60, c.GetSuitColor(), c.GetSuitString());

				lastX += 30;
			}
		}

		private static string Center(string text)
		{
			return String.Format("<CENTER>{0}</CENTER>", text);
		}

		private static string Color(string text, int color)
		{
			return String.Format("<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", color, text);
		}
	}
}