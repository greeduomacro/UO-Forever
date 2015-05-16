/*
[cliloc(s) 1113946 - 1113950]

This crystal portal allows you to teleport directly to a bank or a moongate.<br><br>

For Felucca, say "fel" then same rules as above. So "fel minoc mint" or "fel minoc moongate".
CITY NAMES:<br>
britain, bucs, cove, jhelom, magincia, minoc, moonglow, nujelm, ocllo, serpent, skara, trinsic, vesper, wind, yew
MOONGATE NAMES<br>
moonglow, britain, jhelom, yew, minoc, trinsic, skara, magincia, bucs, vesper<br><br><br>


The same teleportation rules apply regarding criminal flagging, weight, etc.
*/

#region References

using Server.Items;
using Server.Network;

#endregion

namespace Server.Gumps
{
    public class CrystalPortalGump : Gump
    {
        private CrystalPortal CrystalPortal;
        public CrystalPortalGump(Mobile from, CrystalPortal portal)
            : base(25, 25)
        {
            @from.CloseGump(typeof(CrystalPortalGump));

            CrystalPortal = portal;
            Closable = true;
            Disposable = false;
            Dragable = true;
            Resizable = false;

            AddPage(0);
            AddBackground(0, 2, 373, 340, 9200);
            AddHtml(14, 15, 346, 300,
                @"<br><br>This crystal portal allows you to teleport directly to a bank or a moongate.<br><br>Say 'minoc bank' or 'minoc moongate'.<br><br>CITY NAMES:<br>britain, bucs, cove, jhelom, magincia, minoc, moonglow, nujelm, serpent, skara, trinsic, vesper, wind<br><br>MOONGATE NAMES<br>moonglow, britain, jhelom, yew, minoc, trinsic, skara, magincia, bucs, vesper <br><br>The same teleportation rules apply regarding criminal flagging, weight, etc.<br><br>",
                true, true);
            AddLabel(14, 320, 54, "Add Charges?");
            AddButton(100, 319, 2311, 2313, 1, GumpButtonType.Reply, 0);
        }


        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            switch (info.ButtonID)
            {
                case 0:
                    {
                        break;
                    }
                case 1:
                    {
                        CrystalPortal.GetTarget(from);
                        break;
                    }
            }
        }
    }
}