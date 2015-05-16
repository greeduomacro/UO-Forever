using System;
using System.Globalization;

namespace Server.Engines.MailboxSystem
{
	public struct MailSerial : IComparable, IComparable<MailSerial>
	{
		private int m_Serial;

		private static MailSerial m_LastMessage = Zero;

		public static MailSerial LastMessage { get { return m_LastMessage; } }

		public static readonly MailSerial MinusOne = new MailSerial( -1 );
		public static readonly MailSerial Zero = new MailSerial( 0 );

		public static MailSerial NewMessage
		{
			get
			{
				while ( MailSystem.FindMessage( m_LastMessage = (m_LastMessage + 1) ) != null );

				return m_LastMessage;
			}
		}

		private MailSerial( int serial )
		{
			m_Serial = serial;
		}

		public int Value
		{
			get
			{
				return m_Serial;
			}
		}

		public bool IsValid
		{
			get
			{
				return ( m_Serial > 0 );
			}
		}

		public override int GetHashCode()
		{
			return m_Serial;
		}

		public int CompareTo( MailSerial other )
		{
			return m_Serial.CompareTo( other.m_Serial );
		}

		public int CompareTo( object other )
		{
			if ( other is Serial )
				return this.CompareTo( (MailSerial) other );
			else if ( other == null )
				return -1;

			throw new ArgumentException();
		}

		public override bool Equals( object o )
		{
			if ( o == null || !(o is MailSerial) ) return false;

			return ((MailSerial)o).m_Serial == m_Serial;
		}

		public static bool operator == ( MailSerial l, MailSerial r )
		{
			return l.m_Serial == r.m_Serial;
		}

		public static bool operator != ( MailSerial l, MailSerial r )
		{
			return l.m_Serial != r.m_Serial;
		}

		public static bool operator > ( MailSerial l, MailSerial r )
		{
			return l.m_Serial > r.m_Serial;
		}

		public static bool operator < ( MailSerial l, MailSerial r )
		{
			return l.m_Serial < r.m_Serial;
		}

		public static bool operator >= ( MailSerial l, MailSerial r )
		{
			return l.m_Serial >= r.m_Serial;
		}

		public static bool operator <= ( MailSerial l, MailSerial r )
		{
			return l.m_Serial <= r.m_Serial;
		}

		public override string ToString()
		{
			return String.Format( "0x{0:X8}", m_Serial );
		}

		public static implicit operator int( MailSerial a )
		{
			return a.m_Serial;
		}

		public static implicit operator MailSerial( int a )
		{
			return new MailSerial( a );
		}

		public static MailSerial Parse( string value, NumberStyles style )
		{
			return new MailSerial( int.Parse( value, style ) );
		}
	}
}