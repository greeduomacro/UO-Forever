namespace Server.Mobiles
{
    [CorpseName("a titan corpse")]
    public class TitanMonsters : BaseCreature
    {
        public TitanMonsters() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Body = 76;
            BaseSoundID = 609;

            SetStr(700);
            SetDex(200);
            SetInt(400);

            SetHits(700, 800);

            SetDamage(20, 22);

            Fame = 20000;
            Karma = -20000;

            VirtualArmor = 44;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 2);
        }

        public override bool Unprovokable { get { return true; } }
        public override bool Uncalmable { get { return true; } }
        public override Poison PoisonImmune { get { return Poison.Lethal; } }
        public override int TreasureMapLevel { get { return 5; } }

        public TitanMonsters(Serial serial) : base(serial)
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

    public class IceTitan : TitanMonsters
    {
        [Constructable]
        public IceTitan()
        {
            Name = "a ice titan";
            Hue = 1152;

            SetSkill(SkillName.EvalInt, 10.5, 60.0);
            SetSkill(SkillName.Magery, 10.5, 60.0);
            SetSkill(SkillName.MagicResist, 30.1, 80.0);
            SetSkill(SkillName.Tactics, 70.1, 100.0);
            SetSkill(SkillName.Wrestling, 60.1, 100.0);
        }

        public IceTitan(Serial serial) : base(serial)
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

    public class WindTitan : TitanMonsters
    {
        public override bool Unprovokable { get { return true; } }

        [Constructable]
        public WindTitan()
        {
            Name = "an earth titan";
            Hue = 1196;

            Alignment = Alignment.Giantkin;

            SetSkill(SkillName.EvalInt, 60.1, 75.0);
            SetSkill(SkillName.Magery, 60.1, 75.0);
            SetSkill(SkillName.MagicResist, 60.1, 75.0);
            SetSkill(SkillName.Tactics, 60.1, 80.0);
            SetSkill(SkillName.Wrestling, 60.1, 80.0);
        }

        public WindTitan(Serial serial) : base(serial)
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

    public class PoisonTitan : TitanMonsters
    {
        [Constructable]
        public PoisonTitan()
        {
            Name = "a poison titan";
            Hue = 2350;

            SetSkill(SkillName.EvalInt, 80.1, 95.0);
            SetSkill(SkillName.Magery, 80.1, 95.0);
            SetSkill(SkillName.Meditation, 80.2, 120.0);
            SetSkill(SkillName.Poisoning, 90.1, 100.0);
            SetSkill(SkillName.MagicResist, 85.2, 115.0);
            SetSkill(SkillName.Tactics, 80.1, 100.0);
            SetSkill(SkillName.Wrestling, 70.1, 90.0);
        }

        public PoisonTitan(Serial serial) : base(serial)
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

    public class BloodTitan : TitanMonsters
    {
        [Constructable]
        public BloodTitan()
        {
            Name = "a blood titan";
            Hue = 2444;

            SetSkill(SkillName.EvalInt, 85.1, 100.0);
            SetSkill(SkillName.Magery, 85.1, 100.0);
            SetSkill(SkillName.Meditation, 10.4, 50.0);
            SetSkill(SkillName.MagicResist, 80.1, 95.0);
            SetSkill(SkillName.Tactics, 80.1, 100.0);
            SetSkill(SkillName.Wrestling, 80.1, 100.0);
        }

        public BloodTitan(Serial serial) : base(serial)
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

    public class WaterTitan : TitanMonsters
    {
        [Constructable]
        public WaterTitan()
        {
            Name = "a water titan";
            Hue = 2437;

            SetSkill(SkillName.EvalInt, 60.1, 75.0);
            SetSkill(SkillName.Magery, 60.1, 75.0);
            SetSkill(SkillName.MagicResist, 100.1, 115.0);
            SetSkill(SkillName.Tactics, 50.1, 70.0);
            SetSkill(SkillName.Wrestling, 50.1, 70.0);
        }

        public WaterTitan(Serial serial) : base(serial)
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

    public class EtherealTitan : TitanMonsters
    {
        [Constructable]
        public EtherealTitan()
        {
            Name = "a ethereal titan";
            Hue = 0x4E1F;

            Body = 76;
            BaseSoundID = 609;

            SetStr(1000);
            SetDex(300);
            SetInt(600);

            SetHits(1500);

            SetDamage(28, 30);

            Fame = 20000;
            Karma = -20000;

            VirtualArmor = 54;

            SetSkill(SkillName.EvalInt, 60.1, 75.0);
            SetSkill(SkillName.Magery, 60.1, 75.0);
            SetSkill(SkillName.MagicResist, 100.1, 115.0);
            SetSkill(SkillName.Tactics, 60.1, 80.0);
            SetSkill(SkillName.Wrestling, 60.1, 80.0);
        }

        public EtherealTitan(Serial serial) : base(serial)
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