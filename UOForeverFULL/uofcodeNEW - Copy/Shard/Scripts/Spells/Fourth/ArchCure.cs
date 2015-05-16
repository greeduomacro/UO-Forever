#region References
using System;
using System.Collections.Generic;

using Server.Engines.XmlSpawner2;
using Server.Mobiles;
using Server.Targeting;
#endregion

namespace Server.Spells.Fourth
{
	public class ArchCureSpell : MagerySpell
	{
		private static readonly SpellInfo m_Info = new SpellInfo(
			"Arch Cure", "Vas An Nox", 215, 9061, Reagent.Garlic, Reagent.Ginseng, Reagent.MandrakeRoot);

		public override SpellCircle Circle { get { return SpellCircle.Fourth; } }

		public ArchCureSpell(Mobile caster, Item scroll)
			: base(caster, scroll, m_Info)
		{ }

		public override void OnCast()
		{
			Caster.Target = new InternalTarget(this);
		}

		// Arch cure is now 1/4th of a second faster
		public override TimeSpan CastDelayBase { get { return base.CastDelayBase - TimeSpan.FromSeconds(0.25); } }

		public void Target(IPoint3D p)
		{
			if (!Caster.CanSee(p))
			{
				Caster.SendLocalizedMessage(500237); // Target can not be seen.
			}
			else if (CheckSequence())
			{
				SpellHelper.Turn(Caster, p);

				SpellHelper.GetSurfaceTop(ref p);

				var targets = new List<Mobile>();

				Map map = Caster.Map;
				var directTarget = p as Mobile;

				if (map != null)
				{
					bool feluccaRules = (map.Rules == MapRules.FeluccaRules);

					// You can target any living mobile directly, beneficial checks apply
					if (directTarget != null && Caster.CanBeBeneficial(directTarget, false))
					{
						targets.Add(directTarget);
					}

					IPooledEnumerable eable = map.GetMobilesInRange(new Point3D(p), 2);

					foreach (Mobile m in eable)
					{
						if (m != directTarget && AreaCanTarget(m, feluccaRules))
						{
							targets.Add(m);
						}
					}

					eable.Free();
				}

				Effects.PlaySound(p, Caster.Map, 0x299);

				if (targets.Count > 0)
				{
					int cured = 0;

					for (int i = 0; i < targets.Count; ++i)
					{
						Mobile m = targets[i];

						Caster.DoBeneficial(m);

						Poison poison = m.Poison;

						if (poison != null)
						{
							int chanceToCure = 10000 + (int)(Caster.Skills[SkillName.Magery].Value * 75) - ((poison.Level + 1) * 1750);
							chanceToCure /= 100;
							chanceToCure -= 1;

							if (chanceToCure > Utility.Random(100) && m.CurePoison(Caster))
							{
								++cured;
							}
						}

						m.FixedParticles(0x373A, 10, 15, 5012, EffectLayer.Waist);
						m.PlaySound(0x1E0);
					}

					if (cured > 0)
					{
						Caster.SendLocalizedMessage(1010058); // You have cured the target of all poisons!
					}
				}
			}

			FinishSequence();
		}

		private bool AreaCanTarget(Mobile target, bool feluccaRules)
		{
			/* Arch cure area effect won't cure aggressors, victims, murderers, criminals or monsters.
			 * In Felucca, it will also not cure summons and pets.
			 * For red/criminal players it will only cure themselves and guild members.
			 */

			if (!Caster.CanBeBeneficial(target, false))
			{
				return false;
			}

			if (Caster.EraAOS && target != Caster)
			{
				if (IsAggressor(target) || IsAggressed(target))
				{
					return false;
				}

				int notoCaster = Notoriety.Compute(Caster, target);
				int notoTarget = Notoriety.Compute(target, Caster);

				if ((notoCaster != Notoriety.Innocent || notoTarget != Notoriety.Innocent) && notoCaster != Notoriety.Ally)
				{
					return false;
				}

				if (feluccaRules && !(target is PlayerMobile))
				{
					return false;
				}
			}

			return true;
		}

		private bool IsAggressor(Mobile m)
		{
			foreach (AggressorInfo info in Caster.Aggressors)
			{
				if (m == info.Attacker && !info.Expired)
				{
					return true;
				}
			}

			return false;
		}

		private bool IsAggressed(Mobile m)
		{
			foreach (AggressorInfo info in Caster.Aggressed)
			{
				if (m == info.Defender && !info.Expired)
				{
					return true;
				}
			}

			return false;
		}

		private class InternalTarget : Target
		{
			private readonly ArchCureSpell m_Owner;

			public InternalTarget(ArchCureSpell owner)
				: base(owner.Caster.EraML ? 10 : 12, true, TargetFlags.None)
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

				var p = o as IPoint3D;

				if (p != null)
				{
					m_Owner.Target(p);
				}
			}

			protected override void OnTargetFinish(Mobile from)
			{
				m_Owner.FinishSequence();
			}
		}
	}
}