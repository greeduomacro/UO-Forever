#region References
using System;

using Server.Mobiles;

using VitaNex.SuperGumps;
#endregion

namespace Server.Twitch
{
	public class ActionCamUI : SuperGump
	{
		public ActionCamUI(PlayerMobile cam)
			: base(cam, null, 0, 0)
		{
			CanClose = true;
			CanDispose = true;
			CanMove = true;
			CanResize = false;
		}

		protected override void CompileLayout(SuperGumpLayout layout)
		{
			base.CompileLayout(layout);

			layout.Add(
				"everything",
				() =>
				{
					AddImage(628, 14, 10410);
					AddBackground(27, 83, 633, 129, 9200);
					AddAlphaRegion(50, 89, 584, 118);
					AddImageTiled(27, 85, 21, 126, 10464);
					AddImageTiled(636, 85, 21, 126, 10464);
					AddImage(313, 44, 9000);
					AddLabel(60, 92, 1258, @"Ultima Online Forever Action Camera");

					AddLabel(409, 92, 2049, "Current Location:");

					AddLabelCropped(526, 92, 110, 16, 1258, String.IsNullOrWhiteSpace(User.Region.Name) ? "Unknown" : User.Region.Name);

					AddLabel(230, 149, 2049, @"TOTAL KILLS: ");
					AddLabel(335, 149, 137, ActionCams.CurrentDeathCount.ToString("#,0"));

					AddImageTiled(58, 111, 225, 1, 5410);

					AddLabel(62, 165, 2049, @"Total Player Murders: ");
					AddLabel(212, 165, 137, ActionCams.CurrentPlayerMurders.ToString("#,0"));

					AddLabel(62, 188, 2049, @"Top Murderer: ");

					if (ActionCams.TopPlayerMurderer != null)
					{
						AddLabel(155, 188, 137, ActionCams.TopPlayerMurderer.RawName);
					}

					AddLabel(409, 165, 2049, @"Total Monster Murders: ");
					AddLabel(573, 165, 137, ActionCams.CurrentMonsterMurders.ToString("#,0"));

					AddLabel(411, 188, 2049, @"Most Lethal Monster: ");

					if (ActionCams.TopMonsterMurderer != null)
					{
						AddLabelCropped(547, 188, 90, 16, 137, ActionCams.TopMonsterMurderer.RawName);
					}

					AddImage(627, 154, 10412);
				});
		}
	}
}