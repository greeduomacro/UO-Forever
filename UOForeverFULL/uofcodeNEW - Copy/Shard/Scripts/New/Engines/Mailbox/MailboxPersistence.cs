using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;

namespace Server.Engines.MailboxSystem
{
	public class MailboxPersistence : Item
	{
		private static MailboxPersistence m_Instance;
		public static MailboxPersistence Instance { get { return m_Instance; } }

		public override string DefaultName
		{
			get { return "Mailbox Persistence - Internal"; }
		}

		public static void Initialize()
		{
			new MailboxPersistence();
		}

		[Constructable]
		public MailboxPersistence() : base( 1 )
		{
			Movable = false;

			if ( m_Instance == null || m_Instance.Deleted )
				m_Instance = this;
			else
				base.Delete();
		}

		public override void Delete()
		{
		}

		public MailboxPersistence( Serial serial ) : base( serial )
		{
			m_Instance = this;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			writer.Write( MailSystem.MailMessages.Count );

			foreach ( KeyValuePair<MailSerial, IMailMessage> pair in MailSystem.MailMessages )
			{
				writer.Write( (byte) GetMailMessageIndex( pair.Value ) );
				writer.Write( (int) pair.Key ); //Serial
				pair.Value.Serialize( writer ); // IMail
			}
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			int dictcount = reader.ReadInt();

			for ( int i = 0; i < dictcount; i++ )
			{
				IMailMessage message = NewMailMessage( reader.ReadByte(), reader.ReadInt() );
				message.Deserialize( reader );
			}
		}

		private static IMailMessage NewMailMessage( byte index, MailSerial serial )
		{
			switch ( index )
			{
				default: case 0: return new MailMessage( serial );
				case 1: return new MultiMailMessage( serial );
				//case 2: return new AuctionMailMessage( serial );
			}
		}

		private static byte GetMailMessageIndex( IMailMessage obj )
		{
			Type type = obj.GetType();
			if ( type == typeof( MailMessage ) )
				return 0;
			else if ( type == typeof( MultiMailMessage ) )
				return 1;
			//else if ( type == typeof( AuctionMailMessage ) )
			//	return 2;

			return 0;
		}
	}
}