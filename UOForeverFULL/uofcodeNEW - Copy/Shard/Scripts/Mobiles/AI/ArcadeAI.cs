using System;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.Network;
using System.Collections.Generic;

//
// This is a first simple AI
//
//

namespace Server.Mobiles
{
	public class ArcadeAI : BaseAI
	{
		public ArcadeAI(BaseCreature m) : base (m)
		{
		}

		public override bool DoActionWander()
		{
			//m_Mobile.DebugSay( "I have no combatant" );

            if (m_Mobile.ForceWaypoint && (m_Mobile.CurrentWayPoint != null || m_Mobile.TargetLocation != null))
            {
                m_Mobile.CurrentSpeed = m_Mobile.ActiveSpeed;
                base.DoActionWander();
            }
            else if (AcquireFocusMob(m_Mobile.RangePerception, m_Mobile.FightMode, m_Mobile.IsScaryToPets, false, true))
			{
				//if ( m_Mobile.Debug )
					m_Mobile.DebugSay( "I have detected {0}, attacking", m_Mobile.FocusMob.Name );

				m_Mobile.Combatant = m_Mobile.FocusMob;
				Action = ActionType.Combat;
			}
			else
			{
                if (CheckHerding())
                {
                    m_Mobile.DebugSay("Praise the shepherd!");
                }
                else if (m_Mobile.CurrentWayPoint != null)
                {
                    WayPoint point = m_Mobile.CurrentWayPoint;
                    // mod: make it so they only have to get within 1 square of the waypoint
                    if ((Math.Abs(point.X - m_Mobile.Location.X) > 1.0 || Math.Abs(point.Y - m_Mobile.Location.Y) > 1.0) &&
                        point.Map == m_Mobile.Map && point.Parent == null && !point.Deleted)
                    {
                        if (m_Mobile is HydraMotM && !m_Mobile.CantWalk || !(m_Mobile is HydraMotM))
                        {
                            m_Mobile.DebugSay("I will move towards my waypoint.");
                            DoMove(m_Mobile.GetDirectionTo(m_Mobile.CurrentWayPoint));
                        }
                    }
                    else if (OnAtWayPoint())
                    {
                        m_Mobile.DebugSay("I will go to the next waypoint");
                        m_Mobile.CurrentWayPoint = point.NextPoint;
                        if (point.NextPoint != null && point.NextPoint.Deleted)
                        {
                            m_Mobile.CurrentWayPoint = point.NextPoint = point.NextPoint.NextPoint;
                        }
                    }
                }
                else if (m_Mobile.IsAnimatedDead)
                {
                    // animated dead follow their master
                    Mobile master = m_Mobile.SummonMaster;

                    if (master != null && master.Map == m_Mobile.Map && master.InRange(m_Mobile, m_Mobile.RangePerception))
                    {
                        MoveTo(master, false, 1);
                    }
                    else
                    {
                        WalkRandomInHome(2, 2, 1);
                    }
                }
                else if (CheckMove())
                {
                    if (!m_Mobile.CheckIdle())
                    {
                        WalkRandomInHome(2, 2, 1);
                    }
                }

                if (m_Mobile.Combatant != null && !m_Mobile.Combatant.Deleted && m_Mobile.Combatant.Alive &&
                    !m_Mobile.Combatant.IsDeadBondedPet)
                {
                    if (m_Mobile is HydraMotM && !m_Mobile.CantWalk || !(m_Mobile is HydraMotM))
                    {
                        m_Mobile.Direction = m_Mobile.GetDirectionTo(m_Mobile.Combatant);
                    }
                }

                return true;
			}

			return true;
		}

        public override bool DoMove(Direction d, bool badStateOk)
        {
            MoveResult res = DoMoveImpl(d | Direction.Running);

            return (res == MoveResult.Success || res == MoveResult.SuccessAutoTurn || (badStateOk && res == MoveResult.BadState));
        }

        public override bool DoOrderNone()
        {
            m_Mobile.DebugSay("I have no order");

            WalkRandomInHome(3, 2, 1);

            if (m_Mobile.Combatant != null && !m_Mobile.Combatant.Deleted && m_Mobile.Combatant.Alive &&
                !m_Mobile.Combatant.IsDeadBondedPet)
            {
                m_Mobile.Warmode = true;
                if (m_Mobile is HydraMotM && !m_Mobile.CantWalk || !(m_Mobile is HydraMotM))
                {
                    m_Mobile.Direction = m_Mobile.GetDirectionTo(m_Mobile.Combatant);
                }
            }
            else
            {
                m_Mobile.Warmode = false;
            }

            return true;
        }

		public override bool DoActionCombat()
		{
			Mobile combatant = m_Mobile.Combatant;

            if ( m_Mobile.ForceWaypoint && (m_Mobile.CurrentWayPoint != null || m_Mobile.TargetLocation != null ))
            {
                m_Mobile.DebugSay("I'm in combat but must go to my waypoint!");
                Action = ActionType.Wander;
                return true;
            }

			if ( combatant == null || combatant.Deleted || combatant.Map != m_Mobile.Map || !combatant.Alive || combatant.IsDeadBondedPet )
			{
				m_Mobile.DebugSay( "My combatant is gone, so my guard is up" );

				Action = ActionType.Guard;
				return true;
			}

			if ( !m_Mobile.InRange( combatant, m_Mobile.RangePerception ) )
			{
				// They are somewhat far away, can we find something else?

                if (AcquireFocusMob(m_Mobile.RangePerception, m_Mobile.FightMode, m_Mobile.IsScaryToPets, false, true))
				{
					m_Mobile.Combatant = m_Mobile.FocusMob;
					m_Mobile.FocusMob = null;
				}
				else if ( !m_Mobile.InRange( combatant, m_Mobile.RangePerception * 3 ) )
				{
					m_Mobile.Combatant = null;
				}

				combatant = m_Mobile.Combatant;

				if ( combatant == null )
				{
					m_Mobile.DebugSay( "My combatant has fled, so I am on guard" );
					Action = ActionType.Guard;

					return true;
				}
			}

			/*if ( !m_Mobile.InLOS( combatant ) )
			{
				if ( AcquireFocusMob( m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true ) )
				{
					m_Mobile.Combatant = combatant = m_Mobile.FocusMob;
					m_Mobile.FocusMob = null;
				}
			}*/

			if ( MoveTo( combatant, true, m_Mobile.RangeFight ) )
			{
                // if they are 1 square away, then try to walk around them sometimes
                if (m_Mobile.InRange(combatant, 1)) 
                {
                    if (Utility.RandomDouble() < 0.1) // 10% chance
                    {
                        Direction toCombatant = m_Mobile.GetDirectionTo(combatant);
                        List<Direction> possibilities = new List<Direction>();
                        if (toCombatant == Direction.North)
                        {
                            possibilities.Add(Direction.West);
                            possibilities.Add(Direction.Right);
                            possibilities.Add(Direction.East);
                            possibilities.Add(Direction.Up);
                        }
                        else if (toCombatant == Direction.Right)
                        {
                            possibilities.Add(Direction.North);
                            possibilities.Add(Direction.East);
                        }
                        else if (toCombatant == Direction.East)
                        {
                            possibilities.Add(Direction.North);
                            possibilities.Add(Direction.Down);
                            possibilities.Add(Direction.South);
                            possibilities.Add(Direction.Right);
                        }
                        else if (toCombatant == Direction.Down)
                        {
                            possibilities.Add(Direction.East);
                            possibilities.Add(Direction.South);
                        }
                        else if (toCombatant == Direction.South)
                        {
                            possibilities.Add(Direction.East);
                            possibilities.Add(Direction.Left);
                            possibilities.Add(Direction.West);
                            possibilities.Add(Direction.Down);
                        }
                        else if (toCombatant == Direction.Left)
                        {
                            possibilities.Add(Direction.West);
                            possibilities.Add(Direction.South);
                        }
                        else if (toCombatant == Direction.West)
                        {
                            possibilities.Add(Direction.South);
                            possibilities.Add(Direction.Up);
                            possibilities.Add(Direction.North);
                            possibilities.Add(Direction.Left);
                        }
                        else if (toCombatant == Direction.Up)
                        {
                            possibilities.Add(Direction.North);
                            possibilities.Add(Direction.West);
                        }
                        else
                        {
                            possibilities.Add(Direction.North);
                            possibilities.Add(Direction.East);
                            possibilities.Add(Direction.West);
                            possibilities.Add(Direction.South);
                        }

                        if (m_Mobile is HydraMotM && !m_Mobile.CantWalk || !(m_Mobile is HydraMotM))
                        {
                            Direction toGo = possibilities[Utility.Random(possibilities.Count)];
                            m_Mobile.Direction = toGo;
                            m_Mobile.Move(toGo | Direction.Running);
                            m_Mobile.Direction = m_Mobile.GetDirectionTo(combatant);
                        }
                    }
                }
			}
            else if (AcquireFocusMob(m_Mobile.RangePerception, m_Mobile.FightMode, m_Mobile.IsScaryToPets, false, true))
			{
				//if ( m_Mobile.Debug )
					m_Mobile.DebugSay( "My move is blocked, so I am going to attack {0}", m_Mobile.FocusMob.Name );

				m_Mobile.Combatant = m_Mobile.FocusMob;
				Action = ActionType.Combat;

				return true;
			}
			else if ( m_Mobile.GetDistanceToSqrt( combatant ) > m_Mobile.RangePerception + 1 )
			{
				//if ( m_Mobile.Debug )
					m_Mobile.DebugSay( "I cannot find {0}, so my guard is up", combatant.Name );

				Action = ActionType.Guard;

				return true;
			}
			else
			{
				//if ( m_Mobile.Debug )
					m_Mobile.DebugSay( "I should be closer to {0}", combatant.Name );
			}

			if ( !m_Mobile.Controlled && !m_Mobile.Summoned && m_Mobile.CanFlee )
			{
				if ( m_Mobile.Hits < m_Mobile.HitsMax * 20/100 )
				{
					// We are low on health, should we flee?

					bool flee = false;

					if ( m_Mobile.Hits < combatant.Hits )
					{
						// We are more hurt than them

						int diff = combatant.Hits - m_Mobile.Hits;

						flee = ( Utility.Random( 0, 100 ) < (10 + diff) ); // (10 + diff)% chance to flee
					}
					else
					{
						flee = Utility.Random( 0, 100 ) < 10; // 10% chance to flee
					}

					if ( flee )
					{
						if ( m_Mobile.Debug )
							m_Mobile.DebugSay( "I am going to flee from {0}", combatant.Name );

						Action = ActionType.Flee;
					}
				}
			}

			return true;
		}

		public override bool DoActionGuard()
		{
            if (m_Mobile.ForceWaypoint && (m_Mobile.CurrentWayPoint != null || m_Mobile.TargetLocation != null))
            {
                m_Mobile.DebugSay("I'm in guard mode but must go to my waypoint!");
                Action = ActionType.Wander;
                return true;
            }
            else if (AcquireFocusMob(m_Mobile.RangePerception, m_Mobile.FightMode, m_Mobile.IsScaryToPets, false, true))
			{
				//if ( m_Mobile.Debug )
					m_Mobile.DebugSay( "I have detected {0}, attacking", m_Mobile.FocusMob.Name );

				m_Mobile.Combatant = m_Mobile.FocusMob;
				Action = ActionType.Combat;
			}
			else
			{
				base.DoActionGuard();
			}

			return true;
		}

		public override bool DoActionFlee()
		{
            if (m_Mobile.ForceWaypoint && (m_Mobile.CurrentWayPoint != null || m_Mobile.TargetLocation != null))
            {
                m_Mobile.DebugSay("I'm in flee mode but must go to my waypoint!");
                Action = ActionType.Wander;
                return true;
            }
            else if ( m_Mobile.Hits > m_Mobile.HitsMax/2 )
			{
				m_Mobile.DebugSay( "I am stronger now, so I will continue fighting" );
				Action = ActionType.Combat;
			}
			else
			{
				m_Mobile.FocusMob = m_Mobile.Combatant;
				base.DoActionFlee();
			}

			return true;
		}
	}
}