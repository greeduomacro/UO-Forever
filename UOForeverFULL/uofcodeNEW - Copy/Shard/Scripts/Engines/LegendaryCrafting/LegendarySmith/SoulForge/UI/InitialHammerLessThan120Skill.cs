#region References
using System;
using System.Drawing;
using Server.Gumps;
using Server.Mobiles;
#endregion

namespace VitaNex.SuperGumps.UI
{
    public class InitialHammer120LessThanSkill : NoticeDialogGump
    {
        public InitialHammer120LessThanSkill(
            PlayerMobile user,
            Gump parent = null,
            int? x = null,
            int? y = null,
            string title = null)
            : base(user, parent, 0, 0, title)
        {
            CanMove = true;
            Closable = true;
            Modal = false;
        }

        protected override void CompileLayout(SuperGumpLayout layout)
        {
            layout.Add(
                "background/main/",
                () =>
                {
                    AddBackground(5, 2, 203, 70, 9200);
                    AddItem(9, 23, 18491);
                    AddItem(41, 25, 18494);
                    AddItem(81, 28, 18497);
                    AddItem(118, 31, 18515);
                    AddItem(155, 28, 18518);
                });
        }
    }
}