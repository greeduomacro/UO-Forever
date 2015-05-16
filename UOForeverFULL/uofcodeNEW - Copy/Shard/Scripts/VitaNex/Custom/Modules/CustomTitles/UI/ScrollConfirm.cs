#region References
using System;

using Server.Gumps;
using Server.Mobiles;

using VitaNex.SuperGumps;
using VitaNex.SuperGumps.UI;
#endregion

namespace Server.Engines.CustomTitles
{
	public sealed class ScrollConfirmGump : DialogGump
	{
		private readonly TitleObject _Value;

		public ScrollConfirmGump(
			PlayerMobile user,
			Gump parent = null,
			TitleObject value = null,
			Action<GumpButton> onAccept = null,
			Action<GumpButton> onCancel = null)
			: base(user, parent, null, 100, null, null, 23001, onAccept, onCancel)
		{
			_Value = value;

			CanMove = true;
			Modal = false;

			Title = String.Empty;
		}

		protected override void Compile()
		{
			if (String.IsNullOrWhiteSpace(Title))
			{
				if (_Value is Title)
				{
					Title = "Use this title scroll?";
				}
				else if (_Value is TitleHue)
				{
					Title = "Use this title hue scroll?";
				}
				else
				{
					Title = "Use this scroll?";
				}
			}

			base.Compile();
		}

		protected override void CompileLayout(SuperGumpLayout layout)
		{
			base.CompileLayout(layout);

			Width = 275;
			Height = 250;

            //Adds are not displaying properly unless they come before removes
            layout.Add("label/body/title", () => AddLabel(25, 75, 0, Title));

            if (_Value is Title)
            {
                CompileTitleLayout(layout, (Title)_Value);
            }
            else if (_Value is TitleHue)
            {
                CompileHueLayout(layout, (TitleHue)_Value);
            }

			layout.Remove("background/header/base");
			layout.Remove("html/body/info");
			layout.Remove("label/header/title");


			layout.AddReplace("background/body/base", () => AddBackground(0, 50, Width, Height, 5170));

			layout.AddReplace("image/body/icon", () => AddImage(20, 100, Icon));

			layout.AddReplace(
				"button/body/cancel",
				() =>
				{
					AddButton(Width - 90, Height - 10, 4018, 4019, OnCancel);
					AddTooltip(1006045);
				});

			layout.AddReplace(
				"button/body/accept",
				() =>
				{
					AddButton(Width - 50, Height - 10, 4015, 4016, OnAccept);
					AddTooltip(1006044);
				});
		}

		private void CompileTitleLayout(SuperGumpLayout layout, Title title)
		{
			layout.Add("label/body/title/grantmessage", () => AddLabel(70, 110, 1457, "This scroll grants the title: "));
			layout.Add("label/body/title/grant", () => AddLabel(110, 145, 1461, title.ToString(User.Female)));

			layout.Add("image/body/title/Hbar", () => AddImageTiled(100, 160, 120, 2, 2620));

			layout.Add("label/body/title/raritymessage", () => AddLabel(25, 185, 1457, "Title Rarity: "));
			layout.Add("label/body/title/rarity", () => AddLabel(105, 185, title.GetRarityHue(), title.Rarity.ToString()));

			layout.Add("label/body/title/owned", () => AddLabel(25, 205, 1457, "People who have this title: "));
			layout.Add("label/body/title/ownednumber", () => AddLabel(193, 205, 1461, title.GetOwnerCount().ToString("#,0")));
		}

		private void CompileHueLayout(SuperGumpLayout layout, TitleHue hue)
		{
			layout.Add("label/body/hue/grantmessage", () => AddLabel(65, 110, 1457, "This scroll grants the title hue: "));
			layout.Add("label/body/hue/granthue", () => AddLabel(110, 145, hue.Hue-1, "##########"));

			layout.Add("image/body/hue/Hbar", () => AddImageTiled(100, 160, 120, 2, 2620));

			layout.Add("label/body/hue/raritymessage", () => AddLabel(25, 185, 1457, "Hue Rarity: "));
			layout.Add("label/body/hue/rarity", () => AddLabel(105, 185, hue.GetRarityHue(), hue.Rarity.ToString()));

			layout.Add("label/body/hue/owned", () => AddLabel(25, 205, 1457, "People who have this hue: "));
			layout.Add("label/body/hue/ownednumber", () => AddLabel(193, 205, 1461, hue.GetOwnerCount().ToString("#,0")));
		}
	}
}