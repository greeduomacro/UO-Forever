using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("Osiredon of the Deep's corpse")]
    public class Osiredon : BaseCreature
    {
        public override string DefaultName { get { return "Osiredon of the Deep"; } }

        [Constructable]
        public Osiredon()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Body = 1068;
            BaseSoundID = 278;
            Hue = 1366;

            SetStr(805, 900);
            SetDex(121, 165);
            SetInt(386, 435);

            SetHits(1200, 2200);

            SetDamage(20, 27);









            SetSkill(SkillName.EvalInt, 120.1);
            SetSkill(SkillName.Magery, 110.1, 120.0);
            SetSkill(SkillName.MagicResist, 120.0, 120.0);
            SetSkill(SkillName.Tactics, 115.9, 125.0);
            SetSkill(SkillName.Wrestling, 110.0);

            Fame = 7500;
            Karma = -7500;

            VirtualArmor = 76;
            CanSwim = true;

            PackItem(new BlackPearl(6));
            //PackItem( new Gold( 100, 200 ) );

        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich, 2);
            AddLoot(LootPack.FilthyRich);
            AddLoot(LootPack.Potions);
        }

  //      public override bool BardImmune { get { return true; } }
        public override Poison PoisonImmune { get { return Poison.Greater; } }
        public override int TreasureMapLevel { get { return 6; } }
        public override int DefaultBloodHue { get { return -1; } }

        public Osiredon(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt((int)0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}