using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Items;

namespace Server.Mobiles
{    
    [CorpseName("a corpse")]
    public class HumanMob : BaseCreature
    {
        private int m_OwningSerial = -1;
        public int OwningSerial { get { return m_OwningSerial; } set { m_OwningSerial = value; } }

        [Constructable]
        public HumanMob()
            : base(AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.25, 0.8)
        {
            SpeechHue = Utility.RandomDyedHue();
            Name = "a human";
            Hue = Utility.RandomSkinHue();
            if (Utility.RandomBool())
            {
                this.Body = 0x191;
                this.Female = true;
            }
            else
            {
                this.Body = 0x190;
            }

            Utility.AssignRandomHair(this, Utility.RandomHairHue());

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
            BardImmuneCustom = true;
            WeaponDamage = true;
            TakesNormalDamage = true;
            Pseu_AllowInterrupts = true;

            Pseu_SpellDelay = TimeSpan.Zero;
            ClearHandsOnCast = true;
            PowerWords = true;

            Fame = 0;
            Karma = 0; 
        }

        public HumanMob(Serial serial)
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
