#region References

using Server.Items;

#endregion

namespace Server.Mobiles
{
    [CorpseName("a ghostly corpse")]
    public class Ghoul : BaseCreature
    {
        public override string DefaultName { get { return "a ghoul"; } }

        [Constructable]
        public Ghoul() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Body = 153;
            BaseSoundID = 0x482;

            Alignment = Alignment.Undead;

            SetStr(76, 100);
            SetDex(76, 95);
            SetInt(36, 60);

            SetHits(46, 60);
            SetMana(0);

            SetDamage(7, 9);


            SetSkill(SkillName.MagicResist, 45.1, 60.0);
            SetSkill(SkillName.Tactics, 45.1, 60.0);
            SetSkill(SkillName.Wrestling, 45.1, 55.0);

            Fame = 2500;
            Karma = -2500;

            VirtualArmor = 28;

            PackItem(Loot.RandomWeapon());
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Meager);
        }

        public override bool OnBeforeDeath()
        {
            if (base.OnBeforeDeath())
            {
                if (!Summoned && !NoKillAwards && (Region != null && Region.IsPartOf("Skara Brae")))
                {
                    PackItem(new EtherealResidue());
                }

                return true;
            }

            return false;
        }

        public override bool BleedImmune { get { return true; } }
        public override Poison PoisonImmune { get { return Poison.Regular; } }
        public override int DefaultBloodHue { get { return -1; } }

        public Ghoul(Serial serial) : base(serial)
        {}

        public override OppositionGroup OppositionGroup { get { return OppositionGroup.FeyAndUndead; } }

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