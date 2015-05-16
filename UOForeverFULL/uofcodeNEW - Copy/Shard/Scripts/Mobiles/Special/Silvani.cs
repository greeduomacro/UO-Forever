// **********
// RunUO Shard - Silvani.cs
// **********

namespace Server.Mobiles
{
    public class Silvani : BaseCreature
    {
        public override string DefaultName
        {
            get { return "Silvani"; }
        }

        [Constructable]
        public Silvani() : base(AIType.AI_Mage, FightMode.Evil, 18, 1, 0.1, 0.2)
        {
            Body = 176;
            BaseSoundID = 0x467;

            SpecialTitle = "The Fae Queen";
            TitleHue = 1174;

            SetStr(253, 400);
            SetDex(157, 850);
            SetInt(503, 800);

            SetHits(600);

            SetDamage(27, 38);

            SetSkill(SkillName.EvalInt, 100.0);
            SetSkill(SkillName.Magery, 97.6, 107.5);
            SetSkill(SkillName.Meditation, 100.0);
            SetSkill(SkillName.MagicResist, 100.5, 150.0);
            SetSkill(SkillName.Tactics, 97.6, 100.0);
            SetSkill(SkillName.Wrestling, 97.6, 100.0);

            Fame = 20000;
            Karma = 20000;

            VirtualArmor = 50;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.UltraRich, 2);
        }

        public override OppositionGroup OppositionGroup
        {
            get { return OppositionGroup.FeyAndUndead; }
        }

        public override bool Unprovokable
        {
            get { return true; }
        }

        public override Poison PoisonImmune
        {
            get { return Poison.Regular; }
        }

        public override int TreasureMapLevel
        {
            get { return 5; }
        }

        public void SpawnPixies(Mobile target)
        {
            Map map = Map;

            if (map == null)
                return;

            int newPixies = Utility.RandomMinMax(3, 6);

            for (int i = 0; i < newPixies; ++i)
            {
                var pixie = new Pixie {Team = Team, FightMode = FightMode.Closest};

                Point3D loc = Location;

                for (int j = 0; j < 10; ++j)
                {
                    int x = X + Utility.Random(3) - 1;
                    int y = Y + Utility.Random(3) - 1;
                    int z = map.GetAverageZ(x, y);

                    if (false == map.CanFit(x, y, Z, 16, false, false))
                        loc = new Point3D(x, y, Z);
                    else if (false == map.CanFit(x, y, z, 16, false, false))
                        loc = new Point3D(x, y, z);
                }

                pixie.MoveToWorld(loc, map);
                pixie.Combatant = target;
            }
        }

        public override void AlterDamageScalarFrom(Mobile caster, ref double scalar)
        {
            if (0.1 >= Utility.RandomDouble())
                SpawnPixies(caster);
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            int rand = Utility.Random(20, 6);

            defender.Stam -= rand;
            defender.Mana -= rand;
            defender.Damage(rand, this);
        }

        public override void OnGotMeleeAttack(Mobile attacker)
        {
            base.OnGotMeleeAttack(attacker);

            if (0.1 >= Utility.RandomDouble())
                SpawnPixies(attacker);
        }

        public Silvani(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}