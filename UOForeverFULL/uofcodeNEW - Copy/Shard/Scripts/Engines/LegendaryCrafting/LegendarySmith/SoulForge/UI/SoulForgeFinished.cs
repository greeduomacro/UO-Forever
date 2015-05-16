#region References

using System;
using Server.Gumps;
using Server.Mobiles;

#endregion

namespace VitaNex.SuperGumps.UI
{
    public class SoulForgeFinished : NoticeDialogGump
    {
        public SoulForgeFinished(
            PlayerMobile user,
            Gump parent = null,
            int? x = null,
            int? y = null,
            string title = null,
            Action<GumpButton> onAccept = null)
            : base(user, parent, 0, 0, title, null, 0, onAccept)
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
                    AddBackground(0, 0, 384, 492, 9200);
                    AddItem(296, 22, 18491);
                    AddItem(296, 65, 18494);
                    AddItem(296, 108, 18497);
                    AddItem(296, 151, 18515);
                    AddLabel(145, 231, 1258, @"The Soulforge");
                    AddHtml(5, 251, 372, 190,
                        @"With your notepad from Llewellyn in hand, you are able to make out the meaning of the runes at the base of the hammer. The runes state that you are to ground 20 dragonbone shards and mix it into the base materials of the Soulforge.

You are then to take two intact dragon hearts and place them at the center of the forge.

The next step states that you must place both the soul of the Harrower and the soul of the Devourer into the forge.

To complete the arcane construction of the Soulforge, you must throw the heart of Rikktor, the dragon-king, into the forge. But do so at a distance!

If all goes as planned, Rikktor's heart should act as a catalyst and cause the dragon hearts to ignite. The immense heat radiating from the dragon hearts should cause the Harrower's soul to begin slowly emitting some of its dreadful power. The Devourer's soul will soften the raw, untamed power and make it a malleable force to work with when crafting relics in your Soulforge."
                        .WrapUOHtmlTag("BIG"), true, true);
                    AddItem(296, 194, 18518);
                    AddItem(118, 64, 17003);
                    AddItem(186, 20, 16996);
                    AddItem(140, 20, 16999);
                    AddItem(211, 64, 16997);
                    AddItem(163, 8, 16995);
                    AddItem(164, 32, 17000);
                    AddItem(187, 74, 17001);
                    AddItem(96, 65, 17007);
                    AddItem(141, 74, 17004);
                    AddItem(233, 65, 16998);
                    AddItem(164, 97, 17005);
                    AddItem(210, 72, 17002);
                    AddItem(187, 115, 17006);
                    AddItem(118, 72, 17008);
                    AddItem(141, 116, 17009);
                    AddItem(164, 121, 17010);
                    AddLabel(16, 456, 1258, "Craft the Soulforge?");
                    AddButton(235, 455, 247, 248, OnAccept);
                    AddButton(316, 455, 241, 243, OnCancel);
                    AddButton(353, 8, 4017, 4020, OnCancel);
                });
        }
    }
}