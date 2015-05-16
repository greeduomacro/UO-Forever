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

/* Engines/BountySystem/BountyKeeper.cs
 * CHANGELOG:
 *  1/5/07, Rhiannon
 *      Added check so bounties cannot be placed on staff members.
 *  6/2/04, Pixie
 *		Made Bounties property private for arraylist safety purposes.
 *	5/26/04, Pixie
 *		Changed CollectBounty() call for individual message capability for different NPC types.
 *		Changed flagging for the Bounty command to use the PermaFlag system, so the person placing
 *		the bounty is grey only to the person they placed the bounty on, but there's a 2-minute timer
 *		on the grey-flagging.
 *  5/24/04, Pixie
 *		Put try-catch block around saving of the bounty system
 *		so it will never crash the server.
 *		Also, put checks in that if the Placer or the Wanted of a bounty
 *		have been deleted, then the bounty is deleted and the funds are 
 *		added to the LBFund.
 *  5/18/04, Pixie
 *		Changs to LBBonus and to display only
 *		1 bounty per wanted player
 *	5/17/04, Pixie
 *		On use of the bounty command, flags player criminal, but doesn't guardwhack.
 *	5/16/04, Pixie
 *		Initial Version
 */

/*using System;
//using System.Net;
//using System.Text;
using System.Collections;
//using System.Diagnostics;
using System.IO;
using System.Xml;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Commands;
using Server.Commands.Generic;

namespace Server.BountySystem
{
	/// <summary>
	/// Summary description for BountyKeeper.
	/// </summary>
	public class BountyKeeper
	{
		private static bool bEnabled = true;
		private static bool DEBUG = true;
		private static ArrayList m_bounties = new ArrayList();
		private static int m_LBFundAmount;

		public static void Configure()
		{
			EventSink.WorldLoad += new WorldLoadEventHandler(OnLoad); 
			EventSink.WorldSave += new WorldSaveEventHandler(OnSave); 
		} 


		public static void Add(Bounty b)
		{
            m_bounties.Add(b);
		}

		public static void Remove(Bounty b)
		{
			m_bounties.Remove(b);
		}

		private static ArrayList Bounties
		{
			get{ return m_bounties; }
		}

		public static bool Active
		{
			get{ return bEnabled; }
		}

		public static int LBFund
		{
			get{ return m_LBFundAmount; }
			set{ m_LBFundAmount = value; }
		}

		public static bool CanCollectReward( PlayerMobile collector, PlayerMobile dead )
		{
			bool bReturn = true;

			//Put all the checks in here to see if collector can collect the reward on dead's head.
			if( collector == dead )
			{
				bReturn = false;
			}

			if( collector.Guild != null &&
				collector.Guild == dead.Guild )
			{
				bReturn = false;
			}

			if( collector.Account == dead.Account )
			{
				bReturn = false;
			}

			return bReturn;
		}

		public static int CurrentLBBonusAmount
		{
			get
			{
				int iAmount = 0;
				iAmount = BountyKeeper.m_LBFundAmount / NumberOfUniqueBounties;
				return iAmount;
			}
		}

		public static int RewardForPlayer( PlayerMobile p )
		{
			int iReturn = 0;

			foreach( Bounty b in BountyKeeper.Bounties )
			{
				if( b.WantedPlayer == p )
				{
					iReturn += b.Reward;
				}
			}

			return iReturn;
		}

		public static bool IsEligibleForLBBonus( PlayerMobile p )
		{
			bool bReturn = false;
			foreach( Bounty b in BountyKeeper.Bounties )
			{
				if( b.WantedPlayer == p )
				{
					if( b.LBBonus == true )
					{
						bReturn = true;
						break;
					}
				}
			}
			return bReturn;
		}

		public static int BountiesOnPlayer(PlayerMobile p)
		{
			int iCount = 0;
			foreach( Bounty b in BountyKeeper.Bounties )
			{
				if( b.WantedPlayer == p )
				{
					iCount++;
				}
			}
			return iCount;
		}

		public static int NumberOfUniqueBounties
		{
			get
			{
				int iNumber = 0;

				ArrayList uniqueplayers = new ArrayList();
				foreach(Bounty b in BountyKeeper.Bounties)
				{
					if( uniqueplayers.Contains(b.WantedPlayer) )
					{
					}
					else
					{
						uniqueplayers.Add(b.WantedPlayer);
					}
				}
				iNumber = uniqueplayers.Count;

				return iNumber;
			}
		}

		public static Bounty GetBounty(int i)
		{
			int count = 0;
			if( i==0 )
			{
				return (Bounty)BountyKeeper.Bounties[0];
			}

			ArrayList uniqueplayers = new ArrayList();
			foreach(Bounty b in BountyKeeper.Bounties)
			{
				if( uniqueplayers.Contains(b.WantedPlayer) )
				{
				}
				else
				{
					uniqueplayers.Add(b.WantedPlayer);
					count++;

					if( i == (count-1) )
					{
						return b;
					}
				}
			}

			return (Bounty)BountyKeeper.Bounties[0]; //safety
		}

		private static void RemoveOldBounties()
		{
			ArrayList toRemove = new ArrayList();
			foreach( Bounty b in m_bounties )
			{
				//Rewards stay viable for 4 weeks + 1 hour per 100 gold above 500
				//rewarddate + 4 weeks + (amount-500)/100 hours
				if( ( b.RewardDate + TimeSpan.FromDays(28) + TimeSpan.FromHours((b.Reward-500)/100) ) < DateTime.UtcNow )
				{
					toRemove.Add(b);
				}
				else if( b.WantedPlayer == null || b.WantedPlayer.Deleted )
				{
					toRemove.Add(b);
				}
				else if( b.Placer == null || b.Placer.Deleted )
				{
					toRemove.Add(b);
				}
			}

			foreach( Bounty b in toRemove )
			{
				//add reward to LB fund
				m_LBFundAmount += b.Reward;
				//remove reward
				m_bounties.Remove(b);
			}
		}


		//returns:
		//true if head taken, false if not
		//goldRewarded is the gold placed in the placer's back
		//message is:
		//		-1: default - meaningless
		//		-2: player head - no bounty
		//		-3: player head, bounty, illegal return, no bounty given
		//		1: player head - bounty given
		public static bool CollectBounty( Head h, Mobile from, Mobile collector, ref int goldGiven, ref int message )
		{
			bool bReturn = false;
			goldGiven = 0;
			message = -1;
			try
			{
				int goldRewarded = 0;
				int goldForLBBonus = 0;
				bool eligibleforlbbonus = false;
				if( h.IsPlayerHead )
				{
					if( BountyKeeper.CanCollectReward( (PlayerMobile)from, h.Player ) )
					{
						ArrayList found_bounties = new ArrayList();
						foreach( Bounty b in BountyKeeper.Bounties )
						{
							if( b.WantedPlayer == h.Player )
							{
								if( h.Time > b.RewardDate )
								{
									goldRewarded += b.Reward;
									if( b.LBBonus )
									{
										eligibleforlbbonus = true;
									}
									found_bounties.Add(b);
								}
							}
						}

						if( eligibleforlbbonus )
						{
							goldForLBBonus = BountyKeeper.CurrentLBBonusAmount;
						}
						bool bRewardGiven = false;
						if( goldRewarded > 0 )
						{
							message = 1;
							//collector.Say( string.Format("My thanks for slaying this vile person.  Here's the reward of {0} gold!", goldRewarded) );
							Container c = from.Backpack;
							if( c != null )
							{
								//Gold g = new Gold(goldRewarded + goldForLBBonus);
								BankCheck g = new BankCheck(goldRewarded + goldForLBBonus);
								goldGiven = goldRewarded + goldForLBBonus;
								BountyKeeper.LBFund -= goldForLBBonus;
								from.AddToBackpack( g );
								bReturn = true;
								bRewardGiven = true;
								h.Player.MurderBounty = h.Player.MurderBounty + goldRewarded;
							}
						}
						else
						{
							message = -2;
							//collector.Say("You disgusting miscreant!  Why are you giving me an innocent person's head?");
						}

						if( bRewardGiven )
						{
							foreach(Bounty b in found_bounties)
							{
								BountyKeeper.Bounties.Remove(b);
							}
							found_bounties.Clear();
						}
							
					}
					else
					{
						message = -3;
						//collector.Say("I suspect treachery....");
						//collector.Say("I'll take that head, you just run along now.");
						bReturn = true;
					}
				}
			}
			catch( Exception e )
			{
				System.Console.WriteLine("Error (nonfatal) in BaseGuard.OnDragDrop(): " + e.Message);
				System.Console.WriteLine(e.StackTrace);
			}

			return bReturn;
		}
		
		public static void OnSave( WorldSaveEventArgs e ) 
		{ 
			if( DEBUG )	Console.WriteLine("Bounty Saving...");

			//Get rid of stale bounties
			RemoveOldBounties();

			try
			{
				if ( !Directory.Exists( "Saves/BountySystem" ) )
					Directory.CreateDirectory( "Saves/BountySystem" );

				string filePath = Path.Combine( "Saves/BountySystem", "Bounty.xml" );

				using ( StreamWriter op = new StreamWriter( filePath ) )
				{
					XmlTextWriter xml = new XmlTextWriter( op );

					xml.Formatting = Formatting.Indented;
					xml.IndentChar = '\t';
					xml.Indentation = 1;

					xml.WriteStartDocument( true );

					xml.WriteStartElement( "Bounties" );

					xml.WriteAttributeString( "count", m_bounties.Count.ToString() );
					xml.WriteAttributeString( "LBFund", m_LBFundAmount.ToString() );

					foreach ( Bounty b in m_bounties )
					{
						b.Save( xml );
					}

					xml.WriteEndElement();

					xml.Close();
				}
			}
			catch( Exception exc )
			{
				System.Console.WriteLine("Error in BountyKeeper.OnSave(): " + exc.Message);
				System.Console.WriteLine(exc.StackTrace);
			}
		}



		public static void OnLoad( ) 
		{ 
			if(DEBUG) Console.WriteLine("Bounty Loading..."); 
			
			string filePath = Path.Combine( "Saves/BountySystem", "Bounty.xml" );

			if ( !File.Exists( filePath ) )
			{
				m_LBFundAmount = 1000;
				return;
			}

				XmlDocument doc = new XmlDocument();
				doc.Load( filePath );

				XmlElement root = doc["Bounties"];

				try
				{
					string strFund = root.GetAttribute("LBFund");
					m_LBFundAmount = Int32.Parse(strFund);
				}
				catch
				{
					m_LBFundAmount = 1000;
				}

				foreach ( XmlElement bounty in root.GetElementsByTagName( "bounty" ) )
				{
					try
					{
						Bounty b = new Bounty(bounty);

						if( b.WantedPlayer != null &&
							b.Reward > 0 &&
							b.RewardDate <= DateTime.UtcNow )
						{
							m_bounties.Add(b);
						}
					}
					catch
					{
						Console.WriteLine( "Warning: A bounty instance load failed" );
					}
				}
		}

		public static string GetText( XmlElement node, string defaultValue )
		{
			if ( node == null )
				return defaultValue;

			return node.InnerText;
		}
		public static DateTime GetDateTime( string dateTimeString, DateTime defaultValue )
		{
			try
			{
				return XmlConvert.ToDateTime( dateTimeString );
			}
			catch
			{
				try
				{
					return DateTime.Parse( dateTimeString );
				}
				catch
				{
					return defaultValue;
				}
			}
		}
		public static int GetInt32( string intString, int defaultValue )
		{
			try
			{
				return XmlConvert.ToInt32( intString );
			}
			catch
			{
				try
				{
					return Convert.ToInt32( intString );
				}
				catch
				{
					return defaultValue;
				}
			}
		}

		
	}
}*/
