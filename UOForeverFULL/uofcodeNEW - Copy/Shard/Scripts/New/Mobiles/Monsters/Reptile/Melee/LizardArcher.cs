#region References

using Server.Items;

#endregion

namespace Server.Mobiles
{
    [CorpseName("a lizard archer corpse")]
    public class LizardArcher : BaseCreature
    {
        [Constructable]
        public LizardArcher() : base(AIType.AI_Archer, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = NameList.RandomName("lizardman");
            Body = Utility.RandomList(35, 36);
            BaseSoundID = 417;
            Hue = 49;

            Alignment = Alignment.Inhuman;

            SetStr(146, 180);
            SetDex(101, 130);
            SetInt(116, 140);

            SetHits(125, 150);

            SetDamage(4, 10);


            SetSkill(SkillName.Anatomy, 60.2, 100.0);
            SetSkill(SkillName.Archery, 80.1, 90.0);
            SetSkill(SkillName.MagicResist, 65.1, 90.0);
            SetSkill(SkillName.Tactics, 50.1, 75.0);
            SetSkill(SkillName.Wrestling, 50.1, 75.0);

            Fame = 3500;
            Karma = -3500;

            VirtualArmor = 35;

            AddItem(new Bow());
            PackItem(new Arrow(Utility.Random(10, 50)));
            PackGold(75, 125);
        }

        public override int Hides { get { return 8; } }
        public override HideType HideType { get { return HideType.Spined; } }

        public LizardArcher(Serial serial) : base(serial)
        {}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt((int) 0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }
}