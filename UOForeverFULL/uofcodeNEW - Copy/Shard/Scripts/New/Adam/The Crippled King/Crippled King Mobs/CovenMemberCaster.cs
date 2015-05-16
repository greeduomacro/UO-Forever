#region References

using System;
using Server.Items;

#endregion

namespace Server.Mobiles
{
    [CorpseName("a coven defiler corpse")]
    public class CovenMemberCaster : CovenMember
    {
        private static readonly SkillName[] _Skills = ((SkillName) 0).GetValues<SkillName>();

        public override string DefaultName { get { return "a coven defiler"; } }
        public override bool CanRummageCorpses { get { return true; } }
        public override bool AlwaysMurderer { get { return true; } }
        public override bool CanDestroyObstacles { get { return true; } }
        public override bool CanFlee { get { return false; } }
        public override bool Unprovokable { get { return true; } }

        [Constructable]
        public CovenMemberCaster()
            : base()
        {
            Name = NameList.RandomName("ethereal warrior");
            Body = 400;
            Hue = 400;
            SpecialTitle = "Blackthorn's Coven";
            TitleHue = 1102;

            SpeechHue = YellHue = 34;

            Title = "the Defiler";

            SetStr(1254, 1381);
            SetDex(93, 135);
            SetInt(745, 810);

            SetHits(694, 875);

            SetDamage(12, 20);

            foreach (SkillName skill in _Skills)
            {
                SetSkill(skill, 80.0, 110);
            }

            VirtualArmor = 36;
            Fame = 10000;
            Karma = -10000;

            GiveEquipment();
        }

        public CovenMemberCaster(Serial serial)
            : base(serial)
        {}

        public override void GiveEquipment()
        {
            var spellbook = new Spellbook();
            spellbook.Name = "a book of chaos";
            spellbook.Hue = 1920;
            spellbook.Movable = false;
            AddItem(Immovable(spellbook));
            base.GiveEquipment();
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