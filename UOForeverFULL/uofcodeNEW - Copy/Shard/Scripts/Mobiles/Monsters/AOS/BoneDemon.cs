#region References

using System;
using Server.Items;

#endregion

namespace Server.Mobiles
{
    [CorpseName("a bone daemon corpse")]
    public class BoneDaemon : BaseCreature
    {
        public override string DefaultName { get { return "a bone daemon"; } }

        [Constructable]
        public BoneDaemon() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Body = 308;
            BaseSoundID = 0x48D;

            SetStr(1000);
            SetDex(151, 175);
            SetInt(171, 220);

            SetHits(3600);

            SetDamage(34, 36);


            Alignment = Alignment.Undead;


            SetSkill(SkillName.DetectHidden, 80.0);
            SetSkill(SkillName.EvalInt, 77.6, 87.5);
            SetSkill(SkillName.Magery, 77.6, 87.5);
            SetSkill(SkillName.Meditation, 100.0);
            SetSkill(SkillName.MagicResist, 50.1, 75.0);
            SetSkill(SkillName.Tactics, 100.0);
            SetSkill(SkillName.Wrestling, 100.0);

            Fame = 20000;
            Karma = -20000;

            VirtualArmor = 44;
        }


        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 8);

            if (0.08 > Utility.RandomDouble()) // 2 percent - multiply number x 100 to get percent
            {
                var scroll = new SkillScroll();
                scroll.Randomize();
                PackItem(scroll);
            }
        }

        public override Poison PoisonImmune { get { return Poison.Lethal; } }
        public override int TreasureMapLevel { get { return 1; } }

        public BoneDaemon(Serial serial) : base(serial)
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

        private DateTime m_NextAttack;

        public override void OnActionCombat()
        {
            Mobile combatant = Combatant;

            if (combatant == null || combatant.Deleted || combatant.Map != Map || !InRange(combatant, 12) ||
                !CanBeHarmful(combatant) || !InLOS(combatant))
            {
                return;
            }

            if (!Paralyzed && DateTime.UtcNow >= m_NextAttack && Utility.Random(5) == 0)
            {
                SummonUndead(combatant);
                m_NextAttack = DateTime.UtcNow + TimeSpan.FromSeconds(8.0 + (8.0 * Utility.RandomDouble()));
            }
        }

        public void SummonUndead(Mobile target)
        {
            BaseCreature summon = null;

            switch (Utility.Random(11))
            {
                default:
                case 0:
                    summon = new Skeleton();
                    break;
                case 1:
                    summon = new Zombie();
                    break;
                case 2:
                    summon = new Wraith();
                    break;
                case 3:
                    summon = new Spectre();
                    break;
                case 4:
                    summon = new Ghoul();
                    break;
                case 5:
                    summon = new Mummy();
                    break;
                case 6:
                    summon = new Bogle();
                    break;
                case 7:
                    summon = new BoneKnight();
                    break;
                case 8:
                    summon = new SkeletalKnight();
                    break;
                case 9:
                    summon = new Lich();
                    break;
                case 10:
                    summon = new SkeletalMage();
                    break;
            }

            summon.Team = Team;
            summon.FightMode = FightMode.Closest;
            summon.MoveToWorld(target.Location, target.Map);
            Effects.SendLocationEffect(summon.Location, summon.Map, 0x3728, 10, 10, 0, 0);
            summon.Combatant = target;
            summon.PlaySound(summon.GetAttackSound());
        }
    }
}