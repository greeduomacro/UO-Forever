#region References
using System;
using Server.Engines.Portals;
using Server.Gumps;
using Server.Mobiles;
#endregion

namespace VitaNex.SuperGumps.UI
{
    public class InvasionScoreGump : NoticeDialogGump
    {
        private int _Score;

        public InvasionScoreGump(
            PlayerMobile user,
            int score,
            Gump parent = null,
            int? x = null,
            int? y = null,
            string title = null,
            string html = null,
            int icon = 7004,
            Action<GumpButton> onAccept = null,
            Action<GumpButton> onCancel = null)
            : base(user, parent, 0, 0, title, html, icon, onAccept, onCancel)
        {
            CanMove = true;
            Closable = true;
            Modal = false;
            _Score = score;
        }

        protected override void CompileLayout(SuperGumpLayout layout)
        {
            AddBackground(685, 527, 102, 53, 9200);
            AddAlphaRegion(690, 534, 91, 39);
            AddLabel(695, 533, 137, @"Invasion Score");
            AddLabel(695, 552, 1258, _Score.ToString());
        }
    }
}