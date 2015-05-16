#region References
using System;
using System.Drawing;
using Server.Gumps;
using Server.Mobiles;
#endregion

namespace VitaNex.SuperGumps.UI
{
    public class InitialHammer120Skill : NoticeDialogGump
    {
        public InitialHammer120Skill(
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
                    AddBackground(0, 0, 384, 443, 9200);
                    AddItem(296, 22, 18491);
                    AddItem(296, 65, 18494);
                    AddItem(296, 108, 18497);
                    AddItem(296, 151, 18515);
                    AddLabel(145, 231, 1258, @"The Soulforge");
                    AddHtml(5, 251, 372, 190, @"With your skilled eyes, you quickly recognize the runes on the base of the ancient hammer as relating to legends about the Soulforge.  On your journey to hone your crafting skills, you have encountered various books dedicated to discussing this legend.  While most think it a fairy tale, this anicent smithing hammer tells you there may be some credince to the legend after all.  You decide that it may be best to seek out one familiar with the legend of the Soulforge for further guidance.  A good place to start would be the Britain Library.  Perhaps Llewellyn, the Royal Archivist, could be of some assistance.".WrapUOHtmlTag("BIG"), true, true);
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
                    AddButton(353, 8, 4017, 4020, OnCancel);
                });
        }
    }
}