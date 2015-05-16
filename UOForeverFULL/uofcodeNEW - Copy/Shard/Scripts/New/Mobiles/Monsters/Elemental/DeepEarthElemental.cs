#region References

using Server.Items;

#endregion

namespace Server.Mobiles
{
    [CorpseName("a deep earth elemental corpse")]
    public class DeepEarthElemental : BaseCreature
    {
        public override double DispelDifficulty { get { return 117.5; } }
        public override double DispelFocus { get { return 45.0; } }
        public override string DefaultName { get { return "a deep earth elemental"; } }

        [Constructable]
        public DeepEarthElemental() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Body = 14;
            Hue = Utility.Random(1873, 18);
            BaseSoundID = 268;

            Alignment = Alignment.Elemental;

            SetStr(186, 201);
            SetDex(91, 105);
            SetInt(89, 106);

            SetHits(223, 281);

            SetDamage(11, 18);


            SetSkill(SkillName.MagicResist, 80.1, 125.0);
            SetSkill(SkillName.Tactics, 70.1, 118.0);
            SetSkill(SkillName.Wrestling, 90.1, 120.0);

            Fame = 6500;
            Karma = -6500;

            VirtualArmor = 50;
            ControlSlots = 3;

            PackItem(new FertileDirt(Utility.Random(3, 3)));
            PackItem(new IronOre(6)); // TODO: Five small iron ore
            PackItem(new MandrakeRoot());
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.Average, 2);
            AddLoot(LootPack.Meager, 2);
            AddLoot(LootPack.Gems);
        }

        public override bool BleedImmune { get { return true; } }
        public override int TreasureMapLevel { get { return 2; } }
        public override int DefaultBloodHue { get { return -1; } }

        public DeepEarthElemental(Serial serial) : base(serial)
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