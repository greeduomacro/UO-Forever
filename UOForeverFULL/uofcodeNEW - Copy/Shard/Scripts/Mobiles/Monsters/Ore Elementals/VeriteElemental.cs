#region References

using System;
using System.Collections.Generic;
using Server.Items;
using Server.Network;

#endregion

namespace Server.Mobiles
{
    [CorpseName("an ore elemental corpse")]
    public class VeriteElemental : BaseCreature
    {
        public override string DefaultName { get { return "a verite elemental"; } }
        private static readonly int m_DefaultHue = CraftResources.GetInfo(CraftResource.Verite).Hue;

        [Constructable]
        public VeriteElemental() : this(2)
        {}

        [Constructable]
        public VeriteElemental(int oreAmount) : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Body = 113;
            BaseSoundID = 268;
            Hue = m_DefaultHue;

            Alignment = Alignment.Elemental;

            SetStr(226, 255);
            SetDex(126, 145);
            SetInt(71, 92);

            SetHits(136, 153);

            SetDamage(9, 16);


            SetSkill(SkillName.MagicResist, 50.1, 95.0);
            SetSkill(SkillName.Tactics, 60.1, 100.0);
            SetSkill(SkillName.Wrestling, 60.1, 100.0);

            Fame = 3500;
            Karma = -3500;

            VirtualArmor = 35;

            Item ore = new VeriteOre(oreAmount);
            ore.ItemID = 0x19B9;
            PackItem(ore);
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.Gems, 2);
        }

        public override bool AutoDispel { get { return true; } }
        public override bool BleedImmune { get { return true; } }
        public override Poison PoisonImmune { get { return Poison.Deadly; } }
        public override int TreasureMapLevel { get { return 1; } }
        public override int DefaultBloodHue { get { return -1; } }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);
            DoDestroyEquipment(defender);
        }

        private void DoDestroyEquipment(Mobile target)
        {
            if (target.AccessLevel == AccessLevel.Player && InRange(target, 1) && 0.25 > Utility.RandomDouble())
                //15% chance
            {
                var items = new List<Item>();

                for (int i = 0; i < target.Items.Count; i++)
                {
                    Item item = target.Items[i];
                    if (item is IDurability && !item.Deleted && ((IDurability) item).MaxHitPoints > 0 &&
                        item.LootType != LootType.Blessed && item.BlessedFor == null &&
                        !(InsuranceEnabled && item.Insured))
                    {
                        items.Add(item);
                    }
                }

                if (items.Count > 0)
                {
                    Item toDestroy = items[Utility.Random(items.Count)];
                    string name = toDestroy.Name;
                    if (String.IsNullOrEmpty(name))
                    {
                        name = toDestroy.ItemData.Name;
                    }

                    toDestroy.Delete();

                    target.NonlocalOverheadMessage(MessageType.Regular, 0x3B2, 1080034, name);
                        // Their ~1_NAME~ is destroyed by the attack.
                    target.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1080035, name);
                        // Your ~1_NAME~ is destroyed by the attack.
                }
            }
        }

        public VeriteElemental(Serial serial) : base(serial)
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