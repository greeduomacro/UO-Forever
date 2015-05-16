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
    public class LockeColeMirrorImage : BaseCreature
    {
        private static readonly SkillName[] _Skills = ((SkillName) 0).GetValues<SkillName>();

        private DateTime _NextShadowStep = DateTime.UtcNow;
        private DateTime _NextUltima = DateTime.UtcNow;

        private DateTime _NextCast = DateTime.UtcNow;

        public TimeSpan ShadowStepInterval { get { return TimeSpan.FromSeconds(Utility.RandomMinMax(35, 45)); } }

        public TimeSpan UltimaInterval { get { return TimeSpan.FromSeconds(Utility.RandomMinMax(11, 22)); } }

        private List<Timer> CurrentSpell { get; set; }
        public TimeSpan CastInterval { get { return TimeSpan.FromSeconds(Utility.RandomMinMax(10, 15)); } }

        public override bool CanRummageCorpses { get { return true; } }
        public override bool AlwaysMurderer { get { return true; } }
        public override bool CanDestroyObstacles { get { return true; } }
        public override bool CanFlee { get { return true; } }
        public override bool AutoDispel { get { return true; } }
        public override bool Unprovokable { get { return true; } }
        public override Poison PoisonImmune { get { return Poison.Lethal; } }

        [Constructable]
        public LockeColeMirrorImage()
            : base(AIType.AI_Arcade, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Locke Cole";
            Body = 400;
            Hue = 1162;
            SpecialTitle = "Undead Treasure Hunter";
            TitleHue = 1174;

            BaseSoundID = 362;

            SetStr(10, 10);
            SetDex(200);
            SetInt(706, 726);

            SetHits(10000);

            SetDamage(1, 1);

            foreach (SkillName skill in _Skills)
            {
                SetSkill(skill, 65.0, 70.0);
            }

            SpeechHue = YellHue = 34;

            VirtualArmor = 90;

            PackGold(500, 1000);
            PackMagicItems(5, 5, 0.95, 0.95);

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

        public LockeColeMirrorImage(Serial serial)
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
                else if (now > _NextUltima)
                {
                    _NextUltima = now + UltimaInterval;

                    CurrentSpell.Clear();

                    CantWalk = true;
                    Combatant = null;
                    Say("*Begins to cast Ultima*");
                    AnimateSpell();
                    CurrentSpell.Add(Timer.DelayCall(TimeSpan.FromSeconds(2), Ultima_Callback));
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
                    mobile1.Damage(5, this);
                    mobile1.ApplyPoison(this, Poison.Lesser);
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
            var queue = new EffectQueue();
            queue.Deferred = true;

            BaseExplodeEffect e = ExplodeFX.Smoke.CreateInstance(
                impactloc, Map, 4, 0, TimeSpan.FromMilliseconds(1000 - ((10) * 100)), null, () =>
                {
                    foreach (Mobile mobile in AcquireAllTargets(impactloc, 4))
                    {
                        if (mobile is PlayerMobile)
                        {
                            mobile.Damage(5, this);
                        }
                        else
                        {
                            mobile.Damage(25, this);
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
            writer.Write((int) 0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}