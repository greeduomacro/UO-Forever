#region References
using System.Collections.Generic;

using Server.Engines.XmlSpawner2;
using Server.Targeting;
#endregion

namespace Server.Spells.Sixth
{
	public class MassCurseSpell : MagerySpell
	{
		private static readonly SpellInfo m_Info = new SpellInfo(
			"Mass Curse",
			"Vas Des Sanct",
			218,
			9031,
			false,
			Reagent.Garlic,
			Reagent.Nightshade,
			Reagent.MandrakeRoot,
			Reagent.SulfurousAsh);

		public override SpellCircle Circle { get { return SpellCircle.Sixth; } }

		public MassCurseSpell(Mobile caster, Item scroll)
			: base(caster, scroll, m_Info)
		{ }

		public override void OnCast()
		{
			Caster.Target = new InternalTarget(this);
		}

		public void Target(IPoint3D p)
		{
			if (!Caster.CanSee(p))
			{
				Caster.SendLocalizedMessage(500237); // Target can not be seen.
			}
			else if (SpellHelper.CheckTown(p, Caster) && CheckSequence())
			{
				SpellHelper.Turn(Caster, p);

				SpellHelper.GetSurfaceTop(ref p);

				var targets = new List<Mobile>();

				Map map = Caster.Map;

				if (map != null)
				{
					IPooledEnumerable eable = map.GetMobilesInRange(new Point3D(p), 2);

					foreach (Mobile m in eable)
					{
						if (Caster.EraAOS && m == Caster)
						{
							continue;
						}

						if (SpellHelper.ValidIndirectTarget(Caster, m) && Caster.CanSee(m) && Caster.CanBeHarmful(m, false))
						{
							targets.Add(m);
						}
					}

					eable.Free();
				}

				for (int i = 0; i < targets.Count; ++i)
				{
					Mobile m = targets[i];

					Caster.DoHarmful(m);

					SpellHelper.AddStatCurse(Caster, m, StatType.Str, false); // SpellHelper.DisableSkillCheck = true;
					SpellHelper.AddStatCurse(Caster, m, StatType.Dex, false);
					SpellHelper.AddStatCurse(Caster, m, StatType.Int, false); // SpellHelper.DisableSkillCheck = false;

					m.FixedParticles(0x374A, 10, 15, 5028, EffectLayer.Waist);
					m.PlaySound(0x1FB);

					HarmfulSpell(m);
				}
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private readonly MassCurseSpell m_Owner;

			public InternalTarget(MassCurseSpell owner)
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