#region References
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;

using Server.Gumps;
using Server.Mobiles;

using VitaNex.SuperGumps;
using VitaNex.SuperGumps.UI;
#endregion

namespace Server.Engines.CustomTitles
{
	public sealed class CustomTitlesGump : ListGump<TitleObject>
	{
		public enum ViewMode
		{
			Title = 0,
			Hue = 1
		}

		private static readonly int[] _Frames =
		{
			1400, 1401, 1402, 1403, 1404, 1405, 1406, 1407, 1408, 1409, 1410, 1411, 1412,
			1413, 1414
		};

		private readonly TitleProfile _TitleProfile;

		private ViewMode _View = ViewMode.Title;
		private TitleRarity _SelectedRarity;

		private string _AddTitleMale;
		private string _AddTitleFemale;

		private TitleRarity _AddRarity;
		private TitleDisplay _AddDisplay;
		private int _AddHue;

		private bool _ForcedClose;

		private int _Frame;
		private bool? _State = true;

		public CustomTitlesGump(PlayerMobile user, TitleProfile titles)
			: base(user, null, 100, 100)
		{
			Modal = false;
			CanMove = true;
			Sorted = true;

			ForceRecompile = true;

			AutoRefreshRate = TimeSpan.FromMilliseconds(100);

			_TitleProfile = titles;
			EntriesPerPage = 11;
		}

		public override int SortCompare(TitleObject a, TitleObject b)
		{
			int result = 0;

			if (a.CompareNull(b, ref result))
			{
				return result;
			}

			result = a.CompareTo(b);

			return result;
		}

		protected override void CompileList(List<TitleObject> list)
		{
			list.Clear();
            list.TrimExcess();

			if (_State == null)
			{
				switch (_View)
				{
					case ViewMode.Title:
						list.AddRange(_TitleProfile.GetTitles(_SelectedRarity));
						break;
					case ViewMode.Hue:
						list.AddRange(_TitleProfile.GetHues(_SelectedRarity));
						break;
				}
			}

			base.CompileList(list);
		}

		protected override void CompileLayout(SuperGumpLayout layout)
		{
			layout.Add("bg", () => AddBackground(0, 0, 655, 470, 2620));
			layout.Add("bg/alpha", () => AddAlphaRegion(10, 10, 635, 450));

			layout.Add("chest/bg", () => AddImage(8, 7, 1415, 0));
			layout.Add("chest/lid", () => AddImage(8, 9, _Frames[_Frame], 0));

			if (_State != null)
			{
				return;
			}

			layout.Add(
				"frame/close",
				() => AddButton(
					628,
					0,
					2640,
					2641,
					b =>
					{
						_ForcedClose = true;
						Close(b);
					}));

			switch (_View)
			{
				case ViewMode.Title:
					CompileTitleViewLayout(layout);
					break;
				case ViewMode.Hue:
					CompileHueViewLayout(layout);
					break;
			}

			Dictionary<int, TitleObject> range = GetListRange();

			if (PageCount > 1)
			{
				layout.AddReplace(
					"widget/body/scrollbar",
					() =>
					AddScrollbarH(
						410,
						305,
						PageCount,
						Page,
						PreviousPage,
						NextPage,
						new Rectangle2D(30, 0, 91, 13),
						new Rectangle2D(0, 0, 28, 13),
						new Rectangle2D(123, 0, 28, 13)));
			}

			if (range.Count == 0)
			{
				layout.Add(
					"label/chest/noentries",
					() =>
					AddLabel(260, 152, 44, "You do not own any " + _SelectedRarity + (_View == ViewMode.Title ? " titles." : " hues.")));
			}
			else
			{
				CompileEntryLayout(layout, range);
			}
		}

		private void CompileTitleViewLayout(SuperGumpLayout layout)
		{
			layout.Add("label/chest/title/title", () => AddLabel(105, 120, 44, "Titles"));
			layout.Add("label/chest/title/selected/title", () => AddLabel(215, 120, 44, "Current Title: "));
			layout.Add(
				"label/chest/title/selected/value",
				() =>
				{
					TitleHue selectedHue = _TitleProfile.SelectedHue;
					Title selectedTitle = _TitleProfile.SelectedTitle;
					Mobile owner = _TitleProfile.Owner ?? User;

					int hue = selectedHue != null ? selectedHue.Hue - 1 : CustomTitles.CMOptions.DefaultTitleHue;
					string title = selectedTitle != null ? selectedTitle.ToString(owner.Female) : String.Empty;

					if (!String.IsNullOrWhiteSpace(title))
					{
						AddLabel(305, 120, hue, title);
					}
				});

			layout.Add(
				"button/chest/title/clear/",
				() => AddButton(
					405,
					120,
					22150,
					22151,
					btn =>
					{
						_TitleProfile.SelectedTitle = null;
						Refresh(true);
					}));

			layout.Add("label/chest/title/switch", () => AddLabel(450, 115, 44, "To Hues: "));
			layout.Add(
				"button/chest/title/switch",
				() => AddButton(
					515,
					115,
					4006,
					4005,
					b =>
					{
						_View = ViewMode.Hue;
						Refresh(true);
					}));

			layout.Add("image/body/title/Hbar", () => AddImageTiled(101, 140, 452, 1, 0x2458));

			CompileRarityLayout(layout);

			if (User.AccessLevel < CustomTitles.Access)
			{
				return;
			}

			layout.Add("image/body/title/Hbaradmintop", () => AddImageTiled(78, 325, 500, 1, 9304));
			//layout.Add("label/body/title/create", () => AddLabel(105, 330, 44, "Create New Title"));
			//layout.Add("image/body/title/Hbarnewunderline", () => AddImageTiled(107, 347, 100, 1, 9304));

			layout.Add("label/body/title/male", () => AddLabel(105, 330, 100, "Male Title"));
			layout.Add("background/body/title/male", () => AddBackground(105, 355, 125, 25, 9350));
			layout.Add(
				"textentry/body/title/male",
				() => AddTextEntryLimited(
					108,
					360,
					125,
					25,
					100,
					_AddTitleMale,
					15,
					(b, t) =>
					{
						if (!String.IsNullOrWhiteSpace(t))
						{
							_AddTitleMale = t;
						}
					}));

			layout.Add("label/body/title/female", () => AddLabel(235, 330, 100, "Female Title"));
			layout.Add("background/body/female", () => AddBackground(235, 355, 125, 25, 9350));
			layout.Add(
				"textentry/body/title/female",
				() => AddTextEntryLimited(
					238,
					360,
					125,
					25,
					100,
					_AddTitleFemale,
					15,
					(b, t) =>
					{
						if (!String.IsNullOrWhiteSpace(t))
						{
							_AddTitleFemale = t;
						}
					}));

			layout.Add("background/body/title/display", () => AddBackground(365, 330, 125, 25, 9350));
			layout.Add(
				"textentry/body/title/display",
				() => AddEnumSelect(
					365,
					330,
					4005,
					4007,
					35,
					2,
					25,
					90,
					TextHue,
					_AddDisplay,
					d =>
					{
						_AddDisplay = d;
						Refresh(true);
					}));

			layout.Add("background/body/title/rarity", () => AddBackground(365, 355, 125, 25, 9350));
			layout.Add(
				"menu/body/title/rarity",
				() => AddEnumSelect(
					365,
					355,
					4005,
					4007,
					35,
					2,
					25,
					90,
					_AddRarity.AsHue(),
					_AddRarity,
					r =>
					{
						_AddRarity = r;
						Refresh(true);
					}));

			layout.Add("button/body/title/help", () => AddButton(495, 360, 22153, 22154, b => DisplayHelp()));
			layout.Add("button/body/title/accept", () => AddButton(525, 357, 4024, 4025, b => AddTitle()));
		}

		private void CompileHueViewLayout(SuperGumpLayout layout)
		{
			layout.Add("label/chest/hue/title", () => AddLabel(105, 120, 44, "Hues"));
			layout.Add("label/chest/hue/selected/title", () => AddLabel(215, 120, 44, "Current Hue: "));
			layout.Add(
				"label/chest/hue/selected/value",
				() =>
				{
					int hue = _TitleProfile.SelectedHue != null ? _TitleProfile.SelectedHue.Hue - 1 : -1;

					if (hue >= 0)
					{
						AddLabel(305, 120, hue, "##### " + (hue + 1));
					}
				});

			layout.Add(
				"button/chest/hue/clear",
				() => AddButton(
					405,
					120,
					22150,
					22151,
					btn =>
					{
						_TitleProfile.SelectedHue = null;
						Refresh(true);
					}));

			layout.Add("label/chest/hue/switch", () => AddLabel(450, 115, 44, "To Titles: "));
			layout.Add(
				"button/chest/hue/switch",
				() => AddButton(
					515,
					115,
					4006,
					4005,
					b =>
					{
						_View = ViewMode.Title;
						Refresh(true);
					}));

			layout.Add("image/body/hue/Hbar", () => AddImageTiled(101, 140, 452, 1, 0x2458));

			CompileRarityLayout(layout);

			if (User.AccessLevel < CustomTitles.Access)
			{
				return;
			}

			layout.Add("image/body/hue/Hbaradmintop", () => AddImageTiled(78, 325, 500, 1, 9304));
			//layout.Add("label/body/hue/create", () => AddLabel(105, 330, 44, "Create New Hue"));
			//layout.Add("image/body/hue/Hbarnewunderline", () => AddImageTiled(107, 347, 100, 1, 9304));

			layout.Add("label/body/hue/value", () => AddLabel(105, 330, 100, "Hue Value"));
			layout.Add("background/body/hue/value", () => AddBackground(105, 355, 125, 25, 9350));
			layout.Add(
				"textentry/body/hue/value",
				() => AddTextEntryLimited(
					108,
					360,
					125,
					25,
					100,
					_AddHue.ToString(CultureInfo.InvariantCulture),
					15,
					(b, t) =>
					{
						if (String.IsNullOrWhiteSpace(t))
						{
							return;
						}

						int value;

						if (Int32.TryParse(t, out value))
						{
							_AddHue = value;
						}
					}));

			layout.Add("background/body/hue/rarity", () => AddBackground(365, 355, 125, 25, 9350));
			layout.Add(
				"menu/body/hue/rarity",
				() => AddEnumSelect(
					365,
					355,
					4005,
					4007,
					35,
					2,
					25,
					90,
					_AddRarity.AsHue(),
					_AddRarity,
					r =>
					{
						_AddRarity = r;
						Refresh(true);
					}));

			layout.Add("button/body/hue/help", () => AddButton(495, 360, 22153, 22154, b => DisplayHelp()));
			layout.Add("button/body/hue/accept", () => AddButton(525, 357, 4024, 4025, b => AddHue()));
		}

		private void CompileRarityLayout(SuperGumpLayout layout)
		{
			int tIndex = 0;

			CustomTitles.Rarities.ForEach(
				rarity =>
				{
					int index = tIndex;
					string name = rarity.ToString();

					switch (_View)
					{
						case ViewMode.Title:
							name += " Title";
							break;
						case ViewMode.Hue:
							name += " Hue";
							break;
					}

					layout.Add("chest/label/rarity" + tIndex, () => AddLabel(105, 150 + (30 * index), rarity.AsHue(), name));

					layout.Add(
						"chest/button/rarity" + tIndex,
						() => AddButton(
							210,
							150 + (30 * index),
							_SelectedRarity == rarity ? 4006 : 4005,
							4005,
							b =>
							{
								_SelectedRarity = rarity;
								Refresh(true);
							}));

					tIndex++;
				});
		}

		protected override void CompileEntryLayout(
			SuperGumpLayout layout, int length, int index, int pIndex, int yOffset, TitleObject entry)
		{
			int xOffset = 0;

			if (pIndex < EntriesPerPage - 5)
			{
				xOffset = 260;
				yOffset = 152 + (pIndex) * 30;
			}
			else if (pIndex < EntriesPerPage)
			{
				xOffset = 405;
				yOffset = 152 + (pIndex - 6) * 30;
			}

			if (entry is Title)
			{
				CompileTitleLayout(layout, index, pIndex, xOffset, yOffset, (Title)entry);
			}
			else if (entry is TitleHue)
			{
				CompileHueEntryLayout(layout, index, pIndex, xOffset, yOffset, (TitleHue)entry);
			}
		}

		private void CompileTitleLayout(SuperGumpLayout layout, int index, int pIndex, int xOffset, int yOffset, Title title)
		{
			layout.AddReplace(
				"button/list/select/" + index, () => AddButton(xOffset, yOffset + 2, 2224, 2224, btn => SelectEntry(btn, title)));

			layout.AddReplace(
				"label/list/entry/" + index,
				() => AddLabel(xOffset + 20, yOffset - 3, GetLabelHue(index, pIndex, title), title.ToString(User.Female)));

			if (User.AccessLevel >= AccessLevel.Administrator)
			{
				layout.Add(
					"button/list/purge/" + index, () => AddButton(xOffset + 125, yOffset, 22150, 22151, btn => PurgeTitle(title)));
			}
		}

		private void CompileHueEntryLayout(
			SuperGumpLayout layout, int index, int pIndex, int xOffset, int yOffset, TitleHue hue)
		{
			layout.AddReplace(
				"button/list/select/" + index, () => AddButton(xOffset, yOffset + 2, 2224, 2224, btn => SelectEntry(btn, hue)));

			layout.AddReplace(
				"label/list/entry/" + index,
				() => AddLabel(xOffset + 20, yOffset - 3, hue.Hue - 1, "##### " + GetLabelText(index, pIndex, hue)));

			if (User.AccessLevel >= AccessLevel.Administrator)
			{
				layout.Add(
					"button/list/purge/" + index, () => AddButton(xOffset + 125, yOffset, 22150, 22151, btn => PurgeHue(hue)));
			}
		}

		protected override void OnAutoRefresh()
		{
			if (_State == null)
			{
				AutoRefresh = false;
				return;
			}

			if (_State.Value)
			{
				_Frame++;

				if (_Frame == _Frames.Length - 1)
				{
					AutoRefresh = true;
					_State = null;
				}
			}
			else
			{
				if (_Frame <= 0)
				{
					Close(true);
					return;
				}

				_Frame--;
			}

			base.OnAutoRefresh();
		}

		public override void Close(bool all = false)
		{
			if (!all && AutoRefresh && _State == false)
			{
				return;
			}

			if (IsOpen)
			{
				if (!all)
				{
					if (_ForcedClose)
					{
						_State = false;
						AutoRefresh = true;
						Refresh(true);
						return;
					}

					_State = false;
					AutoRefresh = true;
					Refresh(true);
					return;
				}

				AutoRefresh = false;
				_State = null;
			}

			_ForcedClose = false;

			base.Close(true);
		}

		protected override void OnSend()
		{
			if (_State != null)
			{
				AutoRefresh = true;
			}

			base.OnSend();
		}

		protected override void SelectEntry(GumpButton button, TitleObject entry)
		{
			base.SelectEntry(button, entry);

			switch (_View)
			{
				case ViewMode.Title:
					_TitleProfile.SelectedTitle = entry as Title;
					break;
				case ViewMode.Hue:
					_TitleProfile.SelectedHue = entry as TitleHue;
					break;
			}

			Refresh(true);
		}

		public void AddTitle()
		{
			string result;

			bool success = CustomTitles.CreateTitle(_AddTitleMale, _AddTitleFemale, _AddRarity, _AddDisplay, out result) != null;

			if (success)
			{
				ClearVars();
			}

			Refresh(success);

			new NoticeDialogGump(User)
			{
				Title = "Add Title Result",
				Html = result,
				HtmlColor = success ? Color.LawnGreen : Color.OrangeRed
			}.Send();
		}

		public void PurgeTitle(Title title)
		{
			if (title == null)
			{
				Refresh();
				return;
			}

			string result;

			bool success = CustomTitles.PurgeTitle(title, out result);

			Refresh(success);

			new NoticeDialogGump(User)
			{
				Title = "Purge Title Result",
				Html = result,
				HtmlColor = success ? Color.LawnGreen : Color.OrangeRed
			}.Send();
		}

		public void PurgeHue(TitleHue hue)
		{
			if (hue == null)
			{
				Refresh();
				return;
			}

			string result;

			bool success = CustomTitles.PurgeHue(hue, out result);

			Refresh(success);

			new NoticeDialogGump(User)
			{
				Title = "Purge Hue Result",
				Html = result,
				HtmlColor = success ? Color.LawnGreen : Color.OrangeRed
			}.Send();
		}

		public void AddHue()
		{
			string result;

			bool success = CustomTitles.CreateHue(_AddHue, _AddRarity, out result) != null;

			if (success)
			{
				ClearVars();
			}

			Refresh(success);

			new NoticeDialogGump(User)
			{
				Title = "Add Hue Result",
				Html = result,
				HtmlColor = success ? Color.LawnGreen : Color.OrangeRed
			}.Send();
		}

		public void DisplayHelp()
		{
			string title = "Help: ";
			var html = new StringBuilder();

			html.Append(String.Empty.WrapUOHtmlColor(DefaultHtmlColor));

			switch (_View)
			{
				case ViewMode.Title:
					{
						title += "Add Title";

						html.AppendLine("To create a new title, you must specify a male and female title.");
						html.AppendLine("Rarity is selected using the sub-menu, common is the default value.");
						html.AppendLine("Display is selected using the sub-menu, before-name is the default value.");
						html.AppendLine();
					}
					break;
				case ViewMode.Hue:
					{
						title += "Add Hue";

						html.AppendLine("To create a new hue, you must specify at least the hue value.");
						html.AppendLine("Rarity is selected using the sub-menu, common is the default.");
						html.AppendLine();
					}
					break;
			}

			Refresh();

			new NoticeDialogGump(User)
			{
				Title = title,
				Html = html.ToString(),
				HtmlColor = DefaultHtmlColor
			}.Send();
		}

		public void ClearVars()
		{
			_AddHue = 0;
			_AddRarity = TitleRarity.Common;
			_AddDisplay = TitleDisplay.BeforeName;
			_AddTitleFemale = null;
			_AddTitleMale = null;
		}
	}
}