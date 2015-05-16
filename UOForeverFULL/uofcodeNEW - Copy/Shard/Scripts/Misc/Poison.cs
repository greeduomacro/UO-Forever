using System;
using Server;
using Server.Items;
using Server.Network;
using Server.Mobiles;
using Server.Spells;
using Server.Engines.XmlSpawner2;

namespace Server
{
	public class PoisonImpl : Poison
	{
		[CallPriority( 10 )]
		public static void Configure()
		{
			/*if ( Core.AOS )
			{
				Register( new PoisonImpl( "Lesser",		0,  4, 16,  7.5, 3.0, 2.25, 10, 4 ) );
				Register( new PoisonImpl( "Regular",	1,  8, 18, 10.0, 3.0, 3.25, 10, 3 ) );
				Register( new PoisonImpl( "Greater",	2, 12, 20, 15.0, 3.0, 4.25, 10, 2 ) );
				Register( new PoisonImpl( "Deadly",		3, 16, 30, 30.0, 3.0, 5.25, 15, 2 ) );
				Register( new PoisonImpl( "Lethal",		4, 20, 50, 35.0, 3.0, 5.25, 20, 2 ) );
			}
			else*/
			{
/*
				Register( new PoisonImpl( "Lesser",		0, 4, 26,  2.500, 3.5, 3.0, 10, 2 ) );
				Register( new PoisonImpl( "Regular",	1, 5, 26,  3.125, 3.5, 3.0, 10, 2 ) );
				Register( new PoisonImpl( "Greater",	2, 6, 26,  6.250, 3.5, 3.0, 10, 2 ) );
				Register( new PoisonImpl( "Deadly",		3, 7, 26, 12.500, 3.5, 4.0, 10, 2 ) );
				Register( new PoisonImpl( "Lethal",		4, 9, 26, 25.000, 3.5, 5.0, 10, 2 ) );
*/
				Register( new PoisonImpl( "Lesser",		1015017,	0,  2,  26,  1.750, 3.25, 2.75, 10, 2 ) );
				Register( new PoisonImpl( "Regular",	1015018,	1,  5,  26,  3.125, 3.25, 2.75, 10, 2 ) );
				Register( new PoisonImpl( "Greater",	1015019,	2,  7,  26,  6.250, 4.00, 3.50, 10, 2 ) );

				Register( new PoisonImpl( "Deadly",		1015020,	3,  6,  26, 12.500, 5.00, 4.75, 10, 2 ) );
				
				Register( new PoisonImpl( "Lethal",		1113465,	4, 10,  26, 25.000, 5.00, 5.00, 10, 2 ) );
			}
				Register( new PoisonImpl( "Fatal", 		0,			5, 12,  26, 35.000, 4.0, 4.0, 20, 2 ) );
		}

		public static Poison IncreaseLevel( Poison oldPoison )
		{
			Poison newPoison = ( oldPoison == null ? null : GetPoison( oldPoison.Level + 1 ) );

			return ( newPoison == null ? oldPoison : newPoison );
		}

		// Info
		private string m_Name;
		private int m_Level;
		private int m_LabelNumber;

		// Damage
		private int m_Minimum, m_Maximum;
		private double m_Scalar;

		// Timers
		private TimeSpan m_Delay;
		private TimeSpan m_Interval;
		private int m_Count, m_MessageInterval;

		public PoisonImpl( string name, int labelnumber, int level, int min, int max, double percent, double delay, double interval, int count, int messageInterval )
		{
			m_Name = name;
			m_LabelNumber = labelnumber;
			m_Level = level;
			m_Minimum = min;
			m_Maximum = max;
			m_Scalar = percent * 0.01;
			m_Delay = TimeSpan.FromSeconds( delay );
			m_Interval = TimeSpan.FromSeconds( interval );
			m_Count = count;
			m_MessageInterval = messageInterval;
		}

		public override string Name{ get{ return m_Name; } }
		public override int Level{ get{ return m_Level; } }
		public override int LabelNumber{ get{ return m_LabelNumber; } }

		public class PoisonTimer : Timer
		{
			private PoisonImpl m_Poison;
			private Mobile m_Mobile;
			private Mobile m_From;
			private int m_LastDamage;
			private int m_Index;

			public Mobile From{ get{ return m_From; } set{ m_From = value; } }

			public PoisonTimer( Mobile m, PoisonImpl p ) : base( p.m_Delay, p.m_Interval )
			{
				m_From = m;
				m_Mobile = m;
				m_Poison = p;
			}

			protected override void OnTick()
			{
				if ( (m_Poison.Level < 3 && OrangePetals.UnderEffect( m_Mobile )) )
				{
					if ( m_Mobile.CurePoison( m_Mobile ) )
					{
						m_Mobile.LocalOverheadMessage( MessageType.Emote, 0x3F, true,
							"* You feel yourself resisting the effects of the poison *" );

						m_Mobile.NonlocalOverheadMessage( MessageType.Emote, 0x3F, true,
							String.Format( "* {0} seems resistant to the poison *", m_Mobile.Name ) );

						Stop();
						return;
					}
				}

				if ( m_Index++ == m_Poison.m_Count )
				{
					m_Mobile.SendLocalizedMessage( 502136 ); // The poison seems to have worn off.
					m_Mobile.Poison = null;

					Stop();
					return;
				}

				if ( Interval == m_Poison.m_Delay )
					Interval = m_Poison.m_Interval;
				else //if ( Delay == m_Poison.m_Interval )
					Interval = m_Poison.m_Delay;

				int damage;

				if ( m_LastDamage != 0 && Utility.RandomBool() )
				{
					damage = m_LastDamage;
				}
				else
				{
					damage = 1 + (int)(m_Mobile.Hits * m_Poison.m_Scalar);

					if ( damage < m_Poison.m_Minimum )
						damage = m_Poison.m_Minimum;
					else if ( damage > m_Poison.m_Maximum )
						damage = m_Poison.m_Maximum;

					m_LastDamage = damage;
				}

				if ( m_From != null )
					m_From.DoHarmful( m_Mobile, true );

                if (XmlScript.HasTrigger(m_Mobile, TriggerName.onPoisonTick) && UberScriptTriggers.Trigger(m_Mobile, m_From, TriggerName.onPoisonTick, null, null, null, damage))
                    return;

                m_Mobile.Damage(damage, m_From);

				//if ( Utility.RandomBool() ) // OSI: randomly revealed between first and third damage tick, guessing 60% chance
				//		m_Mobile.RevealingAction();

				if ( (m_Index % m_Poison.m_MessageInterval) == 0 )
					m_Mobile.OnPoisoned( m_From, m_Poison, m_Poison );
			}
		}

		public override Timer ConstructTimer( Mobile m )
		{
			return new PoisonTimer( m, this );
		}
	}
}