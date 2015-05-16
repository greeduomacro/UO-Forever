#region References

using System;
using System.Collections.Generic;
using System.Linq;
using Server.Mobiles;

#endregion

namespace Server.Items
{
    public class SlayerGroup
    {
        private static readonly SlayerEntry[] m_TotalEntries;
        private static readonly SlayerGroup[] m_Groups;

        public static SlayerEntry[] TotalEntries { get { return m_TotalEntries; } }

        public static SlayerGroup[] Groups { get { return m_Groups; } }

        public static SlayerEntry GetEntryByName(SlayerName name)
        {
            var v = (int) name;

            if (v >= 0 && v < m_TotalEntries.Length)
            {
                return m_TotalEntries[v];
            }

            return null;
        }

        public static SlayerName GetLootSlayerType(Type type)
        {
            foreach (SlayerGroup grp in m_Groups)
            {
                Type[] foundOn = grp.FoundOn;

                bool inGroup = false;

                for (int j = 0; foundOn != null && !inGroup && j < foundOn.Length; ++j)
                {
                    inGroup = (foundOn[j] == type);
                }

                if (inGroup)
                {
                    int index = Utility.Random(1 + grp.Entries.Length);

                    if (index == 0)
                    {
                        return grp.Super.Name;
                    }

                    return grp.Entries[index - 1].Name;
                }
            }

            return SlayerName.Silver;
        }

        static SlayerGroup()
        {
            var humanoid = new SlayerGroup();
            var undead = new SlayerGroup();
            var elemental = new SlayerGroup();
            var abyss = new SlayerGroup();
            var arachnid = new SlayerGroup();
            var reptilian = new SlayerGroup();
            var fey = new SlayerGroup();

            humanoid.Opposition = new[] {undead};
            humanoid.FoundOn = new[] {typeof(BoneKnight), typeof(Lich), typeof(LichLord)};
            humanoid.Super = new SlayerEntry(
                SlayerName.Repond,
                typeof(ArcticOgreLord),
                typeof(Cyclops),
                typeof(Ettin),
                typeof(EvilMage),
                typeof(EvilMageLord),
                typeof(FrostTroll),
                typeof(MeerCaptain),
                typeof(MeerEternal),
                typeof(MeerMage),
                typeof(MeerWarrior),
                typeof(Ogre),
                typeof(OgreLord),
                typeof(Orc),
                typeof(OrcBomber),
                typeof(OrcBrute),
                typeof(OrcCaptain),
                typeof(OrcishLord),
                typeof(OrcishMage),
                typeof(Ratman),
                typeof(RatmanArcher),
                typeof(RatmanMage),
                typeof(SavageRider),
                typeof(SavageShaman),
                typeof(Savage),
                typeof(Titan),
                typeof(Troglodyte),
                typeof(Troll),
                typeof(OrcishMineOverseer),
                typeof(OrcLeader),
                typeof(LummoxMagePortal),
                typeof(LummoxWarHeroPortal),
                typeof(LummoxWarriorPortal),
                typeof(MinotaurWarHeroPortal),
                typeof(MinotaurWarriorPortal),
                typeof(OrcMineBomber),
                typeof(OrcMiner));
            humanoid.Entries = new[]
            {
                new SlayerEntry(SlayerName.OgreTrashing, 
                    typeof(Ogre), 
                    typeof(OgreLord), 
                    typeof(ArcticOgreLord)),

                new SlayerEntry(SlayerName.OrcSlaying,
                    typeof(Orc),
                    typeof(OrcBomber),
                    typeof(OrcBrute),
                    typeof(OrcCaptain),
                    typeof(OrcishLord),
                    typeof(OrcishMage),
                    typeof(OrcishMineOverseer),
                    typeof(OrcMiner),
                    typeof(OrcLeader),
                    typeof(OrcMineBomber)),

                new SlayerEntry(SlayerName.TrollSlaughter, 
                    typeof(Troll), 
                    typeof(FrostTroll))
            };

            undead.Opposition = new[] {humanoid};
            undead.Super = new SlayerEntry(SlayerName.Silver,
                typeof(AncientLich),
                typeof(Bogle),
                typeof(BoneKnight),
                typeof(BoneMage),
                typeof(DarknightCreeper),
                typeof(FleshGolem),
                typeof(Ghoul),
                typeof(GoreFiend),
                typeof(HellSteed),
                typeof(LadyOfTheSnow),
                typeof(Lich),
                typeof(LichLord),
                typeof(Mummy),
                typeof(PestilentBandage),
                typeof(Revenant),
                typeof(RevenantLion),
                typeof(RottingCorpse),
                typeof(Shade),
                typeof(ShadowKnight),
                typeof(SkeletalKnight),
                typeof(SkeletalMage),
                typeof(SkeletalMount),
                typeof(Skeleton),
                typeof(Spectre),
                typeof(Wraith),
                typeof(DreamWraithPortal),
                typeof(MaddeningHorrorPortal),
                typeof(UndeadWarDogPortal),
                typeof(Zombie));
            undead.Entries = new SlayerEntry[0];

            fey.Opposition = new[] {abyss};
            fey.Super = new SlayerEntry(
                SlayerName.Fey,
                typeof(Centaur),
                typeof(CuSidhe),
                typeof(EtherealWarrior),
                typeof(Kirin),
                typeof(LordOaks),
                typeof(Pixie),
                typeof(Silvani),
                typeof(Treefellow),
                typeof(Unicorn),
                typeof(Wisp),
                typeof(MLDryad),
                typeof(Satyr));
            fey.Entries = new SlayerEntry[0];

            elemental.Opposition = new[] {abyss};
            elemental.FoundOn = new[] {typeof(Balron), typeof(Daemon)};
            elemental.Super = new SlayerEntry(SlayerName.ElementalBan,
                typeof(AcidElemental),
                typeof(ToxicElemental),
                typeof(AgapiteElemental),
                typeof(AirElemental),
                typeof(SummonedAirElemental),
                typeof(BloodElemental),
                typeof(BronzeElemental),
                typeof(CopperElemental),
                typeof(CrystalElemental),
                typeof(CrystalVortex),
                typeof(DullCopperElemental),
                typeof(EarthElemental),
                typeof(SummonedEarthElemental),
                typeof(Efreet),
                typeof(FireElemental),
                typeof(SummonedFireElemental),
                typeof(GoldenElemental),
                typeof(GreaterBloodElemental),
                typeof(IceElemental),
                typeof(KazeKemono),
                typeof(PoisonElemental),
                typeof(RaiJu),
                typeof(SandVortex),
                typeof(ShadowIronElemental),
                typeof(SnowElemental),
                typeof(ValoriteElemental),
                typeof(VeriteElemental),
                typeof(WaterElemental),
                typeof(SummonedWaterElemental),
                typeof(DeepEarthElemental),
                typeof(DeepWaterElemental),
                typeof(GreaterPoisonElemental),
                typeof(MagmaElemental),
                typeof(MagnetiteElemental),
                typeof(PyroclasticElemental));

            elemental.Entries = new[]
            {
                new SlayerEntry(SlayerName.BloodDrinking, 
                    typeof(BloodElemental),
                    typeof(GreaterBloodElemental)),

                new SlayerEntry(SlayerName.EarthShatter,
                    typeof(AgapiteElemental),
                    typeof(BronzeElemental),
                    typeof(CopperElemental),
                    typeof(CrystalVortex),
                    typeof(DullCopperElemental),
                    typeof(EarthElemental),
                    typeof(SummonedEarthElemental),
                    typeof(GoldenElemental),
                    typeof(GreaterBloodElemental),
                    typeof(ShadowIronElemental),
                    typeof(ValoriteElemental),
                    typeof(VeriteElemental),
                    typeof(DeepEarthElemental),
                    typeof(MagnetiteElemental)),

                new SlayerEntry(SlayerName.ElementalHealth, 
                    typeof(PoisonElemental), 
                    typeof(GreaterPoisonElemental)),

                new SlayerEntry(SlayerName.FlameDousing,
                    typeof(FireElemental),
                    typeof(SummonedFireElemental),
                    typeof(MagmaElemental),
                    typeof(PyroclasticElemental)),

                new SlayerEntry(SlayerName.SummerWind, 
                    typeof(SnowElemental), 
                    typeof(IceElemental)),

                new SlayerEntry(SlayerName.Vacuum, 
                    typeof(AirElemental), 
                    typeof(SummonedAirElemental)),

                new SlayerEntry(SlayerName.WaterDissipation, 
                    typeof(WaterElemental), 
                    typeof(SummonedWaterElemental),
                    typeof(DeepWaterElemental))
            };

            abyss.Opposition = new[] {elemental, fey};
            abyss.FoundOn = new[] {typeof(BloodElemental)};

            abyss.Super = new SlayerEntry(
                SlayerName.Exorcism,
                typeof(AbysmalHorror),
                typeof(Balron),
                typeof(BoneDaemon),
                typeof(ChaosDaemon),
                typeof(Daemon),
                typeof(SummonedDaemon),
                typeof(DemonKnight),
                typeof(Devourer),
                typeof(Gargoyle),
                typeof(FireGargoyle),
                typeof(Gibberling),
                typeof(HordeMinion),
                typeof(IceFiend),
                typeof(Imp),
                typeof(NetherImp),
                typeof(BurningImp),
                typeof(Impaler),
                typeof(Ravager),
                typeof(StoneGargoyle),
                typeof(ArcaneDaemon),
                typeof(EnslavedGargoyle),
                typeof(GargoyleDestroyer),
                typeof(GargoyleEnforcer),
                typeof(DevourerPortal),
                typeof(AbysmalHorrorPortal),
                typeof(DarkFatherPortal),
                typeof(Moloch));

            abyss.Entries = new[]
            {
                new SlayerEntry(
                    SlayerName.DaemonDismissal,
                    typeof(AbysmalHorror),
                    typeof(Balron),
                    typeof(BoneDaemon),
                    typeof(ChaosDaemon),
                    typeof(Daemon),
                    typeof(SummonedDaemon),
                    typeof(DemonKnight),
                    typeof(Devourer),
                    typeof(Gibberling),
                    typeof(HordeMinion),
                    typeof(IceFiend),
                    typeof(Imp),
                    typeof(Impaler),
                    typeof(Ravager),
                    typeof(ArcaneDaemon),
                    typeof(Moloch)),

                new SlayerEntry(SlayerName.GargoylesFoe,
                    typeof(FireGargoyle),
                    typeof(Gargoyle),
                    typeof(StoneGargoyle),
                    typeof(EnslavedGargoyle),
                    typeof(GargoyleDestroyer),
                    typeof(GargoyleEnforcer)),

                new SlayerEntry(SlayerName.BalronDamnation, 
                    typeof(Balron))
            };

            arachnid.Opposition = new[] {reptilian};
            arachnid.FoundOn = new[]
            {
                typeof(AncientWyrm), 
                typeof(GreaterDragon), 
                typeof(Dragon), 
                typeof(OphidianMatriarch), 
                typeof(ShadowWyrm),
                typeof(DragonPortal), 
                typeof(AncientWyrmPortal), 
                typeof(ShadowWyrmPortal), 
                typeof(Bahamut)
            };
            arachnid.Super = new SlayerEntry(
                SlayerName.ArachnidDoom,
                typeof(DreadSpider),
                typeof(FrostSpider),
                typeof(GiantBlackWidow),
                typeof(GiantSpider),
                typeof(Mephitis),
                typeof(Scorpion),
                typeof(TerathanAvenger),
                typeof(TerathanDrone),
                typeof(TerathanMatriarch),
                typeof(TerathanWarrior),
                typeof(AbnormalDreadSpider));
            arachnid.Entries = new[]
            {
                new SlayerEntry(SlayerName.ScorpionsBane,
                    typeof(Scorpion)),

                new SlayerEntry(
                    SlayerName.SpidersDeath,
                    typeof(DreadSpider),
                    typeof(FrostSpider),
                    typeof(GiantBlackWidow),
                    typeof(GiantSpider),
                    typeof(Mephitis),
                    typeof(AbnormalDreadSpider)),

                new SlayerEntry(
                    SlayerName.Terathan,
                    typeof(TerathanAvenger),
                    typeof(TerathanDrone),
                    typeof(TerathanMatriarch),
                    typeof(TerathanWarrior))
            };

            reptilian.Opposition = new[] {arachnid};
            reptilian.FoundOn = new[] {typeof(TerathanAvenger), typeof(TerathanMatriarch)};
            reptilian.Super = new SlayerEntry(
                SlayerName.ReptilianDeath,
                typeof(AncientWyrm),
                typeof(DeepSeaSerpent),
                typeof(GreaterDragon),
                typeof(ElderDragon),
                typeof(Dragon), 
                typeof(DragonPortal), 
                typeof(AncientWyrmPortal), 
                typeof(ShadowWyrmPortal),
                typeof(Bahamut),
                typeof(Drake),
                typeof(GiantIceWorm),
                typeof(IceSerpent),
                typeof(GiantSerpent),
                typeof(Hiryu),
                typeof(IceSnake),
                typeof(JukaLord),
                typeof(JukaMage),
                typeof(JukaWarrior),
                typeof(LavaSerpent),
                typeof(LavaSnake),
                typeof(LesserHiryu),
                typeof(Lizardman),
                typeof(OphidianArchmage),
                typeof(OphidianKnight),
                typeof(OphidianMage),
                typeof(OphidianMatriarch),
                typeof(OphidianWarrior),
                typeof(Reptalon),
                typeof(SeaSerpent),
                typeof(Serado),
                typeof(SerpentineDragon),
                typeof(ShadowWyrm),
                typeof(SilverSerpent),
                typeof(SkeletalDragon),
                typeof(Snake),
                typeof(SwampDragon),
                typeof(WhiteWyrm),
                typeof(PathaleoDrake),
                typeof(Wyvern),
                typeof(Yamandon));
            reptilian.Entries = new[]
            {
                new SlayerEntry(
                    SlayerName.DragonSlaying,
                    typeof(AncientWyrm), 
                    typeof(DragonPortal), 
                    typeof(AncientWyrmPortal), 
                    typeof(ShadowWyrmPortal),
                    typeof(Bahamut),
                    typeof(GreaterDragon),
                    typeof(ElderDragon),
                    typeof(Dragon),
                    typeof(Drake),
                    typeof(Hiryu),
                    typeof(LesserHiryu),
                    typeof(Reptalon),
                    typeof(SerpentineDragon),
                    typeof(ShadowWyrm),
                    typeof(SkeletalDragon),
                    typeof(SwampDragon),
                    typeof(WhiteWyrm),
                    typeof(PathaleoDrake),
                    typeof(Wyvern)),

                new SlayerEntry(SlayerName.LizardmanSlaughter,
                    typeof(Lizardman)),

                new SlayerEntry(
                    SlayerName.Ophidian,
                    typeof(OphidianArchmage),
                    typeof(OphidianKnight),
                    typeof(OphidianMage),
                    typeof(OphidianMatriarch),
                    typeof(OphidianWarrior)),

                new SlayerEntry(SlayerName.SnakesBane,
                    typeof(DeepSeaSerpent),
                    typeof(GiantIceWorm),
                    typeof(GiantSerpent),
                    typeof(IceSerpent),
                    typeof(IceSnake),
                    typeof(LavaSerpent),
                    typeof(LavaSnake),
                    typeof(SeaSerpent),
                    typeof(Serado),
                    typeof(SilverSerpent),
                    typeof(Snake),
                    typeof(Yamandon))
            };

            m_Groups = new[] {humanoid, undead, elemental, abyss, arachnid, reptilian, fey};

            m_TotalEntries = CompileEntries(m_Groups);
        }

        private static SlayerEntry[] CompileEntries(IEnumerable<SlayerGroup> groups)
        {
            var entries = new SlayerEntry[28];

            foreach (SlayerGroup g in groups)
            {
                g.Super.Group = g;

                entries[(int) g.Super.Name] = g.Super;

                foreach (SlayerEntry s in g.Entries)
                {
                    s.Group = g;
                    entries[(int) s.Name] = s;
                }
            }

            return entries;
        }

        public SlayerGroup[] Opposition { get; set; }
        public SlayerEntry Super { get; set; }
        public SlayerEntry[] Entries { get; set; }
        public Type[] FoundOn { get; set; }

        public bool OppositionSuperSlays(Mobile m)
        {
            return Opposition.Any(t => t.Super.Slays(m));
        }
    }
}