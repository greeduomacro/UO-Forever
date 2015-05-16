#region References
using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

using Server.Gumps;
using Server.Mobiles;

using VitaNex.SuperGumps;
using VitaNex.SuperGumps.UI;
#endregion

namespace Server.Engines.Conquests
{
	public class ConquestStateListGump : ListGump<ConquestState>
	{
		public ConquestProfile Profile { get; set; }

		public string Category { get; set; }

		public ConquestStateListGump(
			PlayerMobile user,
			Gump parent = null,
			ConquestProfile profile = null,
			string category = "All")
			: base(user, parent, emptyText: "There are no conquests to display.", title: "Conquests")
		{
			Profile = profile ?? Conquests.EnsureProfile(User);

			Category = category.ToUpperWords() ?? "All";

			ForceRecompile = true;
			AutoRefresh = true;

			Sorted = true;
		}

		protected override void Compile()
		{
			if (Profile != null)
			{
				Title = "Conquests for " + Profile.Owner.RawName + " (" + Profile.Registry.Count(s => s.Completed) + "/" +
						Conquests.FindConquests<Conquest>(c => Conquests.Validate(c, Profile.Owner)).Length + ")";
			}

			base.Compile();
		}

		protected override void CompileMenuOptions(MenuGumpOptions list)
		{
			list.Clear();

			if (User.AccessLevel >= Conquests.Access)
			{
				list.AppendEntry(
					new ListGumpEntry(
						"Clear",
						b =>
						Send(
							new ConfirmDialogGump(
								User,
								this,
								title: "Clear Conquests?",
								html:
									"All conquests in the profile will be cleared, erasing all data associated with them.\nThis action can not be reversed.\n\nDo you want to continue?",
								onAccept: OnConfirmClearProfile)),
						HighlightHue));
			}

			if (User != Profile.Owner)
			{
				list.AppendEntry(new ListGumpEntry("My Conquests", OnMyProfile, HighlightHue));
			}

			list.AppendEntry(new ListGumpEntry("Help", ShowHelp));

			base.CompileMenuOptions(list);
		}

		protected override void CompileList(List<ConquestState> list)
		{
			list.Clear();

            list.TrimExcess();

			bool viewSecret = User.AccessLevel >= Conquests.Access;

			Profile.Registry.Not(s => s == null || !s.ConquestExists || !s.Conquest.Enabled || (s.Conquest.Secret && s.Tier <= 0 && !viewSecret))
				   .ForEach(
					   state =>
					   {
						   string cat =
							   (String.IsNullOrWhiteSpace(state.Conquest.Category) ? "Misc" : state.Conquest.Category).ToUpperWords();

						   if (String.IsNullOrWhiteSpace(Category) || Insensitive.Equals(cat, Category) ||
							   Insensitive.Equals(Category, "All"))
						   {
							   list.Add(state);
						   }
					   });

			base.CompileList(list);
		}

		protected override void CompileEntryLayout(
			SuperGumpLayout layout, int length, int index, int pIndex, int yOffset, ConquestState entry)
		{
			base.CompileEntryLayout(layout, length, index, pIndex, yOffset, entry);

			layout.AddReplace(
				"label/list/entry/" + index,
				() =>
				{
					AddHtml(
						65,
						2 + yOffset,
						100,
						20,
						GetLabelText(index, pIndex, entry).WrapUOHtmlColor(GetLabelColor(index, pIndex, entry)),
						false,
						false);

					AddHtml(
						175,
						2 + yOffset,
						85,
						20,
						GetTierLabelText(index, pIndex, entry).WrapUOHtmlColor(GetTierColor(index, pIndex, entry)),
						false,
						false);

					AddHtml(
						270,
						2 + yOffset,
						125,
						20,
						GetProgressLabelText(index, pIndex, entry).WrapUOHtmlColor(GetProgressColor(index, pIndex, entry)),
						false,
						false);
				});
		}

		protected override void SelectEntry(GumpButton button, ConquestState entry)
		{
			base.SelectEntry(button, entry);

			if (button == null || entry == null)
			{
				return;
			}

			if (User.AccessLevel < Conquests.Access)
			{
				Refresh();
				Send(new ConquestStateGump(User, entry));
				return;
			}

			var list = new MenuGumpOptions();

			list.AppendEntry(new ListGumpEntry("View", b => Send(new ConquestStateGump(User, entry)), HighlightHue));

			list.AppendEntry(new ListGumpEntry("Reset", ResetConquest, ErrorHue));
			list.AppendEntry(new ListGumpEntry("Clear", ClearConquest, ErrorHue));
			list.AppendEntry(new ListGumpEntry("Delete", DeleteConquest, ErrorHue));
			
			Send(new MenuGump(User, Refresh(), list, button));
		}

		public override int SortCompare(ConquestState a, ConquestState b)
		{
			int result = 0;

			if (a.CompareNull(b, ref result))
			{
				return result;
			}

			if (a.Completed && b.Completed)
			{
				return Insensitive.Compare(a.Name, b.Name);
			}

			if (a.Completed)
			{
				return 1;
			}

			if (b.Completed)
			{
				return -1;
			}

			double aP = a.Progress / (double)a.ProgressMax;
			double bP = b.Progress / (double)b.ProgressMax;

			if (aP > bP)
			{
				return -1;
			}

			if (aP < bP)
			{
				return 1;
			}

			double aT = a.Tier / (double)a.TierMax;
			double bT = b.Tier / (double)b.TierMax;

			if (aT > bT)
			{
				return -1;
			}

			if (aT < bT)
			{
				return 1;
			}

			return Insensitive.Compare(a.Name, b.Name);
		}

		public override string GetSearchKeyFor(ConquestState key)
		{
			return key != null ? String.Format("{0} {1}", key.Name, key.Conquest.Category) : base.GetSearchKeyFor(null);
		}

		protected virtual void OnConfirmClearProfile(GumpButton button)
		{
			Profile.Clear();
			Refresh(true);
		}

		protected virtual void OnConfirmDeleteConquest(GumpButton button)
		{
			if (Selected == null)
			{
				Refresh(true);
				return;
			}

			Profile.Registry.Remove(Selected);
			Refresh(true);
		}

		protected override string GetLabelText(int index, int pageIndex, ConquestState entry)
		{
			return entry != null ? entry.Name : base.GetLabelText(index, pageIndex, null);
		}

		protected override int GetLabelHue(int index, int pageIndex, ConquestState entry)
		{
			return entry != null && entry.ConquestExists && entry.Conquest.Hue > 0 ? entry.Conquest.Hue : ErrorHue;
		}

		protected virtual Color GetLabelColor(int index, int pageIndex, ConquestState entry)
		{
			return entry != null ? Color.FromKnownColor(entry.Conquest.Color) : Color.White;
		}

		protected virtual string GetTierLabelText(int index, int pageIndex, ConquestState entry)
		{
			return entry != null ? String.Format("Tier {0:#,0}/{1:#,0}", entry.Tier, entry.TierMax) : String.Empty;
		}

		protected virtual Color GetTierColor(int index, int pageIndex, ConquestState entry)
		{
			if (entry == null || !entry.ConquestExists)
			{
				return Color.Yellow;
			}

			double p = entry.Tier / entry.TierMax;

			if (p < 0.50)
			{
				return Color.OrangeRed.Interpolate(Color.Yellow, p / 0.50);
			}

			if (p > 0.50)
			{
				return Color.Yellow.Interpolate(Color.LawnGreen, (p - 0.50) / 0.50);
			}

			return Color.Yellow;
		}

		protected virtual string GetProgressLabelText(int index, int pageIndex, ConquestState entry)
		{
			return entry != null ? String.Format("Progress {0:#,0}/{1:#,0}", entry.Progress, entry.ProgressMax) : String.Empty;
		}

		protected virtual Color GetProgressColor(int index, int pageIndex, ConquestState entry)
		{
			if (entry == null || !entry.ConquestExists)
			{
				return Color.Yellow;
			}

			double p = entry.Progress / entry.ProgressMax;

			if (p < 0.50)
			{
				return Color.OrangeRed.Interpolate(Color.Yellow, p / 0.50);
			}

			if (p > 0.50)
			{
				return Color.Yellow.Interpolate(Color.LawnGreen, (p - 0.50) / 0.50);
			}

			return Color.Yellow;
		}

		private void ResetConquest()
		{
			if (Selected == null)
			{
				return;
			}

			Send(
				new ConfirmDialogGump(User, Refresh())
				{
					Title = "Reset Conquest State?",
					Html =
						"All data associated with this conquest state will be reset.\nThis action can not be reversed!\nDo you want to continue?",
					AcceptHandler = OnResetConquestConfirm
				});
		}

		private void OnResetConquestConfirm(GumpButton button)
		{
			if (Selected != null)
			{
				Selected.Reset();
			}

			Refresh(true);
		}

		private void ClearConquest()
		{
			if (Selected == null)
			{
				return;
			}

			Send(
				new ConfirmDialogGump(User, Refresh())
				{
					Title = "Clear Conquest State?",
					Html =
						"All data associated with this conquest state will be cleared.\nThis action can not be reversed!\nDo you want to continue?",
					AcceptHandler = OnClearConquestConfirm
				});
		}

		private void OnClearConquestConfirm(GumpButton button)
		{
			if (Selected != null)
			{
				Selected.Clear();
			}

			Refresh(true);
		}

		private void DeleteConquest()
		{
			if (Selected == null)
			{
				return;
			}

			Send(
				new ConfirmDialogGump(User, Refresh())
				{
					Title = "Delete Conquest State?",
					Html =
						"All data associated with this conquest state will be deleted.\nThis action can not be reversed!\nDo you want to continue?",
					AcceptHandler = OnDeleteConquestConfirm
				});
		}

		private void OnDeleteConquestConfirm(GumpButton button)
		{
			if (Selected != null)
			{
				Selected.Clear();
				Profile.Registry.Remove(Selected);
			}

			Refresh(true);
		}

		private void OnMyProfile()
		{
			Hide(true);
			Conquests.SendConquestsGump(User);
		}

		private void ShowHelp()
		{
			if (User == null || User.Deleted)
			{
				return;
			}

			StringBuilder sb = ConquestGumpUtility.GetHelpText(User);
			Send(
				new HtmlPanelGump<StringBuilder>(User, Hide(true))
				{
					Selected = sb,
					Html = sb.ToString(),
					Title = "Conquest Help",
					HtmlColor = Color.SkyBlue
				});
		}
	}
}