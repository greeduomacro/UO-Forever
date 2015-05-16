/////////////////////////////////////////
//			                  //
//      Scripted by Raelis             //
//Re-edited for 2.0 by Timeinabottle   //
//		             	       //
////////////////////////////////////////

#region References

using System;

#endregion

namespace Server.Mobiles
{
    [CorpseName("a grotesque corpse")]
    public class PowerfulDemon : BaseCreature
    {
        private static readonly SkillName[] _Skills = ((SkillName) 0).GetValues<SkillName>();

        public override bool AlwaysMurderer { get { return true; } }
        public override bool CanDestroyObstacles { get { return true; } }
        public override bool CanFlee { get { return true; } }
        public override bool Unprovokable { get { return true; } }

        [Constructable]
        public PowerfulDemon()
            : base(AIType.AI_Berserk, FightMode.Closest, 10, 10, 0.001, 0.01)
        {
            Name = "Horrific Beast";
            Body = 785;

            SetStr(767, 945);
            SetDex(90, 185);
            SetInt(706, 726);

            SetHits(1000, 1250);

            SetDamage(20, 25);

            foreach (SkillName skill in _Skills)
            {
                SetSkill(skill, 80.0, 90.0);
            }

            VirtualArmor = 90;

            PackGold(3000, 3500);
            PackMagicItems(5, 5, 0.95, 0.95);
            PackMagicItems(5, 5, 0.80, 0.65);
            PackMagicItems(5, 5, 0.80, 0.65);
            PackMagicItems(6, 6, 0.80, 0.65);
        }

        public PowerfulDemon(Serial serial)
            : base(serial)
        {}

        public override int GetIdleSound()
        {
            return 0x2D3;
        }

        public override int GetHurtSound()
        {
            return 0x2D1;
        }


        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int) 0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}