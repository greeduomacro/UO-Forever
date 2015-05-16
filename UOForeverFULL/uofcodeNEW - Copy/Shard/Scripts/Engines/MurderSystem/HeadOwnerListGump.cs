#region References
using System;
using System.Collections.Generic;
using System.Linq;

using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;
#endregion

namespace Server.Engines.MurderSystem
{
	public class HeadOwnerListGump : Gump
	{
		public static bool OldStyle = PropsConfig.OldStyle;

		public static readonly int GumpOffsetX = PropsConfig.GumpOffsetX;
		public static readonly int GumpOffsetY = PropsConfig.GumpOffsetY;

		public static readonly int TextHue = PropsConfig.TextHue;
		public static readonly int TextOffsetX = PropsConfig.TextOffsetX;

		public static readonly int OffsetGumpID = PropsConfig.OffsetGumpID;
		public static readonly int HeaderGumpID = PropsConfig.HeaderGumpID;
		public static readonly int EntryGumpID = PropsConfig.EntryGumpID;
		public static readonly int BackGumpID = PropsConfig.BackGumpID;
		public static readonly int SetGumpID = PropsConfig.SetGumpID;

		public static readonly int SetWidth = PropsConfig.SetWidth;
		public static readonly int SetOffsetX = PropsConfig.SetOffsetX, SetOffsetY = PropsConfig.SetOffsetY;
		public static readonly int SetButtonID1 = PropsConfig.SetButtonID1;
		public static readonly int SetButtonID2 = PropsConfig.SetButtonID2;

		public static readonly int PrevWidth = PropsConfig.PrevWidth;
		public static readonly int PrevOffsetX = PropsConfig.PrevOffsetX, PrevOffsetY = PropsConfig.PrevOffsetY;
		public static readonly int PrevButtonID1 = PropsConfig.PrevButtonID1;
		public static readonly int PrevButtonID2 = PropsConfig.PrevButtonID2;

		public static readonly int NextWidth = PropsConfig.NextWidth;
		public static readonly int NextOffsetX = PropsConfig.NextOffsetX, NextOffsetY = PropsConfig.NextOffsetY;
		public static readonly int NextButtonID1 = PropsConfig.NextButtonID1;
		public static readonly int NextButtonID2 = PropsConfig.NextButtonID2;

		public static readonly int OffsetSize = PropsConfig.OffsetSize;

		public static readonly int EntryHeight = PropsConfig.EntryHeight;
		public static readonly int BorderSize = PropsConfig.BorderSize;

		private static bool PrevLabel = false, NextLabel = false;

		private static readonly int PrevLabelOffsetX = PrevWidth + 1;
		private static readonly int PrevLabelOffsetY = 0;

		private static readonly int NextLabelOffsetX = -29;
		private static readonly int NextLabelOffsetY = 0;

		private static readonly int EntryWidth = 180;
		private static readonly int EntryCount = 15;

		private static readonly int TotalWidth = OffsetSize + EntryWidth + OffsetSize + SetWidth + OffsetSize;
		private static readonly int TotalHeight = OffsetSize + ((EntryHeight + OffsetSize) * (EntryCount + 1));

		private static readonly int BackWidth = BorderSize + TotalWidth + BorderSize;
		private static readonly int BackHeight = BorderSize + TotalHeight + BorderSize;

		private Mobile m_Owner;
		private readonly List<Head2> m_Heads;
		private int m_Page;

		private class InternalComparer : IComparer<Mobile>
		{
			public static readonly IComparer<Mobile> Instance = new InternalComparer();

			public int Compare(Mobile x, Mobile y)
			{
				if (x == null || y == null)
				{
					throw new ArgumentException();
				}

				if (x.AccessLevel > y.AccessLevel)
				{
					return -1;
				}

				if (x.AccessLevel < y.AccessLevel)
				{
					return 1;
				}

				return Insensitive.Compare(x.Name, y.Name);
			}
		}

		public HeadOwnerListGump(Mobile owner)
			: this(owner, BuildList(owner), 0)
		{ }

		public HeadOwnerListGump(Mobile owner, List<Head2> list, int page)
			: base(GumpOffsetX, GumpOffsetY)
		{
			owner.CloseGump(typeof(HeadOwnerListGump));

			m_Owner = owner;
			m_Heads = list;

			Initialize(page);
		}

		public static List<Head2> BuildList(Mobile owner)
		{
			return
				Head2.AllHeads.Where(head => head != null)
					 .Where(head => !head.Expired && head.TypeOf == HeadType.Regular && head.Player == owner && head.Owner != null)
					 .ToList();
		}

		public void Initialize(int page)
		{
			m_Page = page;

			int count = m_Heads.Count - (page * EntryCount);

			if (count < 0)
			{
				count = 0;
			}
			else if (count > EntryCount)
			{
				count = EntryCount;
			}

			int totalHeight = OffsetSize + ((EntryHeight + OffsetSize) * (count + 1));

			AddPage(0);

			AddBackground(0, 0, BackWidth, BorderSize + totalHeight + BorderSize, BackGumpID);
			AddImageTiled(BorderSize, BorderSize, TotalWidth - (OldStyle ? SetWidth + OffsetSize : 0), totalHeight, OffsetGumpID);

			int x = BorderSize + OffsetSize;
			int y = BorderSize + OffsetSize;

			int emptyWidth = TotalWidth - PrevWidth - NextWidth - (OffsetSize * 4) - (OldStyle ? SetWidth + OffsetSize : 0);

			if (!OldStyle)
			{
				AddImageTiled(
					x - (OldStyle ? OffsetSize : 0), y, emptyWidth + (OldStyle ? OffsetSize * 2 : 0), EntryHeight, EntryGumpID);
			}

			AddLabel(
				x + TextOffsetX,
				y,
				TextHue,
				String.Format("Page {0} of {1} ({2})", page + 1, (m_Heads.Count + EntryCount - 1) / EntryCount, m_Heads.Count));

			x += emptyWidth + OffsetSize;

			if (OldStyle)
			{
				AddImageTiled(x, y, TotalWidth - (OffsetSize * 3) - SetWidth, EntryHeight, HeaderGumpID);
			}
			else
			{
				AddImageTiled(x, y, PrevWidth, EntryHeight, HeaderGumpID);
			}

			if (page > 0)
			{
				AddButton(x + PrevOffsetX, y + PrevOffsetY, PrevButtonID1, PrevButtonID2, 1, GumpButtonType.Reply, 0);

				if (PrevLabel)
				{
					AddLabel(x + PrevLabelOffsetX, y + PrevLabelOffsetY, TextHue, "Previous");
				}
			}

			x += PrevWidth + OffsetSize;

			if (!OldStyle)
			{
				AddImageTiled(x, y, NextWidth, EntryHeight, HeaderGumpID);
			}

			if ((page + 1) * EntryCount < m_Heads.Count)
			{
				AddButton(x + NextOffsetX, y + NextOffsetY, NextButtonID1, NextButtonID2, 2, GumpButtonType.Reply, 1);

				if (NextLabel)
				{
					AddLabel(x + NextLabelOffsetX, y + NextLabelOffsetY, TextHue, "Next");
				}
			}

			for (int i = 0, index = page * EntryCount; i < EntryCount && index < m_Heads.Count; ++i, ++index)
			{
				x = BorderSize + OffsetSize;
				y += EntryHeight + OffsetSize;

				Head2 head = m_Heads[index];

				AddImageTiled(x, y, EntryWidth, EntryHeight, EntryGumpID);
				AddLabelCropped(
					x + TextOffsetX,
					y,
					EntryWidth - TextOffsetX,
					EntryHeight,
					GetHueFor(head.Owner),
					(head.Deleted || head.Owner == null || head.Owner.Deleted) ? "(deleted)" : head.Owner.Name);

				x += EntryWidth + OffsetSize;

				if (SetGumpID != 0)
				{
					AddImageTiled(x, y, SetWidth, EntryHeight, SetGumpID);
				}

				if (head.Owner != null && head.Owner.NetState != null && !head.Owner.Deleted)
				{
					AddButton(x + SetOffsetX, y + SetOffsetY, SetButtonID1, SetButtonID2, i + 3, GumpButtonType.Reply, 0);
				}
			}
		}

		private static int GetHueFor(Mobile m)
		{
			if (m == null)
			{
				return 0x58;
			}

			switch (m.AccessLevel)
			{
				case AccessLevel.Owner:
				case AccessLevel.Developer:
				case AccessLevel.Administrator:
					return 0x516;
				case AccessLevel.Seer:
					return 0x144;
				case AccessLevel.GameMaster:
					return 0x21;
				case AccessLevel.Counselor:
					return 0x2;
					//case AccessLevel.Player:
				default:
					{
						if (m.Kills >= 5)
						{
							return 0x21;
						}

						if (m.Criminal)
						{
							return 0x3B1;
						}

						return 0x58;
					}
			}
		}

		public override void OnResponse(NetState state, RelayInfo info)
		{
			Mobile from = state.Mobile;

			switch (info.ButtonID)
			{
				case 0: // Closed
					return;
				case 1: // Previous
					{
						if (m_Page > 0)
						{
							from.SendGump(new HeadOwnerListGump(from, m_Heads, m_Page - 1));
						}
					}
					break;
				case 2: // Next
					{
						if ((m_Page + 1) * EntryCount < m_Heads.Count)
						{
							from.SendGump(new HeadOwnerListGump(from, m_Heads, m_Page + 1));
						}
					}
					break;
				default:
					{
						int index = (m_Page * EntryCount) + (info.ButtonID - 3);

						if (index >= 0 && index < m_Heads.Count)
						{
							Head2 head = m_Heads[index];

							if (head.Deleted)
							{
								from.SendMessage("That head no longer exists!");
								from.SendGump(new HeadOwnerListGump(from, m_Heads, m_Page));
							}
							else if (head.Owner == null)
							{
								from.SendMessage("Could not find the player who last had possession of your head!");
								from.SendGump(new HeadOwnerListGump(from, m_Heads, m_Page));
							}
							else if (head.Owner.NetState == null)
							{
								from.SendMessage("That player is no longer online.");
								from.SendGump(new HeadOwnerListGump(from, m_Heads, m_Page));
							}
							else if ((DateTime.UtcNow - head.LastOffer).TotalMinutes < 5.0)
							{
								from.SendMessage("You cannot send another offer just yet.");
								from.SendGump(new HeadOwnerListGump(from, m_Heads, m_Page));
							}
							else if (from is PlayerMobile)
							{
								from.CloseGump(typeof(HeadNegotiateGump));
								head.Owner.CloseGump(typeof(HeadNegotiateGump));

								new VictimBuyHeadGump((PlayerMobile)from, head.Owner, head).Send();
							}
						}
					}
					break;
			}
		}
	}
}