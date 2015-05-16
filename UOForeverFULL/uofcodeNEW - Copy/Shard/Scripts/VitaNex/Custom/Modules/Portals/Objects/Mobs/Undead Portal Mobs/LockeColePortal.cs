#region References

using System;
using System.Collections.Generic;
using System.Linq;
using Server.Engines.CannedEvil;
using Server.Engines.Craft;
using Server.Engines.CustomTitles;
using Server.Engines.LegendaryCrafting;
using Server.Items;
using VitaNex.FX;
using VitaNex.Network;

#endregion

namespace Server.Mobiles
{
    [CorpseName("a desiccated corpse")]
    public class LockeColePortal : BaseChampion
    {
        private static readonly SkillName[] _Skills = ((SkillName) 0).GetValues<SkillName>();

        private DateTime _NextShadowStep = DateTime.UtcNow;
        private DateTime _NextUltima = DateTime.UtcNow;
        private DateTime _NextMirrorImage = DateTime.UtcNow;

        //ShadowStep
        private int _ShadowStepDamage = 20;
        private int _ShadowStepKill = 20;

        [CommandProperty(AccessLevel.GameMaster)]
        public int ShadowStepDamage { get { return _ShadowStepDamage; } set { _ShadowStepDamage = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int ShadowStepKill { get { return _ShadowStepKill; } set { _ShadowStepKill = value; } }

        //Mirror Image
        private int _MirrorImageHealth = 2000;

        [CommandProperty(AccessLevel.GameMaster)]
        public int MirrorImageHealth { get { return _MirrorImageHealth; } set { _MirrorImageHealth = value; } }

        private List<LockeColeMirrorImagePortal> MirrorImages;

        //Ultima
        private int _UltimaPetDamage = 250;

        [CommandProperty(AccessLevel.GameMaster)]
        public int UltimaPetDamage { get { return _UltimaPetDamage; } set { _UltimaPetDamage = value; } }

        private int _UltimaDamage = 50;

        [CommandProperty(AccessLevel.GameMaster)]
        public int UltimaDamage { get { return _UltimaDamage; } set { _UltimaDamage = value; } }

        public override bool BleedImmune { get { return true; } }
        public override int TreasureMapLevel { get { return 5; } }

        private DateTime _NextCast = DateTime.UtcNow;

        public TimeSpan ShadowStepInterval { get { return TimeSpan.FromSeconds(Utility.RandomMinMax(35, 45)); } }

        public TimeSpan UltimaInterval { get { return TimeSpan.FromSeconds(Utility.RandomMinMax(11, 22)); } }

        public TimeSpan MirrorImageInterval { get { return TimeSpan.FromSeconds(Utility.RandomMinMax(60, 90)); } }


        private List<Timer> CurrentSpell { get; set; }
        public TimeSpan CastInterval { get { return TimeSpan.FromSeconds(Utility.RandomMinMax(10, 15)); } }

        public override bool CanRummageCorpses { get { return true; } }
        public override bool AlwaysMurderer { get { return true; } }
        public override bool CanDestroyObstacles { get { return true; } }
        public override bool CanFlee { get { return true; } }
        public override bool AutoDispel { get { return true; } }
        public override bool Unprovokable { get { return true; } }
        public override Poison PoisonImmune { get { return Poison.Lethal; } }
        public override bool BardImmune { get { return true; } }

        public override bool NoGoodies { get { return true; } }

        public override int PSToGive { get { return Utility.RandomDouble() >= 0.5 ? 2 : 1; } }

        public override int FactionPSToGive { get { return 0; } }


        public override ChampionSkullType SkullType { get { return ChampionSkullType.Special; } }

        [Constructable]
        public LockeColePortal()
            : base(AIType.AI_Arcade, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Locke Cole";
            Body = 400;
            Hue = 1162;
            SpecialTitle = "Undead Treasure Hunter";
            TitleHue = 1174;

            BaseSoundID = 362;

            SetStr(500);
            SetDex(400);
            SetInt(100);

            SetHits(30000);
            SetMana(5000);

            SetDamage(5, 8);

            foreach (SkillName skill in _Skills)
            {
                SetSkill(skill, 80.0, 140.0);
            }

            MirrorImages = new List<LockeColeMirrorImagePortal>();
            SpeechHue = YellHue = 34;

            VirtualArmor = 90;

            var helm = new SkullCap();
            helm.Name = "Locke's Bandana";
            helm.Hue = 4;
            helm.Identified = true;
            AddItem(Immovable(helm));

            var arms = new JinBaori();
            arms.Name = "Locke's Jacket";
            arms.Hue = 4;
            arms.Identified = true;
            AddItem(Immovable(arms));

            var gloves = new LeatherGloves();
            gloves.Name = "Locke's Gloves";
            gloves.Hue = 4;
            gloves.Identified = true;
            AddItem(Immovable(gloves));

            var tunic = new Shirt();
            tunic.Name = "Locke's Shirt";
            tunic.Hue = 0;
            tunic.Identified = true;
            AddItem(Immovable(tunic));

            var legs = new LeatherNinjaPants();
            legs.Name = "Locke's Pants";
            legs.Hue = 4;
            legs.Identified = true;
            AddItem(Immovable(legs));

            var boots = new ElvenBoots();
            boots.Name = "Locke's Boots";
            boots.Hue = 1175;
            boots.Identified = true;
            AddItem(Immovable(boots));

            var spellbook = new Dagger();
            spellbook.Name = "Thief Dagger";
            spellbook.Hue = 0;
            spellbook.Movable = false;
            AddItem(Immovable(spellbook));
        }

        public LockeColePortal(Serial serial)
            : base(serial)
        {}

        public override bool OnBeforeDeath()
        {
            if (0.5 > Utility.RandomDouble())
            {
                PackItem(new LegendaryHammer());
            }
            PackItem(new Gold(5000));
            PackItem(new Gold(5000));

            return base.OnBeforeDeath();
        }

        public override void OnDeath(Container c)
        {
            c.DropItem(new Platinum {Amount = 50});
            c.DropItem(new GargoyleRune());
            if (Utility.RandomDouble() < 0.5)
            {
                c.DropItem(new GargoyleRune());
            }
            if (Utility.RandomDouble() < 0.1)
            {
                c.DropItem(new GargoyleRune());
            }

            var scroll = new SkillScroll();
            scroll.Randomize();
            c.DropItem(scroll);

            scroll = new SkillScroll();
            scroll.Randomize();
            c.DropItem(scroll);

            if (Utility.RandomDouble() < 0.01)
            {
                c.DropItem(new MaddeninghorrorStatue {Hue = 1172});
            }

            base.OnDeath(c);
        }

        public override void GiveSpecialItems(List<Mobile> eligibleMobs, List<double> eligibleMobScores,
            double totalScores)
        {
            double currentTestValue = 0.0;
            double roll = Utility.RandomDouble() * totalScores;
            if (0.05 > Utility.RandomDouble())
            {
                for (int i = 0; i < eligibleMobScores.Count; i++)
                {
                    currentTestValue += eligibleMobScores[i];

                    if (roll > currentTestValue)
                    {
                        continue;
                    }

                    var recipescroll = new RecipeScroll(Recipe.GetRandomRecipe());
                    if (eligibleMobs[i] is PlayerMobile && eligibleMobs[i].Backpack != null)
                    {
                        eligibleMobs[i].Backpack.DropItem(recipescroll);
                        eligibleMobs[i].SendMessage(54, "You have received a recipe scroll!");
                        break;
                    }
                }
            }

            if (0.1 > Utility.RandomDouble())
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
                        eligibleMobs[i].Backpack.DropItem(new TitleScroll("Locked and Loaded"));
                        eligibleMobs[i].SendMessage(54, "You have received a title scroll.");
                        break;
                    }
                }
            }

            if (0.05 > Utility.RandomDouble())
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
                        eligibleMobs[i].Backpack.DropItem(new LeatherGloves()
                        {
                            Name = "Locke's Gloves",
                            Hue = 4,
                            Identified = true,
                            Slayer = SlayerName.Silver,
                            LootType = LootType.Blessed
                        });
                        eligibleMobs[i].SendMessage(54, "You have received a pair of Locke Cole's gloves!");
                        return;
                    }
                }
            }

        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 3);
            AddLoot(LootPack.MedScrolls, 2);

            if (0.009 > Utility.RandomDouble()) // 2 percent - multipy number x 100 to get percent
            {
                switch (Utility.Random(2))
                {
                        // rolls and number it gives it a case. if the number is , say , 3 it will pack that item
                    case 0:
                        PackItem(new BlackDyeTub());
                        break;
                    case 1:
                        PackItem(new Sandals(1175));
                        break;
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

                if (now > _NextShadowStep && CurrentSpell.Count == 0)
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
                else if (now > _NextUltima && CurrentSpell.Count == 0)
                {
                    _NextUltima = now + UltimaInterval;

                    CurrentSpell.Clear();

                    CantWalk = true;
                    Combatant = null;
                    Say("*Begins to cast Ultima*");
                    AnimateSpell();
                    CurrentSpell.Add(Timer.DelayCall(TimeSpan.FromSeconds(2), Ultima_Callback));
                }
                else if (now > _NextMirrorImage && CurrentSpell.Count == 0 && MirrorImages.Count < 4)
                {
                    _NextMirrorImage = now + MirrorImageInterval;

                    CurrentSpell.Clear();

                    CantWalk = true;
                    Combatant = null;
                    Say("*Begins to cast Mirror Image*");
                    AnimateSpell();
                    CurrentSpell.Add(Timer.DelayCall(TimeSpan.FromSeconds(2), MirrorImage_Callback));
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
                        Say("*Stabs you in the back.*");
                        Animate();
                    });
                    if (mobile1.Hits <= ShadowStepKill)
                    {
                        mobile1.Damage(mobile1.Hits + 1, this);
                    }
                    else
                    {
                        mobile1.Hits = ShadowStepDamage;
                        mobile1.ApplyPoison(this, Poison.Lethal);
                    }
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
                    CurrentSpell.Clear();
                });
            });
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

        public void MirrorImage_Callback()
        {
            HideEffect();
            Hidden = true;
            CantWalk = true;
            Blessed = true;
            var mob1 = new LockeColeMirrorImagePortal();
            var mob2 = new LockeColeMirrorImagePortal();
            var mob3 = new LockeColeMirrorImagePortal();
            var mob4 = new LockeColeMirrorImagePortal();

            mob1.Hidden = true;
            mob1.CantWalk = true;
            mob1.Blessed = true;
            mob2.Hidden = true;
            mob2.CantWalk = true;
            mob2.Blessed = true;
            mob3.Hidden = true;
            mob3.CantWalk = true;
            mob3.Blessed = true;
            mob4.Hidden = true;
            mob4.CantWalk = true;
            mob4.Blessed = true;

            MirrorImages.Add(mob1);
            MirrorImages.Add(mob2);
            MirrorImages.Add(mob3);
            MirrorImages.Add(mob4);

            SetHits(Hits);

            Timer.DelayCall(TimeSpan.FromSeconds(2), () =>
            {
                mob1.MoveToWorld(Location, Map);
                mob2.MoveToWorld(Location, Map);
                mob3.MoveToWorld(Location, Map);
                mob4.MoveToWorld(Location, Map);

                mob1.SetHits(MirrorImageHealth);
                mob2.SetHits(MirrorImageHealth);
                mob3.SetHits(MirrorImageHealth);
                mob4.SetHits(MirrorImageHealth);

                HideEffect();
                mob1.HideEffect();
                mob2.HideEffect();
                mob3.HideEffect();
                mob4.HideEffect();

                Hidden = false;
                CantWalk = false;
                Blessed = false;

                mob1.Hidden = false;
                mob1.CantWalk = false;
                mob1.Blessed = false;
                mob2.Hidden = false;
                mob2.CantWalk = false;
                mob2.Blessed = false;
                mob3.Hidden = false;
                mob3.CantWalk = false;
                mob3.Blessed = false;
                mob4.Hidden = false;
                mob4.CantWalk = false;
                mob4.Blessed = false;

                CurrentSpell.Clear();
            });
        }

        public void Ultima_Callback()
        {
            var startloc = new Point3D(X, Y, 100);
            var point = new Point3D(Location);

            var queue = new EffectQueue();
            queue.Deferred = false;

            queue.Add(
                new MovingEffectInfo(
                    startloc,
                    point,
                    Map,
                    13920,
                    4,
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
                impactloc, Map, 8, 0, TimeSpan.FromMilliseconds(1000 - ((10) * 100)), null, () =>
                {
                    foreach (Mobile mobile in AcquireAllTargets(impactloc, 8))
                    {
                        if (mobile is PlayerMobile)
                        {
                            mobile.Damage(UltimaDamage, this);
                        }
                        else
                        {
                            mobile.Damage(450, this);
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

            int version = writer.SetVersion(1);

            switch (version)
            {
                case 1:
                {
                    writer.Write(MirrorImages.Count);

                    if (MirrorImages.Count > 0)
                    {
                        foreach (Mobile mob in MirrorImages)
                        {
                            writer.Write(mob);
                        }
                    }
                }
                    goto case 0;
                case 0:
                {}
                    break;
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            MirrorImages = new List<LockeColeMirrorImagePortal>();
            switch (version)
            {
                case 1:
                {
                    var count = reader.ReadInt();

                    if (count > 0)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            LockeColeMirrorImagePortal mi = reader.ReadMobile<LockeColeMirrorImagePortal>();
                            if (mi != null)
                            {
                                MirrorImages.Add(mi);
                            }
                        }
                    }
                }
                    goto case 0;
                case 0:
                {}
                    break;
            }
        }
    }
}