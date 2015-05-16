// **********
// RunUO Shard - [ServUO.com]-Retouching Tool.cs
// **********

#region References

using Server.Mobiles;
using Server.Targeting;

#endregion

namespace Server.Items
{
    public class RetouchingTool : Item
    {
        [Constructable]
        public RetouchingTool()
            : base(0x42C6)
        {
            Weight = 1.0;
            Name = "Retouching Tool";
            Hue = 0;
            LootType = LootType.Blessed;
            Stackable = false;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendMessage("You must have the Retouching Tool in your backpack!");
            }
            from.BeginTarget(2, false, TargetFlags.None, new TargetCallback(OnTarget));
        }

        protected void OnTarget(Mobile from, object targeted)
        {
            if (targeted is Item)
            {
                Item item = (Item) targeted;
                if ((item is EtherealMount))
                {
                    if (!item.IsChildOf(from.Backpack))
                        from.SendMessage("The Ethereal Mount must be in your backpack.");

                    else if (item.Hue == 0)
                    {
                        item.Hue = -1;
                        //item.Hue = Hue;
                    }
                    else if (item.Hue == -1)
                    {
                        item.Hue = 0;
                    }
                }
            }
            else
            {
                from.SendMessage("You cannot dye that.");
            }
        }

        public RetouchingTool(Serial serial)
            : base(serial)
        {
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