#region References

using Server.Items;

#endregion

namespace Server.Mobiles
{
    [CorpseName("a rotting corpse")]
    public class ZombieZEvent : BaseCreature
    {
        public override string DefaultName { get { return "a zombie"; } }

        public override bool ReduceSpeedWithDamage { get { return true; } }

        [Constructable]
        public ZombieZEvent()
            : base(AIType.AI_Arcade, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Body = 3;
            BaseSoundID = 471;

            Alignment = Alignment.Undead;

            SetStr(46, 70);
            SetDex(31, 50);
            SetInt(26, 40);

            SetHits(75);

            SetDamage(10, 24);


            SetSkill(SkillName.MagicResist, 15.1, 40.0);
            SetSkill(SkillName.Tactics, 35.1, 50.0);
            SetSkill(SkillName.Wrestling, 50, 75);

            Fame = 600;
            Karma = -600;

            VirtualArmor = 18;

            CurrentSpeed = 0.6;
            PassiveSpeed = 0.6;
            ActiveSpeed = 0.16;
            RangePerception = 20;
            WeaponDamage = false;
            FreelyLootable = true;
        }

        public override bool BleedImmune { get { return true; } }
        public override Poison PoisonImmune { get { return Poison.Regular; } }
        public override int DefaultBloodHue { get { return 1438; } }

        public ZombieZEvent(Serial serial)
            : base(serial)
        { }

        public override OppositionGroup OppositionGroup { get { return OppositionGroup.FeyAndUndead; } }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override bool CheckFlee()
        {
            return false;
        }

        public override void OnDeath(Container c)
        {
            if (Utility.RandomDouble() < 0.01)
            {
                c.DropItem(new UndyingFlesh());
            }
            Engines.ZombieEvent.ZombieEvent.AddItem(c);
            base.OnDeath(c);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}