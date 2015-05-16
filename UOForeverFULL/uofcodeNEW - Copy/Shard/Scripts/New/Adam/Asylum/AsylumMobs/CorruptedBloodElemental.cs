#region References

using Server.Items;

#endregion

namespace Server.Mobiles
{
    [CorpseName("a blood elemental corpse")]
    public class CorruptedBloodElemental : BaseCreature
    {
        public override string DefaultName { get { return "a defiled blood elemental"; } }

        [Constructable]
        public CorruptedBloodElemental() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Body = 159;
            BaseSoundID = 278;

            SpecialTitle = "[Covenant of Blood]";
            TitleHue = 133;

            Hue = 1157;
            Alignment = Alignment.Elemental;

            SetStr(986, 1185);
            SetDex(177, 255);
            SetInt(151, 250);

            SetHits(592, 711);

            SetDamage(22, 29);

            SetSkill(SkillName.Anatomy, 25.1, 50.0);
            SetSkill(SkillName.EvalInt, 90.1, 100.0);
            SetSkill(SkillName.Magery, 95.5, 100.0);
            SetSkill(SkillName.Meditation, 25.1, 50.0);
            SetSkill(SkillName.MagicResist, 100.5, 150.0);
            SetSkill(SkillName.Tactics, 90.1, 100.0);
            SetSkill(SkillName.Wrestling, 90.1, 100.0);

            Fame = 8000;
            Karma = -8000;

            VirtualArmor = 61;
            PackItem(new BloodCrystal(1));
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 2);
        }

        public override bool OnBeforeDeath()
        {
            if (1.00 > Utility.RandomDouble())
            {
                PackItem(new Gold(Utility.RandomMinMax(100, 400)));
            }

            if (0.03 > Utility.RandomDouble()) // 2 percent - multipy number x 100 to get percent
            {
                var scroll = new SkillScroll();
                scroll.Randomize();
                PackItem(scroll);
            }
            return base.OnBeforeDeath();
        }

        public override void OnDeath(Container c)
        {
            if (Utility.RandomDouble() <= 0.1)
            {
                c.DropItem(new AsylumKey { KeyType = AsylumKeyType.Middle, Name = "magical asylum key: goods storage", Hue = 38 });
            }

            if (Utility.RandomDouble() <= 0.005)
            {
                c.DropItem(new WallBlood { Hue = 1157, ItemID = 4655});
            }

            if (Utility.RandomDouble() <= 0.01)
            {
                c.DropItem(new BloodCrystal { Hue = 1157, Name = "a defiled blood crystal" });
            }
            base.OnDeath(c);
        }

        public override int TreasureMapLevel { get { return 5; } }

        public CorruptedBloodElemental(Serial serial)
            : base(serial)
        {}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt((int) 0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }
}