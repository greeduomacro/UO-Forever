#region References

using Server.Items;

#endregion

namespace Server.Mobiles
{
    [CorpseName("an goblin corpse")]
    public class EnslavedGreenGoblinAlchemist : BaseCreature
    {
        [Constructable]
        public EnslavedGreenGoblinAlchemist()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Green Goblin Alchemist";
            Body = 723;
            BaseSoundID = 0x45A;

            Alignment = Alignment.Inhuman;

            SetStr(289, 289);
            SetDex(72, 72);
            SetInt(113, 113);

            SetHits(196, 196);
            SetStam(72, 72);
            SetMana(113, 113);

            SetDamage(5, 7);

            SetSkill(SkillName.MagicResist, 124.1, 126.2);
            SetSkill(SkillName.Tactics, 75.3, 83.6);
            SetSkill(SkillName.Anatomy, 0.0, 0.0);
            SetSkill(SkillName.Wrestling, 90.4, 94.7);

            Fame = 1500;
            Karma = -1500;

            VirtualArmor = 28;

            switch (Utility.Random(20))
            {
                case 0:
                    PackItem(new Scimitar());
                    break;
                case 1:
                    PackItem(new Katana());
                    break;
                case 2:
                    PackItem(new WarMace());
                    break;
                case 3:
                    PackItem(new WarHammer());
                    break;
                case 4:
                    PackItem(new Kryss());
                    break;
                case 5:
                    PackItem(new Pitchfork());
                    break;
            }

            PackItem(new ThighBoots());

            switch (Utility.Random(3))
            {
                case 0:
                    PackItem(new Ribs());
                    break;
                case 1:
                    PackItem(new Shaft());
                    break;
                case 2:
                    PackItem(new Candle());
                    break;
            }
        }


        public EnslavedGreenGoblinAlchemist(Serial serial)
            : base(serial)
        {}

        public override bool CanRummageCorpses { get { return true; } }
        public override int TreasureMapLevel { get { return 1; } }
        public override int Meat { get { return 1; } }
        public override OppositionGroup OppositionGroup { get { return OppositionGroup.SavagesAndOrcs; } }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Meager);
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