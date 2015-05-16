#region References

using System;
using System.Collections.Generic;
using Server.Engines.CannedEvil;
using Server.Engines.CustomTitles;
using Server.Items;

#endregion

namespace Server.Mobiles
{
    public class Neira : BaseChampion
    {
        public override ChampionSkullType SkullType { get { return ChampionSkullType.Death; } }

        [Constructable]
        public Neira()
            : base(AIType.AI_Mage)
        {
            Name = "Neira";
            SpecialTitle = "The Necromancer";
            TitleHue = 1174;

            Body = 401;
            Hue = 0x83EC;

            Alignment = Alignment.Undead;

            SetStr(305, 635);
            SetDex(100, 175);
            SetInt(705, 950);

            SetHits(9900, 12750);
            SetStam(80, 100);

            SetDamage(33, 45);

            SetSkill(SkillName.EvalInt, 120.0);
            SetSkill(SkillName.Magery, 120.0);
            SetSkill(SkillName.Meditation, 120.0);
            SetSkill(SkillName.MagicResist, 150.0);
            SetSkill(SkillName.Tactics, 97.6, 100.0);
            SetSkill(SkillName.Wrestling, 97.6, 100.0);

            Fame = 22500;
            Karma = -22500;

            VirtualArmor = 30;
            Female = true;

            Item shroud = new HoodedShroudOfShadows();

            shroud.Movable = false;

            AddItem(shroud);

            var weapon = new Scimitar
            {
                Skill = SkillName.Wrestling,
                Hue = 38,
                Movable = false,
                Name = "Defiled Cutlass"
            };

            AddItem(weapon);

            //new SkeletalMount().Rider = this;
            AddItem(new VirtualMountItem(this));
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.UltraRich, 3);
            AddLoot(LootPack.Meager);
        }

        public override bool OnBeforeDeath()
        {
            IMount mount = Mount;

            if (mount != null)
            {
                mount.Rider = null;
            }

            if (mount is Mobile)
            {
                ((Mobile) mount).Delete();
            }

            return base.OnBeforeDeath();
        }

        private bool m_SpeedBoost;

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            CheckSpeedBoost();
            base.OnDamage(amount, from, willKill);
        }

        private const double SpeedBoostScalar = 1.2;

        private void CheckSpeedBoost()
        {
            if (Hits < (HitsMax / 4))
            {
                if (m_SpeedBoost)
                {
                    return;
                }
                ActiveSpeed /= SpeedBoostScalar;
                PassiveSpeed /= SpeedBoostScalar;
                m_SpeedBoost = true;
            }
            else if (m_SpeedBoost)
            {
                ActiveSpeed *= SpeedBoostScalar;
                PassiveSpeed *= SpeedBoostScalar;
                m_SpeedBoost = false;
            }
        }

        public override void GiveSpecialItems(List<Mobile> eligibleMobs, List<double> eligibleMobScores,
            double totalScores)
        {
            double currentTestValue = 0.0;
            double roll = Utility.RandomDouble() * totalScores;
            if (0.2 > Utility.RandomDouble())
            {
                for (int i = 0; i < eligibleMobScores.Count; i++)
                {
                    currentTestValue += eligibleMobScores[i];

                    if (roll > currentTestValue)
                    {
                        continue;
                    }

                    var rand = new Random();
                    int value = rand.Next(0, 8);
                    Item item = null;
                    switch (value)
                    {
                        case 0:
                        {
                            item = new MummifiedCorpseArtifact();
                        }
                            break;
                        case 1:
                        {
                            item = new WallBloodArtifact();
                        }
                            break;
                        case 2:
                        {
                            item = new MummifiedCorpseArtifact();
                        }
                            break;
                        case 3:
                        {
                            item = new WallBloodArtifact();
                        }
                            break;
                        case 4:
                        {
                            item = new MummifiedCorpseArtifact();
                        }
                            break;
                        case 5:
                        {
                            item = new WallBloodArtifact();
                        }
                            break;
                        case 6:
                        {
                            item = new MummifiedCorpseArtifact();
                        }
                            break;
                        case 7:
                            item = new NeirasDefiledcutlassArtifact();
                            break;
                    }
                    if (eligibleMobs[i] is PlayerMobile && eligibleMobs[i].Backpack != null && item != null)
                    {
                        eligibleMobs[i].Backpack.DropItem(item);
                        eligibleMobs[i].SendMessage(54, "You have received a " + item.Name + ".");
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
                        eligibleMobs[i].Backpack.DropItem(new TitleScroll("Lord of the Damned"));
                        eligibleMobs[i].SendMessage(54, "You have received a title scroll.");
                        return;
                    }
                }
            }
        }

        private sealed class VirtualMount : IMount
        {
            private readonly VirtualMountItem m_Item;

            public Mobile Rider { get { return m_Item.Rider; } set { } }

            public VirtualMount(VirtualMountItem item)
            {
                m_Item = item;
            }

            public void OnRiderDamaged(int amount, Mobile from, bool willKill)
            {}
        }

        private class VirtualMountItem : Item, IMountItem
        {
            private Mobile m_Rider;
            private VirtualMount m_Mount;

            public Mobile Rider { get { return m_Rider; } }

            public VirtualMountItem(Mobile mob)
                : base(0x3EBB)
            {
                Layer = Layer.Mount;

                Movable = false;

                m_Rider = mob;
                m_Mount = new VirtualMount(this);
            }

            public IMount Mount
            {
                get { return m_Mount; }
                set
                {
                    m_Mount = value as VirtualMount;
                    if (m_Mount == null)
                    {
                        Delete();
                    }
                }
            }

            public VirtualMountItem(Serial serial)
                : base(serial)
            {
                m_Mount = new VirtualMount(this);
            }

            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);

                writer.Write(0); // version

                writer.Write(m_Rider);
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);

                reader.ReadInt();

                m_Rider = reader.ReadMobile();

                if (m_Rider == null)
                {
                    Delete();
                }
            }
        }

        public override bool AlwaysMurderer { get { return true; } }
        public override bool BardImmune { get { return !EraSE; } }
        public override bool Unprovokable { get { return EraSE; } }
        public override bool Uncalmable { get { return EraSE; } }
        public override Poison PoisonImmune { get { return Poison.Deadly; } }
        public override bool ShowFameTitle { get { return false; } }
        public override bool ClickTitle { get { return false; } }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (0.1 >= Utility.RandomDouble()) // 10% chance to drop or throw an unholy bone
            {
                AddUnholyBone(defender, 0.25);
            }

            CheckSpeedBoost();
        }

        public override void OnGotMeleeAttack(Mobile attacker)
        {
            base.OnGotMeleeAttack(attacker);

            if (0.1 >= Utility.RandomDouble()) // 10% chance to drop or throw an unholy bone
            {
                AddUnholyBone(attacker, 0.25);
            }
        }

        public override void AlterDamageScalarFrom(Mobile caster, ref double scalar)
        {
            base.AlterDamageScalarFrom(caster, ref scalar);

            if (0.1 >= Utility.RandomDouble()) // 10% chance to throw an unholy bone
            {
                AddUnholyBone(caster, 1.0);
            }
        }

        public void AddUnholyBone(Mobile target, double chanceToThrow)
        {
            if (Map == null)
            {
                return;
            }

            if (chanceToThrow >= Utility.RandomDouble())
            {
                Direction = GetDirectionTo(target);
                MovingEffect(target, 0xF7E, 10, 1, true, false, 0x496, 0);
                new DelayTimer(this, target).Start();
            }
            else
            {
                new UnholyBone().MoveToWorld(Location, Map);
            }
        }

        private class DelayTimer : Timer
        {
            private readonly Mobile m_Mobile;
            private readonly Mobile m_Target;

            public DelayTimer(Mobile m, Mobile target)
                : base(TimeSpan.FromSeconds(1.0))
            {
                m_Mobile = m;
                m_Target = target;
            }

            protected override void OnTick()
            {
                if (m_Mobile.CanBeHarmful(m_Target))
                {
                    m_Mobile.DoHarmful(m_Target);
                    m_Target.Damage(Utility.RandomMinMax(10, 20), m_Mobile);
                    new UnholyBone().MoveToWorld(m_Target.Location, m_Target.Map);
                }
            }
        }

        public Neira(Serial serial)
            : base(serial)
        {}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1); // version
            writer.Write(m_SpeedBoost);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                {
                    m_SpeedBoost = reader.ReadBool();
                    break;
                }
            }
        }
    }
}