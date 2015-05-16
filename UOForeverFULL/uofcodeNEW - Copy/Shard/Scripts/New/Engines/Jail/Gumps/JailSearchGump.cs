#region References
using System.Collections.Generic;

using Server.Accounting;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
#endregion

namespace Server.Engines.Jail
{
	/// <summary>
	///     This gump is used to display the search results for the jail search engine
	/// </summary>
	public class JailSearchGump : Gump
	{
		private const int LabelHue = 0x480;
		private const int GreenHue = 0x40;
		private const int RedHue = 0x20;

		private readonly List<PlayerMobile> m_Mobiles;
		private readonly List<Account> m_Accounts;
		private readonly Mobile m_User;
		private readonly int m_Page;
		private readonly JailGumpCallback m_Callback;

		public JailSearchGump(List<Account> accs, Mobile user, JailGumpCallback callback)
			: this(accs, user, 0, callback)
		{ }

		public JailSearchGump(List<Account> accs, Mobile user, int page, JailGumpCallback callback)
			: this(new List<PlayerMobile>(), accs, user, page, callback)
		{ }

		public JailSearchGump(List<PlayerMobile> mobs, Mobile user, JailGumpCallback callback)
			: this(mobs, user, 0, callback)
		{ }

		public JailSearchGump(List<PlayerMobile> mobs, Mobile user, int page, JailGumpCallback callback)
			: this(mobs, new List<Account>(), user, page, callback)
		{ }

		public JailSearchGump(List<PlayerMobile> mobs, List<Account> accs, Mobile user, JailGumpCallback callback)
			: this(mobs, accs, user, 0, callback)
		{ }

		public JailSearchGump(List<PlayerMobile> mobs, List<Account> accs, Mobile user, int page, JailGumpCallback callback)
			: base(100, 100)
		{
			user.CloseGump(typeof(JailSearchGump));
			m_Callback = callback;

			m_Accounts = accs;
			m_Mobiles = mobs;
			m_User = user;
			m_Page = page;

			MakeGump();
		}

		private void MakeGump()
		{
			int arraycnt = 0;
			bool usemobs = false;
			if (m_Accounts.Count > 0)
			{
				arraycnt = m_Accounts.Count;
			}
			else if (m_Mobiles.Count > 0)
			{
				arraycnt = m_Mobiles.Count;
				usemobs = true;
			}

			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;
			AddPage(0);
			AddBackground(0, 0, 485, 315, 9250);

			// Title
			AddAlphaRegion(20, 15, 445, 20);
			AddLabel(25, 15, LabelHue, string.Format("Jail System Search: Displaying {0} results.", arraycnt));

			// Table header
			AddImageTiled(20, 40, 120, 20, 9304);
			AddAlphaRegion(20, 40, 120, 20);
			AddLabel(25, 40, LabelHue, @"Account");

			AddImageTiled(145, 40, 120, 20, 9304);
			AddAlphaRegion(145, 40, 120, 20);
			AddLabel(150, 40, LabelHue, @"Player");

			AddImageTiled(270, 40, 90, 20, 9304);
			AddAlphaRegion(270, 40, 90, 20);
			AddLabel(275, 40, LabelHue, @"Status");

			AddImageTiled(365, 40, 35, 20, 9304);
			AddAlphaRegion(365, 40, 35, 20);
			AddLabel(370, 40, LabelHue, @"Jail");

			AddImageTiled(405, 40, 60, 20, 9304);
			AddAlphaRegion(405, 40, 60, 20);
			AddLabel(410, 40, LabelHue, "History");

			// Table background
			AddImageTiled(20, 65, 120, 210, 9274);
			AddAlphaRegion(20, 65, 120, 210);

			AddAlphaRegion(145, 65, 120, 210);

			AddImageTiled(270, 65, 90, 210, 9274);
			AddAlphaRegion(270, 65, 90, 210);

			AddAlphaRegion(365, 65, 35, 210);

			AddImageTiled(405, 65, 60, 210, 9274);
			AddAlphaRegion(405, 65, 60, 210);

			// Load data
			for (int i = m_Page * 10; i < m_Page * 10 + 10 && i < arraycnt; i++)
			{
				Account acc = null;
				Mobile m = null;

				if (usemobs)
				{
					m = m_Mobiles[i];
				}
				else
				{
					acc = m_Accounts[i];
				}

				if (acc != null)
				{
					m = JailSystem.GetOnlineMobile(acc);
				}
				else if (m != null)
				{
					acc = m.Account as Account;
				}

				int index = i - (m_Page * 10);

				// Account label and button
				if (acc != null)
				{
					AddLabelCropped(35, 70 + index * 20, 100, 20, LabelHue, acc.Username);

					if (m_User.AccessLevel >= AccessLevel.Administrator)
					{
						// View account details button : buttons from 10 to 19
						AddButton(15, 75 + index * 20, 2224, 2224, 10 + index, GumpButtonType.Reply, 0);
					}
				}

				// Player label and button
				if (m != null)
				{
					AddLabelCropped(160, 70 + index * 20, 100, 20, LabelHue, m.Name);

					// View props button: buttons from 20 to 29
					if (m_User.AccessLevel >= AccessLevel.Lead) //Only >= Seer get props button
					{
						AddButton(140, 75 + index * 20, 2224, 2224, 20 + index, GumpButtonType.Reply, 0);
					}
				}

				// Status
				if (m != null && m.NetState != null)
				{
					// A mobile is online
					AddLabel(285, 70 + index * 20, GreenHue, @"Online");

					// View Client button: buttons from 30 to 39
					if (m_User.AccessLevel >= AccessLevel.Lead)
					{
						AddButton(265, 75 + index * 20, 2224, 2224, 30 + index, GumpButtonType.Reply, 0);
					}
				}
				else
				{
					// Offline
					AddLabel(285, 70 + index * 20, RedHue, @"Offline");
				}

				// Jail button: buttons from 40 to 49
				if ((m != null && m.AccessLevel == AccessLevel.Player) || (acc != null && !acc.Banned))
				{
					AddButton(375, 73 + index * 20, 5601, 5605, 40 + index, GumpButtonType.Reply, 0);
				}

				// History button: buttons from 50 to 59
				if (acc != null && m_User.AccessLevel >= JailSystem.m_HistoryLevel)
				{
					AddButton(428, 73 + index * 20, 5601, 5605, 50 + index, GumpButtonType.Reply, 0);
				}
			}

			AddAlphaRegion(20, 280, 445, 20);

			if (m_Page * 10 + 10 < arraycnt)
			{
				AddButton(270, 280, 4005, 4006, 2, GumpButtonType.Reply, 0);
				AddLabel(305, 280, LabelHue, @"Next"); // Next: B2
			}

			if (m_Page > 0)
			{
				AddButton(235, 280, 4014, 4015, 1, GumpButtonType.Reply, 0);
				AddLabel(175, 280, LabelHue, @"Previous"); // Prev: B1
			}

			AddButton(20, 280, 4017, 4018, 0, GumpButtonType.Reply, 0);
			AddLabel(55, 280, LabelHue, @"Close"); // Close: B0
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			switch (info.ButtonID)
			{
				case 1: // Previous page
					{
						m_User.SendGump(new JailSearchGump(m_Mobiles, m_Accounts, m_User, m_Page - 1, m_Callback));
						break;
					}
				case 2: // Next page
					{
						m_User.SendGump(new JailSearchGump(m_Mobiles, m_Accounts, m_User, m_Page + 1, m_Callback));
						break;
					}
				case 0: // Close
					{
						if (m_Callback != null)
						{
							try
							{
								m_Callback.DynamicInvoke(new object[] {m_User});
							}
							catch
							{ }
						}
						break;
					}
				default:
					{
						int type = info.ButtonID / 10;
						int index = info.ButtonID % 10;

						bool usemobs = m_Mobiles.Count > 0;

						Mobile m = null;
						Account acc = null;

						if (usemobs)
						{
							m = m_Mobiles[m_Page * 10 + index];
						}
						else
						{
							acc = m_Accounts[m_Page * 10 + index];
						}

						if (acc != null)
						{
							m = JailSystem.GetOnlineMobile(acc);
						}
						else if (m != null)
						{
							acc = m.Account as Account;
						}

						switch (type)
						{
							case 1: // View Account details
								{
									m_User.SendGump(new JailSearchGump(m_Mobiles, m_Accounts, m_User, m_Page, m_Callback));

									if (acc != null)
									{
										m_User.SendGump(
											new AdminGump(
												m_User, AdminGumpPage.AccountDetails_Information, 0, null, "Returned from the jail system", acc));
									}

									break;
								}
							case 2: // View Mobile props
								{
									m_User.SendGump(new JailSearchGump(m_Mobiles, m_Accounts, m_User, m_Page, m_Callback));

									if (m_User.AccessLevel >= AccessLevel.Lead && m != null)
									{
										m_User.SendGump(new PropertiesGump(m_User, m));
									}
									break;
								}
							case 3: // View Client gump
								{
									m_User.SendGump(new JailSearchGump(m_Mobiles, m_Accounts, m_User, m_Page, m_Callback));

									if (m_User.AccessLevel >= AccessLevel.Lead && m != null && m.NetState != null)
									{
										m_User.SendGump(new ClientGump(m_User, m.NetState));
									}
									else
									{
										m_User.SendMessage("That mobile is no longer online or you do not have sufficient access.");
									}
									break;
								}
							case 4: // Jail button
								{
									if (acc != null)
									{
										JailSystem.InitJail(m_User, acc);
									}
									else if (m != null)
									{
										JailSystem.InitJail(m_User, m);
									}

									break;
								}
							case 5: // History button
								{
									if (acc != null)
									{
										List<JailEntry> history = JailSystem.SearchHistory(acc);

										if (history.Count > 0)
										{
											m_User.SendGump(new JailListingGump(m_User, history, JailSearchCallback));
										}
										else
										{
											m_User.SendMessage("No jail entries have been found for that account");
											m_User.SendGump(new JailSearchGump(m_Mobiles, m_Accounts, m_User, m_Page, m_Callback));
										}
									}
									break;
								}
						}
						break;
					}
			}
		}

		private void JailSearchCallback(Mobile user)
		{
			user.SendGump(new JailSearchGump(m_Mobiles, m_Accounts, user, m_Page, m_Callback));
		}
	}
}