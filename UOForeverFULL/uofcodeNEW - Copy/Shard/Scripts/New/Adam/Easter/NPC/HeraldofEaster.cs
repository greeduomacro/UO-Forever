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
    public class HeraldofEaster : SpiritofEaster
    {
        private static readonly SkillName[] _Skills = ((SkillName) 0).GetValues<SkillName>();

        [Constructable]
        public HeraldofEaster()
        {
			Name = NameList.RandomName("pixie");
            Body = 205;
            Hue = 3 + (Utility.Random(20) * 5);
            SpecialTitle = "Herald of Easter";
            TitleHue = 1166;

            Blessed = true;

            CantWalk = true;

            SpeechHue = YellHue = 34;

            foreach (SkillName skill in _Skills)
            {
                SetSkill(skill, 80.0, 110);
            }

            VirtualArmor = 36;
        }

        public HeraldofEaster(Serial serial)
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