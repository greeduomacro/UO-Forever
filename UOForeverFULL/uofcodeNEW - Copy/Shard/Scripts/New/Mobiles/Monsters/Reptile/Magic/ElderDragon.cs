#region References

using Server.Engines.Craft;

#endregion

namespace Server.Mobiles
{
    [CorpseName("an elder dragon corpse")]
    public class ElderDragon : BaseCreature
    {
        public override string DefaultName { get { return "an elder dragon"; } }

        [Constructable]
        public ElderDragon() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Body = Utility.RandomList(12, 59);
            Hue = (Body == 12 && 0.02 > Utility.RandomDouble()) ? Utility.RandomList(1445, 1436, 2006, 2001) : 0;
            BaseSoundID = 362;

            Alignment = Alignment.Dragon;

            SetStr(896, 982);
            SetDex(86, 105);
            SetInt(436, 475);

            SetHits(678, 795);

            SetDamage(20, 25);


            SetSkill(SkillName.EvalInt, 50.1, 70.0);
            SetSkill(SkillName.Magery, 50.1, 70.0);
            SetSkill(SkillName.MagicResist, 99.1, 100.0);
            SetSkill(SkillName.Tactics, 99.1, 104.0);
            SetSkill(SkillName.Wrestling, 99.1, 100.0);

            Fame = 15500;
            Karma = -15500;

            VirtualArmor = 80;

            Tamable = false;
            //ControlSlots = 4;
            //MinTameSkill = 105.9;
        }

        public override void GenerateLoot()
        {
            if (Utility.RandomBool())
            {
                AddLoot(LootPack.UltraRich);
            }
            else
            {
                AddLoot(LootPack.FilthyRich);
            }

            AddLoot(LootPack.Rich);
            AddLoot(LootPack.HighScrolls);
        }

        public override bool OnBeforeDeath()
        {
            if (!Controlled)
            {
                if (0.05 > Utility.RandomDouble())
                {
                    PackItem(new DragonBoneShards());
                }
                if (0.001 > Utility.RandomDouble())
                {
                    PackItem(new DragonHeart());
                }
            }
            return base.OnBeforeDeath();
        }

        public override bool ReacquireOnMovement { get { return !Controlled; } }
        public override bool HasBreath { get { return true; } } // fire breath enabled
        public override bool AutoDispel { get { return true; } }
        public override int TreasureMapLevel { get { return 5; } }
        public override int Meat { get { return 19; } }
        public override int Hides { get { return 30; } }
        public override HideType HideType { get { return HideType.Barbed; } }
        public override int Scales { get { return 9; } }

        public override ScaleType ScaleType
        {
            get { return Hue == 0 ? (Body == 12 ? ScaleType.Yellow : ScaleType.Red) : ScaleType.Green; }
        }

        public override FoodType FavoriteFood { get { return FoodType.Meat; } }
        public override int DefaultBloodHue { get { return -2; } } //Template
        public override int BloodHueTemplate { get { return Utility.RandomGreyHue(); } }

        public ElderDragon(Serial serial) : base(serial)
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