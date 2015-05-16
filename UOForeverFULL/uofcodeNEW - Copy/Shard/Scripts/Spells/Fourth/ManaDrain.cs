#region References
using System;
using System.Collections.Generic;

using Server.Engines.XmlSpawner2;
using Server.Items;
using Server.Targeting;
#endregion

namespace Server.Spells.Fourth
{
	public class ManaDrainSpell : MagerySpell
	{
		private static readonly SpellInfo m_Info = new SpellInfo(
			"Mana Drain", "Ort Rel", 215, 9031, Reagent.BlackPearl, Reagent.MandrakeRoot, Reagent.SpidersSilk);

		public override SpellCircle Circle { get { return SpellCircle.Fourth; } }

		public ManaDrainSpell(Mobile caster, Item scroll)
			: base(caster, scroll, m_Info)
		{ }

		public override void OnCast()
		{
			Caster.Target = new InternalTarget(this);
		}

		private static readonly Dictionary<Mobile, Timer> m_Table = new Dictionary<Mobile, Timer>();

		private class DelayInfo
		{
			public readonly Mobile Target;
			public readonly int ToDrain;

			public DelayInfo(Mobile target, int toDrain)
			{
				Target = target;
				ToDrain = toDrain;
			}
		}

		private void AosDelay_Callback(DelayInfo info)
		{
			Mobile m = info.Target;
			int mana = info.ToDrain;

			if (m.Alive && !m.IsDeadBondedPet)
			{
				m.Mana += mana;

				m.FixedEffect(0x3779, 10, 25);
				m.PlaySound(0x28E);
			}

			m_Table.Remove(m);
		}

		public void Target(Mobile m)
		{
			if (!Caster.CanSee(m))
			{
				Caster.SendLocalizedMessage(500237); // Target can not be seen.
			}
			else if (CheckHSequence(m))
			{
				Caster.DoHarmful(m);
				SpellHelper.Turn(Caster, m);

				Mobile source = Caster, target = m;

				SpellHelper.CheckReflect((int)Circle, ref source, ref target);

				if (m.Spell != null)
				{
					m.Spell.OnCasterHurt();
				}

				target.Paralyzed = false;

				if (source.EraAOS)
				{
					int toDrain = 40 + (int)(GetDamageSkill(Caster) - GetResistSkill(m));

					if (toDrain < 0)
					{
						toDrain = 0;
					}
					else if (toDrain > m.Mana)
					{
						toDrain = m.Mana;
					}

					if (m_Table.ContainsKey(target))
					{
						toDrain = 0;
					}

					target.FixedParticles(0x3789, 10, 25, 5032, EffectLayer.Head);
					target.PlaySound(0x1F8);

					if (toDrain > 0)
					{
						target.Mana -= toDrain;

						m_Table[target] = Timer.DelayCall(TimeSpan.FromSeconds(5.0), AosDelay_Callback, new DelayInfo(target, toDrain));
					}
				}
				else if(source.EraUOR)
				{
					if (CheckResisted(target))
					{
						target.SendLocalizedMessage(501783); // You feel yourself resisting magical energy.
					}
					else
					{
						target.Mana -=
							(int)(target.Mana * Math.Min(Math.Max(((GetDamageSkill(Caster) - GetResistSkill(m)) / 100.0), 0.25), 0.75));
					}
					//target.Mana -= Utility.Random( (int)GetDamageSkill( Caster ) / 10, Math.Min( 120, m.Mana ) );

					m.FixedParticles(0x374A, 10, 15, 5032, EffectLayer.Head);
					m.PlaySound(0x1F8);
				}
				else
				{
					// HACK: Convert to T2A mechanics.

					if (CheckResisted(target))
					{
						target.SendLocalizedMessage(501783); // You feel yourself resisting magical energy.
					}
					else
					{
						target.Mana -=
							(int)(target.Mana * Math.Min(Math.Max(((GetDamageSkill(Caster) - GetResistSkill(m)) / 100.0), 0.25), 0.75));
					}
					//target.Mana -= Utility.Random( (int)GetDamageSkill( Caster ) / 10, Math.Min( 120, m.Mana ) );

					m.FixedParticles(0x374A, 10, 15, 5032, EffectLayer.Head);
					m.PlaySound(0x1F8);
				}

				HarmfulSpell(m);
			}

			FinishSequence();
		}

		public override double GetResistPercent(Mobile target)
		{
			return SpellDamageController._ManaDrainResistChance;
		}

		private class InternalTarget : Target
		{
			private readonly ManaDrainSpell m_Owner;

			public InternalTarget(ManaDrainSpell owner)
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