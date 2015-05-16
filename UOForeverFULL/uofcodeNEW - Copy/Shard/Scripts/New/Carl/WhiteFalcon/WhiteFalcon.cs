#region References

using System;
using Server.Items;

#endregion

namespace Server.Mobiles
{
    [CorpseName("a white falcon's corpse")]
    public class WhiteFalcon : BaseCreature
    {
        

        [Constructable]
        public WhiteFalcon() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a white falcon";
            Body = 5;
            Hue = 1153;

            BaseSoundID = 0x2EE;
            SpeechHue = 1157;

            SetStr(605, 611);
            SetDex(391, 519);
            SetInt(669, 818);

            SetHits(1000, 1500);
            SetDamage(50, 75);

            SetSkill(SkillName.Wrestling, 121.9, 130.6);
            SetSkill(SkillName.Tactics, 114.4, 117.4);
            SetSkill(SkillName.MagicResist, 147.7, 153.0);
            SetSkill(SkillName.Poisoning, 122.8, 124.0);
            SetSkill(SkillName.Magery, 121.8, 127.8);
            SetSkill(SkillName.EvalInt, 103.6, 117.0);
            SetSkill(SkillName.Meditation, 100.0, 110.0);

            Fame = 21000;
            Karma = -21000;

            VirtualArmor = 34;

            Tamable = false;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.UltraRich, 3);
        }

        #region Freeze Attack - Custom Offensive Attacks

        public void FreezeAttack(Mobile m)
        {
            m.Paralyze(TimeSpan.FromSeconds(10));
            m.PlaySound(541);
            m.FixedEffect(0x376A, 6, 1);
            m.HueMod = 1152;
            m.SendMessage("The white falcon's breath turns you to ice!");
            new FreezeAttackTimer(m).Start();
        }

        private class FreezeAttackTimer : Timer
        {
            private Mobile m_Owner;

            public FreezeAttackTimer(Mobile owner) : base(TimeSpan.FromSeconds(10.0))
            {
                m_Owner = owner;

                Priority = TimerPriority.OneSecond;
            }

            protected override void OnTick()
            {
                m_Owner.HueMod = -1;
                m_Owner.PlaySound(65);
            }
        }

        public override void OnGotMeleeAttack(Mobile attacker)
        {
            base.OnGotMeleeAttack(attacker);

            if (0.1 >= Utility.RandomDouble())
            {
                FreezeAttack(attacker);
            }
        }

        public override void OnDamagedBySpell(Mobile attacker)
        {
            base.OnDamagedBySpell(attacker);

            if (Hits > 2000)
            {
                FreezeAttack(attacker);
            }
        }

        #endregion

        #region Mobiles Can Be Assigned Weapon Abilities

        public override WeaponAbility GetWeaponAbility()
        {
            if (Utility.RandomBool())
            {
                return WeaponAbility.ParalyzingBlow;
            }
            return WeaponAbility.BleedAttack;
        }

        #endregion

        //Harvestable Resources
        public override int Meat { get { return 1; } }
        public override MeatType MeatType { get { return MeatType.Bird; } }
        public override int Feathers { get { return 36; } }
        public override FoodType FavoriteFood { get { return FoodType.Meat | FoodType.Fish; } }


        //Reward Drops After Kill
        public override int TreasureMapLevel { get { return 5; } }

        //Has An Area Damage Effect
        public override bool HasAura { get { return true; } }

        //Player Attack Immunities 
        public override bool BardImmune { get { return true; } }

        public WhiteFalcon(Serial serial) : base(serial)
        {}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}