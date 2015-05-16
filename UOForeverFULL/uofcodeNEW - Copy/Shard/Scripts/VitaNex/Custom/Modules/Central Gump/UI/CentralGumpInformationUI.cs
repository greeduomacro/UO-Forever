#region References

using System;
using System.Drawing;
using Server.Gumps;
using Server.Mobiles;
using VitaNex.SuperGumps;
using VitaNex.SuperGumps.UI;

#endregion

namespace Server.Engines.CentralGump
{
    public sealed class CentralGumpInformationUI : ConfirmDialogGump
    {
        private string HTML;

        public CentralGumpInformationUI(PlayerMobile user, Gump parent, string msg)
            : base(user, parent, 115, 0)
        {
            CanMove = true;
            Modal = false;
            HTML = msg;
        }

        protected override void CompileLayout(SuperGumpLayout layout)
        {
            layout.Add(
                "background",
                () =>
                {
                    AddImageTiled(40, 80, 44, 458, 202);
                    AddImage(40, 39, 206);
                    AddImageTiled(80, 38, 422, 45, 201);
                    AddImage(40, 538, 204);
                    AddImageTiled(495, 71, 44, 469, 203);
                    AddImage(496, 39, 207);
                    AddImageTiled(84, 539, 417, 43, 233);
                    AddImageTiled(75, 82, 446, 459, 200);
                    AddImage(171, 16, 1419);
                    AddImage(248, -1, 1417);
                    AddImage(257, 8, 5545);
                    AddImage(496, 538, 205);
                });
            layout.Add("Help",
                () =>
                {
                    AddImageTiled(81, 92, 420, 12, 50);
                    AddHtml(126, 74, 325, 17,
                        String.Format("<BIG>{0}</BIG>", "Ultima Online: Forever Information System")
                            .WrapUOHtmlColor(KnownColor.GhostWhite), false, false);

                    AddImageTiled(81, 104, 404, 413, 2624);
                    AddHtml(86, 103, 414, 413, HTML, false, true);

                    AddImageTiled(81, 518, 420, 12, 50);
                });
        }
    }
}