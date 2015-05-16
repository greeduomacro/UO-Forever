#region References

using Server.Items;

#endregion

namespace Server.Mobiles
{
    [CorpseName("an goblin corpse")]
    public class EnslavedGoblinKeeper : BaseCreature
    {
        //public override InhumanSpeech SpeechType{ get{ return InhumanSpeech.Orc; } }
        [Constructable]
        public EnslavedGoblinKeeper()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Enslaved Goblin Keeper";
            Body = 334;
            BaseSoundID = 0x45A;

            Alignment = Alignment.Inhuman;

            SetStr(297, 297);
            SetDex(80, 80);
            SetInt(118, 118);

            SetHits(174, 174);
            SetStam(80, 80);
            SetMana(118, 118);

            SetDamage(5, 7);

            SetSkill(SkillName.MagicResist, 121.6, 122.2);
            SetSkill(SkillName.Tactics, 80.0, 82.8);
            SetSkill(SkillName.Anatomy, 82.0, 84.8);
            SetSkill(SkillName.Wrestling, 99.2, 100.7);

            Fame = 1500;
            Karma = -1500;

            VirtualArmor = 28;

            // Loot - 30-40gold, magicitem,gem,goblin blood, essence control
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

        public EnslavedGoblinKeeper(Serial serial)
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