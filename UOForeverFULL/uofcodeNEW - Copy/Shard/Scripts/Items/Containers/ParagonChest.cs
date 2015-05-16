#region References
using System;
using System.Linq;

using Server.Games;
#endregion

namespace Server.Items
{
	[Flipable]
	public class ParagonChest : LockableContainer
	{
		private static readonly int[] m_ItemIDs = new[] {0x9AB, 0xE40, 0xE41, 0xE7C};

		private static readonly int[] m_Hues = new[]
		{0x0, 0x455, 0x47E, 0x89F, 0x8A5, 0x8AB, 0x966, 0x96D, 0x972, 0x973, 0x979};

		private string m_Name;
		private bool m_Filled;
		private int m_Level;

		[Constructable]
		public ParagonChest(string name, int level)
			: base(Utility.RandomList(m_ItemIDs))
		{
			m_Name = name;
			m_Level = level;
			Hue = Utility.RandomList(m_Hues);
		}

		public override void OnSingleClick(Mobile from)
		{
			LabelToExpansion(from);

			LabelTo(from, 1063449, m_Name);
		}

		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);

			list.Add(1063449, m_Name);
		}

		private static void GetRandomAOSStats(out int attributeCount, out int min, out int max)
		{
			int rnd = Utility.Random(15);

			if (rnd < 1)
			{
				attributeCount = Utility.RandomMinMax(2, 6);
				min = 20;
				max = 70;
			}
			else if (rnd < 3)
			{
				attributeCount = Utility.RandomMinMax(2, 4);
				min = 20;
				max = 50;
			}
			else if (rnd < 6)
			{
				attributeCount = Utility.RandomMinMax(2, 3);
				min = 20;
				max = 40;
			}
			else if (rnd < 10)
			{
				attributeCount = Utility.RandomMinMax(1, 2);
				min = 10;
				max = 30;
			}
			else
			{
				attributeCount = 1;
				min = 10;
				max = 20;
			}
		}

		public void Flip()
		{
			switch (ItemID)
			{
				case 0x9AB:
					ItemID = 0xE7C;
					break;
				case 0xE7C:
					ItemID = 0x9AB;
					break;

				case 0xE40:
					ItemID = 0xE41;
					break;
				case 0xE41:
					ItemID = 0xE40;
					break;
			}
		}

		private void Fill()
		{
			if (m_Filled)
			{
				return;
			}

			m_Filled = true;

			TrapType = TrapType.ExplosionTrap;
			TrapPower = m_Level * 25;
			TrapLevel = m_Level;
			Locked = true;

			switch (m_Level)
			{
				case 1:
					RequiredSkill = 36;
					break;
				case 2:
					RequiredSkill = 76;
					break;
				case 3:
					RequiredSkill = 84;
					break;
				case 4:
					RequiredSkill = 92;
					break;
				case 5:
					RequiredSkill = 100;
					break;
			}

			LockLevel = RequiredSkill - 10;
			MaxLockLevel = RequiredSkill + 40;

			DropItem(new Gold(m_Level * 200));

			for (int i = 0; i < m_Level; ++i)
			{
				DropItem(Loot.RandomScroll(0, 63, SpellbookType.Regular));
				DropItem(new Platinum(PseudoSeerStone.ParagonChestPlatinumPerLevel));
			}

			for (int i = 0; i < m_Level * 2; ++i)
			{
				Item item = Loot.RandomArmorOrShieldOrWeapon();

				if (item is BaseWeapon)
				{
					var weapon = (BaseWeapon)item;

					int damageLevel = PseudoSeerStone.GetDamageLevel(m_Level);
					
					if (PseudoSeerStone.HighestDamageLevelSpawn < damageLevel)
					{
						/*if (damageLevel == 5 && PseudoSeerStone.ReplaceVanqWithSkillScrolls)
						{
							DropItem(PuzzleChest.CreateRandomSkillScroll());
						}*/
						
						int platAmount = PseudoSeerStone.PlatinumPerMissedDamageLevel *
										 (damageLevel - PseudoSeerStone.Instance._HighestDamageLevelSpawn);
						
						if (platAmount > 0)
						{
							DropItem(new Platinum(platAmount));
						}
					
						damageLevel = PseudoSeerStone.Instance._HighestDamageLevelSpawn;
					}
					
					weapon.DamageLevel = (WeaponDamageLevel)damageLevel;
					weapon.DurabilityLevel = (WeaponDurabilityLevel)PseudoSeerStone.GetDurabilityLevel(m_Level);
					weapon.AccuracyLevel = (WeaponAccuracyLevel)PseudoSeerStone.GetAccuracyLevel(m_Level);
					
					if (0.02 * m_Level >= Utility.RandomDouble())
					{
						weapon.Slayer = (SlayerName)Utility.Random(28);
					}

					DropItem(item);
				}
				else if (item is BaseArmor)
				{
					var armor = (BaseArmor)item;

					armor.ProtectionLevel = (ArmorProtectionLevel)Utility.Random(6);
					armor.Durability = (ArmorDurabilityLevel)Utility.Random(6);

					DropItem(item);
				}
				else if (item is BaseHat)
				{
					DropItem(item);
				}
				else if (item is BaseJewel)
				{
					DropItem(item);
				}
			}

		    if (Utility.RandomDouble() <= 0.05)
		    {
		        var scroll = new SkillScroll();
                scroll.Randomize();
                DropItem(scroll);
		    }

			for (int i = 0; i < m_Level; i++)
			{
				Item item = Loot.RandomPossibleReagent(Expansion);
				item.Amount = Utility.RandomMinMax(40, 60);
				
				DropItem(item);
			}

			for (int i = 0; i < m_Level; i++)
			{
				Item item = Loot.RandomGem();
				DropItem(item);
			}

			DropItem(new TreasureMap(m_Level + 1, Map.Felucca));
		}

		public ParagonChest(Serial serial)
			: base(serial)
		{ }

		protected override void OnExpansionChanged(Expansion old)
		{
			base.OnExpansionChanged(old);

			Items.Where(i => i != null && !i.Deleted).ForEach(i => i.Delete());

			Fill();
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(1); // version

			// 1
			writer.Write(m_Filled);
			writer.Write(m_Level);

			// 0
			writer.Write(m_Name);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			switch (version)
			{
				case 1:
					{
						m_Filled = reader.ReadBool();
						m_Level = reader.ReadInt();
					}
					goto case 0;
				case 0:
					m_Name = Utility.Intern(reader.ReadString());
					break;
			}
		}
	}
}