namespace Server.Mobiles
{
    [CorpseName("a glowing lizardman corpse")]
    public class LizardMage : BaseCreature
    {
        [Constructable]
        public LizardMage() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = NameList.RandomName("lizardman");
            Body = Utility.RandomList(35, 36);
            BaseSoundID = 417;
            Hue = 353;

            Alignment = Alignment.Inhuman;

            SetStr(146, 180);
            SetDex(101, 130);
            SetInt(186, 210);

            SetHits(88, 108);

            SetDamage(7, 14);


            SetSkill(SkillName.EvalInt, 70.1, 80.0);
            SetSkill(SkillName.Magery, 70.1, 80.0);
            SetSkill(SkillName.MagicResist, 65.1, 90.0);
            SetSkill(SkillName.Tactics, 50.1, 75.0);
            SetSkill(SkillName.Wrestling, 50.1, 75.0);

            Fame = 3500;
            Karma = -3500;

            VirtualArmor = 44;

            PackGold(75, 125);
            PackReg(6);
            PackScroll(1, 7);
        }

        public override int Meat { get { return 1; } }
        public override int Hides { get { return 8; } }
        public override HideType HideType { get { return HideType.Spined; } }

        public LizardMage(Serial serial) : base(serial)
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