#region References

using Server.Items;
using Server.Network;

#endregion

namespace Server.Gumps
{
    public class CorruptedCrystalPortalGump : Gump
    {
        private CorruptedCrystalPortal CrystalPortal;
        public CorruptedCrystalPortalGump(Mobile from, CorruptedCrystalPortal portal)
            : base(25, 25)
        {
            @from.CloseGump(typeof(CorruptedCrystalPortalGump));

            CrystalPortal = portal;

            Closable = true;
            Disposable = false;
            Dragable = true;
            Resizable = false;

            AddPage(0);
            AddBackground(0, 2, 373, 340, 9200);
            AddHtml(14, 15, 346, 300,
                @"<br><br>This corrupted portal allows you to teleport directly to a dungeon.<br><br>Say 'dungeon shame'.<br><br>DUNGEON NAMES:<br>covetous, deceit, despise, destard, ice, fire, hythloth, orc, shame, wrong, wind.<br><br>The same teleportation rules apply regarding criminal flagging, weight, etc.<br><br>",
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