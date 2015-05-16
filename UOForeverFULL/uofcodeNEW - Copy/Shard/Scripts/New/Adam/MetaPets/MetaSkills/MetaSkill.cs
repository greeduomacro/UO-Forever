#region References

using System;
using System.Collections.Generic;
using System.Linq;
using Server.Commands.Generic;
using Server.Items;
using Server.Mobiles.MetaSkills;
using Server.Network;
using VitaNex;
using VitaNex.FX;
using VitaNex.Network;

#endregion

namespace Server.Mobiles.MetaPet.Skills
{
    public class BaseMetaSkill : PropertyObject
    {
        public int _level;
        public virtual int Level { get { return _level; } set { _level = value; } }

        public string Name { get; set; }

        public int Experience { get; set; }
        public int NextLevelExperience { get; set; }
        public int MaxLevel { get; set; }

        public MetaSkillType MetaSkillType { get; set; }
        public double ChanceToActivate { get; set; }
        public TimeSpan CoolDown { get; set; }
        public double AbilityMultiplier { get; set; }

        public DateTime NextUse { get; set; }

        public BaseMetaSkill(MetaSkillType skilltype, string name, int nle, int maxlevel, double chance, TimeSpan cooldown,
            double multi)
        {
            Level = 1;
            Name = name;
            MetaSkillType = skilltype;
            Experience = 0;
            NextLevelExperience = nle;
            MaxLevel = maxlevel;
            ChanceToActivate = chance;
            CoolDown = cooldown;
            AbilityMultiplier = multi;
        }

        public BaseMetaSkill(GenericReader reader)
            : base(reader)
        {}

        public override void Reset()
        {}

        public override void Clear()
        {}

        public virtual void SetLevel()
        {}

        public virtual void FindAbility(BaseCreature target, BaseMetaPet pet)
        {
            if (MetaSkillType.GoldFind == MetaSkillType)
            {
                DoAbilityGoldFind(target, pet);
            }

            if (MetaSkillType.VenemousBlood == MetaSkillType && DateTime.UtcNow >= NextUse &&
                Utility.RandomDouble() <= ChanceToActivate)
            {
                DoAbilityNoxiousBlood(target, pet);
            }

            if (MetaSkillType.Quicksilver == MetaSkillType && DateTime.UtcNow >= NextUse &&
                Utility.RandomDouble() <= ChanceToActivate && pet.GetStatMod("QuickSilver") == null)
            {
                DoAbilityQuicksilver(target, pet);
            }

            if (MetaSkillType.Bleed == MetaSkillType && DateTime.UtcNow >= NextUse &&
            Utility.RandomDouble() <= ChanceToActivate)
            {
                DoAbilityBleed(target, pet);
            }

            if (MetaSkillType.Molten == MetaSkillType && DateTime.UtcNow >= NextUse &&
                Utility.RandomDouble() <= ChanceToActivate)
            {
                DoAbilityMoltenBreath(target, pet);
            }
        }

        #region Molten Breath

        public void DoAbilityMoltenBreath(BaseCreature target, BaseMetaPet pet)
        {
            pet.AIObject.NextMove = DateTime.UtcNow + TimeSpan.FromSeconds(1.0);
            pet.PlaySound(pet.BaseSoundID);
            pet.Animate(12, 5, 1, true, false, 0);

            var queue = new EffectQueue();
            queue.Deferred = false;

            Point3D endpoint = target.Location;
            Point3D[] line = pet.Location.GetLine3D(endpoint, pet.Map);

            if (target.InRange(pet.Location, 10))
            {
                int n = 0;
                foreach (Point3D p in line)
                {
                    n += 20;
                    Point3D p1 = p;
                    queue.Add(
                        new EffectInfo(
                            p,
                            pet.Map,
                            14089,
                            0,
                            10,
                            30,
                            EffectRender.Normal,
                            TimeSpan.FromMilliseconds(n),
                            () =>
                            {
                                foreach (Mobile player in AcquireTargets(pet, p1, 0))
                                {
                                    player.Damage(Level * 5, pet);
                                }
                            }));
                }

                queue.Callback = () =>
                {
                    BaseExplodeEffect e = ExplodeFX.Fire.CreateInstance(
                        endpoint, pet.Map, 2, 0, TimeSpan.FromMilliseconds(1000 - ((10) * 100)), null, () =>
                        {
                            foreach (Mobile mobile in AcquireTargets(pet, endpoint, 2))
                            {
                                mobile.Damage(Level * 3, pet);
                            }
                        });
                    e.Send();
                };


                queue.Process();
                Experience++;

                if (Experience >= NextLevelExperience)
                {
                    LevelUpMoltenBreath(pet.ControlMaster);
                }

                NextUse = DateTime.UtcNow + CoolDown;
            }
        }

        public List<Mobile> AcquireTargets(BaseCreature pet, Point3D p, int range)
        {
            return
                p.GetMobilesInRange(pet.Map, range)
                    .Where(
                        m =>
                            m != null && m != pet && m.Alive && m is BaseCreature && !m.IsControlled())
                    .ToList();
        }

        public void LevelUpMoltenBreath(Mobile to)
        {
            if (Level < MaxLevel)
            {
                Level++;
                NextLevelExperience = Level * 200;
                Experience = 0;
                CoolDown = CoolDown.Subtract(TimeSpan.FromSeconds(1));
                ChanceToActivate += 0.01;

                if (to != null)
                {
                    to.SendMessage(54,
                        "Your Meta-Pet's Molten breath ability has leveled up.  It is now Level: " + Level + ".");
                }
            }
        }

        #endregion

        #region Bleed

        public void DoAbilityBleed(BaseCreature target, BaseMetaPet pet)
        {
            var timer = new InternalBleedTimer(pet, target, Level);
            timer.Start();

            Experience ++;
            if (Experience >= NextLevelExperience)
            {
                LevelUpBleed(pet.ControlMaster);
            }
            NextUse = DateTime.UtcNow + CoolDown;
        }

        public void LevelUpBleed(Mobile to)
        {
            if (Level < MaxLevel)
            {
                Level++;
                NextLevelExperience = Level * 100;
                Experience = 0;
                CoolDown = CoolDown.Subtract(TimeSpan.FromSeconds(1));
                ChanceToActivate += 0.01;
                AbilityMultiplier += 1.0;

                if (to != null)
                {
                    to.SendMessage(54,
                        "Your Meta-Pet's bloody talons ability has leveled up.  It is now Level: " + Level + ".");
                }
            }
        }

        private class InternalBleedTimer : Timer
        {
            private BaseCreature _Pet;
            private BaseCreature _Target;
            private int _Count;
            private int _Level;

            public InternalBleedTimer(BaseCreature pet, BaseCreature target, int level)
                : base(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(0.5))
            {
                _Pet = pet;
                _Target = target;
                _Count = 0;
                _Level = level;
            }

            protected override void OnTick()
            {
                if (_Count < 10)
                {
                    CreateBlood(_Pet.Location, _Pet.Map, _Pet.BloodHue, false, _Pet);
                    _Target.Damage(1 * _Level, _Pet);
                    _Count++;
                }
                else
                {
                    Stop();
                }
            }
        }

        public static void CreateBlood(Point3D loc, Map map, int hue, bool delayed, Mobile m)
        {
            int bloodID = Utility.RandomMinMax(4650, 4655);
            new Blood(bloodID, hue, delayed).MoveToWorld(new Point3D(BloodOffset(loc.X), BloodOffset(loc.Y), loc.Z), map);
        }

        public static int BloodOffset(int coord)
        {
            return coord + Utility.RandomMinMax(-1, 1);
        }

        #endregion

        #region Quicksilver

        public void DoAbilityQuicksilver(BaseCreature target, BaseMetaPet pet)
        {
            pet.PublicOverheadMessage(MessageType.Label, 34, true, "*Quicksilver*");
            pet.HueMod = 2955;
            Timer.DelayCall(TimeSpan.FromSeconds((int)Math.Ceiling(2 * AbilityMultiplier)), () =>
            {
                pet.HueMod = -1;
            });

            pet.AddStatMod(new StatMod(StatType.Dex, "QuickSilver", 200 * _level, TimeSpan.FromSeconds(2 * Level)));
            pet.Stam = pet.Dex;

            Experience++;

            if (Experience >= NextLevelExperience)
            {
                LevelUpQuicksilver(pet.ControlMaster);
            }
            NextUse = DateTime.UtcNow + CoolDown;
        }

        public void LevelUpQuicksilver(Mobile to)
        {
            if (Level < MaxLevel)
            {
                Level++;
                NextLevelExperience = Level * 200;
                Experience = 0;
                CoolDown = CoolDown.Subtract(TimeSpan.FromSeconds(1));
                ChanceToActivate += 0.01;
                AbilityMultiplier += 1.0;

                if (to != null)
                {
                    to.SendMessage(54,
                        "Your Meta-Pet's quicksilver ability has leveled up.  It is now Level: " + Level + ".");
                }
            }
        }

        #endregion

        #region Noxious Blood

        public void DoAbilityNoxiousBlood(BaseCreature target, BaseMetaPet pet)
        {
            int range = Utility.RandomMinMax(3, 5);
            int zOffset = 10;

            Point3D src = pet.Location.Clone3D(0, 0, zOffset);
            Point3D[] points = src.GetAllPointsInRange(pet.Map, range, range);

            Effects.PlaySound(pet.Location, pet.Map, 0x19C);

            Timer.DelayCall(
                TimeSpan.FromMilliseconds(100),
                () =>
                {
                    foreach (Point3D trg in points)
                    {
                        int bloodID = Utility.RandomMinMax(4650, 4655);

                        new MovingEffectInfo(src, trg.Clone3D(0, 0, 2), pet.Map, bloodID, 1367).MovingImpact(
                            info =>
                            {
                                var blood = new Blood
                                {
                                    ItemID = bloodID,
                                    Hue = 1368
                                };
                                blood.MoveToWorld(info.Target.Location, info.Map);

                                Effects.PlaySound(info.Target, info.Map, 0x028);
                            });
                    }
                    foreach (
                        Mobile mobile in
                            pet.Location.GetMobilesInRange(pet.Map, range)
                                .Where(m => m is BaseCreature && !m.IsControlled()).Take(4))
                    {
                        var num = (int) Math.Floor((float) Level / 2);
                        if (num == 5)
                            num = 4;
                        mobile.ApplyPoison(pet, Poison.GetPoison(num));
                    }
                    Experience ++;
                    if (Experience >= NextLevelExperience)
                    {
                        LevelUpNoxiousBlood(pet.ControlMaster);
                    }
                });
            NextUse = DateTime.UtcNow + CoolDown;
        }

        public void LevelUpNoxiousBlood(Mobile to)
        {
            if (Level < MaxLevel)
            {
                Level++;
                NextLevelExperience = Level * 200;
                Experience = 0;
                CoolDown = CoolDown.Subtract(TimeSpan.FromSeconds(1));
                ChanceToActivate += 0.01;

                if (to != null)
                {
                    to.SendMessage(54,
                        "Your Meta-Pet's Noxious Blood ability has leveled up.  It is now Level: " + Level + ".");
                }
            }
        }

        #endregion

        #region Gold Find

        public void DoAbilityGoldFind(BaseCreature target, BaseMetaPet pet)
        {
            int g = Utility.Random(1, (int) Math.Ceiling(target.HitsMax * AbilityMultiplier));
            target.PackItem(new Gold(g));
            if (pet.Controlled)
            {
                pet.ControlMaster.SendMessage(54, "Your Meta-Pet has found " + g + " pieces of gold.");
            }
            Experience ++;
            if (Experience >= NextLevelExperience)
            {
                LevelUpGoldFind(pet.ControlMaster);
            }
        }

        public void LevelUpGoldFind(Mobile to)
        {
            if (Level < MaxLevel)
            {
                Level++;
                NextLevelExperience = Level * 100;
                Experience = 0;

                AbilityMultiplier += 0.015;

                if (to != null)
                {
                    to.SendMessage(54,
                        "Your Meta-Pet's gold find ability has leveled up.  It is now Level: " + Level + ".");
                }
            }
        }

        #endregion

        public override void Serialize(GenericWriter writer)
        {
            int version = writer.SetVersion(0);

            switch (version)
            {
                case 0:
                {
                    writer.Write(_level);
                    writer.Write(Name);
                    writer.Write((int) MetaSkillType);
                    writer.Write(ChanceToActivate);
                    writer.Write(CoolDown);
                    writer.Write(AbilityMultiplier);
                    writer.Write(Experience);
                    writer.Write(NextLevelExperience);
                    writer.Write(MaxLevel);
                }
                    break;
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            int version = reader.GetVersion();

            switch (version)
            {
                case 0:
                {
                    _level = reader.ReadInt();
                    Name = reader.ReadString();
                    MetaSkillType = (MetaSkillType) reader.ReadInt();
                    ChanceToActivate = reader.ReadDouble();
                    CoolDown = reader.ReadTimeSpan();
                    AbilityMultiplier = reader.ReadDouble();
                    Experience = reader.ReadInt();
                    NextLevelExperience = reader.ReadInt();
                    MaxLevel = reader.ReadInt();
                }
                    break;
            }
        }
    }
}