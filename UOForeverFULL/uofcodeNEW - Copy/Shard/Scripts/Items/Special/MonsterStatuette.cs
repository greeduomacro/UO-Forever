using System;
using Server;
using Server.Multis;
using Server.Gumps;
using Server.Items;
using Server.Network;
using Server.Engines.VeteranRewards;
using Server.Mobiles;
using System.Collections.Generic;
using Server.Commands;

namespace Server.Items
{
	
    
    public enum MonsterStatuetteType
	{
		Male,
		Female,
		Bear,
		GiantRat,
		Chicken,
		CorpserFrame,
		Demon,
		Deer,
		DogFrame,
		Dragon,
		EarthElemental,
		Ettin,
		Gargoyle_Old,
		Crocodile,
		GrizzlyBearFrame,
		Harpy,
		HorseFrame,
		Lizardman,
		Ogre,
		Orc_Old,
		PolarBear,
		RabbitFrame,
		Ratman_Old,
		ScorpionFrame,
		Serpent,
		Sheep,
		Skeleton_Old,
		Slime,
		Troll,
		WolfFrame,
		WoolySheep,
		Zombie_Old,
		AirElemental_Old,
		BirdFlying,
		BrownBull,
		DarkBrownBull,
		Dolphin,
		EagleFlying,
		FireElemental_Old,
		Gazer_Old,
		Gorilla,
		Llama,
		Cougar_Old,
		Lich_Old,
		Mongbat_Old,
		Reaper,
		SeaSerpent,
		DarkSnake,
		Spider,
		LightSnake,
		Walrus,
		Wisp,
		Pig,
		Panther_Old,
		Cow,
		Daemon_Old,
		DaemonDupe,
		NPCMale,
		NPCFemale,
		MountainGoat,
		Ghoul_Old,
		Headless_Old,
		WaterElemental_Old, //Skip
		ElfMale,
		ElfFemale,
		//Section II
		BlackBear,
		BigCat,
		Bird,
		Cat,
		Dog,
		Eagle,
		GrizzlyBear,
		GrayHorse_Old,
		Horse,
		BrownHorse_Old,
		Wolf_Old,
		Rat,
		TanHorse_Old,
		Rabbit,
		PackHorse,
		PackLlama, //Skip
		//Section III
		TerathanWarrior_Old,
		TerathanDrone_Old,
		TerathanMatriarch,
		Cyclops,
		Titan_Old,
		GiantToad,
		Bullfrog,
		Lizard,
		OphidianMage_Old,
		OphidianWarrior_Old,
		OphidianQueen_Old,
		DesertOstard_Old,
		FrenziedOstard_Old,
		ForestOstard_Old,
		FireSteed,
		BillyGoat,
		Centaur,
		Corpser,
		Cougar,
		Daemon,
		BlackGateDaemon,
		ElderDaemon,
		IceFiendDaemon,
		Hound,
		EtherealWarrior,
		EvilMage,
		EvilMageBoss,
		GiantFrog,
		Gargoyle,
		StoneGargoyle,
		Gazer,
		Efreet,
		Ghoul,
		SilverBackGorilla,
		GreatWorm,
		StoneHarpy,
		Headless,
		HellHound,
		HellCat,
		BrownHorse,
		GrayHorse,
		DarkBrownHorse,
		DarkSteed,
		Nightmare,
		SilverSteed,
		TanHorse,
		Imp,
		Kirin,
		KomodoDragon,
		Kraken,
		SnowLeopard,
		Lich,
		LichLord,
		Mongbat,
		Mummy,
		OgreLord,
		OphidianArchmage,
		OphidianKnight,
		OphidianMage,
		OphidianQueen,
		OphidianWarrior,
		Orc,
		OrcCaptain,
		OrcLord,
		OrcShaman,
		DesertOstard,
		ForestOstard,
		FrienziedOstard,
		Panther,
		Pixie,
		Ratman,
		RottingCorpse,
		GiantScorpion,
		Seahorse,
		Harrower,
		Skeleton,
		SkeletonKnight,
		SkeletonMage,
		GiantSnake,
		GiantIceSnake,
		GiantLavaSnake,
		SilverSerpent,
		BlackWidowSpider,
		DreadSpider,
		FrostSpider,
		GiantSpider,
		Ghost,
		SwampTentacles,
		TerathanDrone,
		TerathanAvenger,
		TerathanQueen,
		TerathanWarrior,
		Titan,
		Unicorn,
		Wolf,
		DireWolf,
		GrayWolf,
		SilverWolf,
		TimberWolf,
		Wyvern,
		Zombie,
		AcidElemental,
		AirElemental,
		BloodElemental,
		FireElemental,
		IceElemental,
		PoisonElemental,
		SnowElemental,
		WaterElemental,
		Adranath, //Skip
		Blackthorn,
		CaptainDasha,
		Dawn,
		Kabur,
		SolenQueen, //Skip
		SolenWarrior,
		SolenWorker,
		ArcaneDemon,
		Betrayer,
		Bogling,
		BogThing,
		ChaosDemon,
		Chariot,
		ExodusMinion,
		ExodusOverseer,
		DoppleGanger,
		MolochDemon, //Four-Arm Demon
		GiantBeetle,
		Golem,
		HordeMinion,
		Juggernaut,
		PlagueBeast,
		Quagmire,
		Ridgeback,
		SandVortex,
		SkeletalMount,
		Sphinx,
		SwampDragon,
		Swarm,
		PlagueBeastLord,
		MeerMage,
		MeerWarrior,
		JukaMage,
		JukaWarrior,
		CrystalElemental,
		Treefellow,
		SkitteringHopper,
		DevourerSouls,
		FleshGolem,
		GoreFiend,
		Impaler,
		Gibberling,
		BoneDaemon,
		PatchworkSkeleton,
		WailingBanshee,
		ShadowKnight,
		AbysmalHorror,
		DarknightCreeper,
		Ravager,
		FleshRenderer,
		WandererVoid,
		VampireBat,
		DemonKnight,
		MoundMaggots,
		BakeKitsune,
		Crane,
		DeathwatchBeetle,
		Raiju,
		FanDancer,
		Gaman,
		GouzanHa,
		Hiryu,
		Kappa,
		LadySnow,
		Oni,
		RevenantLion,
		RuneBeetle,
		TsukiWolf,
		Yamandon,
		YomotsuElder,
		YomotsuPriest,
		YomotsuWarrior,
		FireBeetle,
		ChiefParoxysmus,
		DreadHorn,
		LadyMelisande,
		MonstrousInterred,
		ShimmeringEffusion,
		TormentedMinotaur,
		Minotaur,
		Changeling,
		Hydra,
		Dryad,
		Troglodyte,
		Satyr,
		FetidEssence,
		MantraEffervescense,
		CorporealBrume,
		Chimera,
		CuSidhe,
		Squirrel,
		Farret,
		MinotaurBoss1,
		MinotaurBoss2,
		Parrot,
		MondainSteed,
		PhillipsWoodenSteed,
		RedDeath,
        None
	}

	public class MonsterStatuetteInfo
	{
		private string m_Name;
		private int m_ItemID;
		private int[] m_Sounds;

		public string Name{ get{ return m_Name; } }
		public int ItemID{ get{ return m_ItemID; } }
		public int[] Sounds{ get{ return m_Sounds; } }

		public MonsterStatuetteInfo( string name, int itemid, int baseSoundID )
		{
			m_Name = name;
			m_ItemID = itemid;
			if ( baseSoundID >= 0 )
				m_Sounds = new int[]{ baseSoundID, baseSoundID+1, baseSoundID+2,baseSoundID+3,baseSoundID+4 };
			else
				m_Sounds = new int[]{ -1, -1, -1, -1, -1 };
		}

		public MonsterStatuetteInfo( string name, int itemid, int[] sounds )
		{
			m_Name = name;
			m_ItemID = itemid;
			m_Sounds = sounds;
		}

		private static MonsterStatuetteInfo[] m_Table = new MonsterStatuetteInfo[]
		{
			new MonsterStatuetteInfo( "Male Human", 0x20CD, 340),
			new MonsterStatuetteInfo( "Female Human", 0x20CE, 331),
			new MonsterStatuetteInfo( "Bear", 0x20CF, 0xA3),
			new MonsterStatuetteInfo( "Giant Rat", 0x20D0, 0x188),
			new MonsterStatuetteInfo( "Chicken", 0x20D1, 0x6E),
			new MonsterStatuetteInfo( "Corpser", 0x20D2, 684),
			new MonsterStatuetteInfo( "Demon", 0x20D3, 357),
			new MonsterStatuetteInfo( "Deer", 0x20D4, 0x83),
			new MonsterStatuetteInfo( "Dog", 0x20D5, 0x85),
			new MonsterStatuetteInfo( "Dragon", 0x20D6, 362),
			new MonsterStatuetteInfo( "Earth Elemental", 0x20D7, 268),
			new MonsterStatuetteInfo( "Ettin", 0x20D8, 367),
			new MonsterStatuetteInfo( "Gargoyle", 0x20D9, 372),
			new MonsterStatuetteInfo( "Crocodile", 0x20DA, 660),
			new MonsterStatuetteInfo( "Grizzly Bear", 0x20DB, 0xA3),
			new MonsterStatuetteInfo( "Harpy", 0x20DC, 402),
			new MonsterStatuetteInfo( "Horse", 0x20DD, 0xA8),
			new MonsterStatuetteInfo( "Lizardman", 0x20DE, 417),
			new MonsterStatuetteInfo( "Ogre", 0x20DF, 427),
			new MonsterStatuetteInfo( "Orc", 0x20E0, 1114),
			new MonsterStatuetteInfo( "Polar Bear", 0x20E1, 0xA3),
			new MonsterStatuetteInfo( "Rabbit", 0x20E2, new int[]{ 0xC9, 0xCA, 0xCB }),
			new MonsterStatuetteInfo( "Ratman", 0x20E3, 437),
			new MonsterStatuetteInfo( "Scorpion", 0x20E4, 397),
			new MonsterStatuetteInfo( "Serpent", 0x20E5, 219),
			new MonsterStatuetteInfo( "Sheep", 0x20E6, 0xD6),
			new MonsterStatuetteInfo( "Skeleton", 0x20E7, 1165),
			new MonsterStatuetteInfo( "Slime", 0x20E8, 456),
			new MonsterStatuetteInfo( "Troll", 0x20E9, 461),
			new MonsterStatuetteInfo( "Wolf", 0x20EA, 229),
			new MonsterStatuetteInfo( "Wooly Sheep", 0x20EB, 0xD6),
			new MonsterStatuetteInfo( "Zombie", 0x20EC, 471),
			new MonsterStatuetteInfo( "Air Elemental", 0x20ED, 655),
			new MonsterStatuetteInfo( "Bird", 0x20EE, 0x1B),
			new MonsterStatuetteInfo( "Bull", 0x20EF, 0x64),
			new MonsterStatuetteInfo( "Bull", 0x20F0, 0x64),
			new MonsterStatuetteInfo( "Dolphin", 0x20F1, 0x8A),
			new MonsterStatuetteInfo( "Eagle", 0x20F2, 0x2EE),
			new MonsterStatuetteInfo( "Fire Elemental", 0x20F3, 838),
			new MonsterStatuetteInfo( "Gazer_Old", 0x20F4, 377),
			new MonsterStatuetteInfo( "Gorilla", 0x20F5, 158),
			new MonsterStatuetteInfo( "Llama", 0x20F6, 1011),
			new MonsterStatuetteInfo( "Cougar", 0x20F7, 0x73),
			new MonsterStatuetteInfo( "Lich", 0x20F8, 1001),
			new MonsterStatuetteInfo( "Mongbat", 0x20F9, 422),
			new MonsterStatuetteInfo( "Reaper", 0x20FA, 442),
			new MonsterStatuetteInfo( "Sea Serpent", 0x20FB, 447),
			new MonsterStatuetteInfo( "Snake", 0x20FC, 0xDB),
			new MonsterStatuetteInfo( "Spider", 0x20FD, 0x388),
			new MonsterStatuetteInfo( "Light Snake", 0x20FE, 0xDB),
			new MonsterStatuetteInfo( "Walrus", 0x20FF, 0xE0),
			new MonsterStatuetteInfo( "Wisp", 0x2100, 466),
			new MonsterStatuetteInfo( "Pig", 0x2101, 0xC4),
			new MonsterStatuetteInfo( "Panther", 0x2102, 0x462),
			new MonsterStatuetteInfo( "Cow", 0x2103, 120),
			new MonsterStatuetteInfo( "Daemon", 0x2104, 357),
			new MonsterStatuetteInfo( "Daemon", 0x2105, 357),
			new MonsterStatuetteInfo( "Male Vendor", 0x2106, 340),
			new MonsterStatuetteInfo( "Female Vendor", 0x2107, 331),
			new MonsterStatuetteInfo( "Mountain Goat", 0x2108, 0x99),
			new MonsterStatuetteInfo( "Ghoul", 0x2109, 0x482),
			new MonsterStatuetteInfo( "Headless", 0x210A, 0x39D),
			new MonsterStatuetteInfo( "Water Elemental", 0x210B, 278),
			new MonsterStatuetteInfo( "Elf Male", 0x210C, 340),
			new MonsterStatuetteInfo( "Elf Female", 0x210D, 331),
			//Section II
			new MonsterStatuetteInfo( "Black Bear", 0x2118, 0xA3),
			new MonsterStatuetteInfo( "Big Cat", 0x2119, 0x69),
			new MonsterStatuetteInfo( "Bird", 0x211A, 0x1B),
			new MonsterStatuetteInfo( "Cat", 0x211B, 0x69),
			new MonsterStatuetteInfo( "Dog", 0x211C, 0x85),
			new MonsterStatuetteInfo( "Eagle", 0x211D, 0x2EE),
			new MonsterStatuetteInfo( "Grizzly Bear", 0x211E, 0xA3),
			new MonsterStatuetteInfo( "Gray Horse", 0x211F, 0xA8),
			new MonsterStatuetteInfo( "Horse", 0x2120, 0xA8),
			new MonsterStatuetteInfo( "Brown Horse", 0x2121, 0xA8),
			new MonsterStatuetteInfo( "Wolf", 0x2122, 229),
			new MonsterStatuetteInfo( "Rat", 0x2123, 0xCC),
			new MonsterStatuetteInfo( "Tan Horse", 0x2124, 0xA8),
			new MonsterStatuetteInfo( "Rabbit", 0x2125, new int[]{ 0xC9, 0xCA, 0xCB }),
			new MonsterStatuetteInfo( "Pack Horse", 0x2126, 0xA8),
			new MonsterStatuetteInfo( "Pack Llama", 0x2127, 0x3F3),

			new MonsterStatuetteInfo( "Terathan Warrior", 0x212A, 589),
			new MonsterStatuetteInfo( "Terathan Drone", 0x212B, 594),
			new MonsterStatuetteInfo( "Terathan Matriarch", 0x212C, 599),
			new MonsterStatuetteInfo( "Cyclops", 0x212D, 604),
			new MonsterStatuetteInfo( "Titan", 0x212E, 609),
			new MonsterStatuetteInfo( "Giant Toad", 0x212F, 0x26B),
			new MonsterStatuetteInfo( "Bullfrog", 0x2130, 0x266),
			new MonsterStatuetteInfo( "Lizard", 0x2131, 0x5A),
			new MonsterStatuetteInfo( "Ophidian Mage", 0x2132, 639),
			new MonsterStatuetteInfo( "Ophidian Warrior", 0x2133, 634),
			new MonsterStatuetteInfo( "Ophidian Queen", 0x2134, 644),
			new MonsterStatuetteInfo( "Desert Ostard", 0x2135, 0x270),
			new MonsterStatuetteInfo( "Frenzied Ostard", 0x2136, 0x275),
			new MonsterStatuetteInfo( "Forest Ostard", 0x2137, 0x270),
			//Section III
			new MonsterStatuetteInfo( "Fire Steed", 0x21F1, 0xA8),
			//Section IV
			new MonsterStatuetteInfo( "Billy Goat", 0x2580, 0x99),
			new MonsterStatuetteInfo( "Centaur", 0x2581, 679),
			new MonsterStatuetteInfo( "Corpser", 0x2582, 684),
			new MonsterStatuetteInfo( "Cougar", 0x2583, 0x73),
			new MonsterStatuetteInfo( "Daemon", 0x2584, 357),
			new MonsterStatuetteInfo( "Black Gate Daemon", 0x2585, 357),
			new MonsterStatuetteInfo( "Elder Daemon", 0x2586, 357),
			new MonsterStatuetteInfo( "Ice Fiend Daemon", 0x2587, 357),
			new MonsterStatuetteInfo( "Dog Hound", 0x2588, 229),
			new MonsterStatuetteInfo( "Ethereal Warrior", 0x2589, 0x2F5),
			new MonsterStatuetteInfo( "Evil Mage", 0x258A, 340),
			new MonsterStatuetteInfo( "Evil Mage Boss", 0x258B, 340),
			new MonsterStatuetteInfo( "Giant Frog", 0x258C, 0x266),
			new MonsterStatuetteInfo( "Gargoyle", 0x258D, 372),
			new MonsterStatuetteInfo( "Stone Gargoyle", 0x258E, 0x174),
			new MonsterStatuetteInfo( "Gazer", 0x258F, 377),
			new MonsterStatuetteInfo( "Efreet", 0x2590, 0x300),
			new MonsterStatuetteInfo( "Ghoul", 0x2591, 0x482),
			new MonsterStatuetteInfo( "Silver Back Gorilla", 0x2592, 158),
			new MonsterStatuetteInfo( "Great Worm", 0x2593, 0xDC),
			new MonsterStatuetteInfo( "Stone Harpy", 0x2594, 402),
			new MonsterStatuetteInfo( "Headless", 0x2595, 0x39D),
			new MonsterStatuetteInfo( "Hell Hound", 0x2596, 229),
			new MonsterStatuetteInfo( "Hell Cat", 0x2597, 0x69),
			new MonsterStatuetteInfo( "Brown Horse", 0x2598, 0xA8),
			new MonsterStatuetteInfo( "Gray Horse", 0x2599, 0xA8),
			new MonsterStatuetteInfo( "Dark Brown Horse", 0x259A, 0xA8),
			new MonsterStatuetteInfo( "Dark Steed", 0x259B, 0xA8),
			new MonsterStatuetteInfo( "Nightmare", 0x259C, 0xA8),
			new MonsterStatuetteInfo( "Silver Steed", 0x259D, 0xA8),
			new MonsterStatuetteInfo( "Tan Horse", 0x259E, 0xA8),
			new MonsterStatuetteInfo( "Imp", 0x259F, 422),
			new MonsterStatuetteInfo( "Kirin", 0x25A0, 0x3C5),
			new MonsterStatuetteInfo( "Komodo Dragon", 0x25A1, 0x5A),
			new MonsterStatuetteInfo( "Kraken", 0x25A2, 353),
			new MonsterStatuetteInfo( "Snow Leopard", 0x25A3, 0x73),
			new MonsterStatuetteInfo( "Lich", 0x25A4, 0x3E9),
			new MonsterStatuetteInfo( "Lich Lord", 0x25A5, 0x3E9),
			new MonsterStatuetteInfo( "Mongbat", 0x25A6, 422),
			new MonsterStatuetteInfo( "Mummy", 0x25A7, 471),
			new MonsterStatuetteInfo( "Ogre Lord", 0x25A8, 427),
			new MonsterStatuetteInfo( "Ophidian Archmage", 0x25A9, 639),
			new MonsterStatuetteInfo( "Ophidian Knight", 0x25AA, 634),
			new MonsterStatuetteInfo( "Ophidian Mage", 0x25AB, 639),
			new MonsterStatuetteInfo( "Ophidian Queen", 0x25AC, 644),
			new MonsterStatuetteInfo( "Ophidian Warrior", 0x25AD, 634),
			new MonsterStatuetteInfo( "Orc", 0x25AE, 0x45A),
			new MonsterStatuetteInfo( "Orc Captain", 0x25AF, 0x45A),
			new MonsterStatuetteInfo( "Orc Lord", 0x25B0, 0x45A),
			new MonsterStatuetteInfo( "Orc Shaman", 0x25B1, 0x45A),
			new MonsterStatuetteInfo( "Desert Ostard", 0x25B2, 0x270),
			new MonsterStatuetteInfo( "Forest Ostard", 0x25B3, 0x270),
			new MonsterStatuetteInfo( "Frienzied Ostard", 0x25B4, 0x275),
			new MonsterStatuetteInfo( "Panther", 0x25B5, 0x462),
			new MonsterStatuetteInfo( "Pixie", 0x25B6, 0x467),
			new MonsterStatuetteInfo( "Rat Man", 0x25B7, 437),
			new MonsterStatuetteInfo( "Rotting Corpse", 0x25B8, 471),
			new MonsterStatuetteInfo( "Giant Scorpion", 0x25B9, 397),
			new MonsterStatuetteInfo( "Sea Horse", 0x25BA, 0x478),
			new MonsterStatuetteInfo( "Harrower", 0x25BB, new int[] { 0x289, 0x28A, 0x28B }),
			new MonsterStatuetteInfo( "Skeleton", 0x25BC, 0x48D),
			new MonsterStatuetteInfo( "Skeleton Knight", 0x25BD, 0x48D),
			new MonsterStatuetteInfo( "Skeleton Mage", 0x25BE, 0x48D),
			new MonsterStatuetteInfo( "Giant Snake", 0x25BF, 219),
			new MonsterStatuetteInfo( "Giant Ice Snake", 0x25C0, 219),
			new MonsterStatuetteInfo( "Giant Lava Snake", 0x25C1, 219),
			new MonsterStatuetteInfo( "Silver Serpent", 0x25C2, 219),
			new MonsterStatuetteInfo( "Black Widow Spider", 0x25C3, 357),
			new MonsterStatuetteInfo( "Dread Spider", 0x25C4, 1170),
			new MonsterStatuetteInfo( "Frost Spider", 0x25C5, 0x388),
			new MonsterStatuetteInfo( "Giant Spider", 0x25C6, 0x388),
			new MonsterStatuetteInfo( "Ghost", 0x25C7, 1154),
			new MonsterStatuetteInfo( "Swamp Tentacles", 0x25C8, 352),
			new MonsterStatuetteInfo( "Terathan Drone", 0x25C9, 594),
			new MonsterStatuetteInfo( "Terathan Avenger", 0x25CA, 0x24D),
			new MonsterStatuetteInfo( "Terathan Queen", 0x25CB, 599),
			new MonsterStatuetteInfo( "Terathan Warrior", 0x25CC, 589),
			new MonsterStatuetteInfo( "Titan", 0x25CD, 609),
			new MonsterStatuetteInfo( "Unicorn", 0x25CE, 0x4BC),
			new MonsterStatuetteInfo( "Wolf", 0x25CF, 229),
			new MonsterStatuetteInfo( "Dire Wolf", 0x25D0, 229),
			new MonsterStatuetteInfo( "Gray Wolf", 0x25D1, 229),
			new MonsterStatuetteInfo( "Silver Wolf", 0x25D2, 229),
			new MonsterStatuetteInfo( "Timber Wolf", 0x25D3, 229),
			new MonsterStatuetteInfo( "Wyvern", 0x25D4, 362),
			new MonsterStatuetteInfo( "Zombie", 0x25D5, 471),
			new MonsterStatuetteInfo( "Acid Elemental", 0x25D6, 263),
			new MonsterStatuetteInfo( "Air Elemental", 0x25D7, 655),
			new MonsterStatuetteInfo( "Blood Elemental", 0x25D8, 278),
			new MonsterStatuetteInfo( "Fire Elemental", 0x25D9, 838),
			new MonsterStatuetteInfo( "Ice Elemental", 0x25DA, 268),
			new MonsterStatuetteInfo( "Poison Elemental", 0x25DB, 263),
			new MonsterStatuetteInfo( "Snow Elemental", 0x25DC, 263),
			new MonsterStatuetteInfo( "Water Elemental", 0x25DD, 278),
			//Section V
			new MonsterStatuetteInfo( "Adranath", 0x25F8, 340),
			new MonsterStatuetteInfo( "Lord Blackthorn", 0x25F9, 340),
			new MonsterStatuetteInfo( "CaptainDasha", 0x25FA, 331),
			new MonsterStatuetteInfo( "Dawn", 0x25FB, 331),
			new MonsterStatuetteInfo( "Kabur", 0x25FC, 340),
			//Section VI
			new MonsterStatuetteInfo( "Solen Queen", 0x2602, 959),
			new MonsterStatuetteInfo( "Solen Warrior", 0x2603, 959),
			new MonsterStatuetteInfo( "Solen Worker", 0x2604, 959),
			new MonsterStatuetteInfo( "Arcane Demon", 0x2605, 357),
			new MonsterStatuetteInfo( "Betrayer", 0x2606, -1),
			new MonsterStatuetteInfo( "Bogling", 0x2607, 422),
			new MonsterStatuetteInfo( "Bog Thing", 0x2608, -1),
			new MonsterStatuetteInfo( "Chaos Demon", 0x2609, 0x3E9),
			new MonsterStatuetteInfo( "Chariot", 0x260A, -1),
			new MonsterStatuetteInfo( "Exodus Minion", 0x260B, 0x26C),
			new MonsterStatuetteInfo( "Exodus Overseer", 0x260C, 0x26C),
			new MonsterStatuetteInfo( "Dopple Ganger", 0x260D, -1),
			new MonsterStatuetteInfo( "Moloch Demon", 0x260E, 357),
			new MonsterStatuetteInfo( "Giant Beetle", 0x260F, new int[]{ 0x162, 0x163, 0x21D }),
			new MonsterStatuetteInfo( "Golem", 0x2610, new int[]{ 541, 542, 545, 320 }),
			new MonsterStatuetteInfo( "Horde Minion", 0x2611, 0x39D),
			new MonsterStatuetteInfo( "Juggernaut", 0x2612, 0x26C),
			new MonsterStatuetteInfo( "Plague Beast", 0x2613, 775),
			new MonsterStatuetteInfo( "Quagmire", 0x2614, 352),
			new MonsterStatuetteInfo( "Ridgeback", 0x2615, 0x3F3),
			new MonsterStatuetteInfo( "Sand Vortex", 0x2616, 263),
			new MonsterStatuetteInfo( "Skeletal Mount", 0x2617, -1),
			new MonsterStatuetteInfo( "Sphinx", 0x2618, -1),
			new MonsterStatuetteInfo( "Swamp Dragon", 0x2619, 0x16A),
			new MonsterStatuetteInfo( "Swarm", 0x261A, 377),
			new MonsterStatuetteInfo( "Plague Beast Lord", 0x261B, 775),
			new MonsterStatuetteInfo( "Meer Mage", 0x261C, new int[]{ 0x14D, 0x314, 0x75 }),
			new MonsterStatuetteInfo( "Meer Warrior", 0x261D, new int[]{ 0x156, 0x15C }),
			new MonsterStatuetteInfo( "Juka Mage", 0x261E, new int[]{ 0x1AC, 0x1CD, 0x1D0, 0x28D }),
			new MonsterStatuetteInfo( "Juka Warrior", 0x261F, new int[]{ 0x1AC, 0x1CD, 0x1D0, 0x28D }),
			new MonsterStatuetteInfo( "Crystal Elemental", 0x2620, 278),
			new MonsterStatuetteInfo( "Treefellow", 0x2621, new int[]{ 443, 31, 672 } ),
			new MonsterStatuetteInfo( "Skittering Hopper", 0x2622, 959),
			new MonsterStatuetteInfo( "Devourer of Souls", 0x2623, 357),
			new MonsterStatuetteInfo( "Flesh Golem", 0x2624, 684),
			new MonsterStatuetteInfo( "Gore Fiend", 0x2625, 224),
			new MonsterStatuetteInfo( "Impaler", 0x2626, 0x2A7),
			new MonsterStatuetteInfo( "Gibberling", 0x2627, 422),
			new MonsterStatuetteInfo( "Bone Daemon", 0x2628, 0x48D),
			new MonsterStatuetteInfo( "Patchwork Skeleton", 0x2629, 0x48D),
			new MonsterStatuetteInfo( "Wailing Banshee", 0x262A, 0x482),
			new MonsterStatuetteInfo( "Shadow Knight", 0x262B, new int[]{ 0x2CE, 0x2C1, 0x2D1, 0x2C8 }),
			new MonsterStatuetteInfo( "Abysmal Horror", 0x262C, 0x451),
			new MonsterStatuetteInfo( "Darknight Creeper", 0x262D, 0xE0),
			new MonsterStatuetteInfo( "Ravager", 0x262E, 357),
			new MonsterStatuetteInfo( "Flesh Renderer", 0x262F, new int[]{ 0x34C, 0x354 }),
			new MonsterStatuetteInfo( "Wanderer of the Void", 0x2630, 377),
			new MonsterStatuetteInfo( "Vampire Bat", 0x2631, 0x270),
			new MonsterStatuetteInfo( "Dark Father", 0x2632, 0x165),
			new MonsterStatuetteInfo( "Mound of Maggots", 0x2633, 898),
			//Section VI
			new MonsterStatuetteInfo( "Bake Kitsune", 0x2763, 0x4DB),
			new MonsterStatuetteInfo( "Crane", 0x2764, 0x4D7),
			new MonsterStatuetteInfo( "Deathwatch Beetle", 0x2765, 0x4F0),
			new MonsterStatuetteInfo( "Rai-ju", 0x2766, 0x346),
			new MonsterStatuetteInfo( "Fan Dancer", 0x2767, 0x372),
			new MonsterStatuetteInfo( "Gaman", 0x2768, 0x4F5),
			new MonsterStatuetteInfo( "Kaze Kemono", 0x2769, 655),
			new MonsterStatuetteInfo( "Hiryu", 0x276A, 0x4FB),
			new MonsterStatuetteInfo( "Kappa", 0x276B, 0x508),
			new MonsterStatuetteInfo( "Lady of the Snow", 0x276C, 0x482),
			new MonsterStatuetteInfo( "Oni", 0x276D, 0x4E0),
			new MonsterStatuetteInfo( "Revenant Lion", 0x276E, 0x515),
			new MonsterStatuetteInfo( "Rune Beetle", 0x276F, 0x4E5),
			new MonsterStatuetteInfo( "Tsuki Wolf", 0x2770, 0x52A),
			new MonsterStatuetteInfo( "Yamandon", 0x2771, 1259),
			new MonsterStatuetteInfo( "Yomotsu Elder", 0x2772, 0x452),
			new MonsterStatuetteInfo( "Yomotsu Priest", 0x2773, 0x452),
			//Section VII
			new MonsterStatuetteInfo( "Yomotsu Warrior", 0x281B, 0x452),
			new MonsterStatuetteInfo( "Fire Beetle", 0x281C, new int[]{ 0x162, 0x163, 0x21D }),

			//Section VIII
			new MonsterStatuetteInfo( "Chief Paroxysmus", 0x2D82, 0x56F ),
			new MonsterStatuetteInfo( "Dread Horn", 0x2D83, 0xA8 ),
			new MonsterStatuetteInfo( "Lady Melisande", 0x2D84, 451 ),
			new MonsterStatuetteInfo( "Monstrous Interred", 0x2D85, 589 ),
			new MonsterStatuetteInfo( "Shimmering Effusion", 0x2D87, new int[]{ 0x1BF, 0x1C0, 0x1C1, 0x1C2 } ),
			new MonsterStatuetteInfo( "Tormented Minotaur", 0x2D88, 0x596 ),
			new MonsterStatuetteInfo( "Minotaur", 0x2D89, 0x596 ),
			new MonsterStatuetteInfo( "Changeling", 0x2D8A, 0x470 ),
			new MonsterStatuetteInfo( "Hydra", 0x2D8B, 0x16A ),
			new MonsterStatuetteInfo( "Dryad", 0x2D8C, 0x467 ),
			new MonsterStatuetteInfo( "Troglodyte", 0x2D8D, 0x59D ),
			new MonsterStatuetteInfo( "Satyr", 0x2D90, 0x585 ),
			new MonsterStatuetteInfo( "Fetid Essence", 0x2D92, 0x56C ),
			new MonsterStatuetteInfo( "Mantra Effervescense", 0x2D93, 0x56E ),
			new MonsterStatuetteInfo( "Corporeal Brume", 0x2D94, 0x56B ),
			new MonsterStatuetteInfo( "Chimera", 0x2D95, 362 ),
			new MonsterStatuetteInfo( "Cu Sidhe", 0x2D96, 0x575 ),
			new MonsterStatuetteInfo( "Squirrel", 0x2D97, -1 ),
			new MonsterStatuetteInfo( "Farret", 0x2D98, -1 ),
			new MonsterStatuetteInfo( "Minotaur", 0x2D99, 0x596 ),
			new MonsterStatuetteInfo( "Minotaur", 0x2D9A, 0x596 ),
			new MonsterStatuetteInfo( "Parrot", 0x2D9B, new int[]{ 0x5B2 } ),
			new MonsterStatuetteInfo( "Mondain's Steed", 0x2D9C, 0xA8 ),

			//Section IX
			new MonsterStatuetteInfo( "Phillips Wooden Steed", 0x3FFE, 168 ),

			new MonsterStatuetteInfo( "Red Death", 0x2617, new int[] { } ),
		};

		public static MonsterStatuetteInfo GetInfo( MonsterStatuetteType type )
		{
			int v = (int)type;

			if ( v < 0 || v >= m_Table.Length )
				v = 0;

			return m_Table[v];
		}
	}

	public class MonsterStatuette : Item, IRewardItem
	{
        public static Dictionary<Type, MonsterStatuetteType[]> MonsterStatuetteLookup = new Dictionary<Type, MonsterStatuetteType[]>();

        public static void TryAddStatue(Container c, Type type)
        {
            TryAddStatue(c, type, 0);
        }

        public static void TryAddStatue(Container c, Type type, int hue)
        {
            try
            {
                MonsterStatuetteType[] statueTypes = null;
                MonsterStatuetteLookup.TryGetValue(type, out statueTypes);
                if (statueTypes != null && hue == 0)
                {
                    c.DropItem(new MonsterStatuette(statueTypes[Utility.Random(statueTypes.Length)]));
                }
                else if (statueTypes != null && hue != 0)
                {
                    c.DropItem(new MonsterStatuette(statueTypes[Utility.Random(statueTypes.Length)]){Hue = hue});                   
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("MonsterStatuette error!: " + e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }

        public static void Initialize()
        {
            MonsterStatuetteLookup.Add(typeof(SavageShaman), new MonsterStatuetteType[] { MonsterStatuetteType.Male, MonsterStatuetteType.Female });
            MonsterStatuetteLookup.Add(typeof(Savage), new MonsterStatuetteType[] { MonsterStatuetteType.Male, MonsterStatuetteType.Female });
            MonsterStatuetteLookup.Add(typeof(SavageRider), new MonsterStatuetteType[] { MonsterStatuetteType.Male, MonsterStatuetteType.Female });
            MonsterStatuetteLookup.Add(typeof(BarbarianChieftan), new MonsterStatuetteType[] { MonsterStatuetteType.Male, MonsterStatuetteType.Female });
            MonsterStatuetteLookup.Add(typeof(BarbarianShaman), new MonsterStatuetteType[] { MonsterStatuetteType.Male, MonsterStatuetteType.Female });
            MonsterStatuetteLookup.Add(typeof(BarbarianWarrior), new MonsterStatuetteType[] { MonsterStatuetteType.Male, MonsterStatuetteType.Female });
            MonsterStatuetteLookup.Add(typeof(BlackBear), new MonsterStatuetteType[] { MonsterStatuetteType.Bear, MonsterStatuetteType.BlackBear});
            MonsterStatuetteLookup.Add(typeof(BrownBear), new MonsterStatuetteType[] { MonsterStatuetteType.Bear, MonsterStatuetteType.BlackBear });
            MonsterStatuetteLookup.Add(typeof(GiantRat), new MonsterStatuetteType[] { MonsterStatuetteType.GiantRat });
            MonsterStatuetteLookup.Add(typeof(Chicken), new MonsterStatuetteType[] { MonsterStatuetteType.Chicken });
            MonsterStatuetteLookup.Add(typeof(Corpser), new MonsterStatuetteType[] { MonsterStatuetteType.CorpserFrame, MonsterStatuetteType.Corpser });
            MonsterStatuetteLookup.Add(typeof(Daemon), new MonsterStatuetteType[] { MonsterStatuetteType.Demon, MonsterStatuetteType.Daemon_Old, MonsterStatuetteType.Daemon });
            MonsterStatuetteLookup.Add(typeof(ShadowDaemon), new MonsterStatuetteType[] { MonsterStatuetteType.Demon, MonsterStatuetteType.Daemon_Old, MonsterStatuetteType.Daemon });
            MonsterStatuetteLookup.Add(typeof(Hind), new MonsterStatuetteType[] { MonsterStatuetteType.Deer });
            MonsterStatuetteLookup.Add(typeof(Dog), new MonsterStatuetteType[] { MonsterStatuetteType.DogFrame, MonsterStatuetteType.Hound, MonsterStatuetteType.Dog });
            MonsterStatuetteLookup.Add(typeof(Dragon), new MonsterStatuetteType[] { MonsterStatuetteType.Dragon });
            MonsterStatuetteLookup.Add(typeof(EarthElemental), new MonsterStatuetteType[] { MonsterStatuetteType.EarthElemental });
            MonsterStatuetteLookup.Add(typeof(Ettin), new MonsterStatuetteType[] { MonsterStatuetteType.Ettin });
            MonsterStatuetteLookup.Add(typeof(Alligator), new MonsterStatuetteType[] { MonsterStatuetteType.Crocodile });
            MonsterStatuetteLookup.Add(typeof(GrizzlyBear), new MonsterStatuetteType[] { MonsterStatuetteType.GrizzlyBearFrame, MonsterStatuetteType.GrizzlyBear });
            MonsterStatuetteLookup.Add(typeof(RagingGrizzlyBear), new MonsterStatuetteType[] { MonsterStatuetteType.GrizzlyBearFrame, MonsterStatuetteType.GrizzlyBear });
            MonsterStatuetteLookup.Add(typeof(Harpy), new MonsterStatuetteType[] { MonsterStatuetteType.Harpy });
            MonsterStatuetteLookup.Add(typeof(Horse), new MonsterStatuetteType[] { MonsterStatuetteType.HorseFrame, MonsterStatuetteType.Horse, MonsterStatuetteType.BrownHorse_Old, MonsterStatuetteType.TanHorse_Old, MonsterStatuetteType.BrownHorse, MonsterStatuetteType.GrayHorse, MonsterStatuetteType.DarkBrownHorse, MonsterStatuetteType.TanHorse });
            MonsterStatuetteLookup.Add(typeof(Lizardman), new MonsterStatuetteType[] { MonsterStatuetteType.Lizardman });
            MonsterStatuetteLookup.Add(typeof(Ogre), new MonsterStatuetteType[] { MonsterStatuetteType.Ogre });
            MonsterStatuetteLookup.Add(typeof(Orc), new MonsterStatuetteType[] { MonsterStatuetteType.Orc_Old, MonsterStatuetteType.Orc });
            MonsterStatuetteLookup.Add(typeof(PolarBear), new MonsterStatuetteType[] { MonsterStatuetteType.PolarBear });
            MonsterStatuetteLookup.Add(typeof(Rabbit), new MonsterStatuetteType[] { MonsterStatuetteType.RabbitFrame, MonsterStatuetteType.Rabbit });
            MonsterStatuetteLookup.Add(typeof(Scorpion), new MonsterStatuetteType[] { MonsterStatuetteType.ScorpionFrame, MonsterStatuetteType.GiantScorpion });
            MonsterStatuetteLookup.Add(typeof(SeaSerpent), new MonsterStatuetteType[] { MonsterStatuetteType.Serpent, MonsterStatuetteType.SeaSerpent });
            MonsterStatuetteLookup.Add(typeof(DeepSeaSerpent), new MonsterStatuetteType[] { MonsterStatuetteType.Serpent, MonsterStatuetteType.SeaSerpent });
            MonsterStatuetteLookup.Add(typeof(CrystalSeaSerpent), new MonsterStatuetteType[] { MonsterStatuetteType.Serpent, MonsterStatuetteType.SeaSerpent });
            MonsterStatuetteLookup.Add(typeof(Sheep), new MonsterStatuetteType[] { MonsterStatuetteType.Sheep, MonsterStatuetteType.WoolySheep });
            MonsterStatuetteLookup.Add(typeof(Skeleton), new MonsterStatuetteType[] { MonsterStatuetteType.Skeleton_Old, MonsterStatuetteType.Skeleton });
            MonsterStatuetteLookup.Add(typeof(Slime), new MonsterStatuetteType[] { MonsterStatuetteType.Slime });
            MonsterStatuetteLookup.Add(typeof(Troll), new MonsterStatuetteType[] { MonsterStatuetteType.Troll });
            MonsterStatuetteLookup.Add(typeof(Zombie), new MonsterStatuetteType[] { MonsterStatuetteType.Zombie_Old, MonsterStatuetteType.Zombie });
            MonsterStatuetteLookup.Add(typeof(Bull), new MonsterStatuetteType[] { MonsterStatuetteType.BrownBull, MonsterStatuetteType.DarkBrownBull });
            MonsterStatuetteLookup.Add(typeof(Dolphin), new MonsterStatuetteType[] { MonsterStatuetteType.Dolphin });
            MonsterStatuetteLookup.Add(typeof(Gorilla), new MonsterStatuetteType[] { MonsterStatuetteType.Gorilla, MonsterStatuetteType.SilverBackGorilla });
            MonsterStatuetteLookup.Add(typeof(Llama), new MonsterStatuetteType[] { MonsterStatuetteType.Llama });
            MonsterStatuetteLookup.Add(typeof(LichLord), new MonsterStatuetteType[] { MonsterStatuetteType.LichLord });
            MonsterStatuetteLookup.Add(typeof(Lich), new MonsterStatuetteType[] { MonsterStatuetteType.Lich_Old, MonsterStatuetteType.Lich });
            MonsterStatuetteLookup.Add(typeof(Mongbat), new MonsterStatuetteType[] { MonsterStatuetteType.Mongbat_Old, MonsterStatuetteType.Mongbat });
            MonsterStatuetteLookup.Add(typeof(Reaper), new MonsterStatuetteType[] { MonsterStatuetteType.Reaper });
            MonsterStatuetteLookup.Add(typeof(Walrus), new MonsterStatuetteType[] { MonsterStatuetteType.Walrus });
            MonsterStatuetteLookup.Add(typeof(Wisp), new MonsterStatuetteType[] { MonsterStatuetteType.Wisp });
            MonsterStatuetteLookup.Add(typeof(Pig), new MonsterStatuetteType[] { MonsterStatuetteType.Pig });
            MonsterStatuetteLookup.Add(typeof(Panther), new MonsterStatuetteType[] { MonsterStatuetteType.Panther_Old, MonsterStatuetteType.BigCat, MonsterStatuetteType.Panther });
            MonsterStatuetteLookup.Add(typeof(Cow), new MonsterStatuetteType[] { MonsterStatuetteType.Cow });
            //MonsterStatuetteLookup.Add(typeof(DaemonDupe), new MonsterStatuetteType[] { MonsterStatuetteType.DaemonDupe });
            MonsterStatuetteLookup.Add(typeof(Peasant), new MonsterStatuetteType[] { MonsterStatuetteType.NPCMale, MonsterStatuetteType.NPCFemale });
            MonsterStatuetteLookup.Add(typeof(EscortableMage), new MonsterStatuetteType[] { MonsterStatuetteType.NPCMale, MonsterStatuetteType.NPCFemale });
            MonsterStatuetteLookup.Add(typeof(SeekerOfAdventure), new MonsterStatuetteType[] { MonsterStatuetteType.NPCMale, MonsterStatuetteType.NPCFemale });
            MonsterStatuetteLookup.Add(typeof(HireFighter), new MonsterStatuetteType[] { MonsterStatuetteType.NPCMale, MonsterStatuetteType.NPCFemale });
            MonsterStatuetteLookup.Add(typeof(HirePaladin), new MonsterStatuetteType[] { MonsterStatuetteType.NPCMale, MonsterStatuetteType.NPCFemale });
            MonsterStatuetteLookup.Add(typeof(HireMage), new MonsterStatuetteType[] { MonsterStatuetteType.NPCMale, MonsterStatuetteType.NPCFemale });
            MonsterStatuetteLookup.Add(typeof(Farmer), new MonsterStatuetteType[] { MonsterStatuetteType.NPCMale, MonsterStatuetteType.NPCFemale });
            MonsterStatuetteLookup.Add(typeof(MountainGoat), new MonsterStatuetteType[] { MonsterStatuetteType.MountainGoat, MonsterStatuetteType.BillyGoat });
            MonsterStatuetteLookup.Add(typeof(Goat), new MonsterStatuetteType[] { MonsterStatuetteType.MountainGoat, MonsterStatuetteType.BillyGoat });
            MonsterStatuetteLookup.Add(typeof(Ghoul), new MonsterStatuetteType[] { MonsterStatuetteType.Ghoul_Old, MonsterStatuetteType.Ghoul });
            MonsterStatuetteLookup.Add(typeof(HeadlessOne), new MonsterStatuetteType[] { MonsterStatuetteType.Headless_Old, MonsterStatuetteType.Headless });
            MonsterStatuetteLookup.Add(typeof(WaterElemental), new MonsterStatuetteType[] { MonsterStatuetteType.WaterElemental_Old, MonsterStatuetteType.WaterElemental }); //Skip
            MonsterStatuetteLookup.Add(typeof(ForestElfRanger), new MonsterStatuetteType[] { MonsterStatuetteType.ElfMale, MonsterStatuetteType.ElfFemale });
            MonsterStatuetteLookup.Add(typeof(Bird), new MonsterStatuetteType[] { MonsterStatuetteType.Bird, MonsterStatuetteType.BirdFlying });
            MonsterStatuetteLookup.Add(typeof(Cat), new MonsterStatuetteType[] { MonsterStatuetteType.Cat });
            MonsterStatuetteLookup.Add(typeof(Eagle), new MonsterStatuetteType[] { MonsterStatuetteType.Eagle, MonsterStatuetteType.EagleFlying });
            MonsterStatuetteLookup.Add(typeof(Rat), new MonsterStatuetteType[] { MonsterStatuetteType.Rat });
            MonsterStatuetteLookup.Add(typeof(PackHorse), new MonsterStatuetteType[] { MonsterStatuetteType.PackHorse });
            MonsterStatuetteLookup.Add(typeof(PackLlama), new MonsterStatuetteType[] { MonsterStatuetteType.PackLlama }); //Skip
            MonsterStatuetteLookup.Add(typeof(TerathanWarrior), new MonsterStatuetteType[] { MonsterStatuetteType.TerathanWarrior_Old, MonsterStatuetteType.TerathanWarrior });
            MonsterStatuetteLookup.Add(typeof(TerathanDrone), new MonsterStatuetteType[] { MonsterStatuetteType.TerathanDrone_Old, MonsterStatuetteType.TerathanDrone });
            MonsterStatuetteLookup.Add(typeof(TerathanMatriarch), new MonsterStatuetteType[] { MonsterStatuetteType.TerathanMatriarch, MonsterStatuetteType.TerathanQueen });

            MonsterStatuetteLookup.Add(typeof(TerathanAvenger), new MonsterStatuetteType[] { MonsterStatuetteType.TerathanAvenger });
            MonsterStatuetteLookup.Add(typeof(Cyclops), new MonsterStatuetteType[] { MonsterStatuetteType.Cyclops });
            MonsterStatuetteLookup.Add(typeof(Titan), new MonsterStatuetteType[] { MonsterStatuetteType.Titan_Old, MonsterStatuetteType.Titan });
            MonsterStatuetteLookup.Add(typeof(GiantToad), new MonsterStatuetteType[] { MonsterStatuetteType.GiantToad, MonsterStatuetteType.GiantFrog });
            MonsterStatuetteLookup.Add(typeof(BullFrog), new MonsterStatuetteType[] { MonsterStatuetteType.Bullfrog });
            MonsterStatuetteLookup.Add(typeof(LavaLizard), new MonsterStatuetteType[] { MonsterStatuetteType.Lizard });
            MonsterStatuetteLookup.Add(typeof(FireSteed), new MonsterStatuetteType[] { MonsterStatuetteType.FireSteed });
            MonsterStatuetteLookup.Add(typeof(Centaur), new MonsterStatuetteType[] { MonsterStatuetteType.Centaur });
            MonsterStatuetteLookup.Add(typeof(Cougar), new MonsterStatuetteType[] { MonsterStatuetteType.Cougar, MonsterStatuetteType.BigCat, MonsterStatuetteType.Cougar_Old });
            MonsterStatuetteLookup.Add(typeof(Balron), new MonsterStatuetteType[] { MonsterStatuetteType.BlackGateDaemon, MonsterStatuetteType.ElderDaemon });
            MonsterStatuetteLookup.Add(typeof(ShadowBalron), new MonsterStatuetteType[] { MonsterStatuetteType.BlackGateDaemon, MonsterStatuetteType.ElderDaemon });
            MonsterStatuetteLookup.Add(typeof(ElderDaemon), new MonsterStatuetteType[] { MonsterStatuetteType.BlackGateDaemon, MonsterStatuetteType.ElderDaemon });
            MonsterStatuetteLookup.Add(typeof(IceFiend), new MonsterStatuetteType[] { MonsterStatuetteType.IceFiendDaemon });
            MonsterStatuetteLookup.Add(typeof(EtherealWarrior), new MonsterStatuetteType[] { MonsterStatuetteType.EtherealWarrior });
            MonsterStatuetteLookup.Add(typeof(EvilMage), new MonsterStatuetteType[] { MonsterStatuetteType.EvilMage });
            MonsterStatuetteLookup.Add(typeof(EvilMageLord), new MonsterStatuetteType[] { MonsterStatuetteType.EvilMageBoss });
            MonsterStatuetteLookup.Add(typeof(Gargoyle), new MonsterStatuetteType[] { MonsterStatuetteType.Gargoyle, MonsterStatuetteType.Gargoyle_Old });
            MonsterStatuetteLookup.Add(typeof(StoneGargoyle), new MonsterStatuetteType[] { MonsterStatuetteType.StoneGargoyle });
            MonsterStatuetteLookup.Add(typeof(Gazer), new MonsterStatuetteType[] { MonsterStatuetteType.Gazer_Old });
            MonsterStatuetteLookup.Add(typeof(ElderGazer), new MonsterStatuetteType[] { MonsterStatuetteType.Gazer });
            MonsterStatuetteLookup.Add(typeof(Efreet), new MonsterStatuetteType[] { MonsterStatuetteType.Efreet });
            //MonsterStatuetteLookup.Add(typeof(GreatWorm), new MonsterStatuetteType[] { MonsterStatuetteType.GreatWorm });
            MonsterStatuetteLookup.Add(typeof(StoneHarpy), new MonsterStatuetteType[] { MonsterStatuetteType.StoneHarpy });
            MonsterStatuetteLookup.Add(typeof(HellHound), new MonsterStatuetteType[] { MonsterStatuetteType.HellHound });
            MonsterStatuetteLookup.Add(typeof(HellCat), new MonsterStatuetteType[] { MonsterStatuetteType.HellCat });
            MonsterStatuetteLookup.Add(typeof(UnholySteed), new MonsterStatuetteType[] { MonsterStatuetteType.DarkSteed });
            MonsterStatuetteLookup.Add(typeof(Nightmare), new MonsterStatuetteType[] { MonsterStatuetteType.Nightmare, MonsterStatuetteType.MondainSteed });
            MonsterStatuetteLookup.Add(typeof(SilverSteed), new MonsterStatuetteType[] { MonsterStatuetteType.SilverSteed });
            MonsterStatuetteLookup.Add(typeof(Imp), new MonsterStatuetteType[] { MonsterStatuetteType.Imp });
            MonsterStatuetteLookup.Add(typeof(Kirin), new MonsterStatuetteType[] { MonsterStatuetteType.Kirin });
            //MonsterStatuetteLookup.Add(typeof(KomodoDragon), new MonsterStatuetteType[] { MonsterStatuetteType.KomodoDragon });
            MonsterStatuetteLookup.Add(typeof(Kraken), new MonsterStatuetteType[] { MonsterStatuetteType.Kraken });
            MonsterStatuetteLookup.Add(typeof(SnowLeopard), new MonsterStatuetteType[] { MonsterStatuetteType.SnowLeopard });
            MonsterStatuetteLookup.Add(typeof(Mummy), new MonsterStatuetteType[] { MonsterStatuetteType.Mummy });
            MonsterStatuetteLookup.Add(typeof(OgreLord), new MonsterStatuetteType[] { MonsterStatuetteType.OgreLord });
            MonsterStatuetteLookup.Add(typeof(OphidianArchmage), new MonsterStatuetteType[] { MonsterStatuetteType.OphidianArchmage });
            MonsterStatuetteLookup.Add(typeof(OphidianKnight), new MonsterStatuetteType[] { MonsterStatuetteType.OphidianKnight });
            MonsterStatuetteLookup.Add(typeof(OphidianMage), new MonsterStatuetteType[] { MonsterStatuetteType.OphidianMage, MonsterStatuetteType.OphidianMage_Old });
            MonsterStatuetteLookup.Add(typeof(OphidianWarrior), new MonsterStatuetteType[] { MonsterStatuetteType.OphidianWarrior_Old, MonsterStatuetteType.OphidianWarrior });
            MonsterStatuetteLookup.Add(typeof(OphidianMatriarch), new MonsterStatuetteType[] { MonsterStatuetteType.OphidianQueen_Old, MonsterStatuetteType.OphidianQueen });
            MonsterStatuetteLookup.Add(typeof(OrcCaptain), new MonsterStatuetteType[] { MonsterStatuetteType.OrcCaptain });
            MonsterStatuetteLookup.Add(typeof(OrcishLord), new MonsterStatuetteType[] { MonsterStatuetteType.OrcLord });
            MonsterStatuetteLookup.Add(typeof(OrcishMage), new MonsterStatuetteType[] { MonsterStatuetteType.OrcShaman });
            MonsterStatuetteLookup.Add(typeof(DesertOstard), new MonsterStatuetteType[] { MonsterStatuetteType.DesertOstard, MonsterStatuetteType.DesertOstard_Old });
            MonsterStatuetteLookup.Add(typeof(ForestOstard), new MonsterStatuetteType[] { MonsterStatuetteType.ForestOstard, MonsterStatuetteType.ForestOstard_Old });
            MonsterStatuetteLookup.Add(typeof(FrenziedOstard), new MonsterStatuetteType[] { MonsterStatuetteType.FrienziedOstard, MonsterStatuetteType.FrenziedOstard_Old });
            MonsterStatuetteLookup.Add(typeof(Pixie), new MonsterStatuetteType[] { MonsterStatuetteType.Pixie });
            MonsterStatuetteLookup.Add(typeof(Ratman), new MonsterStatuetteType[] { MonsterStatuetteType.Ratman, MonsterStatuetteType.Ratman_Old });
            MonsterStatuetteLookup.Add(typeof(RottingCorpse), new MonsterStatuetteType[] { MonsterStatuetteType.RottingCorpse });
            MonsterStatuetteLookup.Add(typeof(SeaHorse), new MonsterStatuetteType[] { MonsterStatuetteType.Seahorse });
            MonsterStatuetteLookup.Add(typeof(Harrower), new MonsterStatuetteType[] { MonsterStatuetteType.Harrower });
            MonsterStatuetteLookup.Add(typeof(SkeletalKnight), new MonsterStatuetteType[] { MonsterStatuetteType.SkeletonKnight });
            MonsterStatuetteLookup.Add(typeof(BoneMage), new MonsterStatuetteType[] { MonsterStatuetteType.SkeletonMage });
            MonsterStatuetteLookup.Add(typeof(SkeletalMage), new MonsterStatuetteType[] { MonsterStatuetteType.SkeletonMage });
            MonsterStatuetteLookup.Add(typeof(GiantSerpent), new MonsterStatuetteType[] { MonsterStatuetteType.GiantSnake });
            MonsterStatuetteLookup.Add(typeof(GiantIceWorm), new MonsterStatuetteType[] { MonsterStatuetteType.GiantIceSnake });
            MonsterStatuetteLookup.Add(typeof(LavaSerpent), new MonsterStatuetteType[] { MonsterStatuetteType.GiantLavaSnake });
            MonsterStatuetteLookup.Add(typeof(SilverSerpent), new MonsterStatuetteType[] { MonsterStatuetteType.SilverSerpent });
            MonsterStatuetteLookup.Add(typeof(BlackWidow), new MonsterStatuetteType[] { MonsterStatuetteType.BlackWidowSpider });
            MonsterStatuetteLookup.Add(typeof(DreadSpider), new MonsterStatuetteType[] { MonsterStatuetteType.DreadSpider });
            MonsterStatuetteLookup.Add(typeof(FrostSpider), new MonsterStatuetteType[] { MonsterStatuetteType.FrostSpider });
            MonsterStatuetteLookup.Add(typeof(GiantSpider), new MonsterStatuetteType[] { MonsterStatuetteType.GiantSpider, MonsterStatuetteType.Spider});
            MonsterStatuetteLookup.Add(typeof(SwampTentacle), new MonsterStatuetteType[] { MonsterStatuetteType.SwampTentacles });
            
            MonsterStatuetteLookup.Add(typeof(Unicorn), new MonsterStatuetteType[] { MonsterStatuetteType.Unicorn });
            MonsterStatuetteLookup.Add(typeof(DireWolf), new MonsterStatuetteType[] { MonsterStatuetteType.DireWolf, MonsterStatuetteType.Wolf });
            MonsterStatuetteLookup.Add(typeof(GreyWolf), new MonsterStatuetteType[] { MonsterStatuetteType.GrayWolf, MonsterStatuetteType.Wolf });
            MonsterStatuetteLookup.Add(typeof(WolfMount), new MonsterStatuetteType[] { MonsterStatuetteType.SilverWolf, MonsterStatuetteType.Wolf });
            MonsterStatuetteLookup.Add(typeof(TimberWolf), new MonsterStatuetteType[] { MonsterStatuetteType.TimberWolf, MonsterStatuetteType.Wolf, MonsterStatuetteType.TimberWolf });
            MonsterStatuetteLookup.Add(typeof(Wyvern), new MonsterStatuetteType[] { MonsterStatuetteType.Wyvern });
            MonsterStatuetteLookup.Add(typeof(AcidElemental), new MonsterStatuetteType[] { MonsterStatuetteType.AcidElemental });
            MonsterStatuetteLookup.Add(typeof(AirElemental), new MonsterStatuetteType[] { MonsterStatuetteType.AirElemental, MonsterStatuetteType.AirElemental_Old });
            MonsterStatuetteLookup.Add(typeof(BloodElemental), new MonsterStatuetteType[] { MonsterStatuetteType.BloodElemental });
            MonsterStatuetteLookup.Add(typeof(FireElemental), new MonsterStatuetteType[] { MonsterStatuetteType.FireElemental, MonsterStatuetteType.FireElemental_Old });
            MonsterStatuetteLookup.Add(typeof(IceElemental), new MonsterStatuetteType[] { MonsterStatuetteType.IceElemental });
            MonsterStatuetteLookup.Add(typeof(PoisonElemental), new MonsterStatuetteType[] { MonsterStatuetteType.PoisonElemental });
            MonsterStatuetteLookup.Add(typeof(SnowElemental), new MonsterStatuetteType[] { MonsterStatuetteType.SnowElemental });
            //MonsterStatuetteLookup.Add(typeof(Adranath), new MonsterStatuetteType[] { MonsterStatuetteType.Adranath }); //Skip
            MonsterStatuetteLookup.Add(typeof(Blackthorn), new MonsterStatuetteType[] { MonsterStatuetteType.Blackthorn });
            //MonsterStatuetteLookup.Add(typeof(CaptainDasha), new MonsterStatuetteType[] { MonsterStatuetteType.CaptainDasha });
            MonsterStatuetteLookup.Add(typeof(Dawn), new MonsterStatuetteType[] { MonsterStatuetteType.Dawn });
            //MonsterStatuetteLookup.Add(typeof(Kabur), new MonsterStatuetteType[] { MonsterStatuetteType.Kabur });
            MonsterStatuetteLookup.Add(typeof(BlackSolenQueen), new MonsterStatuetteType[] { MonsterStatuetteType.SolenQueen });
            MonsterStatuetteLookup.Add(typeof(RedSolenQueen), new MonsterStatuetteType[] { MonsterStatuetteType.SolenQueen });
            MonsterStatuetteLookup.Add(typeof(BlackSolenWarrior), new MonsterStatuetteType[] { MonsterStatuetteType.SolenWarrior });
            MonsterStatuetteLookup.Add(typeof(RedSolenWarrior), new MonsterStatuetteType[] { MonsterStatuetteType.SolenWarrior });
            MonsterStatuetteLookup.Add(typeof(BlackSolenWorker), new MonsterStatuetteType[] { MonsterStatuetteType.SolenWorker });
            MonsterStatuetteLookup.Add(typeof(RedSolenWorker), new MonsterStatuetteType[] { MonsterStatuetteType.SolenWorker });
            MonsterStatuetteLookup.Add(typeof(ArcaneDaemon), new MonsterStatuetteType[] { MonsterStatuetteType.ArcaneDemon });
            MonsterStatuetteLookup.Add(typeof(Betrayer), new MonsterStatuetteType[] { MonsterStatuetteType.Betrayer });
            MonsterStatuetteLookup.Add(typeof(Bogling), new MonsterStatuetteType[] { MonsterStatuetteType.Bogling });
            MonsterStatuetteLookup.Add(typeof(BogThing), new MonsterStatuetteType[] { MonsterStatuetteType.BogThing });
            MonsterStatuetteLookup.Add(typeof(ChaosDaemon), new MonsterStatuetteType[] { MonsterStatuetteType.ChaosDemon });
           // MonsterStatuetteLookup.Add(typeof(Chariot), new MonsterStatuetteType[] { MonsterStatuetteType.Chariot });
            MonsterStatuetteLookup.Add(typeof(ExodusMinion), new MonsterStatuetteType[] { MonsterStatuetteType.ExodusMinion });
            MonsterStatuetteLookup.Add(typeof(ExodusOverseer), new MonsterStatuetteType[] { MonsterStatuetteType.ExodusOverseer });
            MonsterStatuetteLookup.Add(typeof(Doppleganger), new MonsterStatuetteType[] { MonsterStatuetteType.DoppleGanger });
            MonsterStatuetteLookup.Add(typeof(Moloch), new MonsterStatuetteType[] { MonsterStatuetteType.MolochDemon }); //Four-Arm Demon
            MonsterStatuetteLookup.Add(typeof(Beetle), new MonsterStatuetteType[] { MonsterStatuetteType.GiantBeetle });
            MonsterStatuetteLookup.Add(typeof(Golem), new MonsterStatuetteType[] { MonsterStatuetteType.Golem });
            MonsterStatuetteLookup.Add(typeof(HordeMinion), new MonsterStatuetteType[] { MonsterStatuetteType.HordeMinion });
            MonsterStatuetteLookup.Add(typeof(Juggernaut), new MonsterStatuetteType[] { MonsterStatuetteType.Juggernaut });
            MonsterStatuetteLookup.Add(typeof(PlagueBeast), new MonsterStatuetteType[] { MonsterStatuetteType.PlagueBeast });
            MonsterStatuetteLookup.Add(typeof(Quagmire), new MonsterStatuetteType[] { MonsterStatuetteType.Quagmire });
            MonsterStatuetteLookup.Add(typeof(Ridgeback), new MonsterStatuetteType[] { MonsterStatuetteType.Ridgeback });
            MonsterStatuetteLookup.Add(typeof(SandVortex), new MonsterStatuetteType[] { MonsterStatuetteType.SandVortex });
            MonsterStatuetteLookup.Add(typeof(SkeletalMount), new MonsterStatuetteType[] { MonsterStatuetteType.SkeletalMount });
           // MonsterStatuetteLookup.Add(typeof(Sphinx), new MonsterStatuetteType[] { MonsterStatuetteType.Sphinx });
            MonsterStatuetteLookup.Add(typeof(SwampDragon), new MonsterStatuetteType[] { MonsterStatuetteType.SwampDragon });
            //MonsterStatuetteLookup.Add(typeof(Swarm), new MonsterStatuetteType[] { MonsterStatuetteType.Swarm });
            MonsterStatuetteLookup.Add(typeof(PlagueBeastLord), new MonsterStatuetteType[] { MonsterStatuetteType.PlagueBeastLord });
            MonsterStatuetteLookup.Add(typeof(MeerMage), new MonsterStatuetteType[] { MonsterStatuetteType.MeerMage });
            MonsterStatuetteLookup.Add(typeof(MeerWarrior), new MonsterStatuetteType[] { MonsterStatuetteType.MeerWarrior });
            MonsterStatuetteLookup.Add(typeof(JukaMage), new MonsterStatuetteType[] { MonsterStatuetteType.JukaMage });
            MonsterStatuetteLookup.Add(typeof(JukaWarrior), new MonsterStatuetteType[] { MonsterStatuetteType.JukaWarrior });
            MonsterStatuetteLookup.Add(typeof(CrystalElemental), new MonsterStatuetteType[] { MonsterStatuetteType.CrystalElemental });
            MonsterStatuetteLookup.Add(typeof(Treefellow), new MonsterStatuetteType[] { MonsterStatuetteType.Treefellow });
            MonsterStatuetteLookup.Add(typeof(SkitteringHopper), new MonsterStatuetteType[] { MonsterStatuetteType.SkitteringHopper });
            MonsterStatuetteLookup.Add(typeof(Devourer), new MonsterStatuetteType[] { MonsterStatuetteType.DevourerSouls });
            MonsterStatuetteLookup.Add(typeof(FleshGolem), new MonsterStatuetteType[] { MonsterStatuetteType.FleshGolem });
            MonsterStatuetteLookup.Add(typeof(GoreFiend), new MonsterStatuetteType[] { MonsterStatuetteType.GoreFiend });
            MonsterStatuetteLookup.Add(typeof(Impaler), new MonsterStatuetteType[] { MonsterStatuetteType.Impaler });
            MonsterStatuetteLookup.Add(typeof(Gibberling), new MonsterStatuetteType[] { MonsterStatuetteType.Gibberling });
            MonsterStatuetteLookup.Add(typeof(BoneDaemon), new MonsterStatuetteType[] { MonsterStatuetteType.BoneDaemon });
            MonsterStatuetteLookup.Add(typeof(PatchworkSkeleton), new MonsterStatuetteType[] { MonsterStatuetteType.PatchworkSkeleton });
            MonsterStatuetteLookup.Add(typeof(WailingBanshee), new MonsterStatuetteType[] { MonsterStatuetteType.WailingBanshee, MonsterStatuetteType.Ghost });
            MonsterStatuetteLookup.Add(typeof(ShadowKnight), new MonsterStatuetteType[] { MonsterStatuetteType.ShadowKnight });
            MonsterStatuetteLookup.Add(typeof(AbysmalHorror), new MonsterStatuetteType[] { MonsterStatuetteType.AbysmalHorror });
            MonsterStatuetteLookup.Add(typeof(DarknightCreeper), new MonsterStatuetteType[] { MonsterStatuetteType.DarknightCreeper });
            MonsterStatuetteLookup.Add(typeof(Ravager), new MonsterStatuetteType[] { MonsterStatuetteType.Ravager });
            MonsterStatuetteLookup.Add(typeof(FleshRenderer), new MonsterStatuetteType[] { MonsterStatuetteType.FleshRenderer });
            MonsterStatuetteLookup.Add(typeof(Wanderer), new MonsterStatuetteType[] { MonsterStatuetteType.WandererVoid });
            MonsterStatuetteLookup.Add(typeof(VampireBat), new MonsterStatuetteType[] { MonsterStatuetteType.VampireBat });
            MonsterStatuetteLookup.Add(typeof(DemonKnight), new MonsterStatuetteType[] { MonsterStatuetteType.DemonKnight });
            MonsterStatuetteLookup.Add(typeof(MoundOfMaggots), new MonsterStatuetteType[] { MonsterStatuetteType.MoundMaggots });
            MonsterStatuetteLookup.Add(typeof(BakeKitsune), new MonsterStatuetteType[] { MonsterStatuetteType.BakeKitsune });
            MonsterStatuetteLookup.Add(typeof(Crane), new MonsterStatuetteType[] { MonsterStatuetteType.Crane });
            MonsterStatuetteLookup.Add(typeof(DeathwatchBeetle), new MonsterStatuetteType[] { MonsterStatuetteType.DeathwatchBeetle });
            //MonsterStatuetteLookup.Add(typeof(Raiju), new MonsterStatuetteType[] { MonsterStatuetteType.Raiju });
            MonsterStatuetteLookup.Add(typeof(FanDancer), new MonsterStatuetteType[] { MonsterStatuetteType.FanDancer });
            MonsterStatuetteLookup.Add(typeof(Gaman), new MonsterStatuetteType[] { MonsterStatuetteType.Gaman });
            //MonsterStatuetteLookup.Add(typeof(GouzanHa), new MonsterStatuetteType[] { MonsterStatuetteType.GouzanHa });
            MonsterStatuetteLookup.Add(typeof(Hiryu), new MonsterStatuetteType[] { MonsterStatuetteType.Hiryu });
            MonsterStatuetteLookup.Add(typeof(Kappa), new MonsterStatuetteType[] { MonsterStatuetteType.Kappa });
            //MonsterStatuetteLookup.Add(typeof(LadySnow), new MonsterStatuetteType[] { MonsterStatuetteType.LadySnow });
            MonsterStatuetteLookup.Add(typeof(Oni), new MonsterStatuetteType[] { MonsterStatuetteType.Oni });
            MonsterStatuetteLookup.Add(typeof(RevenantLion), new MonsterStatuetteType[] { MonsterStatuetteType.RevenantLion });
            MonsterStatuetteLookup.Add(typeof(RuneBeetle), new MonsterStatuetteType[] { MonsterStatuetteType.RuneBeetle });
            MonsterStatuetteLookup.Add(typeof(TsukiWolf), new MonsterStatuetteType[] { MonsterStatuetteType.TsukiWolf });
            MonsterStatuetteLookup.Add(typeof(Yamandon), new MonsterStatuetteType[] { MonsterStatuetteType.Yamandon });
            MonsterStatuetteLookup.Add(typeof(YomotsuElder), new MonsterStatuetteType[] { MonsterStatuetteType.YomotsuElder });
            MonsterStatuetteLookup.Add(typeof(YomotsuPriest), new MonsterStatuetteType[] { MonsterStatuetteType.YomotsuPriest });
            MonsterStatuetteLookup.Add(typeof(YomotsuWarrior), new MonsterStatuetteType[] { MonsterStatuetteType.YomotsuWarrior });
            MonsterStatuetteLookup.Add(typeof(FireBeetle), new MonsterStatuetteType[] { MonsterStatuetteType.FireBeetle });
            //MonsterStatuetteLookup.Add(typeof(ChiefParoxysmus), new MonsterStatuetteType[] { MonsterStatuetteType.ChiefParoxysmus });
           // MonsterStatuetteLookup.Add(typeof(DreadHorn), new MonsterStatuetteType[] { MonsterStatuetteType.DreadHorn });
            //MonsterStatuetteLookup.Add(typeof(LadyMelisande), new MonsterStatuetteType[] { MonsterStatuetteType.LadyMelisande });
            //MonsterStatuetteLookup.Add(typeof(MonstrousInterred), new MonsterStatuetteType[] { MonsterStatuetteType.MonstrousInterred });
           // MonsterStatuetteLookup.Add(typeof(ShimmeringEffusion), new MonsterStatuetteType[] { MonsterStatuetteType.ShimmeringEffusion });
            MonsterStatuetteLookup.Add(typeof(TormentedMinotaur), new MonsterStatuetteType[] { MonsterStatuetteType.TormentedMinotaur });
            MonsterStatuetteLookup.Add(typeof(Changeling), new MonsterStatuetteType[] { MonsterStatuetteType.Changeling });
            //MonsterStatuetteLookup.Add(typeof(Hydra), new MonsterStatuetteType[] { MonsterStatuetteType.Hydra });
            MonsterStatuetteLookup.Add(typeof(MLDryad), new MonsterStatuetteType[] { MonsterStatuetteType.Dryad });
            MonsterStatuetteLookup.Add(typeof(Troglodyte), new MonsterStatuetteType[] { MonsterStatuetteType.Troglodyte });
            MonsterStatuetteLookup.Add(typeof(Satyr), new MonsterStatuetteType[] { MonsterStatuetteType.Satyr });
            MonsterStatuetteLookup.Add(typeof(FetidEssence), new MonsterStatuetteType[] { MonsterStatuetteType.FetidEssence });
            MonsterStatuetteLookup.Add(typeof(MantraEffervescence), new MonsterStatuetteType[] { MonsterStatuetteType.MantraEffervescense });
            MonsterStatuetteLookup.Add(typeof(CorporealBrume), new MonsterStatuetteType[] { MonsterStatuetteType.CorporealBrume });
            //MonsterStatuetteLookup.Add(typeof(Chimera), new MonsterStatuetteType[] { MonsterStatuetteType.Chimera });
            MonsterStatuetteLookup.Add(typeof(CuSidhe), new MonsterStatuetteType[] { MonsterStatuetteType.CuSidhe });
            MonsterStatuetteLookup.Add(typeof(Squirrel), new MonsterStatuetteType[] { MonsterStatuetteType.Squirrel });
            //MonsterStatuetteLookup.Add(typeof(Farret), new MonsterStatuetteType[] { MonsterStatuetteType.Farret });
            MonsterStatuetteLookup.Add(typeof(Minotaur), new MonsterStatuetteType[] { MonsterStatuetteType.MinotaurBoss1 });
            MonsterStatuetteLookup.Add(typeof(MinotaurCaptain), new MonsterStatuetteType[] { MonsterStatuetteType.MinotaurBoss2 });
            MonsterStatuetteLookup.Add(typeof(Parrot), new MonsterStatuetteType[] { MonsterStatuetteType.Parrot });
            //MonsterStatuetteLookup.Add(typeof(MondainSteed), new MonsterStatuetteType[] { MonsterStatuetteType.MondainSteed });
            //MonsterStatuetteLookup.Add(typeof(PhillipsWoodenSteed), new MonsterStatuetteType[] { MonsterStatuetteType.PhillipsWoodenSteed });
            MonsterStatuetteLookup.Add(typeof(RedDeath), new MonsterStatuetteType[] { MonsterStatuetteType.RedDeath });
        }
        
        private MonsterStatuetteType m_Type;
		private bool m_TurnedOn;
		private bool m_IsRewardItem;

		[CommandProperty( AccessLevel.GameMaster )]
		public bool IsRewardItem
		{
			get{ return m_IsRewardItem; }
			set{ m_IsRewardItem = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool TurnedOn
		{
			get{ return m_TurnedOn; }
			set{ m_TurnedOn = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public MonsterStatuetteType Type
		{
			get{ return m_Type; }
			set
			{
				m_Type = value;
				ItemID = MonsterStatuetteInfo.GetInfo( m_Type ).ItemID;
				InvalidateProperties();
			}
		}

		public override double DefaultWeight{ get { return 1.0; } }

		public override void AddNameProperty( ObjectPropertyList list )
		{
			list.Add( String.IsNullOrEmpty( base.Name ) ? (MonsterStatuetteInfo.GetInfo( m_Type ).Name + " Statuette") : base.Name );
		}

		[Constructable]
		public MonsterStatuette() : this( MonsterStatuetteType.Crocodile )
		{
		}

		[Constructable]
		public MonsterStatuette( MonsterStatuetteType type ) : base( MonsterStatuetteInfo.GetInfo( type ).ItemID )
		{
			LootType = LootType.Regular;

			m_Type = type;
			Dyable = true;
		    double rand = Utility.RandomDouble();
		    if (0.001 >= rand)
		    {
		        Hue = 1161;
		    }
            else if (0.01 >= rand)
            {
                Hue = 1153;             
            }
            else if (0.05 >= rand)
            {
                Hue = 1266; 
            }
		}

		public override Type DyeType{ get{ return typeof(StatuetteDyeTub); } }
		public override bool DisplayDyable{ get{ return false; } }

		public override bool HandlesOnMovement{ get{ return m_TurnedOn && IsLockedDown; } }

		public override void OnMovement( Mobile m, Point3D oldLocation )
		{
			if ( m_TurnedOn && IsLockedDown && (!m.Hidden || m.AccessLevel == AccessLevel.Player) && Utility.InRange( m.Location, this.Location, 2 ) && !Utility.InRange( oldLocation, this.Location, 2 ) )
			{
                try
                {
                    int[] sounds = MonsterStatuetteInfo.GetInfo(m_Type).Sounds;

                    Effects.PlaySound(this.Location, this.Map, sounds[Utility.Random(sounds.Length)]);
                }
                catch
                {
					LoggingCustom.Log("ERROR_MonsterStatuette.txt", "Type " + m_Type + " did not have a valid sound!");
                }
			}

			base.OnMovement( m, oldLocation );
		}

		public MonsterStatuette( Serial serial ) : base( serial )
		{
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			if ( m_TurnedOn )
				list.Add( 502695 ); // turned on
			else
				list.Add( 502696 ); // turned off
		}

		public override void OnSingleClick( Mobile from )
		{
			LabelToExpansion(from);

			if ( String.IsNullOrEmpty( Name ) )
				LabelTo( from, String.Format( "a {0} statuette ({1})", MonsterStatuetteInfo.GetInfo( m_Type ).Name.ToLower(), m_TurnedOn ? "ON" : "OFF" ) );
		}

		public bool IsOwner( Mobile mob )
		{
			BaseHouse house = BaseHouse.FindHouseAt( this );

			return ( house != null && house.IsOwner( mob ) );
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( IsOwner( from ) )
			{
				OnOffGump onOffGump = new OnOffGump( this );
				from.SendGump( onOffGump );
			}
			else
			{
				from.SendLocalizedMessage( 502691 ); // You must be the owner to use this.
			}
		}

		private class OnOffGump : Gump
		{
			private MonsterStatuette m_Statuette;

			public OnOffGump( MonsterStatuette statuette ) : base( 150, 200 )
			{
				m_Statuette = statuette;

				AddBackground( 0, 0, 300, 150, 0xA28 );

				AddHtmlLocalized( 45, 20, 300, 35, statuette.TurnedOn ? 1011035 : 1011034, false, false ); // [De]Activate this item

				AddButton( 40, 53, 0xFA5, 0xFA7, 1, GumpButtonType.Reply, 0 );
				AddHtmlLocalized( 80, 55, 65, 35, 1011036, false, false ); // OKAY

				AddButton( 150, 53, 0xFA5, 0xFA7, 0, GumpButtonType.Reply, 0 );
				AddHtmlLocalized( 190, 55, 100, 35, 1011012, false, false ); // CANCEL
			}

			public override void OnResponse( NetState sender, RelayInfo info )
			{
				Mobile from = sender.Mobile;

				if ( info.ButtonID == 1 )
				{
					bool newValue = !m_Statuette.TurnedOn;
					m_Statuette.TurnedOn = newValue;

					if ( newValue && !m_Statuette.IsLockedDown )
						from.SendLocalizedMessage( 502693 ); // Remember, this only works when locked down.
				}
				else
				{
					from.SendLocalizedMessage( 502694 ); // Cancelled action.
				}
			}
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version

			writer.WriteEncodedInt( (int) m_Type );
			writer.Write( (bool) m_TurnedOn );
			writer.Write( (bool) m_IsRewardItem );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();

			switch ( version )
			{
				case 0:
				{
					m_Type = (MonsterStatuetteType)reader.ReadEncodedInt();
					m_TurnedOn = reader.ReadBool();
					m_IsRewardItem = reader.ReadBool();
					break;
				}
			}
		}
	}
}