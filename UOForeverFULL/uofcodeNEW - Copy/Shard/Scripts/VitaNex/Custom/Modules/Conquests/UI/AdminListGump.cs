#region Header
//   Vorspire    _,-'/-'/  Admin.cs
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

using VitaNex.SuperGumps.UI;
#endregion

namespace Server.Engines.Conquests
{
	public sealed class ConquestAdminListGump : ListGump<Conquest>
	{
		public ConquestAdminListGump(PlayerMobile user, Gump parent = null)
			: base(user, parent, emptyText: "There are no conquests to display.", title: "Conquests Control Panel")
		{
			ForceRecompile = true;
		}

		public override string GetSearchKeyFor(Conquest key)
		{
			return key != null ? key.Name : base.GetSearchKeyFor(null);
		}

		protected override int GetLabelHue(int index, int pageIndex, Conquest entry)
		{
			return entry != null ? (entry.Enabled ? HighlightHue : ErrorHue) : base.GetLabelHue(index, pageIndex, null);
		}

		protected override string GetLabelText(int index, int pageIndex, Conquest entry)
		{
			return entry != null ? String.Format("{0}", entry.Name) : base.GetLabelText(index, pageIndex, null);
		}

		protected override void CompileList(List<Conquest> list)
		{
			list.Clear();
            list.TrimExcess();
			list.AddRange(Conquests.ConquestRegistry.Values);

			base.CompileList(list);
		}

		protected override void CompileMenuOptions(MenuGumpOptions list)
		{
			if (User.AccessLevel >= Conquests.Access)
			{
				list.AppendEntry(new ListGumpEntry("System Options", OpenConfig, HighlightHue));
				list.AppendEntry(new ListGumpEntry("Add Conquest", AddConquest, HighlightHue));

				list.AppendEntry(
					new ListGumpEntry(
						"Delete All",
						button => Send(
							new ConfirmDialogGump(User, this)
							{
								Title = "Delete All Conquests?",
								Html =
									"All conquests in the database will be deleted, erasing all data associated with them.\nThis action can not be reversed.\n\nDo you want to continue?",
								AcceptHandler = subButton =>
								{
									Conquest[] conquests = Conquests.ConquestRegistry.Values.Where(c => c != null && !c.Deleted).ToArray();

									conquests.ForEach(c => c.Delete());

									Conquests.ConquestRegistry.Clear();

									Refresh(true);
								}
							}),
						ErrorHue));

                list.AppendEntry(
                    new ListGumpEntry(
                        "Disable All Conquests",
                        button => Send(
                            new ConfirmDialogGump(User, this)
                            {
                                Title = "Disable All Conquests?",
                                Html =
                                    "All conquests in the database will be disabled, which will also reset their states.\n\nDo you want to continue?",
                                AcceptHandler = subButton =>
                                {
                                    Conquest[] conquests = Conquests.ConquestRegistry.Values.Where(c => c != null && !c.Deleted).ToArray();

                                    conquests.ForEach(c => c.Enabled = !c.Enabled);

                                    Refresh(true);
                                }
                            }),
                        ErrorHue));

				list.AppendEntry(new ListGumpEntry("Import XML", b => Conquests.ImportXML(User)));
				list.AppendEntry(new ListGumpEntry("Export XML", b => Conquests.ExportXML(User)));
			}

			//list.AppendEntry(new ListGumpEntry("View Profiles", ShowProfiles));
			list.AppendEntry(new ListGumpEntry("Help", ShowHelp));

			base.CompileMenuOptions(list);
		}

		protected override void SelectEntry(GumpButton button, Conquest entry)
		{
			base.SelectEntry(button, entry);

			var opts = new MenuGumpOptions();

			if (User.AccessLevel >= Conquests.Access)
			{
				opts.AppendEntry(
					new ListGumpEntry(
						"Options",
						b =>
						{
							Refresh();

							User.SendGump(
								new PropertiesGump(User, Selected)
								{
									X = b.X,
									Y = b.Y
								});
						},
						HighlightHue));

				opts.AppendEntry(
					new ListGumpEntry(
						entry.Enabled ? "Disable" : "Enable",
						b1 =>
						{
							entry.Enabled = !entry.Enabled;
							Refresh(true);
						},
						entry.Enabled ? ErrorHue : HighlightHue));

				opts.AppendEntry(
					new ListGumpEntry("Edit Rewards", b => Send(new RewardTypesGump(User, Selected, Hide())), HighlightHue));

				opts.AppendEntry(new ListGumpEntry("Reset", ResetConquest, ErrorHue));
				opts.AppendEntry(new ListGumpEntry("Clear", ClearConquest, ErrorHue));
				opts.AppendEntry(new ListGumpEntry("Delete", DeleteConquest, ErrorHue));
			}

			Send(new MenuGump(User, Refresh(), opts, button));
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
					Title = "Reset Conquest?",
					Html =
						"All data associated with this conquest will be reset.\nThis action can not be reversed!\nDo you want to continue?",
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
					Title = "Clear Conquest?",
					Html =
						"All data associated with this conquest will be cleared.\nThis action can not be reversed!\nDo you want to continue?",
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
					Title = "Delete Conquest?",
					Html =
						"All data associated with this conquest will be deleted.\nThis action can not be reversed!\nDo you want to continue?",
					AcceptHandler = OnDeleteConquestConfirm
				});
		}

		private void OnDeleteConquestConfirm(GumpButton button)
		{
			if (Selected != null)
			{
				Selected.Delete();
			}

			Refresh(true);
		}

		private void AddConquest(GumpButton btn)
		{
			var opts = new MenuGumpOptions();

			Conquests.ConquestTypes.ForEach(t => opts.AppendEntry(new ListGumpEntry(t.Name, b => OnAddConquest(t))));

			Refresh();
			Send(new MenuGump(User, btn.Parent, opts, btn));
		}

		private void OnAddConquest(Type t)
		{
			Selected = Conquests.Create(t);

			Refresh(true);
		}

		private void OpenConfig(GumpButton btn)
		{
			Minimize();

			var p = new PropertiesGump(User, Conquests.CMOptions)
			{
				X = X + btn.X,
				Y = Y + btn.Y
			};

			User.SendGump(p);
		}

		/*private void ShowProfiles(GumpButton button)
		{
			if (User != null && !User.Deleted)
			{
				Send(new ConquestProfileListGump(User, Hide(true)));
			}
		}*/

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