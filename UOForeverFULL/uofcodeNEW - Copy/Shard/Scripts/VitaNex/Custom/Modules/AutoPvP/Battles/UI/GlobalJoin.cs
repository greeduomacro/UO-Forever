#region References

using System.Drawing;
using Server.Gumps;
using Server.Mobiles;
using VitaNex.SuperGumps;
using VitaNex.SuperGumps.UI;

#endregion

namespace VitaNex.Modules.AutoPvP.Battles
{
    public class GlobalJoinGump : ConfirmDialogGump
    {
        public UOF_PvPBattle Battle { get; private set; }

        public GlobalJoinGump(PlayerMobile user, UOF_PvPBattle battle, int icon = 7022)
            : base(user, title: battle.Name, html: "Would you like to join the queue for this battle?", icon: icon)
        {
            Battle = battle;

            Modal = false;

            CanDispose = true;
            CanClose = true;
            CanMove = true;

            Width = 300;
            Height = 150;

            HtmlColor = Color.White;
        }

        protected override void CompileLayout(SuperGumpLayout layout)
        {
            base.CompileLayout(layout);

            layout.Remove("html/body/info");

            layout.Remove("background/body/base");
            layout.Remove("background/header/base");

            layout.Remove("label/header/title");

            layout.Add("background/body/base1", () => AddBackground(28, 0, Width, 150, 9270));

            layout.AddReplace("image/body/icon", () => AddImage(42, 47, Icon));

            layout.Add("image/body/doodad", () => AddImage(0, 6, 1227));

            layout.Add("image/body/waxseal", () => AddImage(57, 123, 9005));

            layout.Add(
                "label/body/content",
                () =>
                {
                    AddLabel(116, 54, 2049, "Would you like to join the queue");
                    AddLabel(116, 75, 2049, "for this battle?");
                });

            layout.Add("background/header/base1", () => AddBackground(28, 0, Width, 40, 9270));

            layout.Add("label/header/title1", () => AddLabel(44, 10, 52, Title));

            layout.AddReplace(
                "button/header/cancel",
                () =>
                {
                    AddButton(295, 6, 2640, 2641, OnCancel);
                    AddTooltip(1006045);
                });

            layout.AddReplace(
                "button/body/cancel",
                () =>
                {
                    AddButton(250, 108, 2119, 2120, OnCancel);
                    AddTooltip(1006045);
                });

            layout.AddReplace(
                "button/body/accept",
                () =>
                {
                    AddButton(176, 108, 2128, 2129, OnAccept);
                    AddTooltip(1006044);
                });
        }

        protected override void OnAccept(GumpButton button)
        {
            base.OnAccept(button);

            Battle.Enqueue(User);
        }

        protected override void OnCancel(GumpButton button)
        {
            base.OnCancel(button);

            User.SendMessage(54, "You decide not to join the queue.");
        }
    }
}