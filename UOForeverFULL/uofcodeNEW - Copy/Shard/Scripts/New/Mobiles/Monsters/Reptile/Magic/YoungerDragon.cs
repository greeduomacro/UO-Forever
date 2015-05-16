namespace Server.Mobiles
{
    [CorpseName("a young dragon corpse")]
    public class YoungerDragon : BaseCreature
    {
        public override string DefaultName { get { return "a young dragon"; } }

        [Constructable]
        public YoungerDragon() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Body = Utility.RandomList(60, 61);
            Hue = 0.15 > Utility.RandomDouble() ? Utility.RandomGreenHue() : 0;
            BaseSoundID = 362;

            Alignment = Alignment.Dragon;

            SetStr(496, 425);
            SetDex(86, 105);
            SetInt(436, 475);

            SetHits(269, 330);

            SetDamage(10, 12);


            SetSkill(SkillName.EvalInt, 30.1, 40.0);
            SetSkill(SkillName.Magery, 20.1, 30.0);
            SetSkill(SkillName.MagicResist, 69.1, 79.0);
            SetSkill(SkillName.Tactics, 57.6, 70.0);
            SetSkill(SkillName.Wrestling, 69.1, 72.5);

            Fame = 7500;
            Karma = -7500;

            VirtualArmor = 30;

            Tamable = true;
            ControlSlots = 2;
            MinTameSkill = 73.9;
        }

        public override void GenerateLoot()
        {
            if (Utility.RandomBool())
            {
                AddLoot(LootPack.FilthyRich);
            }
            else
            {
                AddLoot(LootPack.Rich);
            }

            AddLoot(LootPack.Average);

            AddLoot(LootPack.LowScrolls);
        }

        public override bool ReacquireOnMovement { get { return true; } }
        public override bool HasBreath { get { return true; } } // fire breath enabled
        public override bool AutoDispel { get { return true; } }
        public override int TreasureMapLevel { get { return 3; } }
        public override int Meat { get { return 9; } }
        public override int Hides { get { return 10; } }
        public override HideType HideType { get { return HideType.Barbed; } }
        public override int Scales { get { return 5; } }

        public override ScaleType ScaleType
        {
            get { return Hue == 0 ? (Body == 60 ? ScaleType.Yellow : ScaleType.Red) : ScaleType.Green; }
        }

        public override FoodType FavoriteFood { get { return FoodType.Meat; } }

        public YoungerDragon(Serial serial) : base(serial)
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