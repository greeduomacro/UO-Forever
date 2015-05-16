#region References

using System;
using System.Collections.Generic;
using Server.Items;
using Server.SkillHandlers;

#endregion

namespace Server.Mobiles
{
    [CorpseName("a magnetite elemental corpse")]
    public class MagnetiteElemental : BaseCreature
    {
        public override double DispelDifficulty { get { return 125.0; } }
        public override double DispelFocus { get { return 55.0; } }
        public override string DefaultName { get { return "a magnetite elemental"; } }

        public override bool IsScaredOfScaryThings { get { return false; } }
        public override bool IsScaryToPets { get { return true; } }

        [Constructable]
        public MagnetiteElemental() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.1, 0.1)
        {
            Body = 14;
            BaseSoundID = 268;
            Hue = 2306;

            Alignment = Alignment.Elemental;

            SetStr(300, 355);
            SetDex(95, 115);
            SetInt(71, 92);

            SetHits(400, 650);

            SetDamage(20, 30);


            SetSkill(SkillName.MagicResist, 125.0, 125.0);
            SetSkill(SkillName.Tactics, 100.1, 110.0);
            SetSkill(SkillName.Wrestling, 100.1, 115.0);

            Fame = 14500;
            Karma = -14500;

            VirtualArmor = 75;

            PackItem(new IronOre(25));

            if (0.25 > Utility.RandomDouble())
            {
                PackItem(new IronIngot(50));
            }

            if (0.05 > Utility.RandomDouble())
            {
                PackItem(new ValoriteOre(50));
            }

            if (0.01 > Utility.RandomDouble())
            {
                PackItem(new ValoriteOre(25));
            }
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich, 2);
            AddLoot(LootPack.FilthyRich, 2);
            AddLoot(LootPack.Gems, 4);
        }

        public override void GenerateLoot(bool spawning)
        {
            base.GenerateLoot(spawning);

            if (!spawning)
            {
                double rand = Utility.RandomDouble();
                if (0.02 > rand)
                {
                    PackItem(new MagnetiteOre(Utility.Random(2, 9)));
                }
            }
        }

//		public override bool Unprovokable{ get{ return true; } }
        public override int TreasureMapLevel { get { return 4; } }
        public override Poison PoisonImmune { get { return Poison.Lethal; } }
        public override bool BreathImmune { get { return true; } }
        public override int DefaultBloodHue { get { return -1; } }

        public MagnetiteElemental(Serial serial) : base(serial)
        {}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt((int) 0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }

        public override void AlterMeleeDamageFrom(Mobile from, ref int damage)
        {
            int discordanceEffect = 0;

            if (from is PlayerMobile && Discordance.GetEffect(this, ref discordanceEffect))
            {
                var weapon = from.FindItemOnLayer(Layer.FirstValid) as BaseWeapon;

                if (weapon == null)
                {
                    weapon = from.FindItemOnLayer(Layer.TwoHanded) as BaseWeapon;
                }

                if (weapon != null && CraftResources.GetType(weapon.Resource) == CraftResourceType.Metal)
                {
                    damage /= 2;
                }
                else //Wooden Weapons = garbage?
                {
                    damage = 0;
                }
            }
            else
            {
                damage = 0;
            }
        }

/*
		public override void AlterSpellDamageFrom( Mobile from, ref int damage )
		{
			damage = 0;
		}
*/
/*
		public override void AlterAbilityDamageFrom( Mobile from, ref int damage )
		{
			AlterMeleeDamageFrom( from, ref damage );
		}
*/

        public override void OnGotMeleeAttack(Mobile attacker)
        {
            FixedParticles(0x376A, 20, 10, 0x2530, EffectLayer.Waist);
            PlaySound(0x2F4);

            int discordanceEffect = 0;

            if (attacker is PlayerMobile && Discordance.GetEffect(this, ref discordanceEffect))
            {
                attacker.SendAsciiMessage("The creature's magnetic field is weak.");
            }
            else if (attacker != null)
            {
                attacker.SendAsciiMessage("The creature's magnetic field repels your attack.");

                if (attacker.AccessLevel == AccessLevel.Player && attacker.InRange(Location, 2) &&
                    0.15 > Utility.RandomDouble())
                {
                    var items = new List<Item>();

                    for (int i = 0; i < attacker.Items.Count; i++)
                    {
                        Item item = attacker.Items[i];

                        if (item.Movable && item.LootType != LootType.Blessed && item.LootType != LootType.Newbied &&
                            item.BlessedFor == null)
                        {
                            var resource = CraftResource.None;

                            if (item is BaseWeapon)
                            {
                                resource = ((BaseWeapon) item).Resource;
                            }
                            else if (item is BaseArmor)
                            {
                                resource = ((BaseArmor) item).Resource;
                            }
                            else if (item is BaseJewel)
                            {
                                resource = CraftResource.Iron;
                            }
                            else if (item is BaseClothing)
                            {
                                resource = ((BaseClothing) item).Resource;
                            }
                            else
                            {
                                continue;
                            }

                            if (CraftResources.GetType(resource) == CraftResourceType.Metal)
                            {
                                items.Add(item);
                            }
                        }
                    }

                    if (items.Count > 0)
                    {
                        Item todrop = items[Utility.Random(items.Count)];

                        if (todrop is IDurability)
                        {
                            var dura = (IDurability) todrop;
                            if (dura.MaxHitPoints > 0) //It is not invulnerable
                            {
                                int maxpts = dura.MaxHitPoints / 10;
                                int points = Math.Min(maxpts, dura.HitPoints);

                                dura.HitPoints -= points;
                                if (dura.HitPoints == 0)
                                {
                                    dura.MaxHitPoints -= maxpts - points;
                                }

                                if (dura.MaxHitPoints <= 0)
                                {
                                    attacker.SendMessage("The creature's magnetic field destroyed your {0}.",
                                        todrop.GetDisplayName(attacker.NetState, false));
                                    todrop.Delete();
                                }
                                else
                                {
                                    attacker.SendMessage(
                                        "The creature's magnetic field attracted your {0}, and damaged it in the process.",
                                        todrop.GetDisplayName(attacker.NetState, false));
                                    todrop.MoveToWorld(Location, Map);
                                }
                            }
                        }
                        else
                        {
                            attacker.SendMessage("The creature's magnetic field attracted your {0}.",
                                todrop.GetDisplayName(attacker.NetState, false));
                            todrop.MoveToWorld(Location, Map);
                        }

                        FixedParticles(0, 10, 0, 0x2530, EffectLayer.Waist);
                    }
                }
            }

            base.OnGotMeleeAttack(attacker);
        }

        public override void CheckReflect(Mobile caster, ref bool reflect)
        {
            reflect = true; // Every spell is reflected back to the caster
        }

        public override bool Move(Direction d)
        {
            bool move = base.Move(d);

            if (move && Combatant != null)
            {
                FixedParticles(0, 10, 0, 0x2530, EffectLayer.Waist);
            }

            return move;
        }
    }
}