#region References

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Commands;
using Server.Engines.CannedEvil;
using Server.Engines.CustomTitles;
using Server.Items;
using VitaNex.FX;
using VitaNex.IO;
using VitaNex.Network;
using VitaNex.SuperGumps.UI;

#endregion

namespace Server.Mobiles
{
    [CorpseName("a Sphinx corpse")]
    public class Sphinx : BaseChampion
    {
        public override ChampionSkullType SkullType { get { return ChampionSkullType.Special; } }

        private DateTime _NextCast = DateTime.UtcNow;
        public TimeSpan CastInterval { get { return TimeSpan.FromSeconds(Utility.RandomMinMax(5, 10)); } }

        private DateTime _NextCaveIn = DateTime.UtcNow;
        public TimeSpan CaveInInterval { get { return TimeSpan.FromSeconds(Utility.RandomMinMax(23, 25)); } }
        public int CaveInRange { get { return 3; } }

        private DateTime _NextSandMine = DateTime.UtcNow;
        public TimeSpan SandMineInterval { get { return TimeSpan.FromSeconds(Utility.RandomMinMax(5, 7)); } }

        [Constructable]
        public Sphinx()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Argelmach";
            SpecialTitle = "The Sphinx";
            TitleHue = 1196;

            Body = 788;
            BaseSoundID = 1149;
            Hue = 1196;

            SetStr(510, 700);
            SetDex(510, 750);
            SetInt(310, 400);

            SetHits(40000);

            SetDamage(25, 30);

            SetSkill(SkillName.EvalInt, 120.0);
            SetSkill(SkillName.Magery, 120.0);
            SetSkill(SkillName.Meditation, 120.0);
            SetSkill(SkillName.MagicResist, 150.0);
            SetSkill(SkillName.Tactics, 120.0);
            SetSkill(SkillName.Wrestling, 120.0);


            Fame = 0;
            Karma = -20000;

            VirtualArmor = 45;

            SpeechHue = 34;

            PackGem();
            PackGold(4700, 6950);
        }

        public override void OnDeath(Container c)
        {
            var scroll = new SkillScroll();
            scroll.Randomize();
            c.DropItem(scroll);

            scroll = new SkillScroll();
            scroll.Randomize();
            c.DropItem(scroll);

            scroll = new SkillScroll();
            scroll.Randomize();
            c.DropItem(scroll);

            scroll = new SkillScroll();
            scroll.Randomize();
            c.DropItem(scroll);

            var statue = new SphinxStatue();
            c.DropItem(statue);

            base.OnDeath(c);
        }


        public override bool Unprovokable { get { return true; } }
        public override bool AlwaysMurderer { get { return true; } }
        public override bool Uncalmable { get { return true; } }
        public override Poison PoisonImmune { get { return Poison.Lethal; } }
        public override int TreasureMapLevel { get { return 5; } }

        public Sphinx(Serial serial)
            : base(serial)
        {}

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            if (from != null && !willKill && amount > 5 && from.Player && 2 > Utility.Random(100))
            {
                var toSay = new[]
                {
                    "Who dares to challenge the Sphinx?",
                    "You shall never defeat me!",
                    "{0}!!  You will have to do better than that!",
                    "{0}!!  Prepare to meet your doom!",
                    "{0}!!  My minions will crush you!",
                    "{0}!!  Come forth my minions, defend me!",
                    "{0}!!  You will pay for that!"
                };

                Say(true, String.Format(toSay[Utility.Random(toSay.Length)], from.Name));

                Quicksand(from);
            }

            base.OnDamage(amount, from, willKill);
        }

        public override void OnThink()
        {
            base.OnThink();

            DateTime now = DateTime.UtcNow;

            if (!this.InCombat())
            {
                return;
            }

            if (now > _NextCast)
            {
                _NextCast = now + CastInterval;

                if (now > _NextCaveIn)
                {
                    Say("*Violently stomps the ground*");

                    foreach (
                        Mobile mobile in
                            AcquireTargets(Location, 20)
                                .Where(x => x is BaseCreature && x.IsControlled() || x is PlayerMobile))
                    {
                        mobile.Damage(10, this);
                    }

                    _NextCaveIn = now + CaveInInterval;

                    for (int i = 0; i < 6; i++)
                    {
                        Timer.DelayCall(TimeSpan.FromSeconds(3.0 + i), CaveIn_Callback);
                    }
                }

                if (now > _NextSandMine)
                {
                    _NextSandMine = now + SandMineInterval;

                    Timer.DelayCall(TimeSpan.FromSeconds(0.5), SandMine_Callback);
                }
            }
        }

        public void Quicksand(Mobile target)
        {
            BaseExplodeEffect e = ExplodeFX.Earth.CreateInstance(
                target, Map, 1, 1, TimeSpan.FromMilliseconds(1000 - ((10) * 100)), null, () =>
                {
                    Mobile spawn;

                    switch (Utility.Random(2))
                    {
                        default:
                        case 0:
                            spawn = new Sandworm();
                            break;
                        case 1:
                            spawn = new WindTitan();
                            break;
                    }

                    spawn.MoveToWorld(target.Location, target.Map);
                });
            e.Send();
        }

        public void SandMine_Callback()
        {
            List<Mobile> rangelist = AcquireTargets(Location, 15);
            int index = Utility.Random(rangelist.Count);
            if (index + 1 > rangelist.Count)
            {
                return;
            }

            var startloc = new Point3D(X, Y, Z);
            var point = new Point3D(rangelist[index].Location);

            var queue = new EffectQueue();
            queue.Deferred = false;

            queue.Add(
                new MovingEffectInfo(
                    startloc,
                    point,
                    Map,
                    4586,
                    0,
                    5,
                    EffectRender.Normal,
                    TimeSpan.FromSeconds(1),
                    () =>
                    {
                        var sandmine = new SandMine();
                        sandmine.MoveToWorld(point, Map);
                    }));
            queue.Process();
        }


        public void CaveIn_Callback()
        {
            List<Mobile> rangelist = AcquireTargets(Location, 20);
            int index = Utility.Random(rangelist.Count);
            if (index + 1 > rangelist.Count)
            {
                return;
            }

            var startloc = new Point3D(rangelist[index].X, rangelist[index].Y, 100);
            var point = new Point3D(rangelist[index].Location);

            new MovingEffectInfo(
                startloc,
                point,
                Map,
                4534, 0, 10, EffectRender.Normal, TimeSpan.FromMilliseconds(1)).MovingImpact(
                    e =>
                    {
                        BaseExplodeEffect eff = ExplodeFX.Earth.CreateInstance(
                            point, Map, CaveInRange, 0, TimeSpan.FromMilliseconds(1), null, () =>
                            {
                            });
                        eff.Send();

                        foreach (Mobile player in AcquireAllTargets(point, CaveInRange))
                        {
                            if (player is BaseCreature && player.IsControlled())
                            {
                                player.Damage(250, this);
                            }
                            else
                            {
                                player.Damage(65, this);
                            }
                        }
                    });


            //Timer.DelayCall(TimeSpan.FromSeconds(3.0), CaveInImpact_Callback, point);
        }

        public List<Mobile> AcquireTargets(Point3D p, int range)
        {
            return
                p.GetMobilesInRange(Map, range)
                    .Where(
                        m =>
                            m != null && !m.Deleted && m.Alive && !m.Hidden &&
                            (m.Player || (m is BaseCreature && ((BaseCreature) m).GetMaster() is PlayerMobile && !m.IsDeadBondedPet)))
                    .ToList();
        }

        public List<Mobile> AcquireAllTargets(Point3D p, int range)
        {
            return
                p.GetMobilesInRange(Map, range)
                    .Where(
                        m =>
                            m != null && !m.Deleted && m.Alive &&
                            (m.Player || (m is BaseCreature && ((BaseCreature)m).GetMaster() is PlayerMobile && !m.IsDeadBondedPet)))
                    .ToList();
        }

        public override void GivePowerScrolls()
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

            List<KeyValuePair<Mobile, double>> orderedscores = scores.ToList();

            orderedscores.Sort((firstPair, nextPair) => firstPair.Value.CompareTo(nextPair.Value));
            orderedscores.Reverse();

            double totalScores = 0.0;
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

                totalScores += pair.Value;
            }

            for (int powerScrollIndex = 0;
                powerScrollIndex < ChampionGlobals.CSOptions.PowerScrollsToGive;
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

            foreach (PlayerMobile pm in orderedscores.Select(kvp => kvp.Key as PlayerMobile))
            {
                TitleHue titlehue;

                CustomTitles.TryGetHue(1196, out titlehue);

                TitleProfile p = CustomTitles.EnsureProfile(pm);

                if (pm != null && titlehue != null && p != null && !p.Contains(titlehue))
                {
                    p.Add(titlehue);
                    pm.SendMessage(titlehue.Hue, "You have been granted the title hue: " + titlehue.Hue);
                    sb.Append(pm + " received titlehue " + titlehue.Hue + "\r\n");
                    break;
                }
            }

            LoggingCustom.Log("ChampionScores/" + IOUtility.GetSafeFileName(GetType().FullName) + ".log", sb.ToString());
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}