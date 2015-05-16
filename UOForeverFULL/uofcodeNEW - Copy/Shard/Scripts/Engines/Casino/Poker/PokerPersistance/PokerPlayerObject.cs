#region References
using System;
using System.Collections.Generic;

using Server.Gumps;
#endregion

namespace Server.Poker
{
	public class PokerPlayerObject
	{
        public PokerHandSerial PokerGameId { get; set; }
        public PokerPlayerSerial PokerPlayerId { get; set; }

	    public int Serial { get; set; }
        public int Credit { get; set; }
        public int Debit { get; set; }
        public int Bankroll { get; set; }

        public string charname { get; set; }

        public byte Folded { get; set; }

        public List<Card> HoleCards { get; set; }

        public PokerPlayerObject()
	    {
	    }
	}
}