#region References
using System;
using System.Collections.Generic;

using Server.Gumps;
#endregion

namespace Server.Poker
{
	public class PokerPlayer
	{
		private PlayerAction m_Action;

		public int Currency { get; set; }
		public int Bet { get; set; }
		public int RoundCurrency { get; set; }
		public bool RequestLeave { get; set; }
		public bool IsAllIn { get; set; }
		public bool Forced { get; set; }
		public bool LonePlayer { get; set; }
		public Mobile Mobile { get; set; }
		public PokerGame Game { get; set; }
		public Point3D Seat { get; set; }
		public DateTime BetStart { get; set; }
		public List<Card> HoleCards { get; private set; }
        public int KickNumber { get; set; }
        public bool Chat { get; set; }
        public int PendingCredit { get; set; }
        public int RoundBet { get; set; }

		public PlayerAction Action
		{
			get { return m_Action; }
			set
			{
				m_Action = value;

				switch (m_Action)
				{
					case PlayerAction.None:
						break;
					default:
						{
							if (Game != null)
							{
								Game.PokerGame_PlayerMadeDecision(this);
							}
						}
						break;
				}
			}
		}

		public bool HasDealerButton { get { return Game.DealerButton == this; } }
		public bool HasSmallBlind { get { return Game.SmallBlind == this; } }
		public bool HasBigBlind { get { return Game.BigBlind == this; } }
		public bool HasBlindBet { get { return Game.SmallBlind == this || Game.BigBlind == this; } }

		public PokerPlayer(Mobile from)
		{
			Mobile = from;
			HoleCards = new List<Card>();
		}

		public void ClearGame()
		{
            Bet = 0;
            RoundCurrency = 0;
            RoundBet = 0;
            HoleCards.Clear();
            Game = null;
            CloseAllGumps();
            IsAllIn = false;
            Forced = false;
            LonePlayer = false;

			m_Action = PlayerAction.None;
		}

		public void AddCard(Card card)
		{
			HoleCards.Add(card);
		}

		public void SetBBAction()
		{
			m_Action = PlayerAction.Bet;
		}

		public HandRank GetBestHand(List<Card> communityCards, out List<Card> bestCards)
		{
			return HandRanker.GetBestHand(GetAllCards(communityCards), out bestCards);
		}

		public List<Card> GetAllCards(List<Card> communityCards)
		{
			var hand = new List<Card>(communityCards);
			hand.AddRange(HoleCards);
			hand.Sort();
			return hand;
		}

		public void CloseAllGumps()
		{
			CloseGump(typeof(PokerTableGump));
			CloseGump(typeof(PokerLeaveGump));
			CloseGump(typeof(PokerJoinGump));
			CloseGump(typeof(PokerBetGump));
		}

		public void CloseGump(Type type)
		{
			if (Mobile != null)
			{
				Mobile.CloseGump(type);
			}
		}

		public void SendGump(Gump toSend)
		{
			if (Mobile != null)
			{
				Mobile.SendGump(toSend);
			}
		}

		public void SendMessage(string message)
		{
			if (Mobile != null)
			{
				Mobile.SendMessage(message);
			}
		}

		public void SendMessage(int hue, string message)
		{
			if (Mobile != null)
			{
				Mobile.SendMessage(hue, message);
			}
		}

		public void TeleportToSeat()
		{
			if (Mobile != null && Seat != Point3D.Zero)
			{
				Mobile.Location = Seat;
			}
		}

		public bool IsOnline()
		{
			return Mobile != null && Mobile.NetState != null && Mobile.NetState.Socket != null &&
				   Mobile.NetState.Socket.Connected;
		}
	}
}