using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
	public class MsgSender : Item
	{
		private bool m_Active;
		private string m_Message;

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Active
		{
			get { return m_Active; }
			set { m_Active = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public String Message
		{
			get { return m_Message; }
			set { m_Message = value; InvalidateProperties(); }
		}

		[Constructable]
		public MsgSender() : this( null, false )
		{
		}

		[Constructable]
		public MsgSender( string msg ) : this( msg, true )
		{
		}

		[Constructable]
		public MsgSender( string msg, bool active ) : base( 0x1B73 )
		{
			Movable = false;
			Visible = false;
			Name = "Message Sender";

			m_Active = active;
			m_Message = msg;
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			list.Add( this.Name );

			if ( m_Active )
				list.Add( 1060742 ); // active
			else
				list.Add( 1060743 ); // inactive

			list.Add( m_Message );
		}

		public override void OnSingleClick( Mobile from )
		{
			base.OnSingleClick( from );

			if ( m_Active )
			{
				LabelTo( from, m_Message );
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
				if ( m.Player )
				{
					m.SendMessage( m_Message );
				}
			}
			return true;
		}

		public MsgSender( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version

			writer.Write( m_Active );
			writer.Write( m_Message );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();

			switch ( version )
			{
				case 0:
				{
					m_Active = reader.ReadBool();
					m_Message = reader.ReadString();
					break;
				}
			}
		}
	}
}