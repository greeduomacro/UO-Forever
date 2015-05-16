#region References
using System;

using Server.Engines.XmlSpawner2;
using Server.Items;
using Server.Targeting;
#endregion

namespace Server.Spells.First
{
	public class MagicArrowSpell : MagerySpell
	{
		private static readonly SpellInfo m_Info = new SpellInfo(
			"Magic Arrow", "In Por Ylem", 212, 9041, Reagent.SulfurousAsh);

		public override SpellCircle Circle { get { return SpellCircle.First; } }

		public MagicArrowSpell(Mobile caster, Item scroll)
			: base(caster, scroll, m_Info)
		{ }

		public override bool DelayedDamageStacking { get { return Caster != null && !Caster.EraAOS; } }

		public override void OnCast()
		{
			Caster.Target = new InternalTarget(this);
		}

		public override bool DelayedDamage { get { return true; } }

		public void Target(Mobile m)
		{
			if (!Caster.CanSee(m))
			{
				Caster.SendLocalizedMessage(500237); // Target can not be seen.
			}
			else if (CheckHSequence(m))
			{
				Mobile source = Caster, target = m;

				SpellHelper.Turn(source, m);

				SpellHelper.CheckReflect((int)Circle, ref source, ref target);

				double damage;

				damage = Utility.Random(4, 2);

				if (CheckResisted(target))
				{
					damage = 1;
					//damage *= 0.60;

					target.SendLocalizedMessage(501783); // You feel yourself resisting magical energy.
				}

				damage *= GetDamageScalar(m);

                if (SpellDamageController._OldTravellingSpellDelay)
                {
                    double dist = source.GetDistanceToSqrt(target);
                    SpellHelper.Damage(this, TimeSpan.FromSeconds(0.118 * Math.Min(dist, 6.0)), target, damage);
                }
                else
                {
                    SpellHelper.Damage(this, TimeSpan.FromSeconds(SpellDamageController._MagicArrowDamageDelay), target, damage);
                }

				source.MovingParticles(target, 0x36E4, 5, 0, false, false, 3006, 0, 0);
				source.PlaySound(0x1E5);
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private readonly MagicArrowSpell m_Owner;

			public InternalTarget(MagicArrowSpell owner)
				: base(owner.Caster.EraML ? 10 : 12, false, TargetFlags.Harmful)
			{
				m_Owner = owner;
			}

			protected override void OnTarget(Mobile from, object o)
			{
				var entity = o as IEntity;
				if (XmlScript.HasTrigger(entity, TriggerName.onTargeted) &&
					UberScriptTriggers.Trigger(entity, from, TriggerName.onTargeted, null, null, m_Owner))
				{
					return;
				}
				if (o is Mobile)
				{
					m_Owner.Target((Mobile)o);
				}
			}

			protected override void OnTargetFinish(Mobile from)
			{
				m_Owner.FinishSequence();
			}
		}
	}
}