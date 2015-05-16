using System;
using Server;
using Server.Factions;
using Server.Mobiles;

namespace Server.Items
{
	public class CheckForItemTeleporter : Teleporter
	{
		private Type m_ItemType;
		private string m_DeclineMessage;

		[CommandProperty( AccessLevel.GameMaster)]
		public Type ItemType { get { return m_ItemType; } set { m_ItemType = value; } }

		[CommandProperty( AccessLevel.GameMaster)]
        	public string DeclineMessage { get{ return m_DeclineMessage; } set{ m_DeclineMessage = value; } }

		[Constructable]
		public CheckForItemTeleporter() : base()
		{
		}

		public override bool OnMoveOver( Mobile m )
		{

			if ( m == null || m.Deleted || m.Backpack == null || m.Backpack.Deleted )
				return true;

			if ( Active && m_ItemType != null && (Creatures || m.Player) )
			{
				Item item = m.Backpack.FindItemByType( m_ItemType, true );
                PlayerMobile pm = m as PlayerMobile;
				if ( item != null && (pm != null && pm.GameParticipant == true))
				{
					StartTeleport(m);
					return false;
				}
				else if ( m_DeclineMessage != null )
					m.SendMessage( m_DeclineMessage );
			}

			return true;
		}

		public CheckForItemTeleporter( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version

			writer.Write( m_DeclineMessage );
			Reflector.Serialize( writer, m_ItemType );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();

			m_DeclineMessage = reader.ReadString();
			m_ItemType = Reflector.Deserialize( reader );
		}
	}
}