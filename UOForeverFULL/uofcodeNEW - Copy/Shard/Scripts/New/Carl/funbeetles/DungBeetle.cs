#region References

using System;
using Server.Spells;

#endregion

namespace Server.Mobiles
{
    [CorpseName("a dung beetle corpse")]
    public class DungBeetle : BaseCreature
    {
        [Constructable]
        public DungBeetle()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a dung beetle";
            Body = 0x317;
            BaseSoundID = 0x21D;
            Hue = 99;

            SetStr(300, 500);
            SetDex(100, 300);
            SetInt(500, 700);

            SetHits(200, 400);

            SetDamage(15, 30);

            SetSkill(SkillName.MagicResist, 50.0, 80.0);
            SetSkill(SkillName.Tactics, 50.0, 100.0);
            SetSkill(SkillName.Wrestling, 50.0, 100.0);
            SetSkill(SkillName.EvalInt, 50.0, 75.0);
            SetSkill(SkillName.Magery, 45.0, 65.0);

            Fame = 4000;
            Karma = -4000;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average, 3);
            AddLoot(LootPack.Gems, 3);
        }

        public override void OnGotMeleeAttack(Mobile attacker)
        {
            if (attacker != this)
            {
                BoltEffect(0);
                SpellHelper.Damage(TimeSpan.Zero, attacker, this, 10);
            }
        }

        public DungBeetle(Serial serial)
            : base(serial)
        { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(12); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}