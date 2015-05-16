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
    [CorpseName("a dragon-god corpse")]
    public class Akor : BaseChampion
    {
        public override bool ReacquireOnMovement { get { return true; } }
        public override bool HasBreath { get { return true; } } // fire breath enabled
        public override bool AutoDispel { get { return true; } }
        public override HideType HideType { get { return HideType.Barbed; } }
        public override int Hides { get { return 35; } }
        public override int Meat { get { return 15; } }
        public override int Scales { get { return 18; } }
        public override ScaleType ScaleType { get { return (ScaleType)Utility.Random(4); } }
        public override Poison PoisonImmune { get { return Poison.Deadly; } }
        public override Poison HitPoison { get { return Poison.Deadly; } }
        public override int DefaultBloodHue { get { return -1; } }
        public override bool BardImmune
        {
            get { return true; }
        }

        public override ChampionSkullType SkullType { get { return ChampionSkullType.Special; } }

        public override bool NoGoodies { get { return true; } }

        public override int PSToGive { get { return 1; } }

        public override int FactionPSToGive { get { return 0; } }

        private static readonly SkillName[] _Skills = ((SkillName) 0).GetValues<SkillName>();

        [Constructable]
        public Akor() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Akor";
            SpecialTitle = "The Dragon-God";
            TitleHue = 1174;
            Body = 826;
            BaseSoundID = 412;
            Hue = 1172;

            SetStr(1296, 1385);
            SetDex(186, 275);
            SetInt(786, 875);


            SetHits(5000, 6000);

            SetDamage(35, 45);


            foreach (SkillName skill in _Skills)
            {
                SetSkill(skill, 80.0, 140.0);
            }


            Alignment = Alignment.Dragon;


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
            if (0.3 > Utility.RandomDouble())
            {
                for (int i = 0; i < eligibleMobScores.Count; i++)
                {
                    currentTestValue += eligibleMobScores[i];

                    if (roll > currentTestValue)
                    {
                        continue;
                    }

                    CannonFuse fuse = new CannonFuse();
                    if (eligibleMobs[i] is PlayerMobile && eligibleMobs[i].Backpack != null)
                    {
                        eligibleMobs[i].Backpack.DropItem(fuse);
                        eligibleMobs[i].SendMessage(54, "You have received a cannon fuse!");
                        return;
                    }
                }
            }
        }

        public override int GetIdleSound()
        {
            return 0x2D3;
        }

        public override int GetHurtSound()
        {
            return 0x2D1;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.UltraRich);
            AddLoot(LootPack.FilthyRich, 2);
            AddLoot(LootPack.Gems, 5);
            AddLoot(LootPack.HighScrolls);
            AddLoot(LootPack.MedScrolls, 3);
        }

        public override void GenerateLoot(bool spawning)
        {
            base.GenerateLoot(spawning);

            if (!spawning)
            {
                PackBagofRegs((0.25 > Utility.RandomDouble()) ? 75 : Utility.RandomMinMax(35, 50));
            }
        }

        public override bool BleedImmune { get { return true; } }
        public override int TreasureMapLevel { get { return 5; } }


        public override bool OnBeforeDeath()
        {
            switch (Utility.Random(500))
            {
                case 0:
                    PackItem(new LeatherDyeTub());
                    break;
                case 1:
                    PackItem(new DragonHead());
                    break;
            }

            if (0.05 > Utility.RandomDouble())
            {
                PackItem(new DragonBoneShards());
            }
            if (0.001 > Utility.RandomDouble())
            {
                PackItem(new DragonHeart());
            }
            if (0.5 > Utility.RandomDouble())
            {
                PackItem(new LegendaryHammer());
            }

            if (Utility.RandomDouble() < 0.1)
            {
                PackItem(new EvolutionEgg());
            }

            PackItem(new Gold(20000));
            return base.OnBeforeDeath();
        }

        public Akor(Serial serial)
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