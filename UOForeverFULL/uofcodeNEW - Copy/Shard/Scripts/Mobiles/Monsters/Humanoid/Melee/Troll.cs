#region References

using Server.Items;

#endregion

namespace Server.Mobiles
{
    [CorpseName("a troll corpse")]
    public class Troll : BaseCreature
    {
        public override string DefaultName { get { return "a troll"; } }

        [Constructable]
        public Troll() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Body = Utility.RandomList(53, 54);
            BaseSoundID = 461;

            Alignment = Alignment.Giantkin;

            SetStr(176, 205);
            SetDex(46, 65);
            SetInt(46, 70);

            SetHits(106, 123);

            SetDamage(8, 14);

            SetSkill(SkillName.MagicResist, 45.1, 60.0);
            SetSkill(SkillName.Tactics, 50.1, 70.0);
            SetSkill(SkillName.Wrestling, 50.1, 70.0);

            Fame = 3500;
            Karma = -3500;

            VirtualArmor = 40;

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
            AddLoot(LootPack.Average);
            AddPackedLoot(LootPack.MeagerProvisions, typeof(Bag));
            if (0.3 > Utility.RandomDouble())
            {
                AddPackedLoot(LootPack.AverageProvisions, typeof(Bag));
            }
        }

        public override bool CanRummageCorpses { get { return true; } }
        public override int TreasureMapLevel { get { return 1; } }
        public override int Meat { get { return 2; } }

        public Troll(Serial serial) : base(serial)
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