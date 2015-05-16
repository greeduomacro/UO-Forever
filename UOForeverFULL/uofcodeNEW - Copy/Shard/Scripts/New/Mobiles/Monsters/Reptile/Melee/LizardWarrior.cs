namespace Server.Mobiles
{
    [CorpseName("a lizardman corpse")]
    public class LizardWarrior : BaseCreature
    {
        [Constructable]
        public LizardWarrior() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = NameList.RandomName("lizardman");
            Body = Utility.RandomList(35, 36);
            BaseSoundID = 417;
            Hue = 636;

            Alignment = Alignment.Inhuman;

            SetStr(120, 220);
            SetDex(186, 205);
            SetInt(136, 160);

            SetHits(300, 375);

            SetDamage(25, 27);


            SetSkill(SkillName.MagicResist, 60.0, 75.1);
            SetSkill(SkillName.Tactics, 75.1, 99.0);
            SetSkill(SkillName.Wrestling, 90.1, 100.0);

            Fame = 6000;
            Karma = -6000;

            VirtualArmor = 35;

            PackGold(225, 450);
        }

        public override int Meat { get { return 2; } }
        public override int Hides { get { return 25; } }
        public override HideType HideType { get { return HideType.Spined; } }

        public LizardWarrior(Serial serial) : base(serial)
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