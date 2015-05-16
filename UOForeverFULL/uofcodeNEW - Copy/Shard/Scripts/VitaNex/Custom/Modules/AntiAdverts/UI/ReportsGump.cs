using System;
using System.Collections.Generic;

using Server.Gumps;
using Server.Mobiles;

using VitaNex.SuperGumps.UI;

namespace Server.Misc
{
	public sealed class AntiAvertsReportsGump : ListGump<AntiAdvertsReport>
	{
		public AntiAvertsReportsGump(PlayerMobile user)
			: base(user)
		{
			Title = "Anti-Advert Reports";
			EmptyText = "No reports to display.";

			Sorted = true;
			CanSearch = true;
			CanMove = false;
		}

		protected override void CompileList(List<AntiAdvertsReport> list)
		{
			list.Clear();
			list.AddRange(AntiAdverts.Reports);

			base.CompileList(list);
		}

		protected override void CompileMenuOptions(MenuGumpOptions list)
		{
			list.AppendEntry(
				new ListGumpEntry("Options", () => User.SendGump(new PropertiesGump(User, AntiAdverts.CMOptions)), HighlightHue));

			list.AppendEntry(new ListGumpEntry("Key Words", () => new AntiAdvertsEditKeyWordsGump(User, this).Send(), HighlightHue));

			list.AppendEntry(
				new ListGumpEntry("Whitespace Aliases", () => new AntiAdvertsEditAliasesGump(User, this).Send(), HighlightHue));

			list.AppendEntry(
				new ListGumpEntry(
					"Mark All: Viewed",
					() =>
					{
						AntiAdverts.Reports.ForEach(t => t.Viewed = true);

						User.SendMessage("All reports have been marked as viewed.");
						Refresh(true);
					},
					TextHue));

			list.AppendEntry(
				new ListGumpEntry(
					"Mark All: Not Viewed",
					() =>
					{
						AntiAdverts.Reports.ForEach(t => t.Viewed = false);

						User.SendMessage("All reports have been marked as not viewed.");
						Refresh(true);
					},
					TextHue));

			list.AppendEntry(
				new ListGumpEntry(
					"Delete All",
					() =>
					{
						AntiAdverts.Reports.Clear();

						User.SendMessage("All reports have been deleted.");
						Refresh(true);
					},
					ErrorHue));

			list.AppendEntry(
				new ListGumpEntry(
					"Delete Old",
					() =>
					{
						DateTime expire = DateTime.Now - TimeSpan.FromDays(7);

						AntiAdverts.Reports.RemoveAll(t => t.Date <= expire);

						User.SendMessage("All old reports have been deleted.");
						Refresh(true);
					},
					ErrorHue));

			base.CompileMenuOptions(list);
		}

		protected override void SelectEntry(GumpButton button, AntiAdvertsReport entry)
		{
			base.SelectEntry(button, entry);

			MenuGumpOptions opts = new MenuGumpOptions();

			opts.AppendEntry(
				new ListGumpEntry(
					"View",
					() =>
					{
						entry.Viewed = true;
						Send(
							new NoticeDialogGump(User, Refresh())
							{
								Title = "Anti-Advert Report",
								Html = entry.ToString(),
								Modal = false,
								CanMove = false,
							});
					},
					HighlightHue));

			opts.AppendEntry(
				!entry.Viewed
					? new ListGumpEntry("Mark Viewed", () => entry.Viewed = true)
					: new ListGumpEntry("Mark Not Viewed", () => entry.Viewed = false));

			opts.AppendEntry(new ListGumpEntry("Delete", () => AntiAdverts.Reports.Remove(entry), ErrorHue));

			Send(new MenuGump(User, Refresh(), opts, button));
		}

		protected override int GetLabelHue(int index, int pageIndex, AntiAdvertsReport entry)
		{
			return entry != null ? entry == Selected ? HighlightHue : !entry.Viewed ? TextHue : ErrorHue : ErrorHue;
		}

		protected override string GetLabelText(int index, int pageIndex, AntiAdvertsReport entry)
		{
			return entry != null ? entry.ToString() : String.Empty;
		}

		public override string GetSearchKeyFor(AntiAdvertsReport key)
		{
			return key != null ? key.ToString() : String.Empty;
		}

		public override int SortCompare(AntiAdvertsReport a, AntiAdvertsReport b)
		{
			if (a == b || (a == null && b == null))
			{
				return 0;
			}

			if (a == null)
			{
				return 1;
			}

			if (b == null)
			{
				return -1;
			}

			return a.Date < b.Date ? -1 : a.Date > b.Date ? 1 : 0;
		}
	}
}