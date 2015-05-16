#region References

using System.Collections.Generic;
using Server.Items;

#endregion

namespace Server.Mobiles
{
    [CorpseName("an ore elemental corpse")]
    public class DullCopperElemental : BaseCreature
    {
        public override string DefaultName { get { return "a dull copper elemental"; } }
        private static readonly int m_DefaultHue = CraftResources.GetInfo(CraftResource.DullCopper).Hue;

        [Constructable]
        public DullCopperElemental() : this(2)
        {}

        [Constructable]
        public DullCopperElemental(int oreAmount) : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Body = 110;
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

            Fame = 3500;
            Karma = -3500;

            VirtualArmor = 20;

            Item ore = new DullCopperOre(oreAmount);
            ore.ItemID = 0x19B9;
            PackItem(ore);
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average, 4);
            AddLoot(LootPack.Gems, 2);
        }

        public override bool AutoDispel { get { return true; } }
        public override bool BleedImmune { get { return true; } }
        public override Poison PoisonImmune { get { return Poison.Deadly; } }
        public override int DefaultBloodHue { get { return -1; } }

        public override bool OnBeforeDeath()
        {
            if (base.OnBeforeDeath())
            {
                FixedParticles(0x36BD, 20, 10, 5044, EffectLayer.Head);
                PlaySound(0x307);

                var toDamage = new List<Mobile>();

                foreach (Mobile m in GetMobilesInRange(3))
                {
                    var bc = m as BaseCreature;
                    if (m.Alive && !m.IsDeadBondedPet && (bc == null || bc.Controlled || bc.Summoned))
                    {
                        toDamage.Add(m);
                    }
                }

                for (int i = 0; i < toDamage.Count; i++) //20% damage
                {
                    toDamage[i].Damage(toDamage[i].HitsMax / 5, this);
                }

                return true;
            }

            return false;
        }

        public DullCopperElemental(Serial serial) : base(serial)
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