// **********
// RunUO Shard - Semidar.cs
// **********

#region References

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Server.Engines.CannedEvil;
using Server.Engines.Craft;
using Server.Engines.CustomTitles;
using Server.Items;

#endregion

namespace Server.Mobiles
{
    public class Semidar : BaseChampion
    {
        public override ChampionSkullType SkullType
        {
            get { return ChampionSkullType.Pain; }
        }

        [Constructable]
        public Semidar() : base(AIType.AI_Mage)
        {
            Name = "Semidar";
            Body = 174;
            BaseSoundID = 0x4B0;

            SpecialTitle = "The Succubus";
            TitleHue = 1174;

            Alignment = Alignment.Demon;

            SetStr(502, 750);
            SetDex(80, 100);
            SetInt(601, 850);

            SetHits(9950, 11175);
            SetStam(80, 100);

            SetDamage(33, 45);

            SetSkill(SkillName.EvalInt, 95.1, 100.0);
            SetSkill(SkillName.Magery, 90.1, 105.0);
            SetSkill(SkillName.Meditation, 95.1, 100.0);
            SetSkill(SkillName.MagicResist, 100.2, 120.0);
            SetSkill(SkillName.Tactics, 90.1, 105.0);
            SetSkill(SkillName.Wrestling, 90.1, 105.0);

            Fame = 24000;
            Karma = -24000;

            VirtualArmor = 30;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.UltraRich, 4);
            AddLoot(LootPack.FilthyRich);

            var scroll = new SkillScroll();
            scroll.Randomize();
            PackItem(scroll);
        }

        public override bool Unprovokable
        {
            get { return true; }
        }

        public override Poison PoisonImmune
        {
            get { return Poison.Lethal; }
        }

        public override void CheckReflect(Mobile caster, ref bool reflect)
        {
            if (caster.Body.IsMale)
                reflect = false; // Always reflect if caster isn't female
        }

        public override void AlterDamageScalarFrom(Mobile caster, ref double scalar)
        {
            // Take out the 20x damage (note it still does 2x due to basecreature being target
            //if ( caster.Body.IsMale )
            //	scalar = 20; // Male bodies always reflect.. damage scaled 20x
        }

        public void DrainLifeAOE()
        {
            if (Map == null)
                return;

            var list = new ArrayList();

            foreach (Mobile m in from Mobile m in GetMobilesInRange(2) where m != this && CanBeHarmful(m) select m)
            {
                if (m is BaseCreature &&
                    (((BaseCreature) m).Controlled || ((BaseCreature) m).Summoned || ((BaseCreature) m).Team != Team))
                    list.Add(m);
                else if (m.Player)
                    list.Add(m);
            }

            foreach (Mobile m in list)
            {
                DoHarmful(m);

                m.FixedParticles(0x374A, 10, 15, 5013, 0x496, 0, EffectLayer.Waist);
                m.PlaySound(0x231);

                m.SendMessage("You feel the life drain out of you!");

                int toDrain = Utility.RandomMinMax(5, 20);

                Hits += toDrain;
                m.Damage(toDrain, this);
            }
        }

        public void DrainLife(Mobile target)
        {
            if (Map == null)
                return;

            DoHarmful(target);

            target.FixedParticles(0x374A, 10, 15, 5013, 0x496, 0, EffectLayer.Waist);
            target.PlaySound(0x231);

            target.SendMessage("You feel the life drain out of you!");

            int toDrain = Utility.RandomMinMax(10, 40);

            Hits += toDrain;
            target.Damage(toDrain, this);
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);
            double roll = Utility.RandomDouble();
            if (0.01 >= roll)
                DrainLifeAOE();
            else if (0.25 >= Utility.RandomDouble())
                DrainLife(defender);
        }

        public override void OnGotMeleeAttack(Mobile attacker)
        {
            base.OnGotMeleeAttack(attacker);
            double roll = Utility.RandomDouble();
            if (0.01 >= roll)
                DrainLifeAOE();
            else if (0.25 >= Utility.RandomDouble())
                DrainLife(attacker);
        }

        public override void GiveSpecialItems(List<Mobile> eligibleMobs, List<double> eligibleMobScores, double totalScores)
        {
            double currentTestValue = 0.0;
            double roll = Utility.RandomDouble() * totalScores;
            if (0.3 > Utility.RandomDouble())
            {
                for (int i = 0; i < eligibleMobScores.Count; i++)
                {
                    currentTestValue += eligibleMobScores[i];

                    if (roll > currentTestValue)
                    {
                        continue;
                    }

                    EnchantedMarble enchantedmarble = new EnchantedMarble();
                    if (eligibleMobs[i] is PlayerMobile && eligibleMobs[i].Backpack != null)
                    {
                        eligibleMobs[i].Backpack.DropItem(enchantedmarble);
                        eligibleMobs[i].SendMessage(54, "You have received a slab of enchanted marble!");
                        break;
                    }
                }
            }

            currentTestValue = 0.0;
            roll = Utility.RandomDouble() * totalScores;
            if (0.2 > Utility.RandomDouble())
            {
                for (int i = 0; i < eligibleMobScores.Count; i++)
                {
                    currentTestValue += eligibleMobScores[i];

                    if (roll > currentTestValue)
                    {
                        continue;
                    }

                    Random rand = new Random();
                    int value = rand.Next(0, 8);
                    Item item = null;
                    switch (value)
                    {
                        case 0:
                            {
                                item = new DemonSkullArtifact();
                            }
                            break;
                        case 1:
                            {
                                item = new RockAnimatedArtifact();
                            }
                            break;
                        case 2:
                            {
                                item = new DemonSkullArtifact();
                            }
                            break;
                        case 3:
                            {
                                item = new RockAnimatedArtifact();
                            }
                            break;
                        case 4:
                            {
                                item = new DemonSkullArtifact();
                            }
                            break;
                        case 5:
                            {
                                item = new RockAnimatedArtifact();
                            }
                            break;
                        case 6:
                            {
                                item = new DemonSkullArtifact();
                            }
                            break;
                        case 7:
                            item = new SemidarsBed();
                            break;
                    }
                    if (eligibleMobs[i] is PlayerMobile && eligibleMobs[i].Backpack != null && item != null)
                    {
                        eligibleMobs[i].Backpack.DropItem(item);
                        eligibleMobs[i].SendMessage(54, "You have received " + item.Name + ".");
                        break;
                    }
                }
            }

            if (0.2 > Utility.RandomDouble())
            {
                for (int i = 0; i < eligibleMobScores.Count; i++)
                {
                    currentTestValue += eligibleMobScores[i];

                    if (roll > currentTestValue)
                    {
                        continue;
                    }

                    if (eligibleMobs[i] is PlayerMobile && eligibleMobs[i].Backpack != null)
                    {
                        eligibleMobs[i].Backpack.DropItem(new TitleScroll("The Demon Slayer"));
                        eligibleMobs[i].SendMessage(54, "You have received a title scroll.");
                        return;
                    }
                }
            }
        }

        public Semidar(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
// ReSharper disable once UnusedVariable
            var version = reader.ReadInt();
        }
    }
}