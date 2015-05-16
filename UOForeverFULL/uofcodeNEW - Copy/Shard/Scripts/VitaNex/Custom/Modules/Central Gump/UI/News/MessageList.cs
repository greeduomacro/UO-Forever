#region Header
//   Vorspire    _,-'/-'/  MessageList.cs
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
using System.Linq;
using Server.Engines.CentralGump;
using Server.Gumps;
using Server.Mobiles;
using VitaNex;
using VitaNex.Crypto;
using VitaNex.SuperGumps.UI;
#endregion

namespace Server.Engines.CentralGump
{
	public class NewsMessageListGump : ListGump<Message>
	{
		private InputDialogGump _NewMessageInput;

		public bool UseConfirmDialog { get; set; }

        public NewsMessageListGump(PlayerMobile user, Gump parent = null, bool useConfirm = true)
			: base(user, parent, emptyText: "There are no messages to display.", title: "News Messages")
		{
			UseConfirmDialog = useConfirm;

			ForceRecompile = true;
			CanMove = false;
			CanResize = false;
		}

        protected override string GetLabelText(int index, int pageIndex, Message entry)
		{
			return entry != null ? entry.ToString() : base.GetLabelText(index, pageIndex, null);
		}

        protected override int GetLabelHue(int index, int pageIndex, Message entry)
        {
            return 2049;
        }

		protected override void CompileMenuOptions(MenuGumpOptions list)
		{
			if (User.AccessLevel >= CentralGump.Access)
			{
				list.AppendEntry(
					new ListGumpEntry(
						"Load Messages",
						button =>
						{
							CentralGump.Messages.Import();
							Refresh(true);
						},
						HighlightHue));

				list.AppendEntry(
					new ListGumpEntry(
						"Save Messages",
						button =>
						{
                            CentralGump.Messages.Export();
							Refresh(true);
						},
						HighlightHue));

				list.AppendEntry(
					new ListGumpEntry(
						"Delete All",
						button =>
						{
							if (UseConfirmDialog)
							{
								Send(
									new ConfirmDialogGump(
										User,
										this,
										title: "Delete All Messages?",
										html:
											"All messages in the News database will be deleted, erasing all data associated with them.\nThis action can not be reversed.\n\nDo you want to continue?",
										onAccept: subButton =>
										{
											DeleteAllMessages();
											Refresh(true);
										}));
							}
							else
							{
								DeleteAllMessages();
								Refresh(true);
							}
						},
						HighlightHue));

				list.AppendEntry(
					new ListGumpEntry(
						"New Message",
						button =>
						{
							_NewMessageInput = new InputDialogGump(
								User,
								this,
								title: "Create New Message?",
								html: "This action will create a new message with the selected title.\nDo you want to continue?",
								callback: OnConfirmCreateMessage);
							Send(_NewMessageInput);
						},
						HighlightHue));
			}

			base.CompileMenuOptions(list);
		}

		protected virtual void OnConfirmCreateMessage(GumpButton button, string title)
		{
			TimeStamp date = TimeStamp.UtcNow;
			string author = User.RawName;

			const string content = "Content - BBCode supported:<br>[B]B, U, I, BIG, SMALL, URL, COLOR[/B]";

			string uid = CryptoGenerator.GenString(CryptoHashType.MD5, String.Format("{0}", date.Stamp)).Replace("-", "");
			Selected = new Message(uid, date, title, content, author);

			if (CentralGump.Messages.ContainsKey(uid))
			{
                CentralGump.Messages[uid] = Selected;
			}
			else
			{
                CentralGump.Messages.Add(uid, Selected);
			}

            CentralGump.Messages.Export();

			Refresh(true);

			if (UseConfirmDialog)
			{
				Send(
					new ConfirmDialogGump(
						User,
						this,
						title: "View Message?",
						html: "Your message has been created.\nDo you want to view it now?",
						onAccept: subButton => Send(new NewsMessageOverviewGump(User, this, Selected))));
			}
			else
			{
				Send(new NewsMessageOverviewGump(User, this, Selected));
			}
		}

		private void DeleteAllMessages()
		{
			foreach (Message message in List.Where(message => message != null))
			{
				message.Delete();
			}

            CentralGump.Messages.Clear();
            CentralGump.Messages.Export();
		}

		protected override void CompileList(List<Message> list)
		{
			list.Clear();

			list.AddRange(
                CentralGump.GetSortedMessages().Where(message => message != null && (User.AccessLevel >= CentralGump.Access)));

			base.CompileList(list);
		}

		protected override void SelectEntry(GumpButton button, Message entry)
		{
			base.SelectEntry(button, entry);

			if (button != null && entry != null)
			{
                Send(new NewsMessageOverviewGump(User, this, entry, UseConfirmDialog));
			}
		}
	}
}