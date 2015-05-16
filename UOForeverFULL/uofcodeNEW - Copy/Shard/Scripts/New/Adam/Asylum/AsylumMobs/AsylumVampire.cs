#region References

using System;
using System.Collections.Generic;
using System.Linq;
using Server.Items;
using VitaNex.FX;
using VitaNex.Network;

#endregion

namespace Server.Mobiles
{
    [CorpseName("a desiccated corpse")]
    public class AsylumVampire : BaseCreature
    {
        private static readonly SkillName[] _Skills = ((SkillName) 0).GetValues<SkillName>();

        private DateTime _NextShadowStep = DateTime.UtcNow;
        private DateTime _NextCast = DateTime.UtcNow;

        public TimeSpan ShadowStepInterval { get { return TimeSpan.FromSeconds(Utility.RandomMinMax(35, 45)); } }

        private List<Timer> CurrentSpell { get; set; }
        public TimeSpan CastInterval { get { return TimeSpan.FromSeconds(Utility.RandomMinMax(10, 15)); } }

        public override bool CanRummageCorpses { get { return true; } }
        public override bool AlwaysMurderer { get { return true; } }
        public override bool CanDestroyObstacles { get { return true; } }
        public override bool CanFlee { get { return true; } }
        public override bool Unprovokable { get { return true; } }
        public override Poison PoisonImmune { get { return Poison.Lethal; } }

        public override int TreasureMapLevel { get { return 4; } }

        public override bool BardImmune
        {
            get { return true; }
        }

        [Constructable]
        public AsylumVampire()
            : base(Utility.RandomBool() ? AIType.AI_Arcade : AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = NameList.RandomName("darknight creeper");
            Female = Utility.RandomBool();
            Body = Female ? 401 : 400;
            Hue = 1162;
            SpecialTitle = "[Covenant of Blood]";
            TitleHue = 133;

            Alignment = Alignment.Undead;

            BaseSoundID = 362;

            SetStr(796, 825);
            SetDex(200);
            SetInt(400);

            SetHits(700);

            SetDamage(4, 13);

            foreach (SkillName skill in _Skills)
            {
                SetSkill(skill, 100.0, 100.0);
            }

            SpeechHue = YellHue = 34;

            VirtualArmor = 50;

            EquipItem(
            new MonkRobe
            {
                Name = "shroud",
                Hue = 1157,
                Identified = true,
                Movable = false,
                LootType = LootType.Blessed
            });

        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 2);
            AddLoot(LootPack.Gems, 8);
        }

        public override void OnDeath(Container c)
        {
            if (Utility.RandomDouble() <= 0.001)
            {
                switch (Utility.Random(2))
                {
                    case 0:
                        c.DropItem(new BloodstainedRing());
                        break;
                    case 1:
                        c.DropItem(new BloodstainedNecklace());
                        break;
                    case 2:
                        c.DropItem(new BloodstainedEarrings());
                        break;
                }
            }

            if (Utility.RandomDouble() <= 0.05)
            {
                c.DropItem(new AsylumKey { KeyType = AsylumKeyType.Middle, Name = "magical asylum key: goods storage", Hue = 38 });
            }
            base.OnDeath(c);
        }

        public AsylumVampire(Serial serial)
            : base(serial)
        {}

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

                    HideEffect();
                    CantWalk = true;
                    Hidden = true;
                    Blessed = true;
                    Combatant = null;
                    CurrentSpell.Add(Timer.DelayCall(TimeSpan.FromSeconds(1), BackStab_Callback));
                }
            }
        }

        protected void BackStab_Callback()
        {
            var rnd = new Random();
            List<Mobile> rangelist = AcquireTargets(Location, 15).OrderBy(x => rnd.Next()).Take(5).ToList();

            Point3D origloc = Location;
            int count = 1;
            foreach (Mobile mobile in rangelist)
            {
                Mobile mobile1 = mobile;
                Timer.DelayCall(TimeSpan.FromSeconds(2.5 * count), () =>
                {
                    Location = mobile1.Location;
                    Hidden = false;
                    HideEffect();
                    Timer.DelayCall(TimeSpan.FromSeconds(0.2), () =>
                    {
                        Say("*Bites your neck.*");
                        int range = Utility.RandomMinMax(1, 2);
                        int zOffset = mobile1.Mounted ? 20 : 10;

                        Point3D src = mobile1.Location.Clone3D(0, 0, zOffset);
                        var points = src.GetAllPointsInRange(mobile1.Map, 0, range);

                        Effects.PlaySound(mobile1.Location, mobile1.Map, 0x19C);

                        Timer.DelayCall(
                            TimeSpan.FromMilliseconds(100),
                            () =>
                            {
                                foreach (Point3D trg in points)
                                {
                                    int bloodID = Utility.RandomMinMax(4650, 4655);

                                    new MovingEffectInfo(src, trg.Clone3D(0, 0, 2), mobile1.Map, bloodID).MovingImpact(
                                        info =>
                                        {
                                            var blood = new Blood
                                            {
                                                ItemID = bloodID
                                            };
                                            blood.MoveToWorld(info.Target.Location, info.Map);

                                            Effects.PlaySound(info.Target, info.Map, 0x028);
                                        });
                                }
                            });
                        Animate();
                    });
                    mobile1.Damage(20, this);
                    Hits += 20;
                    Timer.DelayCall(TimeSpan.FromSeconds(1.5), () =>
                    {
                        HideEffect();
                        Hidden = true;
                    });
                });
                count++;
            }
            Timer.DelayCall(TimeSpan.FromSeconds(2.5 * count), () =>
            {
                Location = origloc;
                Timer.DelayCall(TimeSpan.FromSeconds(1), () =>
                {
                    Blessed = false;
                    HideEffect();
                    Hidden = false;
                    CantWalk = false;
                });
            });
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (defender is BaseCreature)
                defender.Damage(25, this);
        }

        public void HideEffect()
        {
            Effects.SendLocationEffect(new Point3D(X + 1, Y, Z + 4), Map, 0x3728, 13);
            Effects.SendLocationEffect(new Point3D(X + 1, Y, Z), Map, 0x3728, 13);
            Effects.SendLocationEffect(new Point3D(X + 1, Y, Z - 4), Map, 0x3728, 13);
            Effects.SendLocationEffect(new Point3D(X, Y + 1, Z + 4), Map, 0x3728, 13);
            Effects.SendLocationEffect(new Point3D(X, Y + 1, Z), Map, 0x3728, 13);
            Effects.SendLocationEffect(new Point3D(X, Y + 1, Z - 4), Map, 0x3728, 13);

            Effects.SendLocationEffect(new Point3D(X + 1, Y + 1, Z + 11), Map, 0x3728, 13);
            Effects.SendLocationEffect(new Point3D(X + 1, Y + 1, Z + 7), Map, 0x3728, 13);
            Effects.SendLocationEffect(new Point3D(X + 1, Y + 1, Z + 3), Map, 0x3728, 13);
            Effects.SendLocationEffect(new Point3D(X + 1, Y + 1, Z - 1), Map, 0x3728, 13);

            PlaySound(0x228);
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