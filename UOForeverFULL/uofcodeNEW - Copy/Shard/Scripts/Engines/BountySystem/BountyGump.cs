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

/* Engines/BountySystem/BountyGump.cs
 * ChangeLog:
 *	1/27/05, Pix
 *		Added optional Placer name to override Placer.Name on the bounty board.
 *	6/2/04, pixie
 *		Changed because we can't use BountyKeeper.Bounties anymore (for 
 *		arraylist safety reasons).
 *	5/23/04, pixie
 *		Removed the "Remove" button (since it doesn't work anyways)
 *  5/18/04, Pixie
 *		Changs to LBBonus and to display only
 *		1 bounty per wanted player
 *	5/16/04, Pixie
 *		Tweaked for BountySystem
 *	5/5/04 Creation by smerX
 */

/*using System;
using System.Collections;
using Server;
using Server.Misc;
using Server.Gumps;
using Server.Network;
using Server.Mobiles;
using Server.Items;

namespace Server.BountySystem
{
	public class BountyGump : Gump
	{
		private Mobile m_Killer;
		private Mobile m_Victim;
		private int m_Amount;
		//private string m_BountyString;

/* The below was just for testing the layout of the bounty gump
		public static void Initialize()
		{
			Commands.Register( "TestBounty", AccessLevel.Administrator, new CommandEventHandler( BountyGumpTest ) );
		}

		[Usage( "TestBounty" )]
		[Description( "Opens a place bounty gump." )]
		public static void BountyGumpTest( CommandEventArgs e )
		{
			e.Mobile.SendGump( new BountyGump( e.Mobile, e.Mobile ) );
		}
*/
	
		/*public BountyGump( Mobile victim, Mobile killer ) : base( 0, 0 )
		{
			m_Killer = killer;
			m_Victim = victim;

			BuildGump();
		}

		private void BuildGump()
		{
			Closable = false;
			Resizable = false;
			
			AddPage( 0 );      			
			
			AddBackground(30,30,380,155, 5054);

			string outputstring = "What price would you like to offer " + m_Killer.Name + "'s head back to him for?";
			AddHtml( 55, 50, 330, 80, outputstring, false, false );
			
			AddHtml( 135, 100, 180, 180, "Amount:", false, false );
			AddImageTiled( 184, 99, 106, 21, PropsConfig.HeaderGumpID );
			AddTextEntry( 185, 100, 105, 20, 0xFA5, 0, "0" );
						
			AddButton( 40, 125, 0xFA5, 0xFA7, 1, GumpButtonType.Reply, 0 );
			AddHtml( 75, 130, 90, 80, "Offer", false, false ); // Yes


			AddButton( 340, 125, 0xFA5, 0xFA7, 2, GumpButtonType.Reply, 0 );
			AddHtml( 375, 130, 90, 80, "Exit", false, false ); // No
		}

		public override void OnResponse( NetState state, RelayInfo info )
		{
			Mobile from = state.Mobile;
			PlayerMobile Killer = m_Killer as PlayerMobile;
			
			switch ( info.ButtonID )
			{
				case 1: 
				{
					try
					{
						TextRelay getTehText = info.GetTextEntry( 0 );
						m_Amount = Convert.ToInt32( getTehText.Text );
					}
					catch
					{
						m_Victim.SendMessage( "Bounty Amount was improperly formatted!" );
						m_Victim.SendGump( new BountyGump( m_Victim, m_Killer ) );
						break;
					}


					Container cont = m_Victim.BankBox;

					if ( m_Amount < 500 )
					{
						m_Victim.SendMessage( "No one would hunt for that low of a bounty!" );
						m_Victim.SendGump( new BountyGump( m_Victim, m_Killer ) );
						break;
					}
					if ( m_Amount > 10000 )
					{
						m_Victim.SendMessage( "There is a maximum bounty of 10k per murder. " );
						m_Victim.SendGump( new BountyGump( m_Victim, m_Killer ) );
						break;
					}
					else if ( cont != null && cont.ConsumeTotal( typeof( Gold ), m_Amount ) )
					{
						BountyKeeper.Add( new Bounty((PlayerMobile)m_Victim, (PlayerMobile)m_Killer, m_Amount, true) );
						m_Victim.SendMessage( "You have posted a bounty for the head of " + m_Killer.Name + "!" );
						m_Victim.SendMessage( "Amount of GP removed from your bank: " + m_Amount );
						m_Killer.SendMessage( "A bounty has been placed on your head in the amount of: " + m_Amount + " gold.");

					}
					else
					{
						m_Victim.SendMessage( "You do not have that much GP in your bank!" );
						m_Victim.SendGump( new BountyGump( m_Victim, m_Killer ) );
						break;
					}
					break;
				}
				case 2: 
				{
					m_Victim.SendMessage( "You decide not to post a bounty." );
					break;
				}
			}

		}
	} //end of BountyGump


	public class BountyDisplayGump : Gump
	{
		private int m_Page;
		private Mobile m_From;

		private const int LabelColor = 0x7FFF;
		private const int LabelHue = 1153;

		public override void OnResponse( NetState sender, RelayInfo info )
		{
			int page = m_Page;
			Mobile from = m_From;
			switch ( info.ButtonID )
			{
				case 4: // Scroll up
				{
					if ( page == 0 )
						page = BountyKeeper.NumberOfUniqueBounties-1;
					else
						page -= 1;

					from.SendGump( new BountyDisplayGump( from, page ) );

					break;
				}
				case 5: // Scroll down
				{
					page += 1;
					if( page > BountyKeeper.NumberOfUniqueBounties -1 )
					{
						page = 0;
					}

					from.SendGump( new BountyDisplayGump( from, page ) );

					break;
				}

			}
		}	
			
			
		public BountyDisplayGump(Mobile from, int page) : base(50,10)
		{
			from.CloseGump( typeof( BountyDisplayGump ) );
			m_Page = page;
			m_From = from;

			AddPage( 0 );
			AddImage( 30, 30, 5400 );
			if( BountyKeeper.NumberOfUniqueBounties != 0 ) //only put scroll-up and scroll-down if there are bounties
			{
				AddButton( 393, 145, 2084, 2084, 4, GumpButtonType.Reply, 0 ); // Scroll up
				AddButton( 390, 371, 2085, 2085, 5, GumpButtonType.Reply, 0 ); // Scroll down
			}

			string title = "<CENTER>Bounty Board</CENTER>";//board.Title;

			if ( title != null )
			{
				AddHtml( 183, 68, 180, 23, title, false, false );
			}
			if( BountyKeeper.NumberOfUniqueBounties > 0 )
			{
				AddLabel( 440, 89, LabelHue, (page + 1).ToString() );
				AddLabel( 455, 89, LabelHue, "/" );
				AddLabel( 470, 89, LabelHue, BountyKeeper.NumberOfUniqueBounties.ToString() );
			}

			if( BountyKeeper.NumberOfUniqueBounties == 0 )
			{
				AddHtml( 150, 240, 250, 100, "There are no current bounties listed for collection.  All the citizens of Britannia have been good!", false, false);
			}
			else //list the bounties!
			{
				if( (page >= 0) && (page < BountyKeeper.NumberOfUniqueBounties) )
				{
					Bounty b = BountyKeeper.GetBounty(page);
					int thisplayerbounties = BountyKeeper.BountiesOnPlayer(b.WantedPlayer);
					bool eligibleforlbbonus = BountyKeeper.IsEligibleForLBBonus(b.WantedPlayer);
					
                    string output = "";

					if( eligibleforlbbonus )
					{
						if( thisplayerbounties > 1 )
						{
							output = string.Format("Lord British and {0} others have placed a bounty on the head of {1}.", thisplayerbounties, b.WantedPlayer.Name);
						}
						else
						{
							output = string.Format("Lord British and {0} have placed a bounty on the head of {1}.", b.PlacerName, b.WantedPlayer.Name);
						}
					}
					else
					{
						if( thisplayerbounties > 1 )
						{
							output = string.Format("{0} people have placed a bounty on the head of {1}.", thisplayerbounties, b.WantedPlayer.Name);
						}
						else
						{
							output = string.Format("{0} has placed a bounty on the head of {1}.", b.PlacerName, b.WantedPlayer.Name);
						}
					}

					AddHtml( 150, 180, 250, 60, output, false, false);
					if( eligibleforlbbonus )
					{
						output = string.Format("The reward is {0} gold!", 
							BountyKeeper.RewardForPlayer(b.WantedPlayer) + BountyKeeper.CurrentLBBonusAmount );
					}
					else
					{
						output = string.Format("The reward is {0} gold!", 
							BountyKeeper.RewardForPlayer(b.WantedPlayer));
					}
					AddHtml( 150, 240, 250, 60, output, false, false);
				}
			}

		}
	}
}*/

