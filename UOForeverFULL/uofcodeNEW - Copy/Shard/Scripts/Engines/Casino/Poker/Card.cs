#region References
using System;
using System.Globalization;
#endregion

namespace Server.Poker
{
	public class Card : IComparable
	{
		public const int Red = 0x26;
		public const int Black = 0x00;

		public Suit Suit { get; private set; }
		public Rank Rank { get; private set; }

		public string Name { get { return String.Format("{0} of {1}", Rank, Suit).ToLower(); } }
		public string RankString { get { return Rank.ToString().ToLower(); } }

		public Card(Suit suit, Rank rank)
		{
			Suit = suit;
			Rank = rank;
		}

		public string GetRankLetter()
		{
			if ((int)Rank < 11)
			{
				return ((int)Rank).ToString(CultureInfo.InvariantCulture);
			}

			switch (Rank)
			{
				case Rank.Jack:
					return "J";
				case Rank.Queen:
					return "Q";
				case Rank.King:
					return "K";
				case Rank.Ace:
					return "A";
			}

			return "?";
		}

        public string GetRankLetterExport()
        {
            if ((int)Rank < 10)
            {
                return ((int)Rank).ToString(CultureInfo.InvariantCulture);
            }

            switch (Rank)
            {
                case Rank.Ten:
                    return "T";
                case Rank.Jack:
                    return "J";
                case Rank.Queen:
                    return "Q";
                case Rank.King:
                    return "K";
                case Rank.Ace:
                    return "A";
            }

            return "?";
        }

		public int GetSuitColor()
		{
			return ((int)Suit < 3 ? Red : Black);
		}

		public string GetSuitString()
		{
			switch ((int)Suit)
			{
				case 1:
					return "\u25C6";
				case 2:
					return "\u2665";
				case 3:
					return "\u2663";
				case 4:
					return "\u2660";
			}
			return "?";
		}

        public string GetSuitLetter()
        {
            switch ((int)Suit)
            {
                case 1:
                    return "d";
                case 2:
                    return "h";
                case 3:
                    return "c";
                case 4:
                    return "s";
            }
            return "?";
        }

		public override string ToString()
		{
			return String.Format("{0} of {1}", Rank, Suit);
		}

		#region IComparable Members
		public int CompareTo(object obj)
		{
			if (obj is Card)
			{
				var card = (Card)obj;

				if (Rank < card.Rank)
				{
					return 1;
				}
				if (Rank > card.Rank)
				{
					return -1;
				}
			}

			return 0;
		}
		#endregion
	}
}