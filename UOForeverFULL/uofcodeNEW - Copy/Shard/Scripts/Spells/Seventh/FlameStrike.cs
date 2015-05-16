using System;
using Server.Targeting;
using Server.Network;
using Server.Items;
using Server.Engines.XmlSpawner2;
using VitaNex.Modules.AutoPvP.Battles;

namespace Server.Spells.Seventh
{
	public class FlameStrikeSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Flame Strike", "Kal Vas Flam",
				245,
				9042,
				Reagent.SpidersSilk,
				Reagent.SulfurousAsh
			);

		public override SpellCircle Circle { get { return SpellCircle.Seventh; } }

		public FlameStrikeSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

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
			else if ( CheckHSequence( m ) )
			{
				Mobile source = Caster, target = m;

				SpellHelper.Turn( source, target );

				SpellHelper.CheckReflect( (int)this.Circle, ref source, ref target );

				double damage;

			    if (Caster.IsT2A)
			    {
                    damage = Utility.RandomMinMax(SpellDamageControllerT2A._FlameStrikeDamageMin,
                        SpellDamageControllerT2A._FlameStrikeDamageMax);
			    }
                else
			    {
			        damage = Utility.RandomMinMax(SpellDamageController._FlameStrikeDamageMin,
			            SpellDamageController._FlameStrikeDamageMax);
			        
			    }

			    if (CheckResisted(target))
                {
                    if (Caster.IsT2A)
                        damage *= SpellDamageControllerT2A._FlameStrikeResistMultiplier;
                    else
                        damage *= SpellDamageController._FlameStrikeResistMultiplier;

                    target.SendLocalizedMessage(501783); // You feel yourself resisting magical energy.
                }

				damage *= GetDamageScalar( m );

				target.FixedParticles( 0x3709, 10, 30, 5052, EffectLayer.LeftFoot );
				target.PlaySound( 0x208 );

                if (Caster.IsT2A)
                    SpellHelper.Damage(this, TimeSpan.FromSeconds(SpellDamageControllerT2A._FlameStrikeDelaySeconds), target, damage);
                else
                    SpellHelper.Damage(this, TimeSpan.FromSeconds(SpellDamageController._FlameStrikeDelaySeconds), target, damage);
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private FlameStrikeSpell m_Owner;

			public InternalTarget(FlameStrikeSpell owner)
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
                    from.Mana -= 40;
                    var brazier = o as BoWBrazier;
                    brazier.Calcdamage(from, 5);
                    Effects.SendLocationEffect(new Point3D(brazier.X, brazier.Y, brazier.Z), brazier.Map, 0x3709, 15, 0, 0);
                    Effects.PlaySound(new Point3D(brazier.X, brazier.Y, brazier.Z), brazier.Map, 0x208);
                }
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}