
#region References

using System;
using System.Collections.Generic;
using System.Linq;
using Server.Engines.CannedEvil;
using Server.Engines.CustomTitles;
using Server.Items;
using Server.Network;
using VitaNex;
using VitaNex.FX;
using VitaNex.Network;
using VitaNex.SuperGumps;
using VitaNex.Targets;

#endregion

namespace Server.Mobiles
{
    [CorpseName("a desecrated corpse")]
    public class CrippledKing : BaseChampion
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


        public override string DefaultName { get { return "a crippled king"; } }
        public override bool CanRummageCorpses { get { return true; } }
        public override bool AlwaysMurderer { get { return true; } }
        public override bool CanDestroyObstacles { get { return true; } }
        public override bool CanFlee { get { return false; } }
        public override bool AutoDispel { get { return true; } }
        public override bool Unprovokable { get { return true; } }
        public override Poison PoisonImmune { get { return Poison.Lethal; } }


        public override ChampionSkullType SkullType { get { return ChampionSkullType.Special; } }

        [Constructable]
        public CrippledKing()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Xyrrath Pactmaker";
            Body = 400;
            Hue = 1162;
            SpecialTitle = "The Crippled King";
            TitleHue = 1174;

            BaseSoundID = 362;

            SetStr(1196, 1285);
            SetDex(90, 185);
            SetInt(706, 726);

            SetHits(5000, 12500);

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

            IsQuest = false;

            Damagers = new Dictionary<PlayerMobile, int>();

            var helm = new RavenHelm();
            helm.Name = "ancient crown";
            helm.Hue = 0;
            helm.Identified = true;
            AddItem(Immovable(helm));

            var arms = new RoyalPlateArms();
            arms.Name = "ancient royal plate arms";
            arms.Hue = 0;
            arms.Identified = true;
            AddItem(Immovable(arms));

            var gloves = new RoyalPlateGloves();
            gloves.Name = "ancient royal plate gloves";
            gloves.Hue = 0;
            gloves.Identified = true;
            AddItem(Immovable(gloves));

            var tunic = new RoyalPlateChest();
            tunic.Name = "ancient royal plate chest";
            tunic.Hue = 0;
            tunic.Identified = true;
            AddItem(Immovable(tunic));

            var legs = new RoyalPlateLegs();
            legs.Name = "ancient royal plate legs";
            legs.Hue = 0;
            legs.Identified = true;
            AddItem(Immovable(legs));

            var gorget = new RoyalPlateGorget();
            gorget.Name = "ancient royal gorget";
            gorget.Hue = 0;
            gorget.Identified = true;
            AddItem(Immovable(gorget));

            var boots = new RoyalPlateBoots();
            boots.Name = "ancient royal boots";
            boots.Hue = 0;
            boots.Identified = true;
            AddItem(Immovable(boots));

            var cloak = new RoyalCloak();
            cloak.Name = "ancient royal cloak";
            cloak.Hue = 0;
            cloak.Identified = true;
            AddItem(Immovable(cloak));

            var spellbook = new FullSpellbook();
            spellbook.Name = "abyssal spellbook";
            spellbook.Hue = 2065;
            spellbook.Movable = false;
            AddItem(Immovable(spellbook));
        }

        public CrippledKing(Serial serial)
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
                Yell("NO, I CANNOT BE DEFEATED LIKE THIS!");
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
                                "The Crippled King has been slain! Rejoice, for the millenia old curse has been lifted from the Marble Keep!!!!",
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

            c.DropItem(new TitleScroll("The Kingslayer"));

            if (0.01 > Utility.RandomDouble())
            {
                c.DropItem(new FullSpellbook() { Name = "abyssal spellbook", LootType = LootType.Regular, Hue = 2065 });
            }

            if (0.01 > Utility.RandomDouble())
            {
                c.DropItem(new HueScroll(2049));
            }
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
                    kvp.Key.BankBox.DropItem(new TitleScroll("The Purifier"));
                    kvp.Key.SendMessage(
                        "You were awarded a title scroll for your contributions towards killing the Crippled King! The title scroll: The Purifier has been placed in your bankbox.");
                }
                List<PlayerMobile> topTen = (from entry in Damagers
                    orderby entry.Value descending
                    select entry.Key)
                    .Take(10)
                    .ToList();
                if (topTen.Count >= 1 && topTen[0] != null)
                {
                    topTen[0].BankBox.DropItem(new EtherealBoura()
                    {
                        Name = "The Crippled King's stone Boura",
                        Hue = 1072
                    });
                    topTen[0].SendMessage(
                        "You were awarded: The Crippled King's stone Boura for doing the most damage to the crippled king!!!!");
                }
                if (topTen.Count >= 2 && topTen[1] != null)
                {
                    topTen[1].BankBox.DropItem(new FullSpellbook()
                    {
                        Name = "The Crippled King's abyssal spellbook",
                        Hue = 2065,
                        LootType = LootType.Blessed
                    });
                    topTen[1].SendMessage(
                        "You were awarded: The Crippled King's abyssal spellbook for doing the second most damage to the crippled king!!!!");
                }
                if (topTen.Count >= 3 && topTen[2] != null)
                {
                    topTen[2].BankBox.DropItem(new HalloweenMinotaurYardAddonDeed());
                    topTen[2].SendMessage(
                        "You were awarded a: Minotaur Statue Deed for doing the third most damage to the crippled king!!!!");
                }
                if (topTen.Count >= 4 && topTen[3] != null)
                {
                    topTen[3].BankBox.DropItem(new HalloweenBloodFountainAddonDeed());
                    topTen[3].SendMessage(
                        "You were awarded a: Blood Pond Deed for doing the fourth most damage to the crippled king!!!!");
                }
                for (int i = 4; i <= 9; i++)
                {
                    if (topTen.Count >= i && topTen[i] != null)
                    {
                        topTen[i].BankBox.DropItem(new SpiderWallCarvingAddonDeed());
                        topTen[i].SendMessage(
                            "You were awarded a: commemorative wall carving for coming in the top 10 of overall damage done to the crippled king!");
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

                    Yell(Utility.RandomBool() ? "Corp Por Kal Des Ylem" : "The heavens betray you! METEOR!!!");

                    Animate();

                    CurrentSpell.Clear();
                    CurrentSpell.Add(Timer.DelayCall(TimeSpan.FromSeconds(3.0), Meteor_Callback));
                    CurrentSpell.Add(Timer.DelayCall(TimeSpan.FromSeconds(4.0), Meteor_Callback));
                }

                else if (ToggleSummon && now > _NextSummon)
                {
                    _NextSummon = now + SummonInterval;

                    Yell(Utility.RandomBool() ? "Kal Xen Bal Beh" : "From the darkest abyss, I summon thee!");

                    Animate();

                    CurrentSpell.Clear();
                    CurrentSpell.Add(Timer.DelayCall(TimeSpan.FromSeconds(3.0), Summon_Callback));
                }

                else if (ToggleGreaterExplosion && now > _NextGreaterExplosion)
                {
                    _NextGreaterExplosion = now + GreaterExplosionInterval;

                    Yell(Utility.RandomBool() ? "Kal Vas Ort Flam" : "MAY YOU BE REDUCED TO ASHES");

                    Animate();

                    CurrentSpell.Clear();
                    CurrentSpell.Add(Timer.DelayCall(TimeSpan.FromSeconds(3.0), GreaterExplosion_Callback));
                }
                else if (TogglePoisonExplosion && now > _NextPoisonExplosion)
                {
                    _NextPoisonExplosion = now + PoisonExplosionInterval;

                    Yell(Utility.RandomBool() ? "In Vas Ort Nox" : "Breathe deep the underworld..");

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

            new PoisonExplodeEffect(
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
                        0,
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
                BaseExplodeEffect e = ExplodeFX.Fire.CreateInstance(
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

            var mobile = new PowerfulDemon();
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
                    13920,
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
                    0,
                    30,
                    EffectRender.Normal,
                    null,
                    () => { }));
            queue.Process();

            Timer.DelayCall(TimeSpan.FromSeconds(2.0), MeteorImpact_Callback, point);
        }

        public void MeteorImpact_Callback(Point3D impactloc)
        {
            var queue = new EffectQueue();
            queue.Deferred = true;

                BaseExplodeEffect e = ExplodeFX.Fire.CreateInstance(
                    impactloc, Map, MeteorRange, 0, TimeSpan.FromMilliseconds(1000 - ((10) * 100)), null, () =>
                    {
                        foreach (Mobile player in AcquireTargets(impactloc, MeteorRange))
                        {
                            player.Damage(50, this);
                        }
                    });
                e.Send();
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
                queue.Add(new EffectInfo(p, Map, 14089, 0, 10, 30, EffectRender.Normal, TimeSpan.FromMilliseconds(n),
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
                queue.Add(new EffectInfo(p, Map, 14089, 0, 10, 30, EffectRender.Normal, TimeSpan.FromMilliseconds(n),
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

            Name = "Xyrrath Chaosborn";
            BodyValue = 1071;
            Hue = 0;
            SpecialTitle = "Great Daemon of Chaos";
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