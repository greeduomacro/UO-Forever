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

/* Engines/BountySystem/Bounty.cs
 * CHANGELOG:
 *	1/27/05, Pix
 *		Added optional Placer name to override Placer.Name on the bounty board.
 *	5/24/04, Pixie
 *		Put checks in Save that all the data in the bounty 
 *		is non-null.
 *	5/16/04, Pixie
 *		Initial Version
 */
/*
using System;
using System.Xml;
using Server.Mobiles;

namespace Server.BountySystem
{
	/// <summary>
	/// Summary description for Bounty.
	/// </summary>
	public class Bounty
	{
		private PlayerMobile player_wanted;
		private PlayerMobile player_placed;
		private DateTime m_datePlaced;
		private int m_reward;
		private bool m_bLordBritishBonus;
		private string m_placername;

		public Bounty(PlayerMobile placer, PlayerMobile wanted, int rewardamount)
		{
			player_wanted = wanted;
			player_placed = placer;
			m_reward = rewardamount;
			m_datePlaced = DateTime.UtcNow;
			m_bLordBritishBonus = false;
		}
		public Bounty(PlayerMobile placer, PlayerMobile wanted, int rewardamount, bool LBBonus)
		{
			player_wanted = wanted;
			player_placed = placer;
			m_reward = rewardamount;
			m_datePlaced = DateTime.UtcNow;
			m_bLordBritishBonus = LBBonus;
		}

		/// <summary>
		/// Special case for [macroer command
		/// Bounty will use "PlacerName" instead of placer.Name for the bounty issuer.
		/// </summary>
		public Bounty(PlayerMobile placer, PlayerMobile wanted, int rewardamount, string PlacerName)
		{
			player_wanted = wanted;
			player_placed = placer;
			m_reward = rewardamount;
			m_datePlaced = DateTime.UtcNow;
			m_bLordBritishBonus = LBBonus;
			m_placername = PlacerName;
		}

		public Bounty( XmlElement node )
		{
			m_datePlaced = BountyKeeper.GetDateTime( BountyKeeper.GetText( node["date"], null ), DateTime.UtcNow );
			m_reward = BountyKeeper.GetInt32( BountyKeeper.GetText( node["amount"], "0"), 0 );

			int serial = BountyKeeper.GetInt32( BountyKeeper.GetText( node["wanted"], "0" ), 0 );
			player_wanted = (PlayerMobile)World.FindMobile( serial );

			serial = BountyKeeper.GetInt32( BountyKeeper.GetText( node["placed"], "0" ), 0 );
			player_placed = (PlayerMobile)World.FindMobile( serial );

			string strValue = BountyKeeper.GetText(node["LBBonus"], "true");
			if( strValue.Equals("true") )
			{
				m_bLordBritishBonus = true;
			}
			else
			{
				m_bLordBritishBonus = false;
			}

			string strTest = BountyKeeper.GetText( node["placername"], null);
			if( strTest != null )
			{
				m_placername = strTest;
			}
		}

		public int Reward
		{
			get{ return m_reward; }
		}

		public PlayerMobile Placer
		{
			get{ return player_placed; }
		}

		public PlayerMobile WantedPlayer
		{
			get{ return player_wanted; }
		}

		public DateTime RewardDate
		{
			get{ return m_datePlaced; }
		}

		public bool LBBonus
		{
			get{ return m_bLordBritishBonus;}
		}

		public string PlacerName
		{
			get
			{
				if( m_placername != null )
				{
					return m_placername;
				}
				else
				{
					return Placer.Name;
				}
			}
		}

		public void Save( XmlTextWriter xml )
		{
			string strWanted = player_wanted.Serial.Value.ToString();
			string strPlaced = player_placed.Serial.Value.ToString();
			string strDate = XmlConvert.ToString( m_datePlaced );
			string strAmount = m_reward.ToString();

			if( strWanted == null ||
				strPlaced == null ||
				strDate == null ||
				strAmount == null )
			{
			}
			else
			{
				xml.WriteStartElement( "bounty" );

				xml.WriteStartElement( "wanted" );
				xml.WriteString( strWanted );
				xml.WriteEndElement();

				xml.WriteStartElement( "placed" );
				xml.WriteString( strPlaced );
				xml.WriteEndElement();

				xml.WriteStartElement( "date" );
				xml.WriteString( strDate );
				xml.WriteEndElement();

				xml.WriteStartElement( "amount" );
				xml.WriteString( strAmount );
				xml.WriteEndElement();

				xml.WriteStartElement( "LBBonus" );
				if( m_bLordBritishBonus )
				{
					xml.WriteString( "true" );
				}
				else
				{
					xml.WriteString( "false" );
				}
				xml.WriteEndElement();

				if( m_placername != null )
				{
					xml.WriteStartElement( "placername" );
					xml.WriteString( m_placername );
					xml.WriteEndElement();
				}

				xml.WriteEndElement();		
			}
		}
	
	}
}
*/