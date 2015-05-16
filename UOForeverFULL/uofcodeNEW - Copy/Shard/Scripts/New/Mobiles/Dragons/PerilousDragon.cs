#region References

using System;
using Server.Items;

#endregion

namespace Server.Mobiles
{
    [CorpseName("a dragon corpse")]
    public class PerilousDragon : BaseCreature
    {
        private DateTime m_NextAbility;

        [Constructable]
        public PerilousDragon()
            : base(AIType.AI_Mage, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            Name = "Mushu";
            Body = 197;
            BaseSoundID = 362;

            Alignment = Alignment.Dragon;

            SetStr(2034, 2140);
            SetDex(215, 256);
            SetInt(1025, 1116);

            SetHits(4500);

            SetDamage(20, 30);

            SetSkill(SkillName.EvalInt, 119.2, 125.3);
            SetSkill(SkillName.Magery, 119.9, 125.5);
            SetSkill(SkillName.MagicResist, 116.3, 125.0);
            SetSkill(SkillName.Tactics, 111.7, 126.3);
            SetSkill(SkillName.Wrestling, 120.5, 128.0);
            SetSkill(SkillName.Meditation, 119.4, 130.0);
            SetSkill(SkillName.Anatomy, 118.7, 125.0);
            SetSkill(SkillName.DetectHidden, 120.0);

            Fame = 5000;
            Karma = -5000;

            VirtualArmor = 70;
        }

        public override bool OnBeforeDeath()
        {
            if (Combatant != null)
            {
                Combatant.Kills++;
            }

            if (0.01 > Utility.RandomDouble())
            {
                AddItem(new SuitOfGoldArmorDeed(Utility.RandomBool()));
            }

            return base.OnBeforeDeath();
        }

        public override bool AlwaysMurderer { get { return true; } }
        public override bool Unprovokable { get { return true; } }
        //      public override bool BardImmune { get { return true; } }
        public override bool ReacquireOnMovement { get { return true; } }
        public override bool HasBreath { get { return true; } } // fire breath enabled
        public override bool AutoDispel { get { return true; } }
        public override bool Uncalmable { get { return true; } }
        public override int Meat { get { return 19; } }
        public override int Hides { get { return 40; } }
        public override HideType HideType { get { return HideType.Barbed; } }
        public override int Scales { get { return 35; } }
        public override ScaleType ScaleType { get { return ScaleType.Any; } }
        public override FoodType FavoriteFood { get { return FoodType.Meat; } }
        public override Poison PoisonImmune { get { return Poison.Lethal; } }
        public override Poison HitPoison { get { return Utility.RandomBool() ? Poison.Deadly : Poison.Lethal; } }
        public override int TreasureMapLevel { get { return 5; } }

        public override void OnThink()
        {
            if (m_NextAbility < DateTime.UtcNow && Combatant != null && Combatant.InRange(Location, 5))
            {
                PlaySound(481);
                Combatant.AddStatMod(new StatMod(StatType.Str, "Mushu Curse Str", -15, TimeSpan.FromMinutes(2.5)));
                Combatant.AddStatMod(new StatMod(StatType.Dex, "Mushu Curse Dex", -15, TimeSpan.FromMinutes(2.5)));
                Combatant.AddStatMod(new StatMod(StatType.Dex, "Mushu Curse Int", -15, TimeSpan.FromMinutes(2.5)));
                Combatant.Paralyzed = true;
                m_NextAbility = DateTime.UtcNow + TimeSpan.FromSeconds(5.0);
            }
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 3);
            AddLoot(LootPack.Gems, 5);
        }

        public PerilousDragon(Serial serial) : base(serial)
        {}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}