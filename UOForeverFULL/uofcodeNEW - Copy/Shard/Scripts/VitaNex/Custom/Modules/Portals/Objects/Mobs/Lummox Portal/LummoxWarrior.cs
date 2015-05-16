#region References

using Server.Items;

#endregion

namespace Server.Mobiles
{
    [CorpseName("a lummox corpse")]
    public class LummoxWarriorPortal : BaseCreature
    {
        [Constructable]
        public LummoxWarriorPortal()
            : base(AIType.AI_Arcade, FightMode.Closest, 14, 1, 0.16, 0.4)
        {
            Name = "a lummox warrior";
            Body = 245;
            //Hue = 0;
            BaseSoundID = 1106;

            Alignment = Alignment.Inhuman;

            SetStr(600, 600);
            SetDex(277, 355);
            SetInt(151, 250);

            SetHits(600);

            SetDamage(37, 41);

            SetSkill(SkillName.Anatomy, 100.0, 100.0);
            SetSkill(SkillName.MagicResist, 120.0, 150.0);
            SetSkill(SkillName.Tactics, 70.0, 80.0);
            SetSkill(SkillName.Wrestling, 90.0, 100.0);

            Fame = 4000;
            Karma = -4000;

            VirtualArmor = 65;
        }

        public LummoxWarriorPortal(Serial serial)
            : base(serial)
        {}

        public override bool BleedImmune { get { return true; } }
        public override OppositionGroup OppositionGroup { get { return OppositionGroup.FeyAndUndead; } }
        public override Poison PoisonImmune { get { return Poison.Lethal; } }
        public override bool Unprovokable { get { return true; } }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            c.DropItem(new Platinum {Amount = 5});
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

        public override int GetIdleSound()
        {
            return 0x5F4;
        }

        public override int GetAngerSound()
        {
            return 0x5F1;
        }

        public override int GetDeathSound()
        {
            return 0x5F2;
        }

        public override int GetHurtSound()
        {
            return 0x5F3;
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