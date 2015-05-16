#region References

using System;
using System.Drawing;
using Server.Gumps;
using Server.Mobiles;

#endregion

namespace VitaNex.SuperGumps.UI
{
    public class WispQuestGiverUI : NoticeDialogGump
    {
        private readonly string HTML;

        public WispQuestGiverUI(
            PlayerMobile user,
            string title = null,
            string html = null,
            Gump parent = null,
            int? x = null,
            int? y = null,
            Action<GumpButton> onAccept = null)
            : base(user, parent, 0, 0, title, null, 0, onAccept)
        {
            Title = title;
            CanMove = true;
            Closable = true;
            Modal = false;

            HTML = html;
        }

        protected override void CompileLayout(SuperGumpLayout layout)
        {
            layout.Add(
                "background/main/",
                () =>
                {
                    AddImageTiled(50, 20, 400, 400, 2624);
                    AddAlphaRegion(50, 20, 400, 400);

                    AddImage(90, 33, 9005);
                    AddHtml(130, 45, 270, 20, Title.WrapUOHtmlColor(Color.White).WrapUOHtmlTag("BIG"), false, false);
                        // Quest Offer
                    AddImageTiled(130, 65, 175, 1, 9101);

                    AddImage(140, 110, 1209);
                    AddHtml(160, 108, 250, 20, Title.WrapUOHtmlColor(Color.Orange).WrapUOHtmlTag("BIG"),
                        false, false);

                    AddHtml(98, 140, 312, 200, HTML.WrapUOHtmlTag("BIG"), false, true);

                    AddButton(85, 355, 247, 248, OnAccept);
                    AddHtml(157, 356, 280, 20, "I accept!".WrapUOHtmlColor(Color.White), false, false); // I accept!

                    AddButton(85, 385, 241, 243, OnCancel);
                    AddHtml(157, 387, 280, 20, "No thanks, I decline.".WrapUOHtmlColor(Color.White), false, false);
                        // No thanks, I decline.

                    AddImageTiled(50, 29, 30, 390, 10460);
                    AddImageTiled(34, 140, 17, 279, 9263);

                    AddImage(48, 135, 10411);
                    AddImage(-16, 285, 10402);
                    AddImage(0, 10, 10421);
                    AddImage(25, 0, 10420);

                    AddImageTiled(83, 15, 350, 15, 10250);

                    AddImage(34, 419, 10306);
                    AddImage(442, 419, 10304);
                    AddImageTiled(51, 419, 392, 17, 10101);

                    AddImageTiled(415, 29, 44, 390, 2605);
                    AddImageTiled(415, 29, 30, 390, 10460);
                    AddImage(425, 0, 10441);

                    AddImage(370, 50, 1417);
                    AddImage(379, 60, 0x15B5);
                });
        }
    }
}