#region References
using System;

using Server.Engines.ConPVP;
using Server.Engines.XmlSpawner2;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
#endregion

namespace Server.Spells.First
{
	public class HealSpell : MagerySpell
	{
		private static readonly SpellInfo m_Info = new SpellInfo(
			"Heal", "In Mani", 224, 9061, Reagent.Garlic, Reagent.Ginseng, Reagent.SpidersSilk);

		public override SpellCircle Circle { get { return SpellCircle.First; } }

		public HealSpell(Mobile caster, Item scroll)
			: base(caster, scroll, m_Info)
		{ }

		public override bool CheckCast()
		{
			if (DuelContext.CheckSuddenDeath(Caster))
			{
				Caster.SendMessage(0x22, "You cannot cast this spell when in sudden death.");
				return false;
			}

			return base.CheckCast();
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget(this);
		}

		public void Target(Mobile m)
		{
			if (!Caster.CanSee(m))
			{
				Caster.SendLocalizedMessage(500237); // Target can not be seen.
			}
			else if (m.IsDeadBondedPet)
			{
				Caster.SendLocalizedMessage(1060177); // You cannot heal a creature that is already dead!
			}
			else if (m is BaseCreature && ((BaseCreature)m).IsAnimatedDead)
			{
				Caster.SendLocalizedMessage(1061654); // You cannot heal that which is not alive.
			}
			else if (m is Golem ||
					 (m is BaseCreature && (((BaseCreature)m).Pseu_CanBeHealed == false || ((BaseCreature)m).ChampSpawn != null)))
				// don't allow champspawn mobs to be healed
			{
				Caster.LocalOverheadMessage(MessageType.Regular, 0x3B2, 500951); // You cannot heal that.
			}
			else if (m.Poisoned && !m.IsT2A|| MortalStrike.IsWounded(m))
			{
				Caster.LocalOverheadMessage(MessageType.Regular, 0x22, (Caster == m) ? 1005000 : 1010398);
			}
			else if (CheckBSequence(m))
			{
				SpellHelper.Turn(Caster, m);

				int toHeal = 0;

				if (Caster.EraAOS)
				{
					toHeal = Utility.RandomMinMax(1, 4) + (Caster.Skills.Magery.Fixed / 120);

					if (Caster.EraSE && Caster != m)
					{
						toHeal = (int)(toHeal * 1.5);
					}
				}
				else if (Caster.EraUOR)
				{
					toHeal = Utility.Random(1, 5) + (int)(Caster.Skills[SkillName.Magery].Value * 0.1);
				}
				else if (Caster.EraT2A)
				{
					// HACK: Convert to T2A mechanics.
					toHeal = Utility.Random(1, 5) + (int)(Caster.Skills[SkillName.Magery].Value * 0.1);
				}

				int healmessage = Math.Min(m.HitsMax - m.Hits, toHeal);

				if (healmessage > 0)
				{
					m.PrivateOverheadMessage(MessageType.Regular, 0x42, false, healmessage.ToString(), m.NetState);
					if (Caster != m)
					{
						m.PrivateOverheadMessage(MessageType.Regular, 0x42, false, healmessage.ToString(), Caster.NetState);
					}
				}

				Caster.DoBeneficial(m);
				SpellHelper.Heal(toHeal, m, Caster);

				m.FixedParticles(0x376A, 9, 32, 5005, EffectLayer.Waist);
				m.PlaySound(0x1F2);
			}

			FinishSequence();
		}

		public class InternalTarget : Target
		{
			private readonly HealSpell m_Owner;

			public InternalTarget(HealSpell owner)
				: base(owner.Caster.EraML ? 10 : 12, false, TargetFlags.Beneficial)
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