#region References

using System;
using System.Collections.Generic;
using Server.Engines.Craft;
using Server.Items;
using Server.Misc;
using Server.Spells;

#endregion

namespace Server.Mobiles
{
    [CorpseName("crimson dragon corpse")]
    public class CrimsonDragon : BaseCreature
    {
        public override string DefaultName { get { return "a crimson dragon"; } }

        [Constructable]
        public CrimsonDragon() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Body = 197;
            BaseSoundID = 362;

            Alignment = Alignment.Dragon;

            SetStr(2034, 2140);
            SetDex(215, 256);
            SetInt(1025, 1116);

            SetHits(2500);

            SetDamage(20, 30);

            SetSkill(SkillName.EvalInt, 110.2, 125.3);
            SetSkill(SkillName.Magery, 110.9, 125.5);
            SetSkill(SkillName.MagicResist, 116.3, 125.0);
            SetSkill(SkillName.Tactics, 111.7, 126.3);
            SetSkill(SkillName.Wrestling, 120.5, 128.0);
            SetSkill(SkillName.Meditation, 119.4, 130.0);
            SetSkill(SkillName.Anatomy, 118.7, 125.0);
            SetSkill(SkillName.DetectHidden, 120.0);

            Fame = 5000;
            Karma = -5000;

            VirtualArmor = 70;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 3);
            AddLoot(LootPack.Gems, 5);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.Random(500) == 1)
            {
                PackItem(new DragonBoneShards());
            }
            if (Utility.Random(10000) == 1)
            {
                PackItem(new DragonHeart());
            }
        }

        public override int GetIdleSound()
        {
            return 0x2D3;
        }

        public override int GetHurtSound()
        {
            return 0x2D1;
        }

        public override bool AlwaysMurderer { get { return true; } }
        public override bool Unprovokable { get { return true; } }
        public override bool BardImmune { get { return true; } }
        //public override bool GivesMinorArtifact{ get{ return true; } }
        public override bool ReacquireOnMovement { get { return true; } }
        public override bool HasBreath { get { return true; } } // fire breath enabled
        public override bool AutoDispel { get { return true; } }
        public override bool Uncalmable { get { return true; } }
        public override int Meat { get { return 19; } }
        public override int Hides { get { return 40; } }
        public override HideType HideType { get { return HideType.Barbed; } }
        public override int Scales { get { return 35; } }
        public override ScaleType ScaleType { get { return ScaleType.Any; } }
        public override FoodType FavoriteFood { get { return FoodType.Meat; } }
        public override Poison PoisonImmune { get { return Poison.Lethal; } }
        public override Poison HitPoison { get { return Utility.RandomBool() ? Poison.Deadly : Poison.Lethal; } }
        public override int TreasureMapLevel { get { return 5; } }

        public override void Damage(int amount, Mobile target)
        {
            if (target is Dragon || target is Drake || target is YoungDragon || target is PathaleoDrake ||
                target is WhiteDrake || target is WhiteWyrm || target is Wyvern)
            {
                var creature = target as BaseCreature;
                if (creature.Controlled || creature.Summoned)
                {
                    amount *= 3;
                }
            }

            base.Damage(amount, target);
        }

        public override void BreathDealDamage(Mobile target)
        {
            base.BreathDealDamage(target);

            Geometry.Circle2D(target.Location, target.Map, 3, new DoEffect_Callback(FireBombEffect));
            Geometry.Circle2D(target.Location, target.Map, 2, new DoEffect_Callback(FireBombEffect));
            Geometry.Circle2D(target.Location, target.Map, 1, new DoEffect_Callback(FireBombEffect));
            Geometry.Circle2D(target.Location, target.Map, 0, new DoEffect_Callback(FireBombEffect));
        }

        public void FireBombEffect(Point3D p, Map map)
        {
            if (map != null && map.CanFit(p, 12, true, false))
            {
                new InternalItem(null, p, map, 5, 5);
            }
        }

        public class InternalItem : Item
        {
            private Mobile m_From;
            private int m_MinDamage;
            private int m_MaxDamage;
            private DateTime m_End;
            private Timer m_Timer;

            public Mobile From { get { return m_From; } }

            public override bool BlocksFit { get { return true; } }

            public InternalItem(Mobile from, Point3D loc, Map map, int min, int max) : base(0x398C)
            {
                Movable = false;
                Light = LightType.Circle300;

                MoveToWorld(loc, map);

                m_From = from;
                m_End = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.Random(3) + 5);

                SetDamage(min, max);

                m_Timer = new InternalTimer(this, m_End);
                m_Timer.Start();
            }

            public override void OnAfterDelete()
            {
                base.OnAfterDelete();

                if (m_Timer != null)
                {
                    m_Timer.Stop();
                }
            }

            public InternalItem(Serial serial) : base(serial)
            {}

            public int GetDamage()
            {
                return Utility.RandomMinMax(m_MinDamage, m_MaxDamage);
            }

            private void SetDamage(int min, int max)
            {
                m_MinDamage = min;
                m_MaxDamage = max;
            }

            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);

                writer.Write((int) 0); // version

                writer.Write((Mobile) m_From);
                writer.Write((DateTime) m_End);
                writer.Write((int) m_MinDamage);
                writer.Write((int) m_MaxDamage);
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);

                int version = reader.ReadInt();

                m_From = reader.ReadMobile();
                m_End = reader.ReadDateTime();
                m_MinDamage = reader.ReadInt();
                m_MaxDamage = reader.ReadInt();

                m_Timer = new InternalTimer(this, m_End);
                m_Timer.Start();
            }

            public override bool OnMoveOver(Mobile m)
            {
                if (Visible && m_From != null && m != m_From && SpellHelper.ValidIndirectTarget(m_From, m) &&
                    m_From.CanBeHarmful(m, false))
                {
                    m_From.DoHarmful(m);

                    m.Damage(GetDamage(), m_From);
                    m.PlaySound(0x208);
                }

                return true;
            }

            private class InternalTimer : Timer
            {
                private readonly InternalItem m_Item;
                private readonly DateTime m_End;

                public InternalTimer(InternalItem item, DateTime end) : base(TimeSpan.Zero, TimeSpan.FromSeconds(1.0))
                {
                    m_Item = item;
                    m_End = end;

                    Priority = TimerPriority.FiftyMS;
                }

                protected override void OnTick()
                {
                    if (m_Item.Deleted)
                    {
                        return;
                    }

                    if (DateTime.UtcNow > m_End)
                    {
                        m_Item.Delete();
                        Stop();
                        return;
                    }

                    Mobile from = m_Item.From;

                    if (m_Item.Map == null || from == null)
                    {
                        return;
                    }

                    var mobiles = new List<Mobile>();

                    foreach (Mobile mobile in m_Item.GetMobilesInRange(0))
                    {
                        mobiles.Add(mobile);
                    }

                    for (int i = 0; i < mobiles.Count; i++)
                    {
                        Mobile m = mobiles[i];

                        if ((m.Z + 16) > m_Item.Z && (m_Item.Z + 12) > m.Z && m != from &&
                            SpellHelper.ValidIndirectTarget(from, m) && from.CanBeHarmful(m, false))
                        {
                            if (from != null)
                            {
                                from.DoHarmful(m);
                            }

                            m.Damage(m_Item.GetDamage(), from);
                            m.PlaySound(0x208);
                        }
                    }
                }
            }
        }

        private DateTime m_NextTerror;

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            base.OnMovement(m, oldLocation);

            if (m_NextTerror < DateTime.UtcNow && m != null && !m.Frozen && InRange(m.Location, 3) &&
                m.AccessLevel == AccessLevel.Player)
            {
                m.Frozen = true;
                m.SendLocalizedMessage(1080342, Name, 33);
                    // Terror slices into your very being, destroying any chance of resisting ~1_name~ you might have had

                Timer.DelayCall<Mobile>(TimeSpan.FromSeconds(5), new TimerStateCallback<Mobile>(Terrorize), m);
            }
        }

        private void Terrorize(Mobile m)
        {
            m.Frozen = false;
            m.SendLocalizedMessage(1005603); // You can move again!

            m_NextTerror = DateTime.UtcNow + TimeSpan.FromMinutes(5);
        }

        public void TurnOnOwner(Mobile attacker)
        {
            if (0.1 > Utility.RandomDouble() &&
                (attacker is Dragon || attacker is Drake || attacker is YoungDragon || attacker is PathaleoDrake ||
                 attacker is WhiteDrake || attacker is WhiteWyrm || attacker is Wyvern))
            {
                var c = attacker as BaseCreature;

                if (c.Controlled && c.ControlMaster != null)
                {
                    c.ControlTarget = c.ControlMaster;
                    c.ControlOrder = OrderType.Attack;
                    c.Combatant = c.ControlMaster;
                }
            }
        }

        public override void OnGotMeleeAttack(Mobile attacker)
        {
            base.OnGotMeleeAttack(attacker);
            TurnOnOwner(attacker);
        }

        public override void OnDamagedBySpell(Mobile attacker)
        {
            base.OnDamagedBySpell(attacker);
            TurnOnOwner(attacker);
        }

        public override bool OnBeforeDeath()
        {
            if (Combatant != null)
            {
                Combatant.Kills++;
            }

            if (Utility.Random(500) == 1)
            {
                PackItem(new DragonBoneShards());
            }
            if (Utility.Random(10000) == 1)
            {
                PackItem(new DragonHeart());
            }
            return base.OnBeforeDeath();
        }

        public CrimsonDragon(Serial serial) : base(serial)
        {}

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