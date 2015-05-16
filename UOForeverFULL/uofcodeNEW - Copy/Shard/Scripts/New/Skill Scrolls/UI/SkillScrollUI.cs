#region References
using System;

using Server.Gumps;
using Server.Items;
using Server.Mobiles;

using VitaNex.SuperGumps;
using VitaNex.SuperGumps.UI;
using VitaNex.Targets;

#endregion

namespace Server.Items
{
	public sealed class SkillScrollUI : DialogGump
	{
        private readonly SkillScroll _SkillScroll;

        public SkillScrollUI(
			PlayerMobile user,
			Gump parent = null,
			SkillScroll scroll = null,
			Action<GumpButton> onAccept = null,
			Action<GumpButton> onCancel = null)
			: base(user, parent, null, 100, null, null, 23001, onAccept, onCancel)
		{
            _SkillScroll = scroll;

			CanMove = true;
			Modal = false;

		}

		protected override void CompileLayout(SuperGumpLayout layout)
		{
            layout.Add("body", () =>
            {
                AddBackground(0, 0, 275, 263, 5170);
                AddImage(24, 29, 23001);
                AddLabel(89, 43, 1271, "UO Forever Skill Scroll");
                AddLabel(24, 88, 0, "This scroll entitles the bearer or one");
                AddLabel(24, 108, 0, "of their pets to:");

                AddImageTiled(74, 151, 120, 2, 2620);
                AddLabel(75, 134, 1258, _SkillScroll.SkillName + ": +" + _SkillScroll.SkillBonus);
                AddItem(37, 134, _SkillScroll.ItemID, _SkillScroll.Hue);

                AddLabel(35, 164, 0, "Who do you wish to use this on?");

                AddLabel(47, 189, 1258, "Yourself?");
                AddButton(58, 211, 4023, 4025, b =>
                {
                    _SkillScroll.ApplySkilltoPlayer(User);
                });

                AddLabel(184, 189, 1258, "A pet?");
                AddButton(188, 211, 4023, 4025, b =>
                {
                    User.Target = new MobileSelectTarget<Mobile>((m, t) => _SkillScroll.GetPet(m, t), m => { });
                });
            });

		}
	}
}