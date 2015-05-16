#region References
using System;
using System.Collections.Generic;
#endregion

namespace Server.Spells.Eighth
{
	public class EarthquakeSpell : MagerySpell
	{
		private static readonly SpellInfo m_Info = new SpellInfo(
			"Earthquake",
			"In Vas Por",
			233,
			9012,
			false,
			Reagent.Bloodmoss,
			Reagent.Ginseng,
			Reagent.MandrakeRoot,
			Reagent.SulfurousAsh);

		public override SpellCircle Circle { get { return SpellCircle.Eighth; } }

		public EarthquakeSpell(Mobile caster, Item scroll)
			: base(caster, scroll, m_Info)
		{ }

		public override bool DelayedDamage { get { return Caster != null && !Caster.EraAOS; } }

		public override void OnCast()
		{
			if (SpellHelper.CheckTown(Caster, Caster) && CheckSequence())
			{
				var targets = new List<Mobile>();

				Map map = Caster.Map;

				if (map != null)
				{
					foreach (Mobile m in Caster.GetMobilesInRange(1 + (int)(Caster.Skills[SkillName.Magery].Value / 15.0)))
					{
						if (Caster != m && /*SpellHelper.ValidIndirectTarget( Caster, m )*/ Caster.CanBeHarmful(m, false) &&
							(!Caster.EraAOS || Caster.InLOS(m))) //hurt everyone but caster
						{
							targets.Add(m);
						}
					}
				}

				Caster.PlaySound(0x220);

				for (int i = 0; i < targets.Count; ++i)
				{
					Mobile m = targets[i];

					int damage;

					if (Caster.EraAOS)
					{
						damage = m.Hits / 2;

						if (!m.Player)
						{
							damage = Math.Max(Math.Min(damage, 100), 15);
						}

						damage += Utility.Random(16);
					}
					else if(Caster.EraUOR)
					{
						//damage = (m.Hits * 6) / 10;

						damage = 0; //split between all hurts mobs

						if (!m.Player && damage < 10)
						{
							damage = 10;
						}
						else if (damage > 75)
						{
							damage = 75;
						}
					}
					else
					{
						// HACK: Convert to T2A mechanics.

						//damage = (m.Hits * 6) / 10;

						damage = 0; //split between all hurts mobs

						if (!m.Player && damage < 10)
						{
							damage = 10;
						}
						else if (damage > 75)
						{
							damage = 75;
						}
					}

					//Caster.DoHarmful( m );
					SpellHelper.Damage(this, m, damage);
				}
			}

			FinishSequence();
		}
	}
}