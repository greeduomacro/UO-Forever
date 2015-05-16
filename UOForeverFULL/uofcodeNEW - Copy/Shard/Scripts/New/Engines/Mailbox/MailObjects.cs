using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;

namespace Server.Engines.MailboxSystem
{
	public interface IMailMessage
	{
		MailSerial Serial{ get; }
		PlayerMobile Sender{ get; }
		string Subject{ get; }
		string Body{ get; }
		DateTime Expiration{ get; }
		DateTime ReceiveDate{ get; }
		bool Expired{ get; }
		bool Received{ get; }

		void Cleanup();
		void Serialize( GenericWriter writer );
		void Deserialize( GenericReader reader );
	}

	public class MailMessage : IMailMessage
	{
		private MailSerial m_Serial;
		private PlayerMobile m_Sender;
		private PlayerMobile m_Recipient;
		private string m_Subject;
		private string m_Body;
		private MailContainer m_Attached;
		private int m_COD;
		private DateTime m_Expiration;
		private DateTime m_ReceiveDate;

		public MailSerial Serial{ get{ return m_Serial; } }
		public PlayerMobile Sender{ get{ return m_Sender; } }
		public PlayerMobile Recipient{ get{ return m_Recipient; } }
		public string Subject{ get{ return m_Subject; } }
		public string Body{ get{ return m_Body; } }
		public MailContainer Attached{ get{ return m_Attached; } }
		public int COD{ get{ return m_COD; } }
		public DateTime Expiration{ get{ return m_Expiration; } }
		public DateTime ReceiveDate{ get{ return m_ReceiveDate; } }

		public bool Expired{ get{ return DateTime.UtcNow > m_Expiration; } }
		public bool Received{ get{ return DateTime.UtcNow > m_ReceiveDate; } }

		public MailMessage( PlayerMobile sender, PlayerMobile recipient, string subject, string body ) : this( sender, recipient, subject, body, null, 0 )
		{
		}

		public MailMessage( PlayerMobile sender, PlayerMobile recipient, string subject, string body, TimeSpan delay ) : this( sender, recipient, subject, body, null, 0, delay )
		{
		}

		public MailMessage( PlayerMobile sender, PlayerMobile recipient, string subject, string body, MailContainer attached, int cod ) : this( sender, recipient, subject, body, attached, cod, TimeSpan.Zero )
		{
		}

		public MailMessage( PlayerMobile sender, PlayerMobile recipient, string subject, string body, MailContainer attached, int cod, TimeSpan delay )
		{
			m_Serial = MailSerial.NewMessage;
			MailSystem.AddMessage( this );
			m_Sender = sender;
			m_Recipient = recipient;
			m_Attached = attached;
			m_COD = cod;
			m_Expiration = DateTime.UtcNow + MailSystem.MailDuration;
			m_ReceiveDate = DateTime.UtcNow + delay;

			Add();
		}

		public MailMessage( MailSerial serial ) //For deserializing
		{
			m_Serial = serial;
			MailSystem.AddMessage( this );
		}

		public void Deserialize( GenericReader reader )
		{
			m_Sender = reader.ReadMobile() as PlayerMobile;
			m_Recipient = reader.ReadMobile() as PlayerMobile;
			m_Subject = reader.ReadString();
			m_Body = reader.ReadString();

			m_Attached = reader.ReadItem() as MailContainer;
			if ( m_Attached != null )
			{
				m_COD = reader.ReadInt();
				m_Attached.AttachedTo = this;
			}

			m_Expiration = reader.ReadDateTime();
			m_ReceiveDate = reader.ReadDateTime();

			if ( Expired )
				Timer.DelayCall( TimeSpan.FromSeconds( 1.0 ), new TimerCallback( Cleanup ) );
			else
				Add();
		}

		public void Serialize( GenericWriter writer )
		{
			writer.Write( m_Sender );
			writer.Write( m_Recipient );
			writer.Write( m_Subject );
			writer.Write( m_Body );
			writer.Write( m_Attached );
			if ( m_Attached != null )
				writer.Write( m_COD );
			writer.Write( m_Expiration );
			writer.Write( m_ReceiveDate );

			if ( Expired )
				Timer.DelayCall( TimeSpan.FromSeconds( 1.0 ), new TimerCallback( Cleanup ) );
		}

		public void Cleanup()
		{
			List<IMailMessage> sendlist;
			MailSystem.Senders.TryGetValue( m_Sender, out sendlist );

			if ( sendlist != null )
				sendlist.Remove( this );

			List<IMailMessage> reclist;
			MailSystem.Recipients.TryGetValue( m_Recipient, out reclist );

			if ( reclist != null )
				reclist.Remove( this );

			if ( reclist.Count == 0 )
					MailSystem.Recipients.Remove( m_Recipient );

			if ( m_Attached != null )
				m_Attached.Delete();
		}

		public void Add()
		{
			if ( m_Sender != null && m_Recipient != null )
			{
				List<IMailMessage> sendlist;
				MailSystem.Senders.TryGetValue( m_Sender, out sendlist );
				if ( sendlist == null )
					sendlist = new List<IMailMessage>();
				sendlist.Add( this );
				MailSystem.Senders[m_Sender] = sendlist;

				List<IMailMessage> reclist;
				MailSystem.Recipients.TryGetValue( m_Recipient, out reclist );
				if ( reclist == null )
					reclist = new List<IMailMessage>();
				reclist.Add( this );
				MailSystem.Recipients[m_Recipient] = reclist;
			}
		}
	}

	public class MultiMailMessage : IMailMessage
	{
		private MailSerial m_Serial;
		private PlayerMobile m_Sender;
		private PlayerMobile[] m_Recipients; //Guild option resolves to the members affected.
		private string m_Subject;
		private string m_Body;
		private DateTime m_Expiration;
		private DateTime m_ReceiveDate;

		public MailSerial Serial{ get{ return m_Serial; } }
		public PlayerMobile Sender{ get{ return m_Sender; } }
		public PlayerMobile[] Recipients{ get{ return m_Recipients; } }
		public string Subject{ get{ return m_Subject; } }
		public string Body{ get{ return m_Body; } }
		public DateTime Expiration{ get{ return m_Expiration; } }
		public DateTime ReceiveDate{ get{ return m_ReceiveDate; } }

		public bool Expired{ get{ return DateTime.UtcNow > m_Expiration; } }
		public bool Received{ get{ return DateTime.UtcNow > m_ReceiveDate; } }

		public MultiMailMessage( PlayerMobile sender, string subject, string body, params PlayerMobile[] recipients ) : this( sender, subject, body, TimeSpan.Zero, recipients )
		{
		}

		public MultiMailMessage( PlayerMobile sender, string subject, string body, TimeSpan delay, params PlayerMobile[] recipients )
		{
			m_Serial = MailSerial.NewMessage;
			MailSystem.AddMessage( this );
			m_Sender = sender;
			m_Recipients = recipients;
			m_Expiration = DateTime.UtcNow + MailSystem.MailDuration;
			m_ReceiveDate = DateTime.UtcNow + delay;

			Add();
		}

		public MultiMailMessage( MailSerial serial )
		{
			m_Serial = serial;
			MailSystem.AddMessage( this );
		}

		public void Deserialize( GenericReader reader )
		{
			m_Sender = reader.ReadMobile() as PlayerMobile;

			int count = reader.ReadInt();
			m_Recipients = new PlayerMobile[count];
			for ( int i = 0; i < count; i++ )
				m_Recipients[i] = reader.ReadMobile() as PlayerMobile;

			m_Subject = reader.ReadString();
			m_Body = reader.ReadString();

			m_Expiration = reader.ReadDateTime();
			m_ReceiveDate = reader.ReadDateTime();

			if ( Expired )
				Timer.DelayCall( TimeSpan.FromSeconds( 1.0 ), new TimerCallback( Cleanup ) );
			else
				Add();
		}

		public void Serialize( GenericWriter writer )
		{
			writer.Write( m_Sender );
			writer.Write( m_Recipients.Length );

			for ( int i = 0; i < m_Recipients.Length; i++ )
				writer.Write( m_Recipients[i] );

			writer.Write( m_Subject );
			writer.Write( m_Body );
			writer.Write( m_Expiration );
			writer.Write( m_ReceiveDate );

			if ( Expired )
				Timer.DelayCall( TimeSpan.FromSeconds( 1.0 ), new TimerCallback( Cleanup ) );
		}

		public void Cleanup()
		{
			List<IMailMessage> sendlist;
			MailSystem.Senders.TryGetValue( m_Sender, out sendlist );

			if ( sendlist != null && sendlist.Contains( this ) )
				sendlist.Remove( this );

			for ( int i = 0;i < m_Recipients.Length; i++ )
			{
				List<IMailMessage> reclist;
				MailSystem.Recipients.TryGetValue( m_Recipients[i], out reclist );

				if ( reclist != null && reclist.Contains( this ) )
					reclist.Remove( this );

				if ( reclist.Count == 0 )
					MailSystem.Recipients.Remove( m_Recipients[i] ); 
			}
		}

		public void Add()
		{
			if ( m_Sender != null && m_Recipients != null && m_Recipients.Length > 0 )
			{
				List<IMailMessage> sendlist;
				MailSystem.Senders.TryGetValue( m_Sender, out sendlist );
				if ( sendlist == null )
					sendlist = new List<IMailMessage>();
				sendlist.Add( this );
				MailSystem.Senders[m_Sender] = sendlist;

				for ( int i = 0; i < m_Recipients.Length; i++ )
				{
					List<IMailMessage> reclist;
					MailSystem.Recipients.TryGetValue( m_Recipients[i], out reclist );
					if ( reclist == null )
						reclist = new List<IMailMessage>();
					reclist.Add( this );
					MailSystem.Recipients[m_Recipients[i]] = reclist;
				}
			}
		}
	}
}