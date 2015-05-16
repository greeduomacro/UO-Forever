#region References
using System;
using System.Globalization;
using System.Linq;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;
#endregion

namespace Server.Poker
{
	public class PokerRebuy : Gump
	{
		private readonly PokerGame m_Game;

        public PokerRebuy(Mobile from, PokerGame game)
			: base(50, 50)
		{
			m_Game = game;
            PokerPlayer pokerplayer = m_Game.GetPlayer(from);
            AddBackground(0, 0, 400, 300, 9270);
            AddAlphaRegion(92, 10, 215, 24);
            AddHtml(14, 14, 371, 24, String.Format("<CENTER><BIG><BASEFONT COLOR=#FFFFFF>{0}</BASEFONT></BIG></CENTER>", "Rebuy in to the Poker Game?"), false, false);
            AddImageTiled(20, 34, 347, 2, 9277);
            AddImageTiled(34, 36, 347, 2, 9277);

            AddHtml(20, 41, 365, 130, String.Format("<LEFT><BASEFONT COLOR=#F7D358>{0}</BASEFONT><</LEFT>", "Are you sure you want to rebuy in to the poke game? Your new buy-in + your current chips must be greater than or equal to the minimum buy-in and cannot exceed the maximum buy-in.  Please note that this is an easy way for irresponsible people to lose money.  No gold will be refunded."), false, false);
            AddImageTiled(20, 150, 347, 2, 9277);
            AddImageTiled(34, 152, 347, 2, 9277);

            AddHtml(53, 160, 100, 25, String.Format("<LEFT><BASEFONT COLOR=#FFFFFF>{0}</BASEFONT></LEFT>", "Min Buy-In:"), false, false);
            AddLabel(125, 160, 1258, m_Game.Dealer.MinBuyIn.ToString("#,0") + " " + (m_Game.Dealer.IsDonation ? "donation coins" : "gold"));
            AddHtml(50, 180, 100, 25, String.Format("<LEFT><BASEFONT COLOR=#FFFFFF>{0}</BASEFONT></LEFT>", "Max Buy-In:"), false, false);
            AddLabel(125, 180, 1258, m_Game.Dealer.MaxBuyIn.ToString("#,0") + " " + (m_Game.Dealer.IsDonation ? "donation coins" : "gold"));
            AddHtml(40, 200, 100, 25, String.Format("<LEFT><BASEFONT COLOR=#FFFFFF>{0}</BASEFONT></LEFT>", "Bank Balance:"), false, false);

            int balance = Banker.GetBalance(from, m_Game.TypeOfCurrency);

            AddLabel(125, 200, balance + pokerplayer.Currency >= m_Game.Dealer.MinBuyIn ? 63 : 137, balance.ToString("#,0") + " " + (m_Game.Dealer.IsDonation ? "donation coins" : "gold"));

            AddHtml(34, 220, 100, 25, String.Format("<LEFT><BASEFONT COLOR=#FFFFFF>{0}</BASEFONT></LEFT>", "Current Chips:"), false, false);
            AddLabel(125, 220, 1258, pokerplayer.Currency.ToString("#,0") + " " + (m_Game.Dealer.IsDonation ? "donation coins" : "gold"));

            AddHtml(32, 240, 100, 25, String.Format("<LEFT><BASEFONT COLOR=#FFFFFF>{0}</BASEFONT></LEFT>", "Pending Credit:"), false, false);
            AddLabel(125, 240, 1258, pokerplayer.PendingCredit.ToString("#,0") + " " + (m_Game.Dealer.IsDonation ? "donation coins" : "gold"));

            AddHtml(31, 260, 100, 25, String.Format("<LEFT><BASEFONT COLOR=#FFFFFF>{0}</BASEFONT></LEFT>", "Buy-In Amount:"), false, false);

            AddImageTiled(125, 260, 80, 19, 0xBBC);
            AddAlphaRegion(125, 260, 80, 19);
            if (pokerplayer.Currency + pokerplayer.PendingCredit > m_Game.Dealer.MaxBuyIn)
                AddTextEntry(128, 260, 77, 19, 99, (int)Handlers.txtBuyInAmount, 0.ToString());
            else
            {
                AddTextEntry(128, 260, 77, 19, 99, (int)Handlers.txtBuyInAmount, (m_Game.Dealer.MaxBuyIn - pokerplayer.Currency - pokerplayer.PendingCredit).ToString());                
            }

            AddButton(250, 260, 247, 248, (int)Handlers.btnOkay, GumpButtonType.Reply, 0);
            AddButton(320, 260, 242, 241, (int)Handlers.btnCancel, GumpButtonType.Reply, 0);
		}

		public enum Handlers
		{
			btnOkay = 1,
			btnCancel,
			txtBuyInAmount
		}

	    public override void OnResponse(NetState state, RelayInfo info)
	    {
	        Mobile from = state.Mobile;
	        int buyInAmount;

	        if (info.ButtonID != 1)
	        {
	            return;
	        }

	        PokerPlayer pokerplayer = m_Game.GetPlayer(from);

	        if (pokerplayer != null)
	        {
	            int balance = Banker.GetBalance(from, m_Game.TypeOfCurrency);

	            int currency = pokerplayer.Currency;

	            if (balance + currency < m_Game.Dealer.MinBuyIn)
	            {
	                from.SendMessage(
	                    0x22,
	                    "You do not have enough {0} to buy back in to the game. Minimum buy-in: {1:#,0}",
                        (m_Game.Dealer.IsDonation ? "donation coins" : "gold"),
	                    m_Game.Dealer.MinBuyIn);

	                return;
	            }


	            var t = info.GetTextEntry(3);

	            if (!Int32.TryParse(t.Text, out buyInAmount))
	            {
	                from.SendMessage(0x22, "Use numbers without commas to input your buy-in amount (ie 25000)");
	                return;
	            }

                if (buyInAmount > balance)
                {
                    from.SendMessage(
                        0x22,
                        "You do not have enough {0} to cover the specified buy-in amount.",
                        (m_Game.Dealer.IsDonation ? "donation coins" : "gold"));

                    return;
                }

                if (buyInAmount + currency < m_Game.Dealer.MinBuyIn)
                {
                    from.SendMessage(
                        0x22,
                        "You must at least specify an amount that equals the minimum buy-in. Minimum buy-in: {0:#,0}",
                        m_Game.Dealer.MinBuyIn);

                    return;
                }

                if (buyInAmount + currency > m_Game.Dealer.MaxBuyIn)
                {
                    from.SendMessage(
                        0x22,
                        "The specified buy-in amount + your current chips would put you over the max allowable buy-in.  Maximum buy-in: {0:#,0}",
                        m_Game.Dealer.MaxBuyIn);

                    return;
                }

                pokerplayer.PendingCredit = buyInAmount;
                pokerplayer.CloseGump(typeof(PokerTableGump));
                pokerplayer.SendGump(new PokerTableGump(m_Game, pokerplayer));
	        }
}
	}
}