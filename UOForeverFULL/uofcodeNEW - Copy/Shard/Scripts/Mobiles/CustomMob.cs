using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Items;

namespace Server.Mobiles
{    
    [CorpseName("a corpse")]
    public class CustomMob : BaseCreature
    {
        [Constructable]
        public CustomMob()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.25, 0.8)
        {
            SpeechHue = Utility.RandomDyedHue();
            Name = "a human";
            
            if (Utility.RandomBool())
            {
                this.Body = 400;
                this.Female = true;
            }
            else
            {
                this.Body = 401;
            }

            Utility.AssignRandomHair(this, 2155);

            SetStr(100, 100);
            SetDex(70, 70);
            SetInt(100, 100);

            HitsMaxSeed = 100;
            Hits = 100;
            StamMaxSeed = 70;
            Stam = 70;
            ManaMaxSeed = 100;
            Mana = 100;

            SetSkill(SkillName.Anatomy, 100.0);
            SetSkill(SkillName.Tactics, 100.0);
            SetSkill(SkillName.MagicResist, 50.0);
            SetSkill(SkillName.Wrestling, 80.0);
            DamageMin = 20;
            DamageMax = 30;

            VirtualArmor = 0;
            TakesNormalDamage = true;
            Pseu_AllowInterrupts = false;
            RangePerception = 18;
            ReduceSpeedWithDamageCustom = false; // normally true

            Pseu_SpellDelay = TimeSpan.Zero;
            ClearHandsOnCast = false;
            PowerWords = false;

            Fame = 3500;
            Karma = -3500;
        }

        public CustomMob(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version 
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}
