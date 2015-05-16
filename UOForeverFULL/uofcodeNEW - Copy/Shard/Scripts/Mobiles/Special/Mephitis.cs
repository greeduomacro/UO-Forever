// **********
// RunUO Shard - Mephitis.cs
// **********

#region References

using System;
using System.Collections.Generic;
using Server.Engines.CannedEvil;
using Server.Engines.Craft;
using Server.Engines.CustomTitles;
using Server.Items;

#endregion

namespace Server.Mobiles
{
    public class Mephitis : BaseChampion
    {
        public override ChampionSkullType SkullType
        {
            get { return ChampionSkullType.Venom; }
        }

        [Constructable]
        public Mephitis() : base(AIType.AI_Melee)
        {
            Body = 173;
            Name = "Mephitis";
            SpecialTitle = "The Spider God";
            TitleHue = 1174;

            BaseSoundID = 0x183;

            SetStr(865, 1250);
            SetDex(172, 375);
            SetInt(506, 670);

            SetHits(9500, 14600);
            SetStam(80, 100);

            SetDamage(37, 49);

            SetSkill(SkillName.MagicResist, 70.7, 140.0);
            SetSkill(SkillName.Tactics, 97.6, 100.0);
            SetSkill(SkillName.Wrestling, 97.6, 100.0);

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

        public override Poison HitPoison
        {
            get { return Poison.Lethal; }
        }

        public override void GiveSpecialItems(List<Mobile> eligibleMobs, List<double> eligibleMobScores, double totalScores)
        {
            double currentTestValue = 0.0;
            double roll = Utility.RandomDouble() * totalScores;
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
                                item = new BloodCocoonArtifact();
                            }
                            break;
                        case 1:
                            {
                                item = new CocoonWebbingArtifact();
                            }
                            break;
                        case 2:
                            {
                                item = new GasTrapArtifact();
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
                        eligibleMobs[i].Backpack.DropItem(new TitleScroll("Daddy Long Leg"));
                        eligibleMobs[i].SendMessage(54, "You have received a title scroll.");
                        return;
                    }
                }
            }
        }


        public override void OnGotMeleeAttack(Mobile attacker)
        {
            base.OnGotMeleeAttack(attacker);

           
        }

        public Mephitis(Serial serial) : base(serial)
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