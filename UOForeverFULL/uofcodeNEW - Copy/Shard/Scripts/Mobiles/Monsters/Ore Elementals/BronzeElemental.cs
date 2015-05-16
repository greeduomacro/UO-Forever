#region References

using Server.Items;

#endregion

namespace Server.Mobiles
{
    [CorpseName("an ore elemental corpse")]
    public class BronzeElemental : BaseCreature
    {
        public override string DefaultName { get { return "a bronze elemental"; } }
        private static readonly int m_DefaultHue = CraftResources.GetInfo(CraftResource.Bronze).Hue;

        [Constructable]
        public BronzeElemental() : this(2)
        {}

        [Constructable]
        public BronzeElemental(int oreAmount) : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            // TODO: Gas attack
            Body = 108;
            BaseSoundID = 268;
            Hue = m_DefaultHue;
            //Hue = Utility.RandomList( 2967, 0 );

            Alignment = Alignment.Elemental;

            SetStr(226, 255);
            SetDex(126, 145);
            SetInt(71, 92);

            SetHits(136, 153);

            SetDamage(9, 16);


            SetSkill(SkillName.MagicResist, 50.1, 95.0);
            SetSkill(SkillName.Tactics, 60.1, 100.0);
            SetSkill(SkillName.Wrestling, 60.1, 100.0);

            Fame = 5000;
            Karma = -5000;

            VirtualArmor = 29;

            Item ore = new BronzeOre(oreAmount);
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

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);
            DoGasAttack(defender);
        }

        public override void OnGotMeleeAttack(Mobile attacker)
        {
            base.OnGotMeleeAttack(attacker);
            DoGasAttack(attacker);
        }

        public override void OnDamagedBySpell(Mobile from)
        {
            base.OnDamagedBySpell(from);
            DoGasAttack(from);
        }

        public override void OnGaveSpellDamage(Mobile defender)
        {
            base.OnGaveSpellDamage(defender);
            DoGasAttack(defender);
        }

        private void DoGasAttack(Mobile target)
        {
            if (Map == null || Map == Map.Internal)
            {
                return;
            }

            var bc = target as BaseCreature;

            if (bc != null && bc.BardProvoked)
            {
                return;
            }

            if (InRange(target, 1) && 0.20 > Utility.RandomDouble())
            {
                Animate(10, 4, 1, true, false, 0);

                DoHarmful(target);

                target.Damage(Utility.RandomMinMax(15, 20), this);

                target.FixedParticles(0x36BD, 1, 10, 0x1F78, 0xA6, 0, (EffectLayer) 255);
                target.ApplyPoison(this, Poison.Deadly);
            }
        }

        public BronzeElemental(Serial serial) : base(serial)
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