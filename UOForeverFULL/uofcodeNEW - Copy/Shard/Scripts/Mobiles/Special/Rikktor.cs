// **********
// RunUO Shard - Rikktor.cs
// **********

#region References

using System;
using System.Collections.Generic;
using System.Linq;
using Server.Engines.CannedEvil;
using Server.Engines.Craft;
using Server.Engines.CustomTitles;
using Server.Items;

#endregion

namespace Server.Mobiles
{
    public class Rikktor : BaseChampion
    {
        public override ChampionSkullType SkullType
        {
            get { return ChampionSkullType.Power; }
        }

        [Constructable]
        public Rikktor() : base(AIType.AI_Melee)
        {
            Body = 172;
            Name = "Rikktor";

            SpecialTitle = "The Dragon King";
            TitleHue = 1174;

            SetStr(901, 1250);
            SetDex(251, 450);
            SetInt(81, 100);

            Alignment = Alignment.Dragon;

            SetHits(10500, 14750);
            SetStam(80, 100);

            SetDamage(33, 60);

            SetSkill(SkillName.Anatomy, 120.0);
            SetSkill(SkillName.MagicResist, 140.2, 160.0);
            SetSkill(SkillName.Tactics, 120.0);
            SetSkill(SkillName.Wrestling, 120.0);

            Fame = 22500;
            Karma = -22500;

            VirtualArmor = 30;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.UltraRich, 4);
        }

        public override Poison PoisonImmune
        {
            get { return Poison.Lethal; }
        }

        public override ScaleType ScaleType
        {
            get { return ScaleType.All; }
        }

        public override int Scales
        {
            get { return 20; }
        }


        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (0.3 >= Utility.RandomDouble())
                Earthquake();
        }


        public void Earthquake()
        {
            Map map = Map;

            if (map == null)
                return;

            var targets = new List<Mobile>();

            foreach (Mobile m in from Mobile m in GetMobilesInRange(8) where m != this && CanBeHarmful(m) select m)
            {
                if (m is BaseCreature &&
                    (((BaseCreature) m).Controlled || ((BaseCreature) m).Summoned || ((BaseCreature) m).Team != Team))
                    targets.Add(m);
                else if (m.Player)
                    targets.Add(m);
            }

            PlaySound(0x2F3);

            foreach (Mobile m in targets)
            {
                double damage = m.Hits*0.6;

                if (damage < 10.0)
                    damage = 10.0;
                else if (damage > 75.0)
                    damage = 75.0;
                damage *= 1.0 - Math.Min(GetDistanceToSqrt(m)/8.0, 1.0);
                DoHarmful(m);

                m.Damage((int) damage, this);

                if (m.Alive && m.Body.IsHuman && !m.Mounted)
                    m.Animate(20, 7, 1, true, false, 0); // take hit
            }
        }

        public override int GetAngerSound()
        {
            return Utility.Random(0x2CE, 2);
        }

        public override int GetIdleSound()
        {
            return 0x2D2;
        }

        public override int GetAttackSound()
        {
            return Utility.Random(0x2C7, 5);
        }

        public override int GetHurtSound()
        {
            return 0x2D1;
        }

        public override int GetDeathSound()
        {
            return 0x2CC;
        }

        public Rikktor(Serial serial) : base(serial)
        {
        }

        public override void GiveSpecialItems(List<Mobile> eligibleMobs, List<double> eligibleMobScores, double totalScores)
        {

            double currentTestValue = 0.0;
            double roll = Utility.RandomDouble() * totalScores;

            for (int i = 0; i < eligibleMobScores.Count; i++)
            {
                currentTestValue += eligibleMobScores[i];

                if (roll > currentTestValue)
                {
                    continue;
                }

                HeartofRikktor heart = new HeartofRikktor();
                if (eligibleMobs[i] is PlayerMobile && eligibleMobs[i].Backpack != null)
                {
                    eligibleMobs[i].Backpack.DropItem(heart);
                    eligibleMobs[i].SendMessage(54, "You have received the smouldering heart of Rikktor!");
                    break;
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
                    int value = rand.Next(0, 3);
                    Item item = null;
                    switch (value)
                    {
                        case 0:
                            {
                                item = new RikktorsHead();
                            }
                            break;
                        case 1:
                            {
                                item = new RocksAnimatedArtifact();
                            }
                            break;
                        case 2:
                            {
                                item = new StoneFaceTrapNoDamageArtifact();
                            }
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
                        eligibleMobs[i].Backpack.DropItem(new TitleScroll("The Dragoon"));
                        eligibleMobs[i].SendMessage(54, "You have received a title scroll.");
                        return;
                    }
                }
            }


        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

// ReSharper disable once UnusedVariable
            var version = reader.ReadInt();
        }
    }
}