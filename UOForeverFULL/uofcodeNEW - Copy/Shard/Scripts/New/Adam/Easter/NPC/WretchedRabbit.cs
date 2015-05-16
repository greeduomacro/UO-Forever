
#region References

using System;
using System.Collections.Generic;
using System.Linq;
using Server.Engines.CannedEvil;
using Server.Engines.CustomTitles;
using Server.Items;
using Server.Network;
using Server.Scripts.New.Adam.Easter.Items;
using VitaNex;
using VitaNex.FX;
using VitaNex.Network;
using VitaNex.SuperGumps;
using VitaNex.Targets;

#endregion

namespace Server.Mobiles
{
    [CorpseName("a wretched corpse")]
    public class WretchedRabbit : BaseChampion
    {
        private static readonly SkillName[] _Skills = ((SkillName) 0).GetValues<SkillName>();

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsQuest { get; set; }

        private Dictionary<PlayerMobile, int> Damagers { get; set; }

        private DateTime _NextPoisonExplosion = DateTime.UtcNow;
        private DateTime _NextGreaterExplosion = DateTime.UtcNow;
        private DateTime _NextSummon = DateTime.UtcNow;
        private DateTime _NextMeteor = DateTime.UtcNow;

        private DateTime _NextCast = DateTime.UtcNow;

        public bool TogglePoisonExplosion { get; set; }
        public TimeSpan PoisonExplosionInterval { get { return TimeSpan.FromSeconds(Utility.RandomMinMax(10, 15)); } }

        public bool ToggleGreaterExplosion { get; set; }
        public TimeSpan GreaterExplosionInterval { get { return TimeSpan.FromSeconds(Utility.RandomMinMax(38, 58)); } }

        public bool ToggleSummon { get; set; }
        public TimeSpan SummonInterval { get { return TimeSpan.FromSeconds(Utility.RandomMinMax(30, 60)); } }

        public bool ToggleMeteor { get; set; }
        public TimeSpan MeteorInterval { get { return TimeSpan.FromSeconds(Utility.RandomMinMax(23, 25)); } }
        public int MeteorRange { get { return Utility.RandomMinMax(3, 5); } }

        private bool Transformed { get; set; }

        private List<Timer> CurrentSpell { get; set; }
        public TimeSpan CastInterval { get { return TimeSpan.FromSeconds(Utility.RandomMinMax(10, 15)); } }


        public override string DefaultName { get { return "a wretched rabbit"; } }
        public override bool CanRummageCorpses { get { return true; } }
        public override bool AlwaysMurderer { get { return true; } }
        public override bool CanDestroyObstacles { get { return true; } }
        public override bool CanFlee { get { return false; } }
        public override bool AutoDispel { get { return true; } }
        public override bool Unprovokable { get { return true; } }
        public override Poison PoisonImmune { get { return Poison.Lethal; } }

        public override bool HasBreath { get { return true; } } // fire breath enabled
        public override int BreathFireDamage { get { return 100; } }
        public override int BreathColdDamage { get { return 100; } }
        public override int TreasureMapLevel { get { return 4; } }

        public override int BreathEffectHue { get { return 1165; } }

        public override int DefaultBloodHue { get { return 1166; } } //Template
        public override int BloodHueTemplate { get { return 1166; } }


        public override ChampionSkullType SkullType { get { return ChampionSkullType.Special; } }

        [Constructable]
        public WretchedRabbit()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Fluffy of Caerbannog";
            Body = 205;
            Hue = 2042;
            SpecialTitle = "The Wretched Rabbit";
            TitleHue = 1174;

            BaseSoundID = 362;

            SetStr(1196, 1285);
            SetDex(90, 185);
            SetInt(706, 726);

            SetHits(25000, 25000);

            SetDamage(10, 15);

            foreach (SkillName skill in _Skills)
            {
                SetSkill(skill, 80.0, 140.0);
            }

            SpeechHue = YellHue = 34;

            VirtualArmor = 90;

            PackGold(3000, 3500);
            PackMagicItems(5, 5, 0.95, 0.95);
            PackMagicItems(5, 5, 0.80, 0.65);
            PackMagicItems(5, 5, 0.80, 0.65);
            PackMagicItems(6, 6, 0.80, 0.65);

            ToggleGreaterExplosion = true;
            TogglePoisonExplosion = true;
            ToggleMeteor = false;
            ToggleSummon = false;

            Transformed = false;

            IsQuest = true;

            Damagers = new Dictionary<PlayerMobile, int>();

            BardImmuneCustom = true;
            GuardImmune = true;
            Tamable = false;
        }

        public WretchedRabbit(Serial serial)
            : base(serial)
        {}

        public override bool OnBeforeDeath()
        {
            if (!Transformed)
            {
                _NextCast = DateTime.UtcNow + TimeSpan.FromSeconds(10);
                if (CurrentSpell != null)
                {
                    foreach (Timer timer in CurrentSpell)
                    {
                        timer.Stop();
                    }
                }
                Frozen = true;
                Transformed = true;
                Yell("THIS ISN'T EVEN MY FINAL FORM!");
                ToggleMeteor = true;
                ToggleSummon = true;
                Hits = HitsMax;
                FlamestrikeSpiral();
                return false;
            }
            if (IsQuest)
            {
                NetState.Instances.Where(ns => ns != null && ns.Mobile is PlayerMobile)
                    .ForEach(
                        ns =>
                            ns.Mobile.SendNotification(
                                "The Wretched Rabbit is no more! Never again will he threaten Easter.  Rejoice citizens of Sosaria!!!",
                                true, 1.0, 10.0));
                GiveItems();
            }
            return base.OnBeforeDeath();
        }

        public override void OnDeath(Container c)
        {
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

            c.DropItem(new TitleScroll("The Bunnyslayer"));

            if (IsQuest)
                c.DropItem(new HueScroll(2049));

            base.OnDeath(c);
        }

        public void GiveItems()
        {
            if (Damagers != null)
            {
                foreach (KeyValuePair<PlayerMobile, int> kvp in Damagers)
                {
                    kvp.Key.BankBox.DropItem(new HueScroll(1168));
                    kvp.Key.SendMessage(
                        "You were awarded a title hue scroll for your contributions towards killing the Wretched Rabbit! The title hue scroll has been placed in your bank box.");
                }
                List<PlayerMobile> topTen = (from entry in Damagers
                    orderby entry.Value descending
                    select entry.Key)
                    .Take(10)
                    .ToList();
                if (topTen.Count >= 1 && topTen[0] != null)
                {
                    topTen[0].BankBox.DropItem(new TitleScroll("The Wretched"));
                    topTen[0].SendMessage(
                        "You were awarded a title scroll: The Wretched for doing the most damage to the Wretched Rabbit!!!!");
                }
                if (topTen.Count >= 2 && topTen[1] != null)
                {
                    topTen[1].BankBox.DropItem(new TitleScroll("The Despicable"));
                    topTen[1].SendMessage(
                        "You were awarded a title scroll: The Despicable for doing the second most damage to the Wretched Rabbit!!!!");
                }
                if (topTen.Count >= 3 && topTen[2] != null)
                {
                    topTen[2].BankBox.DropItem(new TitleScroll("The Pitiful"));
                    topTen[2].SendMessage(
                        "You were awarded a title scroll: The Pitiful for doing the third most damage to the Wretched Rabbit!!!!");
                }
                if (topTen.Count >= 4 && topTen[3] != null)
                {
                    topTen[3].BankBox.DropItem(new HueScroll(1266));
                    topTen[3].SendMessage(
                        "You were awarded a title hue scroll for doing the fourth most damage to the Wretched Rabbit!");
                }
                for (int i = 4; i <= 9; i++)
                {
                    if (topTen.Count >= i && topTen[i] != null)
                    {
                        topTen[i].BankBox.DropItem(new EasterBracelet(1162));
                        topTen[i].SendMessage(
                            "You were awarded an: easter bracelet for coming in the top 10 of overall damage done to the Wretched Rabbit.");
                    }
                }
            }
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            PlayerMobile pm = null;
            int amountmodified = amount;
            if (IsQuest)
            {
                if (Damagers != null)
                {
                    if (from is PlayerMobile)
                    {
                        pm = from as PlayerMobile;
                    }
                    else if (from is BaseCreature && ((BaseCreature) from).GetMaster() is PlayerMobile)
                    {
                        pm = ((BaseCreature) from).GetMaster() as PlayerMobile;
                        amountmodified = (int) Math.Floor(amount * 0.5); //pets only add 50% of their damage
                    }
                    if (pm != null && Damagers.ContainsKey(pm))
                    {
                        Damagers[pm] += amountmodified;
                    }
                    else if (pm != null)
                    {
                        Damagers.Add(pm, amountmodified);
                    }
                }
            }
            base.OnDamage(amount, from, willKill);
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

                if (Hits < HitsMax * .75)
                {
                    ToggleSummon = true;
                }
                if (ToggleMeteor && now > _NextMeteor && Transformed)
                {
                    _NextMeteor = now + MeteorInterval;

                    Yell("FEEL THE WRATH OF THE ONE TRUE EASTER BUNNY");

                    Animate();

                    CurrentSpell.Clear();
                    CurrentSpell.Add(Timer.DelayCall(TimeSpan.FromSeconds(3.0), Meteor_Callback));
                    CurrentSpell.Add(Timer.DelayCall(TimeSpan.FromSeconds(4.0), Meteor_Callback));
                }

                else if (ToggleSummon && now > _NextSummon)
                {
                    _NextSummon = now + SummonInterval;

                    Yell(Utility.RandomBool() ? "Fluff Cand Choc Suga" : "Aid me my fluffy brethren!");

                    Animate();

                    CurrentSpell.Clear();
                    CurrentSpell.Add(Timer.DelayCall(TimeSpan.FromSeconds(3.0), Summon_Callback));
                }

                else if (ToggleGreaterExplosion && now > _NextGreaterExplosion)
                {
                    _NextGreaterExplosion = now + GreaterExplosionInterval;

                    Yell(Utility.RandomBool() ? "I FART IN YOUR GENERAL DIRECTION" : "ISN'T EASTER FUN?");

                    Animate();

                    CurrentSpell.Clear();
                    CurrentSpell.Add(Timer.DelayCall(TimeSpan.FromSeconds(3.0), GreaterExplosion_Callback));
                }

                else if (TogglePoisonExplosion && now > _NextPoisonExplosion)
                {
                    _NextPoisonExplosion = now + PoisonExplosionInterval;

                    Yell("DIABETES!");

                    Animate();

                    CurrentSpell.Clear();
                    CurrentSpell.Add(Timer.DelayCall(TimeSpan.FromSeconds(3.5), PoisonExplosion_Callback));
                }
            }
        }

        protected void PoisonExplosion_Callback()
        {
            if (Deleted || Map == null || Map == Map.Internal || Location == Point3D.Zero)
            {
                return;
            }

            new EnergyExplodeEffect(
                Location,
                Map,
                10,
                0,
                TimeSpan.FromMilliseconds(Math.Max(0, 1000 - ((10 - 1) * 100))),
                e =>
                {
                    foreach (Mobile m in AcquireTargets(Location, 10))
                    {
                        m.ApplyPoison(this, Poison.Lethal);
                    }
                }).Send();
        }

        protected void GreaterExplosion_Callback()
        {
            var queue = new EffectQueue();
            queue.Deferred = false;

            List<Mobile> rangelist = AcquireTargets(Location, 15);
            int index = Utility.Random(rangelist.Count);
            if (index + 1 > rangelist.Count)
            {
                return;
            }

            Point3D endpoint = rangelist[index].Location;
            Point3D[] line = Location.GetLine3D(endpoint, Map);

            int n = 0;
            foreach (Point3D p in line)
            {
                n += 20;
                Point3D p1 = p;
                queue.Add(
                    new EffectInfo(
                        p,
                        Map,
                        14089,
                        1165,
                        10,
                        30,
                        EffectRender.Normal,
                        TimeSpan.FromMilliseconds(n),
                        () =>
                        {
                            foreach (Mobile player in AcquireTargets(p1, 0))
                            {
                                player.Damage(50, this);
                            }
                        }));
            }

            queue.Callback = () =>
            {
                BaseExplodeEffect e = ExplodeFX.Energy.CreateInstance(
                    endpoint, Map, 5, 0, TimeSpan.FromMilliseconds(1000 - ((10) * 100)), null, () =>
                    {
                        foreach (Mobile player in AcquireTargets(endpoint, 5))
                        {
                            player.Damage(25, this);
                        }
                    });
                e.Send();
            };


            queue.Process();
        }

        protected void Summon_Callback()
        {
            if (Deleted || Map == null || Map == Map.Internal || Location == Point3D.Zero)
            {
                return;
            }

            BaseSpecialEffect e = SpecialFX.FirePentagram.CreateInstance(
                Location, Map, 5, 0, TimeSpan.FromMilliseconds(1000 - ((10 - 1) * 100)));
            e.Send();

            var mobile = new PowerfulDemon{ Name = "a wretched minion", BodyValue = 205, Hue = 1266};
            mobile.MoveToWorld(Location, Map);
        }

        public void Meteor_Callback()
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
                    18406,
                    0,
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
                    136,
                    30,
                    EffectRender.Multiply,
                    null,
                    () => { }));
            queue.Process();

            Timer.DelayCall(TimeSpan.FromSeconds(2.0), MeteorImpact_Callback, point);
        }

        public void MeteorImpact_Callback(Point3D impactloc)
        {
            var queue = new EffectQueue();
            queue.Deferred = true;

            for (int i = 0; i < 10; i++)
            {
                BaseExplodeEffect e = ExplodeFX.Energy.CreateInstance(
                    impactloc, Map, MeteorRange, 0, TimeSpan.FromMilliseconds(1000 - ((10) * 100)), null, () =>
                    {
                        foreach (Mobile player in AcquireTargets(impactloc, MeteorRange))
                        {
                            player.Damage(50, this);
                        }
                    });
                e.Send();
            }
            queue.Process();
        }

        public List<Mobile> AcquireTargets(Point3D p, int range)
        {
            return
                p.GetMobilesInRange(Map, range)
                    .Where(
                        m =>
                            m != null && !m.Deleted && m.AccessLevel == AccessLevel.Player && m != this && m.Alive && CanBeHarmful(m, false, true) && (m.Party == null || m.Party != Party) &&
                            (m.Player || (m is BaseCreature && ((BaseCreature)m).GetMaster() is PlayerMobile)))
                    .ToList();
        }

        public void FlamestrikeSpiral()
        {
            var queue = new EffectQueue();
            queue.Deferred = false;

            var points = new List<Point3D>();
            double d;
            double r = 1;
            int newx;
            int newy;
            points.Add(Location);
            //calculate spiral vector
            for (d = 0; d < 4 * Math.PI; d += 0.01)
            {
                newx = (int) Math.Floor(X + (Math.Sin(d) * d) * r);
                newy =
                    (int)
                        Math.Floor(Y + (Math.Sin(d + (Math.PI / 2)) * (d + (Math.PI / 2))) * r);
                var to = new Point3D(newx, newy, Z);
                if (!points.Contains(to))
                {
                    points.Add(to);
                }
            }
            int n = 0;
            //Build the queue based on the points in the line.
            foreach (Point3D p in points)
            {
                n += 20;
                Point3D p1 = p;
                queue.Add(new EffectInfo(p, Map, 3191, 1165, 10, 30, EffectRender.Normal, TimeSpan.FromMilliseconds(n),
                    () =>
                    {
                        foreach (Mobile m in AcquireTargets(p1, 0))
                        {
                            m.Damage(25, this);
                        }
                    }
                    ));
            }
            n += 400; //used to offset when the spiral reverses so it doesn't overlap
            foreach (Point3D p in points.AsEnumerable().Reverse())
            {
                n += 20;
                Point3D p1 = p;
                queue.Add(new EffectInfo(p, Map, 3191, 1165, 10, 30, EffectRender.Normal, TimeSpan.FromMilliseconds(n),
                    () =>
                    {
                        foreach (Mobile m in AcquireTargets(p1, 0))
                        {
                            m.Damage(25, this);
                        }
                    }
                    ));
            }
            queue.Process();

            Timer.DelayCall(TimeSpan.FromSeconds(6.0), Death_Callback);
        }

        public void Death_Callback()
        {
            BaseSpecialEffect e = SpecialFX.FirePentagram.CreateInstance(
                Location, Map, 10, 0, TimeSpan.FromMilliseconds(1000 - ((10 - 1) * 100)));
            e.Send();

            Name = "Fluffy of Caerbannog";
            Body = 221;
            Hue = 2042;
            HitsMaxSeed = 50000;
            Hits = 50000;
            SpecialTitle = "The Wretched Rabbit";
            TitleHue = 1174;
            Frozen = false;
        }

        public void Animate()
        {
            Frozen = true;
            if (Body.IsHuman)
            {
                Animate(263, 7, 1, true, true, 1);
            }
            else if (Body.IsMonster)
            {
                Animate(12, 7, 1, true, true, 1);
            }
            Frozen = false;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int) 0);

            writer.Write(TogglePoisonExplosion);
            writer.Write(ToggleGreaterExplosion);
            writer.Write(ToggleSummon);
            writer.Write(ToggleMeteor);
            writer.Write(Transformed);
            writer.Write(IsQuest);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            TogglePoisonExplosion = reader.ReadBool();
            ToggleGreaterExplosion = reader.ReadBool();
            ToggleSummon = reader.ReadBool();
            ToggleMeteor = reader.ReadBool();
            Transformed = reader.ReadBool();
            IsQuest = reader.ReadBool();
        }
    }
}