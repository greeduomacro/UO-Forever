#region References

using Server.Targeting;
using VitaNex.Targets;

#endregion

namespace Server.Items
{
    public class ZombieEventKnife : Item
    {
        [Constructable]
        public ZombieEventKnife()
            : base(2550)
        {
            Name = "a knife";
            Hue = 61;
            Weight = 1.0;
        }

        public ZombieEventKnife(Serial serial)
            : base(serial)
        {}

        public override void OnDoubleClick(Mobile from)
        {
                from.SendMessage(61, "Target the reagent that you would like to chop up.");
                from.Target = new ItemSelectTarget<Item>((m, t) => DetermineTarget(from, t), m => { }, -1, true,
                    TargetFlags.None);
            base.OnDoubleClick(@from);
        }

        public void DetermineTarget(Mobile pm, Item target)
        {
            if (target is VileTentacles)
            {
                var tentacles = target as VileTentacles;
                tentacles.ChoppedUp = true;
                tentacles.Hue = 38;
                tentacles.Name = "cut-up vile tentacles";
                pm.SendMessage(61, "You chop up the vile tentacles into small pieces.");
            }
            else if (target is UndyingFlesh)
            {
                var flesh = target as UndyingFlesh;
                flesh.ChoppedUp = true;
                flesh.Hue = 38;
                flesh.Name = "cut-up undying flesh";
                pm.SendMessage(61, "You slice the undying flesh into small strips.");
            }
            else if (target is FeyWings)
            {
                var wings = target as FeyWings;
                pm.SendMessage(61, "As you cut into the fey wings, they disintegrate in your hands and fall through your fingers onto the ground.");
                wings.Consume();
            }
            else if (target is DaemonClaw)
            {
                var claw = target as DaemonClaw;
                pm.SendMessage(61, "You hack away at the daemon claw with your knife but it doesn't seem to help.  The daemon claw appears to be ruined now.");
                claw.Consume();
            }
            else if (target is SpiderCarapace)
            {
                var spider = target as SpiderCarapace;
                pm.Poison = Poison.Lethal;
                pm.Damage(10);
                pm.SendMessage(61, "You slice into the spider carapace and a bunch of small spiders scurry out of it and bite you.");
                spider.Consume();
            }
            else if (target is SeedofRenewal)
            {
                var seed = target as SeedofRenewal;
                pm.SendMessage(61, "As you slice into the spongy seed, a green wisp of smoke errupts from it.  Whatever made this seed special is now gone.");
                seed.Consume();
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int) 0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}