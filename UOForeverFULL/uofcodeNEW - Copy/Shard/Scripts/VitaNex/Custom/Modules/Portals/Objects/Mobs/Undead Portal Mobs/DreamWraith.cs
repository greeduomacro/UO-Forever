#region References

using Server.Items;

#endregion

namespace Server.Mobiles
{
    [CorpseName("a dream wraith corpse")]
    public class DreamWraithPortal : BaseCreature
    {
        [Constructable]
        public DreamWraithPortal()
            : base(AIType.AI_Mage, FightMode.Closest, 14, 1, 0.16, 0.4)
        {
            Name = "a Dream Wraith";
            Body = 740;
            //Hue = 0;
            BaseSoundID = 0x482;

            Alignment = Alignment.Undead;

            SetStr(200, 300);
            SetDex(100, 200);
            SetInt(181, 290);

            SetHits(600);

            SetDamage(18, 25);

            SetSkill(SkillName.Anatomy, 0.0, 10.0);
            SetSkill(SkillName.EvalInt, 100.0, 120.0);
            SetSkill(SkillName.Magery, 100.0, 120.0);
            SetSkill(SkillName.Meditation, 100.0, 110.0);
            SetSkill(SkillName.MagicResist, 120.0, 150.0);
            SetSkill(SkillName.Tactics, 70.0, 80.0);
            SetSkill(SkillName.Wrestling, 90.0, 100.0);

            Fame = 4000;
            Karma = -4000;

            VirtualArmor = 35;

            PackReg(10);
        }

        public DreamWraithPortal(Serial serial)
            : base(serial)
        {}

        public override bool BleedImmune { get { return true; } }
        public override OppositionGroup OppositionGroup { get { return OppositionGroup.FeyAndUndead; } }
        public override Poison PoisonImmune { get { return Poison.Lethal; } }

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