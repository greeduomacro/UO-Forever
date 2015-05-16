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
    public class PokerPlayerState : LegendState<PokerPlayerObject>
    {
        public override string TableName { get { return "PokerPlayers"; } }

        protected override void OnCompile(PokerPlayerObject o, IDictionary<string, SimpleType> data)
        {
            if (o == null)
            {
                data.Clear();
                return;
            }

            data.Add("PokerPlayerId", o.PokerPlayerId.ValueHash);

            data.Add("PokerGameId", o.PokerGameId.ValueHash);

            data.Add("char_id", o.Serial);
            data.Add("char_name", o.charname);
            data.Add("Credit", o.Credit);
            data.Add("Debit", o.Debit);
            data.Add("Bankroll", o.Bankroll);
            data.Add("Folded", o.Folded);
            string holecards = null;
            foreach (var card in o.HoleCards)
            {
                holecards += card.GetRankLetterExport() + card.GetSuitLetter();
            }
            data.Add("HoleCards", holecards);
        }
    }
}