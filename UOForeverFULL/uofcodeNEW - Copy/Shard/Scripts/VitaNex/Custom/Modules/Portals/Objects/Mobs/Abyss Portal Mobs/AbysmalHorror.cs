#region References

using Server.Items;

#endregion

namespace Server.Mobiles
{
    [CorpseName("an abyssmal horror corpse")]
    public class AbysmalHorrorPortal : BaseCreature
    {
        public override WeaponAbility GetWeaponAbility()
        {
            return Utility.RandomBool() ? WeaponAbility.MortalStrike : WeaponAbility.WhirlwindAttack;
        }

        public override bool IgnoreYoungProtection { get { return true; } }
        public override string DefaultName { get { return "an abyssmal horror"; } }

        [Constructable]
        public AbysmalHorrorPortal()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Body = 312;
            BaseSoundID = 0x451;

            IsParagon = false;

            Alignment = Alignment.Demon;

            SetStr(401, 420);
            SetDex(81, 90);
            SetInt(401, 420);

            SetHits(6000);

            SetDamage(13, 17);

            SetSkill(SkillName.EvalInt, 200.0);
            SetSkill(SkillName.Magery, 112.6, 117.5);
            SetSkill(SkillName.Meditation, 200.0);
            SetSkill(SkillName.MagicResist, 117.6, 120.0);
            SetSkill(SkillName.Tactics, 100.0);
            SetSkill(SkillName.Wrestling, 84.1, 88.0);

            Fame = 26000;
            Karma = -26000;

            VirtualArmor = 54;
        }

        public override void OnDeath(Container c)
        {
            if (Utility.RandomDouble() >= 0.15)
            {
                var scroll = new SkillScroll();
                scroll.Randomize();
                c.DropItem(scroll);

            }
            if (Utility.RandomDouble() >= 0.15)
            {
                var scroll = new SkillScroll();
                scroll.Randomize();
                c.DropItem(scroll);

            }

            c.DropItem(new Platinum { Amount = 25 });
            c.DropItem(new GargoyleRune());
            if (Utility.RandomDouble() < 0.5)
            {
                c.DropItem(new GargoyleRune());
            }
            if (Utility.RandomDouble() < 0.1)
            {
                c.DropItem(new GargoyleRune());
            }

            base.OnDeath(c);
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.UltraRich, 2);
        }

        public override bool BardImmune { get { return true; } }
        public override bool Unprovokable { get { return true; } }
        public override bool AreaPeaceImmune { get { return EraSE; } }
        public override Poison PoisonImmune { get { return Poison.Lethal; } }
        public override int TreasureMapLevel { get { return 1; } }
        public override bool AutoDispel { get { return true; } }

        public AbysmalHorrorPortal(Serial serial)
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

            if (BaseSoundID == 357)
            {
                BaseSoundID = 0x451;
            }
        }
    }
}