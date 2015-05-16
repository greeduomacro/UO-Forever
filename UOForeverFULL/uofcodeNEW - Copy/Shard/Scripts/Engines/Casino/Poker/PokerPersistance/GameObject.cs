#region References
using System;
using System.Collections.Generic;

using Server.Gumps;
#endregion

namespace Server.Poker
{
	public class GameObject
	{
        public DateTime GameStart { get; set; }
        public DateTime GameEnd { get; set; }

	    public Dictionary<string, int> Players;

        public List<PokerActionObject> PlayerActions;

	    public List<Card> CommunityCards;

	    public GameObject()
	    {
	        Players = new Dictionary<string, int>();
            PlayerActions = new List<PokerActionObject>();
	    }
	}
}