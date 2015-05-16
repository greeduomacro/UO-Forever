#region References
using System;
using System.Collections.Generic;
using Server.Mobiles;

#endregion

namespace Server.Items
{
	public class PowerScroll : SpecialScroll
	{
		/* 
		 * Using a scroll increases the maximum amount of a specific skill or your maximum statistics.
		 * When used, the effect is not immediately seen without a gain of points with that skill or statistics.
		 * You can view your maximum skill values in your skills window.
		 * You can view your maximum statistic value in your statistics window.
		 */
		public override int Message { get { return 1049469; } }

		public override int Title
		{
			get
			{
				double level = (Value - 105.0) / 5.0;

				if (level >= 0.0 && level <= 3.0 && Value % 5.0 == 0.0)
				{
					/* Wonderous Scroll (105 Skill): OR
					 * Exalted Scroll (110 Skill): OR
					 * Mythical Scroll (115 Skill): OR
					 * Legendary Scroll (120 Skill): 
					 */
					return 1049635 + (int)level;
				}

				return 0;
			}
		}

		public override string DefaultTitle { get { return String.Format("<basefont color=#FFFFFF>Power Scroll ({0} Skill):</basefont>", Value); } }

		private static readonly SkillName[] m_OldSkills = new[]
		{
			//
			//SkillName.Swords,
			//SkillName.Fencing,
			//SkillName.Macing,
			//SkillName.Archery,
			//SkillName.Wrestling,
			//SkillName.Parry,
			//SkillName.Tactics,
			//SkillName.Anatomy,
			//SkillName.Healing,
			//SkillName.Magery,
			//SkillName.Meditation,
			//SkillName.EvalInt,
			//SkillName.MagicResist,
			//SkillName.Veterinary,
			//SkillName.Musicianship,
			//SkillName.Discordance,
			//SkillName.Alchemy,
			//SkillName.Fletching,
			SkillName.Blacksmith, //
			SkillName.Tailoring, //
			SkillName.AnimalTaming, //
			SkillName.AnimalLore, //
			SkillName.Provocation, //
			SkillName.Peacemaking, //
			SkillName.Mining, //
			SkillName.Carpentry, //
			SkillName.Fishing, //
			SkillName.Lumberjacking, //
			SkillName.Tinkering //
		};

		private static readonly SkillName[] m_AOSSkills = new[]
		{
			//
			SkillName.Chivalry, //
			SkillName.Focus, //
			SkillName.Necromancy, //
			SkillName.Stealing, //
			SkillName.Stealth, //
			SkillName.SpiritSpeak //
		};

		private static readonly SkillName[] m_SESkills = new[]
		{
			//
			SkillName.Ninjitsu, //
			SkillName.Bushido //
		};

		private static readonly SkillName[] m_MLSkills = new[]
		{
			//
			SkillName.Spellweaving //
		};

		private static readonly SkillName[] m_SASkills = new[]
		{
			//
			SkillName.Throwing, //
			SkillName.Mysticism, //
			SkillName.Imbuing //
		};

		private static readonly SkillName[] m_HSSkills = new[]
		{
			//
			SkillName.Fishing //
		};

		public static List<SkillName> GetSkills(Expansion e)
		{
			var skills = new List<SkillName>(m_OldSkills);

			if (e >= Expansion.AOS)
			{
				skills.AddRange(m_AOSSkills);
			}

			if (e >= Expansion.SE)
			{
				skills.AddRange(m_SESkills);
			}

			if (e >= Expansion.ML)
			{
				skills.AddRange(m_MLSkills);
			}

			if (e >= Expansion.SA)
			{
				skills.AddRange(m_SASkills);
			}

			if (e >= Expansion.HS)
			{
				skills.AddRange(m_HSSkills);
			}

			return skills;
		}

		public static PowerScroll CreateRandom(int min, int max, Expansion e)
		{
			min /= 5;
			max /= 5;

			return new PowerScroll(GetSkills(e).GetRandom(), 100 + (Utility.RandomMinMax(min, max) * 5));
		}

		public static PowerScroll CreateRandomNoCraft(int min, int max, Expansion e)
		{
			min /= 5;
			max /= 5;

			List<SkillName> skills = GetSkills(e);

			SkillName skillName;

			do
			{
				skillName = skills.GetRandom();
			}
			while (skillName == SkillName.Blacksmith || skillName == SkillName.Tailoring);

			return new PowerScroll(skillName, 100 + (Utility.RandomMinMax(min, max) * 5));
		}

		public PowerScroll()
			: this(SkillName.Alchemy, 0.0)
		{ }

		[Constructable]
		public PowerScroll(SkillName skill, double value)
			: base(skill, value)
		{
			Hue = 0x481;

			if (Value == 105.0 || skill == SkillName.Blacksmith || skill == SkillName.Tailoring)
			{
				LootType = LootType.Regular;
			}
		}

		public PowerScroll(Serial serial)
			: base(serial)
		{ }

		public override void AddNameProperty(ObjectPropertyList list)
		{
			double level = (Value - 105.0) / 5.0;

			if (level >= 0.0 && level <= 3.0 && Value % 5.0 == 0.0)
			{
				/* a wonderous scroll of ~1_type~ (105 Skill) OR
				 * an exalted scroll of ~1_type~ (110 Skill) OR
				 * a mythical scroll of ~1_type~ (115 Skill) OR
				 * a legendary scroll of ~1_type~ (120 Skill) 
				 */
				list.Add(1049639 + (int)level, GetNameLocalized());
			}
			else
			{
				list.Add("a power scroll of {0} ({1} Skill)", GetName(), Value);
			}
		}

		public override void OnSingleClick(Mobile from)
		{
			LabelToExpansion(from);

			/*double level = (Value - 105.0) / 5.0;

			if (level >= 0.0 && level <= 3.0 && Value % 5.0 == 0.0)
			{
				LabelTo(from, 1049639 + (int)level, GetNameLocalized());
			}
			else
			{
				LabelTo(from, "a power scroll of {0} ({1} Skill)", GetName(), Value);
			}*/

			LabelTo(from, "a power scroll of {0} ({1} Skill)", GetName(), Value);
		}

		public override bool CanUse(Mobile from)
		{
			if (base.CanUse(from))
			{
				Skill skill = from.Skills[Skill];

				if (skill == null || skill.BaseFixedPoint == 1200)
				{
					from.SendMessage("You already have the maximum level of skill.");
				}
				else if (skill.Value != skill.Cap)
				{
					from.SendMessage(
						"You do not meet the requirements for this powerscroll.  You must be at the skill cap to use this.");
				}
				else if (skill.Cap + 5.0 != Value)
				{
					from.SendMessage(
						String.Format(
							"You do not meet the requirements for this powerscroll. You must use a {0} powerscroll.", skill.Cap + 5.0));
				}
				else
				{
					return true;
				}
			}

			return false;
		}

		public override void Use(Mobile from)
		{
			if (!CanUse(from))
			{
				return;
			}

			// You feel a surge of magic as the scroll enhances your ~1_type~!
			from.SendLocalizedMessage(1049513, GetNameLocalized());

			from.Skills[Skill].Cap = Value;

			int max = 7000 + (int)((Value - 100.0) * 10.0);

			if (max > from.SkillsCap)
			{
				from.SkillsCap = max;
			}

			Effects.SendLocationParticles(
				EffectItem.Create(from.Location, from.Map, EffectItem.DefaultDuration), 0, 0, 0, 0, 0, 5060, 0);
			Effects.PlaySound(from.Location, from.Map, 0x243);

			Effects.SendMovingParticles(
				new Entity(Serial.Zero, new Point3D(from.X - 6, from.Y - 6, from.Z + 15), from.Map),
				from,
				0x36D4,
				7,
				0,
				false,
				true,
				0x497,
				0,
				9502,
				1,
				0,
				(EffectLayer)255,
				0x100);

			Effects.SendMovingParticles(
				new Entity(Serial.Zero, new Point3D(from.X - 4, from.Y - 6, from.Z + 15), from.Map),
				from,
				0x36D4,
				7,
				0,
				false,
				true,
				0x497,
				0,
				9502,
				1,
				0,
				(EffectLayer)255,
				0x100);

			Effects.SendMovingParticles(
				new Entity(Serial.Zero, new Point3D(from.X - 6, from.Y - 4, from.Z + 15), from.Map),
				from,
				0x36D4,
				7,
				0,
				false,
				true,
				0x497,
				0,
				9502,
				1,
				0,
				(EffectLayer)255,
				0x100);

			Effects.SendTargetParticles(from, 0x375A, 35, 90, 0x00, 0x00, 9502, (EffectLayer)255, 0x100);

			Delete();
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.ReadInt();

			if (Value == 105.0 || Skill == SkillName.Blacksmith || Skill == SkillName.Tailoring)
			{
				LootType = LootType.Regular;
			}
			else
			{
				LootType = LootType.Cursed;
				Insured = false;
			}

			var skills = GetSkills(Expansion);

			if (skills.Contains(Skill))
			{
				return;
			}

			SkillName skillName;

			do
			{
				skillName = skills.GetRandom();
			}
			while (skillName == SkillName.Blacksmith || skillName == SkillName.Tailoring);

			Skill = skillName;
		}
	}
}