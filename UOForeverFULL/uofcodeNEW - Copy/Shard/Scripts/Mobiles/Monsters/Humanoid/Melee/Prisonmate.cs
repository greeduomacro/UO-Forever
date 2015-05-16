#region References

using Server.Items;

#endregion

namespace Server.Mobiles
{
    public class Prisonmate : BaseCreature
    {
        public override bool ClickTitle { get { return false; } }

        [Constructable]
        public Prisonmate() : base(AIType.AI_Arcade, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            SpeechHue = Utility.RandomDyedHue();
            SpecialTitle = "Prison Inmate";
            TitleHue = 1174;
            Hue = Utility.RandomSkinHue();

            if (Female == Utility.RandomBool())
            {
                Body = 0x191;
                Name = NameList.RandomName("female");
                AddItem(new Skirt(Utility.RandomNeutralHue()));
            }
            else
            {
                Body = 0x190;
                Name = NameList.RandomName("male");
                AddItem(new ShortPants(Utility.RandomNeutralHue()));
            }

            SetStr(90, 110);
            SetDex(90, 105);
            SetInt(88, 98);

            SetDamage(13, 27);

            SetSkill(SkillName.Fencing, 98.0, 100.5);
            SetSkill(SkillName.Macing, 98.0, 100.5);
            SetSkill(SkillName.MagicResist, 65.0, 98.5);
            SetSkill(SkillName.Swords, 98.0, 100.5);
            SetSkill(SkillName.Tactics, 98.0, 100.5);
            SetSkill(SkillName.Wrestling, 98.0, 100.5);

            Fame = 1500;
            Karma = -1500;
            VirtualArmor = 18;

            CurrentSpeed = 0.6;
            PassiveSpeed = 0.6;
            ActiveSpeed = 0.16;

            AddItem(new Boots(Utility.RandomNeutralHue()));
            AddItem(new FancyShirt());
            AddItem(new Bandana());

            switch (Utility.Random(7))
            {
                case 0:
                    AddItem(new Longsword());
                    break;
                case 1:
                    AddItem(new Cutlass());
                    break;
                case 2:
                    AddItem(new Broadsword());
                    break;
                case 3:
                    AddItem(new Axe());
                    break;
                case 4:
                    AddItem(new Club());
                    break;
                case 5:
                    AddItem(new Dagger());
                    break;
                case 6:
                    AddItem(new Spear());
                    break;
            }

            Utility.AssignRandomHair(this);
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);
            AddPackedLoot(LootPack.AverageProvisions, typeof(Bag));
        }

        public override bool AlwaysMurderer { get { return true; } }

        public Prisonmate(Serial serial) : base(serial)
        {}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}