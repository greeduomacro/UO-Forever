#region References

using Server.Engines.Plants;
using Server.Items;

#endregion

namespace Server.Mobiles
{
    [CorpseName("a titan corpse")]
    public class Titan : BaseCreature
    {
        public override string DefaultName { get { return "a titan"; } }

        [Constructable]
        public Titan()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Body = 76;
            BaseSoundID = 609;

            Alignment = Alignment.Giantkin;

            SetStr(536, 585);
            SetDex(126, 145);
            SetInt(281, 305);

            SetHits(322, 351);

            SetDamage(13, 16);

            SetSkill(SkillName.EvalInt, 85.1, 100.0);
            SetSkill(SkillName.Magery, 85.1, 100.0);
            SetSkill(SkillName.MagicResist, 80.2, 110.0);
            SetSkill(SkillName.Tactics, 60.1, 80.0);
            SetSkill(SkillName.Wrestling, 40.1, 50.0);

            Fame = 11500;
            Karma = -11500;

            VirtualArmor = 40;
        }

        public override bool OnBeforeDeath()
        {
            if (base.OnBeforeDeath())
            {
                if (EraML && Utility.RandomDouble() < 0.33)
                {
                    PackItem(Seed.RandomPeculiarSeed(1));
                }

                return true;
            }

            return false;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich);
            AddLoot(LootPack.Average);
            AddLoot(LootPack.MedScrolls);
            AddPackedLoot(LootPack.AverageProvisions, typeof(Bag));
            AddPackedLoot(LootPack.RichProvisions, typeof(Pouch));

            if (0.08 > Utility.RandomDouble()) // 2 percent - multipy number x 100 to get percent
            {
                var scroll = new SkillScroll();
                scroll.Randomize();
                PackItem(scroll);
            }
        }

        public override int Meat { get { return 4; } }
        public override Poison PoisonImmune { get { return Poison.Regular; } }
        public override int TreasureMapLevel { get { return 5; } }

        public Titan(Serial serial)
            : base(serial)
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