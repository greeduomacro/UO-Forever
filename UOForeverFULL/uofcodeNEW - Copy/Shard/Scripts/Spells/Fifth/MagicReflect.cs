namespace Server.Spells.Fifth
{
	public class MagicReflectSpell : MagerySpell
	{
		private static readonly SpellInfo m_Info = new SpellInfo(
			"Magic Reflection", "In Jux Sanct", 242, 9012, Reagent.Garlic, Reagent.MandrakeRoot, Reagent.SpidersSilk);

		public override SpellCircle Circle { get { return SpellCircle.Fifth; } }

		public MagicReflectSpell(Mobile caster, Item scroll)
			: base(caster, scroll, m_Info)
		{ }

		public override bool CheckCast()
		{
			if (Caster.EraAOS)
			{
				return true;
			}

			if (Caster.MagicDamageAbsorb > 0)
			{
				Caster.SendLocalizedMessage(1005559); // This spell is already in effect.
				return false;
			}

			if (!Caster.CanBeginAction(typeof(DefensiveSpell)))
			{
				Caster.SendLocalizedMessage(1005385); // The spell will not adhere to you at this time.
				return false;
			}

			return true;
		}

		public override void OnCast()
		{
			if (Caster.MagicDamageAbsorb > 0)
			{
				Caster.SendLocalizedMessage(1005559); // This spell is already in effect.
			}
			else if (!Caster.CanBeginAction(typeof(DefensiveSpell)))
			{
				Caster.SendLocalizedMessage(1005385); // The spell will not adhere to you at this time.
			}
			else if (CheckSequence())
			{
				if (Caster.BeginAction(typeof(DefensiveSpell)))
				{
					var value = (int)(Caster.Skills[SkillName.Magery].Value);
                    if (!Caster.IsT2A)
                        value += (int)Caster.Skills[SkillName.Inscribe].Value;
					value = (int)(8 + (value / 200) * 7.0); //absorb from 8 to 15 "circles"

					Caster.MagicDamageAbsorb = value;

					Caster.FixedParticles(0x375A, 10, 15, 5037, EffectLayer.Waist);
					Caster.PlaySound(0x1E9);
				}
				else
				{
					Caster.SendLocalizedMessage(1005385); // The spell will not adhere to you at this time.
				}
			}

			FinishSequence();
		}
	}
}