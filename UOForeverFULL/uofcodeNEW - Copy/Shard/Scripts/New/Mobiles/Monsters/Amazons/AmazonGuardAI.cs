using System;
using System.Collections;
using Server.Targeting;
using Server.Network;
using Server.Misc;
using Server.Items;

namespace Server.Mobiles
{
	public class AmazonGuardAI : BaseAI
	{
		private DateTime m_NextCastTime;

		public AmazonGuardAI(BaseCreature m) : base (m)
		{
		}

		public override bool DoActionWander()
		{
            if (AcquireFocusMob(m_Mobile.RangePerception, m_Mobile.FightMode, m_Mobile.IsScaryToPets, false, true))
			{
				m_Mobile.DebugSay( "I have detected {0}, attacking", m_Mobile.FocusMob.Name );
				m_Mobile.Combatant = m_Mobile.FocusMob;
				Action = ActionType.Combat;
			}
			else
			{
				base.DoActionWander();
			}

			return true;
		}

		public override bool DoActionCombat()
		{
			Mobile combatant = m_Mobile.Combatant;

			if ( combatant == null || combatant.Deleted || combatant.Map != m_Mobile.Map )
			{
				m_Mobile.DebugSay( "My combatant is gone, so my guard is up" );

				Action = ActionType.Guard;
				m_NextCastTime = DateTime.UtcNow;

				return true;
			}

			if ( !m_Mobile.InRange( combatant, 25 ) )
			{
				m_Mobile.FocusMob = null;
			}
			else if ( !m_Mobile.CanSee( combatant ) )
			{
				if ( !m_Mobile.UseSkill( SkillName.DetectHidden ) && Utility.Random( 10 ) == 0 )
					m_Mobile.Say( "Reveal!" );
			}
			else if ( DateTime.UtcNow >= m_NextCastTime && !m_Mobile.InRange( combatant, 8 ) && m_Mobile.InLOS( combatant ) && !combatant.Map.CanSpawnMobile( combatant.X, combatant.Y, combatant.Z ) && Utility.Random( 4 ) == 0 )
			{
				TeleportTo( combatant );
			}
			else if ( !m_Mobile.InRange( combatant, 3 ) )
			{
				if ( DateTime.UtcNow >= m_NextCastTime && !m_Mobile.Move( m_Mobile.GetDirectionTo( combatant ) | Direction.Running ) && m_Mobile.InLOS( combatant ) && !combatant.Map.CanSpawnMobile( combatant.X, combatant.Y, combatant.Z ) )
					TeleportTo( combatant );
			}
			else if ( MoveTo( combatant, true, m_Mobile.RangeFight ) )
			{
				m_Mobile.Direction = m_Mobile.GetDirectionTo( combatant );
			}
            else if (AcquireFocusMob(m_Mobile.RangePerception, m_Mobile.FightMode, m_Mobile.IsScaryToPets, false, true))
			{
				m_Mobile.DebugSay( "My move is blocked, so I am going to attack {0}", m_Mobile.FocusMob.Name );

				m_Mobile.Combatant = m_Mobile.FocusMob;
				Action = ActionType.Combat;
				m_NextCastTime = DateTime.UtcNow;
				return true;
			}
			else if ( m_Mobile.GetDistanceToSqrt( combatant ) > m_Mobile.RangePerception + 1 )
			{
				m_Mobile.DebugSay( "I cannot find {0}, so my guard is up", combatant.Name );

				Action = ActionType.Guard;
				m_NextCastTime = DateTime.UtcNow;
				return true;
			}
			else
			{
				m_Mobile.DebugSay( "I should be closer to {0}", combatant.Name );
			}

			if ( !m_Mobile.Controlled && !m_Mobile.Summoned )
			{
				if ( m_Mobile.Hits < m_Mobile.HitsMax * 20/100 )
				{
					// We are low on health, should we flee?

					bool flee = false;

					if ( m_Mobile.Hits < combatant.Hits )
					{
						// We are more hurt than them

						int diff = combatant.Hits - m_Mobile.Hits;

						flee = (10 + diff) > Utility.Random( 100 ); // (10 + diff)% chance to flee
					}
					else
					{
						flee = 10 > Utility.Random( 100 ); // 10% chance to flee
					}

					if ( flee )
					{
						m_Mobile.DebugSay( "I am going to flee from {0}", combatant.Name );
						Action = ActionType.Flee;
						m_NextCastTime = DateTime.UtcNow;
					}
				}
			}

			return true;
		}

		private void TeleportTo( Mobile target )
		{
			Point3D from = m_Mobile.Location;
			Point3D to = target.Location;
			m_Mobile.Location = to;
			Effects.SendLocationParticles( EffectItem.Create( from, m_Mobile.Map, EffectItem.DefaultDuration ), 0x3728, 10, 10, 2023 );
			Effects.SendLocationParticles( EffectItem.Create(   to, m_Mobile.Map, EffectItem.DefaultDuration ), 0x3728, 10, 10, 5023 );
			m_Mobile.PlaySound( 0x1FE );
			m_NextCastTime = DateTime.UtcNow + TimeSpan.FromSeconds( 3.0 );
		}

		public override bool DoActionGuard()
		{
            if (AcquireFocusMob(m_Mobile.RangePerception, m_Mobile.FightMode, m_Mobile.IsScaryToPets, false, true))
			{
				m_Mobile.DebugSay( "I have detected {0}, attacking", m_Mobile.FocusMob.Name );
				m_Mobile.Combatant = m_Mobile.FocusMob;
				Action = ActionType.Combat;
				m_NextCastTime = DateTime.UtcNow;
			}
			else
			{
				base.DoActionGuard();
			}

			return true;
		}

		public override bool DoActionFlee()
		{
			if ( m_Mobile.Hits > m_Mobile.HitsMax/2 )
			{
				m_Mobile.DebugSay( "I am stronger now, so I will continue fighting" );
				Action = ActionType.Combat;
				m_NextCastTime = DateTime.UtcNow;
			}
			else
			{
				m_Mobile.FocusMob = m_Mobile.Combatant;
				m_NextCastTime = DateTime.UtcNow;
				base.DoActionFlee();
			}

			return true;
		}
	}
}