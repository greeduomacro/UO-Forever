#region References

using Server.Items;

#endregion

namespace Server.Mobiles
{
    [CorpseName("a darknight creeper corpse")]
    public class GoreFiendZombieEvent : BaseCreature
    {
        public override bool IgnoreYoungProtection { get { return EraML; } }

        [Constructable]
        public GoreFiendZombieEvent()
            : base(AIType.AI_Arcade, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = NameList.RandomName("darknight creeper");
            Body = 313;
            BaseSoundID = 0xE0;

            Hue = 61;

            SetStr(796, 825);
            SetDex(86, 105);
            SetInt(436, 475);

            SetHits(250, 300);
            SetDamage(22, 26);

            SetSkill(SkillName.DetectHidden, 80.0);
            SetSkill(SkillName.EvalInt, 118.1, 120.0);
            SetSkill(SkillName.Magery, 112.6, 120.0);
            SetSkill(SkillName.Meditation, 150.0);
            SetSkill(SkillName.Poisoning, 120.0);
            SetSkill(SkillName.MagicResist, 90.1, 90.9);
            SetSkill(SkillName.Tactics, 100.0);
            SetSkill(SkillName.Wrestling, 90.1, 90.9);
            SetSkill(SkillName.Necromancy, 120.1, 130.0);
            SetSkill(SkillName.SpiritSpeak, 120.1, 130.0);

            Fame = 22000;
            Karma = -22000;

            VirtualArmor = 75;

            CurrentSpeed = 0.6;
            PassiveSpeed = 0.6;
            ActiveSpeed = 0.16;
            WeaponDamage = false;
            FreelyLootable = true;
        }

        public override void OnDeath(Container c)
        {
            Engines.ZombieEvent.ZombieEvent.AddItem(c);
            if (Utility.RandomDouble() < 0.2)
            {
                c.DropItem(new CrystalFlask());
            }
            base.OnDeath(c);
        }

        public override bool CheckFlee()
        {
            return false;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 1);
        }

        public override bool BardImmune { get { return true; } }
        public override bool Unprovokable { get { return true; } }
        public override bool AreaPeaceImmune { get { return EraSE; } }
        public override bool BleedImmune { get { return true; } }
        public override Poison PoisonImmune { get { return Poison.Lethal; } }
        public override Poison HitPoison { get { return Poison.Lethal; } }

        public override int TreasureMapLevel { get { return 1; } }

        public GoreFiendZombieEvent(Serial serial)
            : base(serial)
        {}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.ReadInt();

            if (BaseSoundID == 471)
            {
                BaseSoundID = 0xE0;
            }
        }
    }
}