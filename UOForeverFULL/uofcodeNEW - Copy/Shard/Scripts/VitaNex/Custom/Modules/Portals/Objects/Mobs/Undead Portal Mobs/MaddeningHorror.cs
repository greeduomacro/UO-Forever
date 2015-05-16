#region References

using System;
using System.Collections.Generic;
using Server.Engines.CannedEvil;
using Server.Engines.Craft;
using Server.Engines.LegendaryCrafting;
using Server.Items;

#endregion

namespace Server.Mobiles
{
    [CorpseName("a horrific corpse")]
    public class MaddeningHorrorPortal : BaseChampion
    {
        public override bool CanRummageCorpses { get { return true; } }
        public override bool AlwaysMurderer { get { return true; } }
        public override bool CanDestroyObstacles { get { return true; } }
        public override bool CanFlee { get { return false; } }
        public override bool AutoDispel { get { return true; } }
        public override bool Unprovokable { get { return true; } }
        public override Poison PoisonImmune { get { return Poison.Lesser; } }
        public override bool BardImmune
        {
            get { return true; }
        }

        public override ChampionSkullType SkullType { get { return ChampionSkullType.Special; } }

        public override bool NoGoodies { get { return true; } }

        public override int PSToGive { get { return Utility.RandomDouble() >= 0.5 ? 2 : 1; } }

        public override int FactionPSToGive { get { return 0; } }

        private static readonly SkillName[] _Skills = ((SkillName) 0).GetValues<SkillName>();

        [Constructable]
        public MaddeningHorrorPortal() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a maddening horror";
            SpecialTitle = "Horror of the Abyss";
            TitleHue = 1174;
            Body = 721;
            Hue = 1172;
            BaseSoundID = 1551;

            Alignment = Alignment.Undead;

            SetStr(500);
            SetDex(100);
            SetInt(1000);

            SetHits(30000);
            SetMana(5000);

            SetDamage(17, 21);


            foreach (SkillName skill in _Skills)
            {
                SetSkill(skill, 80.0, 140.0);
            }


            Alignment = Alignment.Undead;


            SetSkill(SkillName.EvalInt, 140.0, 140.0);
            SetSkill(SkillName.Magery, 140.0, 140.0);

            Fame = 23000;
            Karma = -23000;

            VirtualArmor = 60;
            PackNecroReg(30, 275);
        }

        public override void GiveSpecialItems(List<Mobile> eligibleMobs, List<double> eligibleMobScores, double totalScores)
        {
            double currentTestValue = 0.0;
            double roll = Utility.RandomDouble() * totalScores;
            if (0.05 > Utility.RandomDouble())
            {
                for (int i = 0; i < eligibleMobScores.Count; i++)
                {
                    currentTestValue += eligibleMobScores[i];

                    if (roll > currentTestValue)
                    {
                        continue;
                    }

                    var recipescroll = new RecipeScroll(Recipe.GetRandomRecipe());
                    if (eligibleMobs[i] is PlayerMobile && eligibleMobs[i].Backpack != null)
                    {
                        eligibleMobs[i].Backpack.DropItem(recipescroll);
                        eligibleMobs[i].SendMessage(54, "You have received a recipe scroll!");
                        return;
                    }
                }
            }
        }

        public override OppositionGroup OppositionGroup { get { return OppositionGroup.FeyAndUndead; } }

        public override int GetIdleSound()
        {
            return 0x19D;
        }

        public override int GetAngerSound()
        {
            return 0x175;
        }

        public override int GetDeathSound()
        {
            return 0x108;
        }

        public override int GetAttackSound()
        {
            return 0xE2;
        }

        public override int GetHurtSound()
        {
            return 0x28B;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 3);
            AddLoot(LootPack.MedScrolls, 2);

            if (0.009 > Utility.RandomDouble()) // 2 percent - multipy number x 100 to get percent
            {
                switch (Utility.Random(2))
                {
                        // rolls and number it gives it a case. if the number is , say , 3 it will pack that item
                    case 0:
                        PackItem(new BlackDyeTub());
                        break;
                    case 1:
                        PackItem(new Sandals(1175));
                        break;
                }
            }
        }

        public override bool BleedImmune { get { return true; } }
        public override int TreasureMapLevel { get { return 5; } }

        private DateTime m_NextAttack;

        public override void OnActionCombat()
        {
            Mobile combatant = Combatant;

            if (combatant == null || combatant.Deleted || combatant.Map != Map || !InRange(combatant, 12) ||
                !CanBeHarmful(combatant) || !InLOS(combatant))
            {
                return;
            }

            if (DateTime.UtcNow >= m_NextAttack && AIObject.Action != ActionType.Combat &&
                AIObject.Action != ActionType.Flee && !Paralyzed && Utility.Random(10) == 0)
            {
                SummonUndead(combatant);
                m_NextAttack = DateTime.UtcNow + TimeSpan.FromSeconds(300.0 + (300.0 * Utility.RandomDouble()));
            }
        }

        public override void OnThink()
        {
            base.OnThink();

            if (DateTime.UtcNow >= m_NextAttack && AIObject.Action != ActionType.Combat &&
                AIObject.Action != ActionType.Flee && !Paralyzed)
            {
                EndPolymorph();
            }
        }

        public override bool OnBeforeDeath()
        {
            if (0.5 > Utility.RandomDouble())
            {
                PackItem(new LegendaryHammer());
            }
            PackItem(new Gold(5000));
            PackItem(new Gold(5000));

            return base.OnBeforeDeath();
        }

        public override void OnDeath(Container c)
        {
            c.DropItem(new Platinum { Amount = 50 });
            c.DropItem(new GargoyleRune());
            if (Utility.RandomDouble() < 0.5)
            {
                c.DropItem(new GargoyleRune());
            }
            if (Utility.RandomDouble() < 0.1)
            {
                c.DropItem(new GargoyleRune());
            }

            var scroll = new SkillScroll();
            scroll.Randomize();
            c.DropItem(scroll);

            scroll = new SkillScroll();
            scroll.Randomize();
            c.DropItem(scroll);

            if (Utility.RandomDouble() < 0.01)
            {
                c.DropItem(new MaddeninghorrorStatue { Hue = Hue });
            }

            base.OnDeath(c);
        }

        public void SummonUndead(Mobile target)
        {
            var locs = new Point3D[4];

            locs[0] = Location;

            for (int i = 1; i < 4; i++)
            {
                bool validLocation = false;

                for (int j = 0; !validLocation && j < 10; ++j)
                {
                    int x = X + Utility.Random(4) - 1;
                    int y = Y + Utility.Random(4) - 1;
                    int z = Map.GetAverageZ(x, y);

                    if (validLocation = Map.CanFit(x, y, Z, 16, false, false))
                    {
                        locs[i] = new Point3D(x, y, Z);
                    }
                    else if (validLocation = Map.CanFit(x, y, z, 16, false, false))
                    {
                        locs[i] = new Point3D(x, y, z);
                    }
                }
            }

            bool movelich = false;

            for (int i = 0; i < 4; i++)
            {
                BaseCreature summon = null;

                if (!movelich && (Utility.Random(4) == 0 || i == 3))
                {
                    summon = this;
                    BodyMod = Utility.RandomList(50, 56, 57, 3, 26, 148, 147, 153, 154, 24, 35, 36);
                    HueMod = 0;
                    movelich = true;
                }
                else
                {
                    switch (Utility.Random(12))
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
                            summon = new Lizardman();
                            break;
                        case 11:
                            summon = new SkeletalMage();
                            break;
                    }

                    summon.Team = Team;
                    summon.FightMode = FightMode.Closest;
                }

                summon.MoveToWorld(locs[i], Map);
                Effects.SendLocationEffect(summon.Location, summon.Map, 0x3728, 10, 10, 0, 0);
                summon.PlaySound(0x48F);
                summon.PlaySound(summon.GetAttackSound());
                summon.Combatant = target;
            }
        }

        public void EndPolymorph()
        {
            BodyMod = 0;
            HueMod = -1;
        }

        public MaddeningHorrorPortal(Serial serial)
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