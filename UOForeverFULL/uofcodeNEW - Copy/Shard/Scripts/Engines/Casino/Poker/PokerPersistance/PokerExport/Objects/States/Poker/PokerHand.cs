#region References
using System;
using System.Collections.Generic;
using System.Linq;

using Server;
using Server.Factions;
using Server.Items;
using Server.Poker;
using VitaNex.Modules.UOFLegends;

#endregion

namespace Server.Poker
{
    public class PokerHandState : LegendState<PokerHandObject>
    {
        public override string TableName { get { return "PokerGames"; } }

        protected override void OnCompile(PokerHandObject o, IDictionary<string, SimpleType> data)
        {
            if (o == null)
            {
                data.Clear();
                return;
            }

            data.Add("PokerGameId", o.PokerGameId.ValueHash);
            data.Add("InitialPlayers", o.InitialPlayers);
            data.Add("FinalPlayers", o.FinalPlayers);

            data.Add("FinalPot", o.FinalPot);

            data.Add("StartTime", o.StartTime.ToString("yyyy-MM-dd HH:mm:ss"));
            data.Add("EndTime", o.EndTime.ToString("yyyy-MM-dd HH:mm:ss"));
            string communitycards = null;
            foreach (var card in o.Community)
            {
                communitycards += card.GetRankLetterExport() + card.GetSuitLetter();
            }
            data.Add("Community", communitycards);
        }
    }
}