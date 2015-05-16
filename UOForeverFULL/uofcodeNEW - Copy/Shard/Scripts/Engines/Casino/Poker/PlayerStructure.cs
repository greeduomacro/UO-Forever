#region References
using System.Collections.Generic;
#endregion

namespace Server.Poker
{
	public class PlayerStructure
	{
		public PokerGame Game { get; private set; }

		public List<PokerPlayer> Players { get; set; }
		public List<PokerPlayer> Round { get; set; }
		public List<PokerPlayer> Turn { get; set; }

		public int Count { get { return Players.Count; } }

		public PokerPlayer this[int index] { get { return Players[index]; } set { Players[index] = value; } }

		public PlayerStructure(PokerGame game)
		{
			Game = game;

			Players = new List<PokerPlayer>();
			Round = new List<PokerPlayer>();
			Turn = new List<PokerPlayer>();
		}

		public void Push(PokerPlayer player)
		{
			if (!Turn.Contains(player))
			{
				Turn.Add(player);
			}
		}

		public PokerPlayer Peek()
		{
			return Turn.Count > 0 ? Turn[Turn.Count - 1] : null;
		}

		public PokerPlayer Prev()
		{
			return Turn.Count > 1 ? Turn[Turn.Count - 2] : null;
		}

		public PokerPlayer Next()
		{
			if (Round.Count == 1 || Turn.Count == Round.Count)
			{
				return null;
			}

			if (Peek() == null) //No turns yet for this round
			{
				if (Game.State == PokerGameState.PreFlop)
				{
					PokerPlayer blind = Game.BigBlind ?? Game.SmallBlind;

					if (blind != null)
					{
						return Round.IndexOf(blind) == Round.Count - 1 ? Round[0] : Round[Round.IndexOf(blind) + 1];
					}
				}

				PokerPlayer dealer = Game.DealerButton;

				if (dealer == null)
				{
					return null;
				}

				return Round.IndexOf(dealer) == Round.Count - 1 ? Round[0] : Round[Round.IndexOf(dealer) + 1];
			}

			return Round.IndexOf(Peek()) == Round.Count - 1 ? Round[0] : Round[Round.IndexOf(Peek()) + 1];
		}

		public bool Contains(PokerPlayer player)
		{
			return Players.Contains(player);
		}

		public void Clear()
		{
			Turn.Clear();
			Round.Clear();
		}
	}
}