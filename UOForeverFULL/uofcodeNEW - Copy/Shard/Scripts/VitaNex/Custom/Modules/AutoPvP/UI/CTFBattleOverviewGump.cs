#region References
using System;
using System.Collections.Generic;
using System.Linq;

using Server;
using Server.Mobiles;

using VitaNex.SuperGumps;
#endregion

namespace VitaNex.Modules.AutoPvP
{
	public class CTFBattleResultsGump : BattleResultsGump
	{
		public CTFBattleResultsGump(
			PlayerMobile user,
			IDictionary<PlayerMobile, PvPProfileHistoryEntry> battleStats,
			IDictionary<PlayerMobile, PvPTeam> teamStats,
			params PvPTeam[] winners)
			: base(user, battleStats, teamStats, winners)
		{
			X = 25;
		}

		protected override void CompileList(List<KeyValuePair<PlayerMobile, PvPProfileHistoryEntry>> list)
		{
			base.CompileList(list);

			switch (SortType)
			{
				case 6:
					list.Sort((l, r) => r.Value["Flags Captured"].CompareTo(l.Value["Flags Captured"]));
					break;
				case 7:
					list.Sort((l, r) => r.Value["Flags Stolen"].CompareTo(l.Value["Flags Stolen"]));
					break;
				case 8:
					list.Sort((l, r) => r.Value["Flags Returned"].CompareTo(l.Value["Flags Returned"]));
					break;
				case 9:
					list.Sort((l, r) => r.Value["Walls Cast"].CompareTo(l.Value["Walls Cast"]));
					break;
			}
		}

		protected override void CompileLayout(SuperGumpLayout layout)
		{
			base.CompileLayout(layout);

			layout.AddReplace(
				"background/header/base",
				() =>
				{
					AddBackground(0, 0, 960, 50, 9270);
					AddImageTiled(10, 10, 940, 30, 2624);
				});

			layout.AddReplace("label/header/close", () => AddLabel(890, 15, 1287, "Close"));

			layout.AddReplace("button/header/minimize", () => AddButton(930, 20, 10740, 10742, Close));

			layout.AddReplace(
				"label/header/winners",
				() => AddLabel(400, 14, HighlightHue, "Winners: " + String.Join(", ", Winners.Select(t => t.Name))));

			if (Minimized)
			{
				return;
			}

			layout.AddReplace("imagetiled/body/spacer", () => AddImageTiled(0, 50, 960, 10, 9274));

			Dictionary<int, KeyValuePair<PlayerMobile, PvPProfileHistoryEntry>> range = GetListRange();

			if (range.Count == 0)
			{
				layout.AddReplace(
					"background/body/base",
					() =>
					{
						AddBackground(0, 55, 960, 820, 9270);
						AddImageTiled(10, 65, 940, 800, 2624);
					});

				layout.Remove("imagetiled/body/vsep/0");
			}
			else
			{
				layout.AddReplace(
					"background/body/base",
					() =>
					{
						AddBackground(0, 55, 960, 47 + (range.Count * 30), 9270);
						AddImageTiled(10, 65, 940, 25 + (range.Count * 30), 2624);
					});

				layout.Add(
					"sort/header/flagscaptured",
					() => AddButton(
						440,
						70,
						0x7d3,
						0x7d3,
						b =>
						{
							SortType = 6;
							Refresh(true);
						}));

				layout.Add("imagetiled/header/flagscaptured", () => AddImageTiled(440, 70, 130, 24, 2624));

				layout.Add("label/header/flagscaptured", () => AddLabel(440, 70, GetSelectedHue(6), "Flags Captured"));

				layout.Add(
					"sort/header/flagsassaulted",
					() => AddButton(
						555,
						70,
						0x7d3,
						0x7d3,
						b =>
						{
							SortType = 7;
							Refresh(true);
						}));

				layout.Add("imagetiled/header/flagsassaulted", () => AddImageTiled(555, 70, 130, 24, 2624));

				layout.Add("label/header/flagsassaulted", () => AddLabel(555, 70, GetSelectedHue(7), "Flags Assaulted"));

				layout.Add(
					"sort/header/flagsdefended",
					() => AddButton(
						670,
						70,
						0x7d3,
						0x7d3,
						b =>
						{
							SortType = 8;
							Refresh(true);
						}));

				layout.Add("imagetiled/header/flagsdefended", () => AddImageTiled(670, 70, 130, 24, 2624));

				layout.Add("label/header/flagsdefended", () => AddLabel(670, 70, GetSelectedHue(8), "Flags Defended"));

				layout.AddReplace(
					"sort/header/wallscast",
					() => AddButton(
						785,
						70,
						0x853,
						0x853,
						b =>
						{
							SortType = 9;
							Refresh(true);
						}));

				layout.AddReplace("imagetiled/header/wallscast", () => AddImageTiled(785, 70, 70, 24, 2624));

				layout.AddReplace("label/header/wallscast", () => AddLabel(785, 70, GetSelectedHue(9), "Walls Cast"));

				layout.AddReplace(
					"sort/header/pointsgained",
					() => AddButton(
						880,
						70,
						0x853,
						0x853,
						b =>
						{
							SortType = 5;
							Refresh(true);
						}));

				layout.AddReplace("imagetiled/header/pointsgained", () => AddImageTiled(880, 70, 70, 24, 2624));

				layout.AddReplace("label/header/pointsgained", () => AddLabel(880, 70, GetSelectedHue(5), "PvP Rating"));

				layout.AddReplace("imagetiled/header/hsep", () => AddImageTiled(10, 95, 940, 5, 9277));

				layout.AddReplace(
					"widget/body/scrollbar",
					() =>
					AddScrollbarH(
						6,
						46,
						PageCount,
						Page,
						PreviousPage,
						NextPage,
						new Rectangle2D(30, 0, 888, 13),
						new Rectangle2D(0, 0, 28, 13),
						new Rectangle2D(920, 0, 28, 13)));

				layout.Remove("imagetiled/body/vsep/0");
			}
		}

		protected override void CompileEntryLayout(
			SuperGumpLayout layout,
			int length,
			int index,
			int pIndex,
			int yOffset,
			KeyValuePair<PlayerMobile, PvPProfileHistoryEntry> entry)
		{
			base.CompileEntryLayout(layout, length, index, pIndex, yOffset, entry);

			yOffset += 30;

			layout.Add(
				"label/list/entry/flagscaptured/" + index,
				() => AddLabel(440, yOffset, GetLabelHue(index, pIndex, entry), entry.Value["Flags Captured"].ToString("#,0")));

			layout.Add(
				"label/list/entry/flagsassaulted/" + index,
				() => AddLabel(555, yOffset, GetLabelHue(index, pIndex, entry), entry.Value["Flags Stolen"].ToString("#,0")));

			layout.Add(
				"label/list/entry/flagsdefended/" + index,
				() => AddLabel(670, yOffset, GetLabelHue(index, pIndex, entry), entry.Value["Flags Returned"].ToString("#,0")));

			layout.Add(
				"label/list/entry/wallscast/" + index,
				() => AddLabel(785, yOffset, GetLabelHue(index, pIndex, entry), entry.Value["Walls Cast"].ToString("#,0")));

			layout.AddReplace(
				"label/list/entry/pointsgained/" + index,
				() =>
				AddLabel(
					880,
					yOffset,
					GetLabelHue(index, pIndex, entry),
					(entry.Value.PointsGained - entry.Value.PointsLost).ToString("#,0")));
		}
	}
}