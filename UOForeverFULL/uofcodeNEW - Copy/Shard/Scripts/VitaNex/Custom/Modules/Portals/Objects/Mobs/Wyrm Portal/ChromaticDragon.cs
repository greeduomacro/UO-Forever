// **********
// RunUO Shard - RuneBeetle.cs
// **********

#region References

using System;
using Server.Engines.Plants;
using Server.Items;

#endregion

namespace Server.Mobiles
{
    [CorpseName("a dragon corpse")]
    public class ChromaticDragonPortal : BaseCreature
    {
        [Constructable]
        public ChromaticDragonPortal()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Body = Utility.RandomList(12, 59);
            Name = "a chromatic dragon";
            Hue = 1281;

            Alignment = Alignment.Dragon;

            SetStr(481, 520);
            SetDex(141, 190);
            SetInt(476, 550);

            SetHits(401, 560);

            SetDamage(19, 27);

            SetSkill(SkillName.EvalInt, 100.0, 100.0);
            SetSkill(SkillName.Magery, 130.0, 130.0);
            SetSkill(SkillName.MagicResist, 95.1, 100.0);
            SetSkill(SkillName.Tactics, 78.1, 93.0);
            SetSkill(SkillName.Wrestling, 70.1, 77.5);
            SetSkill(SkillName.Anatomy, 70.1, 77.5);

            Fame = 15000;
            Karma = -15000;

            VirtualArmor = 65;

            BaseSoundID = 362;

            Tamable = true;
            ControlSlots = 3;
            MinTameSkill = 116.0;
        }

        public override bool CanAngerOnTame
        {
            get { return true; }
        }

        public override int GetAngerSound()
        {
            return 0x4E8;
        }

        public override int GetIdleSound()
        {
            return 0x4E7;
        }

        public override int GetAttackSound()
        {
            return 0x4E6;
        }

        public override int GetHurtSound()
        {
            return 0x4E9;
        }

        public override int GetDeathSound()
        {
            return 0x4E5;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 2);
            AddLoot(LootPack.MedScrolls, 1);
        }

        public override Poison PoisonImmune
        {
            get { return Poison.Greater; }
        }

        public override FoodType FavoriteFood { get { return FoodType.Meat; } }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            c.DropItem(new Platinum { Amount = 25 });
            c.DropItem(new GargoyleRune());
            if (Utility.RandomDouble() < 0.5)
            {
                c.DropItem(new GargoyleRune());
            }
            if (Utility.RandomDouble() < 0.1)
            {
                c.DropItem(new GargoyleRune());
            }
        }

        public override bool ReacquireOnMovement { get { return !Controlled; } }
        public override bool HasBreath { get { return true; } } // fire breath enabled
        public override int BreathFireDamage { get { return Body != 104 ? 100 : 0; } }
        public override int BreathColdDamage { get { return Body == 104 ? 100 : 0; } }
        public override bool AutoDispel { get { return !Controlled; } }
        public override int TreasureMapLevel { get { return 4; } }
        public override int Meat { get { return Body != 104 ? 19 : 0; } }
        public override int Hides { get { return Body != 104 ? 25 : 0; } }
        public override HideType HideType { get { return HideType.Barbed; } }
        public override int Scales { get { return Body != 104 ? 7 : 0; } }

        public override int BreathEffectHue { get { return 1280; } }

        public override int DefaultBloodHue { get { return -2; } } //Template
        public override int BloodHueTemplate { get { return 1281; } }

        public override ScaleType ScaleType
        {
            get { return Hue == 0 ? (Body == 12 ? ScaleType.Yellow : ScaleType.Red) : ScaleType.Green; }
        }

        public ChromaticDragonPortal(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(2); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

			if (version < 2)
			{
				VirtualArmor = 65;
			}

            if (version < 1)
            {
                for (int i = 0; i < Skills.Length; ++i)
                {
                    Skills[i].Cap = Math.Max(100.0, Skills[i].Cap*0.9);

                    if (Skills[i].Base > Skills[i].Cap)
                    {
                        Skills[i].Base = Skills[i].Cap;
                    }
                }
            }
        }
    }
}