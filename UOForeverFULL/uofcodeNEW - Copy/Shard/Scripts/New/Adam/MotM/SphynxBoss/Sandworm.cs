namespace Server.Mobiles
{
    [CorpseName("a sandworm corpse")]
    public class Sandworm : BaseCreature
    {
        [Constructable]
        public Sandworm() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a sandworm";
            Body = 787;
            BaseSoundID = 1006;
            Hue = 0x58B;

            SetStr(496, 520);
            SetDex(181, 205);
            SetInt(136, 260);

            SetHits(451, 562);

            SetDamage(21, 31);

            SetSkill(SkillName.Anatomy, 90.0);
            SetSkill(SkillName.MagicResist, 70.0);
            SetSkill(SkillName.Tactics, 90.0);
            SetSkill(SkillName.Wrestling, 90.0);

            Fame = 14500;
            Karma = -14500;

            VirtualArmor = 65;

            PackGem();
            PackGem();
            PackGold(330, 470);

            int amount = Utility.RandomMinMax(1, 10);
        }

        public override bool Unprovokable { get { return true; } }
        public override int Meat { get { return 5; } }
        public override int Hides { get { return 10; } }
        public override HideType HideType { get { return HideType.Barbed; } }

        public Sandworm(Serial serial) : base(serial)
        {}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}