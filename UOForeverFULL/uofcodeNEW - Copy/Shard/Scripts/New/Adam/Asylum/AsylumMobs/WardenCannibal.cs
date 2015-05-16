#region References

using System;
using System.Collections.Generic;
using System.Linq;
using Server.ContextMenus;
using Server.Engines.CannedEvil;
using Server.Engines.CustomTitles;
using Server.Items;
using Server.Network;
using VitaNex.FX;

#endregion

namespace Server.Mobiles
{
    [CorpseName("remains of a corpse devourer")]
    public class CannibalWarden : BaseChampion
    {
        private static readonly SkillName[] _Skills = ((SkillName) 0).GetValues<SkillName>();
        private DateTime _NextCast = DateTime.UtcNow;
        private DateTime _NextFeast = DateTime.UtcNow;
        private DateTime _NextMerciless = DateTime.UtcNow;
        private DateTime _NextSummon = DateTime.UtcNow;

        [Constructable]
        public CannibalWarden()
            : base(AIType.AI_Arcade, FightMode.Closest, 16, 1, 0.1, 0.2)
        {
            Name = NameList.RandomName("daemon");
            SpecialTitle = "[Asylum Warden]";

            TitleHue = 1161;

            Body = 400;

            Fame = 11250;
            Karma = -11250;

            SpeechHue = YellHue = 133;

            VirtualArmor = 50;

            BaseSoundID = 372;

            SetStr(100, 200);
            SetDex(186, 275);
            SetInt(786, 875);


            SetHits(30000);
            SetDamage(5, 10);

            foreach (SkillName skill in _Skills)
            {
                SetSkill(skill, 80.0, 120.0);
            }

            if (Backpack != null)
            {
                Backpack.Delete();
            }

            EquipItem(new BottomlessBackpack());

            EquipItem(
                new Tetsubo
                {
                    Name = "The Warden's Truncheon",
                    Identified = true,
                    Movable = false,
                    LootType = LootType.Blessed
                });

            EquipItem(
                new WardenChest
                {
                    Identified = true,
                    Movable = false,
                    LootType = LootType.Blessed
                });

            EquipItem(
                new WardenArms
                {
                    Identified = true,
                    Movable = false,
                    LootType = LootType.Blessed
                });

            EquipItem(
                new WardenGloves
                {
                    Identified = true,
                    Movable = false,
                    LootType = LootType.Blessed
                });

            EquipItem(
                new WardenLegs
                {
                    Identified = true,
                    Movable = false,
                    LootType = LootType.Blessed
                });

            EquipItem(
                new WardenMempo
                {
                    Identified = true,
                    Movable = false,
                    LootType = LootType.Blessed
                });
        }

        public CannibalWarden(Serial serial)
            : base(serial)
        {}


        public virtual TimeSpan DevourInterval { get { return TimeSpan.FromSeconds(Utility.RandomMinMax(5, 15)); } }


        public virtual bool ThrowBomb { get { return Alive; } }
        public virtual TimeSpan ThrowBombInterval { get { return TimeSpan.FromSeconds(Utility.RandomMinMax(30, 45)); } }


        public bool ToggleSummon { get; set; }
        public TimeSpan SummonInterval { get { return TimeSpan.FromSeconds(Utility.RandomMinMax(30, 60)); } }

        private List<Timer> CurrentSpell { get; set; }
        public TimeSpan CastInterval { get { return TimeSpan.FromSeconds(Utility.RandomMinMax(10, 15)); } }

        public TimeSpan FeastInterval { get { return TimeSpan.FromSeconds(Utility.RandomMinMax(38, 58)); } }

        public TimeSpan MercilessInterval { get { return TimeSpan.FromSeconds(Utility.RandomMinMax(10, 20)); } }


        public override string DefaultName { get { return "a warden"; } }
        public override FoodType FavoriteFood { get { return FoodType.Gold; } }
        public override bool CanRummageCorpses { get { return true; } }
        public override bool AlwaysMurderer { get { return true; } }
        public override bool CanDestroyObstacles { get { return true; } }
        public override bool CanFlee { get { return false; } }
        public override bool AutoDispel { get { return true; } }
        public override bool Unprovokable { get { return true; } }
        public override bool ShowFameTitle { get { return false; } }

        public override ChampionSkullType SkullType { get { return ChampionSkullType.Death; } }

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

                if (Hits < HitsMax * .75)
                {
                    ToggleSummon = true;
                }
                if (ToggleSummon && now > _NextSummon)
                {
                    _NextSummon = now + SummonInterval;

                    CurrentSpell.Clear();
                    CurrentSpell.Add(Timer.DelayCall(TimeSpan.FromSeconds(3.0), Summon_Callback));
                }

                else if (now > _NextFeast)
                {
                    _NextFeast = now + FeastInterval;

                    CurrentSpell.Clear();
                    CurrentSpell.Add(Timer.DelayCall(TimeSpan.FromSeconds(3.0), Feast_Callback));
                }
                else if (now > _NextMerciless)
                {
                    _NextMerciless = now + MercilessInterval;

                    CurrentSpell.Clear();
                    CurrentSpell.Add(Timer.DelayCall(TimeSpan.FromSeconds(3.5), Merciless_Callback));
                }
            }
        }

        protected void Merciless_Callback()
        {
            Mobile target = Combatant;

            foreach (Mobile m in AcquireTargets(Location, 2).Take(1))
            {
                Animate();

                if (m != null)
                {
                    if (Utility.RandomBool())
                    {
                        Say("Bend over.");
                    }
                    else
                    {
                        Emote("*The warden begins to mercilessly beat {0}.*", target.RawName);
                    }
                    if (target is BaseCreature)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            Animate();
                            target.Damage(100, this);
                            BloodEffect(1, target);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            Animate();
                            target.Damage(10, this);
                            BloodEffect(1, target);
                        }
                    }
                }
            }
        }

        protected void Summon_Callback()
        {
            if (Deleted || Map == null || Map == Map.Internal || Location == Point3D.Zero)
            {
                return;
            }

            var mob1 = new AsylumVampire();
            var mob2 = new AsylumVampire();

            AnimateSpell();

            Timer.DelayCall(TimeSpan.FromSeconds(2), () =>
            {
                BloodEffect(4, this);
                mob1.MoveToWorld(Location, Map);
                mob2.MoveToWorld(Location, Map);
            });


            Yell("Fiends of the night, hear me! FEED!");
        }

        protected void Feast_Callback()
        {
            Emote(Utility.RandomBool() ? "Have a bite." : "I'm not greedy, I'll share.");
            int[] BodyPartArray = {7584, 7583, 7586, 7585, 7588, 7587};
            foreach (Mobile m in AcquireTargets(Location, 15).Take(10))
            {
                Animate();

                Mobile m2 = m;
                new MovingEffectInfo(this, m, Map, BodyPartArray[Utility.Random(BodyPartArray.Length)])
                    .MovingImpact(
                        eff =>
                        {
                            int range = Utility.RandomMinMax(1, 2);

                            var damage = (int) Math.Floor(m.Hits * 0.50);

                            BloodEffect(range, m2);
                            m2.TryParalyze(TimeSpan.FromSeconds(4));
                            m2.Damage(damage, this, true);

                            m2.SendMessage(133, "You were hit with a piece of gnawed on human remains. Gross!");
                        });
            }
        }

        public override void Damage(int amount, Mobile m, bool informMount)
        {
            // Poison will cause all damage to heal the devourer instead.
            if (Poison != null)
            {
                // Uncomment to allow damage while poison to be a beneficial action.
                // This will cause the damager to go grey/criminal.
                /*
				if (m != null)
				{
					m.DoBeneficial(this);
				}
				*/

                Hits += amount;

                if (Utility.RandomDouble() < 0.10)
                {
                    NonlocalOverheadMessage(
                        MessageType.Regular,
                        0x21,
                        false,
                        String.Format(
                            "*{0} {1}*", Name,
                            Utility.RandomList("looks healthy", "looks stronger", "is absorbing damage", "is healing")));
                }
            }
            else
            {
                base.Damage(amount, m, informMount);
            }
        }

        public override void OnPoisoned(Mobile from, Poison poison, Poison oldPoison)
        {
            NonlocalOverheadMessage(MessageType.Regular, 0x21, false, "*The poison seems to have the opposite effect*");
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            list.RemoveAll(e => e is PaperdollEntry);
        }

        public void BloodEffect(int range, Mobile m)
        {
            int zOffset = m.Mounted ? 20 : 10;

            Point3D src = m.Location.Clone3D(0, 0, zOffset);
            Point3D[] points = src.GetAllPointsInRange(m.Map, 0, range);

            Effects.PlaySound(m.Location, m.Map, 0x19C);

            Timer.DelayCall(
                TimeSpan.FromMilliseconds(100),
                () =>
                {
                    foreach (Point3D trg in points)
                    {
                        int bloodID = Utility.RandomMinMax(4650, 4655);

                        new MovingEffectInfo(src, trg.Clone3D(0, 0, 2), m.Map, bloodID)
                            .MovingImpact(
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

        public virtual IEnumerable<Mobile> AcquireTargets(Point3D p, int range)
        {
            return
                p.GetMobilesInRange(Map, range)
                    .Where(
                        m =>
                            m != null && !m.Deleted && m != this && m.AccessLevel <= AccessLevel && m.Alive &&
                            (m.Player || Combatant == m ||
                             (m is BaseCreature && ((BaseCreature) m).GetMaster() is PlayerMobile)));
        }

        public override int GetAngerSound()
        {
            return 373;
        }

        public override int GetAttackSound()
        {
            return Female ? 1524 : 1528;
        }

        public override int GetDeathSound()
        {
            return Female ? 1525 : 1529;
        }

        public override int GetHurtSound()
        {
            return Female ? 1527 : 1531;
        }

        public override int GetIdleSound()
        {
            return Utility.RandomList(372, 373);
        }

        protected override bool OnMove(Direction d)
        {
            bool allow = base.OnMove(d);

            if (allow)
            {
                foreach (Corpse corpse in
                    this.GetEntitiesInRange<Corpse>(Map, 2).Take(10)
                        .Where(
                            c =>
                                c != null && !c.Deleted && !c.IsDecoContainer && !c.DoesNotDecay && !c.IsBones))
                {
                    Emote("*You see the warden eat a corpse.*");

                    BloodEffect(3, this);

                    foreach (Item item in
                        corpse.Items.Where(item => item != null && !item.Deleted && item.Movable && item.Visible)
                            .ToArray())
                    {
                        Backpack.DropItem(item);
                    }

                    corpse.TurnToBones();
                    Hits += 100;
                }
            }

            return allow;
        }

        public override void OnDeath(Container c)
        {
            if (Utility.RandomDouble() < 0.25)
            {
                var rand = Utility.Random(0, 5);
                switch (rand)
                {
                    case 0:
                        c.DropItem(new WardenArms {Identified = true});
                        break;
                    case 1:
                        c.DropItem(new WardenChest {Identified = true});
                        break;
                    case 2:
                        c.DropItem(new WardenGloves {Identified = true});
                        break;
                    case 3:
                        c.DropItem(new WardenLegs {Identified = true});
                        break;
                    case 4:
                        c.DropItem(new WardenMempo {Identified = true});
                        break;
                }
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

            scroll = new SkillScroll();
            scroll.Randomize();
            c.DropItem(scroll);

            scroll = new SkillScroll();
            scroll.Randomize();
            c.DropItem(scroll);

            c.DropItem(new TitleScroll("The Warden"));

            if (0.01 > Utility.RandomDouble())
            {
                c.DropItem(new HueScroll(2049));
            }
            base.OnDeath(c);
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.SuperBoss, 2);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.ReadInt();
        }

        private sealed class BottomlessBackpack : StrongBackpack
        {
            public BottomlessBackpack()
            {
                MaxItems = Int16.MaxValue;
                Movable = false;
                Hue = 16385;
                Weight = 0.0;
            }

            public BottomlessBackpack(Serial serial)
                : base(serial)
            {}

            public override int MaxWeight { get { return Int16.MaxValue; } }

            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);

                writer.SetVersion(0);
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);

                reader.GetVersion();
            }
        }
    }
}