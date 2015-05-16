#region References

using System;
using System.Collections.Generic;
using System.Linq;
using Server.Engines.CustomTitles;
using Server.Gumps;
using Server.Items;
using Server.Network;
using VitaNex.FX;

#endregion

namespace Server.Mobiles
{
    [CorpseName("Herald of Easter's corpse")]
    public class HeraldofEasterPet : BaseCreature
    {
        private static readonly SkillName[] _Skills = ((SkillName) 0).GetValues<SkillName>();

        public override int Meat { get { return 1; } }
        public override int Hides { get { return 1; } }
        public override FoodType FavoriteFood { get { return FoodType.FruitsAndVeggies; } }

        [Constructable]
        public HeraldofEasterPet()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
			Name = NameList.RandomName("pixie");
            Body = 205;
            Hue = 1166;
            SpecialTitle = "Herald of Easter";
            TitleHue = 1166;

            Tamable = true;

            SpeechHue = YellHue = 1166;

            foreach (SkillName skill in _Skills)
            {
                SetSkill(skill, 20.0, 50);
            }

            VirtualArmor = 36;
        }

        public override bool HandlesOnSpeech(Mobile from)
        {
            if (InRange(from, 2))
            {
                return true;
            }
            return base.HandlesOnSpeech(from);
        }

        public override void OnSpeech(SpeechEventArgs e)
        {
            Mobile m = e.Mobile;

            if (!e.Handled && m.InRange(Location, 2))
            {
                string speech = e.Speech.Trim().ToLower();

                if ((speech.IndexOf("no arms", System.StringComparison.OrdinalIgnoreCase) > -1))
                {
                    Say("It's just a flesh wound.");
                }
                else if ((speech.IndexOf("fart", System.StringComparison.OrdinalIgnoreCase) > -1))
                {
                    Say(Utility.RandomBool() ? "I fart in your general direction" : "Your mother was a hamster and your father smelt of elderberries");
                }
                else if ((speech.IndexOf("ni", System.StringComparison.OrdinalIgnoreCase) > -1))
                {
                    Say("We are the Knights who say... NI.");
                }
                else if ((speech.IndexOf("unladen swallow", System.StringComparison.OrdinalIgnoreCase) > -1))
                {
                    Say("What do you mean? An African or European swallow?");
                }
                else if ((speech.IndexOf("king", System.StringComparison.OrdinalIgnoreCase) > -1))
                {
                    Say("Must be a king, only one that hasn't got crap all over 'im");
                }
            }

            base.OnSpeech(e);
        }

        public HeraldofEasterPet(Serial serial)
            : base(serial)
        {}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int) 0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.GetVersion();

        }
    }
}