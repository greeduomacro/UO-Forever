#region References

using Server.Items;

#endregion

namespace Server.Mobiles
{
    [CorpseName("an iron elemental corpse")]
    public class IronElemental : BaseCreature
    {
        public override string DefaultName { get { return "an iron elemental"; } }

        [Constructable]
        public IronElemental() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Body = 14;
            BaseSoundID = 268;
            Hue = 1072;

            Alignment = Alignment.Elemental;

            SetStr(206, 234);
            SetDex(78, 100);
            SetInt(71, 92);

            SetHits(304, 371);

            SetDamage(12, 24);


            SetSkill(SkillName.MagicResist, 80.1, 110.0);
            SetSkill(SkillName.Tactics, 70.1, 105.0);
            SetSkill(SkillName.Wrestling, 85.1, 100.0);

            Fame = 7500;
            Karma = -7500;

            VirtualArmor = 85;

            PackItem(new IronIngot(15));
            PackItem(new BlackPearl());
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich, 2);
            AddLoot(LootPack.Gems);
        }

        public override Poison PoisonImmune { get { return Poison.Deadly; } }

        public override int TreasureMapLevel { get { return 3; } }
        //	public override bool BardImmune{ get{ return true; } }
        public override int DefaultBloodHue { get { return -1; } }

        public IronElemental(Serial serial) : base(serial)
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