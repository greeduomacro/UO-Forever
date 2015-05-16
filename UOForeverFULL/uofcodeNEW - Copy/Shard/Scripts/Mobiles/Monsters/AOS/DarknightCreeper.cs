#region References

using Server.Items;

#endregion

namespace Server.Mobiles
{
    [CorpseName("a darknight creeper corpse")]
    public class DarknightCreeper : BaseCreature
    {
        public override bool IgnoreYoungProtection { get { return EraML; } }

        [Constructable]
        public DarknightCreeper()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = NameList.RandomName("darknight creeper");
            Body = 313;
            BaseSoundID = 0xE0;

            SetStr(301, 330);
            SetDex(101, 110);
            SetInt(301, 330);

            SetHits(4000);

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

            VirtualArmor = 34;
        }

        public override void OnDeath(Container c)
        {
            var scroll = new SkillScroll();
            scroll.Randomize();
            c.DropItem(scroll);

            scroll = new SkillScroll();
            scroll.Randomize();
            c.DropItem(scroll);

            scroll = new SkillScroll();
            scroll.Randomize();
            c.DropItem(scroll);

            scroll = new SkillScroll();
            scroll.Randomize();
            c.DropItem(scroll);

            base.OnDeath(c);
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.UltraRich, 2);
        }

        public override bool BardImmune { get { return true; } }
        public override bool Unprovokable { get { return true; } }
        public override bool AreaPeaceImmune { get { return EraSE; } }
        public override bool BleedImmune { get { return true; } }
        public override Poison PoisonImmune { get { return Poison.Lethal; } }
        public override Poison HitPoison { get { return Poison.Lethal; } }

        public override int TreasureMapLevel { get { return 1; } }

        public DarknightCreeper(Serial serial)
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