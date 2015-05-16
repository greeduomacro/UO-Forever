using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Items;

namespace Server.Mobiles
{    
    [CorpseName( "an orcish corpse" )]
    public class BloodRockOrc : BaseCreature
    {
        private int m_OwningSerial = -1;
        public int OwningSerial { get { return m_OwningSerial; } set { m_OwningSerial = value; } }

        [Constructable]
        public BloodRockOrc()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.25, 0.8)
        {
            SpeechHue = Utility.RandomDyedHue();
            Name = "a bloodclan orc";

            Alignment = Alignment.Orc;

            Body = 17;
			BaseSoundID = 0x45A;
            Hue = 1636;
            
            SetStr(100, 100);
            SetDex(25, 25);
            SetInt(25, 25);

            HitsMaxSeed = 100;
            Hits = 100;
            StamMaxSeed = 25;
            Stam = 25;
            ManaMaxSeed = 25;
            Mana = 25;

            VirtualArmor = 0;
            //BardImmuneCustom = true;
            WeaponDamage = true;
            TakesNormalDamage = true;
            Pseu_AllowInterrupts = true;

            Pseu_SpellDelay = TimeSpan.Zero;
            ClearHandsOnCast = true;
            PowerWords = true;

            Fame = 0;
            Karma = 0;
            
        }

        public BloodRockOrc(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version 
            writer.Write((int)m_OwningSerial);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version >= 1)
            {
                m_OwningSerial = reader.ReadInt();
            }
        }
    }
}
