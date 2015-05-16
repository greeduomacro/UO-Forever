#region References

using System;
using System.Collections.Generic;
using System.Linq;
using Server.Items;
using Server.Items.VitaNex.Items;
using VitaNex.FX;

#endregion

namespace Server.Mobiles
{
    [CorpseName("a dirty corpse")]
    public class AsylumInmate : BaseCreature
    {
        private static readonly SkillName[] _Skills = ((SkillName) 0).GetValues<SkillName>();

        private DateTime _NextShadowStep = DateTime.UtcNow;
        private DateTime _NextCast = DateTime.UtcNow;

        public TimeSpan ShadowStepInterval { get { return TimeSpan.FromSeconds(Utility.RandomMinMax(35, 45)); } }

        private List<Timer> CurrentSpell { get; set; }
        public TimeSpan CastInterval { get { return TimeSpan.FromSeconds(Utility.RandomMinMax(10, 15)); } }

        public override bool CanRummageCorpses { get { return true; } }
        public override bool CanDestroyObstacles { get { return true; } }
        public override bool CanFlee { get { return true; } }

        public override bool AlwaysMurderer { get { return true; } }

        [Constructable]
        public AsylumInmate()
            : base(AIType.AI_Arcade, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Female = Utility.RandomBool();

            Name = Female ? NameList.RandomName("female") : NameList.RandomName("male");
            Body = Female ? 401 : 400;
            Hue = Utility.RandomSkinHue();
            SpecialTitle = "[Asylum Inmate]";
            TitleHue = 1161;

            Utility.AssignRandomHair(this);

            HairHue = Utility.RandomNondyedHue();

            SpeechHue = YellHue = Utility.RandomDyedHue();

            SetStr(96, 125);
            SetDex(81, 95);
            SetInt(61, 75);
            SetHits(180, 250);
            SetDamage(5, 20);

            SetSkill(SkillName.Fencing, 105.0, 115.5);
            SetSkill(SkillName.Macing, 105.0, 115.5);
            SetSkill(SkillName.MagicResist, 107.0, 110.5);
            SetSkill(SkillName.Swords, 105.0, 115.5);
            SetSkill(SkillName.Tactics, 105.0, 110.5);
            SetSkill(SkillName.Wrestling, 50.0, 77.5);

            VirtualArmor = 30;

            EquipItem(
                new Bandana
                {
                    Name = "tattered bandana",
                    Hue = Utility.RandomDyedHue(),
                    Identified = true,
                    Movable = false,
                    LootType = LootType.Blessed
                });

            EquipItem(
                new ShortPants
                {
                    Name = "tattered pants",
                    Hue = Utility.RandomDyedHue(),
                    Identified = true,
                    Movable = false,
                    LootType = LootType.Blessed
                });

            switch (Utility.Random(7))
            {
                case 0:
                    EquipItem(
                        new Cutlass
                        {
                            Identified = true,
                            Movable = false,
                            LootType = LootType.Blessed
                        });
                    break;
                case 1:
                    EquipItem(
                        new Longsword
                        {
                            Identified = true,
                            Movable = false,
                            LootType = LootType.Blessed
                        });
                    break;
                case 2:
                    EquipItem(
                        new Axe
                        {
                            Identified = true,
                            Movable = false,
                            LootType = LootType.Blessed
                        });
                    break;
                case 3:
                    EquipItem(
                        new Dagger
                        {
                            Identified = true,
                            Movable = false,
                            LootType = LootType.Blessed
                        });
                    break;
                case 4:
                    EquipItem(
                        new Cleaver
                        {
                            Identified = true,
                            Movable = false,
                            LootType = LootType.Blessed
                        });
                    break;
                case 5:
                    EquipItem(
                        new Club
                        {
                            Identified = true,
                            Movable = false,
                            LootType = LootType.Blessed
                        });
                    break;
                case 6:
                    EquipItem(
                        new ButcherKnife
                        {
                            Identified = true,
                            Movable = false,
                            LootType = LootType.Blessed
                        });
                    break;
            }
        }

        public AsylumInmate(Serial serial)
            : base(serial)
        {}

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.Average, 2);
            AddLoot(LootPack.MedScrolls, 2);
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

                if (CurrentSpell == null)
                {
                    CurrentSpell = new List<Timer>();
                }

                if (now > _NextShadowStep)
                {
                    _NextShadowStep = now + ShadowStepInterval;

                    CurrentSpell.Clear();

                    CurrentSpell.Add(Timer.DelayCall(TimeSpan.FromSeconds(1), Throw_Poop));
                }
            }
        }

        public void Throw_Poop()
        {
            Emote("*You see {0} throw his feces.*", Name);
            foreach (Mobile m in AcquireTargets(Location, 10).Take(1))
            {
                Animate();

                Mobile m1 = m;
                new MovingEffectInfo(this, m, Map, 3899, 0, 6)
                    .MovingImpact(
                        eff =>
                        {
                            m1.Damage(20);

                            m1.SendMessage(133, "The inmate's feces hit you right in the face!");
                        });
            }
        }

        public List<Mobile> AcquireTargets(Point3D p, int range)
        {
            return
                p.GetMobilesInRange(Map, range)
                    .Where(
                        m =>
                            m != null && !m.Deleted && m.AccessLevel == AccessLevel.Player && m != this && m.Alive &&
                            CanBeHarmful(m, false, true) && (m.Party == null || m.Party != Party) &&
                            (m.Player))
                    .ToList();
        }

        public List<Mobile> AcquireAllTargets(Point3D p, int range)
        {
            return
                p.GetMobilesInRange(Map, range)
                    .Where(
                        m =>
                            m != null && !m.Deleted && m.AccessLevel == AccessLevel.Player && m != this && m.Alive &&
                            CanBeHarmful(m, false, true) && (m.Party == null || m.Party != Party) &&
                            (m.Player || (m is BaseCreature && ((BaseCreature) m).GetMaster() is PlayerMobile)))
                    .ToList();
        }

        public void Animate()
        {
            if (Body.IsHuman)
            {
                Animate(9, 10, 1, true, true, 1);
            }
            else if (Body.IsMonster)
            {
                Animate(12, 7, 1, true, true, 1);
            }
        }

        public void AnimateSpell()
        {
            if (Body.IsHuman)
            {
                Animate(263, 7, 1, true, true, 1);
            }
            else if (Body.IsMonster)
            {
                Animate(12, 7, 1, true, true, 1);
            }
        }

        public override void OnDeath(Container c)
        {
            if (Utility.RandomDouble() <= 0.0005)
            {
                c.DropItem(new HumanFeces());
            }

            if (Utility.RandomDouble() <= 0.0005)
            {
                if (Utility.RandomBool())
                {
                    c.DropItem(new ButcherKnife { Hue = 1157, Name = "bloodied knife", Identified = true });
                }
                else
                {
                    c.DropItem(new Cleaver { Hue = 1157, Name = "bloodied cleaver", Identified = true });
                }
            }
            base.OnDeath(c);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int) 0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}