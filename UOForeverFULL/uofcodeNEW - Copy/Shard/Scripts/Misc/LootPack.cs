#region References
using System;
using System.Linq;

using Server.Games;
using Server.Items;
using Server.Mobiles;
#endregion

namespace Server
{
	public class LootPack
	{
		public static bool CheckLuck(int chance)
		{
			return (chance > Utility.Random(10000));
		}

		private readonly LootPackEntry[] m_Entries;

		public LootPack(LootPackEntry[] entries)
		{
			m_Entries = entries;
		}

		public void Generate(Mobile m, Container c, bool spawning)
		{
			if (c == null)
			{
				return;
			}

			foreach (Item i in
				m_Entries.Where(e => e.Chance > Utility.Random(10000))
						 .Select(entry => entry.Construct(m, spawning))
						 .Where(i => i != null)
						 .Where(i => !i.Stackable || (m == null || !c.TryDropItem(m, i, false))))
			{
				c.DropItem(i);
			}
		}

		public void ForceGenerate(Mobile m, Container c)
		{
			if (c == null)
			{
				return;
			}

			foreach (Item i in
				m_Entries.Where(e => e.Chance > Utility.Random(10000))
						 .Select(entry => entry.ForceConstruct(m))
						 .Where(i => i != null)
						 .Where(i => !i.Stackable || (m == null || !c.TryDropItem(m, i, false))))
			{
				c.DropItem(i);
			}
		}

		public static readonly LootPackItem[] Gold = new[] {new LootPackItem(typeof(Gold), 1)};

		public static readonly LootPackItem[] Instruments = new[] {new LootPackItem(typeof(BaseInstrument), 1)};

		public static readonly LootPackItem[] LowScrollItems = new[] {new LootPackItem(typeof(ClumsyScroll), 1)};

		public static readonly LootPackItem[] MedScrollItems = new[] {new LootPackItem(typeof(ArchCureScroll), 1)};

		public static readonly LootPackItem[] HighScrollItems = new[] {new LootPackItem(typeof(SummonAirElementalScroll), 1)};

		public static readonly LootPackItem[] GemItems = new[] {new LootPackItem(typeof(Amber), 1)};

		public static readonly LootPackItem[] PotionItems = new[]
		{
			new LootPackItem(typeof(AgilityPotion), 1), new LootPackItem(typeof(StrengthPotion), 1),
			new LootPackItem(typeof(RefreshPotion), 1), new LootPackItem(typeof(LesserCurePotion), 1),
			new LootPackItem(typeof(LesserHealPotion), 1), new LootPackItem(typeof(LesserPoisonPotion), 1)
		};

		public static readonly LootPackItem[] WandItems = new[]
		{
			new LootPackItem(typeof(ClumsyWand), 1), new LootPackItem(typeof(GreaterHealWand), 1),
			new LootPackItem(typeof(IDWand), 1), new LootPackItem(typeof(ManaDrainWand), 1),
			new LootPackItem(typeof(FeebleWand), 1), new LootPackItem(typeof(HarmWand), 1),
			new LootPackItem(typeof(LightningWand), 1), new LootPackItem(typeof(WeaknessWand), 1),
			new LootPackItem(typeof(FireballWand), 1), new LootPackItem(typeof(HealWand), 1),
			new LootPackItem(typeof(MagicArrowWand), 1)
		};

		public static readonly LootPackItem[] FoodItems = new[] {new LootPackItem(typeof(Food), 1)};

		public static readonly LootPackItem[] MeatItems = new[] {new LootPackItem(typeof(Ribs), 1)};

		public static readonly LootPackItem[] FruitsAndVeggieItems = new[] {new LootPackItem(typeof(Apple), 1)};

		public static readonly LootPackItem[] GrainsAndHayItems = new[] {new LootPackItem(typeof(SheafOfHay), 1)};

		public static readonly LootPackItem[] FishItems = new[] {new LootPackItem(typeof(RawFishSteak), 1)};

		public static readonly LootPackItem[] EggItems = new[] {new LootPackItem(typeof(Eggs), 1)};

		public static readonly LootPackItem[] ProvisionItems = new[] {new LootPackItem(typeof(Candle), 1)};

		#region Old Magic Items
		public static readonly LootPackItem[] OldMagicItems = new[]
		{
			new LootPackItem(typeof(BaseJewel), 2), new LootPackItem(typeof(BaseArmor), 15),
			new LootPackItem(typeof(BaseWeapon), 15), new LootPackItem(typeof(BaseRanged), 5),
			new LootPackItem(typeof(BaseShield), 10), new LootPackItem(typeof(IDWand), 2)
		};
		#endregion

		#region AOS Magic Items
		public static readonly LootPackItem[] AosMagicItemsPoor = new[]
		{
			new LootPackItem(typeof(BaseWeapon), 23), new LootPackItem(typeof(BaseRanged), 21),
			new LootPackItem(typeof(BaseArmor), 24), new LootPackItem(typeof(BaseShield), 21),
			new LootPackItem(typeof(BaseJewel), 22)
		};

		public static readonly LootPackItem[] AosMagicItemsMeagerType1 = new[]
		{
			new LootPackItem(typeof(BaseWeapon), 66), new LootPackItem(typeof(BaseRanged), 24),
			new LootPackItem(typeof(BaseArmor), 91), new LootPackItem(typeof(BaseShield), 21),
			new LootPackItem(typeof(BaseJewel), 52)
		};

		public static readonly LootPackItem[] AosMagicItemsMeagerType2 = new[]
		{
			new LootPackItem(typeof(BaseWeapon), 38), new LootPackItem(typeof(BaseRanged), 17),
			new LootPackItem(typeof(BaseArmor), 50), new LootPackItem(typeof(BaseShield), 15),
			new LootPackItem(typeof(BaseJewel), 21)
		};

		public static readonly LootPackItem[] AosMagicItemsAverageType1 = new[]
		{
			new LootPackItem(typeof(BaseWeapon), 100), new LootPackItem(typeof(BaseRanged), 33),
			new LootPackItem(typeof(BaseArmor), 140), new LootPackItem(typeof(BaseShield), 27),
			new LootPackItem(typeof(BaseJewel), 78)
		};

		public static readonly LootPackItem[] AosMagicItemsAverageType2 = new[]
		{
			new LootPackItem(typeof(BaseWeapon), 64), new LootPackItem(typeof(BaseRanged), 23),
			new LootPackItem(typeof(BaseArmor), 87), new LootPackItem(typeof(BaseShield), 20),
			new LootPackItem(typeof(BaseJewel), 50)
		};

		public static readonly LootPackItem[] AosMagicItemsRichType1 = new[]
		{
			new LootPackItem(typeof(BaseWeapon), 221), new LootPackItem(typeof(BaseRanged), 63),
			new LootPackItem(typeof(BaseArmor), 313), new LootPackItem(typeof(BaseShield), 49),
			new LootPackItem(typeof(BaseJewel), 168)
		};

		public static readonly LootPackItem[] AosMagicItemsRichType2 = new[]
		{
			new LootPackItem(typeof(BaseWeapon), 180), new LootPackItem(typeof(BaseRanged), 83),
			new LootPackItem(typeof(BaseArmor), 255), new LootPackItem(typeof(BaseShield), 42),
			new LootPackItem(typeof(BaseJewel), 148)
		};

		public static readonly LootPackItem[] AosMagicItemsFilthyRichType1 = new[]
		{
			new LootPackItem(typeof(BaseWeapon), 229), new LootPackItem(typeof(BaseRanged), 65),
			new LootPackItem(typeof(BaseArmor), 325), new LootPackItem(typeof(BaseShield), 51),
			new LootPackItem(typeof(BaseJewel), 174)
		};

		public static readonly LootPackItem[] AosMagicItemsFilthyRichType2 = new[]
		{
			new LootPackItem(typeof(BaseWeapon), 249), new LootPackItem(typeof(BaseRanged), 70),
			new LootPackItem(typeof(BaseArmor), 353), new LootPackItem(typeof(BaseShield), 100),
			new LootPackItem(typeof(BaseJewel), 55)
		};

		public static readonly LootPackItem[] AosMagicItemsUltraRich = new[]
		{
			new LootPackItem(typeof(BaseWeapon), 286), new LootPackItem(typeof(BaseRanged), 79),
			new LootPackItem(typeof(BaseArmor), 407), new LootPackItem(typeof(BaseShield), 62),
			new LootPackItem(typeof(BaseJewel), 217)
		};
		#endregion

		#region ML definitions
		public static readonly LootPack MlRich =
			new LootPack(
				new[]
				{
					new LootPackEntry(true, Gold, 100.00, "4d50+450"),
					new LootPackEntry(false, AosMagicItemsRichType1, 100.00, 1, 3, 0, 75),
					new LootPackEntry(false, AosMagicItemsRichType1, 80.00, 1, 3, 0, 75),
					new LootPackEntry(false, AosMagicItemsRichType1, 60.00, 1, 5, 0, 100),
					new LootPackEntry(false, Instruments, 1.00, 1)
				});
		#endregion

		#region SE definitions
		public static readonly LootPack SePoor =
			new LootPack(
				new[]
				{
					new LootPackEntry(true, Gold, 100.00, "2d10+20"), new LootPackEntry(false, AosMagicItemsPoor, 1.00, 1, 5, 0, 100),
					new LootPackEntry(false, Instruments, 0.02, 1)
				});

		public static readonly LootPack SeMeager =
			new LootPack(
				new[]
				{
					new LootPackEntry(true, Gold, 100.00, "4d10+40"),
					new LootPackEntry(false, AosMagicItemsMeagerType1, 20.40, 1, 2, 0, 50),
					new LootPackEntry(false, AosMagicItemsMeagerType2, 10.20, 1, 5, 0, 100),
					new LootPackEntry(false, Instruments, 0.10, 1)
				});

		public static readonly LootPack SeAverage =
			new LootPack(
				new[]
				{
					new LootPackEntry(true, Gold, 100.00, "8d10+60"),
					new LootPackEntry(false, AosMagicItemsAverageType1, 32.80, 1, 3, 0, 50),
					new LootPackEntry(false, AosMagicItemsAverageType1, 32.80, 1, 4, 0, 75),
					new LootPackEntry(false, AosMagicItemsAverageType2, 19.50, 1, 5, 0, 100),
					new LootPackEntry(false, Instruments, 0.40, 1)
				});

		public static readonly LootPack SeRich =
			new LootPack(
				new[]
				{
					new LootPackEntry(true, Gold, 100.00, "15d10+225"),
					new LootPackEntry(false, AosMagicItemsRichType1, 76.30, 1, 4, 0, 75),
					new LootPackEntry(false, AosMagicItemsRichType1, 76.30, 1, 4, 0, 75),
					new LootPackEntry(false, AosMagicItemsRichType2, 61.70, 1, 5, 0, 100),
					new LootPackEntry(false, Instruments, 1.00, 1)
				});

		public static readonly LootPack SeFilthyRich =
			new LootPack(
				new[]
				{
					new LootPackEntry(true, Gold, 100.00, "3d100+400"),
					new LootPackEntry(false, AosMagicItemsFilthyRichType1, 79.50, 1, 5, 0, 100),
					new LootPackEntry(false, AosMagicItemsFilthyRichType1, 79.50, 1, 5, 0, 100),
					new LootPackEntry(false, AosMagicItemsFilthyRichType2, 77.60, 1, 5, 25, 100),
					new LootPackEntry(false, Instruments, 2.00, 1)
				});

		public static readonly LootPack SeUltraRich =
			new LootPack(
				new[]
				{
					new LootPackEntry(true, Gold, 100.00, "6d100+600"),
					new LootPackEntry(false, AosMagicItemsUltraRich, 100.00, 1, 5, 25, 100),
					new LootPackEntry(false, AosMagicItemsUltraRich, 100.00, 1, 5, 25, 100),
					new LootPackEntry(false, AosMagicItemsUltraRich, 100.00, 1, 5, 25, 100),
					new LootPackEntry(false, AosMagicItemsUltraRich, 100.00, 1, 5, 25, 100),
					new LootPackEntry(false, AosMagicItemsUltraRich, 100.00, 1, 5, 25, 100),
					new LootPackEntry(false, AosMagicItemsUltraRich, 100.00, 1, 5, 33, 100),
					new LootPackEntry(false, Instruments, 2.00, 1)
				});

		public static readonly LootPack SeSuperBoss =
			new LootPack(
				new[]
				{
					new LootPackEntry(true, Gold, 100.00, "10d100+800"),
					new LootPackEntry(false, AosMagicItemsUltraRich, 100.00, 1, 5, 25, 100),
					new LootPackEntry(false, AosMagicItemsUltraRich, 100.00, 1, 5, 25, 100),
					new LootPackEntry(false, AosMagicItemsUltraRich, 100.00, 1, 5, 25, 100),
					new LootPackEntry(false, AosMagicItemsUltraRich, 100.00, 1, 5, 25, 100),
					new LootPackEntry(false, AosMagicItemsUltraRich, 100.00, 1, 5, 33, 100),
					new LootPackEntry(false, AosMagicItemsUltraRich, 100.00, 1, 5, 33, 100),
					new LootPackEntry(false, AosMagicItemsUltraRich, 100.00, 1, 5, 33, 100),
					new LootPackEntry(false, AosMagicItemsUltraRich, 100.00, 1, 5, 33, 100),
					new LootPackEntry(false, AosMagicItemsUltraRich, 100.00, 1, 5, 50, 100),
					new LootPackEntry(false, AosMagicItemsUltraRich, 100.00, 1, 5, 50, 100),
					new LootPackEntry(false, Instruments, 2.00, 1)
				});
		#endregion

		#region AOS definitions
		public static readonly LootPack AosPoor =
			new LootPack(
				new[]
				{
					new LootPackEntry(true, Gold, 100.00, "1d10+10"), new LootPackEntry(false, AosMagicItemsPoor, 0.02, 1, 5, 0, 90),
					new LootPackEntry(false, Instruments, 0.02, 1)
				});

		public static readonly LootPack AosMeager =
			new LootPack(
				new[]
				{
					new LootPackEntry(true, Gold, 100.00, "3d10+20"),
					new LootPackEntry(false, AosMagicItemsMeagerType1, 1.00, 1, 2, 0, 10),
					new LootPackEntry(false, AosMagicItemsMeagerType2, 0.20, 1, 5, 0, 90),
					new LootPackEntry(false, Instruments, 0.10, 1)
				});

		public static readonly LootPack AosAverage =
			new LootPack(
				new[]
				{
					new LootPackEntry(true, Gold, 100.00, "5d10+50"),
					new LootPackEntry(false, AosMagicItemsAverageType1, 5.00, 1, 4, 0, 20),
					new LootPackEntry(false, AosMagicItemsAverageType1, 2.00, 1, 3, 0, 50),
					new LootPackEntry(false, AosMagicItemsAverageType2, 0.50, 1, 5, 0, 90),
					new LootPackEntry(false, Instruments, 0.40, 1)
				});

		public static readonly LootPack AosRich =
			new LootPack(
				new[]
				{
					new LootPackEntry(true, Gold, 100.00, "10d10+150"),
					new LootPackEntry(false, AosMagicItemsRichType1, 20.00, 1, 4, 0, 40),
					new LootPackEntry(false, AosMagicItemsRichType1, 10.00, 1, 5, 0, 60),
					new LootPackEntry(false, AosMagicItemsRichType2, 1.00, 1, 5, 0, 90), new LootPackEntry(false, Instruments, 1.00, 1)
				});

		public static readonly LootPack AosFilthyRich =
			new LootPack(
				new[]
				{
					new LootPackEntry(true, Gold, 100.00, "2d100+200"),
					new LootPackEntry(false, AosMagicItemsFilthyRichType1, 33.00, 1, 4, 0, 50),
					new LootPackEntry(false, AosMagicItemsFilthyRichType1, 33.00, 1, 4, 0, 60),
					new LootPackEntry(false, AosMagicItemsFilthyRichType2, 20.00, 1, 5, 0, 75),
					new LootPackEntry(false, AosMagicItemsFilthyRichType2, 5.00, 1, 5, 0, 100),
					new LootPackEntry(false, Instruments, 2.00, 1)
				});

		public static readonly LootPack AosUltraRich =
			new LootPack(
				new[]
				{
					new LootPackEntry(true, Gold, 100.00, "5d100+500"),
					new LootPackEntry(false, AosMagicItemsUltraRich, 100.00, 1, 5, 25, 100),
					new LootPackEntry(false, AosMagicItemsUltraRich, 100.00, 1, 5, 25, 100),
					new LootPackEntry(false, AosMagicItemsUltraRich, 100.00, 1, 5, 25, 100),
					new LootPackEntry(false, AosMagicItemsUltraRich, 100.00, 1, 5, 25, 100),
					new LootPackEntry(false, AosMagicItemsUltraRich, 100.00, 1, 5, 25, 100),
					new LootPackEntry(false, AosMagicItemsUltraRich, 100.00, 1, 5, 35, 100),
					new LootPackEntry(false, Instruments, 2.00, 1)
				});

		public static readonly LootPack AosSuperBoss =
			new LootPack(
				new[]
				{
					new LootPackEntry(true, Gold, 100.00, "5d100+500"),
					new LootPackEntry(false, AosMagicItemsUltraRich, 100.00, 1, 5, 25, 100),
					new LootPackEntry(false, AosMagicItemsUltraRich, 100.00, 1, 5, 25, 100),
					new LootPackEntry(false, AosMagicItemsUltraRich, 100.00, 1, 5, 25, 100),
					new LootPackEntry(false, AosMagicItemsUltraRich, 100.00, 1, 5, 25, 100),
					new LootPackEntry(false, AosMagicItemsUltraRich, 100.00, 1, 5, 33, 100),
					new LootPackEntry(false, AosMagicItemsUltraRich, 100.00, 1, 5, 33, 100),
					new LootPackEntry(false, AosMagicItemsUltraRich, 100.00, 1, 5, 33, 100),
					new LootPackEntry(false, AosMagicItemsUltraRich, 100.00, 1, 5, 33, 100),
					new LootPackEntry(false, AosMagicItemsUltraRich, 100.00, 1, 5, 50, 100),
					new LootPackEntry(false, AosMagicItemsUltraRich, 100.00, 1, 5, 50, 100),
					new LootPackEntry(false, Instruments, 2.00, 1)
				});
		#endregion

		#region Pre-AOS definitions
		public static readonly LootPack PoorProvisions =
			new LootPack(
				new[]
				{
					new LootPackEntry(false, ProvisionItems, 100.0, 4), new LootPackEntry(false, ProvisionItems, 75.0, 2),
					new LootPackEntry(false, FoodItems, 100.0, 4), new LootPackEntry(false, ProvisionItems, 50.0, 2),
					new LootPackEntry(false, ProvisionItems, 25.0, 2), new LootPackEntry(false, ProvisionItems, 10.0, 1),
					new LootPackEntry(false, FoodItems, 50.0, 2), new LootPackEntry(false, FoodItems, 25.0, 2),
					new LootPackEntry(false, FoodItems, 10.0, 2)
				});

		public static readonly LootPack MeagerProvisions =
			new LootPack(
				new[]
				{
					new LootPackEntry(false, ProvisionItems, 100.0, 6), new LootPackEntry(false, ProvisionItems, 75.0, 3),
					new LootPackEntry(false, FoodItems, 100.0, 4), new LootPackEntry(false, ProvisionItems, 50.0, 2),
					new LootPackEntry(false, ProvisionItems, 50.0, 2), new LootPackEntry(false, ProvisionItems, 25.0, 2),
					new LootPackEntry(false, ProvisionItems, 25.0, 2), new LootPackEntry(false, ProvisionItems, 10.0, 2),
					new LootPackEntry(false, FoodItems, 50.0, 3), new LootPackEntry(false, FoodItems, 25.0, 3),
					new LootPackEntry(false, FoodItems, 10.0, 3)
				});

		public static readonly LootPack AverageProvisions =
			new LootPack(
				new[]
				{
					new LootPackEntry(false, ProvisionItems, 100.0, 6), new LootPackEntry(false, ProvisionItems, 75.0, 3),
					new LootPackEntry(false, ProvisionItems, 75.0, 2), new LootPackEntry(false, FoodItems, 100.0, 6),
					new LootPackEntry(false, FoodItems, 75.0, 3), new LootPackEntry(false, ProvisionItems, 50.0, 3),
					new LootPackEntry(false, ProvisionItems, 50.0, 3), new LootPackEntry(false, ProvisionItems, 25.0, 3),
					new LootPackEntry(false, ProvisionItems, 25.0, 3), new LootPackEntry(false, ProvisionItems, 10.0, 2),
					new LootPackEntry(false, FoodItems, 50.0, 5), new LootPackEntry(false, FoodItems, 25.0, 5),
					new LootPackEntry(false, FoodItems, 10.0, 5)
				});

		public static readonly LootPack RichProvisions =
			new LootPack(
				new[]
				{
					new LootPackEntry(false, ProvisionItems, 100.0, 6), new LootPackEntry(false, ProvisionItems, 75.0, 5),
					new LootPackEntry(false, ProvisionItems, 75.0, 4), new LootPackEntry(false, FoodItems, 100.0, 10),
					new LootPackEntry(false, FoodItems, 75.0, 8), new LootPackEntry(false, ProvisionItems, 50.0, 4),
					new LootPackEntry(false, ProvisionItems, 50.0, 4), new LootPackEntry(false, ProvisionItems, 25.0, 5),
					new LootPackEntry(false, ProvisionItems, 25.0, 4), new LootPackEntry(false, ProvisionItems, 10.0, 3),
					new LootPackEntry(false, FoodItems, 50.0, 6), new LootPackEntry(false, FoodItems, 25.0, 6),
					new LootPackEntry(false, FoodItems, 10.0, 5)
				});

		public static readonly LootPack OldPoor =
			new LootPack(
				new[] {new LootPackEntry(true, Gold, 100.00, "2d10+10"), new LootPackEntry(false, Instruments, 0.02, 1)});

		public static readonly LootPack OldMeager =
			new LootPack(
				new[]
				{
					new LootPackEntry(true, Gold, 100.00, "5d15+25"), new LootPackEntry(false, Instruments, 0.10, 1),
					new LootPackEntry(false, OldMagicItems, 1.00, 1, 1, 0, 60),
					new LootPackEntry(false, OldMagicItems, 0.20, 1, 1, 10, 70)
				});

		public static readonly LootPack OldAverage =
			new LootPack(
				new[]
				{
					new LootPackEntry(true, Gold, 100.00, "10d10+50"), new LootPackEntry(false, Instruments, 0.40, 1),
					new LootPackEntry(false, OldMagicItems, 5.00, 1, 1, 20, 80),
					new LootPackEntry(false, OldMagicItems, 2.00, 1, 1, 30, 90),
					new LootPackEntry(false, OldMagicItems, 0.50, 1, 1, 40, 100)
				});

		public static readonly LootPack OldRich =
			new LootPack(
				new[]
				{
					new LootPackEntry(true, Gold, 100.00, "10d10+250"), new LootPackEntry(false, Instruments, 1.00, 1),
					new LootPackEntry(false, OldMagicItems, 20.00, 1, 1, 40, 100),
					new LootPackEntry(false, OldMagicItems, 10.00, 1, 1, 50, 100)
				});

		public static readonly LootPack OldFilthyRich =
			new LootPack(
				new[]
				{
					new LootPackEntry(true, Gold, 100.00, "2d125+400"), new LootPackEntry(false, Instruments, 2.00, 1),
					new LootPackEntry(false, OldMagicItems, 33.00, 1, 1, 50, 100),
					new LootPackEntry(false, OldMagicItems, 5.00, 1, 1, 20, 80),
					new LootPackEntry(false, OldMagicItems, 2.00, 1, 1, 30, 90)
				});

		public static readonly LootPack OldUltraRich =
			new LootPack(
				new[]
				{
					new LootPackEntry(true, Gold, 100.00, "5d100+500"), new LootPackEntry(false, Instruments, 2.00, 1),
					new LootPackEntry(false, OldMagicItems, 100.00, 1, 1, 30, 100),
					new LootPackEntry(false, OldMagicItems, 100.00, 1, 1, 40, 100),
					new LootPackEntry(false, OldMagicItems, 100.00, 1, 1, 50, 100),
					new LootPackEntry(false, OldMagicItems, 100.00, 1, 1, 55, 100),
					new LootPackEntry(false, OldMagicItems, 100.00, 1, 1, 60, 100)
				});

		public static readonly LootPack OldSuperBoss =
			new LootPack(
				new[]
				{
					new LootPackEntry(true, Gold, 100.00, "50d10+500"), new LootPackEntry(false, Instruments, 2.00, 1),
					new LootPackEntry(false, OldMagicItems, 100.00, 1, 1, 30, 100),
					new LootPackEntry(false, OldMagicItems, 100.00, 1, 1, 30, 100),
					new LootPackEntry(false, OldMagicItems, 100.00, 1, 1, 30, 100),
					new LootPackEntry(false, OldMagicItems, 100.00, 1, 1, 30, 100),
					new LootPackEntry(false, OldMagicItems, 100.00, 1, 1, 40, 100),
					new LootPackEntry(false, OldMagicItems, 100.00, 1, 1, 40, 100),
					new LootPackEntry(false, OldMagicItems, 100.00, 1, 1, 50, 100)
				});
		#endregion

		#region Generic accessors
		public static LootPack Poor { get { return OldPoor; } }
		public static LootPack Meager { get { return OldMeager; } }
		public static LootPack Average { get { return OldAverage; } }
		public static LootPack Rich { get { return OldRich; } }
		public static LootPack FilthyRich { get { return OldFilthyRich; } }
		public static LootPack UltraRich { get { return OldUltraRich; } }
		public static LootPack SuperBoss { get { return OldSuperBoss; } }
		#endregion

		public static readonly LootPack LowScrolls = new LootPack(new[] {new LootPackEntry(false, LowScrollItems, 100.00, 1)});

		public static readonly LootPack MedScrolls = new LootPack(new[] {new LootPackEntry(false, MedScrollItems, 100.00, 1)});

		public static readonly LootPack HighScrolls =
			new LootPack(new[] {new LootPackEntry(false, HighScrollItems, 100.00, 1)});

		public static readonly LootPack Gems = new LootPack(new[] {new LootPackEntry(false, GemItems, 100.00, 1)});

		public static readonly LootPack Potions = new LootPack(new[] {new LootPackEntry(false, PotionItems, 100.00, 1)});

		public static readonly LootPack Wands = new LootPack(new[] {new LootPackEntry(false, WandItems, 100.00, 1)});
	}

	public class LootPackEntry
	{
		private readonly bool m_AtSpawnTime;

		public int Chance { get; set; }
		public LootPackDice Quantity { get; set; }
		public int MaxProps { get; set; }
		public int MinIntensity { get; set; }
		public int MaxIntensity { get; set; }
		public LootPackItem[] Items { get; set; }

		private static bool IsInTokuno(Mobile m)
		{
			if (m == null)
			{
				return false;
			}

			if (m.Region.IsPartOf("Fan Dancer's Dojo"))
			{
				return true;
			}

			if (m.Region.IsPartOf("Yomotsu Mines"))
			{
				return true;
			}

			return (m.Map == Map.Tokuno);
		}

		#region Mondain's Legacy
		private static bool IsMondain(Mobile m)
		{
			return m != null && MondainsLegacy.IsMLRegion(m.Region);
		}
		#endregion

		public Item Construct(Mobile from, bool spawning)
		{
			if (m_AtSpawnTime != spawning)
			{
				return null;
			}

			int totalChance = Items.Sum(t => t.Chance);

			int rnd = Utility.Random(totalChance);

			foreach (LootPackItem item in Items)
			{
				if (rnd < item.Chance)
				{
					return Mutate(from, item.Construct(IsInTokuno(from), IsMondain(from), from.Expansion));
				}

				rnd -= item.Chance;
			}

			return null;
		}

		public Item ForceConstruct(Mobile from)
		{
			int totalChance = Items.Sum(t => t.Chance);

			int rnd = Utility.Random(totalChance);

			foreach (LootPackItem item in Items)
			{
				if (rnd < item.Chance)
				{
					return Mutate(from, item.Construct(IsInTokuno(from), IsMondain(from), from.Expansion));
				}

				rnd -= item.Chance;
			}

			return null;
		}

		private int GetRandomOldBonus()
		{
			int rnd = Utility.RandomMinMax(MinIntensity, MaxIntensity);

			if (50 > rnd)
			{
				return 1;
			}

			rnd -= 50;

			if (25 > rnd)
			{
				return 2;
			}
			rnd -= 25;

			if (14 > rnd)
			{
				return 3;
			}

			rnd -= 14;

			if (8 > rnd)
			{
				return 4;
			}

			return 5;
		}

		public Item Mutate(Mobile from, Item item)
		{           

			if (item != null && !(item is BaseWand))
			{
				if (item is BaseWeapon && 1 > Utility.Random(100))
				{
					item.Delete();
					item = new FireHorn();
					return item;
				}

				if (item is BaseWeapon || item is BaseArmor || item is BaseJewel || item is BaseHat)
				{
					if (item is BaseWeapon)
					{
						var weapon = (BaseWeapon)item;

						if (55 > Utility.Random(100))
						{
							weapon.AccuracyLevel = (WeaponAccuracyLevel)GetRandomOldBonus();
						}

						if (45 > Utility.Random(100))
						{
							int damageLevel = GetRandomOldBonus();

							if (PseudoSeerStone.Instance != null && PseudoSeerStone.Instance._HighestDamageLevelSpawn < damageLevel)
							{
								if (damageLevel == 5 && PseudoSeerStone.ReplaceVanqWithSkillScrolls)
								{
									return PuzzleChest.CreateRandomSkillScroll();
								}
								int platAmount = PseudoSeerStone.PlatinumPerMissedDamageLevel *
												 (damageLevel - PseudoSeerStone.Instance._HighestDamageLevelSpawn);
								if (platAmount > 0)
								{
									return (new Platinum(platAmount));
								}
								damageLevel = PseudoSeerStone.Instance._HighestDamageLevelSpawn;
							}
							weapon.DamageLevel = (WeaponDamageLevel)damageLevel;
						}

						if (25 > Utility.Random(100))
						{
							weapon.DurabilityLevel = (WeaponDurabilityLevel)GetRandomOldBonus();
						}

						if (5 > Utility.Random(100))
						{
							weapon.Slayer = SlayerName.Silver;
						}

						if (1 > Utility.Random(1000) ||
							(weapon.AccuracyLevel == 0 && weapon.DamageLevel == 0 && weapon.DurabilityLevel == 0 &&
							 weapon.Slayer == SlayerName.None && 5 > Utility.Random(100)))
						{
							weapon.Slayer = from != null ? SlayerGroup.GetLootSlayerType(from.GetType()) : BaseRunicTool.GetRandomSlayer();
						}

						if (weapon.AccuracyLevel == 0 && weapon.DamageLevel == 0 && weapon.DurabilityLevel == 0 &&
							weapon.Slayer == SlayerName.None)
						{
							weapon.Identified = true;
						}
					}
					else if (item is BaseArmor)
					{
						var armor = (BaseArmor)item;

						if (55 > Utility.Random(100))
						{
							armor.ProtectionLevel = (ArmorProtectionLevel)GetRandomOldBonus();
						}

						if (25 > Utility.Random(100))
						{
							armor.Durability = (ArmorDurabilityLevel)GetRandomOldBonus();
						}

						if (armor.ProtectionLevel == 0 && armor.Durability == 0)
						{
							armor.Identified = true;
						}
					}
				}
				else if (item is BaseInstrument)
				{
					SlayerName slayer = from == null || from.EraAOS
											? BaseRunicTool.GetRandomSlayer()
											: SlayerGroup.GetLootSlayerType(from.GetType());

					var instr = (BaseInstrument)item;

					instr.Quality = InstrumentQuality.Regular;
					instr.Slayer = slayer;
				}
				else if (item is Spellbook) //Randomize spellbook
				{
					var book = item as Spellbook;

					if (MaxIntensity == 100 && MinIntensity / 1000.0 > Utility.RandomDouble())
					{
						book.LootType = LootType.Blessed;
					}

					if (MaxIntensity == 100 && MinIntensity >= 50 && (MinIntensity / 3000.0 > Utility.RandomDouble()))
					{
						book.Dyable = true;
					}

					int rnd = Utility.RandomMinMax(MinIntensity, MaxIntensity);
					var circle = (int)((rnd / 12.5) + 1.0);

					if (circle >= 8 && 0.33 > Utility.RandomDouble())
					{
						book.Content = ulong.MaxValue;
					}
					else
					{
						circle = Math.Min(circle, 8);

						//do we fill this circle?
						for (int i = 0; i < circle; i++)
						{
							if (Utility.RandomBool())
							{
								book.Content |= (ulong)Utility.Random(0x100) << (i * 8);
							}
						}
					}
				}

				if (item.Stackable)
				{
                    // Note: do not check hits max here if you want to multiply against gold
                    // the max hits have not been set when this function is called
                    // The inital loot is added to the BaseCreature before the attributes are set
                    // for the specific mob type
				    if (item is Gold)
				    {
				        item.Amount = (int) Math.Ceiling(Quantity.Roll() * DynamicSettingsController.GoldMulti);                        
				    }
				    else
				    {
                        item.Amount = Quantity.Roll();
				    }
				}
			}

			return item;
		}

		public LootPackEntry(bool atSpawnTime, LootPackItem[] items, double chance, string quantity)
			: this(atSpawnTime, items, chance, new LootPackDice(quantity), 0, 0, 0)
		{ }

		public LootPackEntry(bool atSpawnTime, LootPackItem[] items, double chance, int quantity)
			: this(atSpawnTime, items, chance, new LootPackDice(0, 0, quantity), 0, 0, 0)
		{ }

		public LootPackEntry(
			bool atSpawnTime,
			LootPackItem[] items,
			double chance,
			string quantity,
			int maxProps,
			int minIntensity,
			int maxIntensity)
			: this(atSpawnTime, items, chance, new LootPackDice(quantity), maxProps, minIntensity, maxIntensity)
		{ }

		public LootPackEntry(
			bool atSpawnTime, LootPackItem[] items, double chance, int quantity, int maxProps, int minIntensity, int maxIntensity)
			: this(atSpawnTime, items, chance, new LootPackDice(0, 0, quantity), maxProps, minIntensity, maxIntensity)
		{ }

		public LootPackEntry(
			bool atSpawnTime,
			LootPackItem[] items,
			double chance,
			LootPackDice quantity,
			int maxProps,
			int minIntensity,
			int maxIntensity)
		{
			m_AtSpawnTime = atSpawnTime;
			Items = items;
			Chance = (int)(100 * chance);
			Quantity = quantity;
			MaxProps = maxProps;
			MinIntensity = minIntensity;
			MaxIntensity = maxIntensity;
		}

		public int GetBonusProperties()
		{
			int p0 = 0, p1 = 0, p2 = 0, p3 = 0, p4 = 0, p5 = 0;

			switch (MaxProps)
			{
				case 1:
					p0 = 3;
					p1 = 1;
					break;
				case 2:
					p0 = 6;
					p1 = 3;
					p2 = 1;
					break;
				case 3:
					p0 = 10;
					p1 = 6;
					p2 = 3;
					p3 = 1;
					break;
				case 4:
					p0 = 16;
					p1 = 12;
					p2 = 6;
					p3 = 5;
					p4 = 1;
					break;
				case 5:
					p0 = 30;
					p1 = 25;
					p2 = 20;
					p3 = 15;
					p4 = 9;
					p5 = 1;
					break;
			}

			int pc = p0 + p1 + p2 + p3 + p4 + p5;

			int rnd = Utility.Random(pc);

			if (rnd < p5)
			{
				return 5;
			}

			rnd -= p5;

			if (rnd < p4)
			{
				return 4;
			}

			rnd -= p4;

			if (rnd < p3)
			{
				return 3;
			}

			rnd -= p3;

			if (rnd < p2)
			{
				return 2;
			}

			rnd -= p2;

			if (rnd < p1)
			{
				return 1;
			}

			return 0;
		}
	}

	public class LootPackItem
	{
		private Type m_Type;

		public Type Type { get { return m_Type; } set { m_Type = value; } }

		public int Chance { get; set; }

		private static readonly Type[] m_BlankTypes = new[] {typeof(BlankScroll)};

		public static Item RandomScroll(int index, int minCircle, int maxCircle)
		{
			--minCircle;
			--maxCircle;

			int scrollCount = ((maxCircle - minCircle) + 1) * 8;

			if (index == 0)
			{
				scrollCount += m_BlankTypes.Length;
			}

			int rnd = Utility.Random(scrollCount);

			if (index == 0 && rnd < m_BlankTypes.Length)
			{
				return Loot.Construct(m_BlankTypes);
			}

			return Loot.RandomScroll(minCircle * 8, (maxCircle * 8) + 7, SpellbookType.Regular);
		}

		public Item Construct(bool inTokuno, bool isMondain, Expansion e)
		{
			try
			{
				Item item;

				if (m_Type == typeof(BaseRanged))
				{
					item = Loot.RandomRangedWeapon();
				}
				else if (m_Type == typeof(BaseWeapon))
				{
					item = Loot.RandomWeapon();
				}
				else if (m_Type == typeof(BaseArmor))
				{
					item = Loot.RandomArmorOrHat();
				}
				else if (m_Type == typeof(BaseShield))
				{
					item = Loot.RandomShield();
				}
				else if (m_Type == typeof(BaseJewel))
				{
					item = e >= Expansion.AOS ? Loot.RandomJewelry() : Loot.RandomArmorOrShieldOrWeapon();
				}
				else if (m_Type == typeof(BaseInstrument))
				{
					item = Loot.RandomInstrument(e >= Expansion.SE);
				}
				else if (m_Type == typeof(BaseWand))
				{
					item = Loot.RandomWand();
				}
				else if (m_Type == typeof(Amber)) // gem
				{
					item = Loot.RandomGem();
				}
				else if (m_Type == typeof(ClumsyScroll)) // low scroll
				{
					item = RandomScroll(0, 1, 3);
				}
				else if (m_Type == typeof(ArchCureScroll)) // med scroll
				{
					item = RandomScroll(1, 4, 7);
				}
				else if (m_Type == typeof(SummonAirElementalScroll)) // high scroll
				{
					item = RandomScroll(2, 8, 8);
				}
				else if (m_Type == typeof(Ribs)) //meats
				{
					item = Loot.Construct(BaseCreature.FoodTypeFromPreference(FoodType.Meat));
				}
				else if (m_Type == typeof(Apple)) //fruits/vegies
				{
					item = Loot.Construct(BaseCreature.FoodTypeFromPreference(FoodType.FruitsAndVeggies));
				}
				else if (m_Type == typeof(SheafOfHay)) //Hay
				{
					item = Loot.Construct(BaseCreature.FoodTypeFromPreference(FoodType.GrainsAndHay));
				}
				else if (m_Type == typeof(RawFishSteak)) //Fish
				{
					item = Loot.Construct(BaseCreature.FoodTypeFromPreference(FoodType.Fish));
				}
				else if (m_Type == typeof(Eggs)) //Eggs
				{
					item = Loot.Construct(BaseCreature.FoodTypeFromPreference(FoodType.Eggs));
				}
				else if (m_Type == typeof(Food)) //Food items
				{
					item = Loot.RandomFood();
				}
				else if (m_Type == typeof(Candle)) //Provisions
				{
					item = Loot.RandomProvision();
				}
				else
				{
					item = m_Type.CreateInstance<Item>();
				}

				return item;
			}
			catch
			{ }

			return null;
		}

		public LootPackItem(Type type, int chance)
		{
			m_Type = type;
			Chance = chance;
		}
	}

	public class LootPackDice
	{
		public int Count { get; set; }
		public int Sides { get; set; }
		public int Bonus { get; set; }

		public int Roll()
		{
			int v = Bonus;

			for (int i = 0; i < Count; ++i)
			{
				v += Utility.Random(1, Sides);
			}

			return v;
		}

		public LootPackDice(string str)
		{
			int start = 0;
			int index = str.IndexOf('d', start);

			if (index < start)
			{
				return;
			}

			Count = Utility.ToInt32(str.Substring(start, index - start));

			start = index + 1;
			index = str.IndexOf('+', start);

			bool negative = index < start;

			if (negative)
			{
				index = str.IndexOf('-', start);
			}

			if (index < start)
			{
				index = str.Length;
			}

			Sides = Utility.ToInt32(str.Substring(start, index - start));

			if (index == str.Length)
			{
				return;
			}

			start = index + 1;
			index = str.Length;

			Bonus = Utility.ToInt32(str.Substring(start, index - start));

			if (negative)
			{
				Bonus *= -1;
			}
		}

		public LootPackDice(int count, int sides, int bonus)
		{
			Count = count;
			Sides = sides;
			Bonus = bonus;
		}
	}
}