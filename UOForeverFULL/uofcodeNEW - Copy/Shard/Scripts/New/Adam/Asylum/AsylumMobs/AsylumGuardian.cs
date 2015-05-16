#region References

using Server.Items;

#endregion

namespace Server.Mobiles
{
    [CorpseName("a guardian corpse")]
    public class AsylumGuardian : BaseCreature
    {
        [Constructable]
        public AsylumGuardian() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Sanctum Guard";
            Body = 9;
            BaseSoundID = 357;
            Hue = 33;

            SpecialTitle = "[Asylum Guardian]";
            TitleHue = 1161;

            Alignment = Alignment.Demon;

            SetStr(1254, 1381);
            SetDex(93, 135);
            SetInt(745, 810);

            SetHits(694, 875);

            SetDamage(12, 20);


            SetSkill(SkillName.EvalInt, 90.1, 100.0);
            SetSkill(SkillName.Magery, 75.1, 95.0);
            SetSkill(SkillName.MagicResist, 95.1, 110.0);
            SetSkill(SkillName.Tactics, 95.1, 100.0);
            SetSkill(SkillName.Wrestling, 90.1, 100.0);

            Fame = 18750;
            Karma = -18750;

            VirtualArmor = 65;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 2);
            AddLoot(LootPack.Rich, 2);
            AddLoot(LootPack.Average, 2);
            AddLoot(LootPack.HighScrolls);
            AddLoot(LootPack.Gems, 5);

            if (0.03 >= Utility.RandomDouble())
            {
                var scroll = new SkillScroll();
                scroll.Randomize();
                PackItem(scroll);
            }
        }

        public override void OnDeath(Container c)
        {
            if (Utility.RandomDouble() <= 0.1)
            {
                c.DropItem(new BlindingAnkh());
            }

            if (Utility.RandomDouble() <= 0.01)
            {
                c.DropItem(new DaemonBlood{Hue = 33, Name = "Daemon Ichor"});
            }

            if (Utility.RandomDouble() <= 0.001)
            {
                if (Utility.RandomBool())
                {
                    c.DropItem(new BloodSpear {Hue = 33});
                }
                else
                {
                    c.DropItem(new BloodFork { Hue = 33 });
                }
            }
            base.OnDeath(c);
        }

        public override int TreasureMapLevel { get { return 6; } }
        public override int Meat { get { return 1; } }
        public override int DefaultBloodHue { get { return 33; } }

        public override bool CanRummageCorpses { get { return true; } }
        public override bool CanDestroyObstacles { get { return true; } }
        public override bool CanFlee { get { return true; } }
        public override bool Unprovokable { get { return true; } }
        public override Poison PoisonImmune { get { return Poison.Lethal; } }

        public AsylumGuardian(Serial serial)
            : base(serial)
        {}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }
}