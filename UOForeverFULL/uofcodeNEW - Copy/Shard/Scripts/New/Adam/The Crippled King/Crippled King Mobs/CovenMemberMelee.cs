#region References

using System;
using Server.Items;

#endregion

namespace Server.Mobiles
{
    [CorpseName("a coven berserker corpse")]
    public class CovenMemberMelee : CovenMember
    {
        private static readonly SkillName[] _Skills = ((SkillName) 0).GetValues<SkillName>();

        public override string DefaultName { get { return "a coven berserker"; } }
        public override bool CanRummageCorpses { get { return true; } }
        public override bool AlwaysMurderer { get { return true; } }
        public override bool CanDestroyObstacles { get { return true; } }
        public override bool CanFlee { get { return false; } }
        public override bool Unprovokable { get { return true; } }

        [Constructable]
        public CovenMemberMelee()
            : base()
        {
            Name = NameList.RandomName("ethereal warrior");
            Body = 400;
            Hue = 400;
            SpecialTitle = "Blackthorn's Coven";
            TitleHue = 1102;

            AI = AIType.AI_Berserk;

            SpeechHue = YellHue = 34;

            Title = "the Berserker";

            SetStr(767, 945);
            SetDex(66, 75);
            SetInt(46, 70);

            SetHits(476, 552);

            SetDamage(20, 25);

            foreach (SkillName skill in _Skills)
            {
                SetSkill(skill, 80.0, 110);
            }

            VirtualArmor = 36;
            Fame = 5000;
            Karma = -10000;

            GiveEquipment();
        }

        public CovenMemberMelee(Serial serial)
            : base(serial)
        {}

        public override void GiveEquipment()
        {
            var machete = new BoneMachete();
            machete.Name = "a chaos blade";
            machete.Hue = 1920;
            machete.Identified = true;
            machete.Movable = false;
            AddItem(Immovable(machete));

            var shield = new ChaosShield();
            shield.Name = "a corrupted chaos shield";
            shield.Hue = 1920;
            shield.Identified = true;
            shield.Movable = false;
            AddItem(Immovable(shield));

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