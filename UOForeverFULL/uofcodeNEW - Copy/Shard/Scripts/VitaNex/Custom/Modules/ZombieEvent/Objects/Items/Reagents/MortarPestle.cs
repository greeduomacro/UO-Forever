#region References

using System;
using System.Collections.Generic;
using Server.Targeting;
using VitaNex.Targets;

#endregion

namespace Server.Items
{
    public class MortarPestleZombie : Item
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Completed { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool ContainsVitriol { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool ContainsTentacles { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool ContainsFlesh { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool ContainsWings { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool WrongStep { get; set; }

        public List<Type> Contains { get; set; }

        [Constructable]
        public MortarPestleZombie() : base(0xE9B)
        {
            Name = "a mortar and pestel";
            Weight = 1.0;
            Contains = new List<Type>();
        }

        public MortarPestleZombie(Serial serial)
            : base(serial)
        {}

        public override void OnDoubleClick(Mobile from)
        {
            from.SendMessage(61, "Target the item you would like to use this on.");
            from.Target = new ItemSelectTarget<Item>((m, t) => DetermineTarget(from, t), m => { }, -1, true,
                TargetFlags.None);
            base.OnDoubleClick(@from);
        }

        public void DetermineTarget(Mobile pm, Item target)
        {
            if (Contains.Exists(x => x == target.GetType()))
            {
                pm.SendMessage(61, "The mortar already contains this ingredient.");
                return;
            }
            if (target is VileTentacles)
            {
                var tentacles = target as VileTentacles;
                if (tentacles.ChoppedUp)
                {
                    ContainsTentacles = true;
                    pm.SendMessage(54, "You place the chopped tentacles into the mortar and grind them up.");
                }
                else
                {
                    WrongStep = true;
                    pm.SendMessage(54, "You place the tentacles into the mortar and grind them up.");
                }
                Contains.Add(typeof(VileTentacles));
                target.Consume();
                pm.PlaySound(0x242);
            }

            else if (target is UndyingFlesh)
            {
                var flesh = target as UndyingFlesh;
                if (flesh.ChoppedUp)
                {
                    ContainsFlesh = true;
                    pm.SendMessage(54, "You place the diced flesh into the mortar and grind it up.");
                }
                else
                {
                    WrongStep = true;
                    pm.SendMessage(54, "You place the undying flesh into the mortar and grind it up.");
                }
                Contains.Add(typeof(UndyingFlesh));
                target.Consume();
                pm.PlaySound(0x242);
            }

            else if (target is FeyWings)
            {
                pm.SendMessage(54,
                    "You place fey wings into the mortar and grind them up into a fine powder.");
                target.Consume();
                ContainsWings = true;
                Contains.Add(typeof(FeyWings));
                pm.PlaySound(0x242);
            }

            else if (target is VialofVitriol)
            {
                pm.SendMessage(54,
                    "You dump the vial of vitriol into the mortar.");
                target.Consume();
                ContainsVitriol = true;
                Contains.Add(typeof(VialofVitriol));
                pm.PlaySound(0x242);
            }
            else if (target is DaemonClaw)
            {
                pm.SendMessage(54,
                    "You rip off one of the daemon claws fingers and place it into the mortar.");
                target.Consume();
                WrongStep = true;
                Contains.Add(typeof(DaemonClaw));
                pm.PlaySound(0x242);
            }
            else if (target is SeedofRenewal)
            {
                pm.SendMessage(54,
                    "You place the seed of renewal into the mortar and grind it up.");
                target.Consume();
                WrongStep = true;
                Contains.Add(typeof(SeedofRenewal));
                pm.PlaySound(0x242);
            }
            else if (target is SpiderCarapace)
            {
                pm.SendMessage(54,
                    "You rip off of the spider carapace and place it into the mortar.");
                target.Consume();
                WrongStep = true;
                Contains.Add(typeof(SpiderCarapace));
                pm.PlaySound(0x242);
            }
            else if (CookableFood.IsHeatSource(target))
            {
                if (ContainsFlesh && ContainsTentacles && ContainsVitriol && ContainsWings)
                {
                    pm.SendMessage(61,
                        "You place the mortar over the heat source and the ingredients inside begin to bubble and fizzle gently.");
                    Hue = 61;
                    Completed = true;
                }
                else if (Contains != null && Contains.Count > 0)
                {
                    Effects.SendLocationEffect(pm, pm.Map, 0x36B0, 25, 1);
                    Effects.PlaySound(pm, pm.Map, Utility.RandomList(0x11B, 0x11C, 0x11D));
                    pm.SendMessage(61,
                        "You place the mortar over the heat source and the ingredients begin to bubble violently.  Suddenly, the mortar explodes in your hands!");
                    Consume();
                    pm.Damage(90);
                }
                else
                {
                    pm.SendMessage(61,
                      "There is nothing in the mortar to heat up.");
                }
            }
            else if (target is PewterBowlZombie)
            {
                var bowl = target as PewterBowlZombie;
                pm.SendMessage(61, "You dump the concoction into the pewter bowl.");
                if (WrongStep || !Completed)
                {
                    bowl.WrongStep = true;
                }
                Consume();
                bowl.ContainsConcoction = true;
                pm.PlaySound(0x240);
            }
            else
            {
                pm.SendMessage(61, "That doesn't look like it should go in the mortar.");
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int) 0); // version
            writer.Write(Completed);
            writer.Write(WrongStep);
            writer.Write(ContainsFlesh);
            writer.Write(ContainsTentacles);
            writer.Write(ContainsVitriol);
            writer.Write(ContainsWings);

            writer.Write(Contains.Count);

            if (Contains.Count > 0)
            {
                foreach (var item in Contains)
                {
                    writer.WriteType(item);
                }
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            Contains = new List<Type>();
            base.Deserialize(reader);

            int version = reader.ReadInt();
            Completed = reader.ReadBool();
            WrongStep = reader.ReadBool();
            ContainsFlesh = reader.ReadBool();
            ContainsTentacles = reader.ReadBool();
            ContainsVitriol = reader.ReadBool();
            ContainsWings = reader.ReadBool();

            var count = reader.ReadInt();

            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    var type = reader.ReadType();
                    Contains.Add(type);
                }
            }
        }
    }
}