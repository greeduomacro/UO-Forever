#region References

using Server.Engines.Craft;
using Server.Items;

#endregion

namespace Server.Mobiles
{
    [CorpseName("a vitriolic corpse")]
    public class Vitriol : BaseCreature
    {
        public override string DefaultName { get { return "a demon"; } }

        [Constructable]
        public Vitriol() : base(AIType.AI_Arcade, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "an abomination";

            Body = 256;
            BaseSoundID = 0x2b8;

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


            CurrentSpeed = 0.6;
            PassiveSpeed = 0.6;
            ActiveSpeed = 0.16;
            RangePerception = 20;

            FreelyLootable = true;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 1);
        }

        public override void OnDeath(Container c)
        {
            if (Utility.RandomDouble() < 0.2)
            {
                c.DropItem(new VialofVitriol());
            }
            Engines.ZombieEvent.ZombieEvent.AddItem(c);
            base.OnDeath(c);
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

        public override bool CheckFlee()
        {
            return false;
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

        public Vitriol(Serial serial)
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