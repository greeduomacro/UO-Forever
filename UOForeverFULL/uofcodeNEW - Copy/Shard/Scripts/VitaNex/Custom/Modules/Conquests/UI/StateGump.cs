#region References
using System;
using System.Drawing;

using Server.Mobiles;

using VitaNex.SuperGumps;
#endregion

namespace Server.Engines.Conquests
{
	public class ConquestStateGump : SuperGump
	{
		public ConquestState State { get; set; }

		public ConquestStateGump(PlayerMobile user, ConquestState state)
			: base(user, null, null, null)
		{
			State = state;

			Modal = false;

			CanClose = true;
			CanDispose = true;
			CanMove = true;
			CanResize = true;

			ForceRecompile = true;
		}

		protected override void CompileLayout(SuperGumpLayout layout)
		{
			base.CompileLayout(layout);

			layout.Add(
				"conquest/bg",
				() =>
				{
					const int x = 35, y = 64, w = 315, h = 80;

					AddBackground(x, y, w, h, 2620);
					AddAlphaRegion(x + 5, y + 5, w - 10, h - 10);
				});

			layout.Add("conquest/dragon", () => AddImage(0, 0, 10400)); //dragon

			layout.Add(
				"conquest/badge",
				() =>
				{
					AddImage(20, 65, 1417); //plate

					if (State.Conquest.Hue <= 0)
					{
						AddItem(35, 85, State.Conquest.ItemID);
					}
					else
					{
						AddItem(35, 85, State.Conquest.ItemID, State.Conquest.Hue);
					}
				});

			layout.Add("conquest/name", () =>
			{
				string name = State.Conquest.Name ?? String.Empty;

				AddLabelCropped(105, 70, 170, 20, HighlightHue, name);
			});
			layout.Add("conquest/desc", () =>
			{
				string desc = State.Conquest.Desc ?? String.Empty;
				
				AddLabelCropped(105, 92, 170, 45, TextHue, desc);
			});

			layout.Add(
				"state/icon",
				() =>
				{
					if (State.WorldFirst)
					{
						AddImage(282, 75, 5608); //globe
					}
					else
					{
						AddImage(280, 74, 5545); //uo icon
					}
				});

			layout.Add(
				"conquest/points",
				() =>
				{
					string points = State.Conquest.Points.ToString("#,0") + " CP";

					points = points.WrapUOHtmlTag("big");
					points = points.WrapUOHtmlTag("center");
					points = points.WrapUOHtmlColor(Color.SkyBlue);

					//AddImageTiled(285, 95, 50, 20, 2624);
					AddAlphaRegion(285, 95, 50, 20);
					AddHtml(285, 95, 50, 40, points, false, false); // points
				});
		}
	}
}