#region References

using Server.Items;

#endregion

namespace Server.Mobiles
{
    [CorpseName("a chaos daemon corpse")]
    public class ChaosDaemon : BaseCreature
    {
        public override WeaponAbility GetWeaponAbility()
        {
            return WeaponAbility.CrushingBlow;
        }

        public override string DefaultName { get { return "a chaos daemon"; } }

        [Constructable]
        public ChaosDaemon() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Body = 792;
            BaseSoundID = 0x3E9;

            Alignment = Alignment.Demon;

            SetStr(106, 130);
            SetDex(171, 200);
            SetInt(56, 80);

            SetHits(91, 110);

            SetDamage(12, 17);


            SetSkill(SkillName.MagicResist, 85.1, 95.0);
            SetSkill(SkillName.Tactics, 70.1, 80.0);
            SetSkill(SkillName.Wrestling, 95.1, 100.0);

            Fame = 3000;
            Karma = -4000;

            VirtualArmor = 15;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);
            AddLoot(LootPack.Meager);
        }

        public ChaosDaemon(Serial serial) : base(serial)
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