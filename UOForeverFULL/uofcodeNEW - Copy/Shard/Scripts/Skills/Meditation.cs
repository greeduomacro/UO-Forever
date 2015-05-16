#region References
using System;

using Server.Items;
using Server.Misc;
#endregion

namespace Server.SkillHandlers
{
	internal class Meditation
	{
		public static void Initialize()
		{
			SkillInfo.Table[46].Callback = OnUse;
		}

		public static bool CheckOkayHolding(Item item)
		{
			if (item == null)
			{
				return true;
			}

			if (item is Spellbook || item is Runebook || item is ChaosShield || item is OrderShield || item is TwilightLantern)
			{
				return true;
			}

			if (item is GlacialStaff || item is InfernoStaff)
			{
				return true;
			}

			return false;
		}

		public static bool CannotMeditateWith(BaseArmor ar)
		{
			return ar != null && ar.MeditationAllowance == ArmorMeditationAllowance.None;
		}

		public static bool CannotMeditateWithArmor(Mobile m)
		{
			return CannotMeditateWith(m.ShieldArmor as BaseArmor) || CannotMeditateWith(m.NeckArmor as BaseArmor) ||
				   CannotMeditateWith(m.HandArmor as BaseArmor) || CannotMeditateWith(m.HeadArmor as BaseArmor) ||
				   CannotMeditateWith(m.ArmsArmor as BaseArmor) || CannotMeditateWith(m.LegsArmor as BaseArmor) ||
				   CannotMeditateWith(m.ChestArmor as BaseArmor);
		}

		public static TimeSpan OnUse(Mobile m)
		{
			m.RevealingAction();

			if (m.Target != null)
			{
				m.SendLocalizedMessage(501845); // You are busy doing something else and cannot focus.

				return TimeSpan.FromSeconds(5.0);
			}
			else if (!m.EraAOS && m.Hits < (m.HitsMax / 10)) // Less than 10% health
			{
				m.SendLocalizedMessage(501849); // The mind is strong but the body is weak.

				return TimeSpan.FromSeconds(5.0);
			}
			else if (m.Mana >= m.ManaMax)
			{
				m.SendLocalizedMessage(501846); // You are at peace.

				return TimeSpan.FromSeconds(m.EraAOS ? 10.0 : 5.0);
			}
			else if ((m.EraAOS && RegenRates.GetArmorOffset(m) > 0.0) || CannotMeditateWithArmor(m))
			{
				m.SendLocalizedMessage(500135); // Regenative forces cannot penetrate your armor!

				return TimeSpan.FromSeconds(5.0);
			}
			else
			{
				Item oneHanded = m.FindItemOnLayer(Layer.OneHanded);
				Item twoHanded = m.FindItemOnLayer(Layer.TwoHanded);

				if (m.EraAOS && m.Player)
				{
					if (!CheckOkayHolding(oneHanded))
					{
						m.AddToBackpack(oneHanded);
					}

					if (!CheckOkayHolding(twoHanded))
					{
						m.AddToBackpack(twoHanded);
					}
				}
				else if (!CheckOkayHolding(oneHanded) || !CheckOkayHolding(twoHanded))
				{
					m.SendLocalizedMessage(502626); // Your hands must be free to cast spells or meditate.

					return TimeSpan.FromSeconds(5.0);
				}

				double skillVal = m.Skills[SkillName.Meditation].Value;
				//double chance = (50.0 + (( skillVal - ( m.ManaMax - m.Mana ) ) * 2)) / 100;
				double chance = (20.0 + skillVal) / 100.0;

				if (chance > Utility.RandomDouble())
				{
					m.CheckSkill(SkillName.Meditation, 0.0, 100.0);

					m.SendLocalizedMessage(501851); // You enter a meditative trance.
					m.Meditating = true;
					BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.ActiveMeditation, 1075657));

					if (m.Player || m.Body.IsHuman)
					{
						m.PlaySound(0xF9);
					}
				}
				else
				{
					m.SendLocalizedMessage(501850); // You cannot focus your concentration.
				}

				return TimeSpan.FromSeconds(7.5);
			}
		}
	}
}