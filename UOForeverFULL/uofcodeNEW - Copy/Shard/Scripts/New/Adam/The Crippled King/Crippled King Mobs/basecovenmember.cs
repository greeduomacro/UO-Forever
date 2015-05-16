#region References

using System;
using Server.Items;

#endregion

namespace Server.Mobiles
{
    [CorpseName("a coven member corpse")]
    public class CovenMember : BaseCreature
    {
        private static readonly SkillName[] _Skills = ((SkillName) 0).GetValues<SkillName>();

        public override string DefaultName { get { return "a coven member"; } }
        public override bool CanRummageCorpses { get { return true; } }
        public override bool AlwaysMurderer { get { return true; } }
        public override bool CanDestroyObstacles { get { return true; } }
        public override bool CanFlee { get { return false; } }
        public override bool Unprovokable { get { return true; } }

        [Constructable]
        public CovenMember()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = NameList.RandomName("ethereal warrior");
            Body = 400;
            Hue = 400;
            SpecialTitle = "Blackthorn's Coven";
            TitleHue = 1102;

            SpeechHue = YellHue = 34;

            foreach (SkillName skill in _Skills)
            {
                SetSkill(skill, 80.0, 110);
            }

            VirtualArmor = 36;
            Fame = 10000;
            Karma = -10000;

            GiveEquipment();
        }

        public CovenMember(Serial serial)
            : base(serial)
        {}

        public virtual void GiveEquipment()
        {
            var shroud = new HoodedShroudOfShadows();
            shroud.Name = "a chaos shroud";
            shroud.Hue = 1920;
            shroud.Identified = true;
            shroud.Movable = false;
            AddItem(Immovable(shroud));
        }

        public override bool OnBeforeDeath()
        {
            Frozen = true;
            Yell(Utility.RandomBool() ? "The Dark Lord welcomes me fool..." : "Why..?");
            AddLoot(LootPack.FilthyRich, 2);
            AddLoot(LootPack.Rich, 2);
            AddLoot(LootPack.Average, 2);
            AddLoot(LootPack.Gems, 5);
            PackItem(new Gold(1400, 1900));

            if (0.05 >= Utility.RandomDouble())
            {
                var scroll = new SkillScroll();
                scroll.Randomize();
                PackItem(scroll);
            }
            if (0.1 >= Utility.RandomDouble())
            {
                PackItem(new covenpaint());
            }

            if (0.5 >= Utility.RandomDouble())
            {
                PackItem(new ChaosScroll());
            }

            return base.OnBeforeDeath();
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