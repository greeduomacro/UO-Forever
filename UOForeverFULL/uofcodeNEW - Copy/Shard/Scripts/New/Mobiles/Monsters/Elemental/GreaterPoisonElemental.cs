#region References

using Server.Items;

#endregion

namespace Server.Mobiles
{
    [CorpseName("a poison elemental corpse")]
    public class GreaterPoisonElemental : BaseCreature
    {
        public override string DefaultName { get { return "a toxic air elemental"; } }

        [Constructable]
        public GreaterPoisonElemental() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Body = 162;
            Hue = Utility.Random(2001, 6);
            BaseSoundID = 263;

            Alignment = Alignment.Elemental;

            SetStr(550, 620);
            SetDex(190, 220);
            SetInt(361, 435);

            SetHits(300, 355);

            SetDamage(13, 19);


            SetSkill(SkillName.EvalInt, 80.1, 95.0);
            SetSkill(SkillName.Magery, 80.1, 95.0);
            SetSkill(SkillName.Meditation, 80.2, 120.0);
            SetSkill(SkillName.Poisoning, 90.1, 100.0);
            SetSkill(SkillName.MagicResist, 85.2, 115.0);
            SetSkill(SkillName.Tactics, 80.1, 100.0);
            SetSkill(SkillName.Wrestling, 70.1, 90.0);

            Fame = 12500;
            Karma = -12500;

            VirtualArmor = 75;

            //PackItem( new PoisonCrystal() );
        }

        public override void GenerateLoot()
        {
            //	AddLoot( LootPack.FilthyRich );
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.MedScrolls);
        }

        public override bool OnBeforeDeath()
        {
            if (1.00 > Utility.RandomDouble())
            {
                PackItem(new Gold(Utility.RandomMinMax(800, 1200)));
            }

            if (0.06 > Utility.RandomDouble()) // 2 percent - multipy number x 100 to get percent
            {
                var scroll = new SkillScroll();
                scroll.Randomize();
                PackItem(scroll);
            }
            return base.OnBeforeDeath();
        }

        public override Poison PoisonImmune { get { return Poison.Lethal; } }
        public override Poison HitPoison { get { return Poison.Lethal; } }
        public override double HitPoisonChance { get { return 0.75; } }
        public override int TreasureMapLevel { get { return 5; } }
        public override int DefaultBloodHue { get { return -1; } }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);
            DoGasAttack(defender);
        }

        public override void OnGotMeleeAttack(Mobile attacker)
        {
            base.OnGotMeleeAttack(attacker);
            DoGasAttack(attacker);
        }

        public override void OnDamagedBySpell(Mobile from)
        {
            base.OnDamagedBySpell(from);
            DoGasAttack(from);
        }

        public override void OnGaveSpellDamage(Mobile defender)
        {
            base.OnGaveSpellDamage(defender);
            DoGasAttack(defender);
        }

        private void DoGasAttack(Mobile target)
        {
            if (Map == null || Map == Map.Internal)
            {
                return;
            }

            var bc = target as BaseCreature;

            if (bc != null && bc.BardProvoked)
            {
                return;
            }

            if (0.20 > Utility.RandomDouble())
            {
                Animate(10, 4, 1, true, false, 0);

                DoHarmful(target);

                target.Damage(Utility.RandomMinMax(20, 25), this);

                target.FixedParticles(0x36BD, 1, 10, 0x1F78, 0xA6, 0, (EffectLayer) 255);
                target.ApplyPoison(this, Poison.Deadly);
            }
        }

        public GreaterPoisonElemental(Serial serial) : base(serial)
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
    }
}