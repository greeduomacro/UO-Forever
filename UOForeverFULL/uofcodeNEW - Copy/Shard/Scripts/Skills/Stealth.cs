#region References
using System;
using System.Linq;

using Server.Items;
using Server.Network;
using Server.Regions;
#endregion

namespace Server.SkillHandlers
{
	public class Stealth
	{
		public static void Initialize()
		{
			SkillInfo.Table[(int)SkillName.Stealth].Callback = OnUse;
		}

		public static double GetHidingRequirement(Mobile m)
		{
			return m == null ? 100.0 : m.EraML ? 30.0 : m.EraSE ? 50.0 : 80.0;
		}

		public static int[,] ArmorTable { get { return m_ArmorTable; } }

		private static readonly int[,] m_ArmorTable = new[,] {//
			//				Gorget, Gloves, Helmet, Arms, Legs, Chest, Shield
			/* Cloth	*/	{0, 0, 0, 0, 0, 0, 0}, //
			/* Leather	*/	{0, 0, 0, 0, 0, 0, 0}, //
			/* Studded	*/	{2, 2, 0, 4, 6, 10, 0}, //
			/* Bone		*/ 	{0, 5, 10, 10, 15, 25, 0}, //
			/* Spined	*/	{0, 0, 0, 0, 0, 0, 0}, //
			/* Horned	*/	{0, 0, 0, 0, 0, 0, 0}, //
			/* Barbed	*/	{0, 0, 0, 0, 0, 0, 0}, //
			/* Ring		*/	{0, 5, 0, 10, 15, 25, 0}, //
			/* Chain	*/	{0, 0, 10, 0, 15, 25, 0}, //
			/* Plate	*/	{5, 5, 10, 10, 15, 25, 0}, //
			/* Dragon	*/	{0, 5, 10, 10, 15, 25, 0} //
		};

		public static int GetArmorRating(Mobile m)
		{
			if (m == null)
			{
				return 0;
			}

			if (!m.EraAOS)
			{
				return (int)m.ArmorRating;
			}

			return
				m.Items.OfType<BaseArmor>()
				 .Not(a => (int)a.MaterialType >= m_ArmorTable.GetLength(0) || (int)a.BodyPosition >= m_ArmorTable.GetLength(1))
				 .Sum(armor => armor.ArmorBase);
		}

		public static TimeSpan OnUse(Mobile m)
		{
			if (!m.Hidden)
			{
				m.SendLocalizedMessage(502725); // You must hide first
				return TimeSpan.FromSeconds(1.0);
			}

			var customRegion = m.Region as CustomRegion;
			if (customRegion != null && customRegion.Controller != null && !customRegion.Controller.AllowHiding)
			{
				m.LocalOverheadMessage(MessageType.Regular, 38, false, "Hiding is not allowed here!");
				return TimeSpan.FromSeconds(1.0);
			}

			if (m.Skills[SkillName.Hiding].Base < ((m.EraML) ? 30.0 : (m.EraSE) ? 50.0 : 80.0))
			{
				m.SendLocalizedMessage(502726); // You are not hidden well enough.  Become better at hiding.
				m.RevealingAction();
			}
			else if (!m.CanBeginAction(typeof(Stealth)))
			{
				m.SendLocalizedMessage(1063086); // You cannot use this skill right now.
				m.RevealingAction();
			}
			else
			{
				int armorRating = GetArmorRating(m);

				if (armorRating >= (m.EraAOS ? 42 : 26)) //I have a hunch '42' was chosen cause someone's a fan of DNA
				{
					m.SendLocalizedMessage(502727); // You could not hope to move quietly wearing this much armor.
					m.RevealingAction();
				}
				else if (m.CheckSkill(SkillName.Stealth, -20.0 + (armorRating * 2), (m.EraAOS ? 60.0 : 80.0) + (armorRating * 2)))
				{
					var steps = (int)(m.Skills[SkillName.Stealth].Value / (m.EraAOS ? 5.0 : 10.0));

					if (steps < 1)
					{
						steps = 1;
					}

					m.AllowedStealthSteps = steps;
					m.IsStealthing = true;

					m.SendLocalizedMessage(502730); // You begin to move quietly.

					return TimeSpan.FromSeconds(10.0);
				}
				else
				{
					m.SendLocalizedMessage(502731); // You fail in your attempt to move unnoticed.
					m.RevealingAction();
				}
			}

			return TimeSpan.FromSeconds(10.0);
		}
	}
}