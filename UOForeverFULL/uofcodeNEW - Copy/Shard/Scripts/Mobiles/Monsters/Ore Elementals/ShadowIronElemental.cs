#region References

using Server.Items;

#endregion

namespace Server.Mobiles
{
    [CorpseName("an ore elemental corpse")]
    public class ShadowIronElemental : BaseCreature
    {
        public override string DefaultName { get { return "a shadow iron elemental"; } }
        private static readonly int m_DefaultHue = CraftResources.GetInfo(CraftResource.ShadowIron).Hue;

        [Constructable]
        public ShadowIronElemental() : this(2)
        {}

        [Constructable]
        public ShadowIronElemental(int oreAmount) : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Body = 111;
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

            Fame = 4500;
            Karma = -4500;

            VirtualArmor = 23;

            Item ore = new ShadowIronOre(oreAmount);
            ore.ItemID = 0x19B9;
            PackItem(ore);
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);
            AddLoot(LootPack.Gems, 2);
        }

        public override bool AutoDispel { get { return true; } }
        public override bool BleedImmune { get { return true; } }
        public override int TreasureMapLevel { get { return 1; } }
        public override Poison PoisonImmune { get { return Poison.Deadly; } }
        //	public override bool Unprovokable{ get{ return true; } }
        public override bool BreathImmune { get { return true; } }
        public override int DefaultBloodHue { get { return -1; } }

        public override void AlterMeleeDamageFrom(Mobile from, ref int damage)
        {
            if (from is BaseCreature)
            {
                var bc = (BaseCreature) from;

                if (bc.Controlled || bc.BardTarget == this)
                {
                    damage = 0; // Immune to pets and provoked creatures
                }
            }
        }

        public override void AlterAbilityDamageFrom(Mobile from, ref int damage)
        {
            AlterMeleeDamageFrom(from, ref damage);
        }

        public override void AlterDamageScalarFrom(Mobile caster, ref double scalar)
        {
            scalar = 0.0; // Immune to magic
        }

        public ShadowIronElemental(Serial serial) : base(serial)
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