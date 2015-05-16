#region References

using Server.Items;

#endregion

namespace Server.Mobiles
{
    [CorpseName("a stone harpy corpse")]
    public class StoneHarpy : BaseCreature
    {
        public override string DefaultName { get { return "a stone harpy"; } }

        [Constructable]
        public StoneHarpy() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Body = 73;
            BaseSoundID = 402;

            Alignment = Alignment.Inhuman;

            Hue = 2407;

            SetStr(296, 320);
            SetDex(86, 110);
            SetInt(51, 75);

            SetHits(178, 192);
            SetMana(0);

            SetDamage(8, 16);

            SpecialTitle = "Stone Construct";
            TitleHue = 891;

            SetSkill(SkillName.MagicResist, 50.1, 65.0);
            SetSkill(SkillName.Tactics, 70.1, 100.0);
            SetSkill(SkillName.Wrestling, 70.1, 100.0);

            Fame = 4500;
            Karma = -4500;

            VirtualArmor = 50;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average, 2);
            AddLoot(LootPack.Gems, 2);
        }

        public override int GetAttackSound()
        {
            return 916;
        }

        public override int GetAngerSound()
        {
            return 916;
        }

        public override int GetDeathSound()
        {
            return 917;
        }

        public override int GetHurtSound()
        {
            return 919;
        }

        public override int GetIdleSound()
        {
            return 918;
        }

        public override bool OnBeforeDeath()
        {
            if (0.1 >= Utility.RandomDouble())
            {
                PackItem(new StoneFeather());
            }

            return base.OnBeforeDeath();
        }

        public override int Meat { get { return 1; } }
        public override int Feathers { get { return 50; } }
        public override int DefaultBloodHue { get { return -1; } }

        public StoneHarpy(Serial serial) : base(serial)
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