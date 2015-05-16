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
    [CorpseName("a rune beetle corpse")]
    public class RuneBeetle : BaseCreature
    {
        public override WeaponAbility GetWeaponAbility()
        {
            return WeaponAbility.BleedAttack;
        }

        public override string DefaultName
        {
            get { return "a rune beetle"; }
        }

        [Constructable]
        public RuneBeetle()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Body = 244;

            SetStr(481, 520);
            SetDex(141, 190);
            SetInt(476, 550);

            SetHits(401, 560);

            SetDamage(19, 27);

            SetSkill(SkillName.EvalInt, 100.1, 125.0);
            SetSkill(SkillName.Magery, 100.1, 110.0);
            SetSkill(SkillName.Poisoning, 120.1, 140.0);
            SetSkill(SkillName.MagicResist, 95.1, 110.0);
            SetSkill(SkillName.Tactics, 78.1, 93.0);
            SetSkill(SkillName.Wrestling, 70.1, 77.5);

            Fame = 15000;
            Karma = -15000;

            VirtualArmor = 65;

            if (Utility.RandomDouble() < .25)
                PackItem(Seed.RandomBonsaiSeed());

            switch (Utility.Random(10))
            {
                case 0:
                    PackItem(new LeftArm());
                    break;
                case 1:
                    PackItem(new RightArm());
                    break;
                case 2:
                    PackItem(new Torso());
                    break;
                case 3:
                    PackItem(new Bone());
                    break;
                case 4:
                    PackItem(new RibCage());
                    break;
                case 5:
                    PackItem(new RibCage());
                    break;
                case 6:
                    PackItem(new BonePile());
                    break;
                case 7:
                    PackItem(new BonePile());
                    break;
                case 8:
                    PackItem(new BonePile());
                    break;
                case 9:
                    PackItem(new BonePile());
                    break;
            }

            Tamable = true;
            ControlSlots = 3;
            MinTameSkill = 110.0;
        }

        public override bool ReacquireOnMovement
        {
            get { return !Controlled; }
        }

        public override bool AutoDispel
        {
            get { return !Controlled; }
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

        public override Poison HitPoison
        {
            get { return Poison.Lethal; }
        }

        public override FoodType FavoriteFood
        {
            get { return FoodType.FruitsAndVeggies | FoodType.GrainsAndHay; }
        }

        public RuneBeetle(Serial serial)
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

            if (MinTameSkill > 110.0)
            {
                MinTameSkill = 110.0;
            }
        }
    }
}