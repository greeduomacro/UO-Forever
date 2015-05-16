#region References

using Server.Items;

#endregion

namespace Server.Mobiles
{
    [CorpseName("an goblin corpse")]
    public class EnslavedGoblinMage : BaseCreature
    {
        //public override InhumanSpeech SpeechType{ get{ return InhumanSpeech.Orc; } }
        [Constructable]
        public EnslavedGoblinMage()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Enslaved Goblin Mage";
            Body = 334;
            BaseSoundID = 0x45A;

            Alignment = Alignment.Inhuman;

            SetStr(297, 297);
            SetDex(94, 94);
            SetInt(510, 510);

            SetHits(174, 174);
            SetStam(94, 94);
            SetMana(510, 510);

            SetDamage(5, 7);

            SetSkill(SkillName.MagicResist, 121.6, 149.7);
            SetSkill(SkillName.Tactics, 80.0, 85.2);
            SetSkill(SkillName.Anatomy, 82.0, 86.6);
            SetSkill(SkillName.Wrestling, 99.2, 106.4);

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


        public EnslavedGoblinMage(Serial serial)
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