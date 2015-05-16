namespace Server.Mobiles
{
    [CorpseName("a snow harpy corpse")]
    public class SnowHarpy : BaseCreature
    {
        public override string DefaultName { get { return "a snow harpy"; } }

        [Constructable]
        public SnowHarpy() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Body = 73;
            Hue = Utility.RandomList(1150, 1151, 1153);
            BaseSoundID = 402;

            Alignment = Alignment.Inhuman;

            SetStr(361, 390);
            SetDex(59, 90);
            SetInt(69, 93);

            SetHits(351, 496);
            SetMana(0);

            SetDamage(10, 15);


            SetSkill(SkillName.MagicResist, 76.1, 89.0);
            SetSkill(SkillName.Tactics, 89.1, 115.0);
            SetSkill(SkillName.Wrestling, 94.1, 110.0);

            Fame = 7500;
            Karma = -7500;

            VirtualArmor = 40;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.Average, 2);
            AddLoot(LootPack.Gems, 2);
        }

        public override int GetAttackSound()
        {
            return 916;
        }

        public override int GetAngerSound()
        {
            return 916;
        }

        public override int GetDeathSound()
        {
            return 917;
        }

        public override int GetHurtSound()
        {
            return 919;
        }

        public override int GetIdleSound()
        {
            return 918;
        }

        public override int Meat { get { return 1; } }
        public override int Feathers { get { return 50; } }

        public SnowHarpy(Serial serial) : base(serial)
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