#region References

using Server.Items;

#endregion

namespace Server.Mobiles
{
    [CorpseName("a water elemental corpse")]
    public class DeepWaterElemental : BaseCreature
    {
        public override string DefaultName { get { return "a deep water elemental"; } }

        [Constructable]
        public DeepWaterElemental() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Body = 16;
            BaseSoundID = 278;
            Hue = 1366;

            Alignment = Alignment.Elemental;

            SetStr(358, 405);
            SetDex(112, 147);
            SetInt(99, 125);

            SetHits(498, 556);

            SetDamage(20, 23);


            SetSkill(SkillName.EvalInt, 100.1);
            SetSkill(SkillName.Magery, 100.1, 110.0);
            SetSkill(SkillName.MagicResist, 120.0, 120.0);
            SetSkill(SkillName.Tactics, 99.9, 105.0);
            SetSkill(SkillName.Wrestling, 100.0);

            Fame = 7500;
            Karma = -7500;

            VirtualArmor = 36;
            CanSwim = true;

            PackItem(new BlackPearl(6));
            //PackItem( new Gold( 100, 200 ) );
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich, 2);
            AddLoot(LootPack.FilthyRich);
            AddLoot(LootPack.Potions);
        }

        //	public override bool BardImmune{ get{ return true; } }
        public override Poison PoisonImmune { get { return Poison.Greater; } }
        public override int TreasureMapLevel { get { return 4; } }
        public override int DefaultBloodHue { get { return -1; } }

        public DeepWaterElemental(Serial serial) : base(serial)
        {}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt((int) 0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}