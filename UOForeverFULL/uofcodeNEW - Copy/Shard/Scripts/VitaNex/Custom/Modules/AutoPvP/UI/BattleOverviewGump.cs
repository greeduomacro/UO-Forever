#region References
using System;
using System.Collections.Generic;
using System.Linq;

using Server;
using Server.Mobiles;

using VitaNex.SuperGumps;
using VitaNex.SuperGumps.UI;
#endregion

namespace VitaNex.Modules.AutoPvP
{
	public class BattleResultsGump : ListGump<KeyValuePair<PlayerMobile, PvPProfileHistoryEntry>>
	{
		public Dictionary<PlayerMobile, PvPProfileHistoryEntry> BattleStats { get; set; }
		public Dictionary<PlayerMobile, PvPTeam> TeamStats { get; set; }

		public PvPTeam[] Winners { get; set; }

		public int SortType { get; set; }

		public BattleResultsGump(
			PlayerMobile user,
			IDictionary<PlayerMobile, PvPProfileHistoryEntry> battleStats,
			IDictionary<PlayerMobile, PvPTeam> teamStats,
			params PvPTeam[] winners)
			: base(user, null, 150, 100, emptyText: "No Stats to Display.", title: "")
		{
			Winners = winners ?? new PvPTeam[0];

			BattleStats = new Dictionary<PlayerMobile, PvPProfileHistoryEntry>(battleStats);
			TeamStats = new Dictionary<PlayerMobile, PvPTeam>(teamStats);

			EntriesPerPage = 15;

			Modal = true;
			CanClose = false;
			CanMove = true;
			//ForceRecompile = true;
		}

		protected override void CompileList(List<KeyValuePair<PlayerMobile, PvPProfileHistoryEntry>> list)
		{
			list.Clear();
			list.AddRange(BattleStats);

			base.CompileList(list);

			switch (SortType)
			{
				case 0:
					list.Sort((l, r) => Insensitive.Compare(l.Key.RawName, r.Key.RawName));
					break;
				case 1:
					list.Sort((l, r) => r.Value.Kills.CompareTo(l.Value.Kills));
					break;
				case 2:
					list.Sort((l, r) => r.Value.Deaths.CompareTo(l.Value.Deaths));
					break;
				case 3:
					list.Sort((l, r) => r.Value.DamageDone.CompareTo(l.Value.DamageDone));
					break;
				case 4:
					list.Sort((l, r) => r.Value.HealingDone.CompareTo(l.Value.HealingDone));
					break;
				case 5:
					list.Sort((l, r) => r.Value.PointsGained.CompareTo(l.Value.PointsGained));
					break;
			}
		}

		protected override void CompileMenuOptions(MenuGumpOptions list)
		{ }

		protected override void CompileLayout(SuperGumpLayout layout)
		{
			base.CompileLayout(layout);

			layout.Remove("button/header/options");

			layout.Remove("label/header/title");

			layout.AddReplace(
				"background/header/base",
				() =>
				{
					AddBackground(0, 0, 520, 50, 9270);
					AddImageTiled(10, 10, 500, 30, 2624);
				});

			layout.Add("label/header/close", () => AddLabel(450, 15, 1287, "Close"));

			layout.AddReplace("button/header/minimize", () => AddButton(490, 20, 10740, 10742, Close));

			layout.Add(
				"label/header/winners",
				() => AddLabel(180, 14, HighlightHue, "Winners: " + String.Join(", ", Winners.Select(t => t.Name))));

			if (Minimized)
			{
				return;
			}

			layout.AddReplace("imagetiled/body/spacer", () => AddImageTiled(0, 50, 520, 10, 9274));

			Dictionary<int, KeyValuePair<PlayerMobile, PvPProfileHistoryEntry>> range = GetListRange();

			if (range.Count == 0)
			{
				layout.AddReplace(
					"background/body/base",
					() =>
					{
						AddBackground(0, 55, 520, 520, 9270);
						AddImageTiled(10, 65, 500, 500, 2624);
					});

				layout.Remove("imagetiled/body/vsep/0");
			}
			else
			{
				layout.AddReplace(
					"background/body/base",
					() =>
					{
						AddBackground(0, 55, 520, 47 + (range.Count * 30), 9270);
						AddImageTiled(10, 65, 500, 25 + (range.Count * 30), 2624);
					});

				layout.Add(
					"sort/header/name",
					() => AddButton(
						12,
						70,
						0x853,
						0x853,
						b =>
						{
							SortType = 0;
							Refresh(true);
						}));

				layout.Add("imagetiled/header/name", () => AddImageTiled(12, 70, 120, 24, 2624));

				layout.Add("label/header/name", () => AddLabel(15, 70, GetSelectedHue(0), "Name"));

				layout.Add(
					"sort/header/kills",
					() => AddButton(
						115,
						70,
						0x81C,
						0x81C,
						b =>
						{
							SortType = 1;
							Refresh(true);
						}));

				layout.Add("imagetiled/header/kills", () => AddImageTiled(115, 70, 90, 24, 2624));

				layout.Add("label/header/kills", () => AddLabel(125, 70, GetSelectedHue(1), "Kills"));

				layout.Add(
					"sort/header/deaths",
					() => AddButton(
						170,
						70,
						0x81C,
						0x81C,
						b =>
						{
							SortType = 2;
							Refresh(true);
						}));

				layout.Add("imagetiled/header/deaths", () => AddImageTiled(170, 70, 90, 24, 2624));

				layout.Add("label/header/deaths", () => AddLabel(175, 70, GetSelectedHue(2), "Deaths"));

				layout.Add(
					"sort/header/damagedone",
					() => AddButton(
						225,
						70,
						0x7d3,
						0x7d3,
						b =>
						{
							SortType = 3;
							Refresh(true);
						}));

				layout.Add("imagetiled/header/damagedone", () => AddImageTiled(225, 70, 130, 24, 2624));

				layout.Add("label/header/damagedone", () => AddLabel(240, 70, GetSelectedHue(3), "Damage Done"));

				layout.Add(
					"sort/header/healingdone",
					() => AddButton(
						330,
						70,
						0x7d3,
						0x7d3,
						b =>
						{
							SortType = 4;
							Refresh(true);
						}));

				layout.Add("imagetiled/header/healingdone", () => AddImageTiled(330, 70, 130, 24, 2624));

				layout.Add("label/header/healingdone", () => AddLabel(335, 70, GetSelectedHue(4), "Healing Done"));

				layout.Add(
					"sort/header/pointsgained",
					() => AddButton(
						440,
						70,
						0x853,
						0x853,
						b =>
						{
							SortType = 5;
							Refresh(true);
						}));

				layout.Add("imagetiled/header/pointsgained", () => AddImageTiled(440, 70, 70, 24, 2624));

				layout.Add("label/header/pointsgained", () => AddLabel(440, 70, GetSelectedHue(5), "PvP Rating"));

				layout.Add("imagetiled/header/hsep", () => AddImageTiled(10, 95, 500, 5, 9277));

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
						new Rectangle2D(30, 0, 448, 13),
						new Rectangle2D(0, 0, 28, 13),
						new Rectangle2D(480, 0, 28, 13)));

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

			layout.Remove("button/list/select/" + index);

			layout.AddReplace(
				"label/list/entry/" + index, () => AddLabel(15, yOffset, GetLabelHue(index, pIndex, entry), entry.Key.RawName));

			layout.Add(
				"label/list/entry/kills/" + index,
				() => AddLabel(125, yOffset, GetLabelHue(index, pIndex, entry), entry.Value.Kills.ToString("#,0")));

			layout.Add(
				"label/list/entry/deaths/" + index,
				() => AddLabel(175, yOffset, GetLabelHue(index, pIndex, entry), entry.Value.Deaths.ToString("#,0")));

			layout.Add(
				"label/list/entry/damagedone/" + index,
				() => AddLabel(240, yOffset, GetLabelHue(index, pIndex, entry), entry.Value.DamageDone.ToString("#,0")));

			layout.Add(
				"label/list/entry/healingdone/" + index,
				() => AddLabel(340, yOffset, GetLabelHue(index, pIndex, entry), entry.Value.HealingDone.ToString("#,0")));

			layout.Add(
				"label/list/entry/pointsgained/" + index,
				() =>
				AddLabel(
					440,
					yOffset,
					GetLabelHue(index, pIndex, entry),
					(entry.Value.PointsGained - entry.Value.PointsLost).ToString("#,0")));

			if (pIndex < (length - 1))
			{
				layout.Remove("imagetiled/body/hsep/" + index);
			}
		}

		public override string GetSearchKeyFor(KeyValuePair<PlayerMobile, PvPProfileHistoryEntry> key)
		{
			return key.Key.RawName;
		}

		public int GetSelectedHue(int categorynumber)
		{
			return SortType == categorynumber ? 1287 : 2066;
		}

		protected override int GetLabelHue(int index, int pageIndex, KeyValuePair<PlayerMobile, PvPProfileHistoryEntry> entry)
		{
			int teamcolor = TextHue;

			if (TeamStats.ContainsKey(entry.Key))
			{
				teamcolor = TeamStats[entry.Key].Color;
			}

			return entry.Key != null ? (entry.Key == User ? HighlightHue : teamcolor) : ErrorHue;
		}
	}
}