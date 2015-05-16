using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a blood daemon corpse")]
    public class AsylumDaemon : BaseCreature
    {
        [Constructable]
        public AsylumDaemon()
            : base(Utility.RandomBool() ? AIType.AI_Arcade : AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = NameList.RandomName("demon knight");
            Hue = 2118;
            Body = 40;
            BaseSoundID = 357;

            SpecialTitle = "[Covenant of Blood]";
            TitleHue = 133;

            Alignment = Alignment.Demon;

            SetStr(986, 1185);
            SetDex(177, 255);
            SetInt(151, 250);

            SetHits(650, 711);

            SetDamage(22, 29);


            SetSkill(SkillName.Anatomy, 25.1, 50.0);
            SetSkill(SkillName.EvalInt, 90.1, 100.0);
            SetSkill(SkillName.Magery, 95.5, 100.0);
            SetSkill(SkillName.Meditation, 25.1, 50.0);
            SetSkill(SkillName.MagicResist, 100.5, 150.0);
            SetSkill(SkillName.Tactics, 90.1, 100.0);
            SetSkill(SkillName.Wrestling, 90.1, 100.0);

            Fame = 24000;
            Karma = -24000;

            VirtualArmor = 90;
        }

        public override void OnDeath(Container c)
        {
            if (Utility.RandomDouble() <= 0.005)
            {
                if (Utility.RandomBool())
                {
                    c.DropItem(new BloodSpear());
                }
                else
                {
                    c.DropItem(new BloodFork());
                }
            }

            if (Utility.RandomDouble() <= 0.1)
            {
                c.DropItem(new AsylumKey { KeyType = AsylumKeyType.Upper, Name = "magical asylum key: inner sanctum", Hue = 1372 });
            }

            if (Utility.RandomDouble() <= 0.01)
            {
                c.DropItem(new DaemonBlood { Hue = 2118, Name = "Daemon Ichor" });
            }
            base.OnDeath(c);
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 2);
            AddLoot(LootPack.MedScrolls, 2);
        }

        public override bool CanRummageCorpses { get { return true; } }
        public override Poison PoisonImmune { get { return Poison.Deadly; } }
        public override int TreasureMapLevel { get { return 5; } }
        public override int Meat { get { return 1; } }

        public override bool Unprovokable { get { return true; } }
        public override bool AlwaysMurderer { get { return true; } }
        public override bool CanDestroyObstacles { get { return true; } }

        public AsylumDaemon(Serial serial)
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
            int version = reader.ReadInt();
        }
    }
}