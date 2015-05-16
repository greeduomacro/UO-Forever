#region Header
//   Vorspire    _,-'/-'/  MessageOverview.cs
//   .      __,-; ,'( '/
//    \.    `-.__`-._`:_,-._       _ , . ``
//     `:-._,------' ` _,`--` -: `_ , ` ,' :
//        `---..__,,--'  (C) 2014  ` -'. -'
//        #  Vita-Nex [http://core.vita-nex.com]  #
//  {o)xxx|===============-   #   -===============|xxx(o}
//        #        The MIT License (MIT)          #
#endregion

#region References
using Server.Gumps;
using Server.Mobiles;

using VitaNex.SuperGumps.UI;
#endregion

namespace Server.Engines.CentralGump
{
	public class NewsMessageOverviewGump : HtmlPanelGump<Message>
	{
		public bool UseConfirmDialog { get; set; }

        public NewsMessageOverviewGump(
            PlayerMobile user, Gump parent = null, Message selected = null, bool useConfirm = true)
			: base(user, parent, emptyText: "No message selected.", title: "News Message Overview", selected: selected)
		{
			UseConfirmDialog = useConfirm;
		}

		protected override void Compile()
		{
			base.Compile();

			if (Selected != null)
			{
				Html = Selected.ToHtmlString();
			}
		}

		protected override void CompileMenuOptions(MenuGumpOptions list)
		{
			if (Selected != null)
			{
				if (User.AccessLevel >= CentralGump.Access)
				{
					list.AppendEntry(
						new ListGumpEntry(
							"Edit Author",
							subButton => Send(
								new InputDialogGump(User, this)
								{
									Title = "Message Author",
									Html = "Enter an author name for your message.\n255 characters max.",
									Limit = 255,
									Callback = (subBtn, input) =>
									{
										Selected.Author = input;
										Refresh(true);
									}
								}),
							HighlightHue));

					list.AppendEntry(
						new ListGumpEntry(
							"Edit Title",
							subButton => Send(
								new InputDialogGump(User, this)
								{
									Title = "Message Title",
									Html = "Enter a title for your message.\n255 characters max.",
									Limit = 255,
									Callback = (subBtn, input) =>
									{
										Selected.Title = input;
										Refresh(true);
									}
								}),
							HighlightHue));

					list.AppendEntry(
						new ListGumpEntry(
							"Edit Content",
							subButton =>
							{
								if (Selected.Content.Length <= 512)
								{
                                    if (Parent is MessageContentEditGump)
									{
                                        ((MessageContentEditGump)Parent).Refresh(true);
									}
									else
									{
                                        Send(new MessageContentEditGump(User, Hide(true), Selected));
									}
								}
								else
								{
									Send(
										new NoticeDialogGump(User, this)
										{
											Title = "Can't Continue!",
											Html =
												"The content for the selected message is too long to be edited in-game!\nYou need to edit MOTD.xml in the MOTD saves directory."
										});
								}
							},
							HighlightHue));

					list.AppendEntry(
						new ListGumpEntry(
							"Delete Message",
							subButton =>
							{
								if (Selected != null)
								{
									Selected.Delete();
								}

								Close();
							},
							ErrorHue));
				}
			}

			base.CompileMenuOptions(list);
		}

		protected virtual void OnDeleteMessage(GumpButton button)
		{
			if (Selected == null)
			{
				Close();
				return;
			}

			if (UseConfirmDialog)
			{
				Send(
					new ConfirmDialogGump(User, Refresh())
					{
						Title = "Delete Message?",
						Html =
							"All data associated with this message will be deleted.\nThis action can not be reversed!\nDo you want to continue?",
						AcceptHandler = OnConfirmDeleteProfile
					});
			}
			else
			{
				Selected.Delete();
				Close();
			}
		}

		protected virtual void OnConfirmDeleteProfile(GumpButton button)
		{
			if (Selected != null)
			{
				Selected.Delete();
			}

			Close();
		}
	}
}