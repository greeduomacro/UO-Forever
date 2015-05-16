using System;
using System.Collections.Generic;
using Server.Mobiles;

namespace Server.Engines.MailboxSystem
{
	public class MailSystem
	{
		private static Queue<IMailMessage> _addQueue, _deleteQueue;
		private static Dictionary<MailSerial, IMailMessage> m_MailMessages;
		private static Dictionary<PlayerMobile, List<IMailMessage>> m_Recipients = new Dictionary<PlayerMobile, List<IMailMessage>>();
		private static Dictionary<PlayerMobile, List<IMailMessage>> m_Senders = new Dictionary<PlayerMobile, List<IMailMessage>>();

		public static Dictionary<MailSerial, IMailMessage> MailMessages{ get{ return m_MailMessages; } }
		public static Dictionary<PlayerMobile, List<IMailMessage>> Recipients{ get{ return m_Recipients; } }
		public static Dictionary<PlayerMobile, List<IMailMessage>> Senders{ get{ return m_Senders; } }

		public static void Configure() //Before world loads
		{
			EventSink.WorldLoad += new WorldLoadEventHandler( EventSink_WorldLoad );

			m_MailMessages = new Dictionary<MailSerial, IMailMessage>();
			_addQueue = new Queue<IMailMessage>();
			_deleteQueue = new Queue<IMailMessage>();
		}

		private static void EventSink_WorldLoad()
		{
			ProcessSafetyQueues();
		}

		public static IMailMessage FindMessage( MailSerial serial )
		{
			IMailMessage message;
			m_MailMessages.TryGetValue( serial, out message );
			return message;
		}

		public static void AddMessage( IMailMessage message )
		{
			if ( World.Saving )
				_addQueue.Enqueue( message );
			else
				m_MailMessages[message.Serial] = message;
		}

		public static void RemoveMessage( IMailMessage message )
		{
			m_MailMessages.Remove( message.Serial );
		}

		private static void ProcessSafetyQueues()
		{
			while ( _addQueue.Count > 0 )
			{
				IMailMessage message = _addQueue.Dequeue();

				if ( message != null )
					AddMessage( message );
			}

			while ( _deleteQueue.Count > 0 )
			{
				IMailMessage message = _deleteQueue.Dequeue();

				if ( message != null )
					RemoveMessage( message );
			}
		}

		//How long the receiver keeps the mail before it expires.  The delay is factored in.
		public static readonly TimeSpan MailDuration = TimeSpan.FromDays( 30.0 );

		//This is the dealy between sending the mail, and receiving it for certain circumstances.
		public static readonly TimeSpan MailReceiveDelay = TimeSpan.FromMinutes( 10.0 );

		//Beyond this capacity will prevent someone from receiving mail or using the auction system.
		public static readonly int MailboxCapacity = 500;

		public static readonly int SubjectLength = 128;

		public static readonly int BodyLength = 1024;

		public static readonly int MaxAttached = 16;

		public static readonly int MaxSenders = 16;

		public static readonly string[] m_DisallowedRecipients = new string[]
			{
				"Player",
				"(-null-)",
				"Generic Player",
				"Admin",
				"GM",
				"Counselor",
				"Seer",
				"Developer",
				"Dev",
				"Founder",
				"Owner"
			};
	}
}