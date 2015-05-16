#region References

using Server.Items;

#endregion

namespace Server.Mobiles
{
    [CorpseName("a balron corpse")]
    public class ShadowBalron : BaseCreature
    {
        [Constructable]
        public ShadowBalron() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = NameList.RandomName("balron");
            Body = 40;
            BaseSoundID = 357;
            Hue = 0x4001;

            Alignment = Alignment.Demon;

            SetStr(1185, 1385);
            SetDex(255, 308);
            SetInt(191, 301);

            SetHits(892, 1090);

            SetDamage(25, 32);


            SetSkill(SkillName.Anatomy, 75.1, 90.0);
            SetSkill(SkillName.EvalInt, 100.0);
            SetSkill(SkillName.Magery, 120.0);
            SetSkill(SkillName.Meditation, 75.1, 90.0);
            SetSkill(SkillName.MagicResist, 150.0);
            SetSkill(SkillName.Tactics, 125.0);
            SetSkill(SkillName.Wrestling, 130.0);

            Fame = 30000;
            Karma = -30000;

            VirtualArmor = 100;

            PackItem(new Longsword());
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 3);
            AddLoot(LootPack.Rich, 2);
            AddLoot(LootPack.MedScrolls, 2);
            AddLoot(LootPack.HighScrolls, 3);
            AddLoot(LootPack.LowScrolls, 3);
            AddLoot(LootPack.Gems, 2);
            AddLoot(LootPack.Gems, 2);
            AddLoot(LootPack.Gems, 2);
        }

        public override void GenerateLoot(bool spawning)
        {
            base.GenerateLoot(spawning);

            if (!spawning)
            {
                PackBagofRegs(Utility.RandomMinMax(35, 60));
            }
        }

        public override bool ReacquireOnMovement { get { return true; } }
        public override bool CanRummageCorpses { get { return true; } }
        public override Poison PoisonImmune { get { return Poison.Lethal; } }
        public override Poison HitPoison { get { return Poison.Deadly; } }
        public override int TreasureMapLevel { get { return 5; } }

        public ShadowBalron(Serial serial) : base(serial)
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