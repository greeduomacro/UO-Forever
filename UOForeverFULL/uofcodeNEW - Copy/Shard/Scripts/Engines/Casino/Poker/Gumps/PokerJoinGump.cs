#region References
using System;
using System.Globalization;

using Server.Gumps;
using Server.Mobiles;
using Server.Network;
#endregion

namespace Server.Poker
{
	public class PokerJoinGump : Gump
	{
		private readonly PokerGame m_Game;

		public PokerJoinGump(Mobile from, PokerGame game)
			: base(50, 50)
		{
			m_Game = game;

            AddBackground(0, 0, 400, 285, 9270);
            AddAlphaRegion(135, 10, 125, 24);
            AddHtml(14, 14, 371, 24, String.Format("<CENTER><BIG><BASEFONT COLOR=#FFFFFF>{0}</BASEFONT></BIG></CENTER>", "Join Poker Table"), false, false);
            AddImageTiled(20, 34, 347, 2, 9277);
            AddImageTiled(34, 36, 347, 2, 9277);

            AddHtml(20, 41, 365, 130, String.Format("<LEFT><BASEFONT COLOR=#F7D358>{0}</BASEFONT><</LEFT>", "You are asking to join a poker table. If you are unfamiliar with the rules of Texas Hold'em, or are uneasy about " +
                                                                           "losing " + (m_Game.Dealer.IsDonation ? "donation coins" : "gold") + ", then you are advised against proceeding. Only specify a buy-in amount that you would be comfortable losing--all bets are made with real gold and there are no refunds."), false, false);
		    if (!m_Game.Dealer.IsDonation)
		    {
		        AddImageTiled(20, 135, 347, 2, 9277);
		        AddImageTiled(34, 137, 347, 2, 9277);
		    }

		    AddHtml(52, 150, 100, 25, String.Format("<LEFT><BASEFONT COLOR=#FFFFFF>{0}</BASEFONT></LEFT>", "Small Blind:"), false, false);
            AddLabel(125, 150, 1258, m_Game.Dealer.SmallBlind.ToString("#,0") + " " + (m_Game.Dealer.IsDonation ? "donation coins" : "gold"));
            AddHtml(64, 170, 100, 25, String.Format("<LEFT><BASEFONT COLOR=#FFFFFF>{0}</BASEFONT></LEFT>", "Big Blind:"), false, false);
            AddLabel(125, 170, 1258, m_Game.Dealer.BigBlind.ToString("#,0") + " " + (m_Game.Dealer.IsDonation ? "donation coins" : "gold"));
            AddHtml(53, 190, 100, 25, String.Format("<LEFT><BASEFONT COLOR=#FFFFFF>{0}</BASEFONT></LEFT>", "Min Buy-In:"), false, false);
            AddLabel(125, 190, 1258, m_Game.Dealer.MinBuyIn.ToString("#,0") + " " + (m_Game.Dealer.IsDonation ? "donation coins" : "gold"));
            AddHtml(50, 210, 100, 25, String.Format("<LEFT><BASEFONT COLOR=#FFFFFF>{0}</BASEFONT></LEFT>", "Max Buy-In:"), false, false);
            AddLabel(125, 210, 1258, m_Game.Dealer.MaxBuyIn.ToString("#,0") + " " + (m_Game.Dealer.IsDonation ? "donation coins" : "gold"));
            AddHtml(40, 230, 100, 25, String.Format("<LEFT><BASEFONT COLOR=#FFFFFF>{0}</BASEFONT></LEFT>", "Bank Balance:"), false, false);

            int balance = Banker.GetBalance(from, m_Game.TypeOfCurrency);

            AddLabel(125, 230, balance >= m_Game.Dealer.MinBuyIn ? 63 : 137, balance.ToString("#,0") + " " + (m_Game.Dealer.IsDonation ? "donation coins" : "gold"));

            AddHtml(31, 250, 100, 25, String.Format("<LEFT><BASEFONT COLOR=#FFFFFF>{0}</BASEFONT></LEFT>", "Buy-In Amount:"), false, false);

            AddImageTiled(125, 250, 80, 19, 0xBBC);
            AddAlphaRegion(125, 250, 80, 19);
            AddTextEntry(128, 250, 77, 19, 99, (int)Handlers.txtBuyInAmount,m_Game.Dealer.MinBuyIn.ToString());

            AddButton(250, 250, 247, 248, (int)Handlers.btnOkay, GumpButtonType.Reply, 0);
            AddButton(320, 250, 242, 241, (int)Handlers.btnCancel, GumpButtonType.Reply, 0);
		}

		public enum Handlers
		{
			btnOkay = 1,
			btnCancel,
			txtBuyInAmount
		}

	    public override void OnResponse(NetState state, RelayInfo info)
	    {
	        try
	        {
	            if (state == null || info == null || m_Game == null || state.Mobile == null)
	                return;
	            Mobile from = state.Mobile;

	            if (from == null)
	                return;

	            int buyInAmount;

	            if (info.ButtonID != 1)
	            {
	                return;
	            }

	            int balance = Banker.GetBalance(from, m_Game.TypeOfCurrency);

	            if (balance < m_Game.Dealer.MinBuyIn)
	            {
	                from.SendMessage(
	                    0x22,
	                    "You may not join with that amount of {0}. Minimum buy-in: {1:#,0}, Maximum buy-in: {2:#,0}",
	                    (m_Game.Dealer.IsDonation ? "donation coins" : "gold"),
	                    m_Game.Dealer.MinBuyIn,
	                    m_Game.Dealer.MaxBuyIn);

	                return;
	            }

	            var t = info.GetTextEntry(3);

	            if (t == null)
	                return;

	            if (!Int32.TryParse(t.Text, out buyInAmount))
	            {
	                from.SendMessage(0x22, "Use numbers without commas to input your buy-in amount (ie 25000)");
	                return;
	            }

	            if (m_Game.Dealer == null)
	                return;

	            if (buyInAmount > balance || buyInAmount < m_Game.Dealer.MinBuyIn || buyInAmount > m_Game.Dealer.MaxBuyIn)
	            {
	                from.SendMessage(
	                    0x22,
	                    "You may not join with that amount of {0}. Minimum buy-in: {1:#,0}, Maximum buy-in: {2:#,0}",
	                    (m_Game.Dealer.IsDonation ? "donation coins" : "gold"),
	                    m_Game.Dealer.MinBuyIn,
	                    m_Game.Dealer.MaxBuyIn);

	                return;
	            }

	            if (m_Game != null)
	                m_Game.AddPlayer(
	                    new PokerPlayer(from)
	                    {
	                        Currency = buyInAmount
	                    });
	        }
            catch { }
	    }
	}
}