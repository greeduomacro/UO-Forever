#region References

using Server.Items;
using Server.Misc;

#endregion

namespace Server.Mobiles
{
    [CorpseName("a ratman's corpse")]
    public class Ratman : BaseCreature
    {
        public override InhumanSpeech SpeechType { get { return InhumanSpeech.Ratman; } }

        [Constructable]
        public Ratman() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = NameList.RandomName("ratman");
            Body = 42;
            BaseSoundID = 437;

            Alignment = Alignment.Inhuman;

            SetStr(96, 120);
            SetDex(81, 100);
            SetInt(36, 60);

            SetHits(58, 72);

            SetDamage(4, 5);


            SetSkill(SkillName.MagicResist, 35.1, 60.0);
            SetSkill(SkillName.Tactics, 50.1, 75.0);
            SetSkill(SkillName.Wrestling, 50.1, 75.0);

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
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Meager);
            AddPackedLoot(LootPack.PoorProvisions, typeof(Bag));

            if (0.3 > Utility.RandomDouble())
            {
                AddPackedLoot(LootPack.MeagerProvisions, typeof(Bag));
            }
        }

        public override bool CanRummageCorpses { get { return true; } }
        public override int Hides { get { return 8; } }
        public override HideType HideType { get { return HideType.Spined; } }

        public Ratman(Serial serial) : base(serial)
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