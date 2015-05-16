#region References
using System;

using Server.Items;
using Server.Mobiles;

using VitaNex.Modules.AutoPvP;
using VitaNex.Modules.AutoPvP.Battles;
#endregion

namespace Server.Spells
{
	public abstract class MagerySpell : Spell
	{
		public MagerySpell(Mobile caster, Item scroll, SpellInfo info)
			: base(caster, scroll, info)
		{ }

		public abstract SpellCircle Circle { get; }

		public override bool ConsumeReagents()
		{
			var pm = Caster as PlayerMobile;

			if (pm != null)
			{
				var battle = AutoPvP.FindBattle(pm) as IUOFBattle;

				if (battle != null && battle.NoConsume)
				{
					return true;
				}
			}

			if (base.ConsumeReagents())
			{
				return true;
			}

			if (ArcaneGem.ConsumeCharges(Caster, (Caster.EraSE ? 1 : 1 + (int)Circle)))
			{
				return true;
			}

			return false;
		}

		private const double ChanceOffset = 20.0, ChanceLength = 100.0 / 7.0;

		public override void GetCastSkills(out double min, out double max)
		{
			var circle = (int)Circle;

			if (Scroll != null)
			{
				circle -= 2;
			}

			double avg = ChanceLength * circle;

			min = avg - ChanceOffset;
			max = avg + ChanceOffset;
		}

		private static readonly int[] m_ManaTable = new[] {4, 6, 9, 11, 14, 20, 40, 50};

		public override int GetMana()
		{
			if ((Scroll is BaseWand || Scroll is GnarledStaff) && !Scroll.EraML)
			{
				return 0;
			}

			return m_ManaTable[(int)Circle];
		}

		public override double GetResistSkill(Mobile m)
		{
			int maxSkill = ((1 + (int)Circle) * 10) + ((1 + ((int)Circle / 6)) * 25);

			if (m.Skills[SkillName.MagicResist].Value < maxSkill)
			{
				m.CheckSkill(SkillName.MagicResist, 0.0, m.Skills[SkillName.MagicResist].Cap);
			}

			return m.Skills[SkillName.MagicResist].Value;
		}

		public virtual bool CheckResisted(Mobile target)
		{
			double n = GetResistPercent(target);

			var circle = (int)Circle;

			int maxSkill = ((circle + 1) * 10) + (((circle / 6) + 1) * 25); //Can't resist with 8th circle spells.

			if (n < 1.0 && target.Skills[SkillName.MagicResist].Value <= maxSkill)
			{
				target.CheckSkill(SkillName.MagicResist, n);
			}

			return n >= Utility.RandomDouble();
		}

		public virtual double GetResistPercentForCircle(Mobile target, SpellCircle circle)
		{
			double firstPercent = target.Skills[SkillName.MagicResist].Value * SpellDamageController.__MaxFirstPercent * 0.01;
			double secondPercent = 0.01 *
								   ((target.Skills[SkillName.MagicResist].Value) -
									(Caster.Skills[CastSkill].Value * SpellDamageController.__MaxSecondPercentMageryAffect)) -
								   (1 + (int)circle) * SpellDamageController.__SecondPercentCircleAffect;

			return (firstPercent > secondPercent ? firstPercent : secondPercent) *
				   SpellDamageController.__GlobalResistChanceMultiplier; // Seems should be about half of what stratics says.
		}

		public virtual double GetResistPercent(Mobile target)
		{
			return GetResistPercentForCircle(target, Circle);
		}

		public override TimeSpan GetCastDelay()
		{
			if (Scroll is BaseWand || Scroll is GnarledStaff)
			{
				return TimeSpan.Zero;
			}

			//if( Core.AOS )
			//	return base.GetCastDelay();

			return TimeSpan.FromSeconds(0.75 + (0.25 * (int)Circle));
		}

		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds((3 + (int)Circle) * CastDelaySecondsPerTick); } }

		public override TimeSpan GetDisturbRecovery()
		{
			//if ( Core.AOS )
			//	return base.GetDisturbRecovery();

			//Time left in casting by %: (DateTime.UtcNow - m_StartCastTime).TotalSeconds / GetCastDelay().TotalSeconds
			var circle = (int)Circle;
			if (circle > 2)
			{
				circle = 2;
			}
			TimeSpan castdelay = TimeSpan.FromSeconds(0.75 + (0.25 * circle));
				//We ignore blade spirits, EVs and summon creatures

			double delay = castdelay.TotalSeconds - (DateTime.UtcNow - StartCastTime).TotalSeconds;

			if (delay < 0.0)
			{
				delay = 0.0;
			}
			else if (delay > 2.0)
			{
				delay = 2.0; // changed from 2.0
			}

			return TimeSpan.FromSeconds(delay);
		}
	}
}