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

/* Engines/BountySystem/BountyBoards.cs
 * CHANGELOG:
 *	1/27/05, Pix
 *		Added optional Placer name to override Placer.Name on the bounty board.
 *	5/16/04, Pixie
 *		Initial Version.
 */

/*using System;
using System.Collections;
using System.Text;
using Server;
using Server.Network;
using Server.BountySystem;

namespace Server.Items
{
	[Flipable( 0x1E5E, 0x1E5F )]
	public class BountyBoard : BaseBountyBoard
	{
		[Constructable]
		public BountyBoard() : base( 0x1E5E )
		{
		}

		public BountyBoard( Serial serial ) : base( serial )
		{
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int LBFundAmount
		{
			get{ return BountyKeeper.LBFund; }
			set
			{
				if( value >= 0 )
				{
					BountyKeeper.LBFund = value;
				}
			}
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public abstract class BaseBountyBoard : Item
	{
		private string m_BoardName;

		[CommandProperty( AccessLevel.GameMaster )]
		public string BoardName
		{
			get{ return m_BoardName; }
			set{ m_BoardName = value; }
		}

		public BaseBountyBoard( int itemID ) : base( itemID )
		{
			m_BoardName = "Bounty board";
			Movable = false;
			Hue = 0x3FF;
		}




		public override void OnSingleClick( Mobile from )
		{
			this.LabelTo( from, "Bounty Board" );
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( CheckRange( from ) )
			{
				//Cleanup();
				//from.Send( new BountyDisplayBoard( this ) );
				//from.Send( new ContainerContent( from, this ) );

				from.SendGump( new BountyDisplayGump(from, 0) );
			}
			else
			{
				from.LocalOverheadMessage( MessageType.Regular, 0x3B2, 1019045 ); // I can't reach that.
			}
		}

		public virtual bool CheckRange( Mobile from )
		{
			if ( from.AccessLevel >= AccessLevel.GameMaster )
				return true;

			return ( from.Map == this.Map && from.InRange( GetWorldLocation(), 2 ) );
		}


		public BaseBountyBoard( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			writer.Write( (string) m_BoardName );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 0:
				{
					m_BoardName = reader.ReadString();
					break;
				}
			}
		}
	}

}*/