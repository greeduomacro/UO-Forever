#region References

using Server.Items;

#endregion

namespace Server.Mobiles
{
    [CorpseName("an ore elemental corpse")]
    public class CopperElemental : BaseCreature
    {
        public override string DefaultName { get { return "a copper elemental"; } }
        private static readonly int m_DefaultHue = CraftResources.GetInfo(CraftResource.Copper).Hue;

        [Constructable]
        public CopperElemental() : this(2)
        {}

        [Constructable]
        public CopperElemental(int oreAmount) : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Body = 109;
            BaseSoundID = 268;
            Hue = m_DefaultHue;

            Alignment = Alignment.Elemental;

            SetStr(226, 255);
            SetDex(126, 145);
            SetInt(71, 92);

            SetHits(136, 153);

            SetDamage(9, 16);


            SetSkill(SkillName.MagicResist, 50.1, 95.0);
            SetSkill(SkillName.Tactics, 60.1, 100.0);
            SetSkill(SkillName.Wrestling, 60.1, 100.0);

            Fame = 4800;
            Karma = -4800;

            VirtualArmor = 26;

            Item ore = new CopperOre(oreAmount);
            ore.ItemID = 0x19B9;
            PackItem(ore);
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);
            AddLoot(LootPack.Gems, 2);
        }

        public override bool BleedImmune { get { return true; } }
        public override Poison PoisonImmune { get { return Poison.Deadly; } }
        public override bool AutoDispel { get { return true; } }
        public override int TreasureMapLevel { get { return 1; } }
        public override int DefaultBloodHue { get { return -1; } }

        public override void CheckReflect(Mobile caster, ref bool reflect)
        {
            reflect = false; // Every spell is reflected back to the caster
        }

        public override void AlterMeleeDamageFrom(Mobile from, ref int damage)
        {
            base.AlterMeleeDamageFrom(from, ref damage);
            damage /= 2; // 50% melee damage
        }

        public CopperElemental(Serial serial) : base(serial)
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