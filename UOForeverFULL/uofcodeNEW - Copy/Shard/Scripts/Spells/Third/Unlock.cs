using System;
using Server.Targeting;
using Server.Network;
using Server.Items;
using Server.Engines.XmlSpawner2;

namespace Server.Spells.Third
{
	public class UnlockSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Unlock Spell", "Ex Por",
				215,
				9001,
				Reagent.Bloodmoss,
				Reagent.SulfurousAsh
			);

		public override SpellCircle Circle { get { return SpellCircle.Third; } }

		public UnlockSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		private class InternalTarget : Target
		{
			private UnlockSpell m_Owner;

			public InternalTarget(UnlockSpell owner)
				: base(owner.Caster.EraML ? 10 : 12, false, TargetFlags.None)
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
                IEntity entity = o as IEntity; if (XmlScript.HasTrigger(entity, TriggerName.onTargeted) && UberScriptTriggers.Trigger(entity, from, TriggerName.onTargeted, null, null, m_Owner))
                {
                    return;
                }
                IPoint3D loc = o as IPoint3D;

				if ( loc == null )
					return;

				if ( m_Owner.CheckSequence() )
				{
					SpellHelper.Turn( from, o );

					if ( o is Mobile )
						from.LocalOverheadMessage( MessageType.Regular, 0x3B2, 503101 ); // That did not need to be unlocked.
					else if ( !( o is LockableContainer ) )
						from.SendLocalizedMessage( 501666 ); // You can't unlock that!
					else
					{
						LockableContainer cont = (LockableContainer)o;

						if ( Multis.BaseHouse.CheckSecured( cont ) )
							from.SendLocalizedMessage( 503098 ); // You cannot cast this on a secure item.
						else if ( !cont.Locked )
							from.LocalOverheadMessage( MessageType.Regular, 0x3B2, 503101 ); // That did not need to be unlocked.
						else if ( cont.LockLevel == 0 )
							from.SendLocalizedMessage( 501666 ); // You can't unlock that!
						else
						{
							Effects.SendLocationParticles( EffectItem.Create( new Point3D( loc ), from.Map, EffectItem.DefaultDuration ), 0x376A, 9, 32, 5024 );
							Effects.PlaySound( loc, from.Map, 0x1FF );

							int level = (int)(from.Skills[SkillName.Magery].Value * 0.55) - 4; // Note from Alan: I see why somebody changed this from below, so you can't magic unlock a GM tinker chest
                                                                                                // but it is also preventing you from picking that lock which is pretty silly
                            //int level = (int)(from.Skills[SkillName.Magery].Value * 0.8) - 4; // <-- this is RunUO default

                            if (cont.LockLevel == -255) // it is magic locked so always succeed at unlocking
                            {
                                cont.Locked = false;
                                cont.LockLevel = cont.RequiredSkill - 10;
                            }
                            else if ( level >= cont.RequiredSkill && !(cont is TreasureMapChest && ((TreasureMapChest)cont).Level > 2) ) // this is basically a magery skill vs lock level check
							{
								cont.Locked = false;
							}
							else
								from.LocalOverheadMessage( MessageType.Regular, 0x3B2, 503099 ); // My spell does not seem to have an effect on that lock.
						}
					}

					m_Owner.FinishSequence();
				}
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}