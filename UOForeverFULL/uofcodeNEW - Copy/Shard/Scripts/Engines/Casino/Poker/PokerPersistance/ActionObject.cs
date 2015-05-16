#region References
using System;
using System.Collections.Generic;

using Server.Gumps;
#endregion

namespace Server.Poker
{
	public class PokerActionObject
	{
        public PokerHandSerial PokerGameId { get; set; }
        public PokerActionSerial PokerActionId { get; set; }
        public PokerPlayerSerial PokerPlayerId { get; set; }

        public int Step { get; set; }
	    public int Stage { get; set; }
        public int Type { get; set; }
        public int Amount { get; set; }

        public PokerActionObject(PokerHandSerial gid, int step, PokerPlayerSerial serial, int state, int action, int bet)
        {
            PokerGameId = gid;
            PokerPlayerId = serial;
            Stage = state;
            Step = step;
            Type = action;
            Amount = bet;

            PokerActionId = new PokerActionSerial();
        }
	}
}