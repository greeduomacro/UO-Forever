#region References

using System;
using Server.Spells;

#endregion

namespace Server.Mobiles
{
    [CorpseName("a deadly beetle corpse")]
    public class DeadlyBeetle : BaseCreature
    {
        [Constructable]
        public DeadlyBeetle()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a deadly beetle";
            Body = 0x317;
            BaseSoundID = 0x21D;
            Hue = 36;

            SetStr(400, 600);
            SetDex(200, 400);
            SetInt(600, 800);

            SetHits(300, 500);

            SetDamage(20, 35);

            SetSkill(SkillName.MagicResist, 50.0, 80.0);
            SetSkill(SkillName.Tactics, 50.0, 100.0);
            SetSkill(SkillName.Wrestling, 50.0, 100.0);
            SetSkill(SkillName.EvalInt, 50.0, 75.0);
            SetSkill(SkillName.Magery, 45.0, 65.0);

            Fame = 5000;
            Karma = -5000;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average, 5);
        }

        public override void OnGotMeleeAttack(Mobile attacker)
        {
            if (attacker != this)
            {
                BoltEffect(0);
                SpellHelper.Damage(TimeSpan.Zero, attacker, this, 10);
            }
        }

        public DeadlyBeetle(Serial serial)
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