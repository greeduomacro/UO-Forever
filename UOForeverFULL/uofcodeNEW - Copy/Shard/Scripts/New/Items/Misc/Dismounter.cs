using System;
using Server;
using Server.Mobiles;
using Server.Gumps;

namespace Server.Items
{
	public class Dismounter : Item
	{
        #region Variables/Properties
        private bool m_Active;
		private Direction m_Direction;
		private string m_Message;
		private bool m_MountOnly;
		private bool m_Silent;
		private double m_Chance;
		private string m_Emote;

		private char[] trimChar = {'*',' '};

        [CommandProperty( AccessLevel.GameMaster )]
		public String Emote
		{
			get { return m_Emote; }
			set
			{
				m_Emote = value;
				m_Emote = '*' + m_Emote.TrimStart( trimChar );
				m_Emote = m_Emote.TrimEnd( trimChar ) + '*';
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int Chance
		{
			get { return (int)( m_Chance * 100 ); }
			set
			{
				int num = value;

				if ( num > 100 )
					num = 100;
				else if ( num < 1 )
				{
					m_Chance = 0;
					m_Active = false;
					return;
				}

				m_Chance = (double)num / 100 ;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Active
		{
			get { return m_Active; }
			set { m_Active = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Direction Facing
		{
			get { return m_Direction; }
			set { m_Direction = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public String Message
		{
			get { return m_Message; }
			set { m_Message = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool BlockMountOnly
		{
			get { return m_MountOnly; }
			set { m_MountOnly = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Silent
		{
			get { return m_Silent; }
			set { m_Silent = value; }
        }
        #endregion

        #region Constructors
        [Constructable]
		public Dismounter() : this( Direction.Down, null, true )
		{
		}

		[Constructable]
		public Dismounter( Direction dir ) : this( dir, null, true )
		{
		}

		[Constructable]
		public Dismounter( Direction dir, string msg ) : this( dir, msg, true )
		{
		}

		[Constructable]
		public Dismounter( Direction dir, bool active ) : this( dir, null, active )
		{
		}

		[Constructable]
		public Dismounter( Direction dir, string msg, bool active ) : base( 0x1B7A )
		{
			Movable = false;
			Visible = false;
			Name = "Dismounter";

			m_Active = active;
			m_Direction = dir;
			m_Message = msg;
			m_Chance = 1; // 100% Chance to dismount
		}
        #endregion

        #region Methods
        public override void OnDoubleClick(Mobile from)
		{
			if ( from.AccessLevel < AccessLevel.GameMaster )
				return;

			from.SendGump( new PropertiesGump( from, this ) );
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			list.Add( this.Name );

			if ( m_Active )
				list.Add( 1060742 ); // active
			else
				list.Add( 1060743 ); // inactive

			list.Add( (m_Direction == Direction.Mask) ? "Up" : ((Direction)m_Direction).ToString() );
		}

		public override void OnSingleClick( Mobile from )
		{
			base.OnSingleClick( from );

			if ( m_Active )
			{
				LabelTo( from, "Facing " + ((m_Direction == Direction.Mask) ? "Up" : ((Direction)m_Direction).ToString()) );
			}
			else
			{
				LabelTo( from, "(inactive)" );
			}
		}

		public override bool OnMoveOver( Mobile m )
		{
			if ( m_Active )
			{
				if ( Utility.RandomDouble() > m_Chance )
				{
					return true;
				}

				if ( m.Player && m.Mounted )
				{
					IMount mount = (IMount)m.Mount;
					mount.Rider = null;

					if ( mount is BaseMount )
					{
						((BaseMount)mount).Direction = m_Direction;
						DoSound( (BaseMount)mount );
					}

					if ( m_Message != null )
						m.SendMessage( m_Message );

					if ( m_Emote != null )
						m.Emote( m_Emote );
				}
				else if ( m_MountOnly && m is BaseMount )
				{
					((BaseMount)m).Direction = m_Direction;
					DoSound( m );
					return false;
				}
			}
			return true;
		}

		private void DoSound( Mobile m )
		{
			if( m_Silent ) return;
			Effects.PlaySound( m.Location, m.Map, m.BaseSoundID + Utility.Random( 4 ) );
        }
        #endregion

        #region Serialize/Deserialize
        public Dismounter( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 3 ); // version

			writer.Write( m_Chance );
			writer.Write( m_Emote );

			writer.Write( m_Silent );
			writer.Write( m_MountOnly );

			writer.Write( m_Message );
			writer.Write( m_Active );
			writer.Write( (int)m_Direction );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 3:
				{
					m_Chance = reader.ReadDouble();
					m_Emote = reader.ReadString();
					goto case 2;
				}
				case 2:
				{
					if( version < 3 ) m_Chance = 100;

					m_Silent = reader.ReadBool();
					m_MountOnly = reader.ReadBool();
					goto case 1;
				}
				case 1:
				{
					m_Message = reader.ReadString();
					goto case 0;
				}
				case 0:
				{
					m_Active = reader.ReadBool();
					m_Direction = (Direction)reader.ReadInt();
					break;
				}
			}
        }
        #endregion
    }
}