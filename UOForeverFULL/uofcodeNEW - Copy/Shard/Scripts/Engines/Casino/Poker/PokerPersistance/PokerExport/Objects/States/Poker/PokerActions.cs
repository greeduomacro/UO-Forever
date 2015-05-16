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
    public class PokerActionState : LegendState<PokerActionObject>
    {
        public override string TableName { get { return "PokerActions"; } }

        protected override void OnCompile(PokerActionObject o, IDictionary<string, SimpleType> data)
        {
            if (o == null)
            {
                data.Clear();
                return;
            }

            data.Add("PokerActionId", o.PokerActionId.ValueHash);
            data.Add("PokerGameId", o.PokerGameId.ValueHash);

            data.Add("PokerPlayerId", o.PokerPlayerId.ValueHash);
            data.Add("Stage", o.Stage);
            data.Add("Step", o.Step);
            data.Add("Type", o.Type);
            data.Add("Amount", o.Amount);
        }
    }
}