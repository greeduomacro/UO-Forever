#region References

using Server.Items;

#endregion

namespace Server.Mobiles
{
    [CorpseName("an goblin corpse")]
    public class EnslavedGoblinScout : BaseCreature
    {
        //public override InhumanSpeech SpeechType{ get{ return InhumanSpeech.Orc; } }
        [Constructable]
        public EnslavedGoblinScout()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Enslaved Goblin Scout";
            Body = 334;
            BaseSoundID = 0x45A;

            Alignment = Alignment.Inhuman;

            SetStr(320, 320);
            SetDex(74, 74);
            SetInt(112, 112);

            SetHits(182, 182);
            SetStam(74, 74);
            SetMana(112, 112);

            SetDamage(5, 7);

            SetSkill(SkillName.MagicResist, 95.0, 95.0);
            SetSkill(SkillName.Tactics, 80.0, 86.9);
            SetSkill(SkillName.Anatomy, 82.0, 89.3);
            SetSkill(SkillName.Wrestling, 99.2, 113.7);

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


        public EnslavedGoblinScout(Serial serial)
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