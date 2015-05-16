#region References
using System.Collections.Generic;

using Server.Engines.XmlSpawner2;
using Server.Mobiles;
using Server.Targeting;
#endregion

namespace Server.Spells.Seventh
{
	public class MeteorSwarmSpell : MagerySpell
	{
		private static readonly SpellInfo m_Info = new SpellInfo(
			"Meteor Swarm",
			"Flam Kal Des Ylem",
			233,
			9042,
			false,
			Reagent.Bloodmoss,
			Reagent.MandrakeRoot,
			Reagent.SulfurousAsh,
			Reagent.SpidersSilk);

		public override SpellCircle Circle { get { return SpellCircle.Seventh; } }

		public MeteorSwarmSpell(Mobile caster, Item scroll)
			: base(caster, scroll, m_Info)
		{ }

		public override void OnCast()
		{
			Caster.Target = new InternalTarget(this);
		}

		public override bool DelayedDamage { get { return true; } }

		public void Target(IPoint3D p)
		{
			if (!Caster.CanSee(p))
			{
				Caster.SendLocalizedMessage(500237); // Target can not be seen.
			}
			else if (SpellHelper.CheckTown(p, Caster) && CheckSequence())
			{
				SpellHelper.Turn(Caster, p);

				if (p is Item)
				{
					p = ((Item)p).GetWorldLocation();
				}

				var targets = new List<Mobile>();

				Map map = Caster.Map;

				//bool playerVsPlayer = false;

				if (map != null)
				{
					IPooledEnumerable eable = map.GetMobilesInRange(new Point3D(p), 2);

					foreach (Mobile m in eable)
					{
						if (Caster != m /*&& SpellHelper.ValidIndirectTarget( Caster, m )*/&& Caster.CanBeHarmful(m, false))
							//harms everyone
						{
							if (Caster.EraAOS && !Caster.InLOS(m))
							{
								continue;
							}

							targets.Add(m);

							//if ( m.Player )
							//playerVsPlayer = true;
						}
					}

					eable.Free();
				}

				double damage = Utility.Random(30, 18); // 30 - 47

				if (targets.Count > 0)
				{
					Effects.PlaySound(p, Caster.Map, 0x160);

					if (Caster is BaseCreature)
					{
						var bc = (BaseCreature)Caster;

						// pseudoseer must be EQPlayerAllowed to do full damage to all
						if (bc.NetState != null && !bc.Pseu_EQPlayerAllowed)
						{
							// NEED TO TEST THIS
							damage /= targets.Count;
						}
					}
					else
					{
						damage /= targets.Count;
					}

					for (int i = 0; i < targets.Count; ++i)
					{
						Mobile m = targets[i];

						double toDeal = damage;

						if (CheckResisted(m))
						{
							toDeal *= 0.5;

							m.SendLocalizedMessage(501783); // You feel yourself resisting magical energy.
						}

						toDeal *= GetDamageScalar(m);
						//Caster.DoHarmful( m );
						SpellHelper.Damage(this, m, toDeal);

						Caster.MovingParticles(m, 0x36D4, 7, 0, false, true, 9501, 1, 0, 0x100);
					}
				}
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private readonly MeteorSwarmSpell m_Owner;

			public InternalTarget(MeteorSwarmSpell owner)
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