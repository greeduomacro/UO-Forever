#region References

using Server.Items;

#endregion

namespace Server.Mobiles
{
    [CorpseName("an ogre corpse")]
    public class Ogre : BaseCreature
    {
        public override string DefaultName { get { return "an ogre"; } }

        [Constructable]
        public Ogre() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Body = 1;
            BaseSoundID = 427;

            Alignment = Alignment.Giantkin;

            SetStr(166, 195);
            SetDex(46, 65);
            SetInt(46, 70);

            SetHits(100, 117);
            SetMana(0);

            SetDamage(9, 11);

            SetSkill(SkillName.MagicResist, 55.1, 70.0);
            SetSkill(SkillName.Tactics, 60.1, 70.0);
            SetSkill(SkillName.Wrestling, 70.1, 80.0);

            Fame = 3000;
            Karma = -3000;

            VirtualArmor = 32;

            PackItem(new Club());
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);
            AddLoot(LootPack.Potions);
            AddPackedLoot(LootPack.MeagerProvisions, typeof(Bag));
            AddPackedLoot(LootPack.AverageProvisions, typeof(Bag));
            if (0.1 > Utility.RandomDouble())
            {
                AddPackedLoot(LootPack.AverageProvisions, typeof(Pouch));
            }
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            defender.Stam -= Utility.Random(1, 5);
        }

        public override bool CanRummageCorpses { get { return true; } }
        public override int TreasureMapLevel { get { return 1; } }
        public override int Meat { get { return 2; } }

        public Ogre(Serial serial) : base(serial)
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