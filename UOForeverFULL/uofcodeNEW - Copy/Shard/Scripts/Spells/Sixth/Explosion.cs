using System;
using Server.Targeting;
using Server.Network;
using Server.Misc;
using Server.Items;
using Server.Engines.XmlSpawner2;
using VitaNex.Modules.AutoPvP.Battles;

namespace Server.Spells.Sixth
{
	public class ExplosionSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Explosion", "Vas Ort Flam",
				230,
				9041,
				Reagent.Bloodmoss,
				Reagent.MandrakeRoot
			);

		public override SpellCircle Circle { get{ return SpellCircle.Sixth; } }

		public ExplosionSpell( Mobile caster, Item scroll )
			: base( caster, scroll, m_Info )
		{
		}

		//public override bool DelayedDamageStacking{ get{ return !Core.AOS; } }
        public override bool DelayedDamageStacking { get { return SpellDamageController._CanStackExplosion; } }

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		public override bool DelayedDamage{ get{ return true; } }

		public void Target( Mobile m )
		{
			if ( !Caster.CanSee( m ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if ( Caster.CanBeHarmful( m ) && CheckSequence() )
			{
				Mobile source = Caster, target = m;

				SpellHelper.Turn( source, target );

				SpellHelper.CheckReflect( (int) this.Circle, ref source, ref target );

				InternalTimer t = new InternalTimer( this, target);
				t.Start();
			}

			FinishSequence();
		}

		private class InternalTimer : Timer
		{
			private MagerySpell m_Spell;
			private Mobile m_Target;

			public InternalTimer( MagerySpell spell, Mobile target)
                : base(TimeSpan.FromSeconds( SpellDamageController._ExplosionBaseDelay + (Utility.Random(SpellDamageController._ExplosionDelayRandomTenths) / 10.0)))
			{
				m_Spell = spell;
				m_Target = target;

				if ( m_Spell != null && m_Spell.DelayedDamage && !m_Spell.DelayedDamageStacking )
					m_Spell.StartDelayedDamageContext( spell.Caster, this );

				Priority = TimerPriority.FiftyMS;
			}

			protected override void OnTick()
			{
                if (m_Spell.Caster.HarmfulCheck(m_Target))
				{
					double damage;
                    if (m_Target.IsT2A)
                    {
                        damage = Utility.RandomMinMax(SpellDamageControllerT2A._ExplosionDamageMin,
                            SpellDamageControllerT2A._ExplosionDamageMax);
                    }
                    else
                    {
                        damage = Utility.RandomMinMax(SpellDamageController._ExplosionDamageMin,
                            SpellDamageController._ExplosionDamageMax);

                    }

                    if (m_Spell.CheckResisted(m_Target))
                    {
                        if (m_Target.IsT2A)
                            damage *= SpellDamageControllerT2A._ExplosionResistMultiplier;
                        else
                            damage *= SpellDamageController._ExplosionResistMultiplier;

                        m_Target.SendLocalizedMessage(501783); // You feel yourself resisting magical energy.
                    }

					damage *= m_Spell.GetDamageScalar( m_Target );
					

                    m_Target.FixedParticles(0x36BD, 20, 10, 5044, EffectLayer.Head);
                    m_Target.PlaySound(0x307);

					SpellHelper.Damage( m_Spell, TimeSpan.Zero, m_Target, damage);
				}
			}
		}

		private class InternalTarget : Target
		{
			private ExplosionSpell m_Owner;

			public InternalTarget( ExplosionSpell owner )
				: base(owner.Caster.EraML ? 10 : 12, false, TargetFlags.Harmful)
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
                IEntity entity = o as IEntity; if (XmlScript.HasTrigger(entity, TriggerName.onTargeted) && UberScriptTriggers.Trigger(entity, from, TriggerName.onTargeted, null, null, m_Owner))
                {
                    return;
                }
                if (o is Mobile)
                    m_Owner.Target((Mobile)o);

                if (o is BoWBrazier)
                {
                    from.Mana -= 20;
                    var brazier = o as BoWBrazier;
                    brazier.Calcdamage(from, 4);
                    Effects.SendLocationEffect(new Point3D(brazier.X, brazier.Y, brazier.Z), brazier.Map, 0x36BD, 10, 0, 0);
                    Effects.PlaySound(new Point3D(brazier.X, brazier.Y, brazier.Z), brazier.Map, 0x307);
                }
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}