using System;
using Server;
using Server.Spells;

namespace Server.Items
{
	public abstract class BaseGMJewel : Item
	{
		private AccessLevel m_AccessLevel;

		[CommandProperty( AccessLevel.GameMaster )]
		public AccessLevel AccessLevel{ get{ return m_AccessLevel; } set{ m_AccessLevel = value; } }

		public virtual bool CastHide{ get{ return false; } }
		public virtual bool CastArea{ get{ return false; } }

		public BaseGMJewel( AccessLevel level, int hue, int itemID ) : base( itemID )
		{
			Stackable = false;
			Hue = hue;
			Weight = 1.0;
			Movable = false;
			LootType = LootType.Blessed;
			m_AccessLevel = level;
		}

		public BaseGMJewel( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version

			writer.Write( (int) m_AccessLevel );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();

			switch ( version )
			{
				case 0:
				{
					m_AccessLevel = (AccessLevel)reader.ReadInt();
					break;
				}
			}
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) )
				from.SendLocalizedMessage( 1042001 );
			else if ( from.AccessLevel < m_AccessLevel )
			{
				from.SendMessage( "You touch this and it vanishes without a trace." );
				Delete();
			}
			else if ( CastHide )
				new HideSpell( this, from ).Cast();
			else if ( CastArea )
				new CastAreaSpell( this, from ).Cast();
			else
				HideEffects( from );
		}

		public virtual void HideEffects( Mobile from )
		{
			return;
			//Ball Effects here ~-
		}

		public override bool VerifyMove( Mobile from )
		{
			return ( from.AccessLevel >= m_AccessLevel );
		}

		private class HideSpell : Spell
		{
			private static SpellInfo m_Info = new SpellInfo( "Staff Hide", "", new Type[0] );

			private Mobile m_From;
			private BaseGMJewel m_Jewel;

			public HideSpell( BaseGMJewel jewel, Mobile from ) : base( from, null, m_Info )
			{
				m_From = from;
				m_Jewel = jewel;
			}

			public override bool ClearHandsOnCast{ get{ return false; } }
			public override bool RevealOnCast{ get{ return false; } }
			public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 1.0 ); } }

			public override TimeSpan GetCastRecovery()
			{
				return TimeSpan.Zero;
			}

			public override TimeSpan GetCastDelay()
			{
				return TimeSpan.FromSeconds( 1.0 );
			}

			public override int GetMana()
			{
				return 0;
			}

			public override bool ConsumeReagents()
			{
				return false;
			}

			public override bool CheckFizzle()
			{
				return false;
			}

			public override bool CheckDisturb( DisturbType type, bool checkFirst, bool resistable )
			{
				return false;
			}

			public override void OnDisturb( DisturbType type, bool message )
			{
			}

			public override void OnCast()
			{
				FinishSequence();
				if ( m_Jewel != null && !m_Jewel.Deleted )
					m_Jewel.HideEffects( m_From );
			}
		}

		private class CastAreaSpell : Spell
		{
			private static SpellInfo m_Info = new SpellInfo( "Staff Hide", "", 263, new Type[0] );

			private Mobile m_From;
			private BaseGMJewel m_Jewel;

			public CastAreaSpell( BaseGMJewel jewel, Mobile from ) : base( from, null, m_Info )
			{
				m_From = from;
				m_Jewel = jewel;
			}

			public override bool ClearHandsOnCast{ get{ return false; } }
			public override bool RevealOnCast{ get{ return false; } }
			public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 1.0 ); } }

			public override TimeSpan GetCastRecovery()
			{
				return TimeSpan.Zero;
			}

			public override TimeSpan GetCastDelay()
			{
				return TimeSpan.FromSeconds( 1.0 );
			}

			public override int GetMana()
			{
				return 0;
			}

			public override bool ConsumeReagents()
			{
				return false;
			}

			public override bool CheckFizzle()
			{
				return false;
			}

			public override bool CheckDisturb( DisturbType type, bool checkFirst, bool resistable )
			{
				return false;
			}

			public override void OnDisturb( DisturbType type, bool message )
			{
			}

			public override void OnCast()
			{
				FinishSequence();
				if ( m_Jewel != null &&!m_Jewel.Deleted )
					m_Jewel.HideEffects( m_From );
			}
		}
	}
}