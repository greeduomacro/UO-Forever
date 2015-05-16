

#region References

using System;
using System.Collections.Generic;
using System.Linq;
using Server.Engines.CannedEvil;
using Server.Items;
using Server.Spells;
using VitaNex.FX;
using VitaNex.Network;

#endregion

namespace Server.Mobiles
{
    [CorpseName("stone magus rubble")]
    public class StoneMagus : StoneGuardian
    {
        private static readonly SkillName[] _Skills = ((SkillName) 0).GetValues<SkillName>();

        private DateTime _NextMeteor = DateTime.UtcNow;
        public TimeSpan MeteorInterval { get { return TimeSpan.FromSeconds(Utility.RandomMinMax(30, 45)); } }
        public int MeteorRange { get { return Utility.RandomMinMax(2, 3); } }

        public override string DefaultName { get { return "a stone magus"; } }

        [Constructable]
        public StoneMagus()
        {
            Name = "a stone magus";
            Body = 400;
            Hue = 2407;

            AI = AIType.AI_Mage;
            SetStr(1254, 1381);
            SetDex(93, 135);
            SetInt(745, 810);

            SetHits(694, 875);

            SetDamage(12, 20);

            foreach (SkillName skill in _Skills)
            {
                SetSkill(skill, 80.0, 140.0);
            }

            IsStone = true;
            Frozen = true;
            Blessed = true;

            SpeechHue = YellHue = EmoteHue = 731;

            VirtualArmor = 90;

            PackGold(3000, 3500);
            PackMagicItems(5, 5, 0.95, 0.95);
            PackMagicItems(5, 5, 0.80, 0.65);
            PackMagicItems(5, 5, 0.80, 0.65);
            PackMagicItems(6, 6, 0.80, 0.65);
        }

        public StoneMagus(Serial serial)
            : base(serial)
        {}

        public override void OnThink()
        {
            base.OnThink();

            if (DateTime.UtcNow > _NextMeteor && !IsStone)
            {
                _NextMeteor = DateTime.UtcNow + MeteorInterval;

                Animate();

                Yell("Corp Por Kal Des Ylem");

                Timer.DelayCall(TimeSpan.FromSeconds(3.0), Meteor_Callback);
            }            
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

            for (int i = 0; i < 10; i++)
            {
                BaseExplodeEffect e = ExplodeFX.Fire.CreateInstance(
                    impactloc, Map, MeteorRange, 0, TimeSpan.FromMilliseconds(1000 - ((10) * 100)), null, () =>
                    {
                        foreach (Mobile target in AcquireTargets(impactloc, MeteorRange))
                        {
                            if (target is BaseCreature)
                                target.Damage(50, this);
                            else
                                target.Damage(10, this);
                        }
                    });
                e.Send();
            }
            queue.Process();
        }

        public override void GiveEquipment()
        {
            ElvenRobe robe = new ElvenRobe();
            robe.Name = "stone robe";
            robe.Hue = 2407;
            robe.Identified = true;
            robe.Movable = false;
            AddItem(Immovable(robe));

            FullSpellbook spellbook = new FullSpellbook();
            spellbook.Name = "stone spellbook";
            spellbook.Hue = 2407;
            spellbook.Movable = false;
            AddItem(Immovable(spellbook));
        }

        public override bool OnBeforeDeath()
        {
            if (0.01 > Utility.RandomDouble())
            {
                PackItem(new FullSpellbook() { Name = "a stone spellbook", LootType = LootType.Regular, Hue = 1072 });  
            }
            if (0.05 >= Utility.RandomDouble())
                PackItem(new StoneFragment());
            return base.OnBeforeDeath();
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
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}