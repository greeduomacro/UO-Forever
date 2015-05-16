using System;
using Server.Targeting;
using Server.Network;
using Server.Items;
using Server.Engines.XmlSpawner2;

namespace Server.Spells.Second
{
	public class HarmSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Harm", "An Mani",
				212,
				9041,
				Reagent.Nightshade,
				Reagent.SpidersSilk
			);

		public override SpellCircle Circle { get { return SpellCircle.Second; } }

		public HarmSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		//public override bool DelayedDamage{ get{ return false; } }


		public override double GetSlayerDamageScalar( Mobile target )
		{
			return 1.0; //This spell isn't affected by slayer spellbooks
		}

		public void Target( Mobile m )
		{
			if ( !Caster.CanSee( m ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if ( CheckHSequence( m ) )
			{
				SpellHelper.Turn( Caster, m );

				Mobile source = Caster, target = m;

				SpellHelper.CheckReflect( (int)this.Circle, source, ref target );

				double damage;

                if (Caster.IsT2A)
                {
                    damage = Utility.RandomMinMax(SpellDamageControllerT2A._HarmDamageMin,
                        SpellDamageControllerT2A._HarmDamageMax);
                }
                else
                {
                    damage = Utility.RandomMinMax(SpellDamageController._HarmDamageMin,
                        SpellDamageController._HarmDamageMax);

                }

                if (CheckResisted(target))
                {
                    if (Caster.IsT2A)
                        damage *= SpellDamageControllerT2A._HarmResistMultiplier;
                    else
                        damage *= SpellDamageController._HarmResistMultiplier;

                    target.SendLocalizedMessage(501783); // You feel yourself resisting magical energy.
                }

				damage *= GetDamageScalar( m );

			    if (!Caster.IsT2A)
			    {
			        //We get our damage numbers from non-reflect
			        if (!m.InRange(Caster, 2))
			            damage *= SpellDamageController._HarmFarDistanceMultiplier; // > 2 tile range
			        else if (!m.InRange(Caster, 1))
			            damage *= SpellDamageController._HarmFarDistanceMultiplier; // 2 tile range
			    }

			    target.FixedParticles( 0x374A, 10, 15, 5013, EffectLayer.Waist );
				target.PlaySound( 0x1F1 );

                SpellHelper.Damage(this, target, damage);
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private HarmSpell m_Owner;

			public InternalTarget(HarmSpell owner)
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
                if ( o is Mobile )
				{
					m_Owner.Target( (Mobile)o );
				}
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}