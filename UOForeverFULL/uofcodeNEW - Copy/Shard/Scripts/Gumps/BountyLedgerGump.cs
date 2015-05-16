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
 
 /* Scripts/Gumps/BountyLedgerGump.cs
  *	ChangeLog:
  * 12/29/06, Kit
  *     Added additional sanity checking in OnTarget due to exception log report.
  * 9/27/05, Adam
  *		Add emergency Try/Catch to OnTarget
  * 1/2/05, Adam
  *		Call PackEntries() to pack-out null/deleted mobs from the ledger list
  *		Called in OnTarget(), BountyLedgerGump()
  * 9/11/04, Adam
  *		pack-out null mobs from the list
  *		Not sure, but null mobs may happen when a char is deleted?
  *	8/9/04, mith
  *		AddIndex() emergency TryCatch added after server crash, until we can solve the problem.
  * 7/21/04, Pixie
  *		Added chance to success in tracking from a bounty ledger instead of a flat
  *		skillcheck comparison between tracker and trackee.
  *		Also, limited how often they can use a book to track.
  *	6/2/04, Pixie
  *		Changed to use utility functions in BountyKeeper instead of iterating
  *		through the bounties arraylist manually.
  *	5/19/04 Created by smerX
  *
  */

/*using System;
using System.Collections;
using Server;
using Server.Items; 
using Server.Network;
using Server.BountySystem;
using Server.Targeting;
using Server.Mobiles;
using Server.Commands;

namespace Server.Gumps
{
	public class BountyLedgerGump : Gump
	{
		private static TimeSpan UseDelay = TimeSpan.FromSeconds( 10.0 );

		private BountyLedger m_Book;
		private int m_EmptyHue = 0;
		private int m_FilledHue = 90;
		private int m_BountyHue = 95;

		public BountyLedger Book{ get{ return m_Book; } }

		public string GetName( string name )
		{
			if ( name == null || (name = name.Trim()).Length <= 0 )
				return "(indescript)";

			return name;
		}

		private void AddBackground()
		{

			AddPage( 0 );

			// Background image
			AddImage( 100, 10, 2200 );

			// Two seperators
			for ( int i = 0; i < 2; ++i )
			{
				int xOffset = 125 + (i * 165);

				AddImage( xOffset, 50, 57 );
				xOffset += 20;

				for ( int j = 0; j < 6; ++j, xOffset += 15 )
					AddImage( xOffset, 50, 58 );

				AddImage( xOffset - 5, 50, 59 );
				
			}

			// First four page buttons
			for ( int i = 0, xOffset = 130, gumpID = 2225; i < 4; ++i, xOffset += 35, ++gumpID )
				AddButton( xOffset, 187, gumpID, gumpID, 0, GumpButtonType.Page, 2 + i );

			// Next four page buttons
			for ( int i = 0, xOffset = 300, gumpID = 2229; i < 4; ++i, xOffset += 35, ++gumpID )
				AddButton( xOffset, 187, gumpID, gumpID, 0, GumpButtonType.Page, 6 + i );

		}

		private void AddIndex()
		{
			// Index
			AddPage( 1 );

			// Rename button
			AddButton( 125, 15, 2472, 2473, 1, GumpButtonType.Reply, 0 );
			AddHtml( 158, 22, 100, 18, "Add entry", false, false ); // add entry

			// List of entries
			try
			{
				if ( m_Book != null )
				{
					ArrayList entries = m_Book.Entries;

					for ( int i = 0; i < 16; ++i )
					{
						string desc;
						int hue;

						if ( i < entries.Count )
						{
							desc = GetName( ((LedgerEntry)entries[i]).Mob.Name );
					
							if ( ((LedgerEntry)entries[i]).IsBounty )
							{
								hue = m_BountyHue;
							}
							else
							{
								hue = m_FilledHue;
							}
						}
						else
						{
							desc = "Empty";
							hue = m_EmptyHue;
						}

						// Track Selected button
						AddButton( 130 + ((i / 8) * 160), 65 + ((i % 8) * 15), 2103, 2104, 2 + (i * 6) + 0, GumpButtonType.Reply, 0 );

						// Description label
						AddLabelCropped( 145 + ((i / 8) * 160), 60 + ((i % 8) * 15), 115, 17, hue, desc );
					}
				}
			}
			catch( Exception ex )
			{
				Console.WriteLine(ex.ToString());
			}

			// Turn page button
			AddButton( 393, 14, 2206, 2206, 0, GumpButtonType.Page, 2 );
		}
		
		private void AddDetails( int index, int half )
		{
			string desc;
			int hue = 0;

			string name = "";
			string amount = "";
			string bounty = "";
			
			if ( index < m_Book.Entries.Count )
			{
				LedgerEntry e = (LedgerEntry)m_Book.Entries[index];

				name = GetName( ((Mobile)e.Mob).Name );

					if ( e.IsBounty )
					{
						desc = "Name: " + name;
						bounty = "Bounty: Yes";
						
						if ( e.Amount == 0 )
							amount = "Amount: ?";
						else
							amount = "Amount: " + e.Amount.ToString();
					}
					else
					{
						desc = "Name: " + name;
						bounty = "Bounty: No";
					}

				// Remove button
				AddButton( 135 + (half * 160), 135, 2437, 2438, 2 + (index * 6) + 1, GumpButtonType.Reply, 0 );
				AddHtml( 150 + (half * 160), 135, 100, 18, "Remove", false, false );

				if ( e != m_Book.Default )
				{
					// Set target
					//AddButton( 160 + (half * 140), 20, 2361, 2361, 2 + (index * 6) + 2, GumpButtonType.Reply, 0 );
					//AddHtml( 175 + (half * 140), 15, 100, 18, "Set target", false, false );

				}
			}
			else
			{
				desc = "Empty";
				hue = m_EmptyHue;
			}

			// Description label
			AddLabelCropped( 145 + (half * 160), 60, 115, 17, hue, desc );
			AddLabelCropped( 145 + (half * 160), 75, 115, 17, hue, bounty );
			AddLabelCropped( 145 + (half * 160), 90, 115, 17, hue, amount );
		}

		public BountyLedgerGump( Mobile from, BountyLedger book ) : base( 150, 200 )
		{
			m_Book = book;
			AddBackground();

			// Packout old entries;
			m_Book.PackEntries();

			ArrayList entries = m_Book.Entries;

			try
			{
				AddIndex();
			}
			catch (Exception e)
			{
				Console.WriteLine( "BountyLedgerGump: Error in AddIndex" );
				Console.WriteLine( e );
			}

			for ( int page = 0; page < 8; ++page )
			{
				AddPage( 2 + page );

				AddButton( 125, 14, 2205, 2205, 0, GumpButtonType.Page, 1 + page );

				if ( page < 7 )
					AddButton( 393, 14, 2206, 2206, 0, GumpButtonType.Page, 3 + page );

				for ( int half = 0; half < 2; ++half )
				{
					try
					{
						AddDetails( (page * 2) + half, half );
					}
					catch (Exception e)
					{
						Console.WriteLine( "BountyLedgerGump: Error in AddDetails" );
						Console.WriteLine( e );
					}
				}

			}
			
		}

		public override void OnResponse( NetState state, RelayInfo info )
		{
			Mobile from = state.Mobile;

			if ( m_Book.Deleted || !from.InRange( m_Book.GetWorldLocation(), 1 ) || !Multis.DesignContext.Check( from ) )
				return;

			int buttonID = info.ButtonID;

			if ( buttonID == 1 ) // Add to Ledger
			{
				if ( m_Book.Movable )
				{
					from.SendMessage( "What would you like to investigate?" );
					from.Target = new AddEntryTarget( m_Book );

				}
				else
				{
					from.SendLocalizedMessage( 502413 ); // That cannot be done while the book is locked down.
				}
			}
			else
			{
				buttonID -= 2;

				int index = buttonID / 6;
				int type = buttonID % 6;
				
				if ( index >= 0 && index < m_Book.Entries.Count )
				{
					LedgerEntry e = (LedgerEntry)m_Book.Entries[index];
					Mobile target = ((LedgerEntry)m_Book.Entries[index]).Mob;

					switch ( type )
					{
						case 0: // track (from index view)
						{
							if( (DateTime.UtcNow - m_Book.m_LastUse) < UseDelay )
							{
								from.SendMessage( "You must wait before using this again" );
							}
							else
							{
								if ( m_Book.Movable )
								{
									//set the last time the book was used!
									m_Book.m_LastUse = DateTime.UtcNow;

									from.CloseGump( typeof( BountyLedgerGump ) );
									//from.SendGump( new BountyLedgerGump( from, m_Book ) );

									double Forensics = from.Skills[SkillName.Forensics].Base;								
									double Tracking = from.Skills[SkillName.Tracking].Base;
									double Hiding = target.Skills[SkillName.Hiding].Base;
									double Stealth = target.Skills[SkillName.Stealth].Base;
									double fromskill = (Forensics + Tracking)/2;
									double targetskill = (Hiding + Stealth )/2;
								
									bool success = false;
								
								
									//if ( fromskill >= targetskill )
									//	success = true;
									if( targetskill <= 0 ) targetskill = 0.01;
									double chance = (fromskill + 20) / ( targetskill * 2 );
									double randomdouble = Utility.RandomDouble();

									if( from.CanSee(target) )
									{
										chance = 1;
									}

									if( target.AccessLevel > AccessLevel.Player )
									{
										chance = 0;
									}
								
									//debug message
									if( from.AccessLevel > AccessLevel.Player )
									{
										from.SendMessage( "Your chance to track {0} is {1:0.00} ({2:0.00})", target.Name, chance, randomdouble );
									}

									if( chance > randomdouble )
									{
										success = true;
									}
									
									int area = 3 * (int)fromskill;
								
									if ( success )
									{
										if( from.InRange( target, area ) )
										{
											from.SendMessage( "You are now hunting {0}!", target.Name );
											from.QuestArrow = new SkillHandlers.TrackArrow( from, target, area );
										}
										else
										{
											from.SendMessage( "You don't believe {0} is in this area.", target.Name );
										}
									}
									else
									{
										from.SendMessage( "You are not intuitive enough to find them." );
									}


								}
								else
								{
									from.SendLocalizedMessage( 502413 ); // That cannot be done while the book is locked down.
								}
							}

							break;
						}
						case 1: // remove
						{
							if ( m_Book.Movable )
							{
								m_Book.RemoveEntry( from, e, index );
							}
							else
							{
								from.SendMessage( "That item is inaccessible." );
							}

							break;
						}

/*		this was the page-by-page view "track" button, and it was redundant.-smerX

						case 2: // track
						{
							if ( m_Book.Movable )
							{
								m_Book.Default = e;

								from.CloseGump( typeof( BountyLedgerGump ) );
								from.SendGump( new BountyLedgerGump( from, m_Book ) );

								double Forensics = from.Skills[SkillName.Forensics].Base;								
								double Tracking = from.Skills[SkillName.Tracking].Base;
								double Hiding = target.Skills[SkillName.Hiding].Base;
								double Stealth = target.Skills[SkillName.Stealth].Base;
								double fromskill = (Forensics + Tracking)/2;
								double targetskill = (Hiding + Stealth)/2;
		
								bool succeded = false;
								
								if ( fromskill > targetskill + 10 )
									succeded = true;
								
								int area = 3 * (int)fromskill;
											
								if ( succeded )
								{
									from.SendMessage( "You are now hunting {0}!", target.Name );
									from.QuestArrow = new SkillHandlers.TrackArrow( from, target, area );
								}
								else
								{
									from.SendMessage( "You are not intuative enough to find them." );
								}


							}
							else
							{
								from.SendLocalizedMessage( 502413 ); // That cannot be done while the book is locked down.
							}

							break;
						}
*/					/*	default:
						{
							break;
						}
					}
				}
			}
		}
	}
	
	public class AddEntryTarget : Target
	{
		private BountyLedger m_Book;
		
		public AddEntryTarget( BountyLedger book) : base( 10, false, TargetFlags.None )
		{
			m_Book = book;
		}

		protected override void OnTarget( Mobile from, object target )
		{
			try 
			{
				OnTargetWrapper( from, target );
			}
			catch( Exception ex )
			{
				Console.WriteLine(ex.ToString());
				// from, target
			}
		}

		protected void OnTargetWrapper( Mobile from, object target )
		{
			double skillvalue = from.Skills[SkillName.Forensics].Value;
			bool wanted = false;
			bool bonusExists = false;
			int amount = 0;

            if(target == null) //kit 12/29/06 sanity checking
            {
                from.SendMessage("You have specified an invalid target.");
                return;
            }
                
			// Packout old entries;
			m_Book.PackEntries();

			if ( target != null && target is Mobile )
			{
				if ( target is PlayerMobile )
				{
					if ( from == (PlayerMobile)target )
					{
						from.SendMessage( "You may not add yourself to your ledger." );
						return;
					}
				
					foreach( LedgerEntry q in m_Book.Entries )
					{
                        if(q == null || q.Mob == null) //kit 12/19/06 sanity check
                            continue;

						if ( (PlayerMobile)q.Mob == (PlayerMobile)target )
						{
							from.SendMessage( "You may not have duplicate entries." );
							return;
						}
					}
				
					if ( skillvalue > 80 )
					{
						
						if( BountyKeeper.BountiesOnPlayer((PlayerMobile)target) > 0 )
						{
							wanted = true;
						}
						amount = BountyKeeper.RewardForPlayer((PlayerMobile)target);
						bonusExists = BountyKeeper.IsEligibleForLBBonus((PlayerMobile)target);

						if( bonusExists )
						{
							amount += BountyKeeper.CurrentLBBonusAmount;
						}
						
						int infoAmount = 1;
						
						if ( skillvalue > 80 )
							infoAmount = 1;
						if ( skillvalue > 95 )
							infoAmount = Utility.Random( 1, 2 );
						if ( skillvalue > 98 )
							infoAmount = Utility.Random( 2, 3 );
						if ( skillvalue == 100 )
							infoAmount = 3;
							
						string whoWants = bonusExists ? "Lord British" : "an Independant Party";
													
						switch ( infoAmount )
						{
							case 0: 
							{ 
								from.SendMessage( "You don't recall any bounties out on this person." ); 
								break;
							}
							case 1: 
							{ 
								if ( wanted )
								{
									from.SendMessage( "That person is wanted." ); 
									
									LedgerEntry e = new LedgerEntry( (Mobile)target, 0, true );
									m_Book.AddEntry( from, e, m_Book.Entries.Count + 1 );
								}

								else
								{
									from.SendMessage( "That person is not wanted." );
								}

								break;
							}
							case 2: 
							{ 
								if ( wanted )
								{
									from.SendMessage( "That person is wanted by {0}.", whoWants ); 

									LedgerEntry e = new LedgerEntry( (Mobile)target, 0, true );
									m_Book.AddEntry( from, e, m_Book.Entries.Count + 1 );
								}

								else
								{
									from.SendMessage( "That person is not wanted." );
								}

								break;
							}
							case 3:
							{ 
								if ( wanted )
								{
									from.SendMessage( "That person is wanted by {0} for {1} GP!", whoWants, amount.ToString() );
									
									LedgerEntry e = new LedgerEntry( (Mobile)target, amount, true );
									m_Book.AddEntry( from, e, m_Book.Entries.Count + 1 );
								}
								else
								{
									from.SendMessage( "That person is not wanted." );
								}
	
								break;
							}
							default: 
							{
								if ( wanted )
								{
									from.SendMessage( "You don't recall any bounties out on this person." ); 
								}
								else
								{
									from.SendMessage( "That person is not wanted." );
								}
								break;
							}

						}
					}
					else
					{
						from.SendMessage( "You fail to recall any bounties out on this person." );
					}
				}
				else
				{
					from.SendMessage( "You can not evaluate their status." );
				}
			}
			else if ( target != null && target is Corpse )
			{
// Looters ----
				if ( from.CheckTargetSkill( SkillName.Forensics, target, 80.0, 100.0 ) )
				{
					Corpse c = (Corpse)target;
					ArrayList looters = new ArrayList();

					if ( c.Looters.Count > 0 )
					{
						foreach ( Mobile mob in c.Looters )
						{
                            if(mob == null) //kit 12/29/06 sanity check
                                continue;

							if ( mob is PlayerMobile )
								looters.Add( mob );
						}
					}
					else
					{
						c.LabelTo( from, 501002 );//The corpse has not been desecrated.
					}
					
					if ( !(c.Killer is PlayerMobile) )
					{
						c.LabelTo( from, "They were killed by {0} (NPC)", c.Killer.Name );
					}
// Corpse Gump Management ----						
					if ( ((Body)c.Amount).IsHuman && c.Killer != null && c.Killer is PlayerMobile )
					{
						if( BountyKeeper.BountiesOnPlayer((PlayerMobile)c.Killer) > 0)
						{
						
							from.SendGump( new ForensicKillerGump( from, c.Killer, m_Book ) );
							return;
						}
						else
							from.SendMessage( "Their killer does not have any bounties on him." );
					}
				}
				else
				{
					from.SendLocalizedMessage( 501001 );//You cannot determain anything useful.
				}
			}
			else
			{
				from.SendMessage( "You have specified an invalid target. Try again." );
				from.Target = new AddEntryTarget( m_Book );
			}
		}
	}
} */