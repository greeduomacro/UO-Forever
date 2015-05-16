#region References

using System;
using System.Collections.Generic;
using Server.Targeting;
using VitaNex.Targets;

#endregion

namespace Server.Items
{
    public class PewterBowlZombie : Item
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Completed { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool WrongStep { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool ContainsConcoction { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool ContainsCarapace { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool ContainersClaw { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool ContainsSeed { get; set; }

        public List<Type> Contains { get; set; }

        [Constructable]
        public PewterBowlZombie() : base(5629)
        {
            Name = "an empty pewter bowl";
            Weight = 1.0;
            Contains = new List<Type>();
        }

        public PewterBowlZombie(Serial serial)
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
                pm.SendMessage(61, "The pewter bowl already contains this ingredient.");
                return;
            }

            if (target is VileTentacles)
            {
                pm.SendMessage(54, "You place the tentacles into the pewter bowl.");
                WrongStep = true;
                target.Consume();
                Contains.Add(typeof(VileTentacles));
            }

            else if (target is UndyingFlesh)
            {
                WrongStep = true;
                pm.SendMessage(54, "You place the undying flesh into the pewter bowl.");
                target.Consume();
                Contains.Add(typeof(UndyingFlesh));
            }

            else if (target is FeyWings)
            {
                WrongStep = true;
                pm.SendMessage(54, "You place the fey wings into the pewter bowl.");
                target.Consume();
                Contains.Add(typeof(FeyWings));
            }

            else if (target is VialofVitriol)
            {
                WrongStep = true;
                pm.SendMessage(54, "You dump the vial of vitriol in the pewter bowl.");
                target.Consume();
                Contains.Add(typeof(VialofVitriol));
            }
            else if (target is SpiderCarapace)
            {
                if (!ContainsCarapace)
                {
                    ContainsCarapace = true;
                    pm.SendMessage(54, "You rip of a piece of the spider carapace off and place it in the pewter bowl.");
                    target.Consume();
                    Contains.Add(typeof(SpiderCarapace));
                }
            }
            else if (target is SeedofRenewal)
            {
                if (!ContainsSeed)
                {
                    ContainsSeed = true;
                    pm.SendMessage(54, "You gently place the seed of renewal in the pewter bowl.");
                    target.Consume();
                    Contains.Add(typeof(SeedofRenewal));
                }
            }
            else if (target is DaemonClaw)
            {
                if (!ContainersClaw)
                {
                    ContainersClaw = true;
                    pm.SendMessage(54, "You place the daemon claw in the pewter bowl.");
                    target.Consume();
                    Contains.Add(typeof(DaemonClaw));
                }
            }
            else if (target is CrystalFlask)
            {
                if (Contains != null && Contains.Count > 0)
                {
                    var vial = target as CrystalFlask;
                    pm.SendMessage(61, "You pour the contents of the pewter bowl into the crystal flask.");
                    target.Name = "the cure";
                    if (Completed)
                    {
                        vial.Hue = 61;
                    }
                    else
                    {
                        vial.Bad = true;
                        vial.Hue = 1157;
                    }
                    vial.Filled = true;
                    Consume();
                }
                else
                {
                    pm.SendMessage(61, "There is nothing in the pewter bowl.");
                }
            }
            else
            {
                pm.SendMessage(61, "That doesn't look like it should go in the pewter bowl.");
            }

            if (Contains != null && Contains.Count > 0)
            {
                ItemID = 5633;
                Name = "a bowl of reagents";
            }

            if (ContainersClaw && ContainsCarapace && ContainsSeed && ContainsConcoction && !WrongStep)
            {
                Completed = true;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int) 0); // version
            writer.Write(Completed);
            writer.Write(WrongStep);
            writer.Write(ContainsSeed);
            writer.Write(ContainersClaw);
            writer.Write(ContainsCarapace);
            writer.Write(ContainsConcoction);

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
            ContainsSeed = reader.ReadBool();
            ContainersClaw = reader.ReadBool();
            ContainsCarapace = reader.ReadBool();
            ContainsConcoction = reader.ReadBool();

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