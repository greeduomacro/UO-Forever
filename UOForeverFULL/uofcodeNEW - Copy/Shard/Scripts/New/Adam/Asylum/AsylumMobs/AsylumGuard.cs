#region References

using Server.Items;

#endregion

namespace Server.Mobiles
{
    [CorpseName("a shimmering corpse")]
    public class AsylumGuard : BaseCreature
    {
        public override bool CanRummageCorpses { get { return true; } }
        public override bool CanDestroyObstacles { get { return true; } }
        public override bool CanFlee { get { return true; } }
        public override int TreasureMapLevel { get { return 3; } }

        [Constructable]
        public AsylumGuard()
            : base(Utility.RandomBool() ? AIType.AI_Melee : AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Female = Utility.RandomBool();

            Name = Female ? NameList.RandomName("female") : NameList.RandomName("male");
            Body = Female ? 401 : 400;
            Hue = 0x4001;

            SpecialTitle = "[Asylum Guardian]";
            TitleHue = 1161;

            Alignment = Alignment.None;

            SpeechHue = YellHue = Utility.RandomDyedHue();

            SetStr(96, 125);
            SetDex(81, 95);
            SetInt(150, 200);
            SetHits(180, 250);

            SetDamage(5, 20);

            SetSkill(SkillName.Fencing, 105.0, 115.5);
            SetSkill(SkillName.Macing, 105.0, 115.5);
            SetSkill(SkillName.MagicResist, 107.0, 110.5);
            SetSkill(SkillName.Swords, 105.0, 115.5);
            SetSkill(SkillName.EvalInt, 90.1, 100.0);
            SetSkill(SkillName.Magery, 75.1, 95.0);
            SetSkill(SkillName.MagicResist, 95.1, 110.0);
            SetSkill(SkillName.Tactics, 95.1, 100.0);
            SetSkill(SkillName.Wrestling, 90.1, 100.0);

            Fame = 1000;
            Karma = -1800;

            VirtualArmor = 50;

            switch (Utility.Random(7))
            {
                case 0:
                    EquipItem(
                        new Broadsword
                        {
                            Identified = true,
                            Movable = false,
                            LootType = LootType.Blessed
                        });
                    break;
                case 1:
                    EquipItem(
                        new Longsword
                        {
                            Identified = true,
                            Movable = false,
                            LootType = LootType.Blessed
                        });
                    break;
                case 2:
                    EquipItem(
                        new WarHammer
                        {
                            Identified = true,
                            Movable = false,
                            LootType = LootType.Blessed
                        });
                    break;
                case 3:
                    EquipItem(
                        new LargeBattleAxe
                        {
                            Identified = true,
                            Movable = false,
                            LootType = LootType.Blessed
                        });
                    break;
                case 4:
                    EquipItem(
                        new Spear
                        {
                            Identified = true,
                            Movable = false,
                            LootType = LootType.Blessed
                        });
                    break;
                case 5:
                    EquipItem(
                        new Halberd
                        {
                            Identified = true,
                            Movable = false,
                            LootType = LootType.Blessed
                        });
                    break;
                case 6:
                    EquipItem(
                        new ShortSpear
                        {
                            Identified = true,
                            Movable = false,
                            LootType = LootType.Blessed
                        });
                    break;
            }

            EquipItem(
                new PlateChest
                {
                    Identified = true,
                    Movable = false,
                    LootType = LootType.Blessed
                });

            EquipItem(
                new PlateGloves
                {
                    Identified = true,
                    Movable = false,
                    LootType = LootType.Blessed
                });

            EquipItem(
                new PlateArms
                {
                    Identified = true,
                    Movable = false,
                    LootType = LootType.Blessed
                });

            EquipItem(
                new PlateGorget
                {
                    Identified = true,
                    Movable = false,
                    LootType = LootType.Blessed
                });

            EquipItem(
                new PlateLegs
                {
                    Identified = true,
                    Movable = false,
                    LootType = LootType.Blessed
                });

        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.Average);
            AddPackedLoot(LootPack.RichProvisions, typeof(Pouch));
            AddPackedLoot(LootPack.AverageProvisions, typeof(Backpack));
            if (0.2 > Utility.RandomDouble())
            {
                AddPackedLoot(LootPack.AverageProvisions, typeof(Bag));
            }
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            defender.Stam -= Utility.Random(10);
        }

        public AsylumGuard(Serial serial)
            : base(serial)
        {}

        public override void OnDeath(Container c)
        {
            if (Utility.RandomDouble() <= 0.1)
            {
                c.DropItem(new AsylumKey { KeyType = AsylumKeyType.Lower, Name = "magical asylum key: lower depths" });
            }

            if (Utility.RandomDouble() <= 0.0005)
            {
                c.DropItem(new DaemonBlood { Hue = 1289, Name = "Enchanted Manacles", ItemID = 6663 });
            }
            base.OnDeath(c);
        }

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