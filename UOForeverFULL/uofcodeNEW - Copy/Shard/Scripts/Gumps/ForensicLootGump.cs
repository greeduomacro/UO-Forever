/***************************************************************************
 *                               CREDITS
 *                         -------------------
 *                         : (C) 2004-2009 Luke Tomasello (AKA Adam Ant)
 *                         :   and the Angel Island Software Team
 *                         :   luke@tomasello.com
 *                         :   Official Documentation:
 *                         :   www.game-master.net, wiki.game-master.net
 *                         :   Official Source Code (SVN Repository):
 *                         :   http://game-master.net:8050/svn/angelisland
 *                         : 
 *                         : (C) May 1, 2002 The RunUO Software Team
 *                         :   info@runuo.com
 *
 *   Give credit where credit is due!
 *   Even though this is 'free software', you are encouraged to give
 *    credit to the individuals that spent many hundreds of hours
 *    developing this software.
 *   Many of the ideas you will find in this Angel Island version of 
 *   Ultima Online are unique and one-of-a-kind in the gaming industry! 
 *
 ***************************************************************************/

/***************************************************************************
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 2 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
/* /Scripts/Gumps/ForensicGump.cs
 *
 *	ChangeLog:
 *  7/24/06, Rhiannon
 *		Changed to use Mobile.GetHueForNameInList() instead of GetHueFor().
 *	5/12/04 created by smerX
 */



/*using System;
using System.Collections;
using System.Collections.Generic;
using Server.Network;
using Server.Items;

namespace Server.Gumps
{
	public class ForensicLootGump : Gump
	{
		private BountyLedger m_Book;
		public static bool OldStyle =		PropsConfig.OldStyle;

		public static int GumpOffsetX = 	PropsConfig.GumpOffsetX;
		public static int GumpOffsetY = 	PropsConfig.GumpOffsetY;

		public static int TextHue = 		PropsConfig.TextHue;
		public static int TextOffsetX = 	PropsConfig.TextOffsetX;

		public static int OffsetGumpID =	PropsConfig.OffsetGumpID;
		public static int HeaderGumpID = 	PropsConfig.HeaderGumpID;
		public static int  EntryGumpID = 	PropsConfig.EntryGumpID;
		public static int   BackGumpID = 	PropsConfig.BackGumpID;
		public static int    SetGumpID =	PropsConfig.SetGumpID;

		public static int SetWidth =		PropsConfig.SetWidth;
		public static int SetOffsetX = 	PropsConfig.SetOffsetX, SetOffsetY = PropsConfig.SetOffsetY;
		public static int SetButtonID1 =	PropsConfig.SetButtonID1;
		public static int SetButtonID2 =	PropsConfig.SetButtonID2;

		public static int PrevWidth =		PropsConfig.PrevWidth;
		public static int PrevOffsetX =	PropsConfig.PrevOffsetX, PrevOffsetY = PropsConfig.PrevOffsetY;
		public static int PrevButtonID1 =	PropsConfig.PrevButtonID1;
		public static int PrevButtonID2 =	PropsConfig.PrevButtonID2;

		public static int NextWidth =		PropsConfig.NextWidth;
		public static int NextOffsetX =	PropsConfig.NextOffsetX, NextOffsetY = PropsConfig.NextOffsetY;
		public static int NextButtonID1 =	PropsConfig.NextButtonID1;
		public static int NextButtonID2 =	PropsConfig.NextButtonID2;

		public static int OffsetSize =		PropsConfig.OffsetSize;

		public static int EntryHeight =	PropsConfig.EntryHeight;
		public static int BorderSize =		PropsConfig.BorderSize;

		private static bool PrevLabel =	false, NextLabel = false;

		private static int PrevLabelOffsetX = PrevWidth + 1;
		private static int PrevLabelOffsetY = 0;

		private static int NextLabelOffsetX = -29;
		private static int NextLabelOffsetY = 0;

		private static int EntryWidth = 180;
		private static int EntryCount = 15;

		private static int TotalWidth =	OffsetSize + EntryWidth + OffsetSize + SetWidth + OffsetSize;
		private static int TotalHeight =	OffsetSize + ((EntryHeight + OffsetSize) * (EntryCount + 1));

		private static int BackWidth =		BorderSize + TotalWidth + BorderSize;
		private static int BackHeight =	BorderSize + TotalHeight + BorderSize;

		private Mobile m_Owner;
		private ArrayList m_Mobiles;
		private int m_Page;

		private class InternalComparer : IComparer
		{
			public static readonly IComparer Instance = new InternalComparer();

			public InternalComparer()
			{
			}

			public int Compare( object x, object y )
			{
				if ( x == null && y == null )
					return 0;
				else if ( x == null )
					return -1;
				else if ( y == null )
					return 1;

				Mobile a = x as Mobile;
				Mobile b = y as Mobile;

				if ( a == null || b == null )
					throw new ArgumentException();

				if ( a.AccessLevel > b.AccessLevel )
					return -1;
				else if ( a.AccessLevel < b.AccessLevel )
					return 1;
				else
					return Insensitive.Compare( a.Name, b.Name );
			}
		}

		public ForensicLootGump( Mobile owner, ArrayList looters, int page, BountyLedger book ) : base( GumpOffsetX, GumpOffsetY )
		{
			owner.CloseGump( typeof( ForensicLootGump ) );

			m_Owner = owner;
			m_Mobiles = looters;
			m_Book = book;

			Initialize( page );
		}

		public static ArrayList BuildFList( Mobile owner )
		{
			ArrayList list = new ArrayList();
			//ArrayList states = NetState.Instances;
            List<NetState> states = NetState.Instances;

			for ( int i = 0; i < states.Count; ++i )
			{
				Mobile m = states[i].Mobile;

				if ( m != null && (m == owner || !m.Hidden || owner.AccessLevel > m.AccessLevel) )
					list.Add( m );
			}

			list.Sort( InternalComparer.Instance );

			return list;
		}

		public void Initialize( int page )
		{
			m_Page = page;

			int count = m_Mobiles.Count - (page * EntryCount);

			if ( count < 0 )
				count = 0;
			else if ( count > EntryCount )
				count = EntryCount;

			int totalHeight = OffsetSize + ((EntryHeight + OffsetSize) * (count + 1));

			AddPage( 0 );

			AddBackground( 0, 0, BackWidth, BorderSize + totalHeight + BorderSize, BackGumpID );
			AddImageTiled( BorderSize, BorderSize, TotalWidth - (OldStyle ? SetWidth + OffsetSize : 0), totalHeight, OffsetGumpID );

			int x = BorderSize + OffsetSize;
			int y = BorderSize + OffsetSize;

			int emptyWidth = TotalWidth - PrevWidth - NextWidth - (OffsetSize * 4) - (OldStyle ? SetWidth + OffsetSize : 0);

			if ( !OldStyle )
				AddImageTiled( x - (OldStyle ? OffsetSize : 0), y, emptyWidth + (OldStyle ? OffsetSize * 2 : 0), EntryHeight, EntryGumpID );

			AddLabel( x + TextOffsetX, y, TextHue, String.Format( "Page {0} of {1} ({2})", page+1, (m_Mobiles.Count + EntryCount - 1) / EntryCount, m_Mobiles.Count ) );

			x += emptyWidth + OffsetSize;

			if ( OldStyle )
				AddImageTiled( x, y, TotalWidth - (OffsetSize * 3) - SetWidth, EntryHeight, HeaderGumpID );
			else
				AddImageTiled( x, y, PrevWidth, EntryHeight, HeaderGumpID );

			if ( page > 0 )
			{
				AddButton( x + PrevOffsetX, y + PrevOffsetY, PrevButtonID1, PrevButtonID2, 1, GumpButtonType.Reply, 0 );

				if ( PrevLabel )
					AddLabel( x + PrevLabelOffsetX, y + PrevLabelOffsetY, TextHue, "Previous" );
			}

			x += PrevWidth + OffsetSize;

			if ( !OldStyle )
				AddImageTiled( x, y, NextWidth, EntryHeight, HeaderGumpID );

			if ( (page + 1) * EntryCount < m_Mobiles.Count )
			{
				AddButton( x + NextOffsetX, y + NextOffsetY, NextButtonID1, NextButtonID2, 2, GumpButtonType.Reply, 1 );

				if ( NextLabel )
					AddLabel( x + NextLabelOffsetX, y + NextLabelOffsetY, TextHue, "Next" );
			}

			for ( int i = 0, index = page * EntryCount; i < EntryCount && index < m_Mobiles.Count; ++i, ++index )
			{
				x = BorderSize + OffsetSize;
				y += EntryHeight + OffsetSize;

				Mobile m = (Mobile)m_Mobiles[index];

				AddImageTiled( x, y, EntryWidth, EntryHeight, EntryGumpID );
				AddLabelCropped( x + TextOffsetX, y, EntryWidth - TextOffsetX, EntryHeight, GetHueFor(m), m.Deleted ? "(deleted)" : m.Name );

				x += EntryWidth + OffsetSize;

				if ( SetGumpID != 0 )
					AddImageTiled( x, y, SetWidth, EntryHeight, SetGumpID );

				if ( m.NetState != null && !m.Deleted )
					AddButton( x + SetOffsetX, y + SetOffsetY, SetButtonID1, SetButtonID2, i + 3, GumpButtonType.Reply, 0 );
			}
		}

		private static int GetHueFor( Mobile m )
		{
			switch ( m.AccessLevel )
			{
				case AccessLevel.Administrator: return 0x516;
				case AccessLevel.Seer: return 0x144;
				case AccessLevel.GameMaster: return 0x21;
				case AccessLevel.Counselor: return 0x2;
				case AccessLevel.Player: default:
				{
					if ( m.Kills >= 5 )
						return 0x21;
					else if ( m.Criminal )
						return 0x3B1;

					return 0x58;
				}
			}
		}

		public override void OnResponse( NetState state, RelayInfo info )
		{
			Mobile from = state.Mobile;

			switch ( info.ButtonID )
			{
				case 0: // Closed
				{
					return;
				}
				case 1: // Previous
				{
					if ( m_Page > 0 )
						from.SendGump( new ForensicLootGump( from, m_Mobiles, m_Page - 1, m_Book ) );

					break;
				}
				case 2: // Next
				{
					if ( (m_Page + 1) * EntryCount < m_Mobiles.Count )
						from.SendGump( new ForensicLootGump( from, m_Mobiles, m_Page + 1, m_Book ) );

					break;
				}
				default:
				{
					int index = (m_Page * EntryCount) + (info.ButtonID - 3);

					if ( index >= 0 && index < m_Mobiles.Count )
					{
						Mobile m = (Mobile)m_Mobiles[index];

						if ( m.Deleted )
						{
							from.SendMessage( "That player has deleted their character." );
							from.SendGump( new ForensicLootGump( from, m_Mobiles, m_Page, m_Book ) );
						}
						else
						{
							LedgerEntry e = new LedgerEntry( m, 0, false );
							m_Book.AddEntry( from, e, m_Book.Entries.Count + 1 );
						}
					}

					break;
				}
			}
		}

	}

}*/