#region References

using System;
using System.Collections.Generic;
using Server.Engines.CannedEvil;
using Server.Engines.Craft;
using Server.Engines.CustomTitles;
using Server.Engines.LegendaryCrafting;
using Server.Items;

#endregion

namespace Server.Mobiles
{
    [CorpseName("a minotaur corpse")]
    public class MinotaurWarHeroPortal : BaseChampion
    {
        public override bool CanRummageCorpses { get { return true; } }
        public override bool AlwaysMurderer { get { return true; } }
        public override bool CanDestroyObstacles { get { return true; } }
        public override bool CanFlee { get { return false; } }
        public override bool AutoDispel { get { return true; } }
        public override bool Unprovokable { get { return true; } }
        public override Poison PoisonImmune { get { return Poison.Lesser; } }
        public override bool BardImmune { get { return true; } }

        public override ChampionSkullType SkullType { get { return ChampionSkullType.Special; } }

        public override bool NoGoodies { get { return true; } }

        public override int PSToGive { get { return Utility.RandomDouble() >= 0.5 ? 2 : 1; } }

        public override int FactionPSToGive { get { return 0; } }

        private static readonly SkillName[] _Skills = ((SkillName) 0).GetValues<SkillName>();

        [Constructable]
        public MinotaurWarHeroPortal() : base(AIType.AI_Arcade, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = NameList.RandomName("tokuno male");
            SpecialTitle = "Minotaur Champion";
            TitleHue = 1174;
            Body = 280;
            BaseSoundID = 1106;

            Alignment = Alignment.Inhuman;

            SetStr(1000);
            SetDex(100);
            SetInt(1000);

            SetHits(30000);
            SetMana(5000);

            SetDamage(17, 21);


            foreach (SkillName skill in _Skills)
            {
                SetSkill(skill, 80.0, 140.0);
            }


            Alignment = Alignment.Inhuman;


            SetSkill(SkillName.Wrestling, 140.0, 140.0);
            SetSkill(SkillName.Anatomy, 140.0, 140.0);
            SetSkill(SkillName.Tactics, 140.0, 140.0);

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
                        break;
                    }
                }
            }

            if (0.1 > Utility.RandomDouble())
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
                        eligibleMobs[i].Backpack.DropItem(new TitleScroll("The Matador"));
                        eligibleMobs[i].SendMessage(54, "You have received a title scroll.");
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
            c.DropItem(new Platinum {Amount = 50});
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
                c.DropItem(new MinotaurStatuePortal { Hue = Hue });
            }

            base.OnDeath(c);
        }

        public MinotaurWarHeroPortal(Serial serial)
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