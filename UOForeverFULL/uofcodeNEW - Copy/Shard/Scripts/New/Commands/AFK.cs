using System;
using System.Text;
using System.Collections.Generic;
using Server;

namespace Server.Commands
{
	public class AFK : Timer
	{
		private static Dictionary<Mobile, AFK> m_AFK = new Dictionary<Mobile, AFK>();
		private Mobile m_Mobile;
		private Point3D m_Location;
		private DateTime m_DateTime;
		private string m_Message;

		public static void Initialize()
		{
			/*
            CommandSystem.Register( "AFK", AccessLevel.Player, new CommandEventHandler( AFK_OnCommand ) );

			EventSink.Logout += new LogoutEventHandler( OnLogout );
			EventSink.Speech += new SpeechEventHandler( OnSpeech );
			EventSink.PlayerDeath += new PlayerDeathEventHandler( OnDeath );
			EventSink.Movement += new MovementEventHandler( OnMovement );
             */
		}

		public static void OnDeath( PlayerDeathEventArgs e )
		{
			WakeUp( e.Mobile );
		}

		public static void OnLogout( LogoutEventArgs e )
		{
			WakeUp( e.Mobile );
		}

		public static void OnSpeech( SpeechEventArgs e )
		{
			WakeUp( e.Mobile );
		}

		public static void OnMovement( MovementEventArgs e )
		{
			WakeUp( e.Mobile );
		}

		[Usage( "AFK <message>" )]
		[Description( "Toggles afk status." )]
		public static void AFK_OnCommand( CommandEventArgs e )
		{
			Mobile m = e.Mobile;

			AFK afk;
			m_AFK.TryGetValue( m, out afk );
			if ( afk != null )
				afk.WakeUp();
			else
			{
				AFK timer = new AFK( m, e.ArgString.Trim() );
				m_AFK.Add( m, timer );
				timer.Start();
			}
		}

		public static void WakeUp( Mobile m )
		{
			AFK afk;
			m_AFK.TryGetValue( m, out afk );
			if ( afk != null )
				afk.WakeUp();
		}

		public void WakeUp()
		{
			m_AFK.Remove( m_Mobile );
			m_Mobile.PlaySound( m_Mobile.Female ? 814 : 1088 );
			m_Mobile.Say( "huh?" );
			m_Mobile.SendMessage( "Welcome back {0}.", m_Mobile.RawName );

			this.Stop();
		}

		public AFK( Mobile afker, string message ) : base( TimeSpan.FromSeconds( 60.0 ), TimeSpan.FromSeconds( 60.0 ) )
		{
			if ( String.IsNullOrEmpty( message ) )
				m_Message = "I'm out of my head.  Come back later.";
			else
				m_Message = message;

			m_Mobile = afker;
			m_DateTime = DateTime.UtcNow;
			m_Location = m_Mobile.Location;
		}
		protected override void OnTick()
		{
			if ( m_Mobile.Location != m_Location )
				WakeUp();
			else
			{
				m_Mobile.Say("zZz");
				TimeSpan ts = DateTime.UtcNow - m_DateTime;
				m_Mobile.Emote("*{0} ({1})*", m_Message, FormatTime( ts ) );
				m_Mobile.PlaySound( m_Mobile.Female ? 819 : 1093 );
			}
		}

		public static string FormatTime( TimeSpan ts )
		{
			StringBuilder sb = new StringBuilder();
			bool hours = false;

			if ( ts.TotalHours >= 1 )
			{
				hours = true;
				int h = (int)Math.Round( ts.TotalHours );
				sb.Append( String.Format( "{0} hour{1}", h, ( h == 1 ) ? "" : "s" ) );
			}
			
			if ( ts.TotalMinutes >= 1 )
			{
				int m = (int)Math.Round( ts.TotalMinutes );
				sb.Append( String.Format( "{2}{0} minute{1}", m, ( m == 1 ) ? "" : "s", hours ? " " : String.Empty ) );
			}
			else if ( !hours )
			{
				int s = Math.Max( (int)Math.Round( ts.TotalSeconds ), 0 );
				return String.Format( "{0} second{1}", s, ( s == 1 ) ? "" : "s" );
			}

			return sb.ToString();
		}
	}
}