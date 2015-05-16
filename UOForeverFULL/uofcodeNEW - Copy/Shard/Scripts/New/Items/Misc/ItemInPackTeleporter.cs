using System;
using Server;
using Server.Factions;

namespace Server.Items
{
	public class ItemInPackTeleporter : Teleporter
	{
		private string m_DeclineMessage;

		[CommandProperty( AccessLevel.GameMaster)]
        	public string DeclineMessage { get{ return m_DeclineMessage; } set{ m_DeclineMessage = value; } }

		[Constructable]
		public ItemInPackTeleporter() : base()
		{
		}

		public override bool OnMoveOver( Mobile m )
		{
			if ( m == null || m.Deleted || m.Backpack == null || m.Backpack.Deleted )
				return true;

			if ( Active && (Creatures || m.Player) )
			{
				if ( m.Backpack != null && m.Backpack.Items.Count > 0 )
				{
					StartTeleport(m);
					return false;
				}
				else if ( !String.IsNullOrEmpty( m_DeclineMessage ) )
					m.SendMessage( m_DeclineMessage );
			}

			return true;
		}

		public ItemInPackTeleporter( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version

			writer.Write( m_DeclineMessage );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();

			m_DeclineMessage = reader.ReadString();
		}
	}
}