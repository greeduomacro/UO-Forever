#region References

using Server.Items;

#endregion

namespace Server.Mobiles
{
    [CorpseName("a gargoyle corpse")]
    public class StoneGargoyle : BaseCreature
    {
        public override string DefaultName { get { return "a stone gargoyle"; } }

        [Constructable]
        public StoneGargoyle() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Body = 67;
            BaseSoundID = 0x174;

            Alignment = Alignment.Inhuman;

            Hue = 2407;

            SpecialTitle = "Stone Construct";
            TitleHue = 891;

            SetStr(246, 275);
            SetDex(76, 95);
            SetInt(81, 105);

            SetHits(148, 165);

            SetDamage(11, 17);

            SetSkill(SkillName.MagicResist, 85.1, 100.0);
            SetSkill(SkillName.Tactics, 80.1, 100.0);
            SetSkill(SkillName.Wrestling, 60.1, 100.0);

            Fame = 4000;
            Karma = -4000;

            VirtualArmor = 50;

            PackItem(new IronOre(Utility.RandomMinMax(10, 32)));

            if (0.05 > Utility.RandomDouble())
            {
                PackItem(new GargoylesPickaxe());
            }
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average, 2);
            AddLoot(LootPack.Gems, 1);
            AddLoot(LootPack.Potions);
            AddPackedLoot(LootPack.AverageProvisions, typeof(Bag));
            AddPackedLoot(LootPack.AverageProvisions, typeof(Pouch));
        }

        public override bool OnBeforeDeath()
        {
            if (0.1 >= Utility.RandomDouble())
            {
                PackItem(new StoneEye());
            }
            return base.OnBeforeDeath();
        }

        public override int TreasureMapLevel { get { return 2; } }

        public StoneGargoyle(Serial serial) : base(serial)
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