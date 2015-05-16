using System;
using Server.Targeting;
using Server.Network;
using Server.Misc;
using Server.Items;
using Server.Engines.XmlSpawner2;
using VitaNex.Modules.AutoPvP.Battles;

namespace Server.Spells.Third
{
	public class FireballSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Fireball", "Vas Flam",
				203,
				9041,
				Reagent.BlackPearl
			);

		public override SpellCircle Circle { get { return SpellCircle.Third; } }

		public FireballSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		//public override bool DelayedDamage{ get{ return true; } }

		public void Target( Mobile m )
		{
			if ( !Caster.CanSee( m ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if ( CheckHSequence( m ) )
			{
				Mobile source = Caster, target = m;

				SpellHelper.Turn( source, target );

				SpellHelper.CheckReflect( (int)this.Circle, ref source, ref target );

				double damage;

                if (Caster.IsT2A)
                    damage = Utility.RandomMinMax(SpellDamageControllerT2A._FireballDamageMin, SpellDamageControllerT2A._FireballDamageMax);
                else
                    damage = Utility.RandomMinMax(SpellDamageController._FireballDamageMin, SpellDamageController._FireballDamageMax);

                if (CheckResisted(target))
                {
                    if (Caster.IsT2A)
                        damage *= SpellDamageControllerT2A._FireballResistMultiplier;
                    else
                        damage *= SpellDamageController._FireballResistMultiplier;

                    target.SendLocalizedMessage(501783); // You feel yourself resisting magical energy.
                }

				damage *= GetDamageScalar( m );

			    if (SpellDamageController._OldTravellingSpellDelay)
			    {
                    double dist = source.GetDistanceToSqrt( target );
                    SpellHelper.Damage(this, TimeSpan.FromSeconds(0.13 * Math.Min(dist, 6.0)), target, damage);
			    }
			    else
			    {
                    if (Caster.IsT2A)
                        SpellHelper.Damage(this, TimeSpan.FromSeconds(SpellDamageControllerT2A._FireballDamageDelay), target, damage);
                    else
                        SpellHelper.Damage(this, TimeSpan.FromSeconds(SpellDamageController._FireballDamageDelay), target, damage);
			    }

				source.MovingParticles( target, 0x36D4, 7, 0, false, true, 9502, 4019, 0x160 );
				source.PlaySound( 0x44B );
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private FireballSpell m_Owner;

			public InternalTarget(FireballSpell owner)
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
					m_Owner.Target( (Mobile)o );

                if (o is BoWBrazier)
                {
                    from.Mana -= 9;
                    var brazier = o as BoWBrazier;
                    brazier.Calcdamage(from, 2);
                    Effects.SendMovingParticles(from, brazier, 0x36D4, 7, 0, false, true, 9502, 4019, 0x160);
                    Effects.PlaySound(new Point3D(brazier.X, brazier.Y, brazier.Z), brazier.Map, 0x44B);
                }
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}