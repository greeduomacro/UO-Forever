#region References

using Server.Items;

#endregion

namespace Server.Mobiles
{
    [CorpseName("a devourer of souls corpse")]
    public class DevourerPortal : BaseCreature
    {
        public override string DefaultName { get { return "a devourer of souls"; } }

        public override bool BardImmune
        {
            get { return true; }
        }

        [Constructable]
        public DevourerPortal() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Body = 303;
            BaseSoundID = 357;

            SetStr(801, 950);
            SetDex(126, 175);
            SetInt(201, 250);

            SetHits(650);

            SetDamage(22, 26);


            Alignment = Alignment.Demon;


            SetSkill(SkillName.EvalInt, 90.1, 100.0);
            SetSkill(SkillName.Magery, 90.1, 100.0);
            SetSkill(SkillName.Meditation, 90.1, 100.0);
            SetSkill(SkillName.MagicResist, 90.1, 105.0);
            SetSkill(SkillName.Tactics, 75.1, 85.0);
            SetSkill(SkillName.Wrestling, 80.1, 100.0);

            Fame = 9500;
            Karma = -9500;

            VirtualArmor = 44;

            PackNecroReg(24, 45);
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 2);
        }

        public override Poison PoisonImmune { get { return Poison.Lethal; } }
        public override int Meat { get { return 3; } }
        public override int DefaultBloodHue { get { return -2; } }
        public override int BloodHueTemplate { get { return Utility.RandomGreenHue(); } }

        public DevourerPortal(Serial serial)
            : base(serial)
        {}

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            c.DropItem(new Platinum {Amount = 10});
            c.DropItem(new GargoyleRune());
            if (Utility.RandomDouble() < 0.5)
            {
                c.DropItem(new GargoyleRune());
            }
            if (Utility.RandomDouble() < 0.1)
            {
                c.DropItem(new GargoyleRune());
            }
        }

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