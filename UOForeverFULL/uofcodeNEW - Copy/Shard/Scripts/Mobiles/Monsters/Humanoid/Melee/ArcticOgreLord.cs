#region References

using Server.Items;

#endregion

namespace Server.Mobiles
{
    [CorpseName("a frozen ogre lord corpse")]
    [TypeAlias("Server.Mobiles.ArticOgreLord")]
    public class ArcticOgreLord : BaseCreature
    {
        public override string DefaultName { get { return "an arctic ogre lord"; } }

        [Constructable]
        public ArcticOgreLord() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Body = 135;
            BaseSoundID = 427;

            Alignment = Alignment.Giantkin;

            SetStr(767, 945);
            SetDex(66, 75);
            SetInt(46, 70);

            SetHits(476, 552);

            SetDamage(20, 25);


            SetSkill(SkillName.MagicResist, 125.1, 140.0);
            SetSkill(SkillName.Tactics, 90.1, 100.0);
            SetSkill(SkillName.Wrestling, 90.1, 100.0);

            Fame = 15000;
            Karma = -15000;

            VirtualArmor = 50;

            PackItem(new Club());
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich);
            AddLoot(LootPack.Rich);
            AddPackedLoot(LootPack.RichProvisions, typeof(Bag));
            AddPackedLoot(LootPack.RichProvisions, typeof(Backpack));
            if (0.25 > Utility.RandomDouble())
            {
                AddPackedLoot(LootPack.RichProvisions, typeof(Bag));
            }
        }

        public override Poison PoisonImmune { get { return Poison.Deadly; } }
        public override int TreasureMapLevel { get { return 3; } }

        public ArcticOgreLord(Serial serial) : base(serial)
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