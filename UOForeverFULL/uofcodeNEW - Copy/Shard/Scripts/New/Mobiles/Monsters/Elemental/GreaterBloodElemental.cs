#region References

using Server.Items;

#endregion

namespace Server.Mobiles
{
    [CorpseName("a blood elemental corpse")]
    public class GreaterBloodElemental : BaseCreature
    {
        public override string DefaultName { get { return "a greater blood elemental"; } }

        [Constructable]
        public GreaterBloodElemental() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Body = 159;
            BaseSoundID = 278;

            Alignment = Alignment.Elemental;

            SetStr(590, 625);
            SetDex(66, 85);
            SetInt(226, 350);

            SetHits(500, 542);

            SetDamage(18, 28);


            SetSkill(SkillName.EvalInt, 85.1, 100.0);
            SetSkill(SkillName.Magery, 85.1, 100.0);
            SetSkill(SkillName.Meditation, 10.4, 50.0);
            SetSkill(SkillName.MagicResist, 80.1, 95.0);
            SetSkill(SkillName.Tactics, 80.1, 100.0);
            SetSkill(SkillName.Wrestling, 80.1, 100.0);

            Fame = 8000;
            Karma = -8000;

            VirtualArmor = 61;
            PackItem(new BloodCrystal(1));
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 2);
        }

        public override bool OnBeforeDeath()
        {
            if (1.00 > Utility.RandomDouble())
            {
                PackItem(new Gold(Utility.RandomMinMax(200, 600)));
            }

            if (0.06 > Utility.RandomDouble()) // 2 percent - multipy number x 100 to get percent
            {
                var scroll = new SkillScroll();
                scroll.Randomize();
                PackItem(scroll);
            }
            return base.OnBeforeDeath();
        }

        public override int TreasureMapLevel { get { return 5; } }

        public GreaterBloodElemental(Serial serial) : base(serial)
        {}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt((int) 0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }
}