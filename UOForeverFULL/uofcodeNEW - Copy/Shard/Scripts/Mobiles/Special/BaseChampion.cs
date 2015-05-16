#region References

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Commands;
using Server.Engines.CannedEvil;
using Server.Engines.Craft;
using Server.Items;
using Server.Mobiles.MetaPet.Skills;
using VitaNex.FX;
using VitaNex.IO;
using VitaNex.Network;
using VitaNex.SuperGumps.UI;

#endregion

namespace Server.Mobiles
{
    public abstract class BaseChampion : BaseCreature
    {
        public static PowerScroll CreateRandomPowerScroll(Expansion e)
        {
            int level;
            double random = Utility.RandomDouble();

            if (0.05 >= random)
            {
                level = 20;
            }
            else if (0.25 >= random)
            {
                level = 15;
            }
            else if (0.50 >= random)
            {
                level = 10;
            }
            else
            {
                level = 5;
            }

            return PowerScroll.CreateRandom(level, level, e);
        }

        public static void GivePowerScrollTo(Mobile m, PowerScroll ps)
        {
            if (m == null || ps == null) //sanity
            {
                return;
            }

            m.SendLocalizedMessage(1049524); // You have received a scroll of power!

            m.AddToBackpack(ps);
        }

        #region Gold Explosion

        public virtual int GoldRange { get { return 10; } }
        public virtual int GoldMin { get { return 100; } }
        public virtual int GoldMax { get { return 200; } }

        public static void BeginGoldExplosion(
            ExplodeFX fx,
            Point3D center,
            Map map,
            int range,
            int minGold,
            int maxGold,
            int[] explodeSounds = null,
            int[] dropSounds = null)
        {
            BaseExplodeEffect efx = fx.CreateInstance(center, map, 2, 3);

            if (efx == null)
            {
                return;
            }

            minGold = Math.Max(10, Math.Min(60000, minGold));
            maxGold = Math.Max(minGold, Math.Min(60000, maxGold));

            explodeSounds = explodeSounds ?? new[] {284, 285, 286, 776, 1231};

            efx.AverageZ = false;

            efx.EffectHandler = e =>
            {
                if (Utility.RandomDouble() < 0.25)
                {
                    Effects.PlaySound(e.Source, e.Map, explodeSounds.GetRandom());
                }
            };

            efx.Callback = () =>
            {
                efx.Start = efx.Start.Clone3D(0, 0, 10);

                if (efx.CurProcess >= efx.Repeat)
                {
                    EndGoldExplosion(fx, center.Clone3D(0, 0, 10 * efx.Repeat), map, range, minGold, maxGold, dropSounds);
                }
            };

            efx.Send();
        }

        public static void EndGoldExplosion(
            ExplodeFX fx, Point3D center, Map map, int range, int minGold, int maxGold, int[] dropSounds = null)
        {
            BaseExplodeEffect efx = fx.CreateInstance(center, map, range);

            if (efx == null)
            {
                return;
            }

            efx.AverageZ = false;

            efx.Callback = () =>
            {
                var points = new List<Point3D>();

                center.ScanRange(
                    map,
                    range,
                    r =>
                    {
                        if (!r.Excluded)
                        {
                            if (r.QueryMap.CanFit(r.Current, 1, false, false) || r.QueryMap.HasWater(r.Current))
                            {
                                points.Add(r.Current);
                            }
                            else
                            {
                                r.Exclude();
                            }
                        }

                        return false;
                    });

                if (points.Count == 0)
                {
                    return;
                }

                dropSounds = dropSounds ?? new[] {553, 554};

                Timer goldTimer = null;

                goldTimer = Timer.DelayCall(
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromMilliseconds(100),
                    points.Count,
                    () =>
                    {
                        if (points.Count == 0)
                        {
                            if (goldTimer != null)
                            {
                                goldTimer.Running = false;
                                goldTimer = null;
                            }

                            return;
                        }

                        Point3D p = points.GetRandom();
                        points.Remove(p);

                        Effects.PlaySound(p, map, dropSounds.GetRandom());

                        new MovingEffectInfo(p.Clone3D(0, 0, Utility.RandomMinMax(30, 50)), p, map, 3823, 0, 10,
                            EffectRender.Lighten)
                            .MovingImpact(
                                e =>
                                {
                                    int amount = Utility.RandomMinMax(minGold, maxGold);

                                    if (amount <= 0)
                                    {
                                        return;
                                    }

                                    var g = new Gold(amount);
                                    g.MoveToWorld(e.Target.Location, e.Map);

                                    new EffectInfo(e.Target, e.Map, 14202, 51, 10, 40, EffectRender.Lighten).Send();
                                    Effects.PlaySound(e.Target, e.Map, g.GetDropSound());
                                });
                    });
            };

            efx.Send();
        }

        #endregion

        public abstract ChampionSkullType SkullType { get; }

        public virtual bool NoGoodies { get { return false; } }

        public virtual int PSToGive { get { return ChampionGlobals.CSOptions.PowerScrollsToGive; } }

        public virtual int FactionPSToGive { get { return 1; } }

        /// <summary>
        ///     When true, this champion will always drop power scrolls when they are not attached to a champ spawn.
        ///     If player scores are not used, this value will have no affect.
        /// </summary>
        public virtual bool AlwaysDropScrolls { get { return true; } }

        // Always use player scores.
        public override bool UseScores { get { return true; } }

        public BaseChampion(AIType aiType)
            : this(aiType, FightMode.Closest)
        {}

        public BaseChampion(AIType aiType, FightMode mode)
            : base(aiType, mode, 18, 1, 0.1, 0.2)
        {}

        public BaseChampion(
            AIType aiType, FightMode mode, int perception, int rangeFight, double activeSpeed, double passiveSpeed)
            : base(aiType, mode, perception, rangeFight, activeSpeed, passiveSpeed)
        {}

        public BaseChampion(Serial serial)
            : base(serial)
        {}

        public virtual bool IsEligible(Mobile mob)
        {
            return ChampionGlobals.IsEligible(this, mob);
        }

        public virtual void GiveRecipe(List<Mobile> eligibleMobs, List<double> eligibleMobScores, double totalScores)
        {
            double currentTestValue = 0.0;
            double roll = Utility.RandomDouble() * totalScores;

            for (int i = 0; i < eligibleMobScores.Count; i++)
            {
                currentTestValue += eligibleMobScores[i];

                if (roll > currentTestValue)
                {
                    continue;
                }

                var recipescroll = new RecipeScroll(Recipe.GetRandomRecipe());
                if (eligibleMobs[i] is PlayerMobile && eligibleMobs[i].Backpack != null)
                {
                    eligibleMobs[i].Backpack.DropItem(recipescroll);
                    eligibleMobs[i].SendMessage(54, "You have received a recipe scroll!");
                    return;
                }
            }
        }

        public virtual void GiveRelic(List<Mobile> eligibleMobs, List<double> eligibleMobScores, double totalScores)
        {
            double currentTestValue = 0.0;
            double roll = Utility.RandomDouble() * totalScores;

            for (int i = 0; i < eligibleMobScores.Count; i++)
            {
                currentTestValue += eligibleMobScores[i];

                if (roll > currentTestValue)
                {
                    continue;
                }

                var relic = BaseMetaRelic.GetRandomRelic();
                if (eligibleMobs[i] is PlayerMobile && eligibleMobs[i].Backpack != null)
                {
                    eligibleMobs[i].Backpack.DropItem(relic);
                    eligibleMobs[i].SendMessage(54, "You have received a relic!");
                    return;
                }
            }
        }

        public virtual void GiveSpecialItems(List<Mobile> eligibleMobs, List<double> eligibleMobScores,
            double totalScores)
        {}

        public virtual void GivePowerScrolls()
        {
            if (ChampSpawn == null && !AlwaysDropScrolls)
            {
                return;
            }

            var scores = new Dictionary<Mobile, double>(Scores);

            var toReceiveScroll = new List<Mobile>();


            // first find all eligible receivers
            var eligibleMobs = new List<Mobile>();
            var eligibleMobScores = new List<double>();

            var eligibleFactionMobs = new List<Mobile>();
            var eligibleFactionMobScores = new List<double>();


            double totalScores = 0.0;
            double totalFactionScores = 0.0;

            var sb = new StringBuilder();

            sb.Append("========== " + DateTime.Now + " Champion " + GetType() + " Report ==============\r\n");
            sb.Append(
                "Score\tEligible\tName\tAccount\tAddress\tStr\tInt\tDex\tTaming\tProvocation\tMagery\tArchery\tMace\tFencing\tSwords\tLumberjack\tWrestling\tAnatomy\r\n");

            foreach (KeyValuePair<Mobile, double> pair in scores)
            {
                Mobile mob = pair.Key;

                if (mob == null)
                {
                    continue;
                }

                bool eligible = IsEligible(mob);

                sb.Append(
                    pair.Value + "\t" + eligible + "\t" + mob + "\t" + mob.Account + "\t" + mob.Address + "\t" + mob.Str +
                    "\t" +
                    mob.Int + "\t" + mob.Dex + "\t" + mob.Skills.AnimalTaming.Value + "\t" +
                    mob.Skills.Provocation.Value + "\t" +
                    mob.Skills.Magery.Value + "\t" + mob.Skills.Archery.Value + "\t" + mob.Skills.Macing.Value + "\t" +
                    mob.Skills.Fencing.Value + "\t" + mob.Skills.Swords.Value + "\t" + mob.Skills.Lumberjacking.Value +
                    "\t" +
                    mob.Skills.Wrestling.Value + "\t" + mob.Skills.Anatomy.Value + "\r\n");

                if (!eligible)
                {
                    continue;
                }

                eligibleMobs.Add(mob);
                eligibleMobScores.Add(pair.Value);

                if (mob is PlayerMobile && ((PlayerMobile) mob).FactionName != null)
                {
                    eligibleFactionMobs.Add(mob);
                    eligibleFactionMobScores.Add(pair.Value);
                    totalFactionScores += pair.Value;
                }

                totalScores += pair.Value;
            }

            if (0.1 > Utility.RandomDouble())
            {
                GiveRecipe(eligibleMobs, eligibleMobScores, totalScores);
            }

            if (0.1 > Utility.RandomDouble())
            {
                GiveRelic(eligibleMobs, eligibleMobScores, totalScores);
            }

            GiveSpecialItems(eligibleMobs, eligibleMobScores, totalScores);

            //determine faction PS winner


            for (int powerScrollIndex = 0;
                powerScrollIndex < FactionPSToGive;
                powerScrollIndex++)
            {
                double currentTestValue = 0.0;
                double roll = Utility.RandomDouble() * totalFactionScores;

                for (int i = 0; i < eligibleFactionMobScores.Count; i++)
                {
                    currentTestValue += eligibleFactionMobScores[i];

                    if (roll > currentTestValue)
                    {
                        continue;
                    }

                    toReceiveScroll.Add(eligibleFactionMobs[i]);

                    totalScores -= eligibleFactionMobScores[i];

                    // remove them from eligible list now
                    eligibleFactionMobs.RemoveAt(i);
                    eligibleFactionMobScores.RemoveAt(i);
                    break;
                }
            }

            for (int powerScrollIndex = 0;
                powerScrollIndex < PSToGive;
                powerScrollIndex++)
            {
                double currentTestValue = 0.0;
                double roll = Utility.RandomDouble() * totalScores;

                for (int i = 0; i < eligibleMobScores.Count; i++)
                {
                    currentTestValue += eligibleMobScores[i];

                    if (roll > currentTestValue)
                    {
                        continue;
                    }

                    toReceiveScroll.Add(eligibleMobs[i]);

                    totalScores -= eligibleMobScores[i];

                    // remove them from eligible list now
                    eligibleMobs.RemoveAt(i);
                    eligibleMobScores.RemoveAt(i);
                    break;
                }
            }

            if (Portal == null && Invasion == null)
            {
                foreach (PlayerMobile mobile in scores.Keys.OfType<PlayerMobile>())
                {
                    PlayerMobile mobile1 = mobile;
                    Timer.DelayCall(
                        TimeSpan.FromMinutes(1),
                        () =>
                        {
                            var scrollgump = new PlayerScoresScrollGump(mobile1, onAccept: x =>
                            {
                                var scoreboard = new PlayerScoreResultsGump(mobile1, this, scores, toReceiveScroll).
                                    Send<PlayerScoreResultsGump>();
                            }).Send<PlayerScoresScrollGump>();
                        });
                }
            }

            foreach (Mobile luckyPlayer in toReceiveScroll)
            {
                PowerScroll ps = CreateRandomPowerScroll(Expansion);

                if (ps == null)
                {
                    continue;
                }

                GivePowerScrollTo(luckyPlayer, ps);
                sb.Append(luckyPlayer + " received powerScoll " + ps.Skill + " " + ps.Value + "\r\n");
            }

            LoggingCustom.Log("ChampionScores/" + IOUtility.GetSafeFileName(GetType().FullName) + ".log", sb.ToString());
        }

        public override bool OnBeforeDeath()
        {
            if (!NoKillAwards)
            {
                GivePowerScrolls();

                if (!NoGoodies && Map != null && Map != Map.Internal)
                {
                    BeginGoldExplosion(ExplodeFX.Random, Location, Map, GoldRange, GoldMin, GoldMax);
                }
            }

            return base.OnBeforeDeath();
        }

        public override void OnDeath(Container c)
        {
            if (SkullType != ChampionSkullType.Special && Map == Map.Felucca)
            {
                PlayerMobile toGive =
                    GetLootingRights(DamageEntries, HitsMax)
                        .Where(ds => ds.m_HasRight && ds.m_Mobile is PlayerMobile)
                        .Select(ds => (PlayerMobile) ds.m_Mobile)
                        .GetRandom();

                if (toGive == null || !toGive.PlaceInBackpack(new ChampionSkull(SkullType)))
                {
                    c.DropItem(new ChampionSkull(SkullType));
                }
            }

            PlayerScores.RemovePlayerScores(ChampSpawn != null ? (IEntity) ChampSpawn : this);
            base.OnDeath(c);
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
        }
    }
}