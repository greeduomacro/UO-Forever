#region References
using System;

using Server.Gumps;
using Server.Mobiles;
#endregion

namespace VitaNex.SuperGumps.UI
{
    public class PortalScoresScrollGump : NoticeDialogGump
    {
        public PortalScoresScrollGump(
            PlayerMobile user,
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
        }

        protected override void CompileLayout(SuperGumpLayout layout)
        {
            AddButton(728, 6, 1262, 1262, OnAccept);
        }
    }
}