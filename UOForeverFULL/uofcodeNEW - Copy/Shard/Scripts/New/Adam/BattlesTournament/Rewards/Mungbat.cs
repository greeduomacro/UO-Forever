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
    [CorpseName("a mungbat corpse")]
    public class Mungbat : BaseCreature
    {
        public override int Meat { get { return 1; } }
        public override int Hides { get { return 1; } }
        public override FoodType FavoriteFood { get { return FoodType.Meat; } }
        public DateTime NextSpeech;

        [Constructable]
        public Mungbat()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
			Name = NameList.RandomName("savage shaman");
            Body = 39;
            BaseSoundID = 422;
            SpecialTitle = "Mungbat";
            TitleHue = 1272;

            Tamable = true;

            SetStr(6, 10);
            SetDex(26, 38);
            SetInt(6, 14);

            SetHits(4, 6);
            SetMana(0);

            SetDamage(1, 2);

            SetSkill(SkillName.MagicResist, 5.1, 14.0);
            SetSkill(SkillName.Tactics, 5.1, 10.0);
            SetSkill(SkillName.Wrestling, 5.1, 10.0);
            SetSkill(SkillName.Magery, 10.1, 20.0);

            Fame = 150;
            Karma = -150;

            VirtualArmor = 30;

            SpeechHue = YellHue = 137;

            ControlSlots = 2;
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

            if (!e.Handled && m.InRange(Location, 2) && DateTime.UtcNow >= NextSpeech)
            {
                var rnd = Utility.Random(0, 4);

                switch (rnd)
                {
                    case 0:
                    {
                        Say("Mungbat.");
                        break;
                    }
                    case 1:
                    {
                        Say("Mungbat?");
                        break;
                    }
                    case 2:
                    {
                        Say("Mungbat? Mungbat.");
                        break;
                    }
                    case 3:
                    {
                        Say("MUUUUNNNGGGBAAATTT");
                        break;
                    }
                }

                NextSpeech = DateTime.UtcNow + TimeSpan.FromSeconds(10);
            }

            base.OnSpeech(e);
        }

        public Mungbat(Serial serial)
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