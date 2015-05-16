#region References

using Server.Items;

#endregion

namespace Server.Mobiles
{
    [CorpseName("a magma elemental corpse")]
    public class MagmaElemental : BaseCreature
    {
        public override double DispelDifficulty { get { return 117.5; } }
        public override double DispelFocus { get { return 45.0; } }
        public override string DefaultName { get { return "a magma elemental"; } }

        [Constructable]
        public MagmaElemental() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Body = 199;
            BaseSoundID = 268;
            Hue = 1360;

            Alignment = Alignment.Elemental;

            SetStr(300, 355);
            SetDex(70, 95);
            SetInt(71, 92);

            SetHits(300, 600);

            SetDamage(20, 30);


            SetSkill(SkillName.MagicResist, 80.1, 95.0);
            SetSkill(SkillName.Tactics, 100.1, 110.0);
            SetSkill(SkillName.Wrestling, 80.1, 100.0);

            Fame = 13500;
            Karma = -13500;

            VirtualArmor = 70;

            PackItem(new SulfurousAsh(5));
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 2);
            AddLoot(LootPack.Gems);
        }

        //	public override bool Unprovokable{ get{ return true; } }
        public override int TreasureMapLevel { get { return 4; } }
        public override bool HasBreath { get { return true; } } // fire breath enabled
        public override Poison PoisonImmune { get { return Poison.Deadly; } }
        public override int DefaultBloodHue { get { return 1260; } }

        public MagmaElemental(Serial serial) : base(serial)
        {}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt((int) 0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }

        public override bool HasAura { get { return true; } }
    }
}