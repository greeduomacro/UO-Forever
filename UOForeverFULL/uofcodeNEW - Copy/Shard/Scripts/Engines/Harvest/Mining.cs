#region References
using System;
using Server.Engines.Conquests;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;
#endregion

namespace Server.Engines.Harvest
{
	public class Mining : HarvestSystem
	{
		private static Mining m_System;

		public static Mining System { get { return m_System ?? (m_System = new Mining()); } }

		public HarvestDefinition OreAndStone { get; private set; }
		public HarvestDefinition Sand { get; private set; }

		private Mining()
		{
			#region Mining for ore and stone
			HarvestDefinition oreAndStone = new HarvestDefinition {
				// Resource banks are every 8x8 tiles
				BankWidth = 4,
				BankHeight = 4,
				// Every bank holds from 10 to 34 ore
				MinTotal = 10,
				MaxTotal = 35,
				// A resource bank will respawn its content every 10 to 20 minutes
				MinRespawn = TimeSpan.FromMinutes(10.0),
				MaxRespawn = TimeSpan.FromMinutes(20.0),
				// Skill checking is done on the Mining skill
				Skill = SkillName.Mining,
				// Set the list of harvestable tiles
				Tiles = m_MountainAndCaveTiles,
				// Players must be within 2 tiles to harvest
				MaxRange = 2,
				// One ore per harvest action
				ConsumedPerHarvest = 1,
				ConsumedPerFeluccaHarvest = 2,
				// The digging effect
				EffectActions = new[] {11},
				EffectSounds = new[] {0x125, 0x126},
				EffectCounts = new[] {1},
				EffectDelay = TimeSpan.FromSeconds(1.6),
				EffectSoundDelay = TimeSpan.FromSeconds(0.9),
				NoResourcesMessage = 503040,
				DoubleHarvestMessage = 503042,
				TimedOutOfRangeMessage = 503041,
				OutOfRangeMessage = 500446,
				FailMessage = 503043,
				PackFullMessage = 1010481,
				ToolBrokeMessage = 1044038
			};

			HarvestResource[] res = new[]
			{
				new HarvestResource(Expansion.None, 00.0, 00.0, 100.0, 1007072, typeof(IronOre), typeof(Granite)),
				new HarvestResource(
					Expansion.None,
					65.0,
					25.0,
					105.0,
					1007073,
					typeof(DullCopperOre),
					typeof(DullCopperGranite),
					typeof(DullCopperElemental)),
				new HarvestResource(
					Expansion.None,
					70.0,
					30.0,
					110.0,
					1007074,
					typeof(ShadowIronOre),
					typeof(ShadowIronGranite),
					typeof(ShadowIronElemental)),
				new HarvestResource(
					Expansion.None, 75.0, 35.0, 115.0, 1007075, typeof(CopperOre), typeof(CopperGranite), typeof(CopperElemental)),
				new HarvestResource(
					Expansion.None, 80.0, 40.0, 120.0, 1007076, typeof(BronzeOre), typeof(BronzeGranite), typeof(BronzeElemental)),
				new HarvestResource(
					Expansion.None, 85.0, 45.0, 125.0, 1007077, typeof(GoldOre), typeof(GoldGranite), typeof(GoldenElemental)),
				new HarvestResource(
					Expansion.None, 90.0, 50.0, 130.0, 1007078, typeof(AgapiteOre), typeof(AgapiteGranite), typeof(AgapiteElemental)),
				new HarvestResource(
					Expansion.None, 95.0, 55.0, 135.0, 1007079, typeof(VeriteOre), typeof(VeriteGranite), typeof(VeriteElemental)),
				new HarvestResource(
					Expansion.None, 99.0, 59.0, 139.0, 1007080, typeof(ValoriteOre), typeof(ValoriteGranite), typeof(ValoriteElemental))
			};

			HarvestVein[] veins = new[]
			{
				new HarvestVein(Expansion.None, 49.6, 0.0, res[0], null), // Iron
				new HarvestVein(Expansion.None, 11.2, 0.5, res[1], res[0]), // Dull Copper
				new HarvestVein(Expansion.None, 09.8, 0.5, res[2], res[0]), // Shadow Iron
				new HarvestVein(Expansion.None, 08.4, 0.5, res[3], res[0]), // Copper
				new HarvestVein(Expansion.None, 07.0, 0.5, res[4], res[0]), // Bronze
				new HarvestVein(Expansion.None, 05.6, 0.5, res[5], res[0]), // Gold
				new HarvestVein(Expansion.None, 04.2, 0.5, res[6], res[0]), // Agapite
				new HarvestVein(Expansion.None, 02.8, 0.5, res[7], res[0]), // Verite
				new HarvestVein(Expansion.None, 01.4, 0.5, res[8], res[0]) // Valorite
			};

			oreAndStone.Resources = res;
			oreAndStone.Veins = veins;

			oreAndStone.PlaceAtFeetIfFull = true;

			oreAndStone.BonusResources = new[]
			{
				new BonusHarvestResource(Expansion.None, 0, 99.1, null, null), //Nothing
				new BonusHarvestResource(Expansion.None, 100, .1, "You have found a diamond!", typeof(Diamond)),
				new BonusHarvestResource(Expansion.None, 100, .1, "You have found a sapphire!", typeof(Sapphire)),
				new BonusHarvestResource(Expansion.None, 100, .1, "You have found a citrine!", typeof(Citrine)),
				new BonusHarvestResource(Expansion.None, 100, .1, "You have found a ruby!", typeof(Ruby)),
				new BonusHarvestResource(Expansion.None, 100, .1, "You have found an emerald!", typeof(Emerald)),
				new BonusHarvestResource(Expansion.None, 100, .1, "You have found a tourmaline!", typeof(Tourmaline)),
				new BonusHarvestResource(Expansion.None, 100, .1, "You have found an amethyst!", typeof(Amethyst)),
				new BonusHarvestResource(Expansion.None, 100, .1, "You have found a star sapphire!", typeof(StarSapphire)),
				new BonusHarvestResource(Expansion.None, 100, .1, "You have found a amber!", typeof(Amber))
			};

			oreAndStone.RaceBonus = false;
			oreAndStone.RandomizeVeins = false;

			OreAndStone = oreAndStone;
			Definitions.Add(OreAndStone);
			#endregion

			#region Mining for sand
			HarvestDefinition sand = new HarvestDefinition {
				// Resource banks are every 8x8 tiles
				BankWidth = 2,
				BankHeight = 2,
				// Every bank holds from 6 to 12 sand
				MinTotal = 6,
				MaxTotal = 12,
				// A resource bank will respawn its content every 10 to 20 minutes
				MinRespawn = TimeSpan.FromMinutes(10.0),
				MaxRespawn = TimeSpan.FromMinutes(20.0),
				// Skill checking is done on the Mining skill
				Skill = SkillName.Mining,
				// Set the list of harvestable tiles
				Tiles = m_SandTiles,
				// Players must be within 2 tiles to harvest
				MaxRange = 2,
				// One sand per harvest action
				ConsumedPerHarvest = 1,
				ConsumedPerFeluccaHarvest = 1,
				// The digging effect
				EffectActions = new[] {11},
				EffectSounds = new[] {0x125, 0x126},
				EffectCounts = new[] {6},
				EffectDelay = TimeSpan.FromSeconds(1.6),
				EffectSoundDelay = TimeSpan.FromSeconds(0.9),
				NoResourcesMessage = 1044629,
				DoubleHarvestMessage = 1044629,
				TimedOutOfRangeMessage = 503041,
				OutOfRangeMessage = 500446,
				FailMessage = 1044630,
				PackFullMessage = 1044632,
				ToolBrokeMessage = 1044038
			};

			res = new[] { new HarvestResource(Expansion.None, 100.0, 70.0, 400.0, 1044631, typeof(Sand)) };
			veins = new[] { new HarvestVein(Expansion.None, 100.0, 0.0, res[0], null) };

			sand.Resources = res;
			sand.Veins = veins;

			sand.PlaceAtFeetIfFull = true;
			
			Sand = sand;
			Definitions.Add(Sand);
			#endregion
		}

		public override Type GetResourceType(
			Mobile from, Item tool, HarvestDefinition def, Map map, Point3D loc, HarvestResource resource)
		{
			if (def == OreAndStone)
			{
				var pm = from as PlayerMobile;

				if (pm != null && pm.StoneMining && pm.ToggleMiningStone && from.Skills[SkillName.Mining].Base >= 100.0 &&
					0.1 > Utility.RandomDouble())
				{
					return resource.Types[1];
				}

				return resource.Types[0];
			}

			return base.GetResourceType(from, tool, def, map, loc, resource);
		}

		public override bool CheckHarvest(Mobile from, Item tool)
		{
			if (!base.CheckHarvest(from, tool))
			{
				return false;
			}

			if (from.Mounted)
			{
				from.SendLocalizedMessage(501864); // You can't mine while riding.
				return false;
			}
			
			if (from.IsBodyMod && !from.Body.IsHuman)
			{
				from.SendLocalizedMessage(501865); // You can't mine while polymorphed.
				return false;
			}

			return true;
		}

		public override void SendSuccessTo(Mobile from, Item item, HarvestResource resource)
		{
			if (item is BaseGranite)
			{
				from.SendLocalizedMessage(1044606); // You carefully extract some workable stone from the ore vein!
			}
			else
			{
				base.SendSuccessTo(from, item, resource);
			}
		}

		public override bool CheckHarvest(Mobile from, Item tool, HarvestDefinition def, object toHarvest)
		{
			if (!base.CheckHarvest(from, tool, def, toHarvest))
			{
				return false;
			}

			if (def == Sand &&
				!(from is PlayerMobile && from.Skills[SkillName.Mining].Base >= 100.0 && ((PlayerMobile)from).SandMining))
			{
				OnBadHarvestTarget(from, tool, toHarvest);
				return false;
			}
			
			if (from.Mounted)
			{
				from.SendLocalizedMessage(501864); // You can't mine while riding.
				return false;
			}
			
			if (from.IsBodyMod && !from.Body.IsHuman)
			{
				from.SendLocalizedMessage(501865); // You can't mine while polymorphed.
				return false;
			}

			return true;
		}

		public override HarvestVein MutateVein(
			Mobile from, Item tool, HarvestDefinition def, HarvestBank bank, object toHarvest, HarvestVein vein)
		{
			if (tool is GargoylesPickaxe && def == OreAndStone)
			{
				int veinIndex = Array.IndexOf(def.Veins, vein);

				if (veinIndex >= 0 && veinIndex < (def.Veins.Length - 1))
				{
					return def.Veins[veinIndex + 1];
				}
			}

			return base.MutateVein(from, tool, def, bank, toHarvest, vein);
		}

		private static readonly int[] m_Offsets = new[] {-1, -1, -1, 0, -1, 1, 0, -1, 0, 1, 1, -1, 1, 0, 1, 1};

		public override void OnHarvestFinished(
			Mobile from,
			Item tool,
			HarvestDefinition def,
			HarvestVein vein,
			HarvestBank bank,
			HarvestResource resource,
			object harvested)
		{
			if (tool is GargoylesPickaxe && def == OreAndStone && 0.03 > Utility.RandomDouble())
			{
				HarvestResource res = vein.PrimaryResource;

				if (res == resource && res.Types.Length >= 3)
				{
					try
					{
						Map map = from.Map;

						if (map == null)
						{
							return;
						}

						var spawned = Activator.CreateInstance(res.Types[2], new object[] {25}) as BaseCreature;

						if (spawned != null)
						{
							int offset = Utility.Random(8) * 2;

							for (int i = 0; i < m_Offsets.Length; i += 2)
							{
								int x = from.X + m_Offsets[(offset + i) % m_Offsets.Length];
								int y = from.Y + m_Offsets[(offset + i + 1) % m_Offsets.Length];

								if (map.CanSpawnMobile(x, y, from.Z))
								{
									spawned.OnBeforeSpawn(new Point3D(x, y, from.Z), map);
									spawned.MoveToWorld(new Point3D(x, y, from.Z), map);
									spawned.Combatant = from;
									return;
								}
								
								int z = map.GetAverageZ(x, y);

								if (Math.Abs(z - from.Z) >= 10 || !map.CanSpawnMobile(x, y, z))
								{
									continue;
								}

								spawned.OnBeforeSpawn(new Point3D(x, y, z), map);
								spawned.MoveToWorld(new Point3D(x, y, z), map);
								spawned.Combatant = from;
								return;
							}

							spawned.OnBeforeSpawn(from.Location, from.Map);
							spawned.MoveToWorld(from.Location, from.Map);
							spawned.Combatant = from;
						}
					}
					catch
					{ }
				}
			}
		}

		public override bool BeginHarvesting(Mobile from, Item tool)
		{
			if (!base.BeginHarvesting(from, tool))
			{
				return false;
			}

			from.SendLocalizedMessage(503033); // Where do you wish to dig?
			return true;
		}

		public override void OnHarvestStarted(Mobile from, Item tool, HarvestDefinition def, object toHarvest)
		{
			base.OnHarvestStarted(from, tool, def, toHarvest);

			if (from.EraML)
			{
				from.RevealingAction();
			}
		}

		public override void OnBadHarvestTarget(Mobile from, Item tool, object toHarvest)
		{
			if (toHarvest is LandTarget)
			{
				from.SendLocalizedMessage(501862); // You can't mine there.
			}
			else
			{
				from.SendLocalizedMessage(501863); // You can't mine that.
			}
		}

		#region Tile lists
		public static readonly int[] m_MountainAndCaveTiles = new[]
		{
			220, 221, 222, 223, 224, 225, 226, 227, 228, 229, 230, 231, 236, 237, 238, 239, 240, 241, 242, 243, 244, 245, 246,
			247, 252, 253, 254, 255, 256, 257, 258, 259, 260, 261, 262, 263, 268, 269, 270, 271, 272, 273, 274, 275, 276, 277,
			278, 279, 286, 287, 288, 289, 290, 291, 292, 293, 294, 296, 296, 297, 321, 322, 323, 324, 467, 468, 469, 470, 471,
			472, 473, 474, 476, 477, 478, 479, 480, 481, 482, 483, 484, 485, 486, 487, 492, 493, 494, 495, 543, 544, 545, 546,
			547, 548, 549, 550, 551, 552, 553, 554, 555, 556, 557, 558, 559, 560, 561, 562, 563, 564, 565, 566, 567, 568, 569,
			570, 571, 572, 573, 574, 575, 576, 577, 578, 579, 581, 582, 583, 584, 585, 586, 587, 588, 589, 590, 591, 592, 593,
			594, 595, 596, 597, 598, 599, 600, 601, 610, 611, 612, 613, 1010, 1741, 1742, 1743, 1744, 1745, 1746, 1747, 1748,
			1749, 1750, 1751, 1752, 1753, 1754, 1755, 1756, 1757, 1771, 1772, 1773, 1774, 1775, 1776, 1777, 1778, 1779, 1780,
			1781, 1782, 1783, 1784, 1785, 1786, 1787, 1788, 1789, 1790, 1801, 1802, 1803, 1804, 1805, 1806, 1807, 1808, 1809,
			1811, 1812, 1813, 1814, 1815, 1816, 1817, 1818, 1819, 1820, 1821, 1822, 1823, 1824, 1831, 1832, 1833, 1834, 1835,
			1836, 1837, 1838, 1839, 1840, 1841, 1842, 1843, 1844, 1845, 1846, 1847, 1848, 1849, 1850, 1851, 1852, 1853, 1854,
			1861, 1862, 1863, 1864, 1865, 1866, 1867, 1868, 1869, 1870, 1871, 1872, 1873, 1874, 1875, 1876, 1877, 1878, 1879,
			1880, 1881, 1882, 1883, 1884, 1981, 1982, 1983, 1984, 1985, 1986, 1987, 1988, 1989, 1990, 1991, 1992, 1993, 1994,
			1995, 1996, 1997, 1998, 1999, 2000, 2001, 2002, 2003, 2004, 2028, 2029, 2030, 2031, 2032, 2033, 2100, 2101, 2102,
			2103, 2104, 2105, 0x453B, 0x453C, 0x453D, 0x453E, 0x453F, 0x4540, 0x4541, 0x4542, 0x4543, 0x4544, 0x4545, 0x4546,
			0x4547, 0x4548, 0x4549, 0x454A, 0x454B, 0x454C, 0x454D, 0x454E, 0x454F, 0x48E0, 0x48E1, 0x48E2, 0x48E3, 0x48E4,
			0x48E5, 0x48E6, 0x48E7, 0x48E8, 0x48E9, 0x48EA, 0x47B7, 0x47B8, 0x47B9, 0x534F, 0x5350, 0x5351, 0x5350, 0x5351,
			0x5352, 0x5353, 0x5354, 0x5355, 0x5356, 0x5357, 0x5358, 0x5359, 0x535A, 0x535B, 0x535C, 0x535D, 0x535E, 0x535F,
			0x5360, 0x5361, 0x5362, 0x5363, 0x5364, 0x5366, 0x5367, 0x5368, 0x5369, 0x536A, 0x536B, 0x536C, 0x536D, 0x5771,
			0x5772, 0x5773, 0x5774, 0x5775, 0x5776, 0x5777, 0x5778, 0x5779, 0x577A, 0x577B, 0x577C, 0x7341, 0x7342, 0x7343,
			0x7344, 0x7345, 0x7346, 0x7347, 0x7348, 0x7349, 0x734A, 0x734B, 0x734C, 0x734D, 0x734E, 0x734F, 0x7350, 0x7351,
			0x742A, 0x742B, 0x7431, 0x7432, 0x7433, 0x7434, 0x7435, 0x7436, 0x7437, 0x7438, 0x7539, 0x753A, 0x753B, 0x753C
		};

		private static readonly int[] m_SandTiles = new[]
		{
			22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50,
			51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 68, 69, 70, 71, 72, 73, 74, 75, 286, 287, 288, 289, 290, 291, 292,
			293, 294, 295, 296, 297, 298, 299, 300, 301, 402, 424, 425, 426, 427, 441, 442, 443, 444, 445, 446, 447, 448, 449,
			450, 451, 452, 453, 454, 455, 456, 457, 458, 459, 460, 461, 462, 463, 464, 465, 642, 643, 644, 645, 650, 651, 652,
			653, 654, 655, 656, 657, 821, 822, 823, 824, 825, 826, 827, 828, 833, 834, 835, 836, 845, 846, 847, 848, 849, 850,
			851, 852, 857, 858, 859, 860, 951, 952, 953, 954, 955, 956, 957, 958, 967, 968, 969, 970, 1447, 1448, 1449, 1450,
			1451, 1452, 1453, 1454, 1455, 1456, 1457, 1458, 1611, 1612, 1613, 1614, 1615, 1616, 1617, 1618, 1623, 1624, 1625,
			1626, 1635, 1636, 1637, 1638, 1639, 1640, 1641, 1642, 1647, 1648, 1649, 1650
		};
		#endregion
	}
}