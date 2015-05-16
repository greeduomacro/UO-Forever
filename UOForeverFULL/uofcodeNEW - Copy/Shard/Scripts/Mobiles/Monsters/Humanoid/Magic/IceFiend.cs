#region References

using Server.Items;

#endregion

namespace Server.Mobiles
{
    [CorpseName("an ice fiend corpse")]
    public class IceFiend : BaseCreature
    {
        public override string DefaultName { get { return "an ice fiend"; } }

        [Constructable]
        public IceFiend() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Body = 43;
            BaseSoundID = 357;

            Alignment = Alignment.Demon;

            SetStr(376, 405);
            SetDex(176, 195);
            SetInt(201, 225);

            SetHits(276, 343);

            SetDamage(8, 19);

            SetSkill(SkillName.EvalInt, 80.1, 90.0);
            SetSkill(SkillName.Magery, 80.1, 90.0);
            SetSkill(SkillName.MagicResist, 75.1, 85.0);
            SetSkill(SkillName.Tactics, 80.1, 90.0);
            SetSkill(SkillName.Wrestling, 80.1, 100.0);


            Fame = 18000;
            Karma = -18000;

            VirtualArmor = 60;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich);
            AddLoot(LootPack.Average);
            AddLoot(LootPack.MedScrolls, 2);

            if (0.0009 > Utility.RandomDouble()) // 2 percent - multipy number x 100 to get percent
            {
                switch (Utility.Random(2))
                {
                    case 0:
                        PackItem(new PolarBearMask());
                        break;
                    case 1:
                        PackItem(new Ginseng(10));
                        break;
                }
            }
        }

        public override int TreasureMapLevel { get { return 4; } }
        public override int Meat { get { return 1; } }

        public IceFiend(Serial serial) : base(serial)
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

        public override bool HasAura { get { return true; } }
    }
}