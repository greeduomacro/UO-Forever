#region References

using Server.Items;
using Server.Misc;

#endregion

namespace Server.Mobiles
{
    [CorpseName("a glowing ratman corpse")]
    public class RatmanMage : BaseCreature
    {
        public override InhumanSpeech SpeechType { get { return InhumanSpeech.Ratman; } }

        [Constructable]
        public RatmanMage() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = NameList.RandomName("ratman");
            Body = 0x8F;
            BaseSoundID = 437;

            Alignment = Alignment.Inhuman;

            SetStr(146, 180);
            SetDex(101, 130);
            SetInt(186, 210);

            SetHits(88, 108);

            SetDamage(7, 14);


            SetSkill(SkillName.EvalInt, 70.1, 80.0);
            SetSkill(SkillName.Magery, 70.1, 80.0);
            SetSkill(SkillName.MagicResist, 65.1, 90.0);
            SetSkill(SkillName.Tactics, 50.1, 75.0);
            SetSkill(SkillName.Wrestling, 50.1, 75.0);

            Fame = 7500;
            Karma = -7500;

            VirtualArmor = 44;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.LowScrolls);
            if (0.02 >= Utility.RandomDouble())
            {
                var scroll = new SkillScroll();
                scroll.Randomize();
                PackItem(scroll);
            }
        }

        public override bool CanRummageCorpses { get { return true; } }
        public override int Meat { get { return 1; } }
        public override int Hides { get { return 8; } }
        public override HideType HideType { get { return HideType.Spined; } }

        public RatmanMage(Serial serial) : base(serial)
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

            if (Body == 42)
            {
                Body = 0x8F;
                Hue = 0;
            }
        }
    }
}