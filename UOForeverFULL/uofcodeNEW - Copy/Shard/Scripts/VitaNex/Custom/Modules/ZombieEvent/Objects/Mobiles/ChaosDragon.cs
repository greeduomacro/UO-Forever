#region References

using System;
using System.Collections.Generic;
using System.Linq;
using Server.Engines.ZombieEvent;
using Server.Items;
using VitaNex.FX;
using VitaNex.Network;

#endregion

namespace Server.Mobiles
{
    [CorpseName("a desiccated corpse")]
    public class ChaosDragon : BaseCreature
    {
        private DateTime _NextCast = DateTime.UtcNow;

        private DateTime _NextUltima = DateTime.UtcNow;

        //ShadowStep
        private int _UltimaDamage = 10;

        public override bool ReduceSpeedWithDamage { get { return true; } }


        [Constructable]
        public ChaosDragon()
            : base(AIType.AI_Arcade, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = NameList.RandomName("centaur");
            BodyValue = 197;
            BaseSoundID = 362;
            SpecialTitle = "Chaos Dragon";
            TitleHue = 1174;

            BaseSoundID = 362;

            SetStr(986, 1185);
            SetDex(177, 255);
            SetInt(151, 250);

            SetDamage(22, 29);

            SetHits(7000);

            SetSkill(SkillName.Anatomy, 25.1, 50.0);
            SetSkill(SkillName.EvalInt, 90.1, 100.0);
            SetSkill(SkillName.Magery, 95.5, 100.0);
            SetSkill(SkillName.Meditation, 25.1, 50.0);
            SetSkill(SkillName.MagicResist, 100.5, 150.0);
            SetSkill(SkillName.Tactics, 90.1, 100.0);
            SetSkill(SkillName.Wrestling, 90.1, 100.0);

            SpeechHue = YellHue = 34;

            VirtualArmor = 90;

            PackGold(3000, 3500);
            PackMagicItems(5, 5, 0.95, 0.95);
            PackMagicItems(5, 5, 0.80, 0.65);
            PackMagicItems(5, 5, 0.80, 0.65);
            PackMagicItems(6, 6, 0.80, 0.65);

            FreelyLootable = true;

            CurrentSpeed = 0.6;
            PassiveSpeed = 0.6;
            ActiveSpeed = 0.16;
            RangePerception = 20;

            Damagers = new Dictionary<ZombieAvatar, int>();
        }

        public ChaosDragon(Serial serial)
            : base(serial)
        {}

        public Dictionary<ZombieAvatar, int> Damagers { get; set; }


        [CommandProperty(AccessLevel.GameMaster)]
        public int UltimaDamage { get { return _UltimaDamage; } set { _UltimaDamage = value; } }

        public TimeSpan UltimaInterval { get { return TimeSpan.FromSeconds(Utility.RandomMinMax(11, 22)); } }


        private List<Timer> CurrentSpell { get; set; }
        public TimeSpan CastInterval { get { return TimeSpan.FromSeconds(Utility.RandomMinMax(10, 15)); } }

        public override bool CanRummageCorpses { get { return true; } }
        public override bool CanDestroyObstacles { get { return true; } }
        public override bool CanFlee { get { return false; } }
        public override bool AutoDispel { get { return true; } }
        public override bool Unprovokable { get { return true; } }
        public override Poison PoisonImmune { get { return Poison.Lethal; } }
        public override bool ReacquireOnMovement { get { return true; } }


        public override bool OnBeforeDeath()
        {
            AwardPoints();

            return base.OnBeforeDeath();
        }

        public override void OnDeath(Container c)
        {
            for (int i = 0; i < 10; i++)
            {
                var suit = new LordBritishSuit();
                suit.AccessLevel = AccessLevel.Player;
                suit.LootType = LootType.Regular;
                c.DropItem(suit);
            }
            Engines.ZombieEvent.ZombieEvent.AddItem(c);
            base.OnDeath(c);

        }
        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            ZombieAvatar avatar = null;
            int amountmodified = amount;
            if (Damagers != null)
            {
                if (from is ZombieAvatar)
                {
                    avatar = from as ZombieAvatar;
                }
                else if (from is BaseCreature && ((BaseCreature) from).GetMaster() is ZombieAvatar)
                {
                    avatar = ((BaseCreature) from).GetMaster() as ZombieAvatar;
                }
                if (avatar != null && Damagers.ContainsKey(avatar))
                {
                    Damagers[avatar] += amountmodified;
                }
                else if (avatar != null)
                {
                    Damagers.Add(avatar, amountmodified);
                }
            }

            base.OnDamage(amount, from, willKill);
        }

        public void AwardPoints()
        {
            if (Damagers != null)
            {
                foreach (ZombieAvatar avatar in Damagers.Keys)
                {
                    if (avatar.Owner != null)
                    {
                        PlayerZombieProfile profile = ZombieEvent.EnsureProfile(avatar.Owner);
                        if (Damagers.ContainsKey(avatar))
                        {
                            profile.DragonBossDamage += Damagers[avatar];
                        }
                    }
                }
            }
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

                 if (now > _NextUltima && CurrentSpell.Count == 0)
                {
                    _NextUltima = now + UltimaInterval;

                    CurrentSpell.Clear();

                    CantWalk = true;
                    Combatant = null;
                    AnimateSpell();
                    CurrentSpell.Clear();
                    CurrentSpell.Add(Timer.DelayCall(TimeSpan.FromSeconds(2.0), Ultima_Callback));
                    CurrentSpell.Add(Timer.DelayCall(TimeSpan.FromSeconds(3.0), Ultima_Callback));
                    CurrentSpell.Add(Timer.DelayCall(TimeSpan.FromSeconds(4.0), Ultima_Callback));
                    CurrentSpell.Add(Timer.DelayCall(TimeSpan.FromSeconds(5.0), Ultima_Callback));
                    CurrentSpell.Add(Timer.DelayCall(TimeSpan.FromSeconds(6.0), Ultima_Callback));
                    CurrentSpell.Add(Timer.DelayCall(TimeSpan.FromSeconds(7.0), Ultima_Callback));
                    CurrentSpell.Add(Timer.DelayCall(TimeSpan.FromSeconds(8.0), Ultima_Callback));
                }
            }
        }

        public void Ultima_Callback()
        {
            List<Mobile> rangelist = AcquireTargets(Location, 15);
            int index = Utility.Random(rangelist.Count);
            if (index + 1 > rangelist.Count)
            {
                return;
            }

            var startloc = new Point3D(rangelist[index].X, rangelist[index].Y, 100);
            var point = new Point3D(rangelist[index].Location);

            var queue = new EffectQueue();
            queue.Deferred = false;

            queue.Add(
                new MovingEffectInfo(
                    startloc,
                    point,
                    Map,
                    13920,
                    60,
                    1,
                    EffectRender.Normal,
                    TimeSpan.FromMilliseconds(60),
                    () => { }));
            queue.Add(
                new MovingEffectInfo(
                    startloc,
                    point,
                    Map,
                    14360,
                    0,
                    30,
                    EffectRender.Normal,
                    null,
                    () => { }));
            queue.Process();

            Timer.DelayCall(TimeSpan.FromSeconds(2.0), UltimaImpact_Callback, point);
        }

        public void UltimaImpact_Callback(Point3D impactloc)
        {
            CurrentSpell.Clear();
            var queue = new EffectQueue();
            queue.Deferred = true;

            BaseExplodeEffect e = ExplodeFX.Smoke.CreateInstance(
                impactloc, Map, 5, 0, TimeSpan.FromMilliseconds(1000 - ((10) * 100)), null, () =>
                {
                    foreach (Mobile mobile in AcquireAllTargets(impactloc, 5))
                    {
                        if (mobile is ZombieAvatar)
                        {
                            mobile.Damage(UltimaDamage, this);
                        }
                    }
                });
            e.Send();

            queue.Process();
            CantWalk = false;
        }

        public List<Mobile> AcquireTargets(Point3D p, int range)
        {
            return
                p.GetMobilesInRange(Map, range)
                    .Where(
                        m =>
                            m != null && !m.Deleted && m != this && m.Alive && m is ZombieAvatar)
                    .ToList();
        }

        public List<Mobile> AcquireAllTargets(Point3D p, int range)
        {
            return
                p.GetMobilesInRange(Map, range)
                    .Where(
                        m =>
                            m != null && !m.Deleted && m != this && m.Alive && m is ZombieAvatar)
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

            int version = writer.SetVersion(0);

            switch (version)
            {
                case 0:
                {
                    writer.Write(Damagers.Count);

                    if (Damagers.Count > 0)
                    {
                        foreach (var kvp in Damagers)
                        {
                            writer.Write(kvp.Key);
                            writer.Write(kvp.Value);
                        }
                    }
                }
                    break;
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            Damagers = new Dictionary<ZombieAvatar, int>();

            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                {
                    int count = reader.ReadInt();

                    if (count > 0)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            var p = reader.ReadMobile<ZombieAvatar>();
                            int amt = reader.ReadInt();
                            if (p != null)
                            {
                                Damagers.Add(p, amt);
                            }
                        }
                    }
                }
                    break;
            }
        }
    }
}