using System;
using Server.Misc;
using Server.Network;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
    public class Khalnga : BaseCreature
    {
        public override bool ClickTitle { get { return true; } }
        public override bool ShowFameTitle { get { return false; } }
        public override string DefaultName { get { return "Khal'nga"; } }

        [Constructable]
        public Khalnga()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Body = 0x190;
            Title = "the Bone Lord";

            SetStr(1254, 1381);
            SetDex(93, 135);
            SetInt(745, 810);

            SetHits(3000, 4500);

            SetDamage(30, 45);

            SetSkill(SkillName.Wrestling, 90.1, 100.0);
            SetSkill(SkillName.Tactics, 90.1, 100.0);
            SetSkill(SkillName.MagicResist, 90.1, 100.0);
            SetSkill(SkillName.Magery, 90.1, 100.0);
            SetSkill(SkillName.EvalInt, 100.0);
            SetSkill(SkillName.Meditation, 120.1, 130.0);

            VirtualArmor = 50;
            Fame = 10000;
            Karma = -10000;

            PlateHelm helm = new PlateHelm();
            helm.Hue = 1157;
            helm.Identified = true;
            AddItem(Immovable(helm));

            PlateArms arms = new PlateArms();
            arms.Hue = 1157;
            arms.Identified = true;
            AddItem(Immovable(arms));

            PlateLegs legs = new PlateLegs();
            legs.Hue = 1157;
            legs.Identified = true;
            AddItem(Immovable(legs));

            PlateGloves gloves = new PlateGloves();
            gloves.Hue = 1157;
            gloves.Identified = true;
            AddItem(Immovable(gloves));

            PlateChest chest = new PlateChest();
            chest.Hue = 1157;
            chest.Identified = true;
            AddItem(Immovable(chest));

            PlateGorget gorget = new PlateGorget();
            gorget.Hue = 1157;
            gorget.Identified = true;
            AddItem(Immovable(gorget));

            Spellbook spellbook = new Spellbook();
            spellbook.Hue = 1157;
            spellbook.Name = "red book of spells";
            AddItem(Immovable(spellbook));
        }

        public override int GetIdleSound()
        {
            return 0x184;
        }

        public override int GetAngerSound()
        {
            return 0x286;
        }

        public override int GetDeathSound()
        {
            return 0x288;
        }

        public override int GetHurtSound()
        {
            return 0x19F;
        }

        public override bool AlwaysMurderer { get { return true; } }
        public override bool Unprovokable { get { return true; } }

        public Khalnga(Serial serial)
            : base(serial)
        {
        }

        public override bool OnBeforeDeath()
        {
            Khalngalich rm = new Khalngalich();

            Say("You know not what you have done.");
            Say("In death, I shall be more powerful than ever!");

            Effects.PlaySound(this, Map, GetDeathSound());
            Effects.SendLocationEffect(Location, Map, 0x3709, 30, 10, 0x835, 0);
            rm.MoveToWorld(Location, Map);

            Delete();
            return false;
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