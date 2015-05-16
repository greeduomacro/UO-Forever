#region References

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Commands;
using Server.Engines.CannedEvil;
using Server.Engines.Craft;
using Server.Engines.CustomTitles;
using Server.Items;
using Server.Spells;
using VitaNex.IO;
using VitaNex.SuperGumps.UI;

#endregion

namespace Server.Mobiles
{
    public class Harrower : BaseChampion
    {
        private class SpawnEntry
        {
            public readonly Point3D _Location;
            public readonly Point3D _Entrance;

            public SpawnEntry(Point3D loc, Point3D ent)
            {
                _Location = loc;
                _Entrance = ent;
            }
        }

        private static readonly SpawnEntry[] m_Entries =
        {
            new SpawnEntry(new Point3D(5242, 945, -40), new Point3D(1176, 2638, 0)), // Destard
            new SpawnEntry(new Point3D(5225, 798, 0), new Point3D(1176, 2638, 0)), // Destard
            new SpawnEntry(new Point3D(5556, 886, 30), new Point3D(1298, 1080, 0)), // Despise
            new SpawnEntry(new Point3D(5187, 615, 0), new Point3D(4111, 432, 5)), // Deceit
            new SpawnEntry(new Point3D(5319, 583, 0), new Point3D(4111, 432, 5)), // Deceit
            new SpawnEntry(new Point3D(5713, 1334, -1), new Point3D(2923, 3407, 8)), // Fire
            new SpawnEntry(new Point3D(5860, 1460, -2), new Point3D(2923, 3407, 8)), // Fire
            new SpawnEntry(new Point3D(5328, 1620, 0), new Point3D(5451, 3143, -60)), // Terathan Keep
            new SpawnEntry(new Point3D(5690, 538, 0), new Point3D(2042, 224, 14)), // Wrong
            new SpawnEntry(new Point3D(5609, 195, 0), new Point3D(514, 1561, 0)), // Shame
            new SpawnEntry(new Point3D(5475, 187, 0), new Point3D(514, 1561, 0)), // Shame
            new SpawnEntry(new Point3D(6085, 179, 0), new Point3D(4721, 3822, 0)), // Hythloth
            new SpawnEntry(new Point3D(6084, 66, 0), new Point3D(4721, 3822, 0)), // Hythloth
            new SpawnEntry(new Point3D(5499, 2003, 0), new Point3D(2499, 919, 0)), // Covetous
            new SpawnEntry(new Point3D(5579, 1858, 0), new Point3D(2499, 919, 0)) // Covetous
        };

        private static readonly double[] m_Offsets =
        {
            Math.Cos(000.0 / 180.0 * Math.PI), Math.Sin(000.0 / 180.0 * Math.PI),
            Math.Cos(040.0 / 180.0 * Math.PI), Math.Sin(040.0 / 180.0 * Math.PI), Math.Cos(080.0 / 180.0 * Math.PI),
            Math.Sin(080.0 / 180.0 * Math.PI), Math.Cos(120.0 / 180.0 * Math.PI), Math.Sin(120.0 / 180.0 * Math.PI),
            Math.Cos(160.0 / 180.0 * Math.PI), Math.Sin(160.0 / 180.0 * Math.PI), Math.Cos(200.0 / 180.0 * Math.PI),
            Math.Sin(200.0 / 180.0 * Math.PI), Math.Cos(240.0 / 180.0 * Math.PI), Math.Sin(240.0 / 180.0 * Math.PI),
            Math.Cos(280.0 / 180.0 * Math.PI), Math.Sin(280.0 / 180.0 * Math.PI), Math.Cos(320.0 / 180.0 * Math.PI),
            Math.Sin(320.0 / 180.0 * Math.PI)
        };

        public static List<Harrower> Instances { get; private set; }

        public static bool CanSpawn { get { return Instances.Count == 0; } }

        static Harrower()
        {
            Instances = new List<Harrower>();
        }

        public static Harrower Spawn(Point3D platLoc, Map platMap)
        {
            if (Instances.Count > 0)
            {
                return null;
            }

            SpawnEntry entry = m_Entries.GetRandom();

            var harrower = new Harrower();

            harrower.MoveToWorld(entry._Location, Map.Felucca);

            harrower._GateItem = new HarrowerGate(harrower, platLoc, platMap, entry._Entrance, Map.Felucca);

            return harrower;
        }

        private bool _TrueForm;
        private Item _GateItem;
        private Timer _Timer;
        private List<HarrowerTentacles> _Tentacles;

        public override int PSToGive { get { return 6; } }

        public override string DefaultName { get { return "harrower"; } }

        public override bool AutoDispel { get { return true; } }
        public override bool Unprovokable { get { return true; } }
        public override Poison PoisonImmune { get { return Poison.Lethal; } }

        public override bool AlwaysDropScrolls { get { return true; } }

        public override ChampionSkullType SkullType { get { return ChampionSkullType.Death; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public override int HitsMax { get { return _TrueForm ? 100000 : 60000; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public override int ManaMax { get { return 5000; } }

        [Constructable]
        public Harrower()
            : base(AIType.AI_Mage, FightMode.Closest, 18, 1, 0.2, 0.4)
        {
            Instances.Add(this);

            BodyValue = 146;

            SpecialTitle = "The Abyssal Horror";
            TitleHue = 1174;

            Alignment = Alignment.Demon;
            SetStr(900, 1000);
            SetDex(125, 135);
            SetInt(1000, 1200);

            Fame = 22500;
            Karma = -22500;

            VirtualArmor = 60;

            SetSkill(SkillName.Wrestling, 90.1, 100.0);
            SetSkill(SkillName.Tactics, 90.2, 110.0);
            SetSkill(SkillName.MagicResist, 120.2, 160.0);
            SetSkill(SkillName.Magery, 120.0);
            SetSkill(SkillName.EvalInt, 120.0);
            SetSkill(SkillName.Meditation, 120.0);

            _Tentacles = new List<HarrowerTentacles>();

            _Timer = new TeleportTimer(this);
            _Timer.Start();
        }

        public Harrower(Serial serial)
            : base(serial)
        {
            Instances.Add(this);
        }

        public void Morph()
        {
            if (_TrueForm)
            {
                return;
            }

            _TrueForm = true;

            Name = "the true harrower";
            BodyValue = 780;
            Hue = 0x497;
            CantWalk = true;

            Hits = HitsMax;
            Stam = StamMax;
            Mana = ManaMax;

            ProcessDelta();

            Say(1049499); // Behold my true form!

            Map map = Map;

            if (map == null)
            {
                return;
            }

            for (int i = 0; i < m_Offsets.Length; i += 2)
            {
                double rx = m_Offsets[i];
                double ry = m_Offsets[i + 1];

                int dist = 0;
                bool ok = false;
                int x = 0, y = 0, z = 0;

                while (!ok && dist < 10)
                {
                    int rdist = 10 + dist;

                    x = X + (int) (rx * rdist);
                    y = Y + (int) (ry * rdist);
                    z = map.GetAverageZ(x, y);

                    if (!(ok = map.CanFit(x, y, Z, 16, false, false)))
                    {
                        ok = map.CanFit(x, y, z, 16, false, false);
                    }

                    if (dist >= 0)
                    {
                        dist = -(dist + 1);
                    }
                    else
                    {
                        dist = -(dist - 1);
                    }
                }

                if (!ok)
                {
                    continue;
                }

                var spawn = new HarrowerTentacles(this)
                {
                    Team = Team
                };

                spawn.MoveToWorld(new Point3D(x, y, z), map);

                _Tentacles.Add(spawn);
            }
        }

        public override void OnAfterDelete()
        {
            Instances.Remove(this);

            base.OnAfterDelete();
        }

        public override bool OnBeforeDeath()
        {
            if (!_TrueForm)
            {
                Morph();
                return false;
            }

            return base.OnBeforeDeath();
        }

        public override void OnDeath(Container c)
        {
            GetLootingRights(DamageEntries, HitsMax)
                .Where(e => e.m_HasRight && e.m_Mobile is PlayerMobile)
                .Select(e => (PlayerMobile) e.m_Mobile)
                .ForEach(PlayerMobile.ChampionTitleInfo.AwardHarrowerTitle);

            _Tentacles.Where(t => !t.Deleted).ForEach(t => t.Kill());
            _Tentacles.Clear();

            if (!NoKillAwards && _GateItem != null)
            {
                _GateItem.Delete();
            }

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

            base.OnDeath(c);
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.SuperBoss, 2);
            AddLoot(LootPack.Meager);
        }

        public override void GiveSpecialItems(List<Mobile> eligibleMobs, List<double> eligibleMobScores, double totalScores)
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

                HarrowerSoul heart = new HarrowerSoul();
                if (eligibleMobs[i] is PlayerMobile && eligibleMobs[i].Backpack != null)
                {
                    eligibleMobs[i].Backpack.DropItem(heart);
                    eligibleMobs[i].SendMessage(54, "You have captured the soul of the Harrower!");
                    break;
                }
            }

            currentTestValue = 0.0;
            roll = Utility.RandomDouble() * totalScores;
            if (0.2 > Utility.RandomDouble())
            {
                for (int i = 0; i < eligibleMobScores.Count; i++)
                {
                    currentTestValue += eligibleMobScores[i];

                    if (roll > currentTestValue)
                    {
                        continue;
                    }

                    Random rand = new Random();
                    int value = rand.Next(0, 2);
                    Item item = null;
                    switch (value)
                    {
                        case 0:
                            {
                                item = new EvilIdotSkullArtifact();
                            }
                            break;
                        case 1:
                            {
                                item = new SkullPollArtifact();
                            }
                            break;
                    }
                    if (eligibleMobs[i] is PlayerMobile && eligibleMobs[i].Backpack != null && item != null)
                    {
                        eligibleMobs[i].Backpack.DropItem(item);
                        eligibleMobs[i].SendMessage(54, "You have received " + item.Name + ".");
                        break;
                    }
                }
            }

            if (0.2 > Utility.RandomDouble())
            {
                for (int i = 0; i < eligibleMobScores.Count; i++)
                {
                    currentTestValue += eligibleMobScores[i];

                    if (roll > currentTestValue)
                    {
                        continue;
                    }

                    if (eligibleMobs[i] is PlayerMobile && eligibleMobs[i].Backpack != null)
                    {
                        eligibleMobs[i].Backpack.DropItem(new TitleScroll("The Defiler"));
                        eligibleMobs[i].SendMessage(54, "You have received a title scroll.");
                        return;
                    }
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version

            writer.Write(_TrueForm);
            writer.Write(_GateItem);
            writer.WriteMobileList(_Tentacles);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                {
                    _TrueForm = reader.ReadBool();
                    _GateItem = reader.ReadItem();
                    _Tentacles = reader.ReadStrongMobileList<HarrowerTentacles>();

                    _Timer = new TeleportTimer(this);
                    _Timer.Start();
                }
                    break;
            }
        }

        private class TeleportTimer : Timer
        {
            private readonly Mobile m_Owner;

            private static readonly int[] _Offsets = {-1, -1, -1, 0, -1, 1, 0, -1, 0, 1, 1, -1, 1, 0, 1, 1};

            public TeleportTimer(Mobile owner)
                : base(TimeSpan.FromSeconds(5.0), TimeSpan.FromSeconds(5.0))
            {
                Priority = TimerPriority.TwoFiftyMS;

                m_Owner = owner;
            }

            protected override void OnTick()
            {
                if (m_Owner.Deleted)
                {
                    Stop();
                    return;
                }

                Map map = m_Owner.Map;

                if (map == null)
                {
                    return;
                }

                if (0.25 < Utility.RandomDouble())
                {
                    return;
                }

                Mobile toTeleport =
                    m_Owner.GetMobilesInRange(map, 16)
                        .Where(
                            mob =>
                                mob != m_Owner && mob.Player && mob.AccessLevel == AccessLevel.Player &&
                                m_Owner.CanBeHarmful(mob) &&
                                m_Owner.CanSee(mob))
                        .GetRandom();

                if (toTeleport == null)
                {
                    return;
                }

                int offset = Utility.Random(8) * 2;

                Point3D to = m_Owner.Location;

                for (int i = 0; i < _Offsets.Length; i += 2)
                {
                    int x = m_Owner.X + _Offsets[(offset + i) % _Offsets.Length];
                    int y = m_Owner.Y + _Offsets[(offset + i + 1) % _Offsets.Length];

                    if (map.CanSpawnMobile(x, y, m_Owner.Z))
                    {
                        to = new Point3D(x, y, m_Owner.Z);
                        break;
                    }

                    int z = map.GetAverageZ(x, y);

                    if (!map.CanSpawnMobile(x, y, z))
                    {
                        continue;
                    }

                    to = new Point3D(x, y, z);
                    break;
                }

                Mobile m = toTeleport;

                Point3D from = m.Location;

                m.Location = to;

                SpellHelper.Turn(m_Owner, toTeleport);
                SpellHelper.Turn(toTeleport, m_Owner);

                m.ProcessDelta();

                Effects.SendLocationParticles(EffectItem.Create(@from, m.Map, EffectItem.DefaultDuration), 0x3728, 10,
                    10, 2023);
                Effects.SendLocationParticles(EffectItem.Create(to, m.Map, EffectItem.DefaultDuration), 0x3728, 10, 10,
                    5023);

                m.PlaySound(0x1FE);

                m_Owner.Combatant = toTeleport;
            }
        }
    }
}