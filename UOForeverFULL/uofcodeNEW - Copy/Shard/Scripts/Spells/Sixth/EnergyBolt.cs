using System;
using Server.Targeting;
using Server.Network;
using Server.Misc;
using Server.Items;
using Server.Engines.XmlSpawner2;
using VitaNex.Modules.AutoPvP.Battles;

namespace Server.Spells.Sixth
{
	public class EnergyBoltSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Energy Bolt", "Corp Por",
				230,
				9022,
				Reagent.BlackPearl,
				Reagent.Nightshade
			);

		public override SpellCircle Circle { get { return SpellCircle.Sixth; } }

		public EnergyBoltSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
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
                {
                    damage = Utility.RandomMinMax(SpellDamageControllerT2A._EnergyBoltDamageMin,
                        SpellDamageControllerT2A._EnergyBoltDamageMax);
                }
                else
                {
                    damage = Utility.RandomMinMax(SpellDamageController._EnergyBoltDamageMin,
                        SpellDamageController._EnergyBoltDamageMax);

                }

                if (CheckResisted(target))
                {
                    if (Caster.IsT2A)
                        damage *= SpellDamageControllerT2A._EnergyBoltResistMultiplier;
                    else
                        damage *= SpellDamageController._EnergyBoltResistMultiplier;

                    target.SendLocalizedMessage(501783); // You feel yourself resisting magical energy.
                }

				// Scale damage based on evalint and resist
				damage *= GetDamageScalar( m );

                if (SpellDamageController._OldTravellingSpellDelay)
                {
                    double dist = source.GetDistanceToSqrt(target);
                    SpellHelper.Damage(this, TimeSpan.FromSeconds(0.13 * Math.Min(dist, 6.0)), target, damage);
                }
                else
                {
                    if (Caster.IsT2A)
                        SpellHelper.Damage(this, TimeSpan.FromSeconds(SpellDamageControllerT2A._EnergyBoltDamageDelay), target, damage);
                    else
                        SpellHelper.Damage(this, TimeSpan.FromSeconds(SpellDamageController._EnergyBoltDamageDelay), target, damage);
                }

				// Do the effects
				source.MovingParticles( m, 0x379F, 7, 0, false, true, 3043, 4043, 0x211 );
				source.PlaySound( 0x20A );
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private EnergyBoltSpell m_Owner;

			public InternalTarget(EnergyBoltSpell owner)
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
                    from.Mana -= 20;
                    var brazier = o as BoWBrazier;
                    brazier.Calcdamage(from, 4);
                    Effects.SendMovingParticles(from, brazier, 0x379F, 7, 0, false, true, 3043, 4043, 0x211);
                    Effects.PlaySound(new Point3D(brazier.X, brazier.Y, brazier.Z), brazier.Map, 0x20A);
                }
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}