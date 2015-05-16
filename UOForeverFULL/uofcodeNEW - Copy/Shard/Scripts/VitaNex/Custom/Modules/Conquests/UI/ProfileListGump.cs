#region Header
//   Vorspire    _,-'/-'/  ProfileList.cs
//   .      __,-; ,'( '/
//    \.    `-.__`-._`:_,-._       _ , . ``
//     `:-._,------' ` _,`--` -: `_ , ` ,' :
//        `---..__,,--'  (C) 2014  ` -'. -'
//        #  Vita-Nex [http://core.vita-nex.com]  #
//  {o)xxx|===============-   #   -===============|xxx(o}
//        #        The MIT License (MIT)          #
#endregion

#region References
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

using Server.Gumps;
using Server.Mobiles;

using VitaNex.SuperGumps;
using VitaNex.SuperGumps.UI;
#endregion

namespace Server.Engines.Conquests
{
	public class ConquestProfileListGump : ListGump<ConquestProfile>
	{
		public ConquestProfileListGump(PlayerMobile user, Gump parent = null)
			: base(user, parent, emptyText: "There are no profiles to display.", title: "Conquest Profiles")
		{
			ForceRecompile = true;

			Sorted = true;

			CanMove = false;
			CanResize = false;
		}

		public override int SortCompare(ConquestProfile a, ConquestProfile b)
		{
			if (a.CheckNull(b))
			{
				return 0;
			}

			long x = a.GetPointsTotal();
			long y = b.GetPointsTotal();

			if (x > y)
			{
				return -1;
			}

			if (x < y)
			{
				return 1;
			}

			if (a.Owner.CheckNull(b.Owner))
			{
				return 0;
			}

			return Insensitive.Compare(a.Owner.RawName, b.Owner.RawName);
		}

		protected override void Compile()
		{
			base.Compile();

			Title = String.Format("Conquest Profiles ({0:#,0})", List.Count);
		}

		protected override void CompileMenuOptions(MenuGumpOptions list)
		{
			list.Clear();

			if (User.AccessLevel >= Conquests.Access)
			{
				list.AppendEntry(
					new ListGumpEntry(
						"Delete All",
						button => Send(
							new ConfirmDialogGump(User, this)
							{
								Title = "Delete All Profiles?",
								Html =
									"All profiles in the database will be deleted, erasing all data associated with them.\nThis action can not be reversed.\n\nDo you want to continue?",
								AcceptHandler = subButton =>
								{
									var profiles = Conquests.Profiles.Values.Where(p => p != null).ToArray();

									Conquests.Profiles.Clear();

									profiles.ForEach(p => p.Clear());

									Refresh(true);
								}
							}),
						ErrorHue));
			}

			list.AppendEntry(new ListGumpEntry("My Conquests", OnMyProfile, HighlightHue));

			list.AppendEntry(new ListGumpEntry("Help", ShowHelp));

			base.CompileMenuOptions(list);
		}

		protected override void CompileList(List<ConquestProfile> list)
		{
			list.Clear();

            list.TrimExcess();
			list.AddRange(Conquests.Profiles.Values.Where(p => p != null));

			base.CompileList(list);
		}

		public override string GetSearchKeyFor(ConquestProfile key)
		{
			return key != null && key.Owner != null ? key.Owner.RawName : base.GetSearchKeyFor(key);
		}

		protected override void CompileEntryLayout(
			SuperGumpLayout layout, int length, int index, int pIndex, int yOffset, ConquestProfile entry)
		{
			base.CompileEntryLayout(layout, length, index, pIndex, yOffset, entry);

			layout.AddReplace(
				"label/list/entry/" + index,
				() =>
				{
					AddLabelCropped(65, 2 + yOffset, 160, 20, GetLabelHue(index, pIndex, entry), GetLabelText(index, pIndex, entry));
					AddLabelCropped(
						225, 2 + yOffset, 150, 20, GetSortLabelHue(index, pIndex, entry), GetSortLabelText(index, pIndex, entry));
				});
		}

		protected override int GetLabelHue(int index, int pageIndex, ConquestProfile entry)
		{
			return index < 3
					   ? HighlightHue
					   : (entry != null
							  ? Notoriety.GetHue(Notoriety.Compute(User, entry.Owner))
							  : base.GetLabelHue(index, pageIndex, null));
		}

		protected override string GetLabelText(int index, int pageIndex, ConquestProfile entry)
		{
			return entry != null && entry.Owner != null
					   ? String.Format("{0}: {1}", (index + 1).ToString("#,0"), entry.Owner.RawName)
					   : base.GetLabelText(index, pageIndex, entry);
		}

		protected virtual string GetSortLabelText(int index, int pageIndex, ConquestProfile entry)
		{
			return entry != null ? String.Format("Points: {0:#,0}", entry.GetPointsTotal()) : String.Empty;
		}

		protected virtual int GetSortLabelHue(int index, int pageIndex, ConquestProfile entry)
		{
			return entry != null ? ((index < 3) ? HighlightHue : TextHue) : ErrorHue;
		}

		protected override void SelectEntry(GumpButton button, ConquestProfile profile)
		{
			base.SelectEntry(button, profile);

			if (profile == null || profile.Owner == null)
			{
				return;
			}

			if (Conquests.CMOptions.UseCategories)
			{
                Send(new ConquestStatesGump(User, Hide(true), profile));
			}
			else
			{
				Send(new ConquestStateListGump(User, Hide(true), profile));
			}
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