#region References

using Server.Items;

#endregion

namespace Server.Mobiles
{
    [CorpseName("a cyclops corpse")]
    public class Cyclops : BaseCreature
    {
        public override string DefaultName { get { return "a cyclops"; } }

        [Constructable]
        public Cyclops() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Body = 75;
            BaseSoundID = 604;

            Alignment = Alignment.Giantkin;

            SetStr(336, 385);
            SetDex(96, 115);
            SetInt(31, 55);

            SetHits(202, 231);
            SetMana(0);

            SetDamage(7, 23);


            SetSkill(SkillName.MagicResist, 60.3, 105.0);
            SetSkill(SkillName.Tactics, 80.1, 100.0);
            SetSkill(SkillName.Wrestling, 80.1, 90.0);

            Fame = 4500;
            Karma = -4500;

            VirtualArmor = 48;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.Average);
            AddPackedLoot(LootPack.RichProvisions, typeof(Pouch));
            AddPackedLoot(LootPack.AverageProvisions, typeof(Backpack));
            if (0.2 > Utility.RandomDouble())
            {
                AddPackedLoot(LootPack.AverageProvisions, typeof(Bag));
            }
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            defender.Stam -= Utility.Random(10);
        }

        public override int Meat { get { return 4; } }
        public override int TreasureMapLevel { get { return 3; } }

        public Cyclops(Serial serial) : base(serial)
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