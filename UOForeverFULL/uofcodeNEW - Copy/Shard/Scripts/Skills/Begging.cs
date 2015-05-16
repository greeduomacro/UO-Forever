using System;
using Server;
using Server.Misc;
using Server.Targeting;
using Server.Items;
using Server.Network;
using Server.Engines.XmlSpawner2;

namespace Server.SkillHandlers
{
	public interface IBegged
	{
		bool CanBeBegged( Mobile from );
		void OnBegged( Mobile beggar );
	}

	public class Begging
	{
		public static void Initialize()
		{
			SkillInfo.Table[(int)SkillName.Begging].Callback = new SkillUseCallback( OnUse );
		}

		public static TimeSpan OnUse( Mobile m )
		{
			m.RevealingAction();

			m.Target = new InternalTarget();
			m.RevealingAction();

			m.SendLocalizedMessage( 500397 ); // To whom do you wish to grovel?

			return TimeSpan.FromSeconds( 10.0 );
		}

		private class InternalTarget : Target
		{
			private bool m_SetSkillTime = true;

			public InternalTarget() :  base ( 12, false, TargetFlags.None )
			{
			}

			protected override void OnTargetFinish( Mobile from )
			{
				if ( m_SetSkillTime )
					from.NextSkillTime = DateTime.UtcNow;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				from.RevealingAction();

                IEntity entity = targeted as IEntity;
                if (XmlScript.HasTrigger(entity, TriggerName.onTargeted) && UberScriptTriggers.Trigger(entity, from, TriggerName.onTargeted, null, null, null, 0, null, SkillName.Begging, from.Skills[SkillName.Begging].Value))
                {
                    return;
                }

				int number = -1;

				if ( targeted is Mobile )
				{
					Mobile targ = (Mobile)targeted;

					if ( targ.Player ) // We can't beg from players
						number = 500398; // Perhaps just asking would work better.
					else if ( !targ.Body.IsHuman ) // Make sure the NPC is human
						number = 500399; // There is little chance of getting money from that!
					else if ( !from.InRange( targ, 2 ) )
					{
						if ( !(targ is IBegged) )
							number = 500403; // That's too far away.  You couldn't beg from it anyway.
						else if ( targ.Female )
							number = 500402; // You are too far away to beg from her.
						else
							number = 500401; // You are too far away to beg from him.
					}
					else if ( from.Mounted ) // If we're on a mount, who would give us money?
						number = 500404; // They seem unwilling to give you any money.
					else if ( targ is IBegged )
					{
						// Face eachother
						from.Direction = from.GetDirectionTo( targ );
						targ.Direction = targ.GetDirectionTo( from );

						from.Animate( 32, 5, 1, true, false, 0 ); // Bow

						new InternalTimer( from, (IBegged)targ ).Start();

						m_SetSkillTime = false;
					}
					else
						number = 500404; // They seem unwilling to give you any money.

				}
				else // Not a Mobile
					number = 500399; // There is little chance of getting money from that!

				if ( number != -1 )
					from.SendLocalizedMessage( number );
			}

			private class InternalTimer : Timer
			{
				private Mobile m_From;
				private IBegged m_Target;

				public InternalTimer( Mobile from, IBegged target ) : base( TimeSpan.FromSeconds( 2.0 ) )
				{
					m_From = from;
					m_Target = target;
					Priority = TimerPriority.OneSecond;
				}

				protected override void OnTick()
				{
					if ( m_Target.CanBeBegged( m_From ) )
						m_Target.OnBegged( m_From );
					else
						m_From.SendLocalizedMessage( 500404 ); // They seem unwilling to give you any money.
				}
			}
		}
	}
}