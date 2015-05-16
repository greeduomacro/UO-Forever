namespace Server.Mobiles
{
    [CorpseName("a drake corpse")]
    public class PathaleoDrake : BaseCreature
    {
        public override string DefaultName { get { return "a pathaleo drake"; } }

        [Constructable]
        public PathaleoDrake() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.15, 0.2)
        {
            Body = Utility.Random(60, 2);
            BaseSoundID = 362;
            Hue = 0x5d;

            Alignment = Alignment.Dragon;

            SetStr(301, 330);
            SetDex(133, 152);
            SetInt(101, 140);

            SetHits(178, 215);

            SetDamage(8, 14);


            SetSkill(SkillName.MagicResist, 65.1, 80.0);
            SetSkill(SkillName.Tactics, 65.1, 90.0);
            SetSkill(SkillName.Wrestling, 65.1, 80.0);

            Fame = 3500;
            Karma = -3500;

            VirtualArmor = 36;

            Tamable = true;
            ControlSlots = 2;
            MinTameSkill = 74.5;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average, 2);
            AddLoot(LootPack.MedScrolls);
        }

        public override bool ReacquireOnMovement { get { return true; } }
        public override bool HasBreath { get { return true; } } // fire breath enabled
        public override int TreasureMapLevel { get { return 2; } }
        public override int Meat { get { return 10; } }
        public override int Hides { get { return 18; } }
        public override HideType HideType { get { return HideType.Spined; } }
        public override int Scales { get { return 2; } }
        public override ScaleType ScaleType { get { return (ScaleType.Blue); } }
        public override FoodType FavoriteFood { get { return FoodType.Meat | FoodType.Fish; } }
        public override int DefaultBloodHue { get { return -2; } } //Template
        public override int BloodHueTemplate { get { return Utility.RandomGreyHue(); } }

        public PathaleoDrake(Serial serial) : base(serial)
        {}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }
}