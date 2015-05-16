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
    [CorpseName("a putrid corpse")]
    public class PutridHorse : Horse
    {
        public override int Meat { get { return 1; } }
        public override int Hides { get { return 1; } }
        public override FoodType FavoriteFood { get { return FoodType.Meat; } }
        public DateTime NextSpeech;
        public DateTime NextSlime;

        [Constructable]
        public PutridHorse()
        {
            Name = "putrid horse";
            Hue = 61;


			SetStr( 22, 98 );
			SetDex( 56, 75 );
			SetInt( 6, 10 );

			SetHits( 28, 45 );
			SetMana( 0 );

			SetDamage( 3, 4 );

			

            SetSkill(SkillName.MagicResist, 5.1, 14.0);
            SetSkill(SkillName.Tactics, 5.1, 10.0);
            SetSkill(SkillName.Wrestling, 5.1, 10.0);

            Fame = 150;
            Karma = -150;

            VirtualArmor = 30;

            SpeechHue = YellHue = 61;

			Tamable = true;
			ControlSlots = 1;
			MinTameSkill = 29.1;
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

            if (!e.Handled && m.InRange(Location, 2) && DateTime.UtcNow >= NextSpeech && Alive)
            {
                Say("Nay.");
                NextSpeech = DateTime.UtcNow + TimeSpan.FromMinutes(5);
            }

            base.OnSpeech(e);
        }

        public PutridHorse(Serial serial)
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