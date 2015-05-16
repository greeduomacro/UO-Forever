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

/* Scripts/Items/Skill Items/Forensic/BountyLedger.cs
 *	ChangeLog:
 *	03/14/07, weaver
 *		Added additional safety in PackEntries() function.
 *  02/15/05, Pixie
 *		CHANGED FOR RUNUO 1.0.0 MERGE.
 *	2/1/05, Adam
 *		Add new routine to pack-out null/deleted mobs from the ledger list
 *		Added PackEntries()
 *	7/21/04, Pixie
 *		Added m_LastUse to limit how often they can try to track someone
 *	5/19/04 Created by smerX
 *
 */

/*using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Multis;
using Server.Mobiles;
using Server.Commands;
using Server.BountySystem;

namespace Server.Items
{
	public class BountyLedger : Item
	{
		private ArrayList m_Entries;
		//private string m_Description;
		private int m_DefaultIndex;
		public DateTime m_LastUse;

		[Constructable]
		public BountyLedger() : base( 0xEFA )
		{
			this.Weight = 3.0;
			this.LootType = LootType.Blessed;
			this.Hue = 0x8ab;
			this.Name = "a bounty ledger";
			this.Layer = Layer.OneHanded;

			m_Entries = new ArrayList();

			m_DefaultIndex = -1;
		}

		public ArrayList Entries{ get{ return m_Entries; } }

		public LedgerEntry Default
		{
			get
			{
				if ( m_DefaultIndex >= 0 && m_DefaultIndex < m_Entries.Count )
					return (LedgerEntry)m_Entries[m_DefaultIndex];

				return null;
			}
			set
			{
				if ( value == null )
					m_DefaultIndex = -1;
				else
					m_DefaultIndex = m_Entries.IndexOf( value );
			}
		}

		public BountyLedger( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			writer.Write( m_Entries.Count );

			for ( int i = 0; i < m_Entries.Count; ++i )
				((LedgerEntry)m_Entries[i]).Serialize( writer );

			writer.Write( m_DefaultIndex );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			LootType = LootType.Blessed;

			if ( Weight == 0.0 )
				Weight = 3.0;

			int version = reader.ReadInt();

			switch ( version )
			{
				case 0:
				{
					int count = reader.ReadInt();

					m_Entries = new ArrayList( count );

					for ( int i = 0; i < count; ++i )
						m_Entries.Add( new LedgerEntry( reader ) );

					m_DefaultIndex = reader.ReadInt();

					break;
				}
			}
		}

		public void AddEntry( Mobile from, LedgerEntry e, int index )
		{

			if ( IsOpen( from ) )
			{
				from.SendMessage( "You can not add Bounties while the Ledger is open." );
			}
			else if ( m_Entries.Count < 16 )
			{

				if ( e != null )
				{
					foreach ( LedgerEntry q in m_Entries )
					{
						if ( e.Mob == q.Mob )
						{
							from.SendMessage( "You may not have duplicate entries." );
							return;
						}
					}
					
					if ( from == e.Mob )
					{
						from.SendMessage( "You may not add yourself to your ledger." );
						return;
					}
					
					
					m_Entries.Add( new LedgerEntry( e.Mob, e.Amount, e.IsBounty ) );

					string desc = e.Mob.Name;

					if ( desc == null || (desc = desc.Trim()).Length == 0 )
						desc = "(indescript)";

					from.SendMessage( "You add {0} to your Ledger.", desc );
				}
			}
			else
			{
				from.SendMessage( "The ledger is full." );
			}
		}


		public void RemoveEntry( Mobile from, LedgerEntry e, int index )
		{
			if ( m_DefaultIndex == index )
				m_DefaultIndex = -1;

			m_Entries.RemoveAt( index );

			from.SendMessage( "You erase the listing." ); // You have removed the rune.
		}

		public bool IsOpen( Mobile toCheck )
		{
			NetState ns = toCheck.NetState;

			if ( ns == null )
				return false;

			//GumpCollection gumps = ns.Gumps;
            List<Gump> gumps = new List<Gump>(ns.Gumps);

            for ( int i = 0; i < gumps.Count; ++i )
			{
				if ( gumps[i] is BountyLedgerGump )
				{
					BountyLedgerGump gump = (BountyLedgerGump)gumps[i];

					if ( gump.Book == this )
						return true;
				}
			}

			return false;
		}

		public override bool DisplayLootType{ get{ return false; } }

		public override void OnDoubleClick( Mobile from )
		{
			if ( from.InRange( GetWorldLocation(), 1 ) )
			{
				from.CloseGump( typeof( BountyLedgerGump ) );
				from.SendGump( new BountyLedgerGump( from, this ) );
			}
		}

		// Adam: pack-out null mobs from the list
		//	Not sure, but null mobs may happen when a char is deleted?
		public void PackEntries()
		{
			Console.WriteLine( "BountyLedger: Packing..." );

			while(m_Entries.Count > 0)
			{
				bool found=false;
				for (int ix=0; ix < m_Entries.Count; ix++)
				{
					// wea: 14/Mar/2007 Added additional safety
					LedgerEntry e = null;

					if( m_Entries[ix] is LedgerEntry )
						e = (LedgerEntry)m_Entries[ix];
										
					if (e == null)
					{
						m_Entries.RemoveAt(ix);
						found=true;
						Console.WriteLine( "BountyLedger: Removing stale entry" );
						break;
					}
					Mobile mob = (Mobile)e.Mob;
					if (mob == null || mob.Deleted == true)
					{
						m_Entries.RemoveAt(ix);
						found=true;
						Console.WriteLine( "BountyLedger: Removing stale mob" );
						break;
					}
					if( BountyKeeper.BountiesOnPlayer((PlayerMobile)mob) == 0)
					{
						m_Entries.RemoveAt(ix);
						found=true;
						Console.WriteLine( "BountyLedger: Removing player with no bounty" );
						break;
					}
				}
				if (found == false)
					break;
			}
		}
	}

	public class LedgerEntry
	{
		private Mobile m_Mob;
		private int m_Amount;
		private bool m_IsBounty;

		public Mobile Mob
		{
			get{ return m_Mob; }
		}

		public int Amount
		{
			get{ return m_Amount; }
		}

		public bool IsBounty
		{
			get{ return m_IsBounty; }
		}

		public LedgerEntry( Mobile m, int a, bool i )
		{
			m_Mob = m;
			m_Amount = a;
			m_IsBounty = i;
		}

		public LedgerEntry( GenericReader reader )
		{
			int version = reader.ReadByte();

			switch ( version )
			{
				case 0:
				{
					m_Mob = reader.ReadMobile();
					m_Amount = reader.ReadInt();
					m_IsBounty = reader.ReadBool();

					break;
				}
			}
		}

		public void Serialize( GenericWriter writer )
		{
			writer.Write( (byte) 0 ); // version

			writer.Write( m_Mob );
			writer.Write( m_Amount );
			writer.Write( m_IsBounty );
		}
	}
}*/