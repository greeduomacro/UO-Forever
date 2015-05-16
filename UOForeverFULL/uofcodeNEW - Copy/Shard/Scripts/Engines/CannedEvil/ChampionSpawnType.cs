#region References
using System;

using Server.Mobiles;
#endregion

namespace Server.Engines.CannedEvil
{
	public enum ChampionSpawnType
	{
		Abyss,
		Arachnid,
		ColdBlood,
		ForestLord,
		VerminHorde,
		UnholyTerror,
		SleepingDragon,
		Glade,
		Pestilence,
		Necro
	}

	public class ChampionSpawnInfo
	{
		private readonly string m_Name;
		private readonly Type m_Champion;
		private readonly Type[][] m_SpawnTypes;
		private readonly string[] m_LevelNames;

		public string Name { get { return m_Name; } }

		public Type Champion { get { return m_Champion; } }

		public Type[][] SpawnTypes { get { return m_SpawnTypes; } }

		public string[] LevelNames { get { return m_LevelNames; } }

		public ChampionSpawnInfo(string name, Type champion, string[] levelNames, Type[][] spawnTypes)
		{
			m_Name = name;
			m_Champion = champion;
			m_LevelNames = levelNames;
			m_SpawnTypes = spawnTypes;
		}

		public static ChampionSpawnInfo[] Table { get { return m_Table; } }

		private static readonly ChampionSpawnInfo[] m_Table =
		{
			new ChampionSpawnInfo(
				"Abyss",
				typeof(Semidar),
				new[] {"Foe", "Assassin", "Conqueror"},
				new[] // Abyss
				{
					// Abyss
					new[] {typeof(GreaterMongbat), typeof(Imp)}, // Level 1
					new[] {typeof(Gargoyle), typeof(Harpy)}, // Level 2
					new[] {typeof(FireGargoyle), typeof(StoneGargoyle)}, // Level 3
					new[] {typeof(Daemon), typeof(Succubus)} // Level 4
				}),
			new ChampionSpawnInfo(
				"Arachnid",
				typeof(Mephitis),
				new[] {"Bane", "Killer", "Vanquisher"},
				new[] // Arachnid
				{
					// Arachnid
					new[] {typeof(Scorpion), typeof(GiantSpider)}, // Level 1
					new[] {typeof(TerathanDrone), typeof(TerathanWarrior)}, // Level 2
					new[] {typeof(DreadSpider), typeof(TerathanMatriarch)}, // Level 3
					new[] {typeof(PoisonElemental), typeof(TerathanAvenger)} // Level 4
				}),
			new ChampionSpawnInfo(
				"Cold Blood",
				typeof(Rikktor),
				new[] {"Blight", "Slayer", "Destroyer"},
				new[] // Cold Blood
				{
					// Cold Blood
					new[] {typeof(Lizardman), typeof(GiantSerpent)}, // Level 1
					new[] {typeof(LavaLizard), typeof(OphidianWarrior)}, // Level 2
					new[] {typeof(Drake), typeof(OphidianArchmage)}, // Level 3
					new[] {typeof(Dragon), typeof(OphidianKnight)} // Level 4
				}),
			new ChampionSpawnInfo(
				"Forest Lord",
				typeof(LordOaks),
				new[] {"Enemy", "Curse", "Slaughterer"},
				new[] // Forest Lord
				{
					// Forest Lord
					new[] {typeof(Pixie), typeof(ShadowWisp)}, // Level 1
					new[] {typeof(Kirin), typeof(Wisp)}, // Level 2
					new[] {typeof(Centaur), typeof(Unicorn)}, // Level 3
					new[] {typeof(EtherealWarrior), typeof(SerpentineDragon)} // Level 4
				}),
			new ChampionSpawnInfo(
				"Vermin Horde",
				typeof(Barracoon),
				new[] {"Adversary", "Subjugator", "Eradicator"},
				new[] // Vermin Horde
				{
					// Vermin Horde
					new[] {typeof(GiantRat), typeof(Slime)}, // Level 1
					new[] {typeof(DireWolf), typeof(RatmanArcher)}, // Level 2
					new[] {typeof(HellHound), typeof(RatmanMage)}, // Level 3
					new[] {typeof(OgreLord), typeof(Titan), typeof(SilverSerpent)} // Level 4
				}),
			new ChampionSpawnInfo(
				"Unholy Terror",
				typeof(Neira),
				new[] {"Scourge", "Punisher", "Nemesis"},
				new[] // Unholy Terror
				{
					// Unholy Terror
					new[] {typeof(Ghoul), typeof(Shade), typeof(Spectre), typeof(Wraith)}, // Level 1
					new[] {typeof(BoneMage), typeof(Mummy), typeof(SkeletalMage)}, // Level 2
					new[] {typeof(BoneKnight), typeof(Lich), typeof(SkeletalKnight)}, // Level 3
					new[] {typeof(LichLord), typeof(RottingCorpse)} // Level 4
				}),
			new ChampionSpawnInfo(
				"Sleeping Dragon",
				typeof(Serado),
				new[] {"Rival", "Challenger", "Antagonist"},
				new[]
				{
					// Sleeping Dragon
					new[] {typeof(DeathwatchBeetleHatchling), typeof(Lizardman)}, // Level 1
					new[] {typeof(DeathwatchBeetle), typeof(Kappa)}, // Level 2
					new[] {typeof(LesserHiryu), typeof(RevenantLion)}, // Level 3
					new[] {typeof(Hiryu), typeof(Oni)} // Level 4
				}),
			new ChampionSpawnInfo(
				"Glade",
				typeof(Twaulo),
				new[] {"Banisher", "Enforcer", "Eradicator"},
				new[]
				{
					// Glade
					new[] {typeof(Pixie), typeof(ShadowWisp)}, // Level 1
					new[] {typeof(Centaur), typeof(MLDryad)}, // Level 2
					new[] {typeof(Satyr), typeof(CuSidhe)}, // Level 3
					new[] {typeof(FerelTreefellow), typeof(RagingGrizzlyBear)} // Level 4
				}),
			new ChampionSpawnInfo(
				"The Corrupt",
				typeof(Ilhenir),
				new[] {"Cleanser", "Expunger", "Depurator"},
				new[]
				{
					// Unholy Terror
					new[] {typeof(PlagueSpawn), typeof(Bogling)}, // Level 1
					new[] {typeof(PlagueBeast), typeof(BogThing)}, // Level 2
					new[] {typeof(PlagueBeastLord), typeof(InterredGrizzle)}, // Level 3
					new[] {typeof(FetidEssence), typeof(PestilentBandage)} // Level 4
				}),
			new ChampionSpawnInfo(
				"Necro",
				typeof(CorpseDevourer),
				new[] {"Defiler", "Vilifier", "Inquisitor"},
				new[]
				{
					// Corpse Devourer (need to replace these types with new types)
					new[] {typeof(InfernalCreeper), typeof(Shade), typeof(Spectre), typeof(Wraith)}, // Level 1
					new[] {typeof(BoneMage), typeof(DamnedSoul), typeof(SkeletalMage)}, // Level 2
					new[] {typeof(BoneKnight), typeof(Lich), typeof(SkeletalKnight)}, // Level 3
					new[] {typeof(LichLord), typeof(RottingCorpse)} // Level 4
				})
		};

		public static ChampionSpawnInfo GetInfo(ChampionSpawnType type)
		{
			var v = (int)type;

			return m_Table.InBounds(v) ? m_Table[v] : null;
		}
	}
}