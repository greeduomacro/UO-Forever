#region References
using System;
using System.Drawing;
using Server.Gumps;
using Server.Mobiles;
#endregion

namespace VitaNex.SuperGumps.UI
{
    public class SFScrollUI : NoticeDialogGump
    {
        private string _Html;

        public SFScrollUI(
            PlayerMobile user,
            Gump parent = null,
            int? x = null,
            int? y = null,
            string title = null,
            string html = null)
            : base(user, parent, 0, 0, title, html)
        {
            CanMove = true;
            Closable = true;
            Modal = false;

            _Html = html;
        }

        protected override void CompileLayout(SuperGumpLayout layout)
        {
            layout.Add(
                "background/main/",
                () =>
                {
                    AddBackground(0, 0, 374, 256, 9380);
                });
            layout.Add(
                "HTML/main/",
                () =>
                {
                    AddHtml(30, 43, 319, 250, String.Format(_Html)
						  .WrapUOHtmlTag("BIG")
						  .WrapUOHtmlColor(Color.DarkRed, false), false, false);
                });
        }
    }
}