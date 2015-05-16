#region References

using Server.Items;

#endregion

namespace Server.Mobiles
{
    [CorpseName("an ogre mages corpse")]
    public class OgreMage : BaseCreature
    {
        public override string DefaultName { get { return "an ogre mage"; } }

        [Constructable]
        public OgreMage() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Hue = 1502;
            Body = 83;
            BaseSoundID = 427;

            Alignment = Alignment.Giantkin;

            SetStr(945, 1023);
            SetDex(66, 75);
            SetInt(536, 675);

            SetHits(670, 792);

            SetDamage(25, 32);


            SetSkill(SkillName.MagicResist, 125.1, 140.0);
            SetSkill(SkillName.Tactics, 90.1, 100.0);
            SetSkill(SkillName.Wrestling, 90.1, 100.0);

            SetSkill(SkillName.Magery, 90.1, 110.0);

            SetSkill(SkillName.EvalInt, 90.1, 100.0);
            SetSkill(SkillName.Meditation, 90.1, 110.0);


            Fame = 15000;
            Karma = -15000;

            VirtualArmor = 75;

            var weapon = new Club();

            weapon.DamageLevel = (WeaponDamageLevel) Utility.Random(1, 5);
            weapon.DurabilityLevel = (WeaponDurabilityLevel) Utility.Random(0, 5);
            weapon.AccuracyLevel = (WeaponAccuracyLevel) Utility.Random(0, 5);

            PackItem(weapon);

            PackArmor(0, 5);
            PackWeapon(0, 5);
            PackGold(1100, 1400);

            if (0.04 > Utility.RandomDouble()) // 2 percent - multipy number x 100 to get percent
            {
                var scroll = new SkillScroll();
                scroll.Randomize();
                PackItem(scroll);
            }
        }

        public override bool CanRummageCorpses { get { return true; } }
        public override Poison PoisonImmune { get { return Poison.Greater; } }
        public override int TreasureMapLevel { get { return 3; } }
        public override int Meat { get { return 2; } }

        public OgreMage(Serial serial) : base(serial)
        {}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt((int) 0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }
}