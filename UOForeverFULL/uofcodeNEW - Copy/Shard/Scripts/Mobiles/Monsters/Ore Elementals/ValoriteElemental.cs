#region References

using System;
using System.Collections.Generic;
using Server.Items;
using Server.Network;

#endregion

namespace Server.Mobiles
{
    [CorpseName("an ore elemental corpse")]
    public class ValoriteElemental : BaseCreature
    {
        public override string DefaultName { get { return "a valorite elemental"; } }
        private static readonly int m_DefaultHue = CraftResources.GetInfo(CraftResource.Valorite).Hue;

        [Constructable]
        public ValoriteElemental() : this(2)
        {}

        [Constructable]
        public ValoriteElemental(int oreAmount) : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Body = 112;
            BaseSoundID = 268;
            Hue = m_DefaultHue;

            Alignment = Alignment.Elemental;

            SetStr(226, 255);
            SetDex(126, 145);
            SetInt(71, 92);

            SetHits(136, 153);

            SetDamage(28);


            SetSkill(SkillName.MagicResist, 50.1, 95.0);
            SetSkill(SkillName.Tactics, 60.1, 100.0);
            SetSkill(SkillName.Wrestling, 60.1, 100.0);

            Fame = 3500;
            Karma = -3500;

            VirtualArmor = 38;

            Item ore = new ValoriteOre(oreAmount);
            ore.ItemID = 0x19B9;
            PackItem(ore);
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich);
            AddLoot(LootPack.Gems, 4);
        }

        public override bool AutoDispel { get { return true; } }
        public override bool BleedImmune { get { return true; } }
        public override int TreasureMapLevel { get { return 1; } }
        public override Poison PoisonImmune { get { return Poison.Lethal; } }
        //	public override bool Unprovokable{ get{ return true; } }
        public override bool BreathImmune { get { return true; } }
        public override int DefaultBloodHue { get { return -1; } }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);
            DoGasAttack(defender);
            DoDestroyEquipment(defender);
        }

        public override void OnGotMeleeAttack(Mobile attacker)
        {
            base.OnGotMeleeAttack(attacker);
            DoGasAttack(attacker);
        }

        public override void OnDamagedBySpell(Mobile from)
        {
            base.OnDamagedBySpell(from);
            DoGasAttack(from);
        }

        public override void OnGaveSpellDamage(Mobile defender)
        {
            base.OnGaveSpellDamage(defender);
            DoGasAttack(defender);
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
                    if (item is IDurability && ((IDurability) item).MaxHitPoints > 0 &&
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

        private void DoGasAttack(Mobile target)
        {
            var bc = target as BaseCreature;

            if (bc != null && bc.BardProvoked)
            {
                return;
            }

            if (InRange(target, 2) && 0.3 > Utility.RandomDouble())
            {
                Animate(10, 4, 1, true, false, 0);

                DoHarmful(target);

                target.Damage(Utility.RandomMinMax(15, 20), this);

                target.FixedParticles(0x36BD, 1, 10, 0x1F78, 0xA6, 0, (EffectLayer) 255);
                target.ApplyPoison(this, Poison.Deadly);
            }
        }

        public override void AlterMeleeDamageFrom(Mobile from, ref int damage)
        {
            if (from is BaseCreature)
            {
                var bc = (BaseCreature) from;

                if (bc.Controlled || bc.BardTarget == this)
                {
                    damage = 0; // Immune to pets and provoked creatures
                }
            }
            else
            {
                damage /= 2; // 50% melee damage
            }
        }

        public override void AlterAbilityDamageFrom(Mobile from, ref int damage)
        {
            AlterMeleeDamageFrom(from, ref damage);
        }

        public override void CheckReflect(Mobile caster, ref bool reflect)
        {
            reflect = false; // Every spell is reflected back to the caster
        }

        public ValoriteElemental(Serial serial) : base(serial)
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