#region References

using Server.Items;

#endregion

namespace Server.Mobiles
{
    [CorpseName("a fleshrenderer corpse")]
    public class FleshRenderer : BaseCreature
    {
        public override WeaponAbility GetWeaponAbility()
        {
            return Utility.RandomBool() ? WeaponAbility.Dismount : WeaponAbility.ParalyzingBlow;
        }

        public override bool IgnoreYoungProtection { get { return EraML; } }
        public override string DefaultName { get { return "a flesh renderer"; } }

        [Constructable]
        public FleshRenderer()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Body = 315;

            SetStr(401, 460);
            SetDex(201, 210);
            SetInt(221, 260);

            Alignment = Alignment.Demon;

            SetHits(4500);

            SetDamage(16, 20);

            SetSkill(SkillName.DetectHidden, 80.0);
            SetSkill(SkillName.MagicResist, 155.1, 160.0);
            SetSkill(SkillName.Meditation, 100.0);
            SetSkill(SkillName.Tactics, 100.0);
            SetSkill(SkillName.Wrestling, 90.1, 100.0);

            Fame = 23000;
            Karma = -23000;

            VirtualArmor = 24;
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

        public override bool AutoDispel { get { return true; } }
        public override bool BardImmune { get { return true; } }
        public override bool Unprovokable { get { return true; } }
        public override bool AreaPeaceImmune { get { return EraSE; } }
        public override Poison PoisonImmune { get { return Poison.Lethal; } }

        public override int TreasureMapLevel { get { return 1; } }

        public override int GetAttackSound()
        {
            return 0x34C;
        }

        public override int GetHurtSound()
        {
            return 0x354;
        }

        public override int GetAngerSound()
        {
            return 0x34C;
        }

        public override int GetIdleSound()
        {
            return 0x34C;
        }

        public override int GetDeathSound()
        {
            return 0x354;
        }

        public FleshRenderer(Serial serial)
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

            if (BaseSoundID == 660)
            {
                BaseSoundID = -1;
            }
        }
    }
}