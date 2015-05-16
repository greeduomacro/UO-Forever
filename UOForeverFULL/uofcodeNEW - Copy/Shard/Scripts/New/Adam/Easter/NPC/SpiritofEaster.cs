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
    [CorpseName("Spirit of Easter's corpse")]
    public class SpiritofEaster : BaseCreature
    {
        private static readonly SkillName[] _Skills = ((SkillName) 0).GetValues<SkillName>();

        [Constructable]
        public SpiritofEaster()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Peter Cottontail";
            Body = 205;
            Hue = 1166;
            SpecialTitle = "Spirit of Easter";
            TitleHue = 1266;

            Blessed = true;

            CantWalk = true;

            SpeechHue = YellHue = 34;

            foreach (SkillName skill in _Skills)
            {
                SetSkill(skill, 80.0, 110);
            }

            VirtualArmor = 36;
        }

        public SpiritofEaster(Serial serial)
            : base(serial)
        {}

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);
            if (!(from is PlayerMobile))
                return;
            from.CloseGump(typeof(EasterRewardsUI));
            from.SendGump(new EasterRewardsUI(from as PlayerMobile));
        }

        public override bool HandlesOnSpeech(Mobile from)
        {
            if (InRange(from, 1))
            {
                return true;
            }
            return base.HandlesOnSpeech(from);
        }

        public override void OnSpeech(SpeechEventArgs e)
        {
            Mobile from = e.Mobile;

            if (!e.Handled && from.InRange(Location, 1))
            {
                if (!(from is PlayerMobile))
                    return;
                from.CloseGump(typeof(EasterRewardsUI));
                from.SendGump(new EasterRewardsUI(from as PlayerMobile));               
            }

            base.OnSpeech(e);
        }

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