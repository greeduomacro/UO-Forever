#region References
using System;
using System.Collections.Generic;

using Server.Gumps;
#endregion

namespace Server.Poker
{
	public class PokerHandObject
	{
        public PokerHandSerial PokerGameId { get; set; }

        public int InitialPlayers { get; set; }
	    public int FinalPlayers { get; set; }

        public int FinalPot { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public List<Card> Community { get; set; }

	    public List<PokerActionObject> Actions;
	    public List<PokerPlayerObject> Players;
	}
}