#region References

using Server.Engines.Craft;
using Server.Items;

#endregion

namespace Server.Mobiles
{
    [CorpseName("a dragon corpse")]
    public class DragonPortal : BaseCreature
    {
        public override string DefaultName { get { return "a dragon"; } }

        [Constructable]
        public DragonPortal() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Body = Utility.RandomList(12, 59);

            Alignment = Alignment.Dragon;

            if (Body == 12)
            {
                int random = Utility.Random(200);

                if (random == 0)
                {
                    Hue = 0x4001;
                }
                else if (random <= 4)
                {
                    Hue = Utility.RandomList(1445, 1436, 2006, 2001);
                }
            }

            BaseSoundID = 362;

            SetStr(796, 825);
            SetDex(86, 105);
            SetInt(436, 475);

            SetHits(478, 495);

            SetDamage(16, 22);


            SetSkill(SkillName.EvalInt, 30.1, 40.0);
            SetSkill(SkillName.Magery, 30.1, 40.0);
            SetSkill(SkillName.MagicResist, 99.1, 100.0);
            SetSkill(SkillName.Tactics, 97.6, 100.0);
            SetSkill(SkillName.Wrestling, 90.1, 93.2);

            Fame = 15000;
            Karma = -15000;

            VirtualArmor = 75;

            Tamable = true;
            ControlSlots = 3;
            MinTameSkill = 93.9;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 2);
            AddLoot(LootPack.Gems, 8);

            if (0.001 > Utility.RandomDouble()) // 2 percent - multipy number x 100 to get percent
            {
                switch (Utility.Random(2))
                {
                        // rolls and number it gives it a case. if the number is , say , 3 it will pack that item

                    case 0:
                        PackItem(new LeatherDyeTub());
                        break;
                    case 1:
                        PackItem(new DragonHead());
                        break;
                }
            }
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

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            c.DropItem(new Platinum { Amount = 3 });
            c.DropItem(new GargoyleRune());
            if (Utility.RandomDouble() < 0.5)
            {
                c.DropItem(new GargoyleRune());
            }
            if (Utility.RandomDouble() < 0.1)
            {
                c.DropItem(new GargoyleRune());
            }
        }

        public override bool ReacquireOnMovement { get { return !Controlled; } }
        public override bool HasBreath { get { return true; } } // fire breath enabled
        public override int BreathFireDamage { get { return Body != 104 ? 100 : 0; } }
        public override int BreathColdDamage { get { return Body == 104 ? 100 : 0; } }
        public override bool AutoDispel { get { return !Controlled; } }
        public override int TreasureMapLevel { get { return 4; } }
        public override int Meat { get { return Body != 104 ? 19 : 0; } }
        public override int Hides { get { return Body != 104 ? 25 : 0; } }
        public override HideType HideType { get { return HideType.Barbed; } }
        public override int Scales { get { return Body != 104 ? 7 : 0; } }

        public override ScaleType ScaleType
        {
            get { return Hue == 0 ? (Body == 12 ? ScaleType.Yellow : ScaleType.Red) : ScaleType.Green; }
        }

        public override FoodType FavoriteFood { get { return FoodType.Meat; } }
        public override bool CanAngerOnTame { get { return true; } }

        public override int BreathEffectHue { get { return Body != 104 ? base.BreathEffectHue : 0x480; } }

        public override int DefaultBloodHue { get { return -2; } } //Template
        public override int BloodHueTemplate { get { return Body != 104 ? Utility.RandomGreyHue() : -1; } }

        public DragonPortal(Serial serial)
            : base(serial)
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