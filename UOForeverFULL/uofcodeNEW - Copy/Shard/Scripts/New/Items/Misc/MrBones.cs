using System;
using Server.Misc;

/* MrBones: RunUO script written by David, Feb 2004.
 * Special release for paid support area only.
 * Please do not distribute, including upload to RunUO.com.
 * Thank you.
 */

namespace Server.Items
{
	[FlipableAttribute( 0x1A01, 0x1A02, 0x1A03, 0x1A04 )]
	public class MrBones : Item
	{
		private bool m_Active;
		private int m_Range;
		private DateTime m_SpeakNext = DateTime.UtcNow;

		private string[] m_Messages =
		{
			"Beware! Beware!",
			"Turn back while you still can!",
			"The living are not welcomed here",
			"Enter if you wish to join the undead",
			"*bones rattle*",
			"*laughter*",
		};

		[CommandProperty( AccessLevel.GameMaster )]
		public string Speak1
		{
			get { return m_Messages[0]; }
			set { m_Messages[0] = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public string Speak2
		{
			get { return m_Messages[1]; }
			set { m_Messages[1] = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public string Speak3
		{
			get { return m_Messages[2]; }
			set { m_Messages[2] = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public string Speak4
		{
			get { return m_Messages[3]; }
			set { m_Messages[3] = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public string Speak5
		{
			get { return m_Messages[4]; }
			set { m_Messages[4] = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public string Speak6
		{
			get { return m_Messages[5]; }
			set { m_Messages[5] = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Active
		{
			get { return m_Active; }
			set { m_Active = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int Range
		{
			get { return m_Range; }
			set { m_Range = value & 0xF; }
		}

		public override bool HandlesOnMovement{ get{ return true; } }
		public override void OnMovement( Mobile m, Point3D oldLocation )
		{
			if( m_Active && m.Player && !m.Hidden && m.InRange( this, m_Range ) )
			{
				if ( DateTime.UtcNow > m_SpeakNext )
				{
					m_SpeakNext = DateTime.UtcNow.AddSeconds( Utility.RandomMinMax( 5, 10 ) );

					string msg = m_Messages[Utility.Random( m_Messages.Length )];
					PublicOverheadMessage( Network.MessageType.Regular, 0x3B2, false, msg );
				}
			}
		}

		[Constructable]
		public MrBones() : base( 0x1A03 )
		{
			m_Active = true;
			m_Range = 4;
			Movable = false;
		}

		public MrBones( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( (int) 0 );
			writer.Write( m_Active );
			writer.Write( m_Range );

			for ( int i = 0; i < m_Messages.Length; ++i )
				writer.Write( m_Messages[i] );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();
			m_Active = reader.ReadBool();
			m_Range = reader.ReadInt();

			for ( int i = 0; i < m_Messages.Length; ++i )
				m_Messages[i] = reader.ReadString();
		}
	}
}