#region References

using System.Linq;
using Server.Network;

#endregion

namespace Server.Items
{
    public class EasterHolyFountain : Item
    {
        [Constructable]
        public EasterHolyFountain()
            : base(17294)
        {
            Name = "a holy easter fountain";
            Weight = 2;
            Movable = false;
            DoesNotDecay = true;
        }

        public EasterHolyFountain(Serial serial)
            : base(serial)
        {}

        public override void OnSingleClick(Mobile m)
        {
            base.OnSingleClick(m);
            LabelTo(m,
                "This fountain is a relic from times long past.  You feel a strong holy presence when you touch it.",
                2049);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.Backpack == null)
                return;
            Item eggs = from.Backpack.FindItemByType(typeof(EasterEggsCorrupted));
            if (eggs != null)
            {
                Effects.SendIndividualFlashEffect(from, (FlashType) 2);

                foreach (Item egg in from.Backpack.Items.Where(i => i is EasterEggsCorrupted).ToArray())
                {
                    EasterEggsPurified newegg = egg.Amount > 0
                        ? new EasterEggsPurified {Amount = egg.Amount}
                        : new EasterEggsPurified();
                    from.Backpack.DropItem(newegg);
                    egg.Delete();
                }
                from.SendMessage(2049,
                    "As you place the eggs in the fountain, you are blinded by a brilliant flash of light as the eggs are cleansed of the Wretched Rabbit's corruption.");
            }
            else
            {
                from.SendMessage(54,
                    "You did not have any corrupted eggs in your pack that are in need of purification.");
            }
            base.OnDoubleClick(from);
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