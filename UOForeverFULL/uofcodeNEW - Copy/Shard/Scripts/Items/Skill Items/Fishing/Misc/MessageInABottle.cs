using System;
using Server.Network;

namespace Server.Items
{
	public class MessageInABottle : Item
	{
		public static int GetRandomLevel()
		{
			if (/* Core.AOS &&*/ 1 > Utility.Random( 25 ) )//changed this to delete the mibs for uor
				return 5; // ancient

			return Utility.RandomMinMax( 2, 4 );
		}

		public override int LabelNumber{ get{ return 1041080; } } // a message in a bottle

		private Map m_TargetMap;
		private int m_Level;

		[CommandProperty( AccessLevel.GameMaster )]
		public Map TargetMap
		{
			get{ return m_TargetMap; }
			set{ m_TargetMap = value; }
		}

        public void UpdateHue()
        {
            if (IsAncient)
                Hue = 0x481;
            else
                Hue = 0;
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsAncient
        {
            get { return (m_Level > 4); }
        }

		[CommandProperty( AccessLevel.GameMaster )]
		public int Level
		{
			get{ return m_Level; }
			set{ m_Level = Math.Max( 2, Math.Min( value, 5 ) ); }
		}

		[Constructable]
		public MessageInABottle() : this( Map.Felucca )
		{
		}

		public MessageInABottle( Map map ) : this( map, GetRandomLevel() )
		{
		}

		[Constructable]
		public MessageInABottle( Map map, int level ) : base( 0x099F )
		{
			Weight = 1.0;
			m_TargetMap = map;
			m_Level = level;
            UpdateHue();
		}

		public MessageInABottle( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 3 ); // version

			writer.Write( (int) m_Level );

			writer.Write( m_TargetMap );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 3:
				case 2:
				{
					m_Level = reader.ReadInt();
					goto case 1;
				}
				case 1:
				{
					m_TargetMap = reader.ReadMap();
					break;
				}
				case 0:
				{
					m_TargetMap = Map.Felucca;
					break;
				}
			}

			if ( version < 2 )
				m_Level = GetRandomLevel();

			if( version < 3 && m_TargetMap == Map.Tokuno )
				m_TargetMap = Map.Felucca;
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( IsChildOf( from.Backpack ) )
			{
				ReplaceWith( new SOS( m_TargetMap, m_Level ) );
				from.LocalOverheadMessage( Network.MessageType.Regular, 0x3B2, 501891 ); // You extract the message from the bottle.
			}
			else
			{
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			}
		}
	}
}