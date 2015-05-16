#region References

using Server.Ethics;
using Server.Factions;
using Server.Items;

#endregion

namespace Server.Mobiles
{
    [CorpseName("a daemon corpse")]
    public class DaemonZombieEvent : BaseCreature
    {
        public override double DispelDifficulty { get { return 125.0; } }
        public override double DispelFocus { get { return 45.0; } }

        public override Faction FactionAllegiance { get { return Shadowlords.Instance; } }
        public override Ethic EthicAllegiance { get { return Ethic.Evil; } }

        [Constructable]
        public DaemonZombieEvent() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = NameList.RandomName("daemon");
            Body = 102;
            BaseSoundID = 357;

            Alignment = Alignment.Demon;

            SetStr(476, 505);
            SetDex(76, 95);
            SetInt(301, 325);

            SetHits(286, 303);

            SetDamage(7, 14);

            SetSkill(SkillName.EvalInt, 70.1, 80.0);
            SetSkill(SkillName.Magery, 70.1, 80.0);
            SetSkill(SkillName.MagicResist, 85.1, 95.0);
            SetSkill(SkillName.Tactics, 70.1, 80.0);
            SetSkill(SkillName.Wrestling, 60.1, 80.0);

            Fame = 15000;
            Karma = -15000;

            FreelyLootable = true;

            VirtualArmor = 58;
            ControlSlots = /*Core.SE ?*/ 4 /*: 5*/;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.Average, 2);
            AddLoot(LootPack.MedScrolls, 2);
        }

        public override void OnDeath(Container c)
        {
            Engines.ZombieEvent.ZombieEvent.AddItem(c);
            if (Utility.RandomDouble() < 0.2)
            {
                c.DropItem(new DaemonClaw());
            }
            base.OnDeath(c);
        }

        public override void GenerateLoot(bool spawning)
        {
            base.GenerateLoot(spawning);

            if (!spawning && 0.40 > Utility.RandomDouble())
            {
                PackBagofRecallRegs(Utility.RandomMinMax(10, 20));
            }
        }

        public override bool CheckFlee()
        {
            return false;
        }

        public override bool ReacquireOnMovement { get { return true; } }
        public override bool CanRummageCorpses { get { return true; } }
        public override Poison PoisonImmune { get { return Poison.Regular; } }
        public override int TreasureMapLevel { get { return 4; } }
        public override int Meat { get { return 1; } }

        public DaemonZombieEvent(Serial serial)
            : base(serial)
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