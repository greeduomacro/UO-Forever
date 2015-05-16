#region References

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Server.Items;
using Server.Spells;

#endregion

namespace Server.Mobiles
{
    public class Peddrenth : BaseCreature
    {
        public MonsterStatuetteType[] StatueTypes { get { return new MonsterStatuetteType[] {}; } }

        private bool m_TrueForm;
        private Item m_GateItem;
        private List<DragonSpirit> m_Spirits;
        private Timer m_Timer;

        private static ArrayList m_Instances = new ArrayList();

        public static ArrayList Instances { get { return m_Instances; } }

        public static bool CanSpawn { get { return (m_Instances.Count == 0); } }

        [Constructable]
        public Peddrenth()
            : base(AIType.AI_Mage, FightMode.Closest, 18, 1, 0.2, 0.4)
        {
            m_Instances.Add(this);

            Name = "Peddrenth Champion of Dragons";
            BodyValue = 826;
            Hue = 2498;

            SetStr(700);
            SetDex(1250, 1350);
            SetInt(1000, 1200);
            SetHits(85000, 95000);
            SetDamage(32, 52);

            SetSkill(SkillName.Wrestling, 110.9, 115.5);
            SetSkill(SkillName.Tactics, 196.9, 202.2);
            SetSkill(SkillName.MagicResist, 131.4, 140.8);
            SetSkill(SkillName.Magery, 156.2, 161.4);
            SetSkill(SkillName.EvalInt, 110.0);
            SetSkill(SkillName.Meditation, 420.0);

            m_Spirits = new List<DragonSpirit>();

            m_Timer = new TeleportTimer(this);
            m_Timer.Start();
        }

        public override int GetIdleSound()
        {
            return 0x667;
        }

        public override int GetAngerSound()
        {
            return 0x63E;
        }

        public override int GetHurtSound()
        {
            return 0x641;
        }

        public override int GetDeathSound()
        {
            return 0x63F;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.SuperBoss, 2);
            AddLoot(LootPack.SuperBoss, 2);
            AddLoot(LootPack.SuperBoss, 2);
            AddLoot(LootPack.Meager);
        }

        public override bool AutoDispel { get { return true; } }
        public override bool Unprovokable { get { return true; } }
        public override Poison PoisonImmune { get { return Poison.Lethal; } }

        private static readonly double[] m_Offsets =
        {
            Math.Cos(000.0 / 180.0 * Math.PI), Math.Sin(000.0 / 180.0 * Math.PI),
            Math.Cos(040.0 / 180.0 * Math.PI), Math.Sin(040.0 / 180.0 * Math.PI),
            Math.Cos(080.0 / 180.0 * Math.PI), Math.Sin(080.0 / 180.0 * Math.PI),
            Math.Cos(120.0 / 180.0 * Math.PI), Math.Sin(120.0 / 180.0 * Math.PI),
            Math.Cos(160.0 / 180.0 * Math.PI), Math.Sin(160.0 / 180.0 * Math.PI),
            Math.Cos(200.0 / 180.0 * Math.PI), Math.Sin(200.0 / 180.0 * Math.PI),
            Math.Cos(240.0 / 180.0 * Math.PI), Math.Sin(240.0 / 180.0 * Math.PI),
            Math.Cos(280.0 / 180.0 * Math.PI), Math.Sin(280.0 / 180.0 * Math.PI),
            Math.Cos(320.0 / 180.0 * Math.PI), Math.Sin(320.0 / 180.0 * Math.PI)
        };

        public void Morph()
        {
            if (m_TrueForm)
            {
                return;
            }

            m_TrueForm = true;

            Name = "the true Peddrenth";
            BodyValue = 198;
            Hue = 2498;

            Hits = HitsMax;
            Stam = StamMax;
            Mana = ManaMax;

            ProcessDelta();

            Say(1049499); // Behold my ultimate form!

            Map map = Map;

            if (map != null)
            {
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

                    var spawn = new DragonSpirit(this) {Team = Team};

                    spawn.MoveToWorld(new Point3D(x, y, z), map);

                    m_Spirits.Add(spawn);
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public override int HitsMax { get { return m_TrueForm ? 65000 : 30000; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public override int ManaMax { get { return 5000; } }

        public Peddrenth(Serial serial)
            : base(serial)
        {
            m_Instances.Add(this);
        }

        public override void OnAfterDelete()
        {
            m_Instances.Remove(this);

            base.OnAfterDelete();
        }

        public override bool DisallowAllMoves { get { return m_TrueForm; } }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version

            writer.Write(m_TrueForm);
            writer.Write(m_GateItem);
            writer.WriteMobileList(m_Spirits);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                {
                    m_TrueForm = reader.ReadBool();
                    m_GateItem = reader.ReadItem();
                    m_Spirits = reader.ReadStrongMobileList<DragonSpirit>();

                    m_Timer = new TeleportTimer(this);
                    m_Timer.Start();

                    break;
                }
            }
        }

        public override bool OnBeforeDeath()
        {
            if (m_TrueForm)
            {
                List<DamageStore> rights = GetLootingRights(DamageEntries, HitsMax);

                if (!NoKillAwards)
                {
                    Map map = Map;

                    if (map != null)
                    {
                        for (int x = -16; x <= 16; ++x)
                        {
                            for (int y = -16; y <= 16; ++y)
                            {
                                double dist = Math.Sqrt(x * x + y * y);

                                if (dist <= 16)
                                {
                                    new GoodiesTimer(map, X + x, Y + y).Start();
                                }
                            }
                        }
                    }

                    m_DamageEntries = new Dictionary<Mobile, int>();

                    foreach (DragonSpirit m in m_Spirits)
                    {
                        if (!m.Deleted)
                        {
                            m.Kill();
                        }

                        RegisterDamageTo(m);
                    }

                    m_Spirits.Clear();

                    RegisterDamageTo(this);

                    if (m_GateItem != null)
                    {
                        m_GateItem.Delete();
                    }
                }


                return base.OnBeforeDeath();
            }
            Morph();
            return false;
        }

        private Dictionary<Mobile, int> m_DamageEntries;

        public virtual void RegisterDamageTo(Mobile m)
        {
            if (m == null)
            {
                return;
            }

            foreach (DamageEntry de in m.DamageEntries)
            {
                Mobile damager = de.Damager;

                Mobile master = damager.GetDamageMaster(m);

                if (master != null)
                {
                    damager = master;
                }

                RegisterDamage(damager, de.DamageGiven);
            }
        }

        public void RegisterDamage(Mobile from, int amount)
        {
            if (from == null || !from.Player)
            {
                return;
            }

            if (m_DamageEntries.ContainsKey(from))
            {
                m_DamageEntries[from] += amount;
            }
            else
            {
                m_DamageEntries.Add(from, amount);
            }

            from.SendMessage(String.Format("Total Damage: {0}", m_DamageEntries[from]));
        }

        private class TeleportTimer : Timer
        {
            private Mobile m_Owner;

            private static int[] m_Offsets =
            {
                -1, -1,
                -1, 0,
                -1, 1,
                0, -1,
                0, 1,
                1, -1,
                1, 0,
                1, 1
            };

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
                    m_Owner.GetMobilesInRange(16)
                        .Cast<Mobile>()
                        .FirstOrDefault(m => m != m_Owner && m.Player && m_Owner.CanBeHarmful(m) && m_Owner.CanSee(m));

                if (toTeleport != null)
                {
                    int offset = Utility.Random(8) * 2;

                    Point3D to = m_Owner.Location;

                    for (int i = 0; i < m_Offsets.Length; i += 2)
                    {
                        int x = m_Owner.X + m_Offsets[(offset + i) % m_Offsets.Length];
                        int y = m_Owner.Y + m_Offsets[(offset + i + 1) % m_Offsets.Length];

                        if (map.CanSpawnMobile(x, y, m_Owner.Z))
                        {
                            to = new Point3D(x, y, m_Owner.Z);
                            break;
                        }
                        int z = map.GetAverageZ(x, y);

                        if (map.CanSpawnMobile(x, y, z))
                        {
                            to = new Point3D(x, y, z);
                            break;
                        }
                    }

                    Mobile m = toTeleport;

                    Point3D from = m.Location;

                    m.Location = to;

                    SpellHelper.Turn(m_Owner, toTeleport);
                    SpellHelper.Turn(toTeleport, m_Owner);

                    m.ProcessDelta();

                    Effects.SendLocationParticles(EffectItem.Create(from, m.Map, EffectItem.DefaultDuration), 0x3728, 10,
                        10, 2023);
                    Effects.SendLocationParticles(EffectItem.Create(to, m.Map, EffectItem.DefaultDuration), 0x3728, 10,
                        10, 5023);

                    m.PlaySound(0x1FE);

                    m_Owner.Combatant = toTeleport;
                }
            }
        }

        private class GoodiesTimer : Timer
        {
            private Map m_Map;
            private int m_X, m_Y;

            public GoodiesTimer(Map map, int x, int y)
                : base(TimeSpan.FromSeconds(Utility.RandomDouble() * 10.0))
            {
                Priority = TimerPriority.TwoFiftyMS;

                m_Map = map;
                m_X = x;
                m_Y = y;
            }

            protected override void OnTick()
            {
                int z = m_Map.GetAverageZ(m_X, m_Y);
                bool canFit = m_Map.CanFit(m_X, m_Y, z, 6, false, false);

                for (int i = -3; !canFit && i <= 3; ++i)
                {
                    canFit = m_Map.CanFit(m_X, m_Y, z + i, 6, false, false);

                    if (canFit)
                    {
                        z += i;
                    }
                }

                if (!canFit)
                {
                    return;
                }

                var g = new Gold(750, 1250);

                g.MoveToWorld(new Point3D(m_X, m_Y, z), m_Map);

                if (0.5 >= Utility.RandomDouble())
                {
                    switch (Utility.Random(3))
                    {
                        case 0: // Fire column
                        {
                            Effects.SendLocationParticles(
                                EffectItem.Create(g.Location, g.Map, EffectItem.DefaultDuration), 0x3709, 10, 30, 5052);
                            Effects.PlaySound(g, g.Map, 0x665);

                            break;
                        }
                        case 1: // Explosion
                        {
                            Effects.SendLocationParticles(
                                EffectItem.Create(g.Location, g.Map, EffectItem.DefaultDuration), 0x36BD, 20, 10, 5044);
                            Effects.PlaySound(g, g.Map, 0x656);

                            break;
                        }
                        case 2: // Ball of fire
                        {
                            Effects.SendLocationParticles(
                                EffectItem.Create(g.Location, g.Map, EffectItem.DefaultDuration), 0x36FE, 10, 10, 5052);

                            break;
                        }
                    }
                }
            }
        }
    }
}