#region References

using Server.Engines.Craft;
using Server.Items;

#endregion

namespace Server.Mobiles
{
    [CorpseName("a white wyrm corpse")]
    public class WhiteWyrm : BaseCreature
    {
        public override string DefaultName { get { return "a white wyrm"; } }

        [Constructable]
        public WhiteWyrm() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Body = (0.05 > Utility.RandomDouble()) ? 49 : 180;
            Hue = Utility.Random(1000) == 0 ? 1154 : 0;
            BaseSoundID = 362;

            Alignment = Alignment.Dragon;

            SetStr(721, 760);
            SetDex(101, 130);
            SetInt(386, 425);

            SetHits(433, 456);

            SetDamage(17, 25);


            SetSkill(SkillName.EvalInt, 99.1, 100.0);
            SetSkill(SkillName.Magery, 99.1, 100.0);
            SetSkill(SkillName.MagicResist, 99.1, 100.0);
            SetSkill(SkillName.Tactics, 97.6, 100.0);
            SetSkill(SkillName.Wrestling, 90.1, 100.0);

            Fame = 16000;
            Karma = -16000;

            VirtualArmor = 64;

            Tamable = true;
            ControlSlots = 2;
            MinTameSkill = 96.3;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 2);
            AddLoot(LootPack.Average);
            AddLoot(LootPack.Gems, Utility.Random(1, 5));
        }

        public override bool OnBeforeDeath()
        {
            if (!IsBonded)
            {
                switch (Utility.Random(500))
                {
                    case 0:
                        PackItem(new PolarBearMask());
                        break;
                }
            }

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

        public override bool ReacquireOnMovement { get { return true; } }
        public override int TreasureMapLevel { get { return 4; } }
        public override int Meat { get { return Body != 104 ? 19 : 0; } }
        public override int Hides { get { return Body != 104 ? 20 : 0; } }
        public override HideType HideType { get { return HideType.Barbed; } }
        public override int Scales { get { return Body != 104 ? 9 : 0; } }
        public override ScaleType ScaleType { get { return ScaleType.White; } }
        public override FoodType FavoriteFood { get { return FoodType.Meat | FoodType.Gold; } }
        public override bool CanAngerOnTame { get { return true; } }

        public override int DefaultBloodHue { get { return -2; } } //Template
        public override int BloodHueTemplate { get { return Body != 104 ? Utility.RandomGreyHue() : -1; } }

        public WhiteWyrm(Serial serial) : base(serial)
        {}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int) 0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}