namespace Server.Mobiles
{
    [CorpseName("an undead horse corpse")]
    public class SkeletalMount : BaseMount
    {
        public override string DefaultName { get { return "a skeletal steed"; } }

        public override int InternalItemItemID { get { return 0x3EBB; } }

        [Constructable]
        public SkeletalMount() : base(793, AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            SetStr(91, 120);
            SetDex(56, 75);
            SetInt(56, 70);

            SetHits(41, 50);

            SetDamage(5, 12);

            SetSkill(SkillName.MagicResist, 95.1, 100.0);
            SetSkill(SkillName.Tactics, 50.0);
            SetSkill(SkillName.Wrestling, 70.1, 80.0);

            Fame = 0;
            Karma = 0;

            Tamable = true;
            MinTameSkill = 115.0;
            ControlSlots = 2;
        }

        public override Poison PoisonImmune { get { return Poison.Lethal; } }
        public override bool BleedImmune { get { return true; } }

        public SkeletalMount(Serial serial) : base(serial)
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

            switch (version)
            {
                case 1:
                case 0:
                {
                    //Name = "a skeletal steed";
                    //Tamable = false;
                    //MinTameSkill = 120.0;
                    //ControlSlots = 2;
                    break;
                }
            }
        }
    }
}