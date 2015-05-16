#region References
using System;
using System.Linq;

using Server.Items;
#endregion

namespace Server
{
	public class Loot
	{
		#region List definitions

		#region Mondain's Legacy
		public static Type[] MLWeaponTypes { get; private set; }
		public static Type[] MLRangedWeaponTypes { get; private set; }
		public static Type[] MLArmorTypes { get; private set; }
		public static Type[] MLClothingTypes { get; private set; }
		#endregion

		public static Type[] SEWeaponTypes { get; private set; }
		public static Type[] AosWeaponTypes { get; private set; }
		public static Type[] WeaponTypes { get; private set; }
		public static Type[] SERangedWeaponTypes { get; private set; }
		public static Type[] AosRangedWeaponTypes { get; private set; }
		public static Type[] RangedWeaponTypes { get; private set; }
		public static Type[] SEArmorTypes { get; private set; }
		public static Type[] ArmorTypes { get; private set; }
		public static Type[] AosShieldTypes { get; private set; }
		public static Type[] ShieldTypes { get; private set; }
		public static Type[] GemTypes { get; private set; }
		public static Type[] JewelryTypes { get; private set; }
		public static Type[] RegTypes { get; private set; }
		public static Type[] NecroRegTypes { get; private set; }
		public static Type[] PotionTypes { get; private set; }
		public static Type[] SEInstrumentTypes { get; private set; }
		public static Type[] InstrumentTypes { get; private set; }
		public static Type[] StatueTypes { get; private set; }
		public static Type[] RegularScrollTypes { get; private set; }
		public static Type[] GrimmochJournalTypes { get; private set; }
		public static Type[] LysanderNotebookTypes { get; private set; }
		public static Type[] TavarasJournalTypes { get; private set; }
		public static Type[] WandTypes { get; private set; }
		public static Type[] OldWandTypes { get; private set; }
		public static Type[] SEClothingTypes { get; private set; }
		public static Type[] AosClothingTypes { get; private set; }
		public static Type[] ClothingTypes { get; private set; }
		public static Type[] SEHatTypes { get; private set; }
		public static Type[] AosHatTypes { get; private set; }
		public static Type[] HatTypes { get; private set; }
		public static Type[] LibraryBookTypes { get; private set; }
		public static Type[] FoodTypes { get; private set; }
		public static Type[] StackFoodTypes { get; private set; }
		public static Type[] ProvisionTypes { get; private set; }

		static Loot()
		{
			ProvisionTypes = new[]
			{
				typeof(Arrow), typeof(Bolt), typeof(Candle), typeof(Torch), typeof(Kindling), typeof(Lantern), typeof(Bandage),
				typeof(Lockpick), typeof(Bandana), typeof(Beeswax), typeof(Bottle), typeof(RedBook), typeof(BlueBook),
				typeof(TanBook), typeof(Key), typeof(Dices), typeof(Necklace), typeof(Beads), typeof(Knife), typeof(Spoon),
				typeof(Fork), typeof(Hammer), typeof(Nails), typeof(BlankScroll), typeof(Saw), typeof(Shovel), typeof(Shaft),
				typeof(Scissors), typeof(MortarPestle), typeof(Tongs), typeof(SewingKit), typeof(Sextant), typeof(FishingPole),
				typeof(Skillet), typeof(ShortPants), typeof(LongPants), typeof(Cloak), typeof(FloppyHat), typeof(WideBrimHat),
				typeof(Cap), typeof(SkullCap), typeof(TallStrawHat), typeof(StrawHat), typeof(WizardsHat), typeof(Bonnet),
				typeof(FeatheredHat), typeof(TricorneHat), typeof(JesterHat), typeof(BodySash), typeof(FullApron), typeof(Doublet),
				typeof(Surcoat), typeof(Tunic), typeof(JesterSuit), typeof(Skirt), typeof(Kilt), typeof(FancyDress), typeof(Robe),
				typeof(PlainDress), typeof(HalfApron), typeof(Boots), typeof(ThighBoots), typeof(Shoes)
			};

			StackFoodTypes = new[]
			{
				typeof(BreadLoaf), typeof(Bacon), typeof(FishSteak), typeof(CheeseWheel), typeof(CheeseWedge), typeof(FrenchBread),
				typeof(FriedEggs), typeof(CookedBird), typeof(RoastPig), typeof(Sausage), typeof(Ham), typeof(Ribs),
				typeof(CheesePizza), typeof(SausagePizza), typeof(Peach), typeof(HoneydewMelon), typeof(YellowGourd),
				typeof(GreenGourd), typeof(Banana), typeof(SplitCoconut), typeof(Lemon), typeof(Lime), typeof(Coconut),
				typeof(Dates), typeof(Grapes), typeof(Pear), typeof(Apple), typeof(Watermelon), typeof(Squash), typeof(Cantaloupe),
				typeof(Carrot), typeof(Cabbage), typeof(Onion), typeof(Lettuce), typeof(Pumpkin), typeof(LambLeg),
				typeof(ChickenLeg), typeof(Bananas)
			};

			FoodTypes = new[]
			{
				typeof(Cake), typeof(Cookies), typeof(Muffins), typeof(FruitPie), typeof(MeatPie), typeof(PumpkinPie),
				typeof(ApplePie), typeof(PeachCobbler), typeof(Quiche)
			};

			LibraryBookTypes = new[]
			{
				typeof(GrammarOfOrcish), typeof(CallToAnarchy), typeof(ArmsAndWeaponsPrimer), typeof(SongOfSamlethe),
				typeof(TaleOfThreeTribes), typeof(GuideToGuilds), typeof(BirdsOfBritannia), typeof(BritannianFlora),
				typeof(ChildrenTalesVol2), typeof(TalesOfVesperVol1), typeof(DeceitDungeonOfHorror), typeof(DimensionalTravel),
				typeof(EthicalHedonism), typeof(MyStory), typeof(DiversityOfOurLand), typeof(QuestOfVirtues),
				typeof(RegardingLlamas), typeof(TalkingToWisps), typeof(TamingDragons), typeof(BoldStranger),
				typeof(BurningOfTrinsic), typeof(TheFight), typeof(LifeOfATravellingMinstrel), typeof(MajorTradeAssociation),
				typeof(RankingsOfTrades), typeof(WildGirlOfTheForest), typeof(TreatiseOnAlchemy), typeof(VirtueBook)
			};

			HatTypes = new[]
			{
				typeof(SkullCap), typeof(Bandana), typeof(FloppyHat), typeof(Cap), typeof(WideBrimHat), typeof(StrawHat),
				typeof(TallStrawHat), typeof(WizardsHat), typeof(Bonnet), typeof(FeatheredHat), typeof(TricorneHat),
				typeof(JesterHat)
			};

			AosHatTypes = new[]
			{
				typeof(FlowerGarland), typeof(BearMask), typeof(DeerMask) //Are Bear& Deer mask inside the Pre-AoS loottables too?
			};

			SEHatTypes = new[] {typeof(ClothNinjaHood), typeof(Kasa)};

			ClothingTypes = new[]
			{
				typeof(Cloak), typeof(Bonnet), typeof(Cap), typeof(FeatheredHat), typeof(FloppyHat), typeof(JesterHat),
				typeof(Surcoat), typeof(SkullCap), typeof(StrawHat), typeof(TallStrawHat), typeof(TricorneHat), typeof(WideBrimHat),
				typeof(WizardsHat), typeof(BodySash), typeof(Doublet), typeof(Boots), typeof(FullApron), typeof(JesterSuit),
				typeof(Sandals), typeof(Tunic), typeof(Shoes), typeof(Shirt), typeof(Kilt), typeof(Skirt), typeof(FancyShirt),
				typeof(FancyDress), typeof(ThighBoots), typeof(LongPants), typeof(PlainDress), typeof(Robe), typeof(ShortPants),
				typeof(HalfApron)
			};

			AosClothingTypes = new[]
			{
				typeof(FurSarong), typeof(FurCape), typeof(FlowerGarland), typeof(GildedDress), typeof(FurBoots),
				typeof(FormalShirt)
			};

			SEClothingTypes = new[]
			{
				typeof(ClothNinjaJacket), typeof(FemaleKimono), typeof(Hakama), typeof(HakamaShita), typeof(JinBaori),
				typeof(Kamishimo), typeof(MaleKimono), typeof(NinjaTabi), typeof(Obi), typeof(SamuraiTabi), typeof(TattsukeHakama),
				typeof(Waraji)
			};

			OldWandTypes = new[] {typeof(IDWand)};

			WandTypes = new[]
			{
				typeof(ClumsyWand), typeof(FeebleWand), typeof(FireballWand), typeof(GreaterHealWand), typeof(HarmWand),
				typeof(HealWand), typeof(LightningWand), typeof(MagicArrowWand), typeof(ManaDrainWand), typeof(WeaknessWand)
			};

			TavarasJournalTypes = new[]
			{
				typeof(TavarasJournal1), typeof(TavarasJournal2), typeof(TavarasJournal3), typeof(TavarasJournal6),
				typeof(TavarasJournal7), typeof(TavarasJournal8), typeof(TavarasJournal9), typeof(TavarasJournal11),
				typeof(TavarasJournal14), typeof(TavarasJournal16), typeof(TavarasJournal16b), typeof(TavarasJournal17),
				typeof(TavarasJournal19)
			};

			LysanderNotebookTypes = new[]
			{
				typeof(LysanderNotebook1), typeof(LysanderNotebook2), typeof(LysanderNotebook3), typeof(LysanderNotebook7),
				typeof(LysanderNotebook8), typeof(LysanderNotebook11)
			};

			GrimmochJournalTypes = new[]
			{
				typeof(GrimmochJournal1), typeof(GrimmochJournal2), typeof(GrimmochJournal3), typeof(GrimmochJournal6),
				typeof(GrimmochJournal7), typeof(GrimmochJournal11), typeof(GrimmochJournal14), typeof(GrimmochJournal17),
				typeof(GrimmochJournal23)
			};

			RegularScrollTypes = new[]
			{
				typeof(ReactiveArmorScroll), typeof(ClumsyScroll), typeof(CreateFoodScroll), typeof(FeeblemindScroll),
				typeof(HealScroll), typeof(MagicArrowScroll), typeof(NightSightScroll), typeof(WeakenScroll), typeof(AgilityScroll),
				typeof(CunningScroll), typeof(CureScroll), typeof(HarmScroll), typeof(MagicTrapScroll), typeof(MagicUnTrapScroll),
				typeof(ProtectionScroll), typeof(StrengthScroll), typeof(BlessScroll), typeof(FireballScroll),
				typeof(MagicLockScroll), typeof(PoisonScroll), typeof(TelekinesisScroll), typeof(TeleportScroll),
				typeof(UnlockScroll), typeof(WallOfStoneScroll), typeof(ArchCureScroll), typeof(ArchProtectionScroll),
				typeof(CurseScroll), typeof(FireFieldScroll), typeof(GreaterHealScroll), typeof(LightningScroll),
				typeof(ManaDrainScroll), typeof(RecallScroll), typeof(BladeSpiritsScroll), typeof(DispelFieldScroll),
				typeof(IncognitoScroll), typeof(MagicReflectScroll), typeof(MindBlastScroll), typeof(ParalyzeScroll),
				typeof(PoisonFieldScroll), typeof(SummonCreatureScroll), typeof(DispelScroll), typeof(EnergyBoltScroll),
				typeof(ExplosionScroll), typeof(InvisibilityScroll), typeof(MarkScroll), typeof(MassCurseScroll),
				typeof(ParalyzeFieldScroll), typeof(RevealScroll), typeof(ChainLightningScroll), typeof(EnergyFieldScroll),
				typeof(FlamestrikeScroll), typeof(GateTravelScroll), typeof(ManaVampireScroll), typeof(MassDispelScroll),
				typeof(MeteorSwarmScroll), typeof(PolymorphScroll), typeof(EarthquakeScroll), typeof(EnergyVortexScroll),
				typeof(ResurrectionScroll), typeof(SummonAirElementalScroll), typeof(SummonDaemonScroll),
				typeof(SummonEarthElementalScroll), typeof(SummonFireElementalScroll), typeof(SummonWaterElementalScroll)
			};

			StatueTypes = new[]
			{
				typeof(StatueSouth), typeof(StatueSouth2), typeof(StatueNorth), typeof(StatueWest), typeof(StatueEast),
				typeof(StatueEast2), typeof(StatueSouthEast), typeof(BustSouth), typeof(BustEast)
			};

			InstrumentTypes = new[]
			{typeof(Drums), typeof(Harp), typeof(LapHarp), typeof(Lute), typeof(Tambourine), typeof(TambourineTassel)};

			SEInstrumentTypes = new[] {typeof(BambooFlute)};

			PotionTypes = new[]
			{
				typeof(AgilityPotion), typeof(StrengthPotion), typeof(RefreshPotion), typeof(LesserCurePotion),
				typeof(LesserHealPotion), typeof(LesserPoisonPotion)
			};

			NecroRegTypes = new[] {typeof(BatWing), typeof(GraveDust), typeof(DaemonBlood), typeof(NoxCrystal), typeof(PigIron)};

			RegTypes = new[]
			{
				typeof(BlackPearl), typeof(Bloodmoss), typeof(Garlic), typeof(Ginseng), typeof(MandrakeRoot), typeof(Nightshade),
				typeof(SulfurousAsh), typeof(SpidersSilk)
			};

			JewelryTypes = new[] {typeof(GoldRing), typeof(GoldBracelet), typeof(SilverRing), typeof(SilverBracelet)};

			GemTypes = new[]
			{
				typeof(Amber), typeof(Amethyst), typeof(Citrine), typeof(Diamond), typeof(Emerald), typeof(Ruby), typeof(Sapphire),
				typeof(StarSapphire), typeof(Tourmaline)
			};

			ShieldTypes = new[]
			{
				typeof(BronzeShield), typeof(Buckler), typeof(HeaterShield), typeof(MetalShield), typeof(MetalKiteShield),
				typeof(WoodenKiteShield), typeof(WoodenShield)
			};

			AosShieldTypes = new[] {typeof(ChaosShield), typeof(OrderShield)};

			SEArmorTypes = new[]
			{
				typeof(ChainHatsuburi), typeof(LeatherDo), typeof(LeatherHaidate), typeof(LeatherHiroSode), typeof(LeatherJingasa),
				typeof(LeatherMempo), typeof(LeatherNinjaHood), typeof(LeatherNinjaJacket), typeof(LeatherNinjaMitts),
				typeof(LeatherNinjaPants), typeof(LeatherSuneate), typeof(DecorativePlateKabuto), typeof(HeavyPlateJingasa),
				typeof(LightPlateJingasa), typeof(PlateBattleKabuto), typeof(PlateDo), typeof(PlateHaidate), typeof(PlateHatsuburi),
				typeof(PlateHiroSode), typeof(PlateMempo), typeof(PlateSuneate), typeof(SmallPlateJingasa),
				typeof(StandardPlateKabuto), typeof(StuddedDo), typeof(StuddedHaidate), typeof(StuddedHiroSode),
				typeof(StuddedMempo), typeof(StuddedSuneate)
			};

			ArmorTypes = new[]
			{
				typeof(BoneArms), typeof(BoneChest), typeof(BoneGloves), typeof(BoneLegs), typeof(BoneHelm), typeof(ChainChest),
				typeof(ChainLegs), typeof(ChainCoif), typeof(Bascinet), typeof(CloseHelm), typeof(Helmet), typeof(NorseHelm),
				typeof(OrcHelm), typeof(FemaleLeatherChest), typeof(LeatherArms), typeof(LeatherBustierArms), typeof(LeatherChest),
				typeof(LeatherGloves), typeof(LeatherGorget), typeof(LeatherLegs), typeof(LeatherShorts), typeof(LeatherSkirt),
				typeof(LeatherCap), typeof(FemalePlateChest), typeof(PlateArms), typeof(PlateChest), typeof(PlateGloves),
				typeof(PlateGorget), typeof(PlateHelm), typeof(PlateLegs), typeof(RingmailArms), typeof(RingmailChest),
				typeof(RingmailGloves), typeof(RingmailLegs), typeof(FemaleStuddedChest), typeof(StuddedArms),
				typeof(StuddedBustierArms), typeof(StuddedChest), typeof(StuddedGloves), typeof(StuddedGorget), typeof(StuddedLegs)
			};

			RangedWeaponTypes = new[] {typeof(Bow), typeof(Crossbow), typeof(HeavyCrossbow)};

			AosRangedWeaponTypes = new[] {typeof(CompositeBow), typeof(RepeatingCrossbow)};

			SERangedWeaponTypes = new[] {typeof(Yumi)};

			WeaponTypes = new[]
			{
				typeof(Axe), typeof(BattleAxe), typeof(DoubleAxe), typeof(ExecutionersAxe), typeof(Hatchet), typeof(LargeBattleAxe),
				typeof(TwoHandedAxe), typeof(WarAxe), typeof(Club), typeof(Mace), typeof(Maul), typeof(WarHammer), typeof(WarMace),
				typeof(Bardiche), typeof(Halberd), typeof(Spear), typeof(ShortSpear), typeof(Pitchfork), typeof(WarFork),
				typeof(BlackStaff), typeof(GnarledStaff), typeof(QuarterStaff), typeof(Broadsword), typeof(Cutlass), typeof(Katana),
				typeof(Kryss), typeof(Longsword), typeof(Scimitar), typeof(VikingSword), typeof(Pickaxe), typeof(HammerPick),
				typeof(ButcherKnife), typeof(Cleaver), typeof(Dagger), typeof(SkinningKnife), typeof(ShepherdsCrook)
			};

			AosWeaponTypes = new[]
			{
				typeof(Scythe), typeof(BoneHarvester), typeof(Scepter), typeof(BladedStaff), typeof(Pike), typeof(DoubleBladedStaff)
				, typeof(Lance), typeof(CrescentBlade)
			};

			SEWeaponTypes = new[]
			{
				typeof(Bokuto), typeof(Daisho), typeof(Kama), typeof(Lajatang), typeof(NoDachi), typeof(Nunchaku), typeof(Sai),
				typeof(Tekagi), typeof(Tessen), typeof(Tetsubo), typeof(Wakizashi)
			};

			MLClothingTypes = new[]
			{
				typeof(MaleElvenRobe), typeof(FemaleElvenRobe), typeof(ElvenPants), typeof(ElvenShirt), typeof(ElvenDarkShirt),
				typeof(ElvenBoots), typeof(VultureHelm), typeof(WoodlandBelt)
			};

			MLArmorTypes = new[]
			{
				typeof(Circlet), typeof(GemmedCirclet), typeof(LeafTonlet), typeof(RavenHelm), typeof(RoyalCirclet),
				typeof(VultureHelm), typeof(WingedHelm), typeof(LeafArms), typeof(LeafChest), typeof(LeafGloves), typeof(LeafGorget)
				, typeof(LeafLegs), typeof(WoodlandArms), typeof(WoodlandChest), typeof(WoodlandGloves), typeof(WoodlandGorget),
				typeof(WoodlandLegs), typeof(HideChest), typeof(HideGloves), typeof(HideGorget), typeof(HidePants),
				typeof(HidePauldrons)
			};

			MLRangedWeaponTypes = new[] {typeof(ElvenCompositeLongbow), typeof(MagicalShortbow)};

			MLWeaponTypes = new[]
			{
				typeof(AssassinSpike), typeof(DiamondMace), typeof(ElvenMachete), typeof(ElvenSpellblade), typeof(Leafblade),
				typeof(OrnateAxe), typeof(RadiantScimitar), typeof(RuneBlade), typeof(WarCleaver), typeof(WildStaff)
			};
		}
		#endregion

		#region Accessors
		public static BaseWand RandomWand()
		{
			return Construct(WandTypes) as BaseWand;
		}

		public static BaseClothing RandomClothing()
		{
			return Construct(ClothingTypes) as BaseClothing;
		}

		public static BaseWeapon RandomRangedWeapon()
		{
			return Construct(RangedWeaponTypes) as BaseWeapon;
		}

		public static BaseWeapon RandomWeapon()
		{
			return Construct(WeaponTypes) as BaseWeapon;
		}

		public static Item RandomWeaponOrJewelry()
		{
			return Construct(WeaponTypes, JewelryTypes);
		}

		public static BaseJewel RandomJewelry()
		{
			return Construct(JewelryTypes) as BaseJewel;
		}

		public static BaseArmor RandomArmor()
		{
			return Construct(ArmorTypes) as BaseArmor;
		}

		public static BaseHat RandomHat()
		{
			return Construct(HatTypes) as BaseHat;
		}

		public static Item RandomArmorOrHat()
		{
			return Construct(ArmorTypes, HatTypes);
		}

		public static BaseShield RandomShield()
		{
			return Construct(ShieldTypes) as BaseShield;
		}

		public static BaseArmor RandomArmorOrShield()
		{
			return Construct(ArmorTypes, ShieldTypes) as BaseArmor;
		}

		public static Item RandomArmorOrShieldOrJewelry()
		{
			return Construct(ArmorTypes, HatTypes, ShieldTypes, JewelryTypes);
		}

		public static Item RandomArmorOrShieldOrWeapon()
		{
			return Construct(WeaponTypes, RangedWeaponTypes, ArmorTypes, HatTypes, ShieldTypes);
		}

		public static Item RandomArmorOrShieldOrWeaponOrJewelry()
		{
			return Construct(WeaponTypes, RangedWeaponTypes, ArmorTypes, HatTypes, ShieldTypes, JewelryTypes);
		}

		#region Chest of Heirlooms
		public static Item ChestOfHeirloomsContains()
		{
			return Construct(SEArmorTypes, SEHatTypes, SEWeaponTypes, SERangedWeaponTypes, JewelryTypes);
		}
		#endregion

		public static Item RandomGem()
		{
			return Construct(GemTypes);
		}

		public static Item RandomReagent()
		{
			return Construct(RegTypes);
		}

		public static Item RandomNecromancyReagent()
		{
			return Construct(NecroRegTypes);
		}

		public static Item RandomPossibleReagent(Expansion e)
		{
			if (e >= Expansion.AOS)
			{
				return Construct(RegTypes, NecroRegTypes);
			}

			return Construct(RegTypes);
		}

		public static Item RandomPotion()
		{
			return Construct(PotionTypes);
		}

		public static Item RandomStackFood()
		{
			return Construct(StackFoodTypes);
		}

		public static Item RandomFood()
		{
			return Construct(FoodTypes, StackFoodTypes);
		}

		public static Item RandomProvision()
		{
			return Construct(ProvisionTypes);
		}

		public static BaseInstrument RandomInstrument(bool se)
		{
			if (se)
			{
				return Construct(InstrumentTypes, SEInstrumentTypes) as BaseInstrument;
			}

			return Construct(InstrumentTypes) as BaseInstrument;
		}

		public static Item RandomStatue()
		{
			return Construct(StatueTypes);
		}

		public static SpellScroll RandomScroll(int minIndex, int maxIndex, SpellbookType type)
		{
			Type[] types;

			switch (type)
			{
				default:
					//case SpellbookType.Regular:
					types = RegularScrollTypes;
					break;
			}

			return Construct(types, Utility.RandomMinMax(minIndex, maxIndex)) as SpellScroll;
		}

		public static Item RandomSpellbookItem()
		{
			double rand = Utility.Random(10000);

			if (500 > rand)
			{
				return new SpellbookBlessDeed();
			}

			if (3000 > rand)
			{
				return new RecipeScroll(1001);
			}

			return new Spellbook();
		}

		public static BaseBook RandomGrimmochJournal()
		{
			return Construct(GrimmochJournalTypes) as BaseBook;
		}

		public static BaseBook RandomLysanderNotebook()
		{
			return Construct(LysanderNotebookTypes) as BaseBook;
		}

		public static BaseBook RandomTavarasJournal()
		{
			return Construct(TavarasJournalTypes) as BaseBook;
		}

		public static BaseBook RandomLibraryBook()
		{
			return Construct(LibraryBookTypes) as BaseBook;
		}

		public static BaseTalisman RandomTalisman()
		{
			var talisman = new BaseTalisman(BaseTalisman.GetRandomItemID())
			{
				Summoner = BaseTalisman.GetRandomSummoner()
			};

			if (talisman.Summoner.IsEmpty)
			{
				talisman.Removal = BaseTalisman.GetRandomRemoval();

				if (talisman.Removal != TalismanRemoval.None)
				{
					talisman.MaxCharges = BaseTalisman.GetRandomCharges();
					talisman.MaxChargeTime = 1200;
				}
			}
			else
			{
				talisman.MaxCharges = Utility.RandomMinMax(10, 50);

				talisman.MaxChargeTime = talisman.Summoner.IsItem ? 60 : 1800;
			}

			talisman.Blessed = BaseTalisman.GetRandomBlessed();
			talisman.Slayer = BaseTalisman.GetRandomSlayer();
			talisman.Protection = BaseTalisman.GetRandomProtection();
			talisman.Killer = BaseTalisman.GetRandomKiller();
			talisman.Skill = BaseTalisman.GetRandomSkill();
			talisman.ExceptionalBonus = BaseTalisman.GetRandomExceptional();
			talisman.SuccessBonus = BaseTalisman.GetRandomSuccessful();
			talisman.Charges = talisman.MaxCharges;

			return talisman;
		}
		#endregion

		#region Construction methods
		public static Item Construct(Type type)
		{
			return type.CreateInstanceSafe<Item>();
		}

		public static Item Construct(Type[] types)
		{
			if (types.Length > 0)
			{
				return Construct(types, Utility.Random(types.Length));
			}

			return null;
		}

		public static Item Construct(Type[] types, int index)
		{
			if (index >= 0 && index < types.Length)
			{
				return Construct(types[index]);
			}

			return null;
		}

		public static Item Construct(params Type[][] types)
		{
			int totalLength = types.Sum(t => t.Length);

			if (totalLength > 0)
			{
				int index = Utility.Random(totalLength);

				foreach (Type[] t in types)
				{
					if (index >= 0 && index < t.Length)
					{
						return Construct(t[index]);
					}

					index -= t.Length;
				}
			}

			return null;
		}
		#endregion
	}
}