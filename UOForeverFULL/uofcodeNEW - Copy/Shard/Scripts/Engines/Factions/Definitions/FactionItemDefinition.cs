#region References
using System;

using Server.Items;
using Server.Mobiles;
#endregion

namespace Server.Factions
{
	public class FactionItemDefinition
	{
		private readonly int m_SilverCost;
		private readonly Type m_VendorType;
		private readonly CraftSkillType m_Skills;

		public int SilverCost { get { return m_SilverCost; } }
		public Type VendorType { get { return m_VendorType; } }
		public CraftSkillType Skills { get { return m_Skills; } }

		public FactionItemDefinition(int silverCost, Type vendorType, CraftSkillType skills)
		{
			m_SilverCost = silverCost;
			m_VendorType = vendorType;
			m_Skills = skills;
		}

		private static readonly FactionItemDefinition m_MetalArmor = new FactionItemDefinition(
			1000, typeof(Blacksmith), CraftSkillType.Smithing);

		private static readonly FactionItemDefinition m_Weapon = new FactionItemDefinition(
			1000, typeof(Blacksmith), CraftSkillType.Smithing);

		private static readonly FactionItemDefinition m_Staves = new FactionItemDefinition(
			1000, typeof(Carpenter), CraftSkillType.Carpentry);

		private static readonly FactionItemDefinition m_RangedWeapon = new FactionItemDefinition(
			1000, typeof(Bowyer), CraftSkillType.Fletching);

		private static readonly FactionItemDefinition m_LeatherArmor = new FactionItemDefinition(
			250, typeof(Tailor), CraftSkillType.Tailoring);

		private static readonly FactionItemDefinition m_Clothing = new FactionItemDefinition(
			250, typeof(Tailor), CraftSkillType.Tailoring);

		private static readonly FactionItemDefinition m_Scroll = new FactionItemDefinition(
			250, typeof(Mage), CraftSkillType.None);

		public static FactionItemDefinition Identify(Item item)
		{
			if (item == null)
			{
				return null;
			}

			if (item is BaseArmor)
			{
				if (CraftResources.GetType(((BaseArmor)item).Resource) == CraftResourceType.Leather)
				{
					return m_LeatherArmor;
				}

				return m_MetalArmor;
			}

			if (item is BaseRanged)
			{
				return m_RangedWeapon;
			}
			else if (item is BaseStaff)
			{
				return m_Staves;
			}
			else if (item is BaseWeapon)
			{
				return m_Weapon;
			}
			else if (item is BaseClothing)
			{
				return m_Clothing;
			}
			else if (item.EraML && item is SpellScroll)
			{
				return m_Scroll;
			}

			return null;
		}
	}
}