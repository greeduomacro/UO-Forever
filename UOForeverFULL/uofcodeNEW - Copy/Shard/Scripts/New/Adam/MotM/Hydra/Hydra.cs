#region References

using System;
using System.Collections.Generic;
using System.Linq;
using Server.Engines.CannedEvil;
using Server.Engines.CustomTitles;
using Server.Items;
using Server.Network;
using VitaNex.FX;
using VitaNex.Network;
using VitaNex.Notify;

#endregion

namespace Server.Mobiles
{
    [CorpseName("a Hydra corpse")]
    public class HydraMotM : BaseChampion
    {
        private readonly Dictionary<Mobile, DateTime?> _FireBreathAffectedMobs = new Dictionary<Mobile, DateTime?>();
        private readonly Dictionary<Mobile, DateTime?> _WaterBreathAffectedMobs = new Dictionary<Mobile, DateTime?>();
        private readonly Dictionary<Mobile, DateTime?> _EarthBreathAffectedMobs = new Dictionary<Mobile, DateTime?>();

        private FireBreathInternalTimer _FireBreathTimer;
        private WaterBreathInternalTimer _WaterBreathTimer;
        private EarthBreathInternalTimer _EarthBreathTimer;

        private readonly List<BloodoftheHydra> _Bloods = new List<BloodoftheHydra>();

        private ExtEventMoongate _Moongate;

        private DateTime _NextCast = DateTime.UtcNow;

        public TimeSpan CastInterval { get { return TimeSpan.FromSeconds(Utility.RandomMinMax(5, 10)); } }

        public override ChampionSkullType SkullType { get { return ChampionSkullType.Special; } }

        public override bool Unprovokable { get { return true; } }
        public override bool AlwaysMurderer { get { return true; } }
        public override bool Uncalmable { get { return true; } }
        public override Poison PoisonImmune { get { return Poison.Lethal; } }
        public override int TreasureMapLevel { get { return 5; } }

        [Constructable]
        public HydraMotM()
            : base(AIType.AI_Arcade, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Lernaean";
            SpecialTitle = "The Hydra";
            TitleHue = 1259;

            Body = 0x109;
            BaseSoundID = 1149;
            Hue = 0x47e;

            SetStr(510, 700);
            SetDex(510, 750);
            SetInt(310, 400);

            SetHits(40000);

            SetDamage(1, 6);

            SetSkill(SkillName.Anatomy, 200.0);
            SetSkill(SkillName.EvalInt, 120.0);
            SetSkill(SkillName.Magery, 120.0);
            SetSkill(SkillName.Meditation, 120.0);
            SetSkill(SkillName.MagicResist, 150.0);
            SetSkill(SkillName.Tactics, 200.0);
            SetSkill(SkillName.Wrestling, 200.0);

            Fame = 0;
            Karma = -20000;

            VirtualArmor = 45;

            SpeechHue = 34;

            PackGem();
            PackGold(4700, 6950);

            _FireBreathTimer = new FireBreathInternalTimer(this);
            _FireBreathTimer.Start();

            _WaterBreathTimer = new WaterBreathInternalTimer(this);
            _WaterBreathTimer.Start();

            _EarthBreathTimer = new EarthBreathInternalTimer(this);
            _EarthBreathTimer.Start();
        }

        public HydraMotM(Serial serial)
            : base(serial)
        {}

        public void AddBlood(BloodoftheHydra blood)
        {
            if (blood != null && !blood.Deleted)
            {
                _Bloods.AddOrReplace(blood);
            }
        }

        public void RemoveBlood(BloodoftheHydra blood)
        {
            if (blood != null)
            {
                _Bloods.Remove(blood);
            }
        }

        public override void OnDeath(Container c)
        {
            _FireBreathTimer.Stop();
            _WaterBreathTimer.Stop();

            InvalidateHue();

            foreach (BloodoftheHydra target in _Bloods.ToArray())
            {
                target.Kill();
            }

            _Bloods.Clear();

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

            if (_Moongate != null && !_Moongate.Deleted)
            {
                _Moongate.Delete();
            }

            if (GlobalTownCrierEntryList.Instance.Entries != null)
            {
                GlobalTownCrierEntryList.Instance.Entries.Clear();
            }

            base.OnDeath(c);
        }

        public override void Delete()
        {
            InvalidateHue();

            foreach (BloodoftheHydra target in _Bloods.ToArray())
            {
                target.Kill();
            }

            _Bloods.Clear();

            if (_Moongate != null)
            {
                _Moongate.Delete();
            }

            if (GlobalTownCrierEntryList.Instance.Entries != null)
            {
                GlobalTownCrierEntryList.Instance.Entries.Clear();
            }

            base.Delete();
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            _FireBreathAffectedMobs.Clear();
            _WaterBreathAffectedMobs.Clear();
            _EarthBreathAffectedMobs.Clear();

            _FireBreathTimer.Stop();
            _FireBreathTimer = null;

            _WaterBreathTimer.Stop();
            _WaterBreathTimer = null;

            _EarthBreathTimer.Stop();
            _EarthBreathTimer = null;
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            if (Utility.RandomDouble() <= 0.05)
            {
                Emote("*Screams out in pain as poisonous blood falls from its wounds*");

                BloodoftheHydra(Utility.RandomMinMax(2, 6));
            }

            base.OnDamage(amount, from, willKill);
        }

        public void BloodoftheHydra(int count)
        {
            if (count <= 0)
            {
                return;
            }

            List<Mobile> targets = AcquireAllTargets(Location, 10);

            if (targets.Count == 0)
            {
                return;
            }

            if (targets.Count > count)
            {
                targets.Shuffle();
            }

            var q = new MovingEffectQueue
            {
                Deferred = true,
                Handler = e => new HydraBlood(this).MoveToWorld(e.Target.Location, e.Map)
            };

            foreach (Mobile t in targets.Where(t => t != null).Take(count))
            {
                q.Add(
                    new MovingEffectInfo(Location, t.Location, t.Map, 4655, 1166, 10, EffectRender.Normal,
                        TimeSpan.FromSeconds(1)));
            }

            q.Process();
        }

        public override void OnBeforeSpawn(Point3D location, Map m)
        {
            Notify.Broadcast<HydraMotMNotifyGump>(
                "The Monster of the Month, the Hydra, has spawned.[br]A moongate to his watery lair has been conjured at Britain bank.",
                true,
                1.0,
                10.0);

            _Moongate = new ExtEventMoongate(new Point3D(5997, 1473, 0), Map.Felucca)
            {
                Dispellable = false,
                Hue = 1259,
                Name = "To the Lair of the Hydra"
            };

            _Moongate.MoveToWorld(new Point3D(1411, 1716, 40), Map.Felucca);

            GlobalTownCrierEntryList.Instance.AddEntry(
                new[]
                {
                    "Adventurers are needed! The Hydra has awakened in his watery lair!",
                    "Please use the moongate located to the west of Britain bank and help vanquish this fiend!"
                },
                TimeSpan.FromMinutes(60));

            base.OnBeforeSpawn(location, m);
        }

        public override void GiveSpecialItems(List<Mobile> eligibleMobs, List<double> eligibleMobScores,
            double totalScores)
        {
            double currentTestValue = 0.0;
            double roll = Utility.RandomDouble() * totalScores;

            if (0.50 >= Utility.RandomDouble())
            {
                for (int i = 0; i < eligibleMobScores.Count; i++)
                {
                    currentTestValue += eligibleMobScores[i];

                    if (roll > currentTestValue)
                    {
                        continue;
                    }

                    if (!(eligibleMobs[i] is PlayerMobile) || eligibleMobs[i].Backpack == null)
                    {
                        continue;
                    }

                    var statue = new HydraStatue();

                    eligibleMobs[i].Backpack.DropItem(statue);
                    eligibleMobs[i].SendMessage(54, "You have received a Hydra statue!");

                    break;
                }
            }

            TitleHue titlehue;

            if (!CustomTitles.TryGetHue(1259, out titlehue) || titlehue == null)
            {
                return;
            }

            PlayerMobile titleWinner =
                Scores.OrderByDescending(kv => kv.Value).Select(kv => kv.Key).OfType<PlayerMobile>().FirstOrDefault();

            if (titleWinner == null)
            {
                return;
            }

            TitleProfile p = CustomTitles.EnsureProfile(titleWinner);

            if (p == null || p.Contains(titlehue))
            {
                return;
            }

            p.Add(titlehue);
            titleWinner.SendMessage(titlehue.Hue, "You have been granted the title hue: " + titlehue.Hue);
        }

        public override void OnThink()
        {
            base.OnThink();

            if (!this.InCombat())
            {
                return;
            }

            DateTime now = DateTime.UtcNow;

            if (now < _NextCast)
            {
                return;
            }

            _NextCast = now + CastInterval;

            CantWalk = true;

            Breath();
        }

        public void Breath()
        {
            Mobile target = AcquireTargets(Location, 15).GetRandom() ?? Combatant ?? FocusMob;

            if (target == null)
            {
                return;
            }

            Direction = GetDirectionTo(target) & Direction.Mask;
            Move(Direction);

            switch (Utility.Random(5))
            {
                case 0:
                {
                    PublicOverheadMessage(MessageType.Label, 1118, true,
                        "*The Hydra's earth attuned head takes a deep breath*");
                    Timer.DelayCall(TimeSpan.FromSeconds(3.0), EarthBreath);
                }
                    break;
                case 1:
                {
                    PublicOverheadMessage(MessageType.Label, 1161, true,
                        "*The Hydra's fire attuned head takes a deep breath*");
                    Timer.DelayCall(TimeSpan.FromSeconds(3.0), FireBreath);
                }
                    break;
                case 2:
                {
                    PublicOverheadMessage(MessageType.Label, 1165, true,
                        "*The Hydra's water attuned head takes a deep breath*");
                    Timer.DelayCall(TimeSpan.FromSeconds(3.0), WaterBreath);
                }
                    break;
                case 3:
                {
                    PublicOverheadMessage(MessageType.Label, 1159, true,
                        "*The Hydra's air attuned head takes a deep breath*");
                    Timer.DelayCall(TimeSpan.FromSeconds(3.0), LightningBreath);
                }
                    break;
                case 4:
                {
                    PublicOverheadMessage(MessageType.Label, 1167, true,
                        "*The Hydra's acid attuned head takes a deep breath*");
                    Timer.DelayCall(TimeSpan.FromSeconds(3.0), AcidBreath);
                }
                    break;
            }
        }

        public void EarthBreath()
        {
            //List<Mobile> buffer = new List<Mobile>();

            TimeSpan timeout = TimeSpan.FromSeconds(10);

            var fx = new EarthWaveEffect(Location, Map, Direction, 20, 0, TimeSpan.FromMilliseconds(1))
            {
                EffectHandler = e =>
                {
                    if (e.ProcessIndex != 0)
                    {
                        return;
                    }

                    DateTime now = DateTime.UtcNow;

                    foreach (Mobile t in AcquireAllTargets(e.Source.Location, 0) /*.Not(buffer.Contains)*/)
                    {
                        //buffer.Add(t);

                        _WaterBreathAffectedMobs.Remove(t);
                        _FireBreathAffectedMobs.Remove(t);

                        _EarthBreathAffectedMobs.AddOrReplace(t, o => (o != null ? o.Value : now) + timeout);

                        t.PrivateOverheadMessage(MessageType.Label, 1100, true, "*Petrified*", t.NetState);

                        if (t is BaseCreature)
                        {
                            t.Damage(200, this);
                        }
                        else
                        {
                            t.Damage(50, this);
                        }

                        if (t.CantWalk)
                        {
                            continue;
                        }

                        t.SolidHueOverride = 2407;
                        t.CantWalk = true;
                    }
                },
                Callback = () =>
                {
                    //buffer.Clear();
                    CantWalk = false;
                }
            };

            fx.Send();
        }

        private class EarthBreathInternalTimer : Timer
        {
            private readonly HydraMotM Hydra;

            public EarthBreathInternalTimer(HydraMotM hydra)
                : base(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1))
            {
                Hydra = hydra;
            }

            protected override void OnTick()
            {
                if (Hydra._EarthBreathAffectedMobs.Count <= 0)
                {
                    return;
                }

                DateTime now = DateTime.UtcNow;

                foreach (Mobile m in
                    Hydra._EarthBreathAffectedMobs.Where(
                        kv => kv.Value == null || kv.Value < now || !kv.Key.Alive || kv.Key.IsDeadBondedPet)
                        .Select(kv => kv.Key)
                        .ToArray())
                {
                    Hydra._EarthBreathAffectedMobs.Remove(m);

                    m.SolidHueOverride = -1;
                    m.CantWalk = false;
                }
            }
        }

        public void FireBreath()
        {
            //List<Mobile> buffer = new List<Mobile>();

            TimeSpan timeout = TimeSpan.FromSeconds(60);

            var fx = new FireWaveEffect(Location, Map, Direction, 20, 0, TimeSpan.FromMilliseconds(1))
            {
                EffectHandler = e =>
                {
                    if (e.ProcessIndex != 0)
                    {
                        return;
                    }

                    DateTime now = DateTime.UtcNow;

                    foreach (Mobile t in AcquireAllTargets(e.Source.Location, 0) /*.Not(buffer.Contains)*/)
                    {
                        //buffer.Add(t);

                        if (_WaterBreathAffectedMobs.Remove(t))
                        {
                            t.SendMessage(54, "The Hydra's fire breath evaporated the water covering your body!");
                        }

                        _FireBreathAffectedMobs.AddOrReplace(t, o => (o != null ? o.Value : now) + timeout);

                        if (t is BaseCreature)
                        {
                            t.Damage(200, this);
                        }
                        else
                        {
                            t.Damage(50, this);
                        }

                        t.SolidHueOverride = 38;
                    }
                },
                Callback = () =>
                {
                    //buffer.Clear();
                    CantWalk = false;
                }
            };

            fx.Send();
        }

        private class FireBreathInternalTimer : Timer
        {
            private readonly HydraMotM Hydra;

            public FireBreathInternalTimer(HydraMotM hydra)
                : base(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2))
            {
                Hydra = hydra;
            }

            protected override void OnTick()
            {
                if (Hydra._FireBreathAffectedMobs.Count <= 0)
                {
                    return;
                }

                DateTime now = DateTime.UtcNow;

                foreach (Mobile m in
                    Hydra._WaterBreathAffectedMobs.Where(
                        kv => kv.Value == null || kv.Value < now || !kv.Key.Alive || kv.Key.IsDeadBondedPet)
                        .Select(kv => kv.Key)
                        .ToArray())
                {
                    Hydra._FireBreathAffectedMobs.Remove(m);

                    m.SolidHueOverride = -1;
                }

                List<Mobile> toremove =
                    Hydra._FireBreathAffectedMobs.Where(
                        x =>
                            !x.Key.Alive || x.Key is BaseCreature && x.Key.IsDeadBondedPet)
                        .Select(x => x.Key)
                        .ToList();

                foreach (Mobile mobile in toremove.Where(mobile => Hydra._FireBreathAffectedMobs.ContainsKey(mobile)))
                {
                    mobile.SolidHueOverride = -1;
                    Hydra._FireBreathAffectedMobs.Remove(mobile);
                }

                foreach (Mobile m in
                    Hydra._FireBreathAffectedMobs.Where(kv => now < kv.Value).Select(kv => kv.Key))
                {
                    m.Damage(5, Hydra);
                }
            }
        }

        public void LightningBreath()
        {
            //List<Mobile> buffer = new List<Mobile>();

            var fx = new AirWaveEffect(Location, Map, Direction, 20, 0, TimeSpan.FromMilliseconds(1))
            {
                EffectHandler = e =>
                {
                    if (e.ProcessIndex != 0)
                    {
                        return;
                    }

                    foreach (Mobile t in AcquireAllTargets(e.Source.Location, 0) /*.Not(buffer.Contains)*/)
                    {
                        //buffer.Add(t);

                        Effects.SendBoltEffect(t); // Actual lightning! :)

                        int damage = 50;

                        if (_WaterBreathAffectedMobs.ContainsKey(t))
                        {
                            damage = 100;

                            t.SendMessage(54, "The water covering your body amplified the Hydra's lightning breath!");
                        }
                        else if (t is BaseCreature)
                        {
                            damage = 200;
                        }

                        t.Damage(damage, this);
                    }
                },
                Callback = () =>
                {
                    //buffer.Clear();
                    CantWalk = false;
                }
            };

            fx.Send();
        }

        public void WaterBreath()
        {
            //List<Mobile> buffer = new List<Mobile>();

            TimeSpan timeout = TimeSpan.FromSeconds(60);

            var fx = new WaterWaveEffect(Location, Map, Direction, 20, 0, TimeSpan.FromMilliseconds(1))
            {
                EffectHandler = e =>
                {
                    if (e.ProcessIndex != 0)
                    {
                        return;
                    }

                    DateTime now = DateTime.UtcNow;

                    foreach (Mobile t in AcquireAllTargets(e.Source.Location, 0) /*.Not(buffer.Contains)*/)
                    {
                        //buffer.Add(t);

                        if (_FireBreathAffectedMobs.Remove(t))
                        {
                            t.SendMessage(54, "The Hydra's water breath has doused the flames covering your body!");
                        }

                        _WaterBreathAffectedMobs.AddOrReplace(t, o => (o != null ? o.Value : now) + timeout);

                        if (t is BaseCreature)
                        {
                            t.Damage(200, this);
                        }
                        else
                        {
                            t.Damage(50, this);
                        }

                        t.SolidHueOverride = 1266;
                    }
                },
                Callback = () =>
                {
                    //buffer.Clear();
                    CantWalk = false;
                }
            };

            fx.Send();
        }

        private class WaterBreathInternalTimer : Timer
        {
            private readonly HydraMotM Hydra;

            public WaterBreathInternalTimer(HydraMotM hydra)
                : base(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1))
            {
                Hydra = hydra;
            }

            protected override void OnTick()
            {
                if (Hydra._WaterBreathAffectedMobs.Count <= 0)
                {
                    return;
                }

                DateTime now = DateTime.UtcNow;

                foreach (Mobile m in
                    Hydra._WaterBreathAffectedMobs.Where(
                        kv => kv.Value == null || kv.Value < now || !kv.Key.Alive || kv.Key.IsDeadBondedPet)
                        .Select(kv => kv.Key)
                        .ToArray())
                {
                    Hydra._WaterBreathAffectedMobs.Remove(m);

                    m.SolidHueOverride = -1;
                }
            }
        }

        public void AcidBreath()
        {
            //List<Mobile> buffer = new List<Mobile>();

            var fx = new PoisonWaveEffect(Location, Map, Direction, 20, 0, TimeSpan.FromMilliseconds(1))
            {
                EffectHandler = e =>
                {
                    if (e.ProcessIndex != 0)
                    {
                        return;
                    }

                    foreach (
                        PlayerMobile t in
                            AcquireAllTargets(e.Source.Location, 0) /*.Not(buffer.Contains)*/.OfType<PlayerMobile>()
                        )
                    {
                        //buffer.Add(t);

                        t.Damage(50, this);

                        bool message = false;

                        foreach (IWearableDurability iwd in
                            t.Items.Where(i => i.LootType != LootType.Blessed).OfType<IWearableDurability>())
                        {
                            iwd.HitPoints = Math.Max(1, iwd.HitPoints - 25);
                            message = true;
                        }

                        if (message)
                        {
                            t.PrivateOverheadMessage(MessageType.Label, 1100, true,
                                "Some of your equipment has been damaged!", t.NetState);
                        }
                    }
                },
                Callback = () =>
                {
                    //buffer.Clear();
                    CantWalk = false;
                }
            };

            fx.Send();
        }

        public List<Mobile> AcquireTargets(Point3D p, int range)
        {
            return
                p.GetMobilesInRange(Map, range)
                    .Where(
                        m =>
                            m != null && !m.Deleted && m.Alive && !m.Hidden &&
                            (m.Player ||
                             (m is BaseCreature && ((BaseCreature) m).GetMaster() is PlayerMobile && !m.IsDeadBondedPet)))
                    .ToList();
        }

        public List<Mobile> AcquireAllTargets(Point3D p, int range)
        {
            return
                p.GetMobilesInRange(Map, range)
                    .Where(
                        m =>
                            m != null && !m.Deleted && m.Alive &&
                            (m.Player ||
                             (m is BaseCreature && ((BaseCreature) m).GetMaster() is PlayerMobile && !m.IsDeadBondedPet)))
                    .ToList();
        }

        public List<Mobile> AcquireAllBloods(Point3D p, int range)
        {
            return
                p.GetMobilesInRange(Map, range)
                    .Where(m => m != null && !m.Deleted && m.Alive && m is BloodoftheHydra)
                    .ToList();
        }

        public void InvalidateHue()
        {
            foreach (Mobile mobile in _FireBreathAffectedMobs.Keys)
            {
                mobile.SolidHueOverride = -1;
            }

            foreach (Mobile mobile in _WaterBreathAffectedMobs.Keys)
            {
                mobile.SolidHueOverride = -1;
            }

            foreach (Mobile mobile in _EarthBreathAffectedMobs.Keys)
            {
                mobile.CantWalk = false;
                mobile.SolidHueOverride = -1;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            int version = writer.SetVersion(1);

            switch (version)
            {
                case 1:
                    writer.WriteList(_Bloods, writer.Write);
                    goto case 0;
                case 0:
                    writer.Write(_Moongate);
                    break;
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    reader.ReadList(reader.ReadMobile<BloodoftheHydra>, _Bloods);
                    goto case 0;
                case 0:
                    _Moongate = reader.ReadItem<ExtEventMoongate>();
                    break;
            }

            _FireBreathTimer = new FireBreathInternalTimer(this);
            _FireBreathTimer.Start();

            _WaterBreathTimer = new WaterBreathInternalTimer(this);
            _WaterBreathTimer.Start();

            _EarthBreathTimer = new EarthBreathInternalTimer(this);
            _EarthBreathTimer.Start();
        }
    }

    public sealed class HydraMotMNotifyGump : NotifyGump
    {
        public HydraMotMNotifyGump(PlayerMobile user, string html)
            : base(user, html)
        {}
    }
}