// **********
// RunUO Shard - HarrowerTentacles.cs
// **********

#region References

using System;
using System.Collections.Generic;
using Server.Items;

#endregion

namespace Server.Mobiles
{
    [CorpseName("a tentacles corpse")]
    public class HarrowerTentaclesPortal : BaseCreature
    {
        private DrainTimer m_Timer;

        public override string DefaultName
        {
            get { return "tentacles of the harrower"; }
        }

        public HarrowerTentaclesPortal() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {

            Body = 129;

            SetStr(901, 1000);
            SetDex(126, 140);
            SetInt(1001, 1200);

            SetHits(541, 600);

            SetDamage(13, 20);


            SetSkill(SkillName.Meditation, 100.0);
            SetSkill(SkillName.MagicResist, 120.1, 140.0);
            SetSkill(SkillName.Swords, 90.1, 100.0);
            SetSkill(SkillName.Tactics, 90.1, 100.0);
            SetSkill(SkillName.Wrestling, 90.1, 100.0);

            Fame = 15000;
            Karma = -15000;

            VirtualArmor = 60;

            m_Timer = new DrainTimer(this);
            m_Timer.Start();

            PackReg(50);
            PackNecroReg(15, 75);
        }

        public override void CheckReflect(Mobile caster, ref bool reflect)
        {
            reflect = true;
        }

        public override int GetIdleSound()
        {
            return 0x101;
        }

        public override int GetAngerSound()
        {
            return 0x5E;
        }

        public override int GetDeathSound()
        {
            return 0x1C2;
        }

        public override int GetAttackSound()
        {
            return -1; // unknown
        }

        public override int GetHurtSound()
        {
            return 0x289;
        }

        public override void GenerateLoot()
        {
                AddLoot(LootPack.FilthyRich, 2);
                AddLoot(LootPack.MedScrolls, 3);
                AddLoot(LootPack.HighScrolls, 2);
        }

        public override bool OnBeforeDeath()
        {
            AddLoot(LootPack.FilthyRich, 3);
            AddLoot(LootPack.Rich, 3);
            AddLoot(LootPack.MedScrolls, 3);
            AddLoot(LootPack.HighScrolls, 2);
            PackItem(new Gold(1400, 1800));

            if (0.05 >= Utility.RandomDouble())
            {
                var scroll = new SkillScroll();
                scroll.Randomize();
                PackItem(scroll);
            }
            if (0.05 >= Utility.RandomDouble())
                PackItem(new BoneContainer());
            return base.OnBeforeDeath();
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            c.DropItem(new Platinum { Amount = 30 });
            c.DropItem(new GargoyleRune());
            if (Utility.RandomDouble() < 0.5)
            {
                c.DropItem(new GargoyleRune());
            }
            if (Utility.RandomDouble() < 0.1)
            {
                c.DropItem(new GargoyleRune());
            }
        }

        public override bool AutoDispel
        {
            get { return true; }
        }

        public override bool Unprovokable
        {
            get { return true; }
        }

        public override Poison PoisonImmune
        {
            get { return Poison.Lethal; }
        }

        public override bool DisallowAllMoves
        {
            get { return true; }
        }

        public HarrowerTentaclesPortal(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                {
                    m_Timer = new DrainTimer(this);
                    m_Timer.Start();

                    break;
                }
            }
        }

        public override void OnAfterDelete()
        {
            if (m_Timer != null)
                m_Timer.Stop();

            m_Timer = null;

            base.OnAfterDelete();
        }

        private class DrainTimer : Timer
        {
            private HarrowerTentaclesPortal m_Owner;

            public DrainTimer(HarrowerTentaclesPortal owner)
                : base(TimeSpan.FromSeconds(5.0), TimeSpan.FromSeconds(5.0))
            {
                m_Owner = owner;
                Priority = TimerPriority.TwoFiftyMS;
            }

            private List<Mobile> m_ToDrain = new List<Mobile>();

            protected override void OnTick()
            {
                if (m_Owner.Deleted)
                {
                    Stop();
                    return;
                }

                foreach (Mobile m in m_Owner.GetMobilesInRange(9))
                {
                    if (m == m_Owner || !m_Owner.CanBeHarmful(m) ||
                        m.AccessLevel != AccessLevel.Player)
                        continue;

                    if (m is BaseCreature)
                    {
                        var bc = m as BaseCreature;

                        if (bc.Controlled || bc.Summoned)
                            m_ToDrain.Add(m);
                    }
                    else if (m.Player)
                        m_ToDrain.Add(m);
                }

                foreach (Mobile m in m_ToDrain)
                {
                    m_Owner.DoHarmful(m);

                    m.FixedParticles(0x374A, 10, 15, 5013, 0x455, 0, EffectLayer.Waist);
                    m.PlaySound(0x1F1);

                    int drain = Utility.RandomMinMax(14, 30);

                    m_Owner.Hits += drain;

                    m.Damage(drain, m_Owner);
                }

                m_ToDrain.Clear();
            }
        }
    }
}